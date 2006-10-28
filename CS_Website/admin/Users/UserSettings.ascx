<%@ Control Language="C#" CodeFile="UserSettings.ascx.cs" AutoEventWireup="true" Inherits="DotNetNuke.Modules.Admin.Users.UserSettings" %>
<%@ Register TagPrefix="dnn" TagName="SectionHead" Src="~/controls/SectionHeadControl.ascx" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register TagPrefix="dnn" Assembly="DotNetNuke" Namespace="DotNetNuke.UI.WebControls" %>

<table width="450" border="0">
    <tr>
        <td>
            <dnn:SectionHead id="dshProvider" cssclass="Head" runat="server" text="Provider Settings"
                section="tblProvider" resourcekey="ProviderSettings" isexpanded="True" includerule="True" />
            <table id="tblProvider" runat="server">
                <tr>
                    <td class="Normal">
                        <asp:Label ID="lblprovider" runat="server" resourcekey="ProviderSettingsHelp" /></td>
                </tr>
                <tr>
                    <td align="center">
                        <dnn:PropertyEditorControl ID="ProviderSettings" runat="server" valuedatafield="PropertyValue"
                            namedatafield="Name" LabelStyle-CssClass="SubHead" HelpStyle-CssClass="Help"
                            EditControlStyle-CssClass="NormalTextBox" LabelWidth="300px" EditControlWidth="150px"
                            Width="450px" SortMode="SortOrderAttribute" />
                    </td>
                </tr>
            </table>
        </td>
    </tr>
    <tr>
        <td height="10">
        </td>
    </tr>
    <tr>
        <td>
            <dnn:SectionHead id="dshPassword" cssclass="Head" runat="server" text="Password Settings"
                section="tblProvider" resourcekey="PasswordSettings" isexpanded="True" includerule="True" />
            <table id="Table1" runat="server">
                <tr>
                    <td class="Normal">
                        <asp:Label ID="lblPassword" runat="server" resourcekey="PasswordSettingsHelp" /></td>
                </tr>
                <tr>
                    <td align="center">
                        <dnn:PropertyEditorControl ID="PasswordSettings" runat="Server" valuedatafield="PropertyValue"
                            namedatafield="Name" LabelStyle-CssClass="SubHead" HelpStyle-CssClass="Help"
                            EditControlStyle-CssClass="NormalTextBox" LabelWidth="300px" EditControlWidth="150px"
                            Width="450px" SortMode="SortOrderAttribute" />
                    </td>
                </tr>
            </table>
        </td>
    </tr>
    <tr>
        <td height="10">
        </td>
    </tr>
    <tr>
        <td>
            <dnn:SectionHead id="Sectionhead1" cssclass="Head" runat="server" text="User Accounts Settings"
                section="tblUserAccounts" resourcekey="UserAccounts.Text" isexpanded="True" includerule="True" />
            <table id="tblUserAccounts" runat="server">
                <tr>
                    <td align="center">
                        <dnn:SettingsEditorControl ID="UserSettingsControl" runat="Server" EditControlWidth="200px" LabelWidth="250px" Width="450px" EditControlStyle-CssClass="NormalTextBox" HelpStyle-CssClass="Help"
                            LabelStyle-CssClass="SubHead" EditMode="Edit" />
                    </td>
                </tr>
            </table>
        </td>
    </tr>
</table>
<p>
    <dnn:CommandButton class="CommandButton" ID="cmdUpdate" ResourceKey="cmdUpdate" runat="server"
        ImageUrl="~/images/add.gif" />
    &nbsp;
    <dnn:CommandButton class="CommandButton" ID="cmdCancel" ResourceKey="cmdCancel" runat="server"
        ImageUrl="~/images/lt.gif" CausesValidation="False" />
</p>
