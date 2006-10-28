using System;
using System.IO;
using System.Net;
using Microsoft.VisualBasic;

namespace DotNetNuke.UI.WebControls
{
    /// <summary>
    /// This class uses an IP lookup database from MaxMind, specifically
    /// the GeoIP Free Database.    
    /// The database and the c# implementation of this class
    /// are available from http://www.maxmind.com/app/csharp
    /// </summary>
    public class CountryLookup
    {
        public MemoryStream m_MemoryStream;

        private static long CountryBegin = 16776960;
        private static string[] CountryName = new string[] {"N/A", "Asia/Pacific Region", "Europe", "Andorra", "United Arab Emirates", "Afghanistan", "Antigua and Barbuda", "Anguilla", "Albania", "Armenia", "Netherlands Antilles", "Angola", "Antarctica", "Argentina", "American Samoa", "Austria", "Australia", "Aruba", "Azerbaijan", "Bosnia and Herzegovina", "Barbados", "Bangladesh", "Belgium", "Burkina Faso", "Bulgaria", "Bahrain", "Burundi", "Benin", "Bermuda", "Brunei Darussalam", "Bolivia", "Brazil", "Bahamas", "Bhutan", "Bouvet Island", "Botswana", "Belarus", "Belize", "Canada", "Cocos (Keeling) Islands", "Congo, The Democratic Republic of the", "Central African Republic", "Congo", "Switzerland", "Cote D\'Ivoire", "Cook Islands", "Chile", "Cameroon", "China", "Colombia", "Costa Rica", "Cuba", "Cape Verde", "Christmas Island", "Cyprus", "Czech Republic", "Germany", "Djibouti", "Denmark", "Dominica", "Dominican Republic", "Algeria", "Ecuador", "Estonia", "Egypt", "Western Sahara", "Eritrea", "Spain", "Ethiopia", "Finland", "Fiji", "Falkland Islands (Malvinas)", "Micronesia, Federated States of", "Faroe Islands", "France", "France, Metropolitan", "Gabon", "United Kingdom", "Grenada", "Georgia", "French Guiana", "Ghana", "Gibraltar", "Greenland", "Gambia", "Guinea", "Guadeloupe", "Equatorial Guinea", "Greece", "South Georgia and the South Sandwich Islands", "Guatemala", "Guam", "Guinea-Bissau", "Guyana", "Hong Kong", "Heard Island and McDonald Islands", "Honduras", "Croatia", "Haiti", "Hungary", "Indonesia", "Ireland", "Israel", "India", "British Indian Ocean Territory", "Iraq", "Iran, Islamic Republic of", "Iceland", "Italy", "Jamaica", "Jordan", "Japan", "Kenya", "Kyrgyzstan", "Cambodia", "Kiribati", "Comoros", "Saint Kitts and Nevis", "Korea, Democratic People\'s Republic of", "Korea, Republic of", "Kuwait", "Cayman Islands", "Kazakstan", "Lao People\'s Democratic Republic", "Lebanon", "Saint Lucia", "Liechtenstein", "Sri Lanka", "Liberia", "Lesotho", "Lithuania", "Luxembourg", "Latvia", "Libyan Arab Jamahiriya", "Morocco", "Monaco", "Moldova, Republic of", "Madagascar", "Marshall Islands", "Macedonia, the Former Yugoslav Republic of", "Mali", "Myanmar", "Mongolia", "Macau", "Northern Mariana Islands", "Martinique", "Mauritania", "Montserrat", "Malta", "Mauritius", "Maldives", "Malawi", "Mexico", "Malaysia", "Mozambique", "Namibia", "New Caledonia", "Niger", "Norfolk Island", "Nigeria", "Nicaragua", "Netherlands", "Norway", "Nepal", "Nauru", "Niue", "New Zealand", "Oman", "Panama", "Peru", "French Polynesia", "Papua New Guinea", "Philippines", "Pakistan", "Poland", "Saint Pierre and Miquelon", "Pitcairn", "Puerto Rico", "Palestinian Territory, Occupied", "Portugal", "Palau", "Paraguay", "Qatar", "Reunion", "Romania", "Russian Federation", "Rwanda", "Saudi Arabia", "Solomon Islands", "Seychelles", "Sudan", "Sweden", "Singapore", "Saint Helena", "Slovenia", "Svalbard and Jan Mayen", "Slovakia", "Sierra Leone", "San Marino", "Senegal", "Somalia", "Suriname", "Sao Tome and Principe", "El Salvador", "Syrian Arab Republic", "Swaziland", "Turks and Caicos Islands", "Chad", "French Southern Territories", "Togo", "Thailand", "Tajikistan", "Tokelau", "Turkmenistan", "Tunisia", "Tonga", "East Timor", "Turkey", "Trinidad and Tobago", "Tuvalu", "Taiwan, Province of China", "Tanzania, United Republic of", "Ukraine", "Uganda", "United States Minor Outlying Islands", "United States", "Uruguay", "Uzbekistan", "Holy See (Vatican City State)", "Saint Vincent and the Grenadines", "Venezuela", "Virgin Islands, British", "Virgin Islands, U.S.", "Vietnam", "Vanuatu", "Wallis and Futuna", "Samoa", "Yemen", "Mayotte", "Yugoslavia", "South Africa", "Zambia", "Zaire", "Zimbabwe", "Anonymous Proxy", "Satellite Provider"};
        private static string[] CountryCode = new string[] {"--", "AP", "EU", "AD", "AE", "AF", "AG", "AI", "AL", "AM", "AN", "AO", "AQ", "AR", "AS", "AT", "AU", "AW", "AZ", "BA", "BB", "BD", "BE", "BF", "BG", "BH", "BI", "BJ", "BM", "BN", "BO", "BR", "BS", "BT", "BV", "BW", "BY", "BZ", "CA", "CC", "CD", "CF", "CG", "CH", "CI", "CK", "CL", "CM", "CN", "CO", "CR", "CU", "CV", "CX", "CY", "CZ", "DE", "DJ", "DK", "DM", "DO", "DZ", "EC", "EE", "EG", "EH", "ER", "ES", "ET", "FI", "FJ", "FK", "FM", "FO", "FR", "FX", "GA", "GB", "GD", "GE", "GF", "GH", "GI", "GL", "GM", "GN", "GP", "GQ", "GR", "GS", "GT", "GU", "GW", "GY", "HK", "HM", "HN", "HR", "HT", "HU", "ID", "IE", "IL", "IN", "IO", "IQ", "IR", "IS", "IT", "JM", "JO", "JP", "KE", "KG", "KH", "KI", "KM", "KN", "KP", "KR", "KW", "KY", "KZ", "LA", "LB", "LC", "LI", "LK", "LR", "LS", "LT", "LU", "LV", "LY", "MA", "MC", "MD", "MG", "MH", "MK", "ML", "MM", "MN", "MO", "MP", "MQ", "MR", "MS", "MT", "MU", "MV", "MW", "MX", "MY", "MZ", "NA", "NC", "NE", "NF", "NG", "NI", "NL", "NO", "NP", "NR", "NU", "NZ", "OM", "PA", "PE", "PF", "PG", "PH", "PK", "PL", "PM", "PN", "PR", "PS", "PT", "PW", "PY", "QA", "RE", "RO", "RU", "RW", "SA", "SB", "SC", "SD", "SE", "SG", "SH", "SI", "SJ", "SK", "SL", "SM", "SN", "SO", "SR", "ST", "SV", "SY", "SZ", "TC", "TD", "TF", "TG", "TH", "TJ", "TK", "TM", "TN", "TO", "TP", "TR", "TT", "TV", "TW", "TZ", "UA", "UG", "UM", "US", "UY", "UZ", "VA", "VC", "VE", "VG", "VI", "VN", "VU", "WF", "WS", "YE", "YT", "YU", "ZA", "ZM", "ZR", "ZW", "A1", "A2"};

