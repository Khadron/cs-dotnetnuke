namespace DotNetNuke.Services.Wizards
{
    /// <Summary>
    /// The WizardCancelEventArgs class extends WizardEventArgs to provide a Cancel
    /// Property for the Event Properties.
    /// </Summary>
    public class WizardCancelEventArgs : WizardEventArgs
    {
        private bool m_Cancel;

        /// <Summary>Constructs a custom WizardEventArgs object</Summary>
        /// <Param name="pageNo">The Page No where the Event happened</Param>
        /// <Param name="prevNo">The Page No where the Event happened</Param>
        /// <Param name="pages">The WizardPageCollection</Param>
        public WizardCancelEventArgs( int pageNo, int prevNo, WizardPageCollection pages ) : base( pageNo, prevNo, pages )
        {
            this.m_Cancel = false;
            this.Cancel = false;
        }

        /// <Summary>Constructs a default WizardEventArgs object</Summary>
        public WizardCancelEventArgs() : this( -1, -1, ( (WizardPageCollection)null ) )
        {
        }

        /// <Summary>Constructs a custom WizardEventArgs object</Summary>
        /// <Param name="pageNo">The Page No where the Event happened</Param>
        /// <Param name="pages">The WizardPageCollection</Param>
        public WizardCancelEventArgs( int pageNo, WizardPageCollection pages ) : base( pageNo, pages )
        {
            this.m_Cancel = false;
            this.Cancel = false;
        }

        public bool Cancel
        {
            get
            {
                return this.m_Cancel;
            }
            set
            {
                this.m_Cancel = value;
            }
        }
    }
}