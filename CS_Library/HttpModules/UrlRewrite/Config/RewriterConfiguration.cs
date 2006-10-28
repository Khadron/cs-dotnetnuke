using System;
using System.IO;
using System.Web.Caching;
using System.Xml.Serialization;
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
            RewriterConfiguration config = (RewriterConfiguration)DataCache.GetCache( "RewriterConfig" );

            if( config == null )
            {
                string filePath = Globals.ApplicationMapPath + "\\SiteUrls.config";

                if( ! File.Exists( filePath ) )
                {
                    //Copy from \Config
                    if( File.Exists( Globals.ApplicationMapPath + Globals.glbConfigFolder + "SiteUrls.config" ) )
                    {
                        File.Copy( Globals.ApplicationMapPath + Globals.glbConfigFolder + "SiteUrls.config", Globals.ApplicationMapPath + "\\SiteUrls.config", true );
                    }
                }

                // Create a new Xml Serializer
                XmlSerializer ser = new XmlSerializer( typeof( RewriterConfiguration ) );

                //Create a FileStream for the Config file
                FileStream fileReader = new FileStream( filePath, FileMode.Open, FileAccess.Read, FileShare.Read );

                // Open up the file to deserialize
                StreamReader reader = new StreamReader( fileReader );

                // Deserialize into RewriterConfiguration
                config = (RewriterConfiguration)ser.Deserialize( reader );

                // Close the Readers
                reader.Close();
                fileReader.Close();

                if( File.Exists( filePath ) )
                {
                    // Create a dependancy on SiteUrls.config
                    CacheDependency dep = new CacheDependency( filePath );

                    // Set back into Cache
                    DataCache.SetCache( "RewriterConfig", config, dep );
                }
            }

            return config;
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