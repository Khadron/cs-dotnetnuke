using System;

namespace DotNetNuke.Entities.Modules.Communications
{
    public class ModuleCommunicationEventArgs : EventArgs
    {
        private string _Type = null;
        private object _Value = null;
        private string _Sender = null;
        private string _Target = null;

        private string _Text = null;

        public string Text
        {
            get
            {
                return _Text;
            }
            set
            {
                _Text = value;
            }
        }

        public string Type
        {
            get
            {
                return _Type;
            }
            set
            {
                _Type = value;
            }
        }

        public object Value
        {
            get
            {
                return _Value;
            }
            set
            {
                _Value = value;
            }
        }

        public string Sender
        {
            get
            {
                return _Sender;
            }
            set
            {
                _Sender = value;
            }
        }

        public string Target
        {
            get
            {
                return _Target;
            }
            set
            {
                _Target = value;
            }
        }

        public ModuleCommunicationEventArgs()
        {
        }

        public ModuleCommunicationEventArgs( string Type, object Value, string Sender, string Target )
        {
            _Type = Type;
            _Value = Value;
            _Sender = Sender;
            _Target = Target;
        }

        public ModuleCommunicationEventArgs( string Text )
        {
            _Text = Text;
        }
    }
}