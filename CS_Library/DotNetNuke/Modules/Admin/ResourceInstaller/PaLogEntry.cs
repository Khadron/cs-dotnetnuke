namespace DotNetNuke.Modules.Admin.ResourceInstaller
{
    public class PaLogEntry
    {
        private string m_Description;
        private PaLogType m_Type;

        public PaLogEntry( PaLogType type, string description )
        {
            this.m_Type = type;
            this.m_Description = description;
        }

        public string Description
        {
            get
            {
                if( this.m_Description != null )
                {
                    return this.m_Description;
                }
                else
                {
                    return "...";
                }
            }
        }

        public PaLogType Type
        {
            get
            {
                return this.m_Type;
            }
        }
    }
}