using System;

namespace DotNetNuke.HttpModules.Config
{
    [Serializable()]
    public class RewriterRule
    {
        private string _lookFor;
        private string _sendTo;

        public string LookFor
        {
            get
            {
                return _lookFor;
            }
            set
            {
                _lookFor = value;
            }
        }

        public string SendTo
        {
            get
            {
                return _sendTo;
            }
            set
            {
                _sendTo = value;
            }
        }
    }
}