        public CountryLookup( MemoryStream ms )
        {
            m_MemoryStream = ms;
        }

        public CountryLookup( string FileLocation )
        {
            // Load the passed in GeoIP Data file to the memorystream

            FileStream _FileStream = new FileStream( FileLocation, FileMode.Open, FileAccess.Read );
            m_MemoryStream = new MemoryStream();
            byte[] _Byte = new byte[257];
            while( _FileStream.Read( _Byte, 0, _Byte.Length ) != 0 )
            {
                m_MemoryStream.Write( _Byte, 0, _Byte.Length );
            }
            _FileStream.Close();
        }

        private long ConvertIPAddressToNumber( IPAddress _IPAddress )
        {
            // Convert an IP Address, (e.g. 127.0.0.1), to the numeric equivalent

            string[] _Address = _IPAddress.ToString().Split( '.' );

            if( ( _Address.Length - 1 ) == 3 )
            {
                return ( (long)( 16777216*Convert.ToDouble( _Address[0] ) + 65536*Convert.ToDouble( _Address[1] ) + 256*Convert.ToDouble( _Address[2] ) + Convert.ToDouble( _Address[3] ) ) );
            }
            else
            {
                return 0;
            }
        }

        public static MemoryStream FileToMemory( string FileLocation )
        {
            // Read a given file into a Memory Stream to return as the result

            FileStream _FileStream;
            MemoryStream _MemStream = new MemoryStream();
            byte[] _Byte = new byte[257];
            try
            {
                _FileStream = new FileStream( FileLocation, FileMode.Open, FileAccess.Read );
                while( _FileStream.Read( _Byte, 0, _Byte.Length ) != 0 )
                {
                    _MemStream.Write( _Byte, 0, _Byte.Length );
                }
                _FileStream.Close();
            }
            catch( FileNotFoundException )
            {
               // Information.Err().Raise( 9999, "FileToMemory", Information.Err().Description + "  Please set the \"GeoIPFile\" Property to specify the location of this file.  The property value must be set to the virtual path to GeoIP.dat (i.e. \"/controls/CountryListBox/Data/GeoIP.dat\")", null, null );
            }
            return _MemStream;
        }

