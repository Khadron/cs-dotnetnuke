<%@ Control Language="C#" CodeBehind="URLControl.ascx.cs" AutoEventWireup="false" Explicit="True" Inherits="DotNetNuke.UI.UserControls.UrlControl" %>
<table cellSpacing="0" cellPadding="0" border="0">
	<tr id="TypeRow" runat="server">
		<td noWrap><br>
			<asp:label id="lblURLType" runat="server" enableviewstate="False" resourcekey="Type" CssClass="NormalBold"></asp:label><br>
			<asp:radiobuttonlist id="optType" CssClass="NormalBold" AutoPostBack="True" RepeatDirection="Vertical"
				Runat="server"></asp:radiobuttonlist><br>
		</td>
	</tr>
	<TR id="URLRow" runat="server">
		<td noWrap><asp:label id="lblURL" runat="server" enableviewstate="False" resourcekey="URL" CssClass="NormalBold"></asp:label><asp:dropdownlist id="cboUrls" runat="server" datatextfield="Url" datavaluefield="Url" cssclass="NormalTextBox"
				Width="300"></asp:dropdownlist><asp:textbox id="txtUrl" runat="server" cssclass="NormalTextBox" Width="300"></asp:textbox><br>
			<asp:linkbutton id="cmdSelect" resourcekey="Select" CssClass="CommandButton" Runat="server" CausesValidation="False">Select</asp:linkbutton><asp:linkbutton id="cmdDelete" resourcekey="Delete" CssClass="CommandButton" Runat="server" CausesValidation="False">Delete</asp:linkbutton><asp:linkbutton id="cmdAdd" resourcekey="Add" CssClass="CommandButton" Runat="server" CausesValidation="False">Add</asp:linkbutton></td>
	</TR>
	<TR id="TabRow" runat="server">
		<td noWrap><asp:label id="lblTab" runat="server" enableviewstate="False" resourcekey="Tab" CssClass="NormalBold"></asp:label><asp:dropdownlist id="cboTabs" runat="server" datatextfield="TabName" datavaluefield="TabId" cssclass="NormalTextBox"
				Width="300"></asp:dropdownlist></td>
	</TR>
	<TR id="FileRow" runat="server">
		<td noWrap><asp:label id="lblFolder" runat="server" enableviewstate="False" resourcekey="Folder" CssClass="NormalBold"></asp:label><asp:dropdownlist id="cboFolders" runat="server" AutoPostBack="True" cssclass="NormalTextBox" Width="300"></asp:dropdownlist>
			<asp:Image ID="imgStorageLocationType" Runat="server" Visible="False"></asp:Image><br>
			<asp:label id="lblFile" runat="server" enableviewstate="False" resourcekey="File" CssClass="NormalBold"></asp:label><asp:dropdownlist id="cboFiles" runat="server" datatextfield="Text" datavaluefield="Value" cssclass="NormalTextBox"
				Width="300"></asp:dropdownlist><INPUT id="txtFile" type="file" size="30" name="txtFile" runat="server" Width="300">
			<br>
			<asp:linkbutton id="cmdUpload" resourcekey="Upload" CssClass="CommandButton" Runat="server" CausesValidation="False">Upload</asp:linkbutton><asp:linkbutton id="cmdSave" resourcekey="Save" CssClass="CommandButton" Runat="server" CausesValidation="False">Save</asp:linkbutton><asp:linkbutton id="cmdCancel" resourcekey="Cancel" CssClass="CommandButton" Runat="server" CausesValidation="False">Cancel</asp:linkbutton></td>
	</TR>
	<TR id="UserRow" runat="server">
		<td noWrap><asp:label id="lblUser" runat="server" enableviewstate="False" resourcekey="User" CssClass="NormalBold"></asp:label>
			<asp:TextBox ID="txtUser" Runat="server" cssclass="NormalTextBox" width="300"></asp:textbox>
		</td>
	</TR>
	<TR id="ErrorRow" runat="server">
		<TD><asp:label id="lblMessage" runat="server" enableviewstate="False" CssClass="NormalRed"></asp:label><br>
		</TD>
	</TR>
	<tr>
		<td noWrap><asp:checkbox id="chkTrack" resourcekey="Track" CssClass="NormalBold" Runat="server" Text="Track?"
				TextAlign="Right"></asp:checkbox><asp:checkbox id="chkLog" resourcekey="Log" CssClass="NormalBold" Runat="server" Text="Log?" TextAlign="Right"></asp:checkbox><asp:checkbox id="chkNewWindow" resourcekey="NewWindow" CssClass="NormalBold" Runat="server" Text="New Window?"
				TextAlign="Right" Visible="False"></asp:checkbox><br>
		</td>
	</tr>
</table>
