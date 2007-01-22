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

namespace DotNetNuke.HttpModules.Compression
{
    /// <summary>
    /// The available compression algorithms to use with the HttpCompressionModule
    /// </summary>
    public enum Algorithms
    {
        /// <summary>Use the Deflate algorithm</summary>
        Deflate = 2,
        /// <summary>Use the GZip algorithm</summary>
        GZip = 1,
        /// <summary>No compression</summary>
        None = 0,
        /// <summary>Use the default algorithm (picked by client)</summary>
        Default = -1
    }

    /// <summary>
    /// The level of compression to use with deflate
    /// </summary>
    public enum CompressionLevels
    {
        /// <summary>Use the default compression level</summary>
        Default = -1,
        /// <summary>The highest level of compression.  Also the slowest.</summary>
        Highest = 9,
        /// <summary>A higher level of compression.</summary>
        Higher = 8,
        /// <summary>A high level of compression.</summary>
        High = 7,
        /// <summary>More compression.</summary>
        More = 6,
        /// <summary>Normal compression.</summary>
        Normal = 5,
        /// <summary>Less than normal compression.</summary>
        Less = 4,
        /// <summary>A low level of compression.</summary>
        Low = 3,
        /// <summary>A lower level of compression.</summary>
        Lower = 2,
        /// <summary>The lowest level of compression that still performs compression.</summary>
        Lowest = 1,
        /// <summary>No compression.  Use this is you are quite silly.</summary>
        None = 0
    }
}