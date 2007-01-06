<%@ Control Language="C#" AutoEventWireup="true" Inherits="DotNetNuke.Modules.Admin.Vendors.EditBanner" CodeFile="EditBanner.ascx.cs" %>
<%@ Register TagPrefix="Portal" TagName="URL" Src="~/controls/URLControl.ascx" %>
<%@ Register TagPrefix="dnn" TagName="Audit" Src="~/controls/ModuleAuditControl.ascx" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<br>
<table cellspacing="2" cellpadding="0" width="560" summary="Edit Banner Design Table">
    <tr valign="top">
        <td colspan="2">
            <asp:DataList ID="lstBanners" runat="server" CellPadding="4" Width="100%" Summary="Banner Design Table"
                EnableViewState="true">
                <ItemStyle HorizontalAlign="Center" BorderWidth="1" BorderColor="#000000"></ItemStyle>
                <ItemTemplate>
                    <asp:Label ID="lblItem" runat="server" Text='<%# FormatItem(Convert.ToInt32(DataBinder.Eval(Container.DataItem,"VendorId")),
                    Convert.ToInt32(DataBinder.Eval(Container.DataItem,"BannerId")),
                    Convert.ToInt32(DataBinder.Eval(Container.DataItem,"BannerTypeId")),
                    Convert.ToString(DataBinder.Eval(Container.DataItem,"BannerName")),
                    Convert.ToString(DataBinder.Eval(Container.DataItem,"ImageFile")),
                    Convert.ToString(DataBinder.Eval(Container.DataItem,"Description")),
                    Convert.ToString(DataBinder.Eval(Container.DataItem,"Url")),
                    Convert.ToInt32(DataBinder.Eval(Container.DataItem,"Width")),
                    Convert.ToInt32(DataBinder.Eval(Container.DataItem,"Height"))) %>'>
                    </asp:Label>
                </ItemTemplate>
            </asp:DataList>
            <br>
        </td>
    </tr>
    <tr valign="top">
        <td class="SubHead" width="125">
            <dnn:label id="plBannerName" runat="server" controlname="txtBannerName" suffix=":">
            </dnn:label></td>
        <td width="400">
            <asp:TextBox ID="txtBannerName" runat="server" MaxLength="100" Columns="30" Width="300"
                CssClass="NormalTextBox"></asp:TextBox>
            <asp:RequiredFieldValidator ID="valBannerName" resourcekey="BannerName.ErrorMessage"
                runat="server" ControlToValidate="txtBannerName" ErrorMessage="You Must Enter a Banner Name"
                Display="Dynamic" CssClass="NormalRed"></asp:RequiredFieldValidator>
        </td>
    </tr>
    <tr valign="top">
        <td class="SubHead" width="125">
            <dnn:label id="plBannerType" runat="server" controlname="cboBannerType" suffix=":">
            </dnn:label></td>
        <td width="400">
            <asp:DropDownList ID="cboBannerType" DataTextField="BannerTypeName" DataValueField="BannerTypeId"
                Width="300" CssClass="NormalTextBox" runat="server" />
        </td>
    </tr>
    <tr valign="top">
        <td class="SubHead" width="125">
            <dnn:label id="plBannerGroup" runat="server" controlname="txtBannerGroup" suffix=":">
            </dnn:label></td>
        <td width="400">
            <asp:TextBox ID="txtBannerGroup" runat="server" MaxLength="100" Columns="30" Width="300"
                CssClass="NormalTextBox"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td colspan="2">
            &nbsp;</td>
    </tr>
    <tr>
        <td class="SubHead" width="125">
            <dnn:label id="plImage" runat="server" controlname="ctlImage" suffix=":">
            </dnn:label></td>
        <td width="400">
            <Portal:url id="ctlImage" runat="server" width="250" Required="False" ShowFiles="True"
                ShowTabs="False" ShowUrls="True" ShowTrack="False" ShowLog="False" UrlType="F" />
        </td>
    </tr>
    <tr valign="top">
        <td class="SubHead" width="125">
            <dnn:label id="plWidth" runat="server" controlname="txtWidth" suffix=":">
            </dnn:label></td>
        <td width="400">
            <asp:TextBox ID="txtWidth" runat="server" MaxLength="100" Columns="30" Width="300"
                CssClass="NormalTextBox"></asp:TextBox>
        </td>
    </tr>
    <tr valign="top">
        <td class="SubHead" width="125">
            <dnn:label id="plHeight" runat="server" controlname="txtHeight" suffix=":">
            </dnn:label></td>
        <td width="400">
            <asp:TextBox ID="txtHeight" runat="server" MaxLength="100" Columns="30" Width="300"
                CssClass="NormalTextBox"></asp:TextBox>
        </td>
    </tr>
    <tr valign="top">
        <td class="SubHead" width="125" valign="middle">
            <dnn:label id="plDescription" runat="server" controlname="txtDescription" suffix=":">
            </dnn:label></td>
        <td width="400">
            <asp:TextBox ID="txtDescription" runat="server" MaxLength="2000" Columns="30" Width="300"
                CssClass="NormalTextBox" TextMode="MultiLine" Rows="5"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td class="SubHead" width="125">
            <dnn:label id="plURL" runat="server" controlname="ctlURL" suffix=":">
            </dnn:label></td>
        <td width="400">
            <Portal:url id="ctlURL" runat="server" width="250" Required="False" ShowFiles="True"
                ShowTabs="True" ShowUrls="True" ShowTrack="False" ShowLog="False" UrlType="U" />
        </td>
    </tr>
    <tr>
        <td colspan="2">
            &nbsp;</td>
    </tr>
    <tr>
        <td class="SubHead" width="125">
            <dnn:label id="plCPM" runat="server" controlname="txtCPM" suffix=":">
            </dnn:label></td>
        <td width="400">
            <asp:TextBox ID="txtCPM" runat="server" MaxLength="7" Columns="30" Width="300" CssClass="NormalTextBox"></asp:TextBox>
            <asp:RequiredFieldValidator ID="valCPM" runat="server" resourcekey="CPM.ErrorMessage"
                ControlToValidate="txtCPM" ErrorMessage="You Must Enter a Valid CPM" Display="Dynamic"
                CssClass="NormalRed"></asp:RequiredFieldValidator>
            <asp:CompareValidator ID="compareCPM" runat="server" resourcekey="CPM.ErrorMessage"
                ControlToValidate="txtCPM" ErrorMessage="You Must Enter a Valid CPM" Display="Dynamic"
                Type="Currency" Operator="DataTypeCheck" CssClass="NormalRed"></asp:CompareValidator>
        </td>
    </tr>
    <tr>
        <td class="SubHead" width="125">
            <dnn:label id="plImpressions" runat="server" controlname="txtImpressions" suffix=":">
            </dnn:label></td>
        <td width="400">
            <asp:TextBox ID="txtImpressions" runat="server" MaxLength="10" Columns="30" Width="300"
                CssClass="NormalTextBox"></asp:TextBox>
            <asp:RequiredFieldValidator ID="valImpressions" resourcekey="Impressions.ErrorMessage"
                runat="server" ControlToValidate="txtImpressions" ErrorMessage="You Must Enter a Valid Number of Impressions"
                Display="Dynamic" CssClass="NormalRed"></asp:RequiredFieldValidator>
            <asp:CompareValidator ID="compareImpressions" resourcekey="Impressions.ErrorMessage"
                runat="server" Display="Dynamic" ErrorMessage="You Must Enter a Valid Number of Impressions"
                ControlToValidate="txtImpressions" Operator="DataTypeCheck" Type="Integer" CssClass="NormalRed"></asp:CompareValidator>
        </td>
    </tr>
    <tr>
        <td class="SubHead" width="125">
            <dnn:label id="plStartDate" runat="server" controlname="txtStartDate" suffix=":">
            </dnn:label></td>
        <td width="400">
            <asp:TextBox ID="txtStartDate" runat="server" CssClass="NormalTextBox" Width="250"
                Columns="30" MaxLength="11"></asp:TextBox>&nbsp;
            <asp:HyperLink ID="cmdStartCalendar" resourcekey="Calendar" CssClass="CommandButton"
                runat="server">Calendar</asp:HyperLink>
        </td>
    </tr>
    <tr>
        <td class="SubHead" width="125">
            <dnn:label id="plEndDate" runat="server" controlname="txtEndDate" suffix=":">
            </dnn:label></td>
        <td width="400">
            <asp:TextBox ID="txtEndDate" runat="server" CssClass="NormalTextBox" Width="250"
                Columns="30" MaxLength="11"></asp:TextBox>&nbsp;
            <asp:HyperLink ID="cmdEndCalendar" resourcekey="Calendar" CssClass="CommandButton"
                runat="server">Calendar</asp:HyperLink>
        </td>
    </tr>
    <tr>
        <td class="SubHead" width="125">
            <dnn:label id="plCriteria" runat="server" controlname="optCriteria" suffix=":">
            </dnn:label></td>
        <td>
            <asp:RadioButtonList ID="optCriteria" runat="server" CssClass="NormalBold" RepeatDirection="Horizontal">
                <asp:ListItem Value="1">OR</asp:ListItem>
                <asp:ListItem Value="0">AND</asp:ListItem>
            </asp:RadioButtonList>
        </td>
    </tr>
