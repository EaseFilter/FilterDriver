///////////////////////////////////////////////////////////////////////////////
//
//    (C) Copyright 2011 EaseFilter Technologies
//    All Rights Reserved
//
//    This software is part of a licensed software product and may
//    only be used or copied in accordance with the terms of that license.
//
//    NOTE:  THIS MODULE IS UNSUPPORTED SAMPLE CODE
//
//    This module contains sample code provided for convenience and
//    demonstration purposes only,this software is provided on an 
//    "AS-IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, 
//     either express or implied.  
//
///////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Microsoft.Win32.SafeHandles;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Threading;
using System.Diagnostics;
using System.Management;
using System.Collections;
using System.Configuration;
using System.Windows.Forms;

using EaseFilter.CommonObjects;

namespace EaseFilter.FolderLocker
{

    public class CacheUserAccessInfo
    {
        public string index = string.Empty;
        public bool accessStatus = false;
        public bool isDownloaded = false;
        public uint accessFlags = 0;
        public DateTime lastAccessTime = DateTime.MinValue;
        public string iv = string.Empty;
        public string key = string.Empty;
        public AutoResetEvent syncEvent = new AutoResetEvent(true);
        public string lastError = string.Empty;

    }

    public class DRServer
    {

        static Dictionary<string, CacheUserAccessInfo> userAccessCache = new Dictionary<string, CacheUserAccessInfo>();
        static int cacheTimeOutInSeconds = 20;
        static System.Timers.Timer deleteCachedItemTimer = new System.Timers.Timer();

        static DRServer()
        {
            deleteCachedItemTimer.Interval = cacheTimeOutInSeconds * 1000; //millisecond
            deleteCachedItemTimer.Start();
            deleteCachedItemTimer.Enabled = true;
            deleteCachedItemTimer.Elapsed += new System.Timers.ElapsedEventHandler(deleteCachedItemTimer_Elapsed);
        }

        static public bool GetFileAccessPermission(ref FilterAPI.MessageSendData messageSend, ref FilterAPI.MessageReplyData messageReply)
        {
            Boolean retVal = true;
            string fileName = messageSend.FileName;
            string lastError = string.Empty;
            string processName = string.Empty;
            string userName = string.Empty;
            string encryptKey = string.Empty;


            try
            {

                FilterAPI.DecodeProcessName(messageSend.ProcessId, out processName);
                FilterAPI.DecodeUserName(messageSend.Sid, out userName);

                //by default the tag data format is "accountName;ivStr"

                int tagDataLength = (int)messageSend.DataBufferLength;
                byte[] tagData = messageSend.DataBuffer;
                Array.Resize(ref tagData, tagDataLength);

                string tagStr = UnicodeEncoding.Unicode.GetString(tagData);

                int index = tagStr.IndexOf(";");
                byte[] iv = tagData;

                if (index > 0)
                {
                    string serverAccount = tagStr.Substring(0, index);
                    string ivStr = tagStr.Substring(index + 1);
                    iv = Utils.ConvertHexStrToByteArray(ivStr);
                }

                uint accessFlag = 0;

                retVal = GetAccessPermissionFromServer(fileName,
                                                userName,
                                                processName,
                                                tagStr,
                                                ref encryptKey,
                                                ref accessFlag,
                                                ref lastError);

                if (retVal && !string.IsNullOrEmpty(encryptKey))
                {
                    byte[] keyArray = Utils.ConvertHexStrToByteArray(encryptKey);

                    //write the iv and key to the reply data buffer with format FilterAPI.AESDataBuffer
                    AESDataBuffer aesData = new AESDataBuffer();
                    if (messageSend.MessageType == (uint)FilterAPI.FilterCommand.FILTER_REQUEST_ENCRYPTION_IV_AND_KEY_AND_ACCESSFLAG)
                    {
                        aesData.AccessFlags = FilterAPI.ALLOW_MAX_RIGHT_ACCESS;
                    }
                    else
                    {
                        aesData.AccessFlags = FilterAPI.ALLOW_MAX_RIGHT_ACCESS;
                    }
                    aesData.IV = iv;
                    aesData.IVLength = (uint)iv.Length;
                    aesData.EncryptionKey = keyArray;
                    aesData.EncryptionKeyLength = (uint)keyArray.Length;

                    byte[] aesDataArray = DigitalRightControl.ConvertAESDataToByteArray(aesData);
                    messageReply.DataBufferLength = (uint)aesDataArray.Length;
                    Array.Copy(aesDataArray, messageReply.DataBuffer, aesDataArray.Length);

                    messageReply.ReturnStatus = (uint)FilterAPI.NTSTATUS.STATUS_SUCCESS;

                }
            }
            catch (Exception ex)
            {
                lastError = "GetFileAccessPermission exception." + ex.Message;
                EventManager.WriteMessage(340, "GetFileAccessPermission", EventLevel.Error, lastError);
                retVal = false;
            }

            if (!retVal)
            {
                byte[] errorBuffer = UnicodeEncoding.Unicode.GetBytes(lastError);
                Array.Copy(errorBuffer, messageSend.DataBuffer, errorBuffer.Length);

                messageSend.DataBufferLength = (uint)errorBuffer.Length;
            }


            return retVal;

        }


