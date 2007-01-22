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
using System.Net;

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

        private static long COUNTRY_BEGIN = 16776960;
        private static string[] countryCode = 
								{ "--","AP","EU","AD","AE","AF","AG","AI","AL","AM","AN","AO","AQ","AR","AS","AT","AU","AW","AZ","BA","BB","BD","BE","BF","BG","BH","BI","BJ","BM","BN","BO","BR","BS","BT","BV","BW","BY","BZ","CA","CC","CD","CF","CG","CH","CI","CK","CL","CM","CN","CO","CR","CU","CV","CX","CY","CZ","DE","DJ","DK","DM","DO","DZ",
									"EC","EE","EG","EH","ER","ES","ET","FI","FJ","FK","FM","FO","FR","FX","GA","GB","GD","GE","GF","GH","GI","GL","GM","GN","GP","GQ","GR","GS","GT","GU","GW","GY","HK","HM","HN","HR","HT","HU","ID","IE","IL","IN","IO","IQ","IR","IS","IT","JM","JO","JP","KE","KG","KH","KI","KM","KN","KP","KR","KW","KY","KZ",
									"LA","LB","LC","LI","LK","LR","LS","LT","LU","LV","LY","MA","MC","MD","MG","MH","MK","ML","MM","MN","MO","MP","MQ","MR","MS","MT","MU","MV","MW","MX","MY","MZ","NA","NC","NE","NF","NG","NI","NL","NO","NP","NR","NU","NZ","OM","PA","PE","PF","PG","PH","PK","PL","PM","PN","PR","PS","PT","PW","PY","QA",
									"RE","RO","RU","RW","SA","SB","SC","SD","SE","SG","SH","SI","SJ","SK","SL","SM","SN","SO","SR","ST","SV","SY","SZ","TC","TD","TF","TG","TH","TJ","TK","TM","TN","TO","TP","TR","TT","TV","TW","TZ","UA","UG","UM","US","UY","UZ","VA","VC","VE","VG","VI","VN","VU","WF","WS","YE","YT","CS","ZA","ZM","ZR","ZW","A1","A2"};
        private static string[] countryName = 
								{"N/A","Asia/Pacific Region","Europe","Andorra","United Arab Emirates","Afghanistan","Antigua and Barbuda","Anguilla","Albania","Armenia","Netherlands Antilles","Angola","Antarctica","Argentina","American Samoa","Austria","Australia","Aruba","Azerbaijan","Bosnia and Herzegovina","Barbados","Bangladesh","Belgium",
									"Burkina Faso","Bulgaria","Bahrain","Burundi","Benin","Bermuda","Brunei Darussalam","Bolivia","Brazil","Bahamas","Bhutan","Bouvet Island","Botswana","Belarus","Belize","Canada","Cocos (Keeling) Islands","Congo, The Democratic Republic of the","Central African Republic","Congo","Switzerland","Cote D'Ivoire",
									"Cook Islands","Chile","Cameroon","China","Colombia","Costa Rica","Cuba","Cape Verde","Christmas Island","Cyprus","Czech Republic","Germany","Djibouti","Denmark","Dominica","Dominican Republic","Algeria","Ecuador","Estonia","Egypt","Western Sahara","Eritrea","Spain","Ethiopia","Finland","Fiji","Falkland Islands (Malvinas)",
									"Micronesia, Federated States of","Faroe Islands","France","France, Metropolitan","Gabon","United Kingdom","Grenada","Georgia","French Guiana","Ghana","Gibraltar","Greenland","Gambia","Guinea","Guadeloupe","Equatorial Guinea","Greece","South Georgia and the South Sandwich Islands","Guatemala","Guam","Guinea-Bissau","Guyana",
									"Hong Kong","Heard Island and McDonald Islands","Honduras","Croatia","Haiti","Hungary","Indonesia","Ireland","Israel","India","British Indian Ocean Territory","Iraq","Iran, Islamic Republic of","Iceland","Italy","Jamaica","Jordan","Japan","Kenya","Kyrgyzstan","Cambodia","Kiribati","Comoros","Saint Kitts and Nevis",
									"Korea, Democratic People's Republic of","Korea, Republic of","Kuwait","Cayman Islands","Kazakstan","Lao People's Democratic Republic","Lebanon","Saint Lucia","Liechtenstein","Sri Lanka","Liberia","Lesotho","Lithuania","Luxembourg","Latvia","Libyan Arab Jamahiriya","Morocco","Monaco","Moldova, Republic of","Madagascar",
									"Marshall Islands","Macedonia","Mali","Myanmar","Mongolia","Macau","Northern Mariana Islands","Martinique","Mauritania","Montserrat","Malta","Mauritius","Maldives","Malawi","Mexico","Malaysia","Mozambique","Namibia","New Caledonia","Niger","Norfolk Island","Nigeria","Nicaragua","Netherlands",
									"Norway","Nepal","Nauru","Niue","New Zealand","Oman","Panama","Peru","French Polynesia","Papua New Guinea","Philippines","Pakistan","Poland","Saint Pierre and Miquelon","Pitcairn Islands","Puerto Rico","Palestinian Territory","Portugal","Palau","Paraguay","Qatar","Reunion","Romania","Russian Federation","Rwanda","Saudi Arabia",
									"Solomon Islands","Seychelles","Sudan","Sweden","Singapore","Saint Helena","Slovenia","Svalbard and Jan Mayen","Slovakia","Sierra Leone","San Marino","Senegal","Somalia","Suriname","Sao Tome and Principe","El Salvador","Syrian Arab Republic","Swaziland","Turks and Caicos Islands","Chad","French Southern Territories","Togo",
									"Thailand","Tajikistan","Tokelau","Turkmenistan","Tunisia","Tonga","East Timor","Turkey","Trinidad and Tobago","Tuvalu","Taiwan","Tanzania, United Republic of","Ukraine","Uganda","United States Minor Outlying Islands","United States","Uruguay","Uzbekistan","Holy See (Vatican City State)","Saint Vincent and the Grenadines",
									"Venezuela","Virgin Islands, British","Virgin Islands, U.S.","Vietnam","Vanuatu","Wallis and Futuna","Samoa","Yemen","Mayotte","Serbia and Montenegro","South Africa","Zambia","Zaire","Zimbabwe","Anonymous Proxy","Satellite Provider"};
		
	
        public CountryLookup( MemoryStream ms )
        {
            m_MemoryStream = ms;
        }

        public CountryLookup( string fileLocation )
        {
            // Load the passed in GeoIP Data file to the memorystream

            FileStream fileStream = new FileStream( fileLocation, FileMode.Open, FileAccess.Read );
            m_MemoryStream = new MemoryStream();
            byte[] buffer = new byte[257];
            while( fileStream.Read( buffer, 0, buffer.Length ) != 0 )
            {
                m_MemoryStream.Write( buffer, 0, buffer.Length );
            }
            fileStream.Close();
        }

        private static long ConvertIPAddressToNumber( IPAddress addr )
        {
            // Convert an IP Address, (e.g. 127.0.0.1), to the numeric equivalent

            string[] address = addr.ToString().Split( '.' );

            if( ( address.Length - 1 ) == 3 )
            {
                return ( (long)( 16777216*Convert.ToDouble( address[0] ) + 65536*Convert.ToDouble( address[1] ) + 256*Convert.ToDouble( address[2] ) + Convert.ToDouble( address[3] ) ) );
            }
            else
            {
                return 0;
            }

//            long ipnum = 0;
//            byte[] b = BitConverter.GetBytes(addr.Address);
//            for (int i = 0; i < 4; ++i)
//            {
//                long y = b[i];
//                if (y < 0)
//                {
//                    y += 256;
//                }
//                ipnum += y << ((3 - i) * 8);
//            }
//            Console.WriteLine(ipnum);
//            return ipnum;
        }

        public static MemoryStream FileToMemory( string fileLocation )
        {
            // Read a given file into a Memory Stream to return as the result

            MemoryStream memStream = new MemoryStream();
            byte[] buffer = new byte[257];
            try
            {
                FileStream fileStream = new FileStream( fileLocation, FileMode.Open, FileAccess.Read );
                while( fileStream.Read( buffer, 0, buffer.Length ) != 0 )
                {
                    memStream.Write( buffer, 0, buffer.Length );
                }
                fileStream.Close();
            }
            catch( FileNotFoundException )
            {
               // Information.Err().Raise( 9999, "FileToMemory", Information.Err().Description + "  Please set the \"GeoIPFile\" Property to specify the location of this file.  The property value must be set to the virtual path to GeoIP.dat (i.e. \"/controls/CountryListBox/Data/GeoIP.dat\")", null, null );
            }
            return memStream;
        }

        public string LookupCountryCode( IPAddress addr )
        {
            // Look up the country code, e.g. US, for the passed in IP Address

            //short temp_short = 31;
            //long temp_long = ConvertIPAddressToNumber( addr );            
            //return countryCode[Convert.ToInt32( SeekCountry( 0, ref temp_long, ref temp_short ) )];               
            return (countryCode[(int)SeekCountry(0, ConvertIPAddressToNumber(addr), 31)]);
        }

        public string LookupCountryCode( string ipAddress )
        {
            // Look up the country code, e.g. US, for the passed in IP Address

            IPAddress addr;
            try
            {
                addr = IPAddress.Parse(ipAddress);
            }
            catch (FormatException )
            {
                return "--";
            }
            return LookupCountryCode(addr);
        }

        public string LookupCountryName( IPAddress addr )
        {
            // Look up the country name, e.g. United States, for the IP Address

            //short temp_short = 31;
            //object temp_object = ConvertIPAddressToNumber( addr );
            //long temp_long = (long)temp_object;
            //return countryName[Convert.ToInt32( SeekCountry( 0, temp_long, temp_short ) )];
            return (countryName[(int)SeekCountry(0, ConvertIPAddressToNumber(addr), 31)]);
        }

        public string LookupCountryName( string _IPAddress )
        {
            // Look up the country name, e.g. United States, for the IP Address

            IPAddress ipAddress;
            try
            {
                ipAddress = IPAddress.Parse( _IPAddress );
            }
            catch( FormatException )
            {
                return "N/A";
            }
            return LookupCountryName( ipAddress );
        }

        private long SeekCountry(long offset, long ipnum, int depth)
        {
            byte[] buf = new byte[6];
            long[] x = new long[2];
            if (depth == 0)
            {
                Console.WriteLine("Error seeking country.");
            }
            try
            {
                m_MemoryStream.Seek(6 * offset, 0);
                m_MemoryStream.Read(buf, 0, 6);
            }
            catch (IOException )
            {
                Console.WriteLine("IO Exception");
            }
            for (int i = 0; i < 2; i++)
            {
                x[i] = 0;
                for (int j = 0; j < 3; j++)
                {
                    int y = buf[i * 3 + j];
                    if (y < 0)
                    {
                        y += 256;
                    }
                    x[i] += (y << (j * 8));
                }
            }

            if ((ipnum & (1 << depth)) > 0)
            {
                if (x[1] >= COUNTRY_BEGIN)
                {
                    return x[1] - COUNTRY_BEGIN;
                }
                return SeekCountry(x[1], ipnum, depth - 1);
            }
            else
            {
                if (x[0] >= COUNTRY_BEGIN)
                {
                    return x[0] - COUNTRY_BEGIN;
                }
                return SeekCountry(x[0], ipnum, depth - 1);
            }
        }
        
    }
}