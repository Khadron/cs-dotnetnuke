using System;
using System.ComponentModel;
using System.Web.UI.Design;

namespace DotNetNuke.UI.WebControls.Design
{
    public class PropertyEditorControlDesigner : ControlDesigner
    {
        public override string GetDesignTimeHtml()
        {
            //TODO:  There is a bug here somewhere that results in a design-time rendering error when the control is re-rendered [jmb]
            string DesignTimeHtml = null;
            //Dim control As PropertyEditorControl = CType(Component, PropertyEditorControl)

            //Try
            //    If control.DataSource Is Nothing Then
            //        control.DataSource = New DefaultDesignerInfo
            //        control.DataBind()
            //    End If
            //    DesignTimeHtml = MyBase.GetDesignTimeHtml()
            //Catch ex As Exception
            //    DesignTimeHtml = GetErrorDesignTimeHtml(ex)
            //Finally
            //    If TypeOf control.DataSource Is DefaultDesignerInfo Then
            //        control.DataSource = Nothing
            //        control.DataBind()
            //    End If
            //End Try

            if (DesignTimeHtml == null)
            {
                DesignTimeHtml = GetEmptyDesignTimeHtml();
            }

            return DesignTimeHtml;
        }

        public override void Initialize( IComponent component )
        {
            if( ! ( component is PropertyEditorControl ) )
            {
                throw new ArgumentException( "Component must be of Type PropertyEditorControl", "component" );
            }
            else
            {
                base.Initialize( component );
            }
        }
    }
}