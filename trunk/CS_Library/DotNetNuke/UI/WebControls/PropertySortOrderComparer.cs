using System;
using System.Collections;
using System.Reflection;

namespace DotNetNuke.UI.WebControls
{
    public class PropertySortOrderComparer : IComparer
    {
        public virtual int Compare( object x, object y )
        {
            if (x is PropertyInfo && y is PropertyInfo)
            {
                PropertyInfo xProp = (PropertyInfo)x;
                PropertyInfo yProp = (PropertyInfo)y;

                object[] xSortOrder = xProp.GetCustomAttributes(typeof(SortOrderAttribute), true);
                int xSortOrderValue;
                if (xSortOrder.Length > 0)
                {
                    xSortOrderValue = ((SortOrderAttribute)xSortOrder[0]).Order;
                }
                else
                {
                    xSortOrderValue = SortOrderAttribute.DefaultOrder;
                }

                object[] ySortOrder = yProp.GetCustomAttributes(typeof(SortOrderAttribute), true);
                int ySortOrderValue;
                if (ySortOrder.Length > 0)
                {
                    ySortOrderValue = ((SortOrderAttribute)ySortOrder[0]).Order;
                }
                else
                {
                    ySortOrderValue = SortOrderAttribute.DefaultOrder;
                }

                if (xSortOrderValue == ySortOrderValue)
                {
                    return string.Compare(xProp.Name, yProp.Name);
                }
                else
                {
                    return xSortOrderValue - ySortOrderValue;
                }
            }
            else
            {
                throw (new ArgumentException("Object is not of type PropertyInfo"));
            }
        }
    }
}