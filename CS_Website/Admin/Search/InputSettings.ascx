<%@ Register Src="~/controls/LabelControl.ascx" TagName="Label" TagPrefix="dnn" %>
<%@ Control AutoEventWireup="true" CodeFile="InputSettings.ascx.cs" Explicit="True" Inherits="DotNetNuke.Modules.SearchInput.InputSettings" Language="C#" %>
<table border="0" cellpadding="2" cellspacing="2">
    <tr>
        <td class="SubHead" nowrap="nowrap" width="180">
            <dnn:Label ID="plModuleCombo" runat="server" ControlName="cboModule" Text="Search Results Module:" />
        </td>
        <td width="300">
            <asp:Label ID="txtModule" runat="server" CssClass="NormalBold"></asp:Label>
            <asp:DropDownList ID="cboModule" runat="server" Width="300">
            </asp:DropDownList>
        </td>
    </tr>
    <tr>
        <td class="SubHead" nowrap="nowrap" width="180">
            <dnn:Label ID="plGoCheck" runat="server" ControlName="chkGo" Text="Show Go Image:" />
        </td>
        <td width="300">
            <asp:CheckBox ID="chkGo" runat="server" /></td>
    </tr>
    <tr>
        <td class="SubHead" nowrap="nowrap" width="180">
            <dnn:Label ID="plSearchCheck" runat="server" ControlName="chkSearchImage" Text="Show Search Image:" />
        </td>
        <td width="300">
            <asp:CheckBox ID="chkSearchImage" runat="server" /></td>
    </tr>
</table>
