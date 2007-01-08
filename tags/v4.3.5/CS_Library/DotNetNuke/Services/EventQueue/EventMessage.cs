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
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;

namespace DotNetNuke.Services.EventQueue
{
    public class EventMessage
    {
        private NameValueCollection _attributes;
        private string _authorizedRoles;
        private string _body;
        private string _exceptionMessage;
        private DateTime _expirationDate;
        private string _id;
        private MessagePriority _priority;
        private string _processorType;
        private string _sender;
        private DateTime _sentDate;
        private string _subscribers;

        public NameValueCollection Attributes
        {
            get
            {
                return _attributes;
            }
            set
            {
                _attributes = value;
            }
        }

        public string AuthorizedRoles
        {
            get
            {
                return _authorizedRoles;
            }
            set
            {
                _authorizedRoles = value;
            }
        }

        public string Body
        {
            get
            {
                return _body;
            }
            set
            {
                _body = value;
            }
        }

        public string ExceptionMessage
        {
            get
            {
                return _exceptionMessage;
            }
            set
            {
                _exceptionMessage = value;
            }
        }

        public DateTime ExpirationDate
        {
            get
            {
                return _expirationDate.ToLocalTime();
            }
            set
            {
                _expirationDate = value.ToUniversalTime();
            }
        }

        public string ID
        {
            get
            {
                return _id;
            }
        }

        public MessagePriority Priority
        {
            get
            {
                return _priority;
            }
            set
            {
                _priority = value;
            }
        }

        public string ProcessorType
        {
            get
            {
                return _processorType;
            }
            set
            {
                _processorType = value;
            }
        }

        public string Sender
        {
            get
            {
                return _sender;
            }
            set
            {
                _sender = value;
            }
        }

        public DateTime SentDate
        {
            get
            {
                return _sentDate.ToLocalTime();
            }
            set
            {
                _sentDate = value.ToUniversalTime();
            }
        }

        public string Subscribers
        {
            get
            {
                return _subscribers;
            }
            set
            {
                _subscribers = value;
            }
        }

        public EventMessage()
        {
            _id = Guid.NewGuid().ToString();
            _processorType = "";
            _body = "";
            _priority = MessagePriority.Low;
            _attributes = new NameValueCollection();
            _sender = "";
            _subscribers = "";
            _authorizedRoles = "";
            _exceptionMessage = "";
        }

        private string GetXMLNodeString( string xmlNodeName, string xmlNodeValue )
        {
            StringBuilder xmlNodeString = new StringBuilder( "<" );
            xmlNodeString.Append( xmlNodeName );
            if( xmlNodeValue == "" )
            {
                xmlNodeString.Append( "/>" );
            }
            else if( xmlNodeValue.IndexOfAny( "<'>\"&".ToCharArray() ) > - 1 )
            {
                xmlNodeString.Append( ">" );
                xmlNodeString.Append( "<![CDATA[" );
                xmlNodeString.Append( xmlNodeValue );
                xmlNodeString.Append( "]]></" );
                xmlNodeString.Append( xmlNodeName );
                xmlNodeString.Append( ">" );
            }
            else
            {
                xmlNodeString.Append( ">" );
                xmlNodeString.Append( xmlNodeValue );
                xmlNodeString.Append( "</" );
                xmlNodeString.Append( xmlNodeName );
                xmlNodeString.Append( ">" );
            }
            return xmlNodeString.ToString();
        }

        public string Serialize()
        {
            StringBuilder configXML = new StringBuilder( "<?xml version=\"1.0\" encoding=\"utf-8\"?>" );

            configXML.Append( "\r\n" );
            configXML.Append( "<EventMessage>" );
            configXML.Append( "\r\n" );
            configXML.Append( '\t' );
            configXML.Append( GetXMLNodeString( "ID", _id ) );
            configXML.Append( "\r\n" );
            configXML.Append( '\t' );
            configXML.Append( GetXMLNodeString( "Priority", _priority.ToString() ) );
            configXML.Append( "\r\n" );
            configXML.Append( '\t' );
            configXML.Append( GetXMLNodeString( "ProcessorType", _processorType ) );
            configXML.Append( "\r\n" );
            configXML.Append( '\t' );
            configXML.Append( GetXMLNodeString( "Body", _body ) );
            configXML.Append( "\r\n" );
            configXML.Append( '\t' );
            configXML.Append( GetXMLNodeString( "Sender", _sender ) );
            configXML.Append( "\r\n" );
            configXML.Append( '\t' );
            configXML.Append( GetXMLNodeString( "Subscribers", _subscribers ) );
            configXML.Append( "\r\n" );
            configXML.Append( '\t' );
            configXML.Append( GetXMLNodeString( "AuthorizedRoles", _authorizedRoles ) );
            configXML.Append( "\r\n" );
            configXML.Append( '\t' );
            configXML.Append( GetXMLNodeString( "ExceptionMessage", _exceptionMessage ) );
            configXML.Append( "\r\n" );
            configXML.Append( '\t' );
            configXML.Append( GetXMLNodeString( "SentDate", _sentDate.ToString( "yyyy-MM-ddTHH:mm:ss.fffffffZ", CultureInfo.InvariantCulture ) ) );
            configXML.Append( "\r\n" );
            configXML.Append( '\t' );
            configXML.Append( GetXMLNodeString( "ExpirationDate", _expirationDate.ToString( "yyyy-MM-ddTHH:mm:ss.fffffffZ", CultureInfo.InvariantCulture ) ) );
            configXML.Append( "\r\n" );
            configXML.Append( '\t' );
            configXML.Append( "<Attributes>" );
            foreach( string key in this.Attributes.Keys )
            {
                configXML.Append( "\r\n" );
                configXML.Append( '\t', 2 );
                configXML.Append( "<Attribute>" );
                configXML.Append( "\r\n" );
                configXML.Append( '\t', 3 );
                configXML.Append( GetXMLNodeString( "Name", key ) );
                configXML.Append( "\r\n" );
                configXML.Append( '\t', 3 );
                configXML.Append( GetXMLNodeString( "Value", this.Attributes[key] ) );
                configXML.Append( "\r\n" );
                configXML.Append( '\t', 2 );
                configXML.Append( "</Attribute>" );
            }
            configXML.Append( "\r\n" );
            configXML.Append( '\t' );
            configXML.Append( "</Attributes>" );
            configXML.Append( "\r\n" );
            configXML.Append( "</EventMessage>" );
            return configXML.ToString();
        }

