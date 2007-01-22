<%@ Register Src="~/controls/SectionHeadControl.ascx" TagName="SectionHead" TagPrefix="dnn" %>
<%@ Register Src="~/controls/LabelControl.ascx" TagName="Label" TagPrefix="dnn" %>
<%@ Register Src="~/controls/URLControl.ascx" TagName="URL" TagPrefix="dnn" %>
<%@ Register Assembly="DotNetNuke" Namespace="DotNetNuke.Security.Permissions.Controls" TagPrefix="Portal" %>
<%@ Register Src="~/controls/SkinControl.ascx" TagName="Skin" TagPrefix="dnn" %>
<%@ Control AutoEventWireup="true" CodeFile="ManageTabs.ascx.cs"  Inherits="DotNetNuke.Modules.Admin.Tabs.ManageTabs" Language="C#" %>
<table border="0" cellpadding="2" cellspacing="2" class="Settings" summary="Manage Tabs Design Table">
    <tr>
        <td valign="top" style="width: 560;">
            <asp:Panel ID="pnlSettings" runat="server" CssClass="WorkPanel" Visible="True">
                <dnn:SectionHead ID="dshBasic" runat="server" CssClass="Head" IncludeRule="True" ResourceKey="BasicSettings" Section="tblBasic" Text="Basic Settings" />
                <table id="tblBasic" runat="server" border="0" cellpadding="2" cellspacing="0" summary="Basic Settings Design Table" width="525">
                    <tr>
                        <td colspan="2">
                            <asp:Label ID="lblBasicSettingsHelp" runat="server" CssClass="Normal" EnableViewState="False" resourcekey="BasicSettingsHelp"></asp:Label></td>
                    </tr>
                    <tr>
                        <td style="width: 25;">
                        </td>
                        <td valign="top" style="width: 475;">
                            <dnn:SectionHead ID="dshPage" runat="server" CssClass="Head" ResourceKey="PageDetails" Section="tblPage" Text="Page Details" />
                            <table id="tblPage" runat="server" border="0" cellpadding="2" cellspacing="2" summary="Site Details Design Table">
                                <tr>
                                    <td class="SubHead" style="width: 150;">
                                        <dnn:Label ID="plTabName" runat="server" ControlName="txtTabName" HelpKey="TabNameHelp" ResourceKey="TabName" Suffix=":" />
                                    </td>
                                    <td style="width: 325;">
                                        <asp:TextBox ID="txtTabName" runat="server" CssClass="NormalTextBox" MaxLength="50" Width="300"></asp:TextBox>
                                        <asp:RequiredFieldValidator ID="valTabName" runat="server" ControlToValidate="txtTabName" CssClass="NormalRed" Display="Dynamic" ErrorMessage="<br>Tab Name Is Required"
                                            resourcekey="valTabName.ErrorMessage"></asp:RequiredFieldValidator></td>
                                </tr>
                                <tr>
                                    <td class="SubHead" style="width: 150;">
                                        <dnn:Label ID="plTitle" runat="server" ControlName="txtTitle" HelpKey="TitleHelp" ResourceKey="Title" Suffix=":" />
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtTitle" runat="server" CssClass="NormalTextBox" MaxLength="200" Width="300"></asp:TextBox></td>
                                </tr>
                                <tr>
                                    <td class="SubHead" style="width: 150;">
                                        <dnn:Label ID="plDescription" runat="server" ControlName="txtDescription" HelpKey="DescriptionHelp" ResourceKey="Description" Suffix=":" />
                                    </td>
                                    <td class="NormalTextBox" style="width: 325;">
                                        <asp:TextBox ID="txtDescription" runat="server" CssClass="NormalTextBox" MaxLength="500" Rows="3" TextMode="MultiLine" Width="300"></asp:TextBox></td>
                                </tr>
                                <tr>
                                    <td class="SubHead" style="width: 150;">
                                        <dnn:Label ID="plKeywords" runat="server" ControlName="txtKeyWords" HelpKey="KeyWordsHelp" ResourceKey="KeyWords" Suffix=":" />
                                    </td>
                                    <td class="NormalTextBox" style="width: 325;">
                                        <asp:TextBox ID="txtKeyWords" runat="server" CssClass="NormalTextBox" MaxLength="500" Rows="3" TextMode="MultiLine" Width="300"></asp:TextBox></td>
                                </tr>
                                <tr>
                                    <td class="SubHead" style="width: 150;">
                                        <dnn:Label ID="plParentTab" runat="server" ControlName="cboTab" HelpKey="ParentTabHelp" ResourceKey="ParentTab" Suffix=":" />
                                    </td>
                                    <td style="width: 325;">
                                        <asp:DropDownList ID="cboTab" runat="server" CssClass="NormalTextBox" DataTextField="TabName" DataValueField="TabId" Width="300">
                                        </asp:DropDownList></td>
                                </tr>
                                <tr id="rowTemplate" runat="server">
                                    <td class="SubHead" style="width: 150;">
                                        <dnn:Label ID="plTemplate" runat="server" ControlName="cboTemplate" HelpKey="TemplateHelp" ResourceKey="Template" Suffix=":" />
                                    </td>
                                    <td style="width: 325;">
                                        <asp:DropDownList ID="cboTemplate" runat="server" CssClass="NormalTextBox" DataTextField="Text" DataValueField="Value" Width="300">
                                        </asp:DropDownList></td>
                                </tr>
                                <tr>
                                    <td class="SubHead" valign="top" style="width: 150;">
                                        <br/>
                                        <br/>
                                        <dnn:Label ID="plPermissions" runat="server" ControlName="dgPermissions" HelpKey="PermissionsHelp" ResourceKey="Permissions" Suffix=":" />
                                    </td>
                                    <td style="width: 325;">
                                        <Portal:TabPermissionsGrid ID="dgPermissions" runat="server">
                                        </Portal:TabPermissionsGrid>
                                    </td>
                                </tr>
                                <tr id="rowCopyPerm" runat="server">
                                    <td class="SubHead">
                                        <dnn:Label ID="plCopyPerm" runat="server" ResourceKey="plCopyPerm" />
                                    </td>
                                    <td>
                                        <asp:LinkButton ID="cmdCopyPerm" runat="server" CssClass="CommandButton" resourcekey="cmdCopyPerm"></asp:LinkButton>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
                <br/>
                <dnn:SectionHead ID="dshCopy" runat="server" CssClass="Head" IncludeRule="True" ResourceKey="Copy" Section="tblCopy" Text="Copy Page" />
                <table id="tblCopy" runat="server" border="0" cellpadding="2" cellspacing="0" summary="Copy Tab Design Table" width="525">
                    <tr>
                        <td style="width: 25;">
                        </td>
                        <td class="SubHead" style="width: 150;">
                            <dnn:Label ID="plCopyPage" runat="server" ControlName="cboCopyPage" HelpKey="CopyModulesHelp" ResourceKey="CopyModules" Suffix=":" />
                        </td>
                        <td style="width: 325;">
                            <asp:DropDownList ID="cboCopyPage" runat="server" AutoPostBack="True" CssClass="NormalTextBox" DataTextField="TabName" DataValueField="TabId" OnSelectedIndexChanged="cboCopyPage_SelectedIndexChanged"
                                style="width: 300;">
                            </asp:DropDownList></td>
                    </tr>
                    <tr id="rowModules" runat="server">
                        <td style="width: 25;">
                        </td>
                        <td class="SubHead" colspan="2">
                            <dnn:Label ID="plModules" runat="server" ControlName="grdModules" HelpKey="CopyContentHelp" ResourceKey="CopyContent" Suffix=":" />
                            <br/>
                            <asp:DataGrid ID="grdModules" runat="server" AutoGenerateColumns="False" BorderStyle="None" BorderWidth="0px" CellPadding="0" CellSpacing="0" DataKeyField="ModuleID"
                                GridLines="None" ItemStyle-CssClass="Normal" ShowHeader="False" Width="100%">
                                <Columns>
                                    <asp:TemplateColumn>
                                        <ItemTemplate>
                                            <asp:CheckBox ID="chkModule" runat="server" Checked="True" CssClass="NormalTextBox" />
                                        </ItemTemplate>
                                    </asp:TemplateColumn>
                                    <asp:TemplateColumn>
                                        <ItemTemplate>
                                            <asp:TextBox ID="txtCopyTitle" runat="server" CssClass="NormalTextBox" Text='<%# DataBinder.Eval(Container.DataItem,"ModuleTitle")%>' Width="150">
                                            </asp:TextBox>
                                        </ItemTemplate>
                                        <ItemStyle Wrap="False" />
                                    </asp:TemplateColumn>
                                    <asp:BoundColumn runat="server" DataField="PaneName"></asp:BoundColumn>
                                    <asp:TemplateColumn>
                                        <ItemTemplate>
                                            <asp:RadioButton ID="optNew" runat="server" Checked="True" CssClass="NormalBold" GroupName="Copy" resourcekey="ModuleNew" Text="New" />
                                            <asp:RadioButton ID="optCopy" runat="server" CssClass="NormalBold" Enabled='<%# DataBinder.Eval(Container.DataItem, "IsPortable") %>' GroupName="Copy" resourcekey="ModuleCopy"
                                                Text="Copy" />
                                            <asp:RadioButton ID="optReference" runat="server" CssClass="NormalBold" Enabled='<%# Convert.ToInt32(DataBinder.Eval(Container.DataItem, "ModuleID")) != -1  %>'
                                                GroupName="Copy" resourcekey="ModuleReference" Text="Reference" />
                                        </ItemTemplate>
                                        <ItemStyle Wrap="False" />
                                    </asp:TemplateColumn>
                                </Columns>
                            </asp:DataGrid></td>
                    </tr>
                </table>
                <br/>
                <dnn:SectionHead ID="dshAdvanced" runat="server" CssClass="Head" IncludeRule="True" IsExpanded="False" ResourceKey="AdvancedSettings" Section="tblAdvanced" Text="Advanced Settings" />
                <table id="tblAdvanced" runat="server" border="0" cellpadding="2" cellspacing="0" summary="Advanced Settings Design Table" width="525">
                    <tr>
                        <td colspan="2">
                            <asp:Label ID="lblAdvancedSettingsHelp" runat="server" CssClass="Normal" EnableViewState="False" resourcekey="AdvancedSettingsHelp"></asp:Label></td>
                    </tr>
                    <tr>
                        <td style="width: 25;">
                        </td>
                        <td valign="top" style="width: 475;">
                            <dnn:SectionHead ID="dhsAppearance" runat="server" CssClass="Head" ResourceKey="Appearance" Section="tblAppearance" Text="Appearance" />
                            <table id="tblAppearance" runat="server" border="0" cellpadding="2" cellspacing="2" summary="Appearance Design Table">
                                <tr>
                                    <td class="SubHead" valign="top" style="width: 150;">
                                        <dnn:Label ID="plIcon" runat="server" ControlName="ctlIcon" HelpKey="IconHelp" ResourceKey="Icon" Suffix=":" />
                                    </td>
                                    <td style="width: 325;">
                                        <dnn:URL ID="ctlIcon" runat="server" ShowLog="False" Width="300" />
                                    </td>
                                </tr>
                                <tr>
                                    <td class="SubHead" valign="top" style="width: 150;">
                                        <dnn:Label ID="plSkin" runat="server" ControlName="ctlSkin" HelpKey="TabSkinHelp" ResourceKey="TabSkin" Suffix=":" />
                                    </td>
                                    <td valign="top" style="width: 325;">
                                        <dnn:Skin ID="ctlSkin" runat="server" />
                                    </td>
                                </tr>
                                <tr>
                                    <td class="SubHead" valign="top" style="width: 150;">
                                        <dnn:Label ID="plContainer" runat="server" ControlName="ctlContainer" HelpKey="TabContainerHelp" ResourceKey="TabContainer" Suffix=":" />
                                    </td>
                                    <td valign="top" style="width: 325;">
                                        <dnn:Skin ID="ctlContainer" runat="server" />
                                    </td>
                                </tr>
                                <tr id="rowCopySkin" runat="server">
                                    <td class="SubHead">
                                        <dnn:Label ID="plCopySkin" runat="server" ResourceKey="plCopySkin" />
                                    </td>
                                    <td>
                                        <asp:LinkButton ID="cmdCopySkin" runat="server" CssClass="CommandButton" resourcekey="cmdCopySkin"></asp:LinkButton>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="SubHead" valign="top" style="width: 150;">
                                        <dnn:Label ID="plHidden" runat="server" ControlName="chkHidden" HelpKey="HiddenHelp" ResourceKey="Hidden" Suffix=":" />
                                    </td>
                                    <td style="width: 325;">
                                        <asp:CheckBox ID="chkHidden" runat="server" Font-Names="Verdana,Arial" Font-Size="8pt" /></td>
                                </tr>
                                <tr>
                                    <td class="SubHead" valign="top" style="width: 150;">
                                        <dnn:Label ID="plDisable" runat="server" ControlName="chkDisableLink" HelpKey="DisabledHelp" ResourceKey="Disabled" Suffix=":" />
                                    </td>
                                    <td style="width: 325;">
                                        <asp:CheckBox ID="chkDisableLink" runat="server" Font-Names="Verdana,Arial" Font-Size="8pt" /></td>
                                </tr>
                                <tr>
                                    <td class="SubHead" style="width: 150;">
                                        <dnn:Label ID="plRefreshInterval" runat="server" ControlName="cboRefreshInterval" HelpKey="RefreshInterval.Help" ResourceKey="RefreshInterval" Suffix=":" />
                                    </td>
                                    <td style="width: 325;">
                                        <asp:TextBox ID="txtRefreshInterval" runat="server" CssClass="NormalTextBox"></asp:TextBox></td>
                                </tr>
                                <tr>
                                    <td class="SubHead" valign="top" style="width: 150;">
                                        <dnn:Label ID="plPageHeadText" runat="server" ControlName="txtPageHeadText" HelpKey="PageHeadText.Help" ResourceKey="PageHeadText" Suffix=":" />
                                    </td>
                                    <td class="NormalTextBox" style="width: 325;">
                                        <asp:TextBox ID="txtPageHeadText" runat="server" Columns="50" CssClass="NormalTextBox" Rows="4" TextMode="MultiLine"></asp:TextBox></td>
                                </tr>
                            </table>
                            <br/>
                            <dnn:SectionHead ID="dshOther" runat="server" CssClass="Head" ResourceKey="OtherSettings" Section="tblOther" Text="Other Settings" />
                            <table id="tblOther" runat="server" border="0" cellpadding="2" cellspacing="2" summary="Appearance Design Table">
                                <tr>
                                    <td class="SubHead" valign="top" style="width: 150;">
                                        <dnn:Label ID="plStartDate" runat="server" ControlName="txtStartDate" Text="Start Date:" />
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtStartDate" runat="server" Columns="30" CssClass="NormalTextBox" MaxLength="11" Width="120"></asp:TextBox>&nbsp;
                                        <asp:HyperLink ID="cmdStartCalendar" runat="server" CssClass="CommandButton" resourcekey="Calendar">Calendar</asp:HyperLink>
                                        <asp:CompareValidator ID="valtxtStartDate" runat="server" ControlToValidate="txtStartDate" Display="Dynamic" ErrorMessage="<br>Invalid Start Date" Operator="DataTypeCheck"
                                            resourcekey="valStartDate.ErrorMessage" Type="Date"></asp:CompareValidator></td>
                                </tr>
                                <tr>
                                    <td class="SubHead" valign="top" width="150">
                                        <dnn:Label ID="plEndDate" runat="server" ControlName="txtEndDate" Text="End Date:" />
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtEndDate" runat="server" Columns="30" CssClass="NormalTextBox" MaxLength="11" Width="120"></asp:TextBox>&nbsp;
                                        <asp:HyperLink ID="cmdEndCalendar" runat="server" CssClass="CommandButton" resourcekey="Calendar">Calendar</asp:HyperLink>
                                        <asp:CompareValidator ID="valtxtEndDate" runat="server" ControlToValidate="txtEndDate" Display="Dynamic" ErrorMessage="<br>Invalid End Date" Operator="DataTypeCheck"
                                            resourcekey="valEndDate.ErrorMessage" Type="Date"></asp:CompareValidator></td>
                                </tr>
                                <tr>
                                    <td class="SubHead" valign="top" style="width: 150;">
                                        <dnn:Label ID="plURL" runat="server" ControlName="ctlURL" HelpKey="UrlHelp" ResourceKey="Url" Suffix=":" />
                                    </td>
                                    <td class="NormalTextBox" style="width: 325;">
                                        <dnn:URL ID="ctlURL" runat="server" ShowLog="False" ShowNone="True" ShowTrack="False" Width="300" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </asp:Panel>
        </td>
    </tr>
</table>
<p>
    <asp:LinkButton ID="cmdUpdate" runat="server" CssClass="CommandButton" OnClick="cmdUpdate_Click" resourcekey="cmdUpdate" Text="Update"></asp:LinkButton>&nbsp;&nbsp;
    <asp:LinkButton ID="cmdCancel" runat="server" BorderStyle="none" CausesValidation="False" CssClass="CommandButton" OnClick="cmdCancel_Click" resourcekey="cmdCancel"
        Text="Cancel"></asp:LinkButton>&nbsp;&nbsp;
    <asp:LinkButton ID="cmdDelete" runat="server" BorderStyle="none" CausesValidation="False" CssClass="CommandButton" OnClick="cmdDelete_Click" resourcekey="cmdDelete"
        Text="Delete"></asp:LinkButton>h    
</p>
