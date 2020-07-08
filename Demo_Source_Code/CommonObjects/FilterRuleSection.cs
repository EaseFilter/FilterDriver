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
using System.Linq;
using System.Text;
using System.Configuration;

namespace EaseFilter.CommonObjects
{
    public class FilterRuleSection : ConfigurationSection
    {
        [ConfigurationProperty("", IsRequired = true, IsDefaultCollection = true)]
        public FilterRuleCollection Instances
        {
            get { return (FilterRuleCollection)this[""]; }
            set { this[""] = value; }
        }
    }

    public class FilterRuleCollection : ConfigurationElementCollection
    {
        public FilterRule this[int index]
        {
            get { return (FilterRule)BaseGet(index); }
            set
            {
                if (BaseGet(index) != null)
                {
                    BaseRemoveAt(index);
                }
                BaseAdd(index, value);
            }
        }

        public void Add(FilterRule filterRule)
        {
            BaseAdd(filterRule);
        }

        public void Clear()
        {
            BaseClear();
        }

        public void Remove(FilterRule filterRule)
        {
            BaseRemove(filterRule.IncludeFileFilterMask);
        }

        public void RemoveAt(int index)
        {
            BaseRemoveAt(index);
        }

        public void Remove(string name)
        {
            BaseRemove(name);
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new FilterRule();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            //set to whatever Element Property you want to use for a key
            return ((FilterRule)element).IncludeFileFilterMask;
        }
    }

    public class FilterRule : ConfigurationElement
    {
        //A filter rule must have a unique include file filter mask, 
        //A filter rule can have multiple exclude file filter masks.
        //Make sure to set IsKey=true for property exposed as the GetElementKey above
        /// <summary>
        /// if the file path matches the includeFileFilterMask, the filter driver will process the registered IOs, or it will skip the IOs for this file.   
        /// </summary>
        [ConfigurationProperty("includeFileFilterMask", IsKey = true, IsRequired = true)]
        public string IncludeFileFilterMask
        {
            get { return (string)base["includeFileFilterMask"]; }
            set { base["includeFileFilterMask"] = value; }
        }

        /// <summary>
        /// If the file path matches the excludeFileFilterMasks, the filter driver will skip the IOs for this file.
        /// </summary>
        [ConfigurationProperty("excludeFileFilterMasks", IsRequired = false)]
        public string ExcludeFileFilterMasks
        {
            get { return (string)base["excludeFileFilterMasks"]; }
            set { base["excludeFileFilterMasks"] = value; }
        }

        /// <summary>
        /// if the file path matches the includeFileFilterMask and the includeProcessNames is not empty,
        /// only the IOs from the process name which matches the includeProcessNames will be managed by filter driver.
        /// </summary>
        [ConfigurationProperty("includeProcessNames", IsRequired = false)]
        public string IncludeProcessNames
        {
            get { return (string)base["includeProcessNames"]; }
            set { base["includeProcessNames"] = value; }
        }

        /// <summary>
        /// if the file path matches the includeFileFilterMask and the excludeProcessNames is not empty,
        /// the IOs from the process name which matches the excludeProcessNames will be skipped by filter driver.
        /// </summary>
        [ConfigurationProperty("excludeProcessNames", IsRequired = false)]
        public string ExcludeProcessNames
        {
            get { return (string)base["excludeProcessNames"]; }
            set { base["excludeProcessNames"] = value; }
        }

        /// <summary>
        /// if the file path matches the includeFileFilterMask and the includeUserNames is not empty,
        /// only the IOs from the user name which matches the includeUserNames will be managed by filter driver.
        /// </summary>
        [ConfigurationProperty("includeUserNames", IsRequired = false)]
        public string IncludeUserNames
        {
            get { return (string)base["includeUserNames"]; }
            set { base["includeUserNames"] = value; }
        }

        /// <summary>
        /// if the file path matches the includeFileFilterMask and the excludeUserNames is not empty,
        /// the IOs from the user name which matches the excludeUserNames will be skipped by filter driver.
        /// </summary>
        [ConfigurationProperty("excludeUserNames", IsRequired = false)]
        public string ExcludeUserNames
        {
            get { return (string)base["excludeUserNames"]; }
            set { base["excludeUserNames"] = value; }
        }