        public void Deserialize( string configXml )
        {
            XmlTextReader oXMLTextReader = new XmlTextReader( new StringReader( configXml ) );
            oXMLTextReader.MoveToContent();
            while( oXMLTextReader.Read() )
            {
                if( oXMLTextReader.NodeType == XmlNodeType.Element )
                {
                    if( oXMLTextReader.Name.ToUpper() == "ID" )
                    {
                        oXMLTextReader.Read();
                        _id = oXMLTextReader.Value;
                    }
                    else if( oXMLTextReader.Name.ToUpper() == "PRIORITY" )
                    {
                        oXMLTextReader.Read();
                        _priority = (MessagePriority)( @Enum.Parse( typeof( MessagePriority ), oXMLTextReader.Value ) );
                    }
                    else if( oXMLTextReader.Name.ToUpper() == "PROCESSORTYPE" )
                    {
                        oXMLTextReader.Read();
                        _processorType = oXMLTextReader.Value;
                    }
                    else if( oXMLTextReader.Name.ToUpper() == "BODY" )
                    {
                        oXMLTextReader.Read();
                        _body = oXMLTextReader.Value;
                    }
                    else if( oXMLTextReader.Name.ToUpper() == "SENDER" )
                    {
                        oXMLTextReader.Read();
                        _sender = oXMLTextReader.Value;
                    }
                    else if( oXMLTextReader.Name.ToUpper() == "SUBSCRIBERS" )
                    {
                        oXMLTextReader.Read();
                        _subscribers = oXMLTextReader.Value;
                    }
                    else if( oXMLTextReader.Name.ToUpper() == "AUTHORIZEDROLES" )
                    {
                        oXMLTextReader.Read();
                        _authorizedRoles = oXMLTextReader.Value;
                    }
                    else if( oXMLTextReader.Name.ToUpper() == "EXCEPTIONMESSAGE" )
                    {
                        oXMLTextReader.Read();
                        _exceptionMessage = oXMLTextReader.Value;
                    }
                    else if( oXMLTextReader.Name.ToUpper() == "EXPIRATIONDATE" )
                    {
                        oXMLTextReader.Read();
                        _expirationDate = DateTime.Parse( oXMLTextReader.Value, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal );
                    }
                    else if( oXMLTextReader.Name.ToUpper() == "SENTDATE" )
                    {
                        oXMLTextReader.Read();
                        _sentDate = DateTime.Parse( oXMLTextReader.Value, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal );
                    }
                    else if( oXMLTextReader.Name.ToUpper() == "ATTRIBUTES" )
                    {
                        //move to the first Attribute node of the EventMessage
                        ReadToFollowing( ref oXMLTextReader, "Attribute" );
                        _attributes = new NameValueCollection();
                        while( oXMLTextReader.Name.ToUpper() != "ATTRIBUTES" )
                        {
                            if( oXMLTextReader.NodeType == XmlNodeType.Element && oXMLTextReader.Name.ToUpper() == "ATTRIBUTE" )
                            {
                                //move to the Name node of the Message Attribute
                                ReadToFollowing( ref oXMLTextReader, "Name" );
                                string attName = oXMLTextReader.ReadString();
                                //move to the Value node of the Message Attribute
                                ReadToFollowing( ref oXMLTextReader, "Value" );
                                _attributes.Add( attName, oXMLTextReader.ReadString() );
                            }
                            //move to the Next Element
                            oXMLTextReader.Read();
                        }
                    }
                }
            }
        }

        //This Private sub is here so that ReadToFollowing can be used in ASP.Net 1.1 and ASP.Net 2.0 with the same code
        private void ReadToFollowing( ref XmlTextReader oXMLTextReader, string NodeName )
        {
            while( oXMLTextReader.Read() )
            {
                if( oXMLTextReader.NodeType == XmlNodeType.Element && oXMLTextReader.Name.ToUpper() == NodeName.ToUpper() )
                {
                    return;
                }
            }
        }
    }
}