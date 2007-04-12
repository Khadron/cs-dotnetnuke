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


namespace DotNetNuke.UI.WebControls
{

	/// <summary>
	/// ITreeNodeWriter interface declaration. All the objects which want to implement
	/// a writer class for the TreeNode should inherit from this interface.
	/// </summary>
	internal interface ITreeNodeWriter
	{
		/// <summary>
		/// When implemented renders an Node inside the tree.
		/// </summary>
		/// <param name="writer"></param>
		/// <param name="Node"></param>
		void RenderNode(HtmlTextWriter writer, TreeNode Node);
	}
	//ITreeNodeWriter

	/// <summary>
	/// IDNNTreeWriter interface declaration. All the objects which want to implement
	/// a writer class for the DNNTree should inherit from this interface.
	/// </summary>
	internal interface IDNNTreeWriter
	{
		/// <summary>
		/// When implemented renders the tree.
		/// </summary>
		/// <param name="writer"></param>
		/// <param name="tree"></param>
		void RenderTree(HtmlTextWriter writer, DNNTree tree);
	}

	/// <summary>
	/// IMenuNodeWriter interface declaration. All the objects which want to implement
	/// a writer class for the MenuNode should inherit from this interface.
	/// </summary>
	internal interface IMenuNodeWriter
	{
		/// <summary>
		/// When implemented renders an Node inside the Menu.
		/// </summary>
		/// <param name="writer"></param>
		/// <param name="Node"></param>
		void RenderNode(HtmlTextWriter writer, MenuNode Node);
	}
	//IMenuNodeWriter

	/// <summary>
	/// IDNNMenuWriter interface declaration. All the objects which want to implement
	/// a writer class for the DNNMenu should inherit from this interface.
	/// </summary>
	internal interface IDNNMenuWriter
	{
		/// <summary>
		/// When implemented renders the Menu.
		/// </summary>
		/// <param name="writer"></param>
		/// <param name="Menu"></param>
		void RenderMenu(HtmlTextWriter writer, DNNMenu menu);
	}

	public interface IDNNToolBar
	{
		string ToolBarId {
			get;
			set;
		}
	}

	public interface IDNNToolBarSupportedActions
	{
		string[] Actions {
			get;
		}
	}
}

