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