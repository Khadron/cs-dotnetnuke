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
using ICSharpCode.SharpZipLib.Zip;

namespace DotNetNuke.Modules.Admin.ResourceInstaller
{
    public class PaFile : ResourceInstallerBase
    {
        private byte[] _Buffer;
        private PaTextEncoding _Encoding;
        private string _Name;
        private string _Path;
        private PaFileType _Type;

        public byte[] Buffer
        {
            get
            {
                return _Buffer;
            }
        }

        public PaTextEncoding Encoding
        {
            get
            {
                return _Encoding;
            }
        }

        public string Extension
        {
            get
            {
                string ext = System.IO.Path.GetExtension( _Name );

                if( ext == null || ext.Length == 0 )
                {
                    return "";
                }
                else
                {
                    return ext.Substring( 1 );
                }
            }
        }

        public string FullName
        {
            get
            {
                return System.IO.Path.Combine( _Path, _Name );
            }
        }

        public string Name
        {
            get
            {
                return _Name;
            }
        }

        public string Path
        {
            get
            {
                return _Path;
            }
            set
            {
                _Path = value.Trim( '/', '\\' );
            }
        }

        public PaFileType Type
        {
            get
            {
                return _Type;
            }
        }

        public PaFile( ZipInputStream unzip, ZipEntry entry )
        {
            string s = entry.Name;
            int i = s.LastIndexOf( "/" );
            if( i < 0 )
            {
                _Name = s.Substring( 0, s.Length );
                _Path = "";
            }
            else
            {
                _Name = s.Substring( i + 1, s.Length - ( i + 1 ) );
                _Path = s.Substring( 0, i );
            }

            _Buffer = new byte[Convert.ToInt32( entry.Size ) - 1 + 1];
            int size = 0;
            while( size < _Buffer.Length )
            {
                size += unzip.Read( _Buffer, size, _Buffer.Length - size );
            }
            if( size != _Buffer.Length )
            {
                throw ( new Exception( EXCEPTION_FileRead + _Buffer.Length + "/" + size ) );
            }
            string ext = Extension;

            _Encoding = PaTextEncoding.Unknown;

            switch( ext.ToLower() )
            {
                case "dnn":

                    _Type = PaFileType.Dnn;
                    break;
                case "dll":

                    _Type = PaFileType.Dll;
                    break;
                case "ascx":

                    _Type = PaFileType.Ascx;
                    break;
                case "sql":

                    _Type = PaFileType.Sql;
                    _Encoding = GetTextEncodingType( _Buffer );
                    break;
                default:

                    if( ext.ToLower().EndsWith( "dataprovider" ) )
                    {
                        _Type = PaFileType.DataProvider;
                        _Encoding = GetTextEncodingType( _Buffer );
                    }
                    else
                    {
                        _Type = PaFileType.Other;
                    }
                    break;
            }
        } 

        private PaTextEncoding GetTextEncodingType( byte[] Buffer )
        {
            //UTF7 = No byte higher than 127
            //UTF8 = first three bytes EF BB BF
            //UTF16BigEndian = first two bytes FE FF
            //UTF16LittleEndian = first two bytes FF FE

            //Lets do the easy ones first
            if( Buffer[0] == 255 && Buffer[1] == 254 )
            {
                return PaTextEncoding.UTF16LittleEndian;
            }
            if( Buffer[0] == 254 && Buffer[1] == 255 )
            {
                return PaTextEncoding.UTF16BigEndian;
            }
            if( Buffer[0] == 239 && Buffer[1] == 187 && Buffer[2] == 191 )
            {
                return PaTextEncoding.UTF8;
            }

            //This does a simple test to verify that there are no bytes with a value larger than 127
            //which would be invalid in UTF-7 encoding
            int i;
            for( i = 0; i <= 100; i++ )
            {
                if( Buffer[i] > 127 )
                {
                    return PaTextEncoding.Unknown;
                }
            }
            return PaTextEncoding.UTF7;
        }
    }
}