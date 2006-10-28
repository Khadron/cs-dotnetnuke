namespace DotNetNuke.Services.Localization
{
    public class LocaleFileUtil
    {
        public static string GetFullFileName(LocaleFileInfo LocaleFile)
        {
            string Result = LocaleFile.LocaleFileType.ToString() + "\\";
            if (LocaleFile.LocaleModule != null)
            {
                Result += LocaleFile.LocaleModule + "\\";
            }
            if (LocaleFile.LocalePath != null)
            {
                Result += LocaleFile.LocalePath + "\\";
            }
            Result += LocaleFile.LocaleFileName;
            return Result;
        }
    }
}