        /// <summary>
        /// if the file path matches the includeFileFilterMask and the includeProcessIds is not empty,
        /// only the IOs from the process Id which matches the includeProcessIds will be managed by filter driver.
        /// </summary>
        [ConfigurationProperty("includeProcessIds", IsRequired = false)]
        public string IncludeProcessIds
        {
            get { return (string)base["includeProcessIds"]; }
            set { base["includeProcessIds"] = value; }
        }

        /// <summary>
        /// if the file path matches the includeFileFilterMask and the excludeProcessIds is not empty,
        /// the IOs from the process Id which matches the excludeProcessIds will be skipped by filter driver.
        /// </summary>
        [ConfigurationProperty("excludeProcessIds", IsRequired = false)]
        public string ExcludeProcessIds
        {
            get { return (string)base["excludeProcessIds"]; }
            set { base["excludeProcessIds"] = value; }
        }

        /// <summary>
        /// if the file path matches hiddenFileFilterMasks, the file will be hidden 
        /// </summary>
        [ConfigurationProperty("hiddenFileFilterMasks", IsRequired = false)]
        public string HiddenFileFilterMasks
        {
            get { return (string)base["hiddenFileFilterMasks"]; }
            set { base["hiddenFileFilterMasks"] = value; }
        }

        /// <summary>
        /// if the reparse file was enabled, the open to the file path will be reparsed to the new path replaced by reparseFileFilterMasks
        /// </summary>
        [ConfigurationProperty("reparseFileFilterMasks", IsRequired = false)]
        public string ReparseFileFilterMasks
        {
            get { return (string)base["reparseFileFilterMasks"]; }
            set { base["reparseFileFilterMasks"] = value; }
        }

        /// <summary>
        /// The file access rights for the user name list, seperated with ";" for multiple users
        /// the format is "userName!accessFlags;", e.g. "mydomain\user1!123456;"
        /// </summary>
        [ConfigurationProperty("userRights", IsRequired = false)]
        public string UserRights
        {
            get { return (string)base["userRights"]; }
            set { base["userRights"] = value; }
        }

        /// <summary>
        /// The file access rights inside the filter rule for the process name list, seperated with ";" for multiple processes
        /// the format is "processName!accessFlags;", e.g. "notepad.exe!123456;"
        /// </summary>
        [ConfigurationProperty("processRights", IsRequired = false)]
        public string ProcessRights
        {
            get { return (string)base["processRights"]; }
            set { base["processRights"] = value; }
        }

        /// <summary>
        /// The file access rights inside the filter rule for the process Id list, seperated with ";" for multiple processes
        /// the format is "processId!accessFlags;", e.g. "1234!123456;"
        /// </summary>
        [ConfigurationProperty("processIdRights", IsRequired = false)]
        public string ProcessIdRights
        {
            get { return (string)base["processIdRights"]; }
            set { base["processIdRights"] = value; }
        }

    
        [ConfigurationProperty("encryptionPassPhrase", IsRequired = false)]
        public string EncryptionPassPhrase
        {
            get
            {
                string key = (string)base["encryptionPassPhrase"];
                if (Utils.IsBase64String(key))
                {
                    key = FilterAPI.AESEncryptDecryptStr(key, FilterAPI.EncryptType.Decryption);
                }

                return key;
            }
            set 
            {
                string key = value.Trim();

                if (key.Length > 0)
                {
                    key = FilterAPI.AESEncryptDecryptStr(key, FilterAPI.EncryptType.Encryption);
                }

                base["encryptionPassPhrase"] = key; 
            }
        }

        /// <summary>
        /// the AES encryption key size, it could be 8bytes(128bit), 24bytes(196bits), 32bytes(256bits).
        /// </summary>
        [ConfigurationProperty("encryptionKeySize", IsRequired = false)]
        public int EncryptionKeySize
        {
            get { return (int)base["encryptionKeySize"]; }
            set { base["encryptionKeySize"] = value; }
        }


