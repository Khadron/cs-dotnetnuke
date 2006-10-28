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