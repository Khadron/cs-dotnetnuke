using System;

namespace DotNetNuke.UI.WebControls
{
    /// <Summary>
    /// The EditControlFactory control provides a factory for creating the
    /// appropriate Edit Control
    /// </Summary>
    public class EditControlFactory
    {
        /// <Summary>
        /// CreateEditControl creates the appropriate Control based on the EditorField or
        /// TypeDataField
        /// </Summary>
        /// <Param name="editorInfo">An EditorInfo object</Param>
        public static EditControl CreateEditControl( EditorInfo editorInfo )
        {
            EditControl propEditor;

            if (editorInfo.Editor == "UseSystemType")
            {
                Type type = Type.GetType(editorInfo.Type);
                //Use System Type

                switch (type.FullName)
                {
                    case "System.Boolean":

                        propEditor = new CheckEditControl();
                        break;
                    case "System.Int32":
                        propEditor = new IntegerEditControl();
                        break;

                    case "System.Int16":

                        propEditor = new IntegerEditControl();
                        break;
                    default:

                        if (type.IsEnum)
                        {
                            propEditor = new EnumEditControl(editorInfo.Type);
                        }
                        else
                        {
                            propEditor = new TextEditControl(editorInfo.Type);
                        }
                        break;
                }
            }
            else
            {
                //Use Editor
                Type editType = Type.GetType(editorInfo.Editor, true, true);
                propEditor = (EditControl)Activator.CreateInstance(editType, null);
            }

            propEditor.ID = editorInfo.Name;
            propEditor.Name = editorInfo.Name;

            propEditor.EditMode = editorInfo.EditMode;
            propEditor.Required = editorInfo.Required;

            propEditor.Value = editorInfo.Value;
            propEditor.OldValue = editorInfo.Value;

            propEditor.CustomAttributes = editorInfo.Attributes;

            return propEditor;
        }
    }
}