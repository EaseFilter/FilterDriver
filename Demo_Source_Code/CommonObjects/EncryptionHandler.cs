///////////////////////////////////////////////////////////////////////////////
//
//    (C) Copyright 2011 EaseFilter Technologies Inc.
//    All Rights Reserved
//
//    This software is part of a licensed software product and may
//    only be used or copied in accordance with the terms of that license.
//
///////////////////////////////////////////////////////////////////////////////

using System;
using System.Text;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Win32.SafeHandles;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Security.Cryptography;

namespace EaseFilter.CommonObjects
{


    public enum AESFlags : uint
    {
        Flags_Enabled_Expire_Time = 0x00000010,
        Flags_Enabled_Check_ProcessName = 0x00000020,
        Flags_Enabled_Check_UserName = 0x00000040,
        Flags_Enabled_Check_AccessFlags = 0x00000080,
        Flags_Enabled_Check_User_Permit = 0x00000100,
        Flags_Enabled_Request_AES_KEY = 0x00000200,
        Flags_Enabled_Request_AES_IV_KEY = 0x00000400, 

    }


    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct AESAccessPolicy
    {
        public uint AESVerificationKey;
        public uint AESFlags;
        public uint IVLength;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public byte[] IV;
        public long ExpireTime;
        public uint AccessFlags;
        public long FileSize;
        public uint LengthOfIncludeProcessNames;
        public uint OffsetOfIncludeProcessNames;
        public uint LengthOfExcludeProcessNames;
        public uint OffsetOfExcludeProcessNames;
        public uint LengthOfIncludeUserNames;
        public uint OffsetOfIncludeUserNames;
        public uint LengthOfExcludeUserNames;
        public uint OffsetOfExcludeUserNames;
        public string IncludeProcessNames;
        public string ExcludeProcessNames;
        public string IncludeUserNames;
        public string ExcludeUserNames;
        public uint SizeOfAESData;

    }

    public class EncryptionHandler
    {
        public const uint AES_VERIFICATION_KEY = 0xccb76e80;
        public static string WorkingFolder = string.Empty;
        public static string PassPhrase = string.Empty;

