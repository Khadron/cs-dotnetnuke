<%@ Register Src="~/controls/LabelControl.ascx" TagName="Label" TagPrefix="dnn" %>
<%@ Control AutoEventWireup="true" CodeFile="EditSkins.ascx.cs" Inherits="DotNetNuke.Modules.Admin.Skins.EditSkins" Language="C#" %>
<center>
    <table border="0" cellpadding="0" cellspacing="0" width="500">
        <tr>
            <td colspan="4">
                &nbsp;</td>
        </tr>
        <tr id="typeRow" runat="server">
            <td align="right" class="SubHead" colspan="2" valign="middle">
                <dnn:Label ID="plType" runat="server" Text="Skin Type:" />
            </td>
            <td align="left" colspan="2">
                &nbsp;&nbsp;
                <asp:CheckBox ID="chkHost" runat="server" AutoPostBack="True" Checked="True" CssClass="SubHead" OnCheckedChanged="chkHost_CheckedChanged" resourcekey="Host" Text="Host" />&nbsp;&nbsp;
                <asp:CheckBox ID="chkSite" runat="server" AutoPostBack="True" Checked="True" CssClass="SubHead" OnCheckedChanged="chkSite_CheckedChanged" resourcekey="Site" Text="Site" />
            </td>
        </tr>
        <tr>
            <td colspan="4">
                &nbsp;</td>
        </tr>
        <tr>
            <td class="SubHead" valign="middle">
                <dnn:Label ID="plSkins" runat="server" ControlName="cboSkins" Suffix=":" />
            </td>
            <td>
                <asp:DropDownList ID="cboSkins" runat="server" AutoPostBack="True" OnSelectedIndexChanged="cboSkins_SelectedIndexChanged">
                </asp:DropDownList></td>
            <td class="SubHead" valign="middle">
                <dnn:Label ID="plContainers" runat="server" ControlName="cboContainers" Suffix=":" />
            </td>
            <td>
                <asp:DropDownList ID="cboContainers" runat="server" AutoPostBack="True" OnSelectedIndexChanged="cboContainers_SelectedIndexChanged">
                </asp:DropDownList></td>
        </tr>
        <tr>
            <td colspan="4">
                &nbsp;</td>
        </tr>
        <tr>
            <td align="center" colspan="4">
                <asp:LinkButton ID="cmdRestore" runat="server" CssClass="CommandButton" OnClick="cmdRestore_Click" resourcekey="cmdRestore">Restore Default Skin</asp:LinkButton></td>
        </tr>
        <tr>
            <td colspan="4">
                &nbsp;</td>
        </tr>
        <tr>
            <td colspan="4">
                <asp:Label ID="lblGallery" runat="server"></asp:Label></td>
        </tr>
        <tr>
            <td align="center" class="SubHead" colspan="4">
                <asp:Panel ID="pnlSkin" runat="server" Visible="False">
                    <asp:Label ID="lblApply" runat="server" resourcekey="ApplyTo">Apply To</asp:Label>: &nbsp;&nbsp;
                    <asp:CheckBox ID="chkPortal" runat="server" Checked="True" CssClass="SubHead" resourcekey="Portal" Text="Portal" />&nbsp;&nbsp;
                    <asp:CheckBox ID="chkAdmin" runat="server" Checked="True" CssClass="SubHead" resourcekey="Admin" Text="Admin" /><br>
                    <br/>
                    <asp:LinkButton ID="cmdParse" runat="server" CssClass="CommandButton" OnClick="cmdParse_Click" resourcekey="cmdParse">Parse Skin Package</asp:LinkButton>&nbsp;&nbsp;
                    <asp:LinkButton ID="cmdDelete" runat="server" CssClass="CommandButton" OnClick="cmdDelete_Click" resourcekey="cmdDelete">Delete Skin Package</asp:LinkButton></asp:Panel>
            </td>
        </tr>
        <tr>
            <td colspan="4">
                &nbsp;</td>
        </tr>
        <tr>
            <td align="center" colspan="4">
                <asp:Panel ID="pnlParse" runat="server" Visible="False">
                    <table border="0" cellpadding="0" cellspacing="0">
                        <tr>
                            <td class="SubHead">
                                <asp:Label ID="lblParseOptions" runat="server" resourcekey="ParseOptions">Parse Options</asp:Label>:</td>
                            <td>
                                <asp:RadioButtonList ID="optParse" runat="server" CssClass="SubHead" RepeatDirection="Horizontal">
                                    <asp:ListItem resourcekey="Localized" Selected="True" Value="L">Localized</asp:ListItem>
                                    <asp:ListItem resourcekey="Portable" Value="P">Portable</asp:ListItem>
                                </asp:RadioButtonList></td>
                        </tr>
                    </table>
                </asp:Panel>
            </td>
        </tr>
        <tr>
            <td colspan="4">
                <asp:Label ID="lblOutput" runat="server" CssClass="Normal" EnableViewState="False"></asp:Label></td>
        </tr>
    </table>
</center>