</table>
<p>
    <asp:LinkButton cssclass="CommandButton" ID="cmdUpdate" resourcekey="cmdUpdate" runat="server"
        Text="Update" BorderStyle="none" OnClick="cmdUpdate_Click"></asp:LinkButton>&nbsp;
    <asp:LinkButton cssclass="CommandButton" ID="cmdCancel" resourcekey="cmdCancel" runat="server"
        Text="Cancel" BorderStyle="none" CausesValidation="False" OnClick="cmdCancel_Click"></asp:LinkButton>&nbsp;
    <asp:LinkButton cssclass="CommandButton" ID="cmdDelete" resourcekey="cmdDelete" runat="server"
        Text="Delete" BorderStyle="none" CausesValidation="False" OnClick="cmdDelete_Click"></asp:LinkButton>&nbsp;
    <asp:LinkButton cssclass="CommandButton" ID="cmdCopy" resourcekey="cmdCopy" runat="server"
        Text="Copy" BorderStyle="none" CausesValidation="False" OnClick="cmdCopy_Click"></asp:LinkButton>&nbsp;
    <asp:LinkButton cssclass="CommandButton" ID="cmdEmail" resourcekey="cmdEmail" runat="server"
        Text="Email Status to Vendor" BorderStyle="none" CausesValidation="False" OnClick="cmdEmail_Click"></asp:LinkButton>
</p>
<dnn:audit id="ctlAudit" runat="server" />
