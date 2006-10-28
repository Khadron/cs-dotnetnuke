using System;
using System.Collections;
using System.Reflection;

namespace DotNetNuke.UI.WebControls
{
    public class PropertyNameComparer : IComparer
    {
        public virtual int Compare( object x, object y )
        {
            if( ( x is PropertyInfo ) && ( y is PropertyInfo ) )
            {
                return string.Compare( ( (PropertyInfo)x ).Name, ( (PropertyInfo)y ).Name );
            }
            throw new ArgumentException( "Object is not of type PropertyInfo" );
        }
    }
}