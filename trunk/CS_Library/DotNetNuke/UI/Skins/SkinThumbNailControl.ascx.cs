using System;
using System.IO;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using DotNetNuke.Framework;
using DotNetNuke.Services.Localization;
using Microsoft.VisualBasic;
using Globals=DotNetNuke.Common.Globals;
using Image=System.Drawing.Image;

namespace DotNetNuke.UI.Skins
{
    /// <Summary>
    /// SkinThumbNailControl is a user control that provides that displays the skins
    /// as a Radio ButtonList with Thumbnail Images where available
    /// </Summary>
    public abstract class SkinThumbNailControl : UserControlBase
    {

        private int _Columns = -1;
        protected HtmlGenericControl ControlContainer;
        protected RadioButtonList optSkin;

        public string Border
        {
            get
            {
                return Convert.ToString( this.ViewState["SkinControlBorder"] );
            }
            set
            {
                this.ViewState["SkinControlBorder"] = value;
                if( value!="")
                {
                    return;
                }
                this.ControlContainer.Style.Add( "border-top", value );
                this.ControlContainer.Style.Add( "border-bottom", value );
                this.ControlContainer.Style.Add( "border-left", value );
                this.ControlContainer.Style.Add( "border-right", value );
            }
        }

        public int Columns
        {
            get
            {
                return Convert.ToInt32( this.ViewState["SkinControlColumns"] );
            }
            set
            {
                this.ViewState["SkinControlColumns"] = value;
                if( value <= 0 )
                {
                    return;
                }
                this.optSkin.RepeatColumns = value;
            }
        }

       
        public string Height
        {
            get
            {
                return Convert.ToString( this.ViewState["SkinControlHeight"] );
            }
            set
            {
                this.ViewState["SkinControlHeight"] = value;
                if( value!="" )
                {
                    return;
                }
                this.ControlContainer.Style.Add( "height", value );
            }
        }

        
        public string SkinRoot
        {
            get
            {
                return Convert.ToString( this.ViewState["SkinRoot"] );
            }
            set
            {
                this.ViewState["SkinRoot"] = value;
            }
        }

        public string SkinSrc
        {
            get
            {
                if( this.optSkin.SelectedItem == null )
                {
                    return "";
                }
                else
                {
                    return this.optSkin.SelectedItem.Value;
                }
            }
            set
            {
                // select current skin
                int intIndex;
                for (intIndex = 0; intIndex <= optSkin.Items.Count - 1; intIndex++)
                {
                    if (optSkin.Items[intIndex].Value == value)
                    {
                        optSkin.Items[intIndex].Selected = true;
                        break;
                    }
                }
            }
        }

        public string Width
        {
            get
            {
                return Convert.ToString( this.ViewState["SkinControlWidth"] );
            }
            set
            {
                ViewState["SkinControlWidth"] = value;
                if (value != "")
                {
                    ControlContainer.Style.Add("width", value);
                }
            }
        }

        public SkinThumbNailControl()
        {
            base.Load += new EventHandler( this.Page_Load );
            this._Columns = -1;
        }

        /// <Summary>CreateThumbnail creates a thumbnail of the Preview Image</Summary>
        /// <Param name="strImage">The Image File Name</Param>
        private string CreateThumbnail( string strImage )
        {
            bool blnCreate = true;

            string strThumbnail = strImage.Replace(Path.GetFileName(strImage), "thumbnail_" + Path.GetFileName(strImage));

            // check if image has changed
            if (File.Exists(strThumbnail))
            {
                DateTime d1 = File.GetLastWriteTime(strThumbnail);
                DateTime d2 = File.GetLastWriteTime(strImage);
                if (File.GetLastWriteTime(strThumbnail) == File.GetLastWriteTime(strImage))
                {
                    blnCreate = false;
                }
            }

            if (blnCreate)
            {
                double dblScale;
                int intHeight;
                int intWidth;

                int intSize = 140; // size of the thumbnail

                Image objImage;
                try
                {
                    objImage = Image.FromFile(strImage);

                    // scale the image to prevent distortion
                    if (objImage.Height > objImage.Width)
                    {
                        //The height was larger, so scale the width
                        dblScale = intSize / objImage.Height;
                        intHeight = intSize;
                        intWidth = Convert.ToInt32(objImage.Width * dblScale);
                    }
                    else
                    {
                        //The width was larger, so scale the height
                        dblScale = intSize / objImage.Width;
                        intWidth = intSize;
                        intHeight = Convert.ToInt32(objImage.Height * dblScale);
                    }

                    // create the thumbnail image
                    Image objThumbnail;
                    objThumbnail = objImage.GetThumbnailImage(intWidth, intHeight, null, IntPtr.Zero);

                    // delete the old file ( if it exists )
                    if (File.Exists(strThumbnail))
                    {
                        File.Delete(strThumbnail);
                    }

                    // save the thumbnail image
                    objThumbnail.Save(strThumbnail, objImage.RawFormat);

                    // set the file attributes
                    File.SetAttributes(strThumbnail, FileAttributes.Normal);
                    File.SetLastWriteTime(strThumbnail, File.GetLastWriteTime(strImage));

                    // tidy up
                    objImage.Dispose();
                    objThumbnail.Dispose();
                }
                catch
                {
                    // problem creating thumbnail
                }
            }

            strThumbnail = Globals.ApplicationPath + "\\" + strThumbnail.Substring(strThumbnail.ToLower().IndexOf("portals\\"));

            // return thumbnail filename
            return strThumbnail;
        }

