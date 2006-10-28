using System.ComponentModel;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Users;
using DotNetNuke.UI.WebControls;
using Microsoft.VisualBasic.CompilerServices;

namespace DotNetNuke.Entities.Profile
{
    /// <Summary>
    /// The ProfilePropertyDefinition class provides a Business Layer entity for
    /// property Definitions
    /// </Summary>
    public class ProfilePropertyDefinition
    {
        private int _DataType;
        private string _DefaultValue;
        private bool _IsDirty;
        private int _Length;
        private int _ModuleDefId;
        private int _PortalId;
        private string _PropertyCategory;
        private int _PropertyDefinitionId;
        private string _PropertyName;
        private string _PropertyValue;
        private bool _Required;
        private string _ValidationExpression;
        private int _ViewOrder;
        private UserVisibilityMode _Visibility;
        private bool _Visible;

        public ProfilePropertyDefinition()
        {
            this._DataType = Null.NullInteger;
            this._ModuleDefId = Null.NullInteger;
            this._PropertyDefinitionId = Null.NullInteger;
            this._Visibility = UserVisibilityMode.AdminOnly;
        }

        /// <Summary>Gets and sets the Data Type of the Profile Property</Summary>
        [Editor( "DotNetNuke.UI.WebControls.DNNListEditControl, DotNetNuke", typeof( EditControl ) ), SortOrderAttribute( 1 ), RequiredAttribute( true ), IsReadOnlyAttribute( true ), ListAttribute( "DataType", "", ListBoundField.Id, ListBoundField.Value )]
        public int DataType
        {
            get
            {
                return this._DataType;
            }
            set
            {
                if( this._DataType != value )
                {
                    this._IsDirty = true;
                }
                this._DataType = value;
            }
        }

        /// <Summary>Gets and sets the Default Value of the Profile Property</Summary>
        [SortOrderAttribute( 4 )]
        public string DefaultValue
        {
            get
            {
                return this._DefaultValue;
            }
            set
            {
                if( Operators.CompareString( this._DefaultValue, value, false ) != 0 )
                {
                    this._IsDirty = true;
                }
                this._DefaultValue = value;
            }
        }

        /// <Summary>
        /// Gets whether the Definition has been modified since it has been retrieved
        /// </Summary>
        [BrowsableAttribute( false )]
        public bool IsDirty
        {
            get
            {
                return this._IsDirty;
            }
        }

        /// <Summary>Gets and sets the Length of the Profile Property</Summary>
        [SortOrderAttribute( 3 )]
        public int Length
        {
            get
            {
                return this._Length;
            }
            set
            {
                if( this._Length != value )
                {
                    this._IsDirty = true;
                }
                this._Length = value;
            }
        }

        /// <Summary>Gets and sets the ModuleDefId</Summary>
        [BrowsableAttribute( false )]
        public int ModuleDefId
        {
            get
            {
                return this._ModuleDefId;
            }
            set
            {
                this._ModuleDefId = value;
            }
        }

        /// <Summary>Gets and sets the PortalId</Summary>
        [BrowsableAttribute( false )]
        public int PortalId
        {
            get
            {
                return this._PortalId;
            }
            set
            {
                this._PortalId = value;
            }
        }

        /// <Summary>Gets and sets the Category of the Profile Property</Summary>
        [SortOrderAttribute( 2 ), RequiredAttribute( true )]
        public string PropertyCategory
        {
            get
            {
                return this._PropertyCategory;
            }
            set
            {
                if( Operators.CompareString( this._PropertyCategory, value, false ) != 0 )
                {
                    this._IsDirty = true;
                }
                this._PropertyCategory = value;
            }
        }

        /// <Summary>Gets and sets the Id of the ProfilePropertyDefinition</Summary>
        [BrowsableAttribute( false )]
        public int PropertyDefinitionId
        {
            get
            {
                return this._PropertyDefinitionId;
            }
            set
            {
                this._PropertyDefinitionId = value;
            }
        }

        /// <Summary>Gets and sets the Name of the Profile Property</Summary>
        [SortOrderAttribute( 0 ), RequiredAttribute( true ), IsReadOnlyAttribute( true )]
        public string PropertyName
        {
            get
            {
                return this._PropertyName;
            }
            set
            {
                if( Operators.CompareString( this._PropertyName, value, false ) != 0 )
                {
                    this._IsDirty = true;
                }
                this._PropertyName = value;
            }
        }

        /// <Summary>Gets and sets the Value of the Profile Property</Summary>
        [BrowsableAttribute( false )]
        public string PropertyValue
        {
            get
            {
                return this._PropertyValue;
            }
            set
            {
                if( Operators.CompareString( this._PropertyValue, value, false ) != 0 )
                {
                    this._IsDirty = true;
                }
                this._PropertyValue = value;
            }
        }

        /// <Summary>Gets and sets whether the property is required</Summary>
        [SortOrderAttribute( 6 )]
        public bool Required
        {
            get
            {
                return this._Required;
            }
            set
            {
                if( this._Required != value )
                {
                    this._IsDirty = true;
                }
                this._Required = value;
            }
        }

        /// <Summary>
        /// Gets and sets a Validation Expression (RegEx) for the Profile Property
        /// </Summary>
        [SortOrderAttribute( 5 )]
        public string ValidationExpression
        {
            get
            {
                return this._ValidationExpression;
            }
            set
            {
                if( Operators.CompareString( this._ValidationExpression, value, false ) != 0 )
                {
                    this._IsDirty = true;
                }
                this._ValidationExpression = value;
            }
        }

        /// <Summary>Gets and sets the View Order of the Property</Summary>
        [RequiredAttribute( true ), SortOrderAttribute( 8 )]
        public int ViewOrder
        {
            get
            {
                return this._ViewOrder;
            }
            set
            {
                if( this._ViewOrder != value )
                {
                    this._IsDirty = true;
                }
                this._ViewOrder = value;
            }
        }

        /// <Summary>Gets and sets whether the property is visible</Summary>
        [BrowsableAttribute( false )]
        public UserVisibilityMode Visibility
        {
            get
            {
                return this._Visibility;
            }
            set
            {
                if( this._Visibility != value )
                {
                    this._IsDirty = true;
                }
                this._Visibility = value;
            }
        }

        /// <Summary>Gets and sets whether the property is visible</Summary>
        [SortOrderAttribute( 7 )]
        public bool Visible
        {
            get
            {
                return this._Visible;
            }
            set
            {
                if( this._Visible != value )
                {
                    this._IsDirty = true;
                }
                this._Visible = value;
            }
        }

        /// <Summary>Clears the IsDirty Flag</Summary>
        public void ClearIsDirty()
        {
            this._IsDirty = false;
        }
    }
}