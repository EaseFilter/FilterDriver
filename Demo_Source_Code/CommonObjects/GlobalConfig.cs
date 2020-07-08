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
using System.ComponentModel;
using System.Data;
using System.Text;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.IO;
using System.Configuration;
using System.Collections;
using System.Xml;
using System.Text.RegularExpressions;
using System.Threading;
using System.Reflection;
using System.Diagnostics;

namespace EaseFilter.CommonObjects
{

    public class GlobalConfig
    {
        //Purchase a license key with the link: http://www.easefilter.com/Order.htm
        //Email us to request a trial key: info@easefilter.com //free email is not accepted.
        public static string registerKey = "************************************";

        static Assembly assembly = System.Reflection.Assembly.GetEntryAssembly();
        public static string AssemblyPath = Path.GetDirectoryName(assembly.Location);
        public static string AssemblyName = assembly.Location;

        //the message output level. It will output the messages which less than this level.
        static EventLevel eventLevel = EventLevel.Information;
        static bool[] selectedDisplayEvents = new bool[] { false, true, true, true, false, false };
        static EventOutputType eventOutputType = EventOutputType.File;
        //The log file name if outputType is ToFile.
        static string eventLogFileName = "EventLog.txt";
        static int maxEventLogFileSize = 4 * 1024 * 1024; //4MB
        static string eventSource = "EaseFilter";
        static string eventLogName = "EaseFilter";

        static uint filterConnectionThreads = 5;
        static uint connectionTimeOut = 10; //seconds
        static private Dictionary<string, FilterRule> filterRules = new Dictionary<string, FilterRule>();
        static List<uint> includePidList = new List<uint>();
        static List<uint> excludePidList = new List<uint>();
        static List<uint> protectPidList = new List<uint>();
        static string includedUsers = string.Empty;
        static string excludedUsers = string.Empty;

        static uint requestIORegistration = 0;
        static uint displayEvents = 0;

        static string accountName = "";
        static string masterPassword = "testpassword";
        static string activatedLicense = string.Empty;

        static int maximumFilterMessages = 500;
        static string filterMessageLogName = "filterMessage.log";
        static long filterMessageLogFileSize = 10 * 1024 * 1024;
        static bool enableLogTransaction = false;
        static bool outputMessageToConsole = true;
        static bool enableNotification = false;

        static string protectFolder = "c:\\EaseFilter\\protectFolder";
        static string protectFolderWhiteList = "*";
        static string protectFolderBlackList = "explorer.exe";
        static string shareFolder = "c:\\EaseFilter\\shareFolder";
        static string shareFolderWhiteList = "notepad.exe;wordpad.exe";
        static string shareFolderBlackList = "explorer.exe";
        static string drInfoFolder = "c:\\EaseFilter\\DRInfoFolder";
        static bool storeSharedFileMetaDataInServer = false;
        static string shareFileExt = ".psf";

        static string configFileName = ConfigSetting.GetFilePath();

        //the filter driver will use the default IV to encrypt the new file if it is true.
        static bool enableDefaultIVKey = false;

        static uint currentPid = (uint)System.Diagnostics.Process.GetCurrentProcess().Id;

        //dont display the directory IO request if it is true.
        static bool disableDirIO = false;

        static uint booleanConfig = 0;

        public static bool isRunning = true;
        public static ManualResetEvent stopEvent = new ManualResetEvent(false);

        public static FilterAPI.FilterType filterType = FilterAPI.FilterType.FILE_SYSTEM_MONITOR;

        public static Stopwatch stopWatch = new Stopwatch();

        //the process filter rule collection
        static private Dictionary<string, ProcessFilterRule> processFilterRules = new Dictionary<string, ProcessFilterRule>();

        //the registry filter rule collection
        static private Dictionary<string, RegistryFilterRule> registryFilterRules = new Dictionary<string, RegistryFilterRule>();

        //the global filter rule Id counter;
        static uint filterRuleId = 1;

        static GlobalConfig()
        {
            Utils.CopyOSPlatformDependentFiles();
            stopWatch.Start();

            Load();
        }

        public static void Load()
        {
            filterRules.Clear();
            processFilterRules.Clear();
            registryFilterRules.Clear();

            try
            {

                filterRules = ConfigSetting.GetFilterRules();
                processFilterRules = ConfigSetting.GetProcessFilterRules();
                registryFilterRules = ConfigSetting.GetRegistryFilterRules();

                booleanConfig = ConfigSetting.Get("booleanConfig", booleanConfig);
                filterConnectionThreads = ConfigSetting.Get("filterConnectionThreads", filterConnectionThreads);
                requestIORegistration = ConfigSetting.Get("requestIORegistration", requestIORegistration);
                displayEvents = ConfigSetting.Get("displayEvents", displayEvents);
                filterMessageLogName = ConfigSetting.Get("filterMessageLogName", filterMessageLogName);
                filterMessageLogFileSize = ConfigSetting.Get("filterMessageLogFileSize", filterMessageLogFileSize);
                maximumFilterMessages = ConfigSetting.Get("maximumFilterMessages", maximumFilterMessages);
                enableLogTransaction = ConfigSetting.Get("enableLogTransaction", enableLogTransaction);
                activatedLicense = ConfigSetting.Get("activatedLicense", activatedLicense);
                enableDefaultIVKey = ConfigSetting.Get("enableDefaultIVKey", enableDefaultIVKey);
                accountName = ConfigSetting.Get("accountName", accountName);

                outputMessageToConsole = ConfigSetting.Get("outputMessageToConsole", outputMessageToConsole);
                enableNotification = ConfigSetting.Get("enableNotification", enableNotification);
                eventLevel = (EventLevel)ConfigSetting.Get("eventLevel", (uint)eventLevel);

                masterPassword = ConfigSetting.Get("masterPassword", masterPassword);
                masterPassword = FilterAPI.AESEncryptDecryptStr(masterPassword, FilterAPI.EncryptType.Decryption);

                includedUsers = ConfigSetting.Get("includedUsers", includedUsers);
                excludedUsers = ConfigSetting.Get("excludedUsers", excludedUsers);

                //setting for secure file sharing
                protectFolder = ConfigSetting.Get("protectFolder", protectFolder);
                protectFolderWhiteList = ConfigSetting.Get("protectFolderWhiteList", protectFolderWhiteList);
                protectFolderBlackList = ConfigSetting.Get("protectFolderBlackList", protectFolderBlackList);
                drInfoFolder = ConfigSetting.Get("drInfoFolder", drInfoFolder);
                shareFolder = ConfigSetting.Get("shareFolder", shareFolder);
                shareFolderWhiteList = ConfigSetting.Get("shareFolderWhiteList", shareFolderWhiteList);
                shareFolderBlackList = ConfigSetting.Get("shareFolderBlackList", shareFolderBlackList);
                storeSharedFileMetaDataInServer = ConfigSetting.Get("storeSharedFileMetaDataInServer", storeSharedFileMetaDataInServer);
                shareFileExt = ConfigSetting.Get("shareFileExt", shareFileExt);

            }
            catch (Exception ex)
            {
                EventManager.WriteMessage(176, "LoadConfigSetting", EventLevel.Error, "Load config file " + configFileName + " failed with error:" + ex.Message);
            }
        }

