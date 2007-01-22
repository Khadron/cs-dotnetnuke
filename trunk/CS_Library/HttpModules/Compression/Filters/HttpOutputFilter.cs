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

namespace DotNetNuke.HttpModules.Compression
{
    /// <summary>
    /// The base of anything you want to latch onto the Filter property of a <see cref="System.Web.HttpResponse"/>
    /// object.
    /// </summary>
    /// <remarks>
    /// <p></p>These are generally used with <see cref="HttpModule"/> but you could really use them in
    /// other HttpModules.  This is a general, write-only stream that writes to some underlying stream.  When implementing
    /// a real class, you have to override void Write(byte[], int offset, int count).  Your work will be performed there.
    /// </remarks>
    public abstract class HttpOutputFilter : Stream
    {
        private Stream _sink;

        /// <summary>
        /// Subclasses need to call this on contruction to setup the underlying stream
        /// </summary>
        /// <param name="baseStream">The stream we're wrapping up in a filter</param>
        protected HttpOutputFilter( Stream baseStream )
        {
            _sink = baseStream;
        }

        /// <summary>
        /// Allow subclasses access to the underlying stream
        /// </summary>
        protected Stream BaseStream
        {
            get
            {
                return _sink;
            }
        }

        /// <summary>
        /// False.  These are write-only streams
        /// </summary>
        public override bool CanRead
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// False.  These are write-only streams
        /// </summary>
        public override bool CanSeek
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// True.  You can write to the stream.  May change if you call Close or Dispose
        /// </summary>
        public override bool CanWrite
        {
            get
            {
                return _sink.CanWrite;
            }
        }

        /// <summary>
        /// Not supported.  Throws an exception saying so.
        /// </summary>
        /// <exception cref="NotSupportedException">Thrown.  Always.</exception>
        public override long Length
        {
            get
            {
                throw new NotSupportedException();
            }
        }

        /// <summary>
        /// Not supported.  Throws an exception saying so.
        /// </summary>
        /// <exception cref="NotSupportedException">Thrown.  Always.</exception>
        public override long Position
        {
            get
            {
                throw new NotSupportedException();
            }
            set
            {
                throw new NotSupportedException();
            }
        }

        /// <summary>
        /// Not supported.  Throws an exception saying so.
        /// </summary>
        /// <exception cref="NotSupportedException">Thrown.  Always.</exception>
        public override long Seek( long offset, SeekOrigin direction )
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Not supported.  Throws an exception saying so.
        /// </summary>
        /// <exception cref="NotSupportedException">Thrown.  Always.</exception>
        public override void SetLength( long length )
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Closes this Filter and the underlying stream.
        /// </summary>
        /// <remarks>
        /// If you override, call up to this method in your implementation.
        /// </remarks>
        public override void Close()
        {
            _sink.Close();
        }

        /// <summary>
        /// Fluses this Filter and the underlying stream.
        /// </summary>
        /// <remarks>
        /// If you override, call up to this method in your implementation.
        /// </remarks>
        public override void Flush()
        {
            _sink.Flush();
        }

        /// <summary>
        /// Not supported.
        /// </summary>
        /// <param name="buffer">The buffer to write into.</param>
        /// <param name="offset">The offset on the buffer to write into</param>
        /// <param name="count">The number of bytes to write.  Must be less than buffer.Length</param>
        /// <returns>An int telling you how many bytes were written</returns>
        public override int Read( byte[] buffer, int offset, int count )
        {
            throw new NotSupportedException();
        }
    }
}