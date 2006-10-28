<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Control Language="C#" AutoEventWireup="false" Inherits="DotNetNuke.Modules.SearchInput.SearchInput" CodeFile="SearchInput.ascx.cs" %>
<table cellSpacing="0" cellPadding="4" summary="Search Input Table" border="0">
	<tr>
		<td nowrap><dnn:label id="plSearch" runat="server" controlname="cboModule" suffix=":"></dnn:label><asp:image id="imgSearch" runat="server"></asp:image></td>
		<td><asp:textbox id="txtSearch" runat="server" Wrap="False" Width="150px" columns="35" maxlength="200"
				cssclass="NormalTextBox"></asp:textbox></td>
		<td><asp:imagebutton id="imgGo" runat="server"></asp:imagebutton><asp:Button id="cmdGo" runat="server" Text="Go"></asp:Button></td>
	</tr>
</table>
