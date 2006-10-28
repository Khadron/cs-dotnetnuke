using System;
using System.IO;
using DotNetNuke.Common;
using DotNetNuke.Framework;
using DotNetNuke.Security;
using DotNetNuke.Services.EventQueue.Config;
using DotNetNuke.Services.Log.EventLog;

namespace DotNetNuke.Services.EventQueue
{
    public class EventQueueController
    {
        private string m_messagePath;

        public EventQueueController()
        {
            m_messagePath = Globals.HostMapPath + "EventQueue\\";
        }

        private EventMessage DeserializeMessage( string filePath, string subscriberId )
        {
            EventMessage message = new EventMessage();
            StreamReader oStreamReader = File.OpenText( filePath );
            string messageString = oStreamReader.ReadToEnd();
            if( messageString.IndexOf( "EventMessage" ) < 0 )
            {
                PortalSecurity oPortalSecurity = new PortalSecurity();
                messageString = oPortalSecurity.Decrypt( EventQueueConfiguration.GetConfig().EventQueueSubscribers[subscriberId].PrivateKey, messageString );
            }
            message.Deserialize( messageString );
            oStreamReader.Close();

            //remove the persisted message from the queue if it has expired
            if( message.ExpirationDate < DateTime.Now )
            {
                File.Delete( filePath );
            }

            return message;
        }

        public EventMessage GetMessage( string eventName, string subscriberId, string messageId )
        {
            return DeserializeMessage( m_messagePath + MessageName( eventName, subscriberId, messageId ), subscriberId );
        }

        public EventMessageCollection GetMessages( string eventName )
        {
            EventMessageCollection oEventMessageCollection = new EventMessageCollection();
            string[] EventMessageFiles;
            string EventMessageFile;
            string[] subscribers = new string[0];
            if( EventQueueConfiguration.GetConfig().PublishedEvents[eventName] != null )
            {
                subscribers = EventQueueConfiguration.GetConfig().PublishedEvents[eventName].Subscribers.Split( ";".ToCharArray() );
            }
            else
            {
                subscribers[0] = "";
            }
            for( int indx = 0; indx <= subscribers.Length - 1; indx++ )
            {
                EventMessageFiles = Directory.GetFiles( m_messagePath, MessageName( eventName, subscribers[indx], "*" ) );
                foreach( string tempLoopVar_EventMessageFile in EventMessageFiles )
                {
                    EventMessageFile = tempLoopVar_EventMessageFile;
                    oEventMessageCollection.Add( DeserializeMessage( EventMessageFile, subscribers[indx] ) );
                }
            }
            return oEventMessageCollection;
        }

        public EventMessageCollection GetMessages( string eventName, string subscriberId )
        {
            EventMessageCollection oEventMessageCollection = new EventMessageCollection();
            string[] EventMessageFiles;
            string EventMessageFile;
            EventMessageFiles = Directory.GetFiles( m_messagePath, MessageName( eventName, subscriberId, "*" ) );
            foreach( string tempLoopVar_EventMessageFile in EventMessageFiles )
            {
                EventMessageFile = tempLoopVar_EventMessageFile;
                oEventMessageCollection.Add( DeserializeMessage( EventMessageFile, subscriberId ) );
            }
            return oEventMessageCollection;
        }

        private string MessageName( string eventName, string subscriberId, string messageId )
        {
            return eventName + "-" + subscriberId + "-" + messageId + ".config";
        }

        public bool ProcessMessages( string eventName )
        {
            return ProcessMessages( GetMessages( eventName ) );
        }

        public bool ProcessMessages( string eventName, string subscriberId )
        {
            return ProcessMessages( GetMessages( eventName, subscriberId ) );
        }

        public bool ProcessMessages( EventMessageCollection eventMessages )
        {
            EventMessage message;
            foreach( EventMessage tempLoopVar_message in eventMessages )
            {
                message = tempLoopVar_message;
                try
                {
                    object oMessageProcessor = Reflection.CreateObject( message.ProcessorType, message.ProcessorType );
                    if( !( (EventMessageProcessorBase)oMessageProcessor ).ProcessMessage( message ) )
                    {
                        throw new Exception();
                    }
                }
                catch
                {
                    //log if message could not be processed
                    EventLogController objEventLog = new EventLogController();
                    LogInfo objEventLogInfo = new LogInfo();
                    objEventLogInfo.AddProperty( "EventQueue.ProcessMessage", "Message Processing Failed" );
                    objEventLogInfo.AddProperty( "ProcessorType", message.ProcessorType );
                    objEventLogInfo.AddProperty( "Body", message.Body );
                    objEventLogInfo.AddProperty( "Sender", message.Sender );
                    foreach( string key in message.Attributes.Keys )
                    {
                        objEventLogInfo.AddProperty( key, message.Attributes[key] );
                    }
                    if( message.ExceptionMessage.Length > 0 )
                    {
                        objEventLogInfo.AddProperty( "ExceptionMessage", message.ExceptionMessage );
                    }
                    objEventLogInfo.LogTypeKey = EventLogController.EventLogType.HOST_ALERT.ToString();
                    objEventLog.AddLog( objEventLogInfo );
                }
            }
            return true;
        }

        public bool SendMessage( EventMessage message, string eventName )
        {
            return SendMessage( message, eventName, false );
        }

        public bool SendMessage( EventMessage message, string eventName, bool encryptMessage )
        {
            //set the sent date if it wasn't set by the sender
            if( message.SentDate == DateTime.MinValue )
            {
                message.SentDate = DateTime.Now;
            }

            string[] subscribers = new string[0];
            if( EventQueueConfiguration.GetConfig().PublishedEvents[eventName] != null )
            {
                subscribers = EventQueueConfiguration.GetConfig().PublishedEvents[eventName].Subscribers.Split( ";".ToCharArray() );
            }
            else
            {
                subscribers[0] = "";
            }
            //send a message for each subscriber of the specified event
            for( int indx = 0; indx <= subscribers.Length - 1; indx++ )
            {
                StreamWriter oStream = File.CreateText( m_messagePath + MessageName( eventName, subscribers[indx], message.ID ) );
                string messageString = message.Serialize();
                if( encryptMessage )
                {
                    PortalSecurity oPortalSecurity = new PortalSecurity();
                    messageString = oPortalSecurity.Encrypt( EventQueueConfiguration.GetConfig().EventQueueSubscribers[subscribers[indx]].PrivateKey, messageString );
                }
                oStream.WriteLine( messageString );
                oStream.Close();
            }

            return true;
        }
    }
}