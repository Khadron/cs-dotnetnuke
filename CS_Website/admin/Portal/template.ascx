<%@ Control Language="C#" AutoEventWireup="false" Inherits="DotNetNuke.Modules.Admin.PortalManagement.Template" CodeFile="Template.ascx.cs" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<table cellspacing="1" cellpadding="1" border="0">
	<tr>
		<td class="SubHead" width="150" valign="top"><dnn:label id="plPortals" text="Portal:" controlname="cboPortals" runat="server" /></td>
		<td><asp:dropdownlist id="cboPortals" runat="server" width="300px"></asp:dropdownlist></td>
	</tr>
	<tr>
		<td class="SubHead" width="150" valign="top"><dnn:label id="plTemplateName" text="Template Filename:" controlname="txtTemplateName" runat="server" /></td>
		<td>
			<asp:textbox id="txtTemplateName" runat="server" width="300px" enableviewstate="False"></asp:textbox><br>
			<asp:requiredfieldvalidator id="valFileName" runat="server" errormessage="Template File Name is required" controltovalidate="txtTemplateName"
				display="Dynamic" resourcekey="valFileName.ErrorMessage"></asp:requiredfieldvalidator>
		</td>
	</tr>
	<tr>
		<td class="SubHead" width="150" valign="top"><dnn:label id="plDescription" text="Template Description:" controlname="txtDescription" runat="server" /></td>
		<td>
			<asp:textbox id="txtDescription" runat="server" width="300px" enableviewstate="False" TextMode="MultiLine"
				Height="150px"></asp:textbox><br>
			<asp:requiredfieldvalidator id="valDescription" runat="server" errormessage="Description is required" controltovalidate="txtDescription"
				display="Dynamic" resourcekey="valDescription.ErrorMessage"></asp:requiredfieldvalidator>
		</td>
	</tr>
	<TR>
		<TD class="SubHead" width="150" valign="top">
			<dnn:label id="plContent" runat="server" controlname="chkContent" text="Include Content:"></dnn:label></TD>
		<TD>
			<asp:CheckBox id="chkContent" runat="server"></asp:CheckBox></TD>
	</TR>
</table>
<p>
	<asp:linkbutton id="cmdExport" cssclass="CommandButton" runat="server" resourcekey="cmdExport">Export template</asp:linkbutton>
</p>
<asp:label id="lblMessage" runat="server" enableviewstate="False" CssClass="Normal"></asp:label>
