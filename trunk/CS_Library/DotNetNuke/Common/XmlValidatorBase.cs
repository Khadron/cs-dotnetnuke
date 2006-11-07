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
using System.Collections;
using System.IO;
using System.Xml;
using System.Xml.Schema;

namespace DotNetNuke.Common
{
    public class XmlValidatorBase
    {
        private ArrayList _errs = new ArrayList();
        private XmlTextReader _reader;
        private XmlSchemaSet _schemaSet = new XmlSchemaSet();

        public ArrayList Errors
        {
            get
            {
                return _errs;
            }
            set
            {
                _errs = value;
            }
        }

        public XmlSchemaSet SchemaSet
        {
            get
            {
                return _schemaSet;
            }
        }

        protected void ValidationCallBack( object sender, ValidationEventArgs args )
        {
            _errs.Add( args.Message );
        }

        public bool IsValid()
        {
            //There is a bug here which I haven't been able to fix.
            //If the XML Instance does not include a reference to the
            //schema, then the validation fails.  If the reference exists
            //the the validation works correctly.

            //Create a validating reader
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.Schemas = _schemaSet;
            settings.ValidationType = ValidationType.Schema;

            //Set the validation event handler.
            settings.ValidationEventHandler += new ValidationEventHandler( new ValidationEventHandler( ValidationCallBack ) );
            XmlReader vreader = XmlReader.Create( _reader, settings );

            //Read and validate the XML data.
            while( vreader.Read() )
            {
            }

            //Close the reader.
            vreader.Close();

            return ( _errs.Count == 0 );
        }

        public virtual bool Validate( Stream xmlStream )
        {
            xmlStream.Seek( 0, SeekOrigin.Begin );
            _reader = new XmlTextReader( xmlStream );

            return IsValid();
        }

        public virtual bool Validate( string filename )
        {
            _reader = new XmlTextReader( filename );

            return IsValid();
        }
    }
}