        /// <summary>
        /// the file access control flags for the filter rule.
        /// </summary>
        [ConfigurationProperty("accessFlag", IsRequired = true)]
        public uint AccessFlag
        {
            get { return (uint)base["accessFlag"]; }
            set { base["accessFlag"] = value; }
        }

        /// <summary>
        /// The register the file I/O events
        /// </summary>
        [ConfigurationProperty("eventType", IsRequired = false)]
        public uint EventType
        {
            get { return (uint)base["eventType"]; }
            set { base["eventType"] = value; }
        }

        /// <summary>
        /// register monitor I/O requests
        /// </summary>
        [ConfigurationProperty("monitorIO", IsRequired = false)]
        public uint MonitorIO
        {
            get { return (uint)base["monitorIO"]; }
            set { base["monitorIO"] = value; }
        }

        /// <summary>
        /// register control I/O requests, the filter driver will block and wait for the response.
        /// </summary>
        [ConfigurationProperty("controlIO", IsRequired = false)]
        public uint ControlIO
        {
            get { return (uint)base["controlIO"]; }
            set { base["controlIO"] = value; }
        }

        [ConfigurationProperty("filterDesiredAccess", IsRequired = false)]
        public uint FilterDesiredAccess
        {
            get { return (uint)base["filterDesiredAccess"]; }
            set { base["filterDesiredAccess"] = value; }
        }

        [ConfigurationProperty("filterDisposition", IsRequired = false)]
        public uint FilterDisposition
        {
            get { return (uint)base["filterDisposition"]; }
            set { base["filterDisposition"] = value; }
        }

        [ConfigurationProperty("filterCreateOptions", IsRequired = false)]
        public uint FilterCreateOptions
        {
            get { return (uint)base["filterCreateOptions"]; }
            set { base["filterCreateOptions"] = value; }
        }
        /// <summary>
        /// Enable the filter rule in boot time for control filter if it is true.
        /// </summary>
        [ConfigurationProperty("isResident", IsRequired = false)]
        public bool IsResident
        {
            get { return (bool)base["isResident"]; }
            set { base["isResident"] = value; }
        }

        /// <summary>
        /// the encryption method for filter driver based on the filter rule, reference EncryptMethod enumeration definition
        /// </summary>
        [ConfigurationProperty("encryptMethod", IsRequired = false)]
        public int EncryptMethod
        {
            get { return (int)base["encryptMethod"]; }
            set { base["encryptMethod"] = value; }
        }

        /// <summary>
        /// The filter rule type to categorize the filter rules.
        /// </summary>
        [ConfigurationProperty("type", IsRequired = false)]
        public int Type
        {
            get { return (int)base["type"]; }
            set { base["type"] = value; }
        }

        /// <summary>
        /// the filter rule Id
        /// </summary>
        [ConfigurationProperty("id", IsRequired = false)]
        public uint Id
        {
            get { return (uint)base["id"]; }
            set { base["id"] = value; }
        }

        public FilterRule Copy()
        {
            FilterRule dest = new FilterRule();
            dest.AccessFlag = AccessFlag;
            dest.ControlIO = ControlIO;
            dest.EncryptionPassPhrase = EncryptionPassPhrase;
            dest.EncryptMethod = EncryptMethod;
            dest.EventType = EventType;
            dest.ExcludeFileFilterMasks = ExcludeFileFilterMasks;
            dest.ExcludeProcessIds = ExcludeProcessIds;
            dest.ExcludeProcessNames = ExcludeProcessNames;
            dest.ExcludeUserNames = ExcludeUserNames;
            dest.HiddenFileFilterMasks = HiddenFileFilterMasks;
            dest.IncludeFileFilterMask = IncludeFileFilterMask;
            dest.IncludeProcessIds = IncludeProcessIds;
            dest.IncludeProcessNames = IncludeProcessNames;
            dest.IncludeUserNames = IncludeUserNames;
            dest.IsResident = IsResident;
            dest.MonitorIO = MonitorIO;
            dest.ProcessIdRights = ProcessIdRights;
            dest.ProcessRights = ProcessRights;            
            dest.ReparseFileFilterMasks = ReparseFileFilterMasks;
            dest.Type = Type;
            dest.UserRights = UserRights;
            dest.Id = Id;

            return dest;
        }
    }


}
