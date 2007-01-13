#region DotNetNuke License
// DotNetNuke® - http://www.dotnetnuke.com
// Copyright (c) 2002-2006
// by DotNetNuke Corporation
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
// documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
// the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
// to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions 
// of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
// TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
// THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
// CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
// DEALINGS IN THE SOFTWARE.
#endregion
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