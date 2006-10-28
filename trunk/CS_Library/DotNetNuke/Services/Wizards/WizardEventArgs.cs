using System;

namespace DotNetNuke.Services.Wizards
{
    /// <Summary>
    /// The WizardEventArgs class extends EventArgs to provide Wizard specific
    /// Properties.
    /// </Summary>
    public class WizardEventArgs : EventArgs
    {
        private int m_CurPageNo;
        private WizardPageCollection m_Pages;
        private int m_PrevPageNo;

        /// <Summary>Constructs a custom WizardEventArgs object</Summary>
        /// <Param name="pageNo">The Page No where the Event happened</Param>
        /// <Param name="prevNo">The Page No where the Event happened</Param>
        /// <Param name="pages">The WizardPageCollection</Param>
        public WizardEventArgs( int pageNo, int prevNo, WizardPageCollection pages )
        {
            this.m_CurPageNo = 0;
            this.m_PrevPageNo = -1;
            this.m_CurPageNo = pageNo;
            this.m_PrevPageNo = prevNo;
            this.m_Pages = pages;
        }

        /// <Summary>Constructs a custom WizardEventArgs object</Summary>
        /// <Param name="pageNo">The Page No where the Event happened</Param>
        /// <Param name="pages">The WizardPageCollection</Param>
        public WizardEventArgs( int pageNo, WizardPageCollection pages )
        {
            this.m_CurPageNo = 0;
            this.m_PrevPageNo = -1;
            this.m_CurPageNo = pageNo;
            this.m_PrevPageNo = pageNo;
            this.m_Pages = pages;
        }

        /// <Summary>Constructs a default WizardEventArgs object</Summary>
        public WizardEventArgs() : this( -1, -1, ( (WizardPageCollection)null ) )
        {
        }

        public WizardPage Page
        {
            get
            {
                WizardPage retValue = null;
                if (m_CurPageNo > -1)
                {
                    retValue = m_Pages[this.m_CurPageNo];
                }
                return retValue;
            }
        }

        public int PageNo
        {
            get
            {
                return this.m_CurPageNo;
            }
        }

        public WizardPage PreviousPage
        {
            get
            {
                WizardPage retValue = null;
                if (m_PrevPageNo > -1)
                {
                    retValue = m_Pages[m_PrevPageNo];
                }
                return retValue;
            }
        }

        public int PreviousPageNo
        {
            get
            {
                return this.m_PrevPageNo;
            }
        }
    }
}