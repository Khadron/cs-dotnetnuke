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
using System.ComponentModel;

namespace DotNetNuke.Services.Localization
{
    /// <Summary>
    /// CultureDropDownTypes allows the user to specify which culture name is displayed in the drop down list that is filled
    /// by using one of the helper methods.
    /// </Summary>
    [Serializable()]
    public enum CultureDropDownTypes
    {
        [Description("Displays the culture name in the format &lt;languagefull&gt; (&lt;country/regionfull&gt;) in the .NET Framework language")]
        DisplayName,
        //Displays the culture name in the format "&lt;languagefull&gt; (&lt;country/regionfull&gt;) in English
        EnglishName,
        //Displays the culture identifier
        Lcid,
        //Displays the culture name in the format "&lt;languagecode2&gt; (&lt;country/regioncode2&gt;)
        Name,
        //Displays the culture name in the format "&lt;languagefull&gt; (&lt;country/regionfull&gt;) in the language that the culture is set to display
        NativeName,
        //Displays the IS0 639-1 two letter code
        TwoLetterIsoCode,
        //Displays the ISO 629-2 three letter code "&lt;languagefull&gt; (&lt;country/regionfull&gt;)
        ThreeLetterIsoCode
    }
}