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