        public string LookupCountryCode( IPAddress _IPAddress )
        {
            // Look up the country code, e.g. US, for the passed in IP Address

            short temp_short = 31;
            object temp_object = ConvertIPAddressToNumber( _IPAddress );
            long temp_long = (long)temp_object;
            return CountryCode[Convert.ToInt32( SeekCountry( 0, ref temp_long, ref temp_short ) )];
        }

        public string LookupCountryCode( string _IPAddress )
        {
            // Look up the country code, e.g. US, for the passed in IP Address

            IPAddress _Address;

            try
            {
                _Address = IPAddress.Parse( _IPAddress );
            }
            catch( FormatException )
            {
                return "--";
            }
            return LookupCountryCode( _Address );
        }

        public string LookupCountryName( IPAddress addr )
        {
            // Look up the country name, e.g. United States, for the IP Address

            short temp_short = 31;
            object temp_object = ConvertIPAddressToNumber( addr );
            long temp_long = (long)temp_object;
            return CountryName[Convert.ToInt32( SeekCountry( 0, ref temp_long, ref temp_short ) )];
        }

        public string LookupCountryName( string _IPAddress )
        {
            // Look up the country name, e.g. United States, for the IP Address

            IPAddress _Address;
            try
            {
                _Address = IPAddress.Parse( _IPAddress );
            }
            catch( FormatException )
            {
                return "N/A";
            }
            return LookupCountryName( _Address );
        }

        private long vbShiftLeft( long Value, int Count )
        {
            long returnValue;

            // Replacement for Bitwise operators which are missing in VB.NET,
            // these functions are present in .NET 1.1, but for developers
            // using 1.0, replacement functions must be implemented

            int _Iterator;

            returnValue = Value;
            for( _Iterator = 1; _Iterator <= Count; _Iterator++ )
            {
                returnValue = returnValue*2;
            }
            return returnValue;
        }

        private long vbShiftRight( long Value, int Count )
        {
            long returnValue;

            // Replacement for Bitwise operators which are missing in VB.NET,
            // these functions are present in .NET 1.1, but for developers
            // using 1.0, replacement functions must be implemented

            int _Iterator;

            returnValue = Value;
            for( _Iterator = 1; _Iterator <= Count; _Iterator++ )
            {
                returnValue = returnValue/2;
            }
            return returnValue;
        }

        public int SeekCountry( int Offset, ref long Ipnum, ref short Depth )
        {
            int returnValue = 0;
            try
            {
                byte[] Buffer = new byte[7];
                int[] X = new int[3];
                short I;
                short J;
                byte Y;

                if( Depth == 0 )
                {
                    throw ( new Exception() );
                }

                m_MemoryStream.Seek( 6*Offset, 0 );
                m_MemoryStream.Read( Buffer, 0, 6 );

                for( I = 0; I <= 1; I++ )
                {
                    X[I] = 0;

                    for( J = 0; J <= 2; J++ )
                    {
                        Y = Buffer[I*3 + J];
                        if( Y < 0 )
                        {
                            Y = ( (byte)( ( (int)Y ) + 256 ) );
                        }
                        X[I] = Convert.ToInt32( X[I] + vbShiftLeft( Y, J*8 ) );
                    }
                }

                if( ( Ipnum & vbShiftLeft( 1, Depth ) ) > 0 )
                {
                    if( X[1] >= CountryBegin )
                    {
                        returnValue = Convert.ToInt32( X[1] - CountryBegin );
                        return returnValue;
                    }

                    short temp_short = ( (short)( Depth - 1 ) );
                    object temp_object = Ipnum;
                    long temp_long = (long)temp_object;
                    returnValue = SeekCountry( X[1], ref temp_long, ref temp_short );
                    return returnValue;
                }
                else
                {
                    if( X[0] >= CountryBegin )
                    {
                        returnValue = Convert.ToInt32( X[0] - CountryBegin );
                        return returnValue;
                    }

                    short temp_short2 = (short)( Depth - 1 );
                    object temp_object2 = Ipnum;
                    long temp_long2 = (long)temp_object2;
                    returnValue = SeekCountry( X[0], ref temp_long2, ref temp_short2 );
                    return returnValue;
                }
            }
            catch( Exception exc )
            {
                //Information.Err().Raise( 9999, "CountryLookup.SeekCountry", "Error seeking country: " + exc.ToString(), null, null );
            }
            return returnValue;
        }
    }
}