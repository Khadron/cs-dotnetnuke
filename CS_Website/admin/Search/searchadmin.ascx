<%@ Control Language="C#" AutoEventWireup="false" Inherits="DotNetNuke.Modules.Admin.Search.SearchAdmin" CodeFile="SearchAdmin.ascx.cs" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<table cellSpacing="0" cellPadding="2" width="525" summary="Basic Settings Design Table"
	border="0">
	<tr>
		<td colSpan="2"><asp:label id="lblSearchSettingsHelp" runat="server" enableviewstate="False" cssclass="Normal"></asp:label></td>
	</tr>
	<tr>
		<td class="SubHead" width="200"><dnn:label id="plMaxWordLength" runat="server" controlname="txtMaxWordLength" text="Maximum Word Length:"></dnn:label></td>
		<td class="NormalTextBox" vAlign="top" width="75"><asp:textbox id="txtMaxWordLength" runat="server" cssclass="NormalTextBox" maxlength="128" width="325"></asp:textbox></td>
	</tr>
	<tr>
		<td class="SubHead" width="200"><dnn:label id="plMinWordLength" runat="server" controlname="txtMinWordLength" text="Minimum Word Length:"></dnn:label></td>
		<td class="NormalTextBox" vAlign="top" width="75"><asp:textbox id="txtMinWordLength" runat="server" cssclass="NormalTextBox" maxlength="128" width="325"></asp:textbox></td>
	</tr>
	<tr>
		<td class="SubHead" width="200"><dnn:label id="plIncludeCommon" runat="server" controlname="chkIncludeCommon" text="Include Common Words:"></dnn:label></td>
		<td class="NormalTextBox" vAlign="top" width="75"><asp:CheckBox ID="chkIncludeCommon" Runat="server" CssClass="Normal"></asp:CheckBox></td>
	</tr>
	<tr>
		<td class="SubHead" width="200"><dnn:label id="plIncludeNumeric" runat="server" controlname="chkIncludeNumeric" text="Include Numbers:"></dnn:label></td>
		<td class="NormalTextBox" vAlign="top" width="75"><asp:CheckBox ID="chkIncludeNumeric" Runat="server" CssClass="Normal"></asp:CheckBox></td>
	</tr>
</table>
<p>
	<asp:linkbutton cssclass="CommandButton" id="cmdUpdate" resourcekey="cmdUpdate" runat="server" text="Update"></asp:linkbutton>&nbsp;&nbsp;
	<asp:linkbutton cssclass="CommandButton" id="cmdCancel" resourcekey="cmdCancel" runat="server" text="Cancel"
		causesvalidation="False" borderstyle="none"></asp:linkbutton>&nbsp;&nbsp;
	<asp:linkbutton cssclass="CommandButton" id="cmdReIndex" resourcekey="cmdReIndex" runat="server"
		text="Re-Index Content" causesvalidation="False" borderstyle="none"></asp:linkbutton>&nbsp;&nbsp;
</p>
