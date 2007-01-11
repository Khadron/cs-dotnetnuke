#region DotNetNuke License
// DotNetNuke® - http://www.dotnetnuke.com
// Copyright (c) 2002-2006
// by DotNetNuke Corporation
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
using System.Collections;
using DotNetNuke.Common;
using DotNetNuke.Services.Scheduling;

namespace DotNetNuke.Services.Cache.BroadcastPollingCachingProvider
{
    public class Poll : SchedulerClient
    {
        public Poll( ScheduleHistoryItem objScheduleHistoryItem )
        {
            this.ScheduleHistoryItem = objScheduleHistoryItem; //REQUIRED
        }

        public override void DoWork()
        {
            try
            {
                Controller c = new Controller();
                ArrayList arr = c.GetBroadcasts( Globals.ServerName );
                int i;
                for( i = 0; i <= arr.Count - 1; i++ )
                {
                    BroadcastInfo objBroadcastInfo;
                    objBroadcastInfo = (BroadcastInfo)arr[i];
                    switch( objBroadcastInfo.BroadcastType )
                    {
                        case "RemoveCachedItem":

                            CachingProvider.Instance().Remove( objBroadcastInfo.BroadcastMessage );
                            break;
                    }
                    this.ScheduleHistoryItem.AddLogNote( "Broadcast Type: " + objBroadcastInfo.BroadcastType + ", Broadcast Message: " + objBroadcastInfo.BroadcastMessage + '\r' );
                }
                this.ScheduleHistoryItem.Succeeded = true; //REQUIRED
            }
            catch( Exception exc ) //REQUIRED
            {
                this.ScheduleHistoryItem.Succeeded = false; //REQUIRED

                this.ScheduleHistoryItem.AddLogNote( string.Format( "An error ocurred polling for server broadcasts.", exc.ToString() ) ); //OPTIONAL

                //notification that we have errored
                this.Errored( ref exc ); //REQUIRED

                //log the exception
                Exceptions.Exceptions.LogException( exc ); //OPTIONAL
            }
        }
    }
}