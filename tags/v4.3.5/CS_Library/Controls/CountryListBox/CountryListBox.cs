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
using System.ComponentModel;
using System.IO;
using System.Web.Caching;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DotNetNuke.UI.WebControls
{
    /// <summary>
    /// CountryListBox ASP.NET Web Control, lists	countries and automatically detects	country	of
    /// visitors. This web control	will load a	listbox	with all countries and upon loading will
    /// attempt to	automatically recognize	the country that the visitor	is visiting	the	website
    /// from.
    /// </summary>
    [ToolboxData( "<{0}:CountryListBox runat=server></{0}:CountryListBox>" )]
    public class CountryListBox : DropDownList
    {
        private bool _CacheGeoIPData = true;
        private string _TestIP;
        private string _LocalhostCountryCode;
        private string _GeoIPFile;

        [Bindable( true ), Category( "Caching" ), DefaultValue( true )]
        public bool CacheGeoIPData
        {
            get
            {
                return _CacheGeoIPData;
            }

            set
            {
                _CacheGeoIPData = value;
                if( value == false )
                {
                    Context.Cache.Remove( "GeoIPData" );
                }
            }
        }

        [Bindable( true ), Category( "Appearance" ), DefaultValue( "" )]
        public string GeoIPFile
        {
            get
            {
                return _GeoIPFile;
            }
            set
            {
                _GeoIPFile = value;
            }
        }

        [Bindable( true ), Category( "Appearance" ), DefaultValue( "" )]
        public string TestIP
        {
            get
            {
                return _TestIP;
            }
            set
            {
                _TestIP = value;
            }
        }

        [Bindable( true ), Category( "Appearance" ), DefaultValue( "" )]
        public string LocalhostCountryCode
        {
            get
            {
                return _LocalhostCountryCode;
            }
            set
            {
                _LocalhostCountryCode = value;
            }
        }

        protected override void OnDataBinding( EventArgs e )
        {
            bool isLocal = false;

            if( ! Page.IsPostBack )
            {
                //If GeoIPFile is not provided, assume they put it in BIN.
                if( String.IsNullOrEmpty( _GeoIPFile ) )
                {
                    _GeoIPFile = "controls/CountryListBox/Data/GeoIP.dat";
                }

                EnsureChildControls();

                //Check to see if a TestIP is specified
                string ipAddress;
                if( !String.IsNullOrEmpty(_TestIP) )
                {
                    //TestIP is specified, let's use it
                    ipAddress = _TestIP;
                }
                else if( this.Page.Request.UserHostAddress == "127.0.0.1" )
                {
                    //The country cannot be detected because the user is local.
                    isLocal = true;
                    //Set the IP address in case they didn't specify LocalhostCountryCode
                    ipAddress = this.Page.Request.UserHostAddress;
                }
                else
                {
                    //Set the IP address so we can find the country
                    ipAddress = this.Page.Request.UserHostAddress;
                }

                //Check to see if we need to generate the Cache for the GeoIPData file
                if( Context.Cache.Get( "GeoIPData" ) == null && _CacheGeoIPData )
                {
                    //Store it as	well as	setting	a dependency on	the	file
                    Context.Cache.Insert( "GeoIPData", CountryLookup.FileToMemory( Context.Server.MapPath( _GeoIPFile ) ), new CacheDependency( Context.Server.MapPath( _GeoIPFile ) ) );
                }

                //Check to see if the request is a localhost request
                //and see if the LocalhostCountryCode is specified
                if( isLocal && !String.IsNullOrEmpty(_LocalhostCountryCode) )
                {
                    //Bing the data
                    base.OnDataBinding( e );
                    //Pre-Select the value in the drop-down based
                    //on the LocalhostCountryCode specified.
                    if( !Convert.ToBoolean( this.Items.FindByValue( _LocalhostCountryCode ) == null ) )
                    {
                        this.Items.FindByValue( _LocalhostCountryCode ).Selected = true;
                    }
                }
                else
                {
                    //Either this is a remote request or it is a local
                    //request with no LocalhostCountryCode specified
                    CountryLookup _CountryLookup;

                    //Check to see if we are using the Cached
                    //version of the GeoIPData file
                    if( _CacheGeoIPData )
                    {
                        //Yes, get it from cache
                        _CountryLookup = new CountryLookup( (MemoryStream)Context.Cache.Get( "GeoIPData" ) );
                    }
                    else
                    {
                        //No, get it from file
                        _CountryLookup = new CountryLookup( Context.Server.MapPath( _GeoIPFile ) );
                    }

                    //Get the country code based on the IP address
                    string _UserCountryCode = _CountryLookup.LookupCountryCode( ipAddress );

                    //Bind the datasource
                    base.OnDataBinding( e );

                    //Make sure the value returned is actually
                    //in the drop-down list.
                    if( !Convert.ToBoolean( this.Items.FindByValue( _UserCountryCode ) == null ) )
                    {
                        //Yes, it's there, select it based on its value
                        this.Items.FindByValue( _UserCountryCode ).Selected = true;
                    }
                    else
                    {
                        //No it's not there.  Let's get the Country description
                        //and add a new list item for the Country detected
                        string _UserCountry = _CountryLookup.LookupCountryName( ipAddress );
                        if( _UserCountry != "N/A" )
                        {
                            ListItem newItem = new ListItem();
                            newItem.Value = _UserCountryCode;
                            newItem.Text = _UserCountry;
                            this.Items.Insert( 0, newItem );
                            //Now let's Pre-Select it
                            this.Items.FindByValue( _UserCountryCode ).Selected = true;
                        }
                    }
                }
            }
        }
    }
}