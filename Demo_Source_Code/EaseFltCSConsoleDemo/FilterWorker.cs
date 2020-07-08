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
using System.ServiceProcess;
using System.Management;

using EaseFilter.CommonObjects;


namespace EaseFltCSConsoleDemo
{
    public class FilterWorker
    {

        public static bool StartService()
        {
            //Purchase a license key with the link: http://www.easefilter.com/Order.htm
        //Email us to request a trial key: info@easefilter.com //free email is not accepted.
          string registerKey = GlobalConfig.registerKey;

            bool ret = false;

            try
            {
                string lastError = string.Empty;

                EventManager.WriteMessage(37, "StartFilter", EventLevel.Information, "Starting filter service.....");

                ret = FilterAPI.StartFilter( (int)GlobalConfig.FilterConnectionThreads
                                            , registerKey
                                            , new FilterAPI.FilterDelegate(FilterCallback)
                                            , new FilterAPI.DisconnectDelegate(DisconnectCallback)
                                            , ref lastError);
                if (!ret)
                {
                    EventManager.WriteMessage(43, "StartFilter", EventLevel.Error, "Start filter service failed with error " + lastError);
                    return ret;
                }

                GlobalConfig.SendConfigSettingsToFilter();
                EventManager.WriteMessage(102, "StartFilter", EventLevel.Information, "Start filter service succeeded.");
            }
            catch (Exception ex)
            {
                EventManager.WriteMessage(104, "StartFilter", EventLevel.Error, "Start filter service failed with error " + ex.Message);
            }

            return ret;
        }

     

        public static bool StopService()
        {
            FilterAPI.StopFilter();
            GlobalConfig.Stop();

            return true;
        }


        public static Boolean FilterCallback(IntPtr sendDataPtr, IntPtr replyDataPtr)
        {
            Boolean ret = true;

            try
            {
                FilterAPI.MessageSendData messageSend = new FilterAPI.MessageSendData();
                messageSend = (FilterAPI.MessageSendData)Marshal.PtrToStructure(sendDataPtr, typeof(FilterAPI.MessageSendData));

                if (FilterAPI.MESSAGE_SEND_VERIFICATION_NUMBER != messageSend.VerificationNumber)
                {
                    EventManager.WriteMessage(139, "FilterCallback", EventLevel.Error, "Received message corrupted.Please check if the MessageSendData structure is correct.");
                    return false;
                }

                MessageInfo.DisplayFilterMessage(messageSend);

                if (replyDataPtr.ToInt64() != 0)
                {

                    FilterAPI.MessageReplyData messageReply = (FilterAPI.MessageReplyData)Marshal.PtrToStructure(replyDataPtr, typeof(FilterAPI.MessageReplyData));

                    //here you can control the IO behaviour and modify the data.
                    FilterService.IOAccessControl(messageSend, ref messageReply);

                    messageReply.MessageId = messageSend.MessageId;
                    messageReply.MessageType = messageSend.MessageType;
                    messageReply.ReturnStatus = (uint)FilterAPI.NTSTATUS.STATUS_SUCCESS;


                    //to comple the PRE_IO
                    //messageReply.ReturnStatus = (uint)FilterAPI.NTSTATUS.STATUS_ACCESS_DENIED;
                    //messageReply.FilterStatus = (uint)FilterAPI.FilterStatus.FILTER_COMPLETE_PRE_OPERATION;

                    Marshal.StructureToPtr(messageReply, replyDataPtr, true);
                }


                return ret;
            }
            catch (Exception ex)
            {
                EventManager.WriteMessage(134, "FilterCallback", EventLevel.Error, "filter callback exception." + ex.Message);
                return false;
            }

        }

        static void DisconnectCallback()
        {
            EventManager.WriteMessage(697, "DisconnectCallback", EventLevel.Information, "Filter Disconnected." + FilterAPI.GetLastErrorMessage());
        }


    }

}
