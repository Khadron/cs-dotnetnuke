<%@ Register Src="~/controls/DualListControl.ascx" TagName="DualList" TagPrefix="Portal" %>
<%@ Register Src="~/controls/URLControl.ascx" TagName="URL" TagPrefix="Portal" %>
<%@ Register Src="~/controls/SkinControl.ascx" TagName="Skin" TagPrefix="dnn" %>
<%@ Register Src="~/controls/LabelControl.ascx" TagName="Label" TagPrefix="dnn" %>
<%@ Register Src="~/controls/SectionHeadControl.ascx" TagName="SectionHead" TagPrefix="dnn" %>
<%@ Control AutoEventWireup="true" CodeFile="SiteSettings.ascx.cs" EnableViewState="True" Inherits="DotNetNuke.Modules.Admin.PortalManagement.SiteSettings" Language="C#" %>
<!-- Settings Tables -->
<table border="0" cellpadding="2" cellspacing="2" class="Settings" summary="Site Settings Design Table" style="width: 560;">
    <tr>
        <td valign="top" style="width: 560;">
            <asp:Panel ID="pnlSettings" runat="server" CssClass="WorkPanel" Visible="True">
                <dnn:SectionHead ID="dshBasic" runat="server" CssClass="Head" IncludeRule="True" ResourceKey="BasicSettings" Section="tblBasic" Text="Basic Settings" />
                <table id="tblBasic" runat="server" border="0" cellpadding="2" cellspacing="0" summary="Basic Settings Design Table" style="width: 525;">
                    <tr>
                        <td colspan="2">
                            <asp:Label ID="lblBasicSettingsHelp" runat="server" CssClass="Normal" EnableViewState="False" resourcekey="BasicSettingsHelp">In this section, you can set up the basic settings for your site.</asp:Label></td>
                    </tr>
                    <tr>
                        <td style="width: 25;">
                        </td>
                        <td valign="top" style="width: 475;">
                            <dnn:SectionHead ID="dshSite" runat="server" CssClass="Head" ResourceKey="SiteDetails" Section="tblSite" Text="Site Details" />
                            <table id="tblSite" runat="server" border="0" cellpadding="2" cellspacing="2" summary="Site Details Design Table">
                                <tr>
                                    <td class="SubHead" style="width: 150;">
                                        <dnn:Label ID="plPortalName" runat="server" ControlName="txtPortalName" Text="Title:" />
                                    </td>
                                    <td class="NormalTextBox" valign="top" style="width: 325;">
                                        <asp:TextBox ID="txtPortalName" runat="server" CssClass="NormalTextBox" MaxLength="128" Width="325"></asp:TextBox></td>
                                </tr>
                                <tr>
                                    <td class="SubHead" valign="top" style="width: 150;">
                                        <dnn:Label ID="plDescription" runat="server" ControlName="txtDescription" Text="Description:" />
                                    </td>
                                    <td class="NormalTextBox" style="width: 325;">
                                        <asp:TextBox ID="txtDescription" runat="server" CssClass="NormalTextBox" MaxLength="475" Rows="3" TextMode="MultiLine" Width="325"></asp:TextBox></td>
                                </tr>
                                <tr>
                                    <td class="SubHead" valign="top" style="width: 150;">
                                        <dnn:Label ID="plKeyWords" runat="server" ControlName="txtKeyWords" Text="Key Words:" />
                                    </td>
                                    <td class="NormalTextBox" style="width: 325;">
                                        <asp:TextBox ID="txtKeyWords" runat="server" CssClass="NormalTextBox" MaxLength="475" Rows="3" TextMode="MultiLine" Width="325"></asp:TextBox></td>
                                </tr>
                            </table>
                            <br/>
                            <dnn:SectionHead ID="dshAppearance" runat="server" CssClass="Head" ResourceKey="Appearance" Section="tblAppearance" Text="Appearance" />
                            <table id="tblAppearance" runat="server" border="0" cellpadding="2" cellspacing="2" summary="Appearance Design Table">
                                <tr>
                                    <td class="SubHead" valign="top" style="width: 150;">
                                        <dnn:Label ID="plLogo" runat="server" ControlName="ctlLogo" Suffix="" />
                                    </td>
                                    <td style="width: 325;">
                                        <Portal:URL ID="ctlLogo" runat="server" Required="False" ShowLog="False" ShowTabs="False" ShowTrack="False" ShowUrls="False" Width="325" />
                                    </td>
                                </tr>
                                <tr>
                                    <td class="SubHead" valign="top" style="width: 150;">
                                        <dnn:Label ID="plBackground" runat="server" ControlName="cboBackground" Text="Body Background:" />
                                    </td>
                                    <td style="width: 325;">
                                        <Portal:URL ID="ctlBackground" runat="server" Required="False" ShowLog="False" ShowTabs="False" ShowTrack="False" ShowUrls="False" Width="325" />
                                    </td>
                                </tr>
                                <tr>
                                    <td class="SubHead" style="width: 150;">
                                        <dnn:Label ID="plPortalSkin" runat="server" ControlName="ctlPortalSkin" Text="Portal Skin:" />
                                    </td>
                                    <td valign="top" style="width: 325;">
                                        <dnn:Skin ID="ctlPortalSkin" runat="server" />
                                    </td>
                                </tr>
                                <tr>
                                    <td class="SubHead" style="width: 150;">
                                        <dnn:Label ID="plPortalContainer" runat="server" ControlName="ctlPortalContainer" Text="Portal Container:" />
                                    </td>
                                    <td valign="top" style="width: 325;">
                                        <dnn:Skin ID="ctlPortalContainer" runat="server" />
                                    </td>
                                </tr>
                                <tr>
                                    <td class="SubHead" style="width: 150;">
                                        <dnn:Label ID="plAdminSkin" runat="server" ControlName="ctlAdminSkin" Text="Admin Skin:" />
                                    </td>
                                    <td valign="top" style="width: 325;">
                                        <dnn:Skin ID="ctlAdminSkin" runat="server" />
                                    </td>
                                </tr>
                                <tr>
                                    <td class="SubHead" style="width: 150;">
                                        <dnn:Label ID="plAdminContainer" runat="server" ControlName="ctlAdminContainer" Text="Admin Container:" />
                                    </td>
                                    <td valign="top" style="width: 325;">
                                        <dnn:Skin ID="ctlAdminContainer" runat="server" />
                                    </td>
                                </tr>
                                <tr>
                                    <td align="center" colspan="2">
                                        <br/>
                                        <asp:HyperLink ID="lnkUploadSkin" runat="server" CssClass="CommandButton" resourcekey="SkinUpload">Upload 
                  Skin</asp:HyperLink>&nbsp;&nbsp;
                                        <asp:HyperLink ID="lnkUploadContainer" runat="server" CssClass="CommandButton" resourcekey="ContainerUpload">Upload 
                  Container</asp:HyperLink></td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
                <br/>
                <dnn:SectionHead ID="dshAdvanced" runat="server" CssClass="Head" IncludeRule="True" IsExpanded="False" ResourceKey="AdvancedSettings" Section="tblAdvanced" Text="Advanced Settings" />
                <table id="tblAdvanced" runat="server" border="0" cellpadding="2" cellspacing="0" summary="Advanced Settings Design Table" width="525">
                    <tr>
                        <td colspan="2">
                            <asp:Label ID="lblAdvancedSettingsHelp" runat="server" CssClass="Normal" EnableViewState="False" resourcekey="AdvancedSettingsHelp">In this section, you can set up more advanced settings for your site.</asp:Label></td>
                    </tr>
                    <tr>
                        <td style="width: 25;">
                        </td>
                        <td valign="top" style="width: 475;">
                            <dnn:SectionHead ID="dhsSecurity" runat="server" CssClass="Head" ResourceKey="SecuritySettings" Section="tblSecurity" Text="Security Settings" />
                            <table id="tblSecurity" runat="server" border="0" cellpadding="2" cellspacing="2" summary="Security Settings Design Table">
                                <tr>
                                    <td class="SubHead" style="width: 150;">
                                        <dnn:Label ID="plUserRegistration" runat="server" ControlName="optUserRegistration" Text="User Registration:" />
                                    </td>
                                    <td class="NormalTextBox" valign="top" style="width: 325;">
                                        <asp:RadioButtonList ID="optUserRegistration" runat="server" CssClass="Normal" EnableViewState="False" RepeatDirection="Horizontal">
                                            <asp:ListItem resourcekey="None" Value="0">None</asp:ListItem>
                                            <asp:ListItem resourcekey="Private" Value="1">Private</asp:ListItem>
                                            <asp:ListItem resourcekey="Public" Value="2">Public</asp:ListItem>
                                            <asp:ListItem resourcekey="Verified" Value="3">Verified</asp:ListItem>
                                        </asp:RadioButtonList></td>
                                </tr>
                            </table>
                            <br/>
                            <dnn:SectionHead ID="dshPages" runat="server" CssClass="Head" ResourceKey="Pages" Section="tblPages" Text="Page Management" />
                            <table id="tblPages" runat="server" border="0" cellpadding="2" cellspacing="2" summary="Page Management Design Table">
                                <tr>
                                    <td class="SubHead" style="width: 150;">
                                        <dnn:Label ID="plSplashTabId" runat="server" ControlName="cboSplashTabId" Text="Splash Page:" />
                                    </td>
                                    <td class="NormalTextBox" valign="top" style="width: 325;">
                                        <asp:DropDownList ID="cboSplashTabId" runat="server" CssClass="NormalTextBox" DataTextField="TabName" DataValueField="TabId" Width="325">
                                        </asp:DropDownList></td>
                                </tr>
                                <tr>
                                    <td class="SubHead" style="width: 150;">
                                        <dnn:Label ID="plHomeTabId" runat="server" ControlName="cboHomeTabId" Text="Home Page:" />
                                    </td>
                                    <td class="NormalTextBox" valign="top" style="width: 325;">
                                        <asp:DropDownList ID="cboHomeTabId" runat="server" CssClass="NormalTextBox" DataTextField="TabName" DataValueField="TabId" Width="325">
                                        </asp:DropDownList></td>
                                </tr>
                                <tr>
                                    <td class="SubHead" style="width: 150;">
                                        <dnn:Label ID="plLoginTabId" runat="server" ControlName="cboLoginTabId" Text="Login Page:" />
                                    </td>
                                    <td class="NormalTextBox" valign="top" style="width: 325;">
                                        <asp:DropDownList ID="cboLoginTabId" runat="server" CssClass="NormalTextBox" DataTextField="TabName" DataValueField="TabId" Width="325">
                                        </asp:DropDownList></td>
                                </tr>
                                <tr>
                                    <td class="SubHead" style="width: 150;">
                                        <dnn:Label ID="plUserTabId" runat="server" ControlName="cboUserTabId" Text="User Page:" />
                                    </td>
                                    <td class="NormalTextBox" valign="top" style="width: 325;">
                                        <asp:DropDownList ID="cboUserTabId" runat="server" CssClass="NormalTextBox" DataTextField="TabName" DataValueField="TabId" Width="325">
                                        </asp:DropDownList></td>
                                </tr>
                                <tr>
                                    <td class="SubHead" valign="top" style="width: 150;">
                                        <dnn:Label ID="plHomeDirectory" runat="server" ControlName="txtHomeDirectory" Text="Home Directory:" />
                                    </td>
                                    <td class="NormalTextBox" style="width: 325;">
                                        <asp:TextBox ID="txtHomeDirectory" runat="server" CssClass="NormalTextBox" Enabled="False" MaxLength="100" Width="325"></asp:TextBox></td>
                                </tr>
                            </table>
                            <br/>
                            <dnn:SectionHead ID="dshPayment" runat="server" CssClass="Head" IsExpanded="False" ResourceKey="Payment" Section="tblPayment" Text="Payment Settings" />
                            <table id="tblPayment" runat="server" border="0" cellpadding="2" cellspacing="2" summary="Payment Setttings Design Table">
                                <tr>
                                    <td class="SubHead" style="width: 150;">
                                        <dnn:Label ID="plCurrency" runat="server" ControlName="cboCurrency" Text="Currency:" />
                                    </td>
                                    <td class="NormalTextBox" valign="top" style="width: 325;">
                                        <asp:DropDownList ID="cboCurrency" runat="server" CssClass="NormalTextBox" DataTextField="text" DataValueField="value" Width="325">
                                        </asp:DropDownList></td>
                                </tr>
                                <tr>
                                    <td class="SubHead" valign="top" style="width: 150;">
                                        <dnn:Label ID="plProcessor" runat="server" ControlName="cboProcessor" Text="Payment Processor:" />
                                    </td>
                                    <td class="NormalTextBox" valign="top" style="width: 325;">
                                        <asp:DropDownList ID="cboProcessor" runat="server" CssClass="NormalTextBox" DataTextField="value" DataValueField="text" Width="325">
                                        </asp:DropDownList><br/>
                                        <asp:LinkButton ID="cmdProcessor" runat="server" CssClass="CommandButton" EnableViewState="False" OnClick="cmdProcessor_Click" resourcekey="ProcessorWebSite">Go To Payment Processor Website</asp:LinkButton></td>
                                </tr>
                                <tr>
                                    <td class="SubHead" style="width: 150;">
                                        <dnn:Label ID="plUserId" runat="server" ControlName="txtUserId" Text="Processor UserId:" />
                                    </td>
                                    <td class="NormalTextBox" valign="top" style="width: 325;">
                                        <asp:TextBox ID="txtUserId" runat="server" CssClass="NormalTextBox" MaxLength="50" Width="325"></asp:TextBox></td>
                                </tr>
                                <tr>
                                    <td class="SubHead" style="width: 150;">
                                        <dnn:Label ID="plPassword" runat="server" ControlName="txtPassword" Text="Processor Password:" />
                                    </td>
                                    <td class="NormalTextBox" valign="top" style="width: 325;">
                                        <asp:TextBox ID="txtPassword" runat="server" CssClass="NormalTextBox" MaxLength="50" TextMode="Password" Width="325"></asp:TextBox></td>
                                </tr>
                            </table>
                            <br/>
                            <dnn:SectionHead ID="dsOther" runat="server" CssClass="Head" IsExpanded="False" ResourceKey="Other" Section="tblOther" Text="Other Settings" />
                            <table id="tblOther" runat="server" border="0" cellpadding="2" cellspacing="2" summary="Other Setttings Design Table">
                                <tr>
                                    <td class="SubHead" style="width: 150;">
                                        <dnn:Label ID="plFooterText" runat="server" ControlName="txtFooterText" Text="Copyright:" />
                                    </td>
                                    <td class="NormalTextBox" style="width: 325;">
                                        <asp:TextBox ID="txtFooterText" runat="server" CssClass="NormalTextBox" MaxLength="100" Width="300"></asp:TextBox></td>
                                </tr>
                                <tr>
                                    <td class="SubHead" style="width: 150;">
                                        <dnn:Label ID="plBannerAdvertising" runat="server" ControlName="optBannerAdvertising" Text="Banner Advertising:" />
                                    </td>
                                    <td class="NormalTextBox" style="width: 325;">
                                        <asp:RadioButtonList ID="optBannerAdvertising" runat="server" CssClass="Normal" EnableViewState="False" RepeatDirection="Horizontal">
                                            <asp:ListItem resourcekey="None" Value="0">None</asp:ListItem>
                                            <asp:ListItem resourcekey="Site" Value="1">Site</asp:ListItem>
                                            <asp:ListItem resourcekey="Host" Value="2">Host</asp:ListItem>
                                        </asp:RadioButtonList></td>
                                </tr>
                                <tr>
                                    <td class="SubHead" style="width: 150;">
                                        <dnn:Label ID="plAdministrator" runat="server" ControlName="cboAdministratorId" Text="Administrator:" />
                                    </td>
                                    <td class="NormalTextBox" style="width: 325;">
                                        <asp:DropDownList ID="cboAdministratorId" runat="server" CssClass="NormalTextBox" DataTextField="FullName" DataValueField="UserId" Width="300">
                                        </asp:DropDownList></td>
                                </tr>
                                <tr>
                                    <td class="SubHead" style="width: 150;">
                                        <dnn:Label ID="plDefaultLanguage" runat="server" ControlName="cboDefaultLanguage" Text="Default Language:" />
                                    </td>
                                    <td style="width: 325;">
                                        <asp:DropDownList ID="cboDefaultLanguage" runat="server" CssClass="NormalTextBox" Width="300">
                                        </asp:DropDownList></td>
                                </tr>
                                <tr>
                                    <td class="SubHead" style="width: 150;">
                                        <dnn:Label ID="plTimeZone" runat="server" ControlName="cboTimeZone" Text="Portal TimeZone:" />
                                    </td>
                                    <td style="width: 325;">
                                        <asp:DropDownList ID="cboTimeZone" runat="server" CssClass="NormalTextBox" Width="300">
                                        </asp:DropDownList></td>
                                </tr>
                            </table>
                            <br/>
                            <dnn:SectionHead ID="dshHost" runat="server" CssClass="Head" IsExpanded="False" ResourceKey="HostSettings" Section="tblHost" Text="Host Settings" />
                            <table id="tblHost" runat="server" border="0" cellpadding="2" cellspacing="2" summary="Host Settings Design Table">
                                <tr>
                                    <td class="SubHead" style="width: 150;">
                                        <dnn:Label ID="plExpiryDate" runat="server" ControlName="txtExpiryDate" Text="Expiry Date:" />
                                    </td>
                                    <td class="NormalTextBox" style="width: 325;">
                                        <asp:TextBox ID="txtExpiryDate" runat="server" CssClass="NormalTextBox" MaxLength="15" Width="150"></asp:TextBox>
                                        <asp:HyperLink ID="cmdExpiryCalendar" runat="server" CssClass="CommandButton" resourcekey="Calendar">Calendar</asp:HyperLink>
                                        <asp:CompareValidator ID="valExpiryDate" runat="server" ControlToValidate="txtExpiryDate" CssClass="NormalRed" Display="Dynamic" ErrorMessage="<br>Invalid expiry date!"
                                            Operator="DataTypeCheck" Type="Date"></asp:CompareValidator></td>
                                </tr>
                                <tr>
                                    <td class="SubHead" style="width: 150;">
                                        <dnn:Label ID="plHostFee" runat="server" ControlName="txtHostFee" Text="Hosting Fee:" />
                                    </td>
                                    <td class="NormalTextBox" style="width: 325;">
                                        <asp:TextBox ID="txtHostFee" runat="server" CssClass="NormalTextBox" MaxLength="10" Width="300"></asp:TextBox></td>
                                </tr>
                                <tr>
                                    <td class="SubHead" style="width: 150;">
                                        <dnn:Label ID="plHostSpace" runat="server" ControlName="txtHostSpace" Text="Disk Space:" />
                                    </td>
                                    <td class="NormalTextBox" style="width: 100;">
                                        <asp:TextBox ID="txtHostSpace" runat="server" CssClass="NormalTextBox" MaxLength="6" Width="300"></asp:TextBox></td>
                                </tr>
                                <tr>
                                    <td class="SubHead" style="width: 150;">
                                        <dnn:Label ID="plPageQuota" runat="server" ControlName="txtPageQuota" Text="Page Quota:" />
                                    </td>
                                    <td class="NormalTextBox" style="width: 100;">
                                        <asp:TextBox ID="txtPageQuota" runat="server" CssClass="NormalTextBox" MaxLength="6" Width="300"></asp:TextBox></td>
                                </tr>
                                <tr>
                                    <td class="SubHead" style="width: 150;">
                                        <dnn:Label ID="plUserQuota" runat="server" ControlName="txtUserQuota" Text="User Quota:" />
                                    </td>
                                    <td class="NormalTextBox" style="width: 100;">
                                        <asp:TextBox ID="txtUserQuota" runat="server" CssClass="NormalTextBox" MaxLength="6" Width="300"></asp:TextBox></td>
                                </tr>
                                <tr>
                                    <td class="SubHead" style="width: 150;">
                                        <dnn:Label ID="plSiteLogHistory" runat="server" ControlName="txtSiteLogHistory" Text="Site Log History:" />
                                    </td>
                                    <td class="NormalTextBox" style="width: 325;">
                                        <asp:TextBox ID="txtSiteLogHistory" runat="server" CssClass="NormalTextBox" MaxLength="3" Width="300"></asp:TextBox></td>
                                </tr>
                                <tr>
                                    <td class="SubHead" valign="top" style="width: 150;">
                                        <br/>
                                        <br/>
                                        <dnn:Label ID="plDesktopModules" runat="server" ControlName="ctlDesktopModules" Text="Premium Modules:" />
                                    </td>
                                    <td class="NormalTextBox" style="width: 325;">
                                        <Portal:DualList ID="ctlDesktopModules" runat="server" DataTextField="FriendlyName" DataValueField="DesktopModuleID" ListBoxHeight="130" ListBoxWidth="130" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
                <br/>
                <dnn:SectionHead ID="dshStylesheet" runat="server" CssClass="Head" IncludeRule="True" IsExpanded="False" ResourceKey="StylesheetEditor" Section="tblStylesheet" Text="Stylesheet Editor" />
                <table id="tblStylesheet" runat="server" border="0" cellpadding="2" cellspacing="0" summary="Stylesheet Editor Design Table" style="width: 525;">
                    <tr>
                        <td>
                            <asp:TextBox ID="txtStyleSheet" runat="server" Columns="100" CssClass="NormalTextBox" Rows="20" TextMode="MultiLine" Wrap="False"></asp:TextBox></td>
                    </tr>
                    <tr>
                        <td>
                            <asp:LinkButton ID="cmdSave" runat="server" CssClass="CommandButton" EnableViewState="False" OnClick="cmdSave_Click" resourcekey="SaveStyleSheet">Save Style Sheet</asp:LinkButton>&nbsp;&nbsp;
                            <asp:LinkButton ID="cmdRestore" runat="server" CssClass="CommandButton" EnableViewState="False" OnClick="cmdRestore_Click" resourcekey="RestoreDefaultStyleSheet">Restore Default Style Sheet</asp:LinkButton></td>
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
        Text="Delete"></asp:LinkButton>&nbsp;&nbsp;
    <asp:LinkButton ID="cmdGoogle" runat="server" CssClass="CommandButton" OnClick="cmdGoogle_Click" resourcekey="cmdGoogle" Text="Submit Site To Google"></asp:LinkButton>
</p>
