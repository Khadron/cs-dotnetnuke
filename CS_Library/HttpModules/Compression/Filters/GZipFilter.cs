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

using System.IO;
using System.IO.Compression;

namespace DotNetNuke.HttpModules.Compression
{
    /// <summary>
    /// This is a little filter to support HTTP compression using GZip
    /// </summary>
    public class GZipFilter : CompressingFilter
    {
        /// <summary>
        /// compression stream member
        /// has to be a member as we can only have one instance of the
        /// actual filter class
        /// </summary>
        private GZipStream m_stream = null;

        /// <summary>
        /// Primary constructor.  Need to pass in a stream to wrap up with gzip.
        /// </summary>
        /// <param name="baseStream">The stream to wrap in gzip.  Must have CanWrite.</param>
        public GZipFilter( Stream baseStream ) : base( baseStream, CompressionLevels.Normal )
        {
            m_stream = new GZipStream( baseStream, CompressionMode.Compress );
        }

        /// <summary>
        /// Write content to the stream and have it compressed using gzip.
        /// </summary>
        /// <param name="buffer">The bytes to write</param>
        /// <param name="offset">The offset into the buffer to start reading bytes</param>
        /// <param name="count">The number of bytes to write</param>
        public override void Write( byte[] buffer, int offset, int count )
        {
            if( !HasWrittenHeaders )
            {
                WriteHeaders();
            }
            m_stream.Write( buffer, offset, count );
        }

        /// <summary>
        /// The Http name of this encoding.  Here, gzip.
        /// </summary>
        public override string ContentEncoding
        {
            get
            {
                return "gzip";
            }
        }

        /// <summary>
        /// Closes this Filter and calls the base class implementation.
        /// </summary>
        public override void Close()
        {
            m_stream.Close(); // this will close the gzip stream along with the underlying stream
            // no need for call to base.Close() here.
        }

        /// <summary>
        /// Flushes the stream out to underlying storage
        /// </summary>
        public override void Flush()
        {
            m_stream.Flush();
        }
    }
}