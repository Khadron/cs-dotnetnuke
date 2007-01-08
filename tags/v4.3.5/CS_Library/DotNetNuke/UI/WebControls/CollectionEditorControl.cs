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
using System.ComponentModel;
using System.Reflection;
using System.Web.UI;
using System.Web.UI.WebControls;
using DotNetNuke.Common.Utilities;

namespace DotNetNuke.UI.WebControls
{
    /// <Summary>
    /// The CollectionEditorControl control provides a Control to display Collection
    /// Properties.
    /// </Summary>
    [ToolboxData( "<{0}:CollectionEditorControl runat=server></{0}:CollectionEditorControl>" )]
    public class CollectionEditorControl : PropertyEditorControl
    {
        private string _CategoryDataField;
        private string _EditorDataField;
        private string _LengthDataField;
        private string _NameDataField;
        private string _RequiredDataField;
        private string _TypeDataField;
        private IEnumerable _UnderlyingDataSource;
        private string _ValidationExpressionDataField;
        private string _ValueDataField;
        private string _VisibilityDataField;
        private string _VisibleDataField;

        /// <Summary>Gets and sets the value of the Category</Summary>
        [Description( "Enter the name of the field that is data bound to the Category." ), CategoryAttribute( "Data" ), BrowsableAttribute( true ), DefaultValueAttribute( "" )]
        public string CategoryDataField
        {
            get
            {
                return this._CategoryDataField;
            }
            set
            {
                this._CategoryDataField = value;
            }
        }

        /// <Summary>Gets and sets the value of the Editor Type to use</Summary>
        [BrowsableAttribute( true ), DefaultValueAttribute( "" ), CategoryAttribute( "Data" ), DescriptionAttribute( "Enter the name of the field that is data bound to the Editor Type." )]
        public string EditorDataField
        {
            get
            {
                return this._EditorDataField;
            }
            set
            {
                this._EditorDataField = value;
            }
        }

        /// <Summary>
        /// Gets and sets the value of the Field that determines the length
        /// </Summary>
        [BrowsableAttribute( true ), CategoryAttribute( "Data" ), DescriptionAttribute( "Enter the name of the field that determines the length." ), DefaultValueAttribute( "" )]
        public string LengthDataField
        {
            get
            {
                return this._LengthDataField;
            }
            set
            {
                this._LengthDataField = value;
            }
        }

        /// <Summary>
        /// Gets and sets the value of the Field that is bound to the Label
        /// </Summary>
        [BrowsableAttribute( true ), DescriptionAttribute( "Enter the name of the field that is data bound to the Label's Text property." ), DefaultValueAttribute( "" ), CategoryAttribute( "Data" )]
        public string NameDataField
        {
            get
            {
                return this._NameDataField;
            }
            set
            {
                this._NameDataField = value;
            }
        }

        /// <Summary>
        /// Gets and sets the value of the Field that determines whether an item is required
        /// </Summary>
        [DefaultValueAttribute( "" ), BrowsableAttribute( true ), DescriptionAttribute( "Enter the name of the field that determines whether an item is required." ), CategoryAttribute( "Data" )]
        public string RequiredDataField
        {
            get
            {
                return this._RequiredDataField;
            }
            set
            {
                this._RequiredDataField = value;
            }
        }

        /// <Summary>
        /// Gets and sets the value of the Field that is bound to the EditControl
        /// </Summary>
        [BrowsableAttribute( true ), CategoryAttribute( "Data" ), DescriptionAttribute( "Enter the name of the field that is data bound to the EditControl's Type." ), DefaultValueAttribute( "" )]
        public string TypeDataField
        {
            get
            {
                return this._TypeDataField;
            }
            set
            {
                this._TypeDataField = value;
            }
        }

        /// <Summary>Gets the Underlying DataSource</Summary>
        protected override IEnumerable UnderlyingDataSource
        {
            get
            {
                if (_UnderlyingDataSource == null)
                {
                    _UnderlyingDataSource = (IEnumerable)DataSource;
                }
                return _UnderlyingDataSource;
            }
        }

        /// <Summary>
        /// Gets and sets the value of the Field that is bound to the EditControl's
        /// Expression Validator
        /// </Summary>
        [DefaultValueAttribute( "" ), DescriptionAttribute( "Enter the name of the field that is data bound to the EditControl's Expression Validator." ), CategoryAttribute( "Data" ), BrowsableAttribute( true )]
        public string ValidationExpressionDataField
        {
            get
            {
                return this._ValidationExpressionDataField;
            }
            set
            {
                this._ValidationExpressionDataField = value;
            }
        }

