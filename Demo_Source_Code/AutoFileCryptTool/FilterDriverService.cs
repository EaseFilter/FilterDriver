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
using System.Runtime.InteropServices;

using EaseFilter.CommonObjects;

namespace AutoFileCryptTool
{
    public class FilterDriverService
    {
        public static bool StartFilterService(out string lastError)
        {
            //Purchase a license key with the link: http://www.easefilter.com/Order.htm
        //Email us to request a trial key: info@easefilter.com //free email is not accepted.
            string registerKey = GlobalConfig.registerKey;

            bool ret = false;
            try
            {
               
                lastError = string.Empty;

                ret = FilterAPI.StartFilter((int)GlobalConfig.FilterConnectionThreads
                                            , registerKey
                                            , new FilterAPI.FilterDelegate(FilterCallback)
                                            , new FilterAPI.DisconnectDelegate(DisconnectCallback)
                                            , ref lastError);
                if (!ret)
                {
                    EventManager.WriteMessage(30, "StartFilter", EventLevel.Error, "Start filter service failed with error " + lastError);
                    return ret;
                }


                GlobalConfig.EnableDefaultIVKey = true;
                GlobalConfig.SendConfigSettingsToFilter();

                EventManager.WriteMessage(102, "StartFilter", EventLevel.Information, "Start filter service succeeded.");
            }
            catch (Exception ex)
            {
                lastError = ex.Message;
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

        static Boolean FilterCallback(IntPtr sendDataPtr, IntPtr replyDataPtr)
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
            EventManager.WriteMessage(82, "DisconnectCallback", EventLevel.Information, "filter service is disconnected.");
        }

    }
}
