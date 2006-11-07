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
using System.Collections;


namespace DotNetNuke.UI.Utilities
{
    /// <Summary>
    /// Event arguments passed to a delegate associated to a client postback event
    /// </Summary>
    public class ClientAPIPostBackEventArgs
    {
        public Hashtable EventArguments;
        public string EventName;

        public ClientAPIPostBackEventArgs( string strEventArgument )
        {
            int i1;
            this.EventArguments = new Hashtable();
            string[] splitter = { ClientAPI.COLUMN_DELIMITER };
            string[] stringArray1 = strEventArgument.Split(splitter, StringSplitOptions.None);              
            if( stringArray1.Length > 0 )
            {
                this.EventName = stringArray1[0];
            }
            int i2 = ( stringArray1.Length - 1 );
            for( i1 = 1; ( i1 <= i2 ); i1 += 2 )
            {
                this.EventArguments.Add( stringArray1[i1], stringArray1[( i1 + 1 )] );
            }
        }

        public ClientAPIPostBackEventArgs()
        {
            this.EventArguments = new Hashtable();
        }
    }
}