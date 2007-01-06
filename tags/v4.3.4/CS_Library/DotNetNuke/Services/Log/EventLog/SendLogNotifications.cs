#region DotNetNuke License
// DotNetNuke® - http://www.dotnetnuke.com
// Copyright (c) 2002-2006
// by Perpetual Motion Interactive Systems Inc. ( http://www.perpetualmotion.ca )
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
// documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
// the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
// to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions 
// of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
// TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
// THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
// CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
// DEALINGS IN THE SOFTWARE.
#endregion
using System;
using DotNetNuke.Services.Scheduling;

namespace DotNetNuke.Services.Log.EventLog
{
    public class SendLogNotifications : SchedulerClient
    {
        public SendLogNotifications(ScheduleHistoryItem objScheduleHistoryItem)
        {
            this.ScheduleHistoryItem = objScheduleHistoryItem;
        }

        public override void DoWork()
        {
            try
            {
                //notification that the event is progressing
                this.Progressing(); //OPTIONAL
                LoggingProvider.Instance().SendLogNotifications();
                this.ScheduleHistoryItem.Succeeded = true; //REQUIRED
                this.ScheduleHistoryItem.AddLogNote("Sent log notifications successfully"); //OPTIONAL
            }
            catch (Exception exc) //REQUIRED
            {
                this.ScheduleHistoryItem.Succeeded = false; //REQUIRED
                this.ScheduleHistoryItem.AddLogNote("EXCEPTION: " + exc.ToString()); //OPTIONAL
                this.Errored(ref exc); //REQUIRED
                //log the exception
                Exceptions.Exceptions.LogException(exc); //OPTIONAL
            }
        }
    }
}