using System.Collections;
using System.Web.UI;
using System.Web.UI.WebControls;
using DotNetNuke.Framework;

namespace DotNetNuke.Modules.HTMLEditorProvider
{
    public abstract class HtmlEditorProvider : UserControlBase
    {
        private static HtmlEditorProvider objProvider;

        public abstract ArrayList AdditionalToolbars { get; set; }

        public abstract string ControlID { get; set; }

        public abstract Unit Height { get; set; }

        public abstract Control HtmlEditorControl { get; }

        public abstract string RootImageDirectory { get; set; }

        public abstract string Text { get; set; }

        public abstract Unit Width { get; set; }

        static HtmlEditorProvider()
        {
            objProvider = null;
            CreateProvider();
        }

        public static HtmlEditorProvider Instance()
        {
            CreateProvider();
            return objProvider;
        }

        public abstract void AddToolbar();

        private static void CreateProvider()
        {
            objProvider = ( (HtmlEditorProvider)Reflection.CreateObject( "htmlEditor" ) );
        }

        public abstract void Initialize();
    }
}