        public static void Stop()
        {
            isRunning = false;
            stopEvent.Set();
            EventManager.Stop();           
        }
     
        public static bool SaveConfigSetting()
        {
            bool ret = true;

            try
            {
                ConfigSetting.Save();

                if (FilterAPI.IsFilterStarted)
                {
                    SendConfigSettingsToFilter();
                }
            }
            catch (Exception ex)
            {
                EventManager.WriteMessage(235, "SaveConfigSetting", EventLevel.Error, "Save config file " + configFileName + " failed with error:" + ex.Message);
                ret = false;
            }

            return ret;
        }

        /// <summary>
        /// The filter rule Id is the identifier of the filter rule, when you add filter rule to filter with the Id, 
        /// then you can get the Id in the messageSend structure when the registered IO was callback.
        /// </summary>
        /// <returns></returns>
        static public uint GetFilterRuleId()
        {
            return filterRuleId++;
        }

        static public string  ConfigFilePath
        {
            get { return configFileName; }
            set {  configFileName = value; }
        }

        /// <summary>
        /// the globalboolean config setting, please reference the enumeration of FilterAPI.BooleanConfig
        /// </summary>
        static public uint BooleanConfig
        {
            get { return booleanConfig; }
            set 
            {
                booleanConfig = value;
                ConfigSetting.Set("booleanConfig", value.ToString());
            }
        }

        /// <summary>
        /// don't display the directory IO request if it is true.
        /// </summary>
        static public bool DisableDirIO
        {
            get { return disableDirIO; }
            set { disableDirIO = value; }
        }

        static public bool IsRunning
        {
            get { return isRunning; }
        }

        static public ManualResetEvent StopEvent
        {
            get { return stopEvent; }
        }

        static public bool[] SelectedDisplayEvents
        {
            get
            {
                return selectedDisplayEvents;
            }
            set
            {
                selectedDisplayEvents = value;
            }
        }

        static public EventLevel EventLevel
        {
            get
            {
                return eventLevel;
            }
            set
            {
                eventLevel = value;
                EventManager.Level = value;
                ConfigSetting.Set("eventLevel", ((uint)value).ToString());
            }
        }

        static public EventOutputType EventOutputType
        {
            get
            {
                return eventOutputType;
            }
            set
            {
                eventOutputType = value;
            }
        }

        static public string EventLogFileName
        {
            get
            {
                return eventLogFileName;
            }
            set
            {
                eventLogFileName = value;
            }
        }

        static public int MaxEventLogFileSize
        {
            get
            {
                return maxEventLogFileSize;
            }
            set
            {
                maxEventLogFileSize = value;
            }
        }

        static public string EventSource
        {
            get
            {
                return eventSource;
            }
            set
            {
                eventSource = value;
            }
        }


        static public string EventLogName
        {
            get
            {
                return eventLogName;
            }
            set
            {
                eventLogName = value;
            }
        }


        public static uint FilterConnectionThreads
        {
            get { return filterConnectionThreads; }
            set
            { 
                filterConnectionThreads = value;
                ConfigSetting.Set("filterConnectionThreads", value.ToString());
            }
        }

        public static uint RequestIORegistration
        {
            get { return requestIORegistration; }
            set 
            { 
                requestIORegistration = value;
                ConfigSetting.Set("requestIORegistration", value.ToString());
            }
        }

        public static uint DisplayEvents
        {
            get { return displayEvents; }
            set 
            { 
                displayEvents = value;
                ConfigSetting.Set("displayEvents", value.ToString());
            }
        }

        public static string FilterMessageLogName
        {
            get { return filterMessageLogName; }
            set 
            { 
                filterMessageLogName = value;
                ConfigSetting.Set("filterMessageLogName", value.ToString());
            }
        }

        public static long FilterMessageLogFileSize
        {
            get { return filterMessageLogFileSize; }
            set 
            {
                filterMessageLogFileSize = value;
                ConfigSetting.Set("filterMessageLogFileSize", value.ToString());
            }
        }

        public static int MaximumFilterMessages
        {
            get { return maximumFilterMessages; }
            set
            { 
                maximumFilterMessages = value;
                ConfigSetting.Set("maximumFilterMessages", value.ToString());
            }
        }

        public static bool EnableLogTransaction
        {
            get { return enableLogTransaction; }
            set 
            { 
                enableLogTransaction = value;
                ConfigSetting.Set("enableLogTransaction", value.ToString());
            }
        }

        public static bool OutputMessageToConsole
        {
            get { return outputMessageToConsole; }
            set
            {
                outputMessageToConsole = value;
                ConfigSetting.Set("outputMessageToConsole", value.ToString());
            }
        }

        public static bool EnableNotification
        {
            get { return enableNotification; }
            set
            {
                enableNotification = value;
                ConfigSetting.Set("enableNotification", value.ToString());
            }
        }

        public static List<uint> IncludePidList
        {
            get { return includePidList; }
            set { includePidList = value; }
        }

        public static List<uint> ExcludePidList
        {
            get { return excludePidList; }
            set { excludePidList = value; }
        }

        public static List<uint> ProtectPidList
        {
            get { return protectPidList; }
            set { protectPidList = value; }
        }


        public static uint ConnectionTimeOut
        {
            get { return connectionTimeOut; }
            set 
            {
                connectionTimeOut = value;
                ConfigSetting.Set("connectionTimeOut", value.ToString());
            }
        }

