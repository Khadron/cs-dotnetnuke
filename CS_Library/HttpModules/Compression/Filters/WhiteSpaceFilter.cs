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
using System.Text;
using System.Text.RegularExpressions;

namespace DotNetNuke.HttpModules.Compression
{
    internal class WhitespaceFilter : HttpOutputFilter
    {
        //private static Regex reg = new Regex(@"(?<=[^])\t{2,}|(?<=[>])\s{2,}(?=[<])|(?<=[>])\s{2,11}(?=[<])|(?=[\n])\s{2,}");
        private static Regex _reg;

        /// <summary>
        /// Primary constructor.
        /// </summary>
        /// <param name="baseStream">The stream to wrap in gzip.  Must have CanWrite.</param>
        /// <param name="reg"></param>
        public WhitespaceFilter( Stream baseStream, Regex reg ) : base( baseStream )
        {
            _reg = reg;
        }

        public override void Write( byte[] buffer, int offset, int count )
        {
            byte[] data = new byte[count];
            Buffer.BlockCopy( buffer, offset, data, 0, count );
            string html = Encoding.Default.GetString( buffer );

            html = _reg.Replace( html, string.Empty );

            byte[] outdata = Encoding.Default.GetBytes( html );
            BaseStream.Write( outdata, 0, outdata.GetLength( 0 ) );
        }
    }
}