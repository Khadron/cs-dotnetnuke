using System;

namespace DotNetNuke.UI.WebControls
{
    /// <Summary>
    /// The DNNDataGridCheckChangedEventArgs class is a cusom EventArgs class for
    /// handling Event Args from the CheckChanged event in a CheckBox Column
    /// </Summary>
    public class PropertyEditorEventArgs : EventArgs
    {
        private string mName;
        private object mOldValue;
        private string mStringValue;
        private object mValue;

        /// <Summary>Constructs a new PropertyEditorEventArgs</Summary>
        /// <Param name="name">The name of the property</Param>
        public PropertyEditorEventArgs( string name ) : this( name, null, null )
        {
        }

        /// <Summary>Constructs a new PropertyEditorEventArgs</Summary>
        /// <Param name="name">The name of the property</Param>
        /// <Param name="newValue">The new value of the property</Param>
        /// <Param name="oldValue">The old value of the property</Param>
        public PropertyEditorEventArgs( string name, object newValue, object oldValue )
        {
            this.mName = name;
            this.mValue = newValue;
            this.mOldValue = oldValue;
        }

        /// <Summary>Gets and sets the Name of the Property being changed</Summary>
        public string Name
        {
            get
            {
                return this.mName;
            }
            set
            {
                this.mName = value;
            }
        }

        /// <Summary>Gets and sets the OldValue of the Property being changed</Summary>
        public object OldValue
        {
            get
            {
                return this.mOldValue;
            }
            set
            {
                this.mOldValue = value;
            }
        }

        /// <Summary>
        /// Gets and sets the String Value of the Property being changed
        /// </Summary>
        public string StringValue
        {
            get
            {
                return this.mStringValue;
            }
            set
            {
                this.mStringValue = value;
            }
        }

        /// <Summary>Gets and sets the Value of the Property being changed</Summary>
        public object Value
        {
            get
            {
                return this.mValue;
            }
            set
            {
                this.mValue = value;
            }
        }
    }
}