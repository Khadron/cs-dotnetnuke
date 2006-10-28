using System;
using System.Collections;
using System.IO;
using System.Xml;
using DotNetNuke.Entities.Modules.Definitions;

namespace DotNetNuke.Modules.Admin.ResourceInstaller
{
    /// <summary>
    /// The PaDnnAdapter_V2Provider extends PaDnnAdapterBase to support V2 Providers
    /// </summary>
    /// <remarks>
    /// </remarks>
    public class PaDnnAdapter_V2Provider : PaDnnAdapterBase
    {
        public PaDnnAdapter_V2Provider(PaInstallInfo InstallerInfo)
            : base(InstallerInfo)
        {
        }

        public override PaFolderCollection ReadDnn()
        {
            //This is a very long subroutine and should probably be broken down
            //into a couple of smaller routines.  That would make it easier to
            //maintain.
            InstallerInfo.Log.StartJob(DNN_Reading);

            //Determine if XML conforms to designated schema
            ArrayList DnnErrors = ValidateDnn();
            if (DnnErrors.Count == 0)
            {
                InstallerInfo.Log.AddInfo(string.Format(DNN_Valid, "2.0"));

                PaFolderCollection Folders = new PaFolderCollection();

                XmlDocument doc = new XmlDocument();
                MemoryStream buffer = new MemoryStream(InstallerInfo.DnnFile.Buffer, false);
                doc.Load(buffer);
                InstallerInfo.Log.AddInfo(XML_Loaded);

                //Create an XmlNamespaceManager for resolving namespaces.
                //Dim nsmgr As XmlNamespaceManager = New XmlNamespaceManager(doc.NameTable)
                //nsmgr.AddNamespace("dnn", "urn:ModuleDefinitionSchema")

                XmlNode dnnRoot = doc.DocumentElement;

                XmlElement FolderElement = (XmlElement)dnnRoot.SelectSingleNode("folder");
                //We will process each folder individually.  So we don't need to keep
                //as much in memory.

                //As we loop through each folder, we will save the data
                PaFolder Folder = new PaFolder();

                //The folder/name node is a required node.  If empty, this will throw
                //an exception.
                try
                {
                    Folder.Name = FolderElement.SelectSingleNode("name").InnerText.Trim();
                }
                catch (Exception)
                {
                    throw (new Exception(EXCEPTION_FolderName));
                }

                try
                {
                    Folder.ProviderType = FolderElement.SelectSingleNode("type").InnerText.Trim();
                }
                catch (Exception)
                {
                    throw (new Exception(EXCEPTION_FolderProvider));
                }

                // loading files
                InstallerInfo.Log.AddInfo(FILES_Loading);
                XmlElement file;
                foreach (XmlElement tempLoopVar_file in FolderElement.SelectNodes("files/file"))
                {
                    file = tempLoopVar_file;
                    //The file/name node is a required node.  If empty, this will throw
                    //an exception.
                    string name;
                    try
                    {
                        name = file.SelectSingleNode("name").InnerText.Trim();
                    }
                    catch (Exception)
                    {
                        throw (new Exception(EXCEPTION_FileName));
                    }
                    PaFile paf = (PaFile)InstallerInfo.FileTable[name.ToLower()];
                    if (paf == null)
                    {
                        InstallerInfo.Log.AddFailure(FILE_NotFound + name);
                    }
                    else
                    {
                        //A file path may or may not exist
                        XmlElement pathElement = (XmlElement)file.SelectSingleNode("path");
                        if (!(pathElement == null))
                        {
                            string filepath = pathElement.InnerText.Trim();
                            InstallerInfo.Log.AddInfo(string.Format(FILE_Found, filepath, name));
                            paf.Path = filepath;
                        }
                        else
                        {
                            // This is needed to override any file path which may exist in the zip file
                            paf.Path = "";
                        }
                        Folder.Files.Add(paf);
                    }
                }

                Folders.Add(Folder);

                if (!InstallerInfo.Log.Valid)
                {
                    throw (new Exception(EXCEPTION_LoadFailed));
                }
                InstallerInfo.Log.EndJob(DNN_Success);

                return Folders;
            }
            else
            {
                string err;
                foreach (string tempLoopVar_err in DnnErrors)
                {
                    err = tempLoopVar_err;
                    InstallerInfo.Log.AddFailure(err);
                }
                throw (new Exception(EXCEPTION_Format));
            }
        }

        private ArrayList ValidateDnn()
        {
            //Create New Validator
            ModuleDefinitionValidator ModuleValidator = new ModuleDefinitionValidator();

            //Tell Validator what XML to use
            MemoryStream xmlStream = new MemoryStream(InstallerInfo.DnnFile.Buffer);
            ModuleValidator.Validate(xmlStream);
            return ModuleValidator.Errors;
        }
    }
}