        public static string ActivatedLisense
        {
            get { return activatedLicense; }
            set
            { 
                activatedLicense = value;
                ConfigSetting.Set("activatedLicense", value.ToString());
            }
        }

        public static bool EnableDefaultIVKey
        {
            get { return enableDefaultIVKey; }
            set
            {
                enableDefaultIVKey = value;
                ConfigSetting.Set("enableDefaultIVKey", value.ToString());
            }
        }

        public static string AccountName
        {
            get { return accountName; }
            set
            {
                accountName = value;
                ConfigSetting.Set("accountName", value.ToString());
            }
        }


        public static string MasterPassword
        {
            get
            {
                return masterPassword; 
            }
            set
            {
                masterPassword = value;
                string encryptedPassword = FilterAPI.AESEncryptDecryptStr(value.ToString(), FilterAPI.EncryptType.Encryption);
                ConfigSetting.Set("masterPassword", encryptedPassword);
            }
        }

        public static string IncludedUsers
        {
            get { return includedUsers; }
            set
            {
                includedUsers = value;
                ConfigSetting.Set("includedUsers", value.ToString());
            }
        }

        public static string ExcludedUsers
        {
            get { return excludedUsers; }
            set
            {
                excludedUsers = value;
                ConfigSetting.Set("excludedUsers", value.ToString());
            }
        }

        public static string ProtectFolder
        {
            get { return protectFolder; }
            set
            {
                protectFolder = value;
                ConfigSetting.Set("protectFolder", value.ToString());
            }
        }

        public static string ProtectFolderWhiteList
        {
            get { return protectFolderWhiteList; }
            set
            {
                protectFolderWhiteList = value;
                ConfigSetting.Set("protectFolderWhiteList", value.ToString());
            }
        }

        public static string ProtectFolderBlackList
        {
            get { return protectFolderBlackList; }
            set
            {
                protectFolderBlackList = value;
                ConfigSetting.Set("protectFolderBlackList", value.ToString());
            }
        }

        public static string DRInfoFolder
        {
            get { return drInfoFolder; }
            set
            {
                drInfoFolder = value;
                ConfigSetting.Set("drInfoFolder", value.ToString());
            }
        }

        public static string ShareFolder
        {
            get { return shareFolder; }
            set
            {
                shareFolder = value;
                ConfigSetting.Set("shareFolder", value.ToString());
            }
        }

        public static string ShareFolderWhiteList
        {
            get { return shareFolderWhiteList; }
            set
            {
                shareFolderWhiteList = value;
                ConfigSetting.Set("shareFolderWhiteList", value.ToString());
            }
        }

        public static string ShareFolderBlackList
        {
            get { return shareFolderBlackList; }
            set
            {
                shareFolderBlackList = value;
                ConfigSetting.Set("shareFolderBlackList", value.ToString());
            }
        }

        public static bool StoreSharedFileMetaDataInServer
        {
            get { return storeSharedFileMetaDataInServer; }
            set
            {
                storeSharedFileMetaDataInServer = value;
                ConfigSetting.Set("storeSharedFileMetaDataInServer", value.ToString());
            }
        }

        public static string ShareFileExt
        {
            get { return shareFileExt; }
            set
            {
                shareFileExt = value;
                ConfigSetting.Set("shareFileExt", value.ToString());
            }
        }

        public static bool AddFilterRule( FilterRule newRule)
        {
            if (filterRules.ContainsKey(newRule.IncludeFileFilterMask))
            {
                //the exist filter rule already there,remove it
                filterRules.Remove(newRule.IncludeFileFilterMask);
                ConfigSetting.RemoveFilterRule(newRule.IncludeFileFilterMask);
            }

            filterRules.Add(newRule.IncludeFileFilterMask, newRule);

            ConfigSetting.AddFilterRule(newRule);

            return true;
        }

        public static void RemoveFilterRule(string includeFilterMask)
        {
            if (filterRules.ContainsKey(includeFilterMask))
            {
                filterRules.Remove(includeFilterMask);
                ConfigSetting.RemoveFilterRule(includeFilterMask);
            }

        }

        public static void RemoveAllFilterRules()
        {
            foreach (FilterRule filterRule in filterRules.Values)
            {
                ConfigSetting.RemoveFilterRule(filterRule.IncludeFileFilterMask);
            }

            filterRules.Clear();
        }

        public static bool IsFilterRuleExist(string includeFilterMask)
        {
            if (filterRules.ContainsKey(includeFilterMask))
            {
                return true;
            }

            return false;
        }

        public static Dictionary<string, FilterRule> FilterRules
        {
            get { return filterRules; }
        }

        public static bool AddProcessFilterRule(ProcessFilterRule filterRule)
        {
            string key = filterRule.ProcessNameFilterMask;

            if (key.Trim().Length == 0)
            {
                key = filterRule.ProcessId;
            }

            if (processFilterRules.ContainsKey(key))
            {
                processFilterRules.Remove(key);
                ConfigSetting.RemoveProcessFilterRule(filterRule);
            }

            processFilterRules.Add(key, filterRule);
            ConfigSetting.AddProcessFilterRule(filterRule);

            return true;
        }

        public static void RemoveProcessFilterRule(ProcessFilterRule filterRule)
        {
            string key = filterRule.ProcessNameFilterMask;

            if (key.Trim().Length == 0)
            {
                key = filterRule.ProcessId;
            }

            if (processFilterRules.ContainsKey(key))
            {
                processFilterRules.Remove(key);
                ConfigSetting.RemoveProcessFilterRule(filterRule);
            }
        }

        public static ProcessFilterRule GetProcessFilterRule(string processId, string processNameFilterMask)
        {
            string key = processNameFilterMask;

            if (key.Trim().Length == 0)
            {
                key = processId;
            }

            if (processFilterRules.ContainsKey(key))
            {
                return processFilterRules[key];
            }

            return null;
        }

        public static Dictionary<string, ProcessFilterRule> ProcessFilterRules
        {
            get { return processFilterRules; }
        }

        public static bool AddRegistryFilterRule(RegistryFilterRule filterRule)
        {
            string key = filterRule.ProcessNameFilterMask;

            if (key.Trim().Length == 0)
            {
                key = filterRule.ProcessId;
            }

            if (registryFilterRules.ContainsKey(key))
            {
                registryFilterRules.Remove(key);
                ConfigSetting.RemoveRegistryFilterRule(filterRule.ProcessId, filterRule.ProcessNameFilterMask);
            }

            registryFilterRules.Add(key, filterRule);
            ConfigSetting.AddRegistryFilterRule(filterRule);

            return true;
        }

