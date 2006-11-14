<%@ Control Language="C#" AutoEventWireup="true" Explicit="True" Inherits="DotNetNuke.Modules.Admin.Portals.EditPortalAlias" CodeFile="EditPortalAlias.ascx.cs" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<br>
<table border="0" cellspacing="2" cellpadding="2">
	<tr>
		<td class="SubHead"><dnn:label id="plAlias" runat="server" controlname="txtAlias" suffix=":"></dnn:label></td>
		<td><asp:TextBox ID="txtAlias" Runat="server" CssClass="NormalTextBox" MaxLength="255" Width="300"/></td>
	</tr>
</table>
<p>
    <asp:linkbutton cssclass="CommandButton" id="cmdUpdate" resourcekey="cmdUpdate" runat="server" text="Update"></asp:linkbutton>&nbsp;&nbsp;
    <asp:linkbutton cssclass="CommandButton" id="cmdCancel" resourcekey="cmdCancel" runat="server" text="Cancel" causesvalidation="False" borderstyle="none"></asp:linkbutton>&nbsp;&nbsp;
    <asp:linkbutton cssclass="CommandButton" id="cmdDelete" resourcekey="cmdDelete" runat="server" text="Delete" causesvalidation="False" borderstyle="none"></asp:linkbutton>
</p>

