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
    /// Summary description for DeflateFilter.
    /// </summary>
    public class DeflateFilter : CompressingFilter
    {
        /// <summary>
        /// compression stream member
        /// has to be a member as we can only have one instance of the
        /// actual filter class
        /// </summary>
        private DeflateStream m_stream = null;

        /// <summary>
        /// Basic constructor that uses the Normal compression level
        /// </summary>
        /// <param name="baseStream">The stream to wrap up with the deflate algorithm</param>
        public DeflateFilter( Stream baseStream ) : this( baseStream, CompressionLevels.Normal )
        {
        }

        /// <summary>
        /// Full constructor that allows you to set the wrapped stream and the level of compression
        /// </summary>
        /// <param name="baseStream">The stream to wrap up with the deflate algorithm</param>
        /// <param name="compressionLevel">The level of compression to use</param>
        public DeflateFilter( Stream baseStream, CompressionLevels compressionLevel ) : base( baseStream, compressionLevel )
        {
            m_stream = new DeflateStream( baseStream, CompressionMode.Compress );
        }

        /// <summary>
        /// Write out bytes to the underlying stream after compressing them using deflate
        /// </summary>
        /// <param name="buffer">The array of bytes to write</param>
        /// <param name="offset">The offset into the supplied buffer to start</param>
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
        /// Return the Http name for this encoding.  Here, deflate.
        /// </summary>
        public override string ContentEncoding
        {
            get
            {
                return "deflate";
            }
        }

        /// <summary>
        /// Closes this Filter and calls the base class implementation.
        /// </summary>
        public override void Close()
        {
            m_stream.Close();
        }

        /// <summary>
        /// Flushes that the filter out to underlying storage
        /// </summary>
        public override void Flush()
        {
            m_stream.Flush();
        }
    }
}