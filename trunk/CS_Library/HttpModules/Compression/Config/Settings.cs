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
using System.Collections.Specialized;
using System.IO;
using System.Text.RegularExpressions;
using System.Web.Caching;
using System.Xml.XPath;
using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Host;

namespace DotNetNuke.HttpModules.Compression
{
    /// <summary>
    /// This class encapsulates the settings for an HttpCompressionModule
    /// </summary>
    public sealed class Settings
    {
        private Algorithms _preferredAlgorithm;
        private CompressionLevels _compressionLevel;
        private StringCollection _excludedTypes;
        private StringCollection _excludedPaths;
        private Regex _reg;
        private bool _whitespace;

        private Settings()
        {
            _preferredAlgorithm = Algorithms.None;
            _compressionLevel = CompressionLevels.None;
            _excludedTypes = new StringCollection();
            _excludedPaths = new StringCollection();
            _whitespace = false;
        }

        /// <summary>
        /// The preferred compression level
        /// </summary>
        public CompressionLevels CompressionLevel
        {
            get
            {
                return _compressionLevel;
            }
        }

        /// <summary>
        /// The default settings.  Deflate + normal.
        /// </summary>
        public static Settings Default
        {
            get
            {
                return new Settings();
            }
        }

        /// <summary>
        /// The preferred algorithm to use for compression
        /// </summary>
        public Algorithms PreferredAlgorithm
        {
            get
            {
                return _preferredAlgorithm;
            }
        }

        /// <summary>
        /// The regular expression used for Whitespace removal
        /// </summary>
        public Regex Reg
        {
            get
            {
                return _reg;
            }
        }

        /// <summary>
        /// Determines if Whitespace filtering is enabled
        /// </summary>
        public bool Whitespace
        {
            get
            {
                return _whitespace;
            }
        }

        /// <summary>
        /// Get the current settings from the xml config file
        /// </summary>
        public static Settings GetSettings()
        {
            Settings settings = (Settings)DataCache.GetCache( "CompressionConfig" );

            if( settings == null )
            {
                settings = Default;

                //Place this in a try/catch as during install the host settings will not exist
                try
                {
                    if( !String.IsNullOrEmpty( HostSettings.GetHostSetting( "HttpCompression" ) ) )
                    {
                        settings._preferredAlgorithm = (Algorithms)( Convert.ToInt32( HostSettings.GetHostSetting( "HttpCompression" ) ) );
                    }
                    if( !String.IsNullOrEmpty( HostSettings.GetHostSetting( "HttpCompressionLevel" ) ) )
                    {
                        settings._compressionLevel = (CompressionLevels)( Convert.ToInt32( HostSettings.GetHostSetting( "HttpCompressionLevel" ) ) );
                    }
                    if( !String.IsNullOrEmpty( HostSettings.GetHostSetting( "WhitespaceFilter" ) ) )
                    {
                        settings._whitespace = HostSettings.GetHostSetting( "WhitespaceFilter" ) == "Y" ? true : false;
                    }
                }
                catch( Exception e )
                {
                }

                string filePath = Globals.ApplicationMapPath + "\\Compression.config";

                if( !File.Exists( filePath ) )
                {
                    //Copy from \Config
                    if( File.Exists( Globals.ApplicationMapPath + Globals.glbConfigFolder + "Compression.config" ) )
                    {
                        File.Copy( Globals.ApplicationMapPath + Globals.glbConfigFolder + "Compression.config", Globals.ApplicationMapPath + "\\Compression.config", true );
                    }
                }

                //Create a FileStream for the Config file
                FileStream fileReader = new FileStream( filePath, FileMode.Open, FileAccess.Read, FileShare.Read );

                XPathDocument doc = new XPathDocument( fileReader );

                settings._reg = new Regex( doc.CreateNavigator().SelectSingleNode( "compression/whitespace" ).Value );

                foreach( XPathNavigator nav in doc.CreateNavigator().Select( "compression/excludedMimeTypes/mimeType" ) )
                {
                    settings._excludedTypes.Add( nav.Value.ToLower() );
                }
                foreach( XPathNavigator nav in doc.CreateNavigator().Select( "compression/excludedPaths/path" ) )
                {
                    settings._excludedPaths.Add( nav.Value.ToLower() );
                }

                if( File.Exists( filePath ) )
                {
                    //Create a dependancy on SiteUrls.config
                    CacheDependency dep = new CacheDependency( filePath );

                    //Set back into Cache
                    DataCache.SetCache( "CompressionConfig", settings, dep );
                }
            }

            return settings;
        }

        /// <summary>
        /// Checks a given mime type to determine if it has been excluded from compression
        /// </summary>
        /// <param name="mimetype">The MimeType to check.  Can include wildcards like image/* or */xml.</param>
        /// <returns>true if the mime type passed in is excluded from compression, false otherwise</returns>
        public bool IsExcludedMimeType( string mimetype )
        {
            return _excludedTypes.Contains( mimetype.ToLower() );
        }

        /// <summary>
        /// Looks for a given path in the list of paths excluded from compression
        /// </summary>
        /// <param name="relUrl">the relative url to check</param>
        /// <returns>true if excluded, false if not</returns>
        public bool IsExcludedPath( string relUrl )
        {
            return _excludedPaths.Contains( relUrl.ToLower() );
        }
    }
}