        static private bool GetAccessPermissionFromServer(string fileName,
                                                            string userName,
                                                            string processName,
                                                            string tagStr,
                                                            ref string encryptKey,
                                                            ref uint accessFlag,
                                                            ref string lastError)
        {
            Boolean retVal = true;

            try
            {
                CacheUserAccessInfo cacheUserAccessInfo = new CacheUserAccessInfo();

                string index = userName + "_" + processName + "_" + tagStr;

                //cache the same user/process/filename access.
                lock (userAccessCache)
                {
                    if (userAccessCache.ContainsKey(index))
                    {
                        cacheUserAccessInfo = userAccessCache[index];
                        EventManager.WriteMessage(446, "GetUserPermission", EventLevel.Verbose, "Thread" + Thread.CurrentThread.ManagedThreadId + ",userInfoKey " + index + " exists in the cache table.");
                    }
                    else
                    {
                        cacheUserAccessInfo.isDownloaded = false;
                        cacheUserAccessInfo.index = index;
                        cacheUserAccessInfo.lastAccessTime = DateTime.Now;
                        userAccessCache.Add(index, cacheUserAccessInfo);
                        EventManager.WriteMessage(435, "GetUserPermission", EventLevel.Verbose, "Thread" + Thread.CurrentThread.ManagedThreadId + ",add userInfoKey " + index + " to the cache table.");
                    }
                }

                //synchronize the same file access.
                if (!cacheUserAccessInfo.isDownloaded && !cacheUserAccessInfo.syncEvent.WaitOne(new TimeSpan(0, 0, cacheTimeOutInSeconds)))
                {
                    string info = "User name: " + userName + ",processname:" + processName + ",file name:" + fileName + " wait for permission timeout.";
                    EventManager.WriteMessage(402, "GetUserPermission", EventLevel.Warning, info);
                    return false;
                }

                TimeSpan timeSpan = DateTime.Now - cacheUserAccessInfo.lastAccessTime;

                if (cacheUserAccessInfo.isDownloaded && timeSpan.TotalSeconds < cacheTimeOutInSeconds)
                {
                    //the access was cached, return the last access status.
                    retVal = cacheUserAccessInfo.accessStatus;

                    if (!retVal)
                    {
                        EventManager.WriteMessage(308, "GetAccessPermissionFromServer", EventLevel.Error, cacheUserAccessInfo.lastError);
                    }
                    else
                    {
                        string info = "thread" + Thread.CurrentThread.ManagedThreadId + ",  Cached userInfoKey " + index + " in the cache table,return " + retVal;
                        EventManager.WriteMessage(451, "GetUserPermission", EventLevel.Verbose, info);
                    }

                    encryptKey = cacheUserAccessInfo.key;
                    accessFlag = cacheUserAccessInfo.accessFlags;
                    lastError = cacheUserAccessInfo.lastError;

                    cacheUserAccessInfo.syncEvent.Set();

                    return retVal;
                }

                string encryptionIV = tagStr;

                retVal = WebAPIServices.GetSharedFilePermission(fileName, processName, userName, tagStr, ref encryptionIV, ref encryptKey, ref accessFlag, ref lastError);
                cacheUserAccessInfo.accessStatus = retVal;
                cacheUserAccessInfo.isDownloaded = true;
                cacheUserAccessInfo.syncEvent.Set();

                if (!retVal)
                {
                    string message = "Get file " + fileName + " permission from server return error:" + lastError;
                    cacheUserAccessInfo.lastError = message;
                    cacheUserAccessInfo.accessStatus = false;

                    EventManager.WriteMessage(293, "GetAccessPermissionFromServer", EventLevel.Error, message);

                    return retVal;
                }
                else
                {
                    string message = "Get file " + fileName + " permission frome server return succeed.";
                    EventManager.WriteMessage(208, "GetAccessPermissionFromServer", EventLevel.Verbose, message);
                }

                cacheUserAccessInfo.key = encryptKey;
                cacheUserAccessInfo.iv = encryptionIV;
                cacheUserAccessInfo.accessFlags = accessFlag;


            }
            catch (Exception ex)
            {
                EventManager.WriteMessage(286, "GetAccessPermissionFromServer", EventLevel.Error, "Get file " + fileName + "permission failed with exception:" + ex.Message);
                retVal = false;
            }

            return retVal;

        }



