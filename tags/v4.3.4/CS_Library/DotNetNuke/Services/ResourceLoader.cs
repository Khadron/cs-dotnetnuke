using System;
using System.Collections;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;
using System.Resources;

namespace DotNetNuke.Services
{
    public class ResourceLoader
    {
        private const string NO_RESOURCES = "No resources loaded";
        private const string THIS_ASSEMBLY = "DotNetNuke.";

        private static Hashtable myIconsCache;
        private static Hashtable myImagesCache;
        private static Assembly ourResourceAssembly;
        private static ResourceManager globalResources;
        private static ResourceManager sharedResources;
        private static ResourceManager templateResources;
        

        static ResourceLoader()
        {
            myImagesCache = new Hashtable();
            myIconsCache = new Hashtable();
            try
            {
                ourResourceAssembly = Assembly.GetExecutingAssembly();
                globalResources = new ResourceManager(THIS_ASSEMBLY + "Resources.GlobalResources", ourResourceAssembly);
                sharedResources = new ResourceManager(THIS_ASSEMBLY + "Resources.SharedResources", ourResourceAssembly);
                templateResources = new ResourceManager(THIS_ASSEMBLY + "Resources.Template", ourResourceAssembly);
            }
            catch( Exception exc)
            {
                Exceptions.Exceptions.LogException(exc);
            }
        }

        public static Stream GetFileAsStream( string fileName )
        {
            return ourResourceAssembly.GetManifestResourceStream( THIS_ASSEMBLY + fileName );
        }

        public static string GetGlobalString(string id)
        {
            if (globalResources != null)
            {
                return globalResources.GetString(id);
            }
            return NO_RESOURCES;
        }

        public static string GetSharedString(string id)
        {
            if (sharedResources != null)
            {
                return sharedResources.GetString(id);
            }
            return NO_RESOURCES;
        }

        public static string GetTemplateString(string id)
        {
            if (templateResources != null)
            {
                return templateResources.GetString(id);
            }
            return NO_RESOURCES;
        }

        public static Icon GetIcon(string iconName)
        {
            Icon icon = (Icon)myIconsCache[iconName];
            if (icon == null)
            {
                icon = new Icon(ourResourceAssembly.GetManifestResourceStream(THIS_ASSEMBLY + "icons." + iconName));
                myIconsCache[iconName] = icon;
            }
            return icon;
        }

        public static bool HasImage( string imageName )
        {
            Image image = (Image)myImagesCache[imageName];
            if( image != null )
            {
                return true;
            }
            Stream stream = ourResourceAssembly.GetManifestResourceStream( THIS_ASSEMBLY + "icons." + imageName );
            if( stream == null )
            {
                return false;
            }
            stream.Close();
            return true;
        }

        public static Image GetImage( string imageName )
        {
            Image image = (Image)myImagesCache[imageName];
            if( image == null )
            {
                image = Image.FromStream( ourResourceAssembly.GetManifestResourceStream( THIS_ASSEMBLY + "icons." + imageName ) );
                myImagesCache[imageName] = image;
            }
            return image;
        }

        public static Image MakeTransparent( Image image )
        {
            Bitmap bmp = image as Bitmap;
            if( bmp != null && bmp.RawFormat.Guid != ImageFormat.Icon.Guid )
            {
                bmp.MakeTransparent();
            }
            return image;
        }

        public static Stream GetResourceStream( string fileName )
        {
            StackTrace trace = new StackTrace();
            StackFrame parentFrame = trace.GetFrame( 1 );
            MethodBase parentMethod = parentFrame.GetMethod();
            Type parentType = parentMethod.DeclaringType;
            Assembly parentAssembly = parentType.Assembly;
            return GetResourceStream( parentAssembly, parentType.Namespace, fileName );
        }

        public static Image CreateImageFromResources( string name, Assembly asm )
        {
            return CreateBitmapFromResources( name, asm );
        }

        public static Bitmap CreateBitmapFromResources( string name, Assembly asm )
        {
            Stream stream = asm.GetManifestResourceStream( name );
            Bitmap image = (Bitmap)Bitmap.FromStream( stream );
            return image;
        }

        public static Icon CreateIconFromResources( string name, Assembly asm )
        {
            Stream stream = asm.GetManifestResourceStream( name );
            Icon icon = new Icon( stream );
            return icon;
        }

        public static Stream GetResourceStream( Assembly assembly, string namespaceName, string fileName )
        {
            return assembly.GetManifestResourceStream( namespaceName + "." + fileName );
        }
    }
}