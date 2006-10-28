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