        /// <Summary>
        /// Gets and sets the value of the Field that is bound to the EditControl
        /// </Summary>
        [DefaultValueAttribute( "" ), BrowsableAttribute( true ), CategoryAttribute( "Data" ), DescriptionAttribute( "Enter the name of the field that is data bound to the EditControl's Value property." )]
        public string ValueDataField
        {
            get
            {
                return this._ValueDataField;
            }
            set
            {
                this._ValueDataField = value;
            }
        }

        /// <Summary>
        /// Gets and sets the value of the Field that determines the visibility
        /// </Summary>
        [BrowsableAttribute( true ), CategoryAttribute( "Data" ), DescriptionAttribute( "Enter the name of the field that determines the visibility." ), DefaultValueAttribute( "" )]
        public string VisibilityDataField
        {
            get
            {
                return this._VisibilityDataField;
            }
            set
            {
                this._VisibilityDataField = value;
            }
        }

        /// <Summary>
        /// Gets and sets the value of the Field that determines whether the control is visible
        /// </Summary>
        [CategoryAttribute( "Data" ), BrowsableAttribute( true ), DefaultValueAttribute( "" ), DescriptionAttribute( "Enter the name of the field that determines whether the item is visble." )]
        public string VisibleDataField
        {
            get
            {
                return this._VisibleDataField;
            }
            set
            {
                this._VisibleDataField = value;
            }
        }

        /// <Summary>GetCategory gets the Category of an object</Summary>
        protected override string GetCategory( object obj )
        {
            PropertyInfo objProperty;
            string _Category = Null.NullString;

            //Get Category Field
            if (!String.IsNullOrEmpty(CategoryDataField))
            {
                objProperty = obj.GetType().GetProperty(CategoryDataField);
                if (!((objProperty == null) || (objProperty.GetValue(obj, null) == null)))
                {
                    _Category = Convert.ToString(objProperty.GetValue(obj, null));
                }
            }

            return _Category;
        }

        /// <Summary>
        /// GetGroups gets an array of Groups/Categories from the DataSource
        /// </Summary>
        protected override string[] GetGroups( IEnumerable arrObjects )
        {
            ArrayList arrGroups = new ArrayList();
            PropertyInfo objProperty;
            string[] strGroups = new string[0];

            foreach (object obj in arrObjects)
            {
                //Get Category Field
                if (!String.IsNullOrEmpty(CategoryDataField))
                {
                    objProperty = obj.GetType().GetProperty(CategoryDataField);
                    if (!((objProperty == null) || (objProperty.GetValue(obj, null) == null)))
                    {
                        string _Category = Convert.ToString(objProperty.GetValue(obj, null));

                        if (!arrGroups.Contains(_Category))
                        {
                            arrGroups.Add(_Category);
                        }
                    }
                }
            }

            strGroups = new string[arrGroups.Count - 1 + 1];
            for (int i = 0; i <= arrGroups.Count - 1; i++)
            {
                strGroups[i] = arrGroups[i].ToString();
            }
            return strGroups;
        }

        /// <Summary>
        /// GetRowVisibility determines the Visibility of a row in the table
        /// </Summary>
        /// <Param name="obj">The property</Param>
        protected override bool GetRowVisibility( object obj )
        {
            bool isVisible = true;
            PropertyInfo objProperty;
            objProperty = obj.GetType().GetProperty(VisibleDataField);
            if (!((objProperty == null) || (objProperty.GetValue(obj, null) == null)))
            {
                isVisible = Convert.ToBoolean(objProperty.GetValue(obj, null));
            }

            return isVisible;
        }

        protected override void AddEditorRow( ref Table tbl, object obj )
        {
            Hashtable fields = new Hashtable();
            fields.Add("Category", CategoryDataField);
            fields.Add("Editor", EditorDataField);
            fields.Add("Name", NameDataField);
            fields.Add("Required", RequiredDataField);
            fields.Add("Type", TypeDataField);
            fields.Add("ValidationExpression", ValidationExpressionDataField);
            fields.Add("Value", ValueDataField);
            fields.Add("Visibility", VisibilityDataField);


            AddEditorRow(ref tbl, NameDataField, new CollectionEditorInfoAdapter(obj, this.ID, NameDataField, fields));
            
        }
    }
}