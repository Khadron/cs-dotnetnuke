namespace DotNetNuke.Services.Exceptions
{
    public class ExceptionInfo
    {
        private int _FileColumnNumber;
        private int _FileLineNumber;
        private string _FileName;
        private string _Method;

        public int FileColumnNumber
        {
            get
            {
                return this._FileColumnNumber;
            }
            set
            {
                this._FileColumnNumber = value;
            }
        }

        public int FileLineNumber
        {
            get
            {
                return this._FileLineNumber;
            }
            set
            {
                this._FileLineNumber = value;
            }
        }

        public string FileName
        {
            get
            {
                return this._FileName;
            }
            set
            {
                this._FileName = value;
            }
        }

        public string Method
        {
            get
            {
                return this._Method;
            }
            set
            {
                this._Method = value;
            }
        }
    }
}