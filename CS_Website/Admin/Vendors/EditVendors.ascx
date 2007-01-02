<%@ Control Language="C#" AutoEventWireup="true" Inherits="DotNetNuke.Modules.Admin.Vendors.EditVendors"
    CodeFile="EditVendors.ascx.cs" %>
<%@ Reference Control="~/admin/vendors/affiliates.ascx" %>
<%@ Reference Control="~/admin/vendors/banners.ascx" %>
<%@ Register TagPrefix="dnn" TagName="Address" Src="~/controls/Address.ascx" %>
<%@ Register TagPrefix="dnn" TagName="Audit" Src="~/controls/ModuleAuditControl.ascx" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register TagPrefix="Portal" TagName="URL" Src="~/controls/URLControl.ascx" %>
<%@ Register TagPrefix="dnn" TagName="SectionHead" Src="~/controls/SectionHeadControl.ascx" %>
<table cellspacing="0" cellpadding="4" border="0" summary="Edit Vendors Design Table">
    <tr>
        <td align="top">
            <table cellspacing="0" cellpadding="0" border="0" summary="Edit Vendors Design Table">
                <tr>
                    <td valign="top" width="560">
                        <dnn:SectionHead id="dshSettings" runat="server" cssclass="Head" includerule="True"
                            resourcekey="Settings" section="tblSettings" text="Vendor Details" />
                        <table id="tblSettings" runat="server" cellspacing="2" cellpadding="2" border="0"
                            summary="Edit Vendors Design Table">
                            <tr valign="top">
                                <td class="SubHead" width="120">
                                    <dnn:Label id="plVendorName" runat="server" controlname="txtVendorName" suffix=":">
                                    </dnn:Label></td>
                                <td align="left" class="NormalBold" nowrap>
                                    <asp:TextBox ID="txtVendorName" runat="server" CssClass="NormalTextBox" Width="200px"
                                        MaxLength="50" TabIndex="1"></asp:TextBox>&nbsp;*
                                    <asp:RequiredFieldValidator ID="valVendorName" runat="server" CssClass="NormalRed"
                                        Display="Dynamic" ErrorMessage="<br>Company Name Is Required" ControlToValidate="txtVendorName"></asp:RequiredFieldValidator>
                                </td>
                            </tr>
                            <tr valign="top">
                                <td class="SubHead" width="120">
                                    <dnn:Label id="plFirstName" runat="server" controlname="txtFirstName" suffix=":">
                                    </dnn:Label></td>
                                <td class="NormalBold" nowrap>
                                    <asp:TextBox ID="txtFirstName" runat="server" CssClass="NormalTextBox" Width="200px"
                                        MaxLength="50" TabIndex="2"></asp:TextBox>&nbsp;*
                                    <asp:RequiredFieldValidator ID="valFirstName" runat="server" CssClass="NormalRed"
                                        Display="Dynamic" ErrorMessage="<br>First Name Is Required" ControlToValidate="txtFirstName"></asp:RequiredFieldValidator>
                                </td>
                            </tr>
                            <tr valign="top">
                                <td class="SubHead" width="120">
                                    <dnn:Label id="plLastName" runat="server" controlname="txtLastName" suffix=":">
                                    </dnn:Label></td>
                                <td class="NormalBold" nowrap>
                                    <asp:TextBox ID="txtLastName" runat="server" CssClass="NormalTextBox" Width="200px"
                                        MaxLength="50" TabIndex="3"></asp:TextBox>&nbsp;*
                                    <asp:RequiredFieldValidator ID="valLastName" runat="server" CssClass="NormalRed"
                                        Display="Dynamic" ErrorMessage="<br>Last Name Is Required" ControlToValidate="txtLastName"></asp:RequiredFieldValidator>
                                </td>
                            </tr>
                            <tr valign="top">
                                <td class="SubHead" width="120">
                                    <dnn:Label id="plEmail" runat="server" controlname="txtEmail" suffix=":">
                                    </dnn:Label></td>
                                <td class="NormalBold" nowrap>
                                    <asp:TextBox ID="txtEmail" runat="server" CssClass="NormalTextBox" Width="200px"
                                        MaxLength="50" TabIndex="4"></asp:TextBox>&nbsp;*
                                    <asp:RequiredFieldValidator ID="valEmail" runat="server" CssClass="NormalRed" Display="Dynamic"
                                        ErrorMessage="<br>Email Is Required" ControlToValidate="txtEmail"></asp:RequiredFieldValidator>
                                    <asp:RegularExpressionValidator ID="valEmail2" runat="server" ErrorMessage="<br>Email Must be Valid."
                                        ValidationExpression="[\w\.-]+(\+[\w-]*)?@([\w-]+\.)+[\w-]+" ControlToValidate="txtEmail"
                                        Display="Dynamic"></asp:RegularExpressionValidator>
                                </td>
                            </tr>
                        </table>
                        <br>
                        <dnn:SectionHead id="dshAddress" runat="server" cssclass="Head" includerule="True"
                            resourcekey="Address" section="tblAddress" text="Address Details" />
                        <table id="tblAddress" runat="server" cellspacing="2" cellpadding="2" border="0"
                            summary="Edit Vendors Design Table">
                            <tr>
                                <td>
                                    <dnn:address runat="server" id="addresssVendor" /></td>
                            </tr>
                        </table>
                        <br>
                        <dnn:SectionHead id="dshOther" runat="server" cssclass="Head" includerule="True"
                            resourcekey="Other" section="tblOther" text="Other Details" />
                        <table id="tblOther" runat="server" cellspacing="2" cellpadding="2" border="0" summary="Edit Vendors Design Table">
                            <tr valign="top">
                                <td class="SubHead" width="120">
                                    <dnn:Label id="plWebsite" runat="server" controlname="txtWebsite" suffix=":">
                                    </dnn:Label></td>
                                <td>
                                    <asp:TextBox ID="txtWebsite" runat="server" CssClass="NormalTextBox" Width="200px"
                                        MaxLength="100" TabIndex="20"></asp:TextBox></td>
                            </tr>
                            <tr id="rowVendor1" runat="server" valign="top">
                                <td class="SubHead" width="120" valign="middle">
                                    <dnn:Label id="plLogo" runat="server" controlname="ctlLogo" suffix="">
                                    </dnn:Label></td>
                                <td width="325">
                                    <Portal:url id="ctlLogo" runat="server" width="200" showurls="False" showtabs="False"
                                        showlog="False" showtrack="False" required="False" /></td>
                            </tr>
                            <tr id="rowVendor2" runat="server" valign="top">
                                <td class="SubHead" width="120">
                                    <dnn:Label id="plAuthorized" runat="server" controlname="chkAuthorized" suffix=":">
                                    </dnn:Label></td>
                                <td>
                                    <asp:CheckBox ID="chkAuthorized" runat="server" TabIndex="22"></asp:CheckBox></td>
                            </tr>
                        </table>
                        <br>
                        <asp:Panel ID="pnlVendor" runat="server">
                            <dnn:SectionHead id="dshClassification" runat="server" cssclass="Head" includerule="True"
                                isExpanded="False" resourcekey="VendorClassification" section="tblClassification"
                                text="Vendor Classification" />
                            <table id="tblClassification" runat="server" cellspacing="2" cellpadding="2" border="0"
                                summary="Edit Vendors Design Table">
                                <tr valign="top">
                                    <td class="SubHead" align="center" width="200">
                                        <dnn:Label id="plClassifications" runat="server" controlname="lstClassifications"
                                            suffix=":">
                                        </dnn:Label></td>
                                    <td width="60">
                                    </td>
                                    <td class="SubHead" align="center" width="200">
                                        <dnn:Label id="plKeyWords" runat="server" controlname="txtKeyWords" suffix=":">
                                        </dnn:Label></td>
                                </tr>
                                <tr valign="top">
                                    <td align="center" width="200">
                                        <asp:ListBox ID="lstClassifications" Width="200px" CssClass="NormalTextBox" runat="server"
                                            Rows="10" SelectionMode="Multiple" TabIndex="16"></asp:ListBox></td>
                                    <td width="60">
                                    </td>
                                    <td align="center" width="200">
                                        <asp:TextBox ID="txtKeyWords" Width="200px" CssClass="NormalTextBox" runat="server"
                                            Rows="10" TextMode="MultiLine" TabIndex="17"></asp:TextBox></td>
                                </tr>
                            </table>
                        </asp:Panel>
                        <br>
                        <asp:PlaceHolder ID="pnlBanners" runat="server">
                            <dnn:SectionHead id="dshBanners" runat="server" cssclass="Head" includerule="True"
                                isexpanded="False" resourcekey="BannerAdvertising" section="divBanners" text="Banner Advertising" />
                            <div id="divBanners" runat="server" />
                        </asp:PlaceHolder>
                        <br>
                        <asp:PlaceHolder ID="pnlAffiliates" runat="server">
                            <dnn:SectionHead id="dshAffiliates" runat="server" cssclass="Head" includerule="True"
                                isexpanded="False" resourcekey="AffiliateReferrals" section="divAffiliates" text="Affiliate Referrals" />
                            <div id="divAffiliates" runat="server" />
                        </asp:PlaceHolder>
                    </td>
                </tr>
            </table>
        </td>
    </tr>
</table>
<p>
    <asp:LinkButton class="CommandButton" ID="cmdUpdate" resourcekey="cmdUpdate" runat="server"
        Text="Update" BorderStyle="none" OnClick="cmdUpdate_Click"></asp:LinkButton>&nbsp;
    <asp:LinkButton class="CommandButton" ID="cmdCancel" resourcekey="cmdCancel" runat="server"
        Text="Cancel" BorderStyle="none" CausesValidation="False" OnClick="cmdCancel_Click"></asp:LinkButton>&nbsp;
    <asp:LinkButton class="CommandButton" ID="cmdDelete" resourcekey="cmdDelete" runat="server"
        Text="Delete" BorderStyle="none" CausesValidation="False" OnClick="cmdDelete_Click"></asp:LinkButton>
</p>
<table cellspacing="0" cellpadding="4" border="0" summary="Edit Vendors Design Table">
    <tr>
        <td>
            <asp:Panel ID="pnlAudit" runat="server">
                <dnn:audit id="ctlAudit" runat="server" />
            </asp:Panel>
        </td>
    </tr>
</table>