        static private bool IsFileAccessAuthorized(string fileName,
                                          string userName,
                                          string processName,
                                          string tagStr,
                                          ref string encryptKey,
                                          ref uint accessFlag,
                                          ref string lastError)
        {
            Boolean retVal = true;
            string expireTime = string.Empty;
            string authorizedProcessNames = string.Empty;
            string unauthorizedProcessNames = string.Empty;
            string authorizedUserNames = string.Empty;
            string unauthorizedUserNames = string.Empty;
            string accessFlags = string.Empty;

            lastError = string.Empty;
            DateTime currentTime = DateTime.Now;

            try
            {
                if (GlobalConfig.StoreSharedFileMetaDataInServer)
                {
                    return GetAccessPermissionFromServer(fileName, userName, processName, tagStr, ref encryptKey, ref accessFlag, ref lastError);
                }

                string drFilePath = GlobalConfig.DRInfoFolder + "\\" + tagStr + ".xml";
                Dictionary<string, string> keyValues = new Dictionary<string, string>();

                if (!File.Exists(drFilePath))
                {
                    lastError = "The meta data file " + drFilePath + " doesn't exist.";
                    return false;
                }

                Utils.LoadAppSetting(drFilePath, ref keyValues);

                encryptKey = string.Empty;
                keyValues.TryGetValue("key", out encryptKey);
                if (string.IsNullOrEmpty(encryptKey))
                {
                    lastError = "The encryption key is empty.";
                    return false;
                }

                keyValues.TryGetValue("accessFlags", out accessFlags);
                uint.TryParse(accessFlags, out accessFlag);
                keyValues.TryGetValue("expireTime", out expireTime);
                keyValues.TryGetValue("authorizedProcessNames", out authorizedProcessNames);
                keyValues.TryGetValue("unauthorizedProcessNames", out unauthorizedProcessNames);
                keyValues.TryGetValue("authorizedUserNames", out authorizedUserNames);
                keyValues.TryGetValue("unauthorizeUserNames", out unauthorizedUserNames);

                if (!string.IsNullOrEmpty(expireTime))
                {
                    DateTime exTime = DateTime.FromFileTime(long.Parse(expireTime));

                    if (currentTime > exTime)
                    {
                        lastError = "the file is expired,current time:" + currentTime.ToString("yyyy-MM-dd-HH-mm-ss") + ",expire time:" + exTime.ToString("yyyy-MM-dd-HH-mm-ss");
                        return false;
                    }
                }

                if (!string.IsNullOrEmpty(authorizedProcessNames))
                {
                    if (authorizedProcessNames.IndexOf(processName.ToLower().Trim()) < 0)
                    {
                        lastError = "the process " + processName + " is not in authorized process list.";
                        return false;
                    }
                }

                if (!string.IsNullOrEmpty(unauthorizedProcessNames))
                {
                    if (unauthorizedProcessNames.IndexOf(processName.ToLower().Trim()) >= 0)
                    {
                        lastError = "the process " + processName + " is in unauthorized process list.";
                        return false;
                    }
                }

                if (!string.IsNullOrEmpty(authorizedUserNames))
                {
                    if (authorizedUserNames.IndexOf(userName.ToLower().Trim()) < 0)
                    {
                        lastError = "the user " + userName + " is not in authorized user list.";
                        return false;
                    }
                }

                if (!string.IsNullOrEmpty(unauthorizedUserNames))
                {
                    if (unauthorizedUserNames.IndexOf(userName.ToLower().Trim()) >= 0)
                    {
                        lastError = "the user " + userName + " is in unauthorized user list.";
                        return false;
                    }
                }


            }
            catch (Exception ex)
            {
                EventManager.WriteMessage(286, "IsFileAccessAuthorized", EventLevel.Error, "Get file " + fileName + "permission failed with exception:" + ex.Message);
                retVal = false;
            }

            return retVal;

        }