        /// <Summary>format skin name</Summary>
        /// <Param name="strSkinFolder">The Folder Name</Param>
        /// <Param name="strSkinFile">The File Name without extension</Param>
        private string FormatSkinName( string strSkinFolder, string strSkinFile )
        {
            if (strSkinFolder.ToLower() == "_default")
            {
                // host folder
                return strSkinFile;
            }
            else // portal folder
            {
                switch (strSkinFile.ToLower())
                {
                    case "skin":
                        return strSkinFolder;

                    case "container":
                        return strSkinFolder;

                    case "default":
                        return strSkinFolder + " - " + strSkinFile;
                    default:
                        return strSkinFolder;
                }
            }
        }

        /// <Summary>
        /// AddDefaultSkin adds the not-specified skin to the radio button list
        /// </Summary>
        private void AddDefaultSkin()
        {
            string strDefault = Localization.GetString("Not_Specified") + "<br>";
            strDefault += "<img src=\"" + Globals.ApplicationPath + "/images/spacer.gif\" width=\"140\" height=\"135\" border=\"0\">";
            optSkin.Items.Insert(0, new ListItem(strDefault, ""));
        }

        /// <Summary>AddSkin adds the skin to the radio button list</Summary>
        /// <Param name="strFolder">The Skin Folder</Param>
        /// <Param name="strFile">The Skin File</Param>
        private void AddSkin( string root, string strFolder, string strFile )
        {
            string strImage = "";

            if (File.Exists(strFile.Replace(".ascx", ".jpg")))
            {
                strImage += "<a href=\"" + CreateThumbnail(strFile.Replace(".ascx", ".jpg")).Replace("thumbnail_", "") + "\" target=\"_new\"><img src=\"" + CreateThumbnail(strFile.Replace(".ascx", ".jpg")) + "\" border=\"1\"></a>";
            }
            else
            {
                strImage += "<img src=\"" + Globals.ApplicationPath + "/images/thumbnail.jpg\" border=\"1\">";
            }

            optSkin.Items.Add(new ListItem(FormatSkinName(strFolder, Path.GetFileNameWithoutExtension(strFile)) + "<br>" + strImage, root + "/" + strFolder + "/" + Path.GetFileName(strFile)));
        }

        /// <Summary>Clear clears the radio button list</Summary>
        public void Clear()
        {
            this.optSkin.Items.Clear();
        }

        /// <Summary>
        /// LoadAllSkins loads all the available skins (Host and Site) to the radio button list
        /// </Summary>
        /// <Param name="includeNotSpecified">
        /// Optionally include the "Not Specified" option
        /// </Param>
        public void LoadAllSkins( bool includeNotSpecified )
        {
            // default value
            if (includeNotSpecified)
            {
                AddDefaultSkin();
            }

            // load host skins (includeNotSpecified = false as we have already added it)
            LoadHostSkins(false);

            // load portal skins (includeNotSpecified = false as we have already added it)
            LoadPortalSkins(false);
        }

        /// <Summary>
        /// LoadHostSkins loads all the available Host skins to the radio button list
        /// </Summary>
        /// <Param name="includeNotSpecified">
        /// Optionally include the "Not Specified" option
        /// </Param>
        public void LoadHostSkins( bool includeNotSpecified )
        {
            string strRoot;
            string strFolder;
            string[] arrFolders;

            // default value
            if (includeNotSpecified)
            {
                AddDefaultSkin();
            }

            // load host skins
            strRoot = Globals.HostMapPath + SkinRoot;
            if (Directory.Exists(strRoot))
            {
                arrFolders = Directory.GetDirectories(strRoot);
                foreach (string tempLoopVar_strFolder in arrFolders)
                {
                    strFolder = tempLoopVar_strFolder;
                    if (!strFolder.EndsWith(Globals.glbHostSkinFolder))
                    {
                        LoadSkins(strFolder, "[G]", false);
                    }
                }
            }
        }

        /// <Summary>
        /// LoadHostSkins loads all the available Site/Portal skins to the radio button list
        /// </Summary>
        /// <Param name="includeNotSpecified">
        /// Optionally include the "Not Specified" option
        /// </Param>
        public void LoadPortalSkins( bool includeNotSpecified )
        {
            string strRoot;
            string strFolder;
            string[] arrFolders;

            // default value
            if (includeNotSpecified)
            {
                AddDefaultSkin();
            }

            // load portal skins
            strRoot = PortalSettings.HomeDirectoryMapPath + SkinRoot;
            if (Directory.Exists(strRoot))
            {
                arrFolders = Directory.GetDirectories(strRoot);
                foreach (string tempLoopVar_strFolder in arrFolders)
                {
                    strFolder = tempLoopVar_strFolder;
                    LoadSkins(strFolder, "[L]", false);
                }
            }
        }

        /// <Summary>
        /// LoadSkins loads all the available skins in a specific folder to the radio button list
        /// </Summary>
        /// <Param name="strFolder">The folder to search for skins</Param>
        /// <Param name="skinType">
        /// A string that identifies whether the skin is Host "[G]" or Site "[L]"
        /// </Param>
        /// <Param name="includeNotSpecified">
        /// Optionally include the "Not Specified" option
        /// </Param>
        public void LoadSkins( string strFolder, string skinType, bool includeNotSpecified )
        {
            string strFile;
            string[] arrFiles;

            // default value
            if (includeNotSpecified)
            {
                AddDefaultSkin();
            }

            if (Directory.Exists(strFolder))
            {
                arrFiles = Directory.GetFiles(strFolder, "*.ascx");
                strFolder = strFolder.Substring(Strings.InStrRev(strFolder, "\\", -1, 0) + 1 - 1);

                foreach (string tempLoopVar_strFile in arrFiles)
                {
                    strFile = tempLoopVar_strFile;
                    AddSkin(skinType + SkinRoot, strFolder, strFile);
                }
            }
        }

        /// <Summary>Page_Load runs when the control is loaded</Summary>
        private void Page_Load( object sender, EventArgs e )
        {
        }
    }
}