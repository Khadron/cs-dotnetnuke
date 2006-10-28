using System;
using System.Collections;

namespace DotNetNuke.UI.WebControls
{
    public class SettingNameComparer : IComparer
    {
        public virtual int Compare( object x, object y )
        {
            if( ( x is SettingInfo ) && ( y is SettingInfo ) )
            {
                return string.Compare( ( (SettingInfo)x ).Name, ( (SettingInfo)y ).Name );
            }
            throw new ArgumentException( "Object is not of type SettingInfo" );
        }
    }
}