        public static void RemoveRegistryFilterRule(RegistryFilterRule filterRule)
        {
            string key = filterRule.ProcessNameFilterMask;

            if (key.Trim().Length == 0)
            {
                key = filterRule.ProcessId;
            }

            if (registryFilterRules.ContainsKey(key))
            {
                registryFilterRules.Remove(key);
                ConfigSetting.RemoveRegistryFilterRule(filterRule.ProcessId, filterRule.ProcessNameFilterMask);
            }
        }

        public static RegistryFilterRule GetRegistryFilterRule(string processId, string processNameFilterMask)
        {
            string key = processNameFilterMask;

            if (key.Trim().Length == 0)
            {
                key = processId;
            }

            if (registryFilterRules.ContainsKey(key))
            {
                return registryFilterRules[key];
            }

            return null;
        }

        public static Dictionary<string, RegistryFilterRule> RegistryFilterRules
        {
            get { return registryFilterRules; }
        }

        public static void SendConfigSettingsToFilter()
        {
            try
            {
                if (!FilterAPI.IsFilterStarted)
                {
                    EventManager.WriteMessage(479, "SetFilterType", EventLevel.Error, "SendConfigSettingsToFilter failed, the filter driver is not loaded.");
                    return;
                }

                FilterAPI.ResetConfigData();

                FilterAPI.SetConnectionTimeout(connectionTimeOut);

                if (!FilterAPI.SetFilterType((uint)filterType))
                {
                    EventManager.WriteMessage(443, "SetFilterType", EventLevel.Error, "SetFilterType " + filterType.ToString() + " failed:" + FilterAPI.GetLastErrorMessage());
                }
                else
                {
                    EventManager.WriteMessage(447, "SetFilterType", EventLevel.Information, "SetFilterType " + filterType.ToString() + " succeeded.");
                }

                //
                //Process filter driver config settings here.
                //
                foreach (ProcessFilterRule filterRule in processFilterRules.Values)
                {
                    if (filterRule.ControlFlag > 0 && filterRule.ProcessNameFilterMask.Length > 0)
                    {
                        //set the process control flag for the process filter mask. 
                        if (!FilterAPI.AddProcessFilterRule((uint)filterRule.ProcessNameFilterMask.Length * 2, filterRule.ProcessNameFilterMask, filterRule.ControlFlag, filterRule.Id))
                        {
                            EventManager.WriteMessage(791, "AddProcessFilterRule", EventLevel.Error, "AddProcessFilterRule " + filterRule.ProcessNameFilterMask + " failed:" + FilterAPI.GetLastErrorMessage());
                        }
                        else
                        {
                            EventManager.WriteMessage(791, "AddProcessFilterRule", EventLevel.Information, "AddProcessFilterRule " + filterRule.ProcessNameFilterMask + ",ProcessControlFlag:" + filterRule.ControlFlag + " succeeded.");
                        }

                    }


                    //Filter and control the file accesss by process name or Id.
                    string[] fileAccessRights = filterRule.FileAccessRights.Split(new char[] { ';' });
                    if (fileAccessRights.Length > 0)
                    {
                        foreach (string fileAccessRight in fileAccessRights)
                        {
                            if (fileAccessRight.Trim().Length > 0)
                            {
                                string fileNameMask = fileAccessRight.Substring(0, fileAccessRight.IndexOf('!'));
                                uint accessFlag = uint.Parse(fileAccessRight.Substring(fileAccessRight.IndexOf('!') + 1));

                                if (filterRule.ProcessId.Length > 0)
                                {
                                    string[] pids = filterRule.ProcessId.Split(new char[] { ';' });
                                    for (int i = 0; i < pids.Length; i++)
                                    {
                                        uint processId = 0;
                                        if (uint.TryParse(pids[i], out processId) && processId > 0)
                                        {
                                            //filter by process Id
                                            if (!FilterAPI.AddFileControlToProcessById(processId, (uint)fileNameMask.Length * 2, fileNameMask.Trim(), accessFlag))
                                            {
                                                EventManager.WriteMessage(725, "AddFileControlToProcessByName", EventLevel.Error, "AddFileControlToProcessByName " + filterRule.ProcessNameFilterMask + ",fileNameMask:" + fileNameMask + ",accessFlags:" + accessFlag + " failed:" + FilterAPI.GetLastErrorMessage());
                                            }
                                            else
                                            {
                                                EventManager.WriteMessage(729, "AddFileControlToProcessByName", EventLevel.Information, "AddFileControlToProcessByName " + filterRule.ProcessNameFilterMask + ",fileNameMask:" + fileNameMask + ",accessFlags:" + accessFlag + " succeeded.");
                                            }
                                        }
                                    }
                                }
                                else if (filterRule.ProcessNameFilterMask.Length > 0)
                                {
                                    //add file access control with process name
                                    if (!FilterAPI.AddFileControlToProcessByName((uint)filterRule.ProcessNameFilterMask.Length * 2, 
                                        filterRule.ProcessNameFilterMask, (uint)fileNameMask.Length * 2, fileNameMask.Trim(), accessFlag ))
                                    {
                                        EventManager.WriteMessage(725, "AddFileControlToProcessByName", EventLevel.Error, "AddFileControlToProcessByName " + filterRule.ProcessNameFilterMask + ",fileNameMask:" + fileNameMask + ",accessFlags:" + accessFlag + " failed:" + FilterAPI.GetLastErrorMessage());
                                    }
                                    else
                                    {
                                        EventManager.WriteMessage(729, "AddFileControlToProcessByName", EventLevel.Information, "AddFileControlToProcessByName " + filterRule.ProcessNameFilterMask + ",fileNameMask:" + fileNameMask + ",accessFlags:" + accessFlag + " succeeded.");
                                    }
                                }
                            }
                        }

                    }
                }

                //
                //Registry filter driver config setting
                //
                foreach (RegistryFilterRule filterRule in registryFilterRules.Values)
                {
                    uint processId = 0;
                    uint.TryParse(filterRule.ProcessId,out processId);

                    if (!FilterAPI.AddRegistryFilterRule((uint)filterRule.ProcessNameFilterMask.Length * 2, filterRule.ProcessNameFilterMask, processId, 0, "",
                        (uint)filterRule.RegistryKeyNameFilterMask.Length*2, filterRule.RegistryKeyNameFilterMask,
                         filterRule.AccessFlag, filterRule.RegCallbackClass, filterRule.IsExcludeFilter))
                    {
                        EventManager.WriteMessage(900, "AddRegistryFilterRule", EventLevel.Error, "AddRegistryFilterRule failed:" + FilterAPI.GetLastErrorMessage());
                    }
                    else
                    {
                        EventManager.WriteMessage(901, "AddRegistryFilterRule", EventLevel.Information, "AddRegistryFilterRule, processName:" + filterRule.ProcessNameFilterMask + ",RegistryControlFlag:" + filterRule.AccessFlag + " succeeded.");
                    }


                }

                //
                //General file access filter rule
                //
                foreach (FilterRule filterRule in filterRules.Values)
                {


                    //add filter rule to filter driver here, the filter rule is unique with the include file filter mask.
                    //you can't have the mutiple filter rules with the same include file filter mask,if there are the same 
                    //one exist, the new one with accessFlag will overwrite the old accessFlag.
                    //for control filter, if isResident is true, the access control will be enabled in boot time.
                    if (!FilterAPI.AddFileFilterRule((uint)filterRule.AccessFlag, filterRule.IncludeFileFilterMask, filterRule.IsResident, filterRule.Id))
                    {
                        EventManager.WriteMessage(456, "SendFilterRule", EventLevel.Error, "Send filter rule failed:" + FilterAPI.GetLastErrorMessage());
                    }
                    else
                    {
                        EventManager.WriteMessage(460, "SendFilterRule", EventLevel.Information, "Send filter rule:" + filterRule.IncludeFileFilterMask);
                    }

                    if (!FilterAPI.RegisterEventTypeToFilterRule(filterRule.IncludeFileFilterMask, (uint)filterRule.EventType))
                    {
                        EventManager.WriteMessage(478, "SendFilterRule", EventLevel.Error, "Register event type:" + (FilterAPI.EVENTTYPE)filterRule.EventType + " failed:" + FilterAPI.GetLastErrorMessage());
                    }
                    else
                    {
                        EventManager.WriteMessage(482, "SendFilterRule", EventLevel.Information, "Register event type:" + (FilterAPI.EVENTTYPE)filterRule.EventType + " succeed.");
                    }

                    if (!FilterAPI.RegisterMoinitorIOToFilterRule(filterRule.IncludeFileFilterMask, filterRule.MonitorIO))
                    {
                        EventManager.WriteMessage(499, "SendFilterRule", EventLevel.Error, "Register monitor IO:" + filterRule.MonitorIO + " failed:" + FilterAPI.GetLastErrorMessage());
                    }
                    else
                    {
                        EventManager.WriteMessage(503, "SendFilterRule", EventLevel.Information, "Register monitor IO:" + filterRule.MonitorIO + " succeed.");
                    }

                    if (!FilterAPI.RegisterControlIOToFilterRule(filterRule.IncludeFileFilterMask, filterRule.ControlIO))
                    {
                        EventManager.WriteMessage(508, "SendFilterRule", EventLevel.Error, "Register control IO:" + filterRule.ControlIO + " failed:" + FilterAPI.GetLastErrorMessage());
                    }
                    else
                    {
                        EventManager.WriteMessage(512, "SendFilterRule", EventLevel.Information, "Register control IO:" + filterRule.ControlIO + " succeed.");
                    }

                    if (!FilterAPI.AddRegisterIOFilterToFilterRule(filterRule.IncludeFileFilterMask, filterRule.FilterDesiredAccess,filterRule.FilterDisposition, filterRule.FilterCreateOptions))
                    {
                        EventManager.WriteMessage(508, "SendFilterRule", EventLevel.Error, "AddRegisterIOFilterToFilterRule failed:" + FilterAPI.GetLastErrorMessage());
                    }
                    else
                    {
                        EventManager.WriteMessage(512, "SendFilterRule", EventLevel.Information
                            , "AddRegisterIOFilterToFilterRule FilterDesiredAccess:" + filterRule.FilterDesiredAccess + ",FilterDisposition:" 
                            + filterRule.FilterDisposition + ",FilterCreateOptions:" + filterRule.FilterCreateOptions + " succeed.");
                    }

                    //every filter rule can have multiple exclude file filter masks, you can exclude the files 
                    //which matches the exclude filter mask.
                    string[] excludeFilterMasks = filterRule.ExcludeFileFilterMasks.Split(new char[] { ';' });
                    if (excludeFilterMasks.Length > 0)
                    {
                        foreach (string excludeFilterMask in excludeFilterMasks)
                        {
                            if (excludeFilterMask.Trim().Length > 0)
                            {
                                if (!FilterAPI.AddExcludeFileMaskToFilterRule(filterRule.IncludeFileFilterMask, excludeFilterMask.Trim()))
                                {
                                    EventManager.WriteMessage(496, "AddExcludeFileMaskToFilterRule", EventLevel.Error, "AddExcludeFileMaskToFilterRule " + excludeFilterMask + " failed:" + FilterAPI.GetLastErrorMessage());
                                }
                                else
                                {
                                    EventManager.WriteMessage(500, "AddExcludeFileMaskToFilterRule", EventLevel.Information, "AddExcludeFileMaskToFilterRule " + excludeFilterMask + " succeeded.");
                                }
                            }
                        }

                    }

                    //you can enable the encryption per filter rule, set the FILE_ENCRYPTION_RULE to accessFlag.
                    if ((filterRule.AccessFlag & (uint)FilterAPI.AccessFlag.ENABLE_FILE_ENCRYPTION_RULE) > 0)
                    {
                        byte[] encryptionKey = null;
                        uint encryptionKeyLength = 0;
                        byte[] iv = null;
                        uint ivLength = 0;

                        if (filterRule.EncryptionKeySize <= 0)
                        {
                            filterRule.EncryptionKeySize = 32;
                        }

                        encryptionKey = Utils.GetKeyByPassPhrase(filterRule.EncryptionPassPhrase,filterRule.EncryptionKeySize);
                        encryptionKeyLength = (uint)encryptionKey.Length;

                        switch ((FilterAPI.EncryptionMethod)filterRule.EncryptMethod)
                        {
                            case FilterAPI.EncryptionMethod.ENCRYPT_FILE_WITH_SAME_KEY_AND_IV:
                                {
                                    //Set an encryption folder, assume all files inside the folder were encrypted, all encrypted files use the same encryption key and IV key. 
                                    //The encryption file doesn’t embed any encryption information, you can’t tell if the file was encrypted or not by checking the file information, 
                                    //you can share or transfer the encrypted file without limitation. To check if the file was encrypted, you need to stop the encryption filter driver service, 
                                    //then open the encrypted file, you will get cipher data instead of the clear data. 

                                    iv = Utils.GetIVByPassPhrase(filterRule.EncryptionPassPhrase);
                                    ivLength = (uint)iv.Length;

                                    if (!FilterAPI.AddEncryptionKeyAndIVToFilterRule(filterRule.IncludeFileFilterMask, encryptionKeyLength, encryptionKey, ivLength, iv))
                                    {
                                        EventManager.WriteMessage(482, "AddEncryptionKeyAndIVToFilterRule", EventLevel.Error, "AddEncryptionKeyAndIVToFilterRule " + filterRule.IncludeFileFilterMask + " failed:" + FilterAPI.GetLastErrorMessage());
                                    }
                                    else
                                    {
                                        EventManager.WriteMessage(482, "AddEncryptionKeyAndIVToFilterRule", EventLevel.Information, "AddEncryptionKeyAndIVToFilterRule succeeded.");
                                    }

                                    break;
                                }

                            case FilterAPI.EncryptionMethod.ENCRYPT_FILE_WITH_SAME_KEY_AND_UNIQUE_IV:
                                {
                                    //Set an encryption folder, every encrypted file has the unique iv key, 
                                    //the encrypted information was embedded into to the file as an extended attribute called reparse point, 
                                    //it only can be supported in NTFS. The same folder can mix encrypted files and unencrypted files,
                                    //the filter driver will know if the file was encrypted by checking the file reparse point attribute.
                                    //Since the reparse point attribute can’t be transferred, the encrypted file can’t be shared or copied outside of your computer with normal method, 
                                    //you need to append the reparse point data to the end of the file, and recreate the new reparse point data to the new file after you copied it to the new computer.

                                    if (!FilterAPI.AddEncryptionKeyToFilterRule(filterRule.IncludeFileFilterMask, encryptionKeyLength, encryptionKey))
                                    {
                                        EventManager.WriteMessage(606, "AddEncryptionKeyToFilterRule", EventLevel.Error, "AddEncryptionKeyToFilterRule " + filterRule.IncludeFileFilterMask + " failed:" + FilterAPI.GetLastErrorMessage());
                                    }
                                    else
                                    {
                                        EventManager.WriteMessage(607, "AddEncryptionKeyToFilterRule", EventLevel.Information, "AddEncryptionKeyToFilterRule succeeded.");
                                    }

                                    break;
                                }
                            case FilterAPI.EncryptionMethod.ENCRYPT_FILE_WITH_KEY_AND_IV_FROM_SERVICE:
                                {
                                    //with this setting, to open or create encrypted file, it will request the encryption key and iv from the user mode callback service.
                                    //the encrypted file won't have reparse point tag data attached.

                                    uint booleanConfig = (uint)FilterAPI.BooleanConfig.REQUEST_ENCRYPT_KEY_AND_IV_FROM_SERVICE;

                                    if (!FilterAPI.AddBooleanConfigToFilterRule(filterRule.IncludeFileFilterMask, booleanConfig))
                                    {
                                        EventManager.WriteMessage(606, "AddBooleanConfigToFilterRule", EventLevel.Error, "AddBooleanConfigToFilterRule " + filterRule.IncludeFileFilterMask + " failed:" + FilterAPI.GetLastErrorMessage());
                                    }
                                    else
                                    {
                                        EventManager.WriteMessage(607, "AddBooleanConfigToFilterRule", EventLevel.Information, "AddBooleanConfigToFilterRule succeeded.");
                                    }

                                    break;

                                }
                            //case FilterAPI.EncryptionMethod.ENCRYPT_FILE_WITH_KEY_IV_AND_TAGDATA_FROM_SERVICE:
                            //    {
                            //        //with this setting, to open or create encrypted file, it will request the encryption key and iv from the user mode callback service.
                            //        //the encrypted file will have your own customized reparse point tag data attached.

                            //        uint booleanConfig = (uint)FilterAPI.BooleanConfig.REQUEST_ENCRYPT_KEY_IV_AND_TAGDATA_FROM_SERVICE;
                            //        if (!FilterAPI.AddBooleanConfigToFilterRule(filterRule.IncludeFileFilterMask, booleanConfig))
                            //        {
                            //            EventManager.WriteMessage(606, "AddBooleanConfigToFilterRule", EventLevel.Error, "AddBooleanConfigToFilterRule " + filterRule.IncludeFileFilterMask + " failed:" + FilterAPI.GetLastErrorMessage());
                            //        }
                            //        else
                            //        {
                            //            EventManager.WriteMessage(607, "AddBooleanConfigToFilterRule", EventLevel.Information, "AddBooleanConfigToFilterRule succeeded.");
                            //        }

                            //        break;

                            //    }

                            default: break;


                        }
                    }


                    string[] includeProcessNames = filterRule.IncludeProcessNames.Split(new char[] { ';' });
                    if (includeProcessNames.Length > 0)
                    {
                        foreach (string includeProcessName in includeProcessNames)
                        {
                            if (includeProcessName.Trim().Length > 0)
                            {
                                if (!FilterAPI.AddIncludeProcessNameToFilterRule(filterRule.IncludeFileFilterMask, includeProcessName.Trim()))
                                {
                                    EventManager.WriteMessage(536, "AddIncludeProcessNameToFilterRule", EventLevel.Error, "AddIncludeProcessNameToFilterRule " + includeProcessName + " failed:" + FilterAPI.GetLastErrorMessage());
                                }
                                else
                                {
                                    EventManager.WriteMessage(540, "AddIncludeProcessNameToFilterRule", EventLevel.Information, "AddIncludeProcessNameToFilterRule " + includeProcessName + " succeeded.");
                                }
                            }
                        }

                    }

                    string[] excludeProcessNames = filterRule.ExcludeProcessNames.Split(new char[] { ';' });
                    if (excludeProcessNames.Length > 0)
                    {
                        foreach (string excludeProcessName in excludeProcessNames)
                        {
                            if (excludeProcessName.Trim().Length > 0)
                            {
                                if (!FilterAPI.AddExcludeProcessNameToFilterRule(filterRule.IncludeFileFilterMask, excludeProcessName.Trim()))
                                {
                                    EventManager.WriteMessage(556, "AddExcludeProcessNameToFilterRule", EventLevel.Error, "AddExcludeProcessNameToFilterRule " + excludeProcessName + " failed:" + FilterAPI.GetLastErrorMessage());
                                }
                                else
                                {
                                    EventManager.WriteMessage(560, "AddExcludeProcessNameToFilterRule", EventLevel.Information, "AddExcludeProcessNameToFilterRule " + excludeProcessName + " succeeded.");
                                }
                            }
                        }

                    }

                    //set include process list for this filter rule.
                    string[] includePidListInFilterRule = filterRule.IncludeProcessIds.Split(new char[] { ';' });
                    if (includePidListInFilterRule.Length > 0)
                    {
                        foreach (string inPidstr in includePidListInFilterRule)
                        {
                            if (inPidstr.Trim().Length > 0)
                            {
                                uint inPid = uint.Parse(inPidstr.Trim());
                                if (!FilterAPI.AddIncludeProcessIdToFilterRule(filterRule.IncludeFileFilterMask, inPid))
                                {
                                    EventManager.WriteMessage(523, "AddIncludeProcessIdToFilterRule", EventLevel.Error, "AddIncludeProcessIdToFilterRule " + filterRule.IncludeFileFilterMask + " PID:" + inPidstr + " failed:" + FilterAPI.GetLastErrorMessage());
                                }
                                else
                                {
                                    EventManager.WriteMessage(527, "AddIncludeProcessIdToFilterRule", EventLevel.Information, "AddIncludeProcessIdToFilterRule " + filterRule.IncludeFileFilterMask + " PID:" + inPidstr + " succeeded.");
                                }
                            }
                        }

                    }

                    //set exclude process list for this filter rule.
                    string[] excludePidListInFilterRule = filterRule.ExcludeProcessIds.Split(new char[] { ';' });
                    if (excludePidListInFilterRule.Length > 0)
                    {
                        foreach (string exPidstr in excludePidListInFilterRule)
                        {
                            if (exPidstr.Trim().Length > 0)
                            {
                                uint exPid = uint.Parse(exPidstr.Trim());
                                if (!FilterAPI.AddExcludeProcessIdToFilterRule(filterRule.IncludeFileFilterMask, exPid))
                                {
                                    EventManager.WriteMessage(545, "AddExcludeProcessIdToFilterRule", EventLevel.Error, "AddExcludeProcessIdToFilterRule " + filterRule.IncludeFileFilterMask + " PID:" + exPidstr + " failed:" + FilterAPI.GetLastErrorMessage());
                                }
                                else
                                {
                                    EventManager.WriteMessage(549, "AddExcludeProcessIdToFilterRule", EventLevel.Information, "AddExcludeProcessIdToFilterRule " + filterRule.IncludeFileFilterMask + " PID:" + exPidstr + " succeeded.");
                                }
                            }

                        }
                    }


                    string[] includeUserNames = filterRule.IncludeUserNames.Split(new char[] { ';' });
                    if (includeUserNames.Length > 0)
                    {
                        foreach (string includeUserName in includeUserNames)
                        {
                            if (includeUserName.Trim().Length > 0)
                            {
                                if (!FilterAPI.AddIncludeUserNameToFilterRule(filterRule.IncludeFileFilterMask, includeUserName.Trim()))
                                {
                                    EventManager.WriteMessage(536, "AddIncludeUserNameToFilterRule", EventLevel.Error, "AddIncludeUserNameToFilterRule " + includeUserName + " failed:" + FilterAPI.GetLastErrorMessage());
                                }
                                else
                                {
                                    EventManager.WriteMessage(540, "AddIncludeUserNameToFilterRule", EventLevel.Information, "AddIncludeUserNameToFilterRule " + includeUserName + " succeeded.");
                                }
                            }
                        }

                    }

                    string[] excludeUserNames = filterRule.ExcludeUserNames.Split(new char[] { ';' });
                    if (excludeUserNames.Length > 0)
                    {
                        foreach (string excludeUserName in excludeUserNames)
                        {
                            if (excludeUserName.Trim().Length > 0)
                            {
                                if (!FilterAPI.AddExcludeUserNameToFilterRule(filterRule.IncludeFileFilterMask, excludeUserName.Trim()))
                                {
                                    EventManager.WriteMessage(556, "AddExcludeUserNameToFilterRule", EventLevel.Error, "AddExcludeUserNameToFilterRule " + excludeUserName + " failed:" + FilterAPI.GetLastErrorMessage());
                                }
                                else
                                {
                                    EventManager.WriteMessage(560, "AddExcludeUserNameToFilterRule", EventLevel.Information, "AddExcludeUserNameToFilterRule " + excludeUserName + " succeeded.");
                                }
                            }
                        }

                    }

                    string[] processRights = filterRule.ProcessRights.Split(new char[] { ';' });
                    if (processRights.Length > 0)
                    {
                        foreach (string processRight in processRights)
                        {
                            if (processRight.Trim().Length > 0)
                            {
                                string processName = processRight.Substring(0, processRight.IndexOf('!'));
                                uint accessFlags = uint.Parse(processRight.Substring(processRight.IndexOf('!') + 1));

                                if (!FilterAPI.AddProcessRightsToFilterRule(filterRule.IncludeFileFilterMask, processName.Trim(), accessFlags))
                                {
                                    EventManager.WriteMessage(725, "AddProcessRightsToFilterRule", EventLevel.Error, "AddProcessRightsToFilterRule " + filterRule.IncludeFileFilterMask + ",processName:" + processName + ",accessFlags:" + accessFlags + " failed:" + FilterAPI.GetLastErrorMessage());
                                }
                                else
                                {
                                    EventManager.WriteMessage(729, "AddProcessRightsToFilterRule", EventLevel.Information, "AddProcessRightsToFilterRule " + filterRule.IncludeFileFilterMask + ",processName:" + processName + ",accessFlags:" + accessFlags + " succeeded.");
                                }
                            }
                        }

                    }

                    string[] processIdRights = filterRule.ProcessIdRights.Split(new char[] { ';' });
                    if (processIdRights.Length > 0)
                    {
                        foreach (string processIdRight in processIdRights)
                        {
                            if (processIdRight.Trim().Length > 0)
                            {
                                uint processId = uint.Parse(processIdRight.Substring(0, processIdRight.IndexOf('!')));
                                uint accessFlags = uint.Parse(processIdRight.Substring(processIdRight.IndexOf('!') + 1));

                                if (!FilterAPI.AddProcessIdRightsToFilterRule(filterRule.IncludeFileFilterMask, processId, accessFlags))
                                {
                                    EventManager.WriteMessage(725, "AddProcessIdRightsToFilterRule", EventLevel.Error, "AddProcessIdRightsToFilterRule " + filterRule.IncludeFileFilterMask + ",processId:" + processId + ",accessFlags:" + accessFlags + " failed:" + FilterAPI.GetLastErrorMessage());
                                }
                                else
                                {
                                    EventManager.WriteMessage(729, "AddProcessIdRightsToFilterRule", EventLevel.Information, "AddProcessIdRightsToFilterRule " + filterRule.IncludeFileFilterMask + ",processId:" + processId + ",accessFlags:" + accessFlags + " succeeded.");
                                }
                            }
                        }

                    }

                    string[] userRights = filterRule.UserRights.Split(new char[] { ';' });
                    if (userRights.Length > 0)
                    {
                        foreach (string userRight in userRights)
                        {
                            if (userRight.Trim().Length > 0)
                            {
                                string userName = userRight.Substring(0, userRight.IndexOf(':'));
                                uint accessFlags = uint.Parse(userRight.Substring(userRight.IndexOf(':') + 1));

                                if (!FilterAPI.AddUserRightsToFilterRule(filterRule.IncludeFileFilterMask, userName.Trim(), accessFlags))
                                {
                                    EventManager.WriteMessage(748, "AddUserRightsToFilterRule", EventLevel.Error, "AddUserRightsToFilterRule " + filterRule.IncludeFileFilterMask + ",userName:" + userName + ",accessFlags:" + accessFlags + " failed:" + FilterAPI.GetLastErrorMessage());
                                }
                                else
                                {
                                    EventManager.WriteMessage(752, "AddUserRightsToFilterRule", EventLevel.Information, "AddUserRightsToFilterRule " + filterRule.IncludeFileFilterMask + ",userName:" + userName + ",accessFlags:" + accessFlags + " succeeded.");
                                }
                            }
                        }

                    }

                    //Hide the files which match the hidden file filter masks when the user browse the managed directory.
                    string[] hiddenFileFilterMasks = filterRule.HiddenFileFilterMasks.Split(new char[] { ';' });
                    if ((filterRule.AccessFlag & (uint)FilterAPI.AccessFlag.ENABLE_HIDE_FILES_IN_DIRECTORY_BROWSING) > 0 && hiddenFileFilterMasks.Length > 0)
                    {
                        foreach (string hiddenFilterMask in hiddenFileFilterMasks)
                        {
                            if (hiddenFilterMask.Trim().Length > 0)
                            {
                                if (!FilterAPI.AddHiddenFileMaskToFilterRule(filterRule.IncludeFileFilterMask, hiddenFilterMask.Trim()))
                                {
                                    EventManager.WriteMessage(567, "AddHiddenFileMaskToFilterRule", EventLevel.Error, "AddHiddenFileMaskToFilterRule " + filterRule.IncludeFileFilterMask + " hiddenFilterMask:" + hiddenFilterMask + " failed:" + FilterAPI.GetLastErrorMessage());
                                }
                                else
                                {
                                    EventManager.WriteMessage(567, "AddHiddenFileMaskToFilterRule", EventLevel.Information, "AddHiddenFileMaskToFilterRule " + filterRule.IncludeFileFilterMask + " hiddenFilterMask:" + hiddenFilterMask + " succeeded.");
                                }
                            }
                        }

                    }

                    //reparse the file open to another file with the filter mask.
                    //For example:
                    //FilterMask = c:\test\*txt
                    //ReparseFilterMask = d:\reparse\*doc
                    //If you open file c:\test\MyTest.txt, it will reparse to the file d:\reparse\MyTest.doc.

                    string reparseFileFilterMask = filterRule.ReparseFileFilterMasks;
                    if ((filterRule.AccessFlag & (uint)FilterAPI.AccessFlag.ENABLE_REPARSE_FILE_OPEN) > 0 && reparseFileFilterMask.Trim().Length > 0)
                    {
                        if (!FilterAPI.AddReparseFileMaskToFilterRule(filterRule.IncludeFileFilterMask, reparseFileFilterMask.Trim()))
                        {
                            EventManager.WriteMessage(791, "AddReparseFileMaskToFilterRule", EventLevel.Error, "AddReparseFileMaskToFilterRule " + filterRule.IncludeFileFilterMask + " reparseFileFilterMask:" + reparseFileFilterMask + " failed:" + FilterAPI.GetLastErrorMessage());
                        }
                        else
                        {
                            EventManager.WriteMessage(791, "AddReparseFileMaskToFilterRule", EventLevel.Information, "AddReparseFileMaskToFilterRule " + filterRule.IncludeFileFilterMask + " reparseFileFilterMask:" + reparseFileFilterMask + " succeeded.");
                        }

                    }

                }


                //below is the global setting.

                //if you send the include process Id to filter driver, then only the include processes can
                //apply to the filter rules, all other processes will be skipped.
                foreach (uint includedPid in includePidList)
                {
                    FilterAPI.AddIncludedProcessId(includedPid);
                }


                //exclude current process Id.
                //uint currentPid = FilterAPI.GetCurrentProcessId();
                //FilterAPI.AddExcludedProcessId(currentPid);

                //if the exclude process list is not empty, all process in this list will be skipped by filter driver.
                foreach (uint excludedPid in excludePidList)
                {
                    FilterAPI.AddExcludedProcessId(excludedPid);
                }

                FilterAPI.RegisterIoRequest(requestIORegistration);

                foreach (uint protectPid in protectPidList)
                {
                    FilterAPI.AddProtectedProcessId(protectPid);
                }

                FilterAPI.SetBooleanConfig(GlobalConfig.BooleanConfig);
            }
            catch (Exception ex)
            {
                EventManager.WriteMessage(502, "SendConfigSettingsToFilter", EventLevel.Error, "Send config settings to filter failed with error " + ex.Message);
            }

           
        }
    }
}
