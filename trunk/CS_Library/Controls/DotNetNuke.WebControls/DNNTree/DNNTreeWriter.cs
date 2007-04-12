//
// DotNetNuke® - http://www.dotnetnuke.com
// Copyright (c) 2002-2005
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
//

using System;
using System.Web.UI;
using System.Web.UI.WebControls;
namespace DotNetNuke.UI.WebControls
{

	internal class DNNTreeWriter : WebControl, IDNNTreeWriter
	{
		private DNNTree _tree;

		/// -----------------------------------------------------------------------------
		/// <summary>
		/// 
		/// </summary>
		/// <remarks>
		/// </remarks>
		/// <history>
		/// 	[jbrinkman]	5/6/2004	Created
		/// </history>
		/// -----------------------------------------------------------------------------
		public DNNTreeWriter()
		{
		}

		/// -----------------------------------------------------------------------------
		/// <summary>
		/// 
		/// </summary>
		/// <param name="writer"></param>
		/// <param name="tree"></param>
		/// <remarks>
		/// </remarks>
		/// <history>
		/// 	[jbrinkman]	5/6/2004	Created
		/// </history>
		/// -----------------------------------------------------------------------------
		public void RenderTree(HtmlTextWriter writer, DNNTree tree)
		{
			_tree = tree;
			RenderControl(writer);
		}

		/// -----------------------------------------------------------------------------
		/// <summary>
		/// 
		/// </summary>
		/// <param name="writer"></param>
		/// <remarks>
		/// </remarks>
		/// <history>
		/// 	[jbrinkman]	5/6/2004	Created
		/// </history>
		/// -----------------------------------------------------------------------------
		protected override void RenderContents(HtmlTextWriter writer)
		{
			writer.AddAttribute(HtmlTextWriterAttribute.Width, "100%");
			writer.AddAttribute(HtmlTextWriterAttribute.Class, "DNNTree");
			writer.AddAttribute(HtmlTextWriterAttribute.Name, _tree.UniqueID);
			writer.AddAttribute(HtmlTextWriterAttribute.Id, _tree.ClientID);

			writer.RenderBeginTag(HtmlTextWriterTag.Div);
			RenderChildren(writer);
			writer.RenderEndTag();
		}

		/// -----------------------------------------------------------------------------
		/// <summary>
		/// 
		/// </summary>
		/// <param name="writer"></param>
		/// <remarks>
		/// </remarks>
		/// <history>
		/// 	[jbrinkman]	5/6/2004	Created
		/// </history>
		/// -----------------------------------------------------------------------------
		protected override void RenderChildren(HtmlTextWriter writer)
		{
			foreach (TreeNode TempNode in _tree.TreeNodes) {
				TempNode.Render(writer);
			}
		}

	}
}