        static public bool AddDRInfoToFile(string fileName,
                                    string authorizedProcessNames,
                                    string unauthorizedProcessNames,
                                    string authorizedUserNames,
                                    string unauthorizedUserNames,
                                    DateTime expireTime,
                                    byte[] encryptIV,
                                    byte[] encryptKey,
                                    string accessFlags)
        {

            try
            {
                if (!Directory.Exists(GlobalConfig.DRInfoFolder))
                {
                    Directory.CreateDirectory(GlobalConfig.DRInfoFolder);
                }

                string iv = Utils.ByteArrayToHexStr(encryptIV);
                string key = Utils.ByteArrayToHexStr(encryptKey);

                string drFilePath = GlobalConfig.DRInfoFolder + "\\" + iv + ".xml";
                Dictionary<string, string> keyValues = new Dictionary<string, string>();

                keyValues.Add("fileName", fileName);
                keyValues.Add("key", key);
                keyValues.Add("iv", iv);
                keyValues.Add("accessFlags", accessFlags);
                keyValues.Add("authorizedProcessNames", authorizedProcessNames.ToLower());
                keyValues.Add("unauthorizedProcessNames", unauthorizedProcessNames.ToLower());
                keyValues.Add("authorizedUserNames", authorizedUserNames.ToLower());
                keyValues.Add("unauthorizedUserNames", unauthorizedUserNames.ToLower());
                keyValues.Add("expireTime", expireTime.ToFileTime().ToString());
                keyValues.Add("creationTime", DateTime.Now.ToFileTime().ToString());

                Utils.SaveAppSetting(drFilePath, keyValues);

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }

        static private void deleteCachedItemTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {

            try
            {
                List<string> keysToRemove = new List<string>();

                foreach (KeyValuePair<string, CacheUserAccessInfo> userItem in userAccessCache)
                {

                    TimeSpan tsSinceLastAccess = DateTime.Now - userItem.Value.lastAccessTime;

                    if (tsSinceLastAccess.TotalSeconds >= cacheTimeOutInSeconds)
                    {
                        keysToRemove.Add(userItem.Key);
                    }
                }

                foreach (string key in keysToRemove)
                {
                    lock (userAccessCache)
                    {
                        userAccessCache.Remove(key);

                        EventManager.WriteMessage(573, "deleteCachedItemTimer_Elapsed", EventLevel.Verbose, "Delete cached item " + key);
                    }
                }
            }
            catch (System.Exception ex)
            {
                EventManager.WriteMessage(46, "deleteCachedItemTimer_Elapsed", EventLevel.Error, "Delete cached item failed with error:" + ex.Message);
            }

        }

    }
}
