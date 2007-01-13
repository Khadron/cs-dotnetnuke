<%@ Control AutoEventWireup="true" CodeFile="RecycleBin.ascx.cs" Inherits="DotNetNuke.Modules.Admin.Tabs.RecycleBin" Language="C#" %>
<%@ Register Src="~/controls/LabelControl.ascx" TagName="Label" TagPrefix="dnn" %>
<%@ Register Src="~/controls/HelpButtonControl.ascx" TagName="HelpButton" TagPrefix="dnn" %>
<%@ Register Src="~/controls/SectionHeadControl.ascx" TagName="SectionHead" TagPrefix="dnn" %>
<table border="0" cellpadding="2" cellspacing="2" class="Settings" summary="Recycle Bin Design Table">
    <tr>
        <td width="560">
            <asp:Panel ID="pnlTabs" runat="server" CssClass="WorkPanel" Visible="True">
                <dnn:SectionHead ID="dshBasic" runat="server" CssClass="Head" IncludeRule="True" ResourceKey="Tabs" Section="tblTabs" Text="Tabs" />
                <table id="tblTabs" runat="server" border="0" cellpadding="2" cellspacing="0" summary="Tbas Design Table" width="525">
                    <tr valign="top">
                        <td width="250">
                            <asp:ListBox ID="lstTabs" runat="server" CssClass="Normal" DataTextField="TabName" DataValueField="TabId" Rows="5" SelectionMode="Multiple" Width="350px"></asp:ListBox>
                        </td>
                        <td valign="top">
                            <table summary="Tabs Design Table">
                                <tr>
                                    <td valign="top">
                                        <asp:ImageButton ID="cmdRestoreTab" runat="server" ImageUrl="~/images/restore.gif" OnClick="cmdRestoreTab_Click" resourcekey="cmdRestoreTab" />
                                    </td>
                                    <td valign="top">
                                        <dnn:HelpButton ID="hbtnRestoreTabHelp" runat="server" ResourceKey="cmdRestoreTab" />
                                    </td>
                                </tr>
                                <tr>
                                    <td valign="top">
                                        <asp:ImageButton ID="cmdDeleteTab" runat="server" ImageUrl="~/images/delete.gif" OnClick="cmdDeleteTab_Click" resourcekey="cmdDeleteTab" /></td>
                                    <td valign="top">
                                        <dnn:HelpButton ID="hbtnDeleteTabHelp" runat="server" ResourceKey="cmdDeleteTab" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
                <br>
                <dnn:SectionHead ID="Sectionhead1" runat="server" CssClass="Head" IncludeRule="True" ResourceKey="Modules" Section="tblModules" Text="Modules" />
                <table id="tblModules" runat="server" border="0" cellpadding="0" cellspacing="0" summary="Basic Settings Design Table" width="525">
                    <tr valign="Top">
                        <td width="250">
                            <asp:ListBox ID="lstModules" runat="server" CssClass="Normal" DataTextField="ModuleTitle" DataValueField="ModuleId" Rows="5" SelectionMode="Multiple" Width="350px">
                            </asp:ListBox>
                        </td>
                        <td valign="top">
                            <table summary="Tabs Design Table">
                                <tr>
                                    <td valign="top">
                                        <asp:ImageButton ID="cmdRestoreModule" runat="server" AlternateText="Restore Module" ImageUrl="~/images/restore.gif" OnClick="cmdRestoreModule_Click" />
                                    </td>
                                    <td valign="top">
                                        <dnn:HelpButton ID="hbtnRestoreModuleHelp" runat="server" ResourceKey="cmdRestoreModule" />
                                    </td>
                                </tr>
                                <tr>
                                    <td valign="top">
                                        <asp:ImageButton ID="cmdDeleteModule" runat="server" AlternateText="Delete Module" ImageUrl="~/images/delete.gif" OnClick="cmdDeleteModule_Click" />
                                    </td>
                                    <td valign="top">
                                        <dnn:HelpButton ID="hbtnDeleteModuleHelp" runat="server" ResourceKey="cmdDeleteModule" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td align="center" class="SubHead">
                            <dnn:Label ID="plTab" runat="server" ControlName="cboTab" HelpKey="TabHelp" ResourceKey="Tab" Suffix=":" />
                        </td>
                        <td>
                            &nbsp;</td>
                    </tr>
                    <tr>
                        <td>
                            <asp:DropDownList ID="cboTab" runat="server" CssClass="NormalTextBox" DataTextField="TabName" DataValueField="TabId" Width="350">
                            </asp:DropDownList></td>
                        <td>
                            &nbsp;</td>
                    </tr>
                </table>
            </asp:Panel>
        </td>
        <td width="10">
            &nbsp;</td>
    </tr>
</table>
<p>
    <asp:LinkButton ID="cmdEmpty" runat="server" CssClass="CommandButton" OnClick="cmdEmpty_Click" resourcekey="cmdEmpty">Empty Recycle Bin</asp:LinkButton>
</p>
