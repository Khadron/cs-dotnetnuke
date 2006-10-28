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