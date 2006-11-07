#region DotNetNuke License
// DotNetNuke® - http://www.dotnetnuke.com
// Copyright (c) 2002-2006
// by Perpetual Motion Interactive Systems Inc. ( http://www.perpetualmotion.ca )
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