        /// <summary>
        /// Create an encrypted file with embedded access control policy, distribute the encrypted file via internet, 
        /// only the authorized users and processes can access the encrypted file.
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="passPhrase"></param>
        /// <param name="policy"></param>
        /// <param name="lastError"></param>
        /// <returns></returns>
        public static bool EncryptFileWithEmbeddedPolicy(string fileName, string passPhrase, AESAccessPolicy policy, out string lastError)
        {
            bool ret = false;
            FileStream fs = null;
            lastError = string.Empty;

            try
            {
                if (!File.Exists(fileName))
                {
                    lastError = fileName + " doesn't exist.";
                    return false;
                }

                FileAttributes attributes = File.GetAttributes(fileName);
                attributes = (~FileAttributes.ReadOnly) & attributes;
                File.SetAttributes(fileName, attributes);

                byte[] encryptionKey = Utils.GetKeyByPassPhrase(passPhrase);
                byte[] iv = Utils.GetRandomIV();

                //encrypt the file with encryption key and a random iv key.
                ret = FilterAPI.AESEncryptFile(fileName, (uint)encryptionKey.Length, encryptionKey, (uint)iv.Length, iv, false);
                if (!ret)
                {
                    lastError = "Encrypt file " + fileName + " failed with error:" + FilterAPI.GetLastErrorMessage();
                    return ret;
                }

                fs = new FileStream(fileName, FileMode.Append, FileAccess.Write, FileShare.Read);
                long fileSize = fs.Length;

                MemoryStream ms = new MemoryStream();
                BinaryWriter bw = new BinaryWriter(ms);
                bw.Write(AES_VERIFICATION_KEY);
                bw.Write(policy.AESFlags);
                bw.Write(iv.Length);
                bw.Write(iv);
                bw.Write(policy.ExpireTime);

                bw.Write(policy.AccessFlags);
                bw.Write(fileSize);
                bw.Write(policy.LengthOfIncludeProcessNames);
                policy.OffsetOfIncludeProcessNames = (uint)ms.Length + 7 * 4;
                bw.Write(policy.OffsetOfIncludeProcessNames);
                bw.Write(policy.LengthOfExcludeProcessNames);
                policy.OffsetOfExcludeProcessNames = policy.OffsetOfIncludeProcessNames + policy.LengthOfIncludeProcessNames;
                bw.Write(policy.OffsetOfExcludeProcessNames);
                bw.Write(policy.LengthOfIncludeUserNames);
                policy.OffsetOfIncludeUserNames = policy.OffsetOfExcludeProcessNames + policy.LengthOfExcludeProcessNames;
                bw.Write(policy.OffsetOfIncludeUserNames);
                bw.Write(policy.LengthOfExcludeUserNames);
                policy.OffsetOfExcludeUserNames = policy.OffsetOfIncludeUserNames + policy.LengthOfIncludeUserNames;
                bw.Write(policy.OffsetOfExcludeUserNames);

                byte[] strBuffer;
                if (policy.LengthOfIncludeProcessNames > 0)
                {
                    strBuffer = UnicodeEncoding.Unicode.GetBytes(policy.IncludeProcessNames);
                    bw.Write(strBuffer);
                }

                if (policy.LengthOfExcludeProcessNames > 0)
                {
                    strBuffer = UnicodeEncoding.Unicode.GetBytes(policy.ExcludeProcessNames);
                    bw.Write(strBuffer);
                }

                if (policy.LengthOfIncludeUserNames > 0)
                {
                    strBuffer = UnicodeEncoding.Unicode.GetBytes(policy.IncludeUserNames);
                    bw.Write(strBuffer);
                }

                if (policy.LengthOfExcludeUserNames > 0)
                {
                    strBuffer = UnicodeEncoding.Unicode.GetBytes(policy.ExcludeUserNames);
                    bw.Write(strBuffer);
                }

                uint sizeOfAESData = (uint)ms.Length + 4;

                byte[] AESBuffer = ms.ToArray();

                //encrypt the access policy except the sizeOfAESData;
                FilterAPI.AESEncryptDecryptBuffer(AESBuffer, 0, encryptionKey, FilterAPI.DEFAULT_IV_TAG);

                //append the access policy to the encrypted file.
                fs.Write(AESBuffer, 0, AESBuffer.Length);
                fs.Write(BitConverter.GetBytes(sizeOfAESData), 0, 4);                

                //set the encrypted file to readonly here.
                attributes = File.GetAttributes(fileName) | FileAttributes.ReadOnly;
                File.SetAttributes(fileName, attributes);
            }
            catch (Exception ex)
            {
                ret = false;
                lastError = "EncryptFileAndEmbedExpireTime " + fileName + " failed with error:" + ex.Message;
            }
            finally
            {
                if (null != fs)
                {
                    fs.Close();
                }
            }

            return ret;
        }

        /// <summary>
        /// Process the encrypted file's embedded access policy, remove embedded information, add AESTagData to encrypted file, 
        /// Create a filter driver aware encrypted file. Then you can read the encrypted file transparently via filter driver encryption engine.
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="passPhrase"></param>
        /// <param name="lastError"></param>
        /// <returns></returns>
        public static bool ConvertFileToFilterDriverAwareEncryptFile(string fileName, string passPhrase, out string lastError)
        {
            bool ret = false;
            lastError = string.Empty;

            try
            {
                if (!File.Exists(fileName))
                {
                    lastError = fileName + " doesn't exist.";
                    return false;
                }

                FileAttributes attributes = File.GetAttributes(fileName);
                attributes = (~FileAttributes.ReadOnly) & attributes;
                File.SetAttributes(fileName, attributes);

                FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.ReadWrite, FileShare.Read);
                long fileSize = fs.Length;

                //read the last 4 bytes data, it is the total size of the embedded data.

                fs.Position = fileSize - 4;
                BinaryReader br = new BinaryReader(fs);
                uint sizeOfAESData = br.ReadUInt32();

                if (sizeOfAESData >= fileSize)
                {
                    lastError = fileName + " is not valid share encrypted file, the sizeOfAESData:" + sizeOfAESData + " >= file size:" + fileSize;
                    return false;
                }

                fs.Position = fileSize - sizeOfAESData;

                //Read the embedded data 
                byte[] AESBuffer = new byte[sizeOfAESData];
                fs.Read(AESBuffer, 0, (int)sizeOfAESData);

                //decrypt the embedded data, since the last 4 bytes is not encrypted, after decryption,need to write the clear size back.
                byte[] encryptionKey = Utils.GetKeyByPassPhrase(passPhrase);
                FilterAPI.AESEncryptDecryptBuffer(AESBuffer, 0, encryptionKey, FilterAPI.DEFAULT_IV_TAG);

                //since the last 4 bytes for sizeOfAESData is not encrypted, we need to put back the clear value back.
                MemoryStream ms = new MemoryStream(AESBuffer);
                ms.Position = 0;
                br = new BinaryReader(ms);
                uint verificationKey = br.ReadUInt32();

                //verify if this is the valid embedded data.
                if (verificationKey != AES_VERIFICATION_KEY)
                {
                    lastError = fileName + " is not valid share encrypted file, the encryption key:" + verificationKey + " is not valid.";
                    return false;
                }

                //write back the size of embedded data here.
                ms.Position = ms.Length - 4;
                BinaryWriter bw = new BinaryWriter(ms);
                bw.Write(sizeOfAESData);

                //Remove the embedded data, this is the original file size without the embedded information.
                fs.SetLength(fileSize - sizeOfAESData);

                fs.Close();
                fs = null;

                //add the embedded data to the tag data of the encrypted file.
                ret = FilterAPI.AddAESData(fileName, AESBuffer, out lastError);
                

            }
            catch (Exception ex)
            {
                ret = false;
                lastError = "EncryptFileAndEmbedExpireTime " + fileName + " failed with error:" + ex.Message;
            }


            return ret;
        }


        public static bool EncryptFileAndEmbedExpireTime(string fileName, string passPhrase, DateTime expireTimeUtc, out string lastError)
        {
            bool ret = false;
            lastError = string.Empty;

            try
            {
                if (!File.Exists(fileName))
                {
                    lastError = fileName + " doesn't exist.";
                    return false;
                }

                byte[] encryptionKey = Utils.GetKeyByPassPhrase(passPhrase);
                byte[] iv = Utils.GetRandomIV();

                ret = FilterAPI.AESEncryptFile(fileName, (uint)encryptionKey.Length, encryptionKey, (uint)iv.Length, iv, false);
                if (!ret)
                {
                    lastError = "Encrypt file " + fileName + " failed with error:" + FilterAPI.GetLastErrorMessage();
                    return ret;
                }

                FileStream fs = new FileStream(fileName, FileMode.Append, FileAccess.Write, FileShare.Read);
                      
                long fileSize = fs.Length;
                BinaryWriter bw = new BinaryWriter(fs);
                bw.Write(FilterAPI.MESSAGE_SEND_VERIFICATION_NUMBER);
                bw.Write(fileSize);
                bw.Write(iv);
                bw.Write(expireTimeUtc.ToFileTimeUtc());


                fs.Close();

                FileAttributes attributes = File.GetAttributes(fileName) | FileAttributes.ReadOnly;

                File.SetAttributes(fileName, attributes);
            }
            catch (Exception ex)
            {
                ret = false;
                lastError = "EncryptFileAndEmbedExpireTime " + fileName + " failed with error:" + ex.Message;
            }

            return ret;
        }

        public static bool ProcessEncryptedFile(string sourceFileName, string destFileName, out string lastError)
        {
            bool ret = false;
            lastError = string.Empty;

            try
            {

                ret = FilterAPI.ProcessEncryptedFile(sourceFileName, destFileName);
                if (!ret)
                {
                    lastError = "ProcessEncryptedFile " + sourceFileName + ", destFileName " + destFileName + " failed with error:" + FilterAPI.GetLastErrorMessage();
                    return ret;
                }

                File.SetAttributes(sourceFileName, FileAttributes.Normal);
                File.Delete(sourceFileName);
            }
            catch (Exception ex)
            {
                ret = false;
                lastError = "ProcessEncryptedFile " + sourceFileName + ", destFileName " + destFileName + " failed with error:" + ex.Message;
            }

            return ret;
        }
    }
}
