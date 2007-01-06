<%@ Control Language="C#" AutoEventWireup="true"  Inherits="DotNetNuke.Modules.Admin.Modules.Import" CodeFile="Import.ascx.cs" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<br/>
<table width="560" cellspacing="0" cellpadding="0" border="0" summary="Edit Links Design Table">
    <tr>
        <td class="SubHead"><dnn:label id="plFile" runat="server" controlname="ctlFile" suffix=":"></dnn:label></td>
        <td><asp:DropDownList ID="cboFiles" Runat="server" CssClass="NormalTextBox" Width="300"></asp:DropDownList></td>
    </tr>
</table>
<p>
    <asp:linkbutton id="cmdImport" resourcekey="cmdImport" runat="server" cssclass="CommandButton" text="Import" borderstyle="none" OnClick="cmdImport_Click"></asp:linkbutton>&nbsp;
    <asp:linkbutton id="cmdCancel" resourcekey="cmdCancel" runat="server" cssclass="CommandButton" text="Cancel" borderstyle="none" causesvalidation="False" OnClick="cmdCancel_Click"></asp:linkbutton>
</p>
