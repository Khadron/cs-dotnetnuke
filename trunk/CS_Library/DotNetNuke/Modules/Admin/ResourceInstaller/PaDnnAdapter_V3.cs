using System;
using System.Xml;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Modules.Definitions;

namespace DotNetNuke.Modules.Admin.ResourceInstaller
{
    /// <summary>
    /// The PaDnnAdapter_V3 extends PaDnnAdapter_V2 to support V3 Modules
    /// </summary>
    /// <remarks>
    /// </remarks>
    public class PaDnnAdapter_V3 : PaDnnAdapter_V2
    {
        public PaDnnAdapter_V3(PaInstallInfo InstallerInfo)
            : base(InstallerInfo)
        {
        }

        protected override PaFolder GetFolderAttributesFromNode(XmlElement FolderElement)
        {
            // call the V2 implementation to load the values
            PaFolder folder = base.GetFolderAttributesFromNode(FolderElement);

            // V3 .dnn file format adds the optional businesscontrollerclass node to the folder node element
            XmlElement businessControllerElement = (XmlElement)FolderElement.SelectSingleNode("businesscontrollerclass");
            if (businessControllerElement != null)
            {
                folder.BusinessControllerClass = businessControllerElement.InnerText.Trim();
            }

            // V3 .dnn file format adds the optional friendlyname/foldername/modulename nodes to the folder node element
            //For these new nodes the defaults are as follows
            // <friendlyname>, <name>
            // <foldernamee>, <name>, "MyModule"
            // <modulename>, <friendlyname>, <name>
            XmlElement friendlynameElement = (XmlElement)FolderElement.SelectSingleNode("friendlyname");
            if (friendlynameElement != null)
            {
                folder.FriendlyName = friendlynameElement.InnerText.Trim();
                folder.ModuleName = friendlynameElement.InnerText.Trim();
            }

            XmlElement foldernameElement = (XmlElement)FolderElement.SelectSingleNode("foldername");
            if (foldernameElement != null)
            {
                folder.FolderName = foldernameElement.InnerText.Trim();
            }
            if (folder.FolderName == "")
            {
                folder.FolderName = "MyModule";
            }

            XmlElement modulenameElement = (XmlElement)FolderElement.SelectSingleNode("modulename");
            if (modulenameElement != null)
            {
                folder.ModuleName = modulenameElement.InnerText.Trim();
            }

            return folder;
        }

        protected override ModuleControlInfo GetModuleControlFromNode(string Foldername, int TempModuleID, XmlElement ModuleControl)
        {
            // Version 3 .dnn file format adds the helpurl node to the controls/control node element
            ModuleControlInfo ModControl = base.GetModuleControlFromNode(Foldername, TempModuleID, ModuleControl);
            if (ModControl != null)
            {
                XmlElement helpElement = (XmlElement)ModuleControl.SelectSingleNode("helpurl");
                if (helpElement != null)
                {
                    ModControl.HelpURL = helpElement.InnerText.Trim();
                }
            }
            return ModControl;
        }

        protected override ModuleDefinitionInfo GetModuleFromNode(int TempModuleDefinitionID, PaFolder Folder, XmlElement DNNModule)
        {
            ModuleDefinitionInfo ModuleDef = base.GetModuleFromNode(TempModuleDefinitionID, Folder, DNNModule);

            if (ModuleDef != null)
            {
                XmlElement cacheElement = (XmlElement)DNNModule.SelectSingleNode("cachetime");
                if (cacheElement != null)
                {
                    ModuleDef.DefaultCacheTime = Convert.ToInt32(cacheElement.InnerText.Trim());
                }
            }

            return ModuleDef;
        }

        protected override void LogValidFormat()
        {
            InstallerInfo.Log.AddInfo(string.Format(DNN_Valid, "3.0"));
        }
    }
}