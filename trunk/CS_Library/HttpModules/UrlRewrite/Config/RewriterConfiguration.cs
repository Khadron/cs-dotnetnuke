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
using System.IO;
using System.Web.Caching;
using System.Xml.Serialization;
using System.Xml.XPath;
using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;

namespace DotNetNuke.HttpModules.Config
{
    [Serializable(), XmlRoot( "RewriterConfig" )]
    public class RewriterConfiguration
    {
        private RewriterRuleCollection _rules;

        public RewriterRuleCollection Rules
        {
            get
            {
                return _rules;
            }
            set
            {
                _rules = value;
            }
        }

        public static RewriterConfiguration GetConfig()
        {
            RewriterConfiguration Config = new RewriterConfiguration();
            Config.Rules = new RewriterRuleCollection();
            FileStream fileReader = null;
            string filePath = "";
            try
            {
                Config = (RewriterConfiguration)(DataCache.GetCache("RewriterConfig"));

                if (Config == null)
                {
                    filePath = Globals.ApplicationMapPath + "\\SiteUrls.config";

                    if (!(File.Exists(filePath)))
                    {
                        //Copy from \Config
                        if (File.Exists(Globals.ApplicationMapPath + Globals.glbConfigFolder + "SiteUrls.config"))
                        {
                            File.Copy(Globals.ApplicationMapPath + Globals.glbConfigFolder + "SiteUrls.config", Globals.ApplicationMapPath + "\\SiteUrls.config", true);
                        }
                    }

                    //Create a FileStream for the Config file
                    fileReader = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);

                    XPathDocument doc = new XPathDocument(fileReader);
                    Config = new RewriterConfiguration();
                    Config.Rules = new RewriterRuleCollection();

                    foreach (XPathNavigator nav in doc.CreateNavigator().Select("RewriterConfig/Rules/RewriterRule"))
                    {
                        RewriterRule rule = new RewriterRule();
                        rule.LookFor = nav.SelectSingleNode("LookFor").Value;
                        rule.SendTo = nav.SelectSingleNode("SendTo").Value;
                        Config.Rules.Add(rule);
                    }


                    if (File.Exists(filePath))
                    {
                        // Create a dependancy on SiteUrls.config
                        CacheDependency dep = new CacheDependency(filePath);

                        // Set back into Cache
                        DataCache.SetCache("RewriterConfig", Config, dep);
                    }

                }
            }
            catch (Exception ex)
            {
                //log it
                Services.Log.EventLog.EventLogController objEventLog = new Services.Log.EventLog.EventLogController();
                Services.Log.EventLog.LogInfo objEventLogInfo = new Services.Log.EventLog.LogInfo();
                objEventLogInfo.AddProperty("UrlRewriter.RewriterConfiguration", "GetConfig Failed");
                objEventLogInfo.AddProperty("FilePath", filePath);
                objEventLogInfo.AddProperty("ExceptionMessage", ex.Message);
                objEventLogInfo.LogTypeKey = Services.Log.EventLog.EventLogController.EventLogType.HOST_ALERT.ToString();
                objEventLog.AddLog(objEventLogInfo);

            }
            finally
            {
                if (fileReader != null)
                {
                    //Close the Reader
                    fileReader.Close();
                }

            }

            return Config;

        }

        public static void SaveConfig( RewriterRuleCollection rules )
        {
            if( rules != null )
            {
                RewriterConfiguration config = new RewriterConfiguration();
                config.Rules = rules;

                // Create a new Xml Serializer
                XmlSerializer ser = new XmlSerializer( typeof( RewriterConfiguration ) );

                //Create a FileStream for the Config file
                string filePath = Globals.ApplicationMapPath + "\\SiteUrls.config";
                if( File.Exists( filePath ) )
                {
                    // make sure file is not read-only
                    File.SetAttributes( filePath, FileAttributes.Normal );
                }
                FileStream fileWriter = new FileStream( filePath, FileMode.Create, FileAccess.Write, FileShare.Write );

                // Open up the file to serialize
                StreamWriter writer = new StreamWriter( fileWriter );

                // Serialize the RewriterConfiguration
                ser.Serialize( writer, config );

                // Close the Writers
                writer.Close();
                fileWriter.Close();

                // Create a dependancy on SiteUrls.config
                CacheDependency dep = new CacheDependency( filePath );

                // Set Cache
                DataCache.SetCache( "RewriterConfig", config, dep );
            }
        }
    }
}