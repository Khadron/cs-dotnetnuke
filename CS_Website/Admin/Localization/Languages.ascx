<%@ Control AutoEventWireup="true" CodeFile="Languages.ascx.cs" Inherits="DotNetNuke.Services.Localization.Languages" Language="C#" %>
<%@ Register Src="~/controls/SectionHeadControl.ascx" TagName="SectionHead" TagPrefix="dnn" %>
<%@ Register Src="~/controls/LabelControl.ascx" TagName="Label" TagPrefix="dnn" %>
<dnn:SectionHead ID="dshBasic" runat="server" CssClass="Head" IncludeRule="False" ResourceKey="SupportedLocales" Section="tblBasic" Text="Suported Locales" />
<br/>
<table id="tblBasic" runat="server" border="0" cellpadding="1" cellspacing="1">
    <tr>
        <td nowrap="nowrap">
            <asp:DataGrid ID="dgLocales" runat="server" AutoGenerateColumns="False" CellPadding="4" CssClass="Normal" GridLines="None" OnItemCommand="dgLocales_ItemCommand" OnItemCreated="dgLocales_ItemCreated" OnItemDataBound="dgLocales_ItemDataBound">
                <AlternatingItemStyle Wrap="False" />
                <ItemStyle Wrap="False" />
                <HeaderStyle Font-Bold="True" Wrap="False" />
                <Columns>
                    <asp:BoundColumn DataField="name" HeaderText="Name" ReadOnly="True"></asp:BoundColumn>
                    <asp:BoundColumn DataField="key" HeaderText="Key" ReadOnly="True"></asp:BoundColumn>
                    <asp:TemplateColumn HeaderText="Status">
                        <ItemTemplate>
                            <asp:Label ID="lblStatus" runat="server" CssClass="Normal"></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateColumn>
                    <asp:TemplateColumn>
                        <ItemTemplate>
                            <asp:LinkButton ID="cmdDisable" runat="server" CausesValidation="false" CommandName="Disable" CssClass="CommandButton" Text="Disable"></asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateColumn>
                    <asp:TemplateColumn>
                        <ItemTemplate>
                            <asp:LinkButton ID="cmdDelete" runat="server" CausesValidation="False" CommandName="Delete" CssClass="CommandButton" resourcekey="cmdDelete" Text="Delete" Visible="<%# UserInfo.IsSuperUser %>">Delete</asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateColumn>
                </Columns>
            </asp:DataGrid>
            <asp:CheckBox ID="chkDeleteFiles" runat="server" CssClass="Normal" resourcekey="DeleteFiles" Text="Delete all resources of this locale, too" />
            <p><asp:CheckBox ID="chkEnableBrowser" runat="server" AutoPostBack="True" CssClass="SubHead" Text="Enable Browser Language Detection" TextAlign="left" OnCheckedChanged="chkEnableBrowser_CheckedChanged" />&nbsp;</p>
        </td>
    </tr>
</table>
<asp:Panel ID="pnlAdd" runat="server">
    
    <dnn:SectionHead ID="dshAdd" runat="server" CssClass="Head" IncludeRule="False" ResourceKey="AddNewLocale" Section="tblAdd" Text="Add New Locale" />
    <br/>
    <table id="tblAdd" runat="server" border="0" cellpadding="1" cellspacing="1">
        <tr>
            <td nowrap="nowrap">
            </td>
            <td nowrap="nowrap">
                <asp:RadioButtonList ID="rbDisplay" runat="server" AutoPostBack="True" CssClass="Normal" OnSelectedIndexChanged="rbDisplay_SelectedIndexChanged" RepeatDirection="Horizontal"
                    RepeatLayout="Flow">
                    <asp:ListItem resourcekey="DisplayEnglish" Value="English">English</asp:ListItem>
                    <asp:ListItem resourcekey="DisplayNative" Selected="True" Value="Native">Native</asp:ListItem>
                </asp:RadioButtonList></td>
        </tr>
        <tr>
            <td class="SubHead" nowrap="nowrap" valign="top" width="150">
                <dnn:Label ID="lbLocale" runat="server" ControlName="txtName" Text="Name" />
            </td>
            <td nowrap="nowrap" valign="top">
                <asp:DropDownList ID="cboLocales" runat="server">
                </asp:DropDownList></td>
        </tr>
        <tr>
            <td nowrap="nowrap">
            </td>
            <td nowrap="nowrap">
                <asp:LinkButton ID="cmdAdd" runat="server" CssClass="CommandButton" OnClick="cmdAdd_Click" resourcekey="Add">Add</asp:LinkButton></td>
        </tr>
    </table>
    
</asp:Panel>
