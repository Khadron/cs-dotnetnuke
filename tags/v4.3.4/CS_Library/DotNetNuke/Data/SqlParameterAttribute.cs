#region DotNetNuke License
// DotNetNuke® - http://www.dotnetnuke.com
// Copyright (c) 2002-2006
// by Perpetual Motion Interactive Systems Inc. ( http://www.perpetualmotion.ca )
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
using System.Data;

namespace DotNetNuke.Data
{
    [AttributeUsage( AttributeTargets.Parameter )]
    public class SqlParameterAttribute : Attribute
    {
        private string _name;
        private bool _paramTypeDefined;
        private SqlDbType _paramType;
        private int _size;
        private byte _precision;
        private byte _scale;
        private bool _directionDefined;
        private ParameterDirection _direction;

        public SqlParameterAttribute( string name, SqlDbType paramType, int size, byte precision, byte scale, ParameterDirection direction )
        {
            _name = name;
            _paramType = paramType;
            if( paramType == null )
            {
                _paramTypeDefined = false;
            }
            else
            {
                _paramTypeDefined = true;
            }
            _size = size;
            _precision = precision;
            _scale = scale;
            _direction = direction;
            if( direction == null )
            {
                _directionDefined = false;
            }
            else
            {
                _directionDefined = true;
            }
        } //New

        public string Name
        {
            get
            {
                if( _name == null )
                {
                    return string.Empty;
                }
                else
                {
                    return _name;
                }
            }

            set
            {
                _name = value;
            }
        }

        public int Size
        {
            get
            {
                return _size;
            }

            set
            {
                _size = value;
            }
        }

        public byte Precision
        {
            get
            {
                return _precision;
            }

            set
            {
                _precision = value;
            }
        }

        public byte Scale
        {
            get
            {
                return _scale;
            }

            set
            {
                _scale = value;
            }
        }

        public ParameterDirection Direction
        {
            get
            {
                return _direction;
            }

            set
            {
                _direction = value;
            }
        }

        public SqlDbType SqlDbType
        {
            get
            {
                return _paramType;
            }

            set
            {
                _paramType = value;
            }
        }

        public bool IsNameDefined
        {
            get
            {
                bool returnValue;
                returnValue = true;
                if( _name == null )
                {
                    returnValue = false;
                }
                else
                {
                    if( _name.Length == 0 )
                    {
                        returnValue = false;
                    }
                }
                return returnValue;
            }
        }

        public bool IsSizeDefined
        {
            get
            {
                return _size != 0;
            }
        }

        public bool IsTypeDefined
        {
            get
            {
                return _paramTypeDefined;
            }
        }

        public bool IsDirectionDefined
        {
            get
            {
                return _directionDefined;
            }
        }

        public bool IsScaleDefined
        {
            get
            {
                return _scale != 0;
            }
        }

        public bool IsPrecisionDefined
        {
            get
            {
                return _precision != 0;
            }
        }
    }
}