namespace DotNetNuke.Services.Vendors
{
    public class ClassificationInfo
    {
        private int _ClassificationId;
        private string _ClassificationName;
        private bool _IsAssociated;
        private int _ParentId;

        public int ClassificationId
        {
            get
            {
                return this._ClassificationId;
            }
            set
            {
                this._ClassificationId = value;
            }
        }

        public string ClassificationName
        {
            get
            {
                return this._ClassificationName;
            }
            set
            {
                this._ClassificationName = value;
            }
        }

        public bool IsAssociated
        {
            get
            {
                return this._IsAssociated;
            }
            set
            {
                this._IsAssociated = value;
            }
        }

        public int ParentId
        {
            get
            {
                return this._ParentId;
            }
            set
            {
                this._ParentId = value;
            }
        }
    }
}