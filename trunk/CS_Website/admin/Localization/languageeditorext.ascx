<%@ Register TagPrefix="dnn" TagName="TextEditor" Src="~/controls/TextEditor.ascx"%>
<%@ Control Language="C#" AutoEventWireup="true" CodeFile="LanguageEditorExt.ascx.cs" Inherits="DotNetNuke.Services.Localization.LanguageEditorExt" %>
<table cellspacing="2" cellpadding="2" border="0">
	<tr>
		<td class="SubHead">
			<asp:Label id="Label1" runat="server" resourcekey="Name">Resource Name</asp:Label></td>
	</tr>
	<tr>
		<td class="Normal">
			<asp:Label id="lblName" runat="server"></asp:Label></td>
	</tr>
	<tr height="10">
		<td></td>
	</tr>
	<tr>
		<td class="SubHead">
			<asp:Label id="Label3" runat="server" resourcekey="DefaultValue">Default Value</asp:Label></td>
	</tr>
	<tr>
		<td class="Normal">
			<asp:Label id="lblDefault" runat="server"></asp:Label></td>
	</tr>
	<tr valign="top">
		<td><dnn:texteditor id="teContent" runat="server" height="400" width="600"></dnn:texteditor></td>
	</tr>
</table>
<p>
	<asp:linkbutton class="CommandButton" id="cmdUpdate" resourcekey="cmdUpdate" runat="server" borderstyle="none"
		text="Update"></asp:linkbutton>
	<asp:linkbutton class="CommandButton" id="cmdCancel" resourcekey="cmdCancel" runat="server" borderstyle="none"
		text="Cancel" causesvalidation="False"></asp:linkbutton>&nbsp;
</p>
