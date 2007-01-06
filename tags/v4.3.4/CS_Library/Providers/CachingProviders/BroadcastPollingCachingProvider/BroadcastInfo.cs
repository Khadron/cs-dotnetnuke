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

namespace DotNetNuke.Services.Cache.BroadcastPollingCachingProvider
{
    public class BroadcastInfo
    {
        private int _BroadcastID;
        private int _SourceServerID;
        private string _SourceServerName;
        private string _BroadcastType;
        private string _BroadcastMessage;
        private DateTime _BroadcastDate;

        public int BroadcastID
        {
            get
            {
                return _BroadcastID;
            }
            set
            {
                _BroadcastID = value;
            }
        }

        public int SourceServerID
        {
            get
            {
                return _SourceServerID;
            }
            set
            {
                _SourceServerID = value;
            }
        }

        public string SourceServerName
        {
            get
            {
                return _SourceServerName;
            }
            set
            {
                _SourceServerName = value;
            }
        }

        public string BroadcastType
        {
            get
            {
                return _BroadcastType;
            }
            set
            {
                _BroadcastType = value;
            }
        }

        public string BroadcastMessage
        {
            get
            {
                return _BroadcastMessage;
            }
            set
            {
                _BroadcastMessage = value;
            }
        }

        public DateTime BroadcastDate
        {
            get
            {
                return _BroadcastDate;
            }
            set
            {
                _BroadcastDate = value;
            }
        }
    }
}