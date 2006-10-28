<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Control Language="C#" AutoEventWireup="true" Inherits="DotNetNuke.Modules.Admin.Vendors.EditAffiliate" CodeFile="EditAffiliate.ascx.cs" %>
<table cellspacing="2" cellpadding="0" width="560">
    <tr>
        <td class="SubHead" width="200">
            <dnn:label id="plStartDate" runat="server" controlname="txtStartDate" suffix=":">
            </dnn:label></td>
        <td width="325">
            <asp:TextBox ID="txtStartDate" runat="server" CssClass="NormalTextBox" Width="120"
                Columns="30" MaxLength="11"></asp:TextBox>&nbsp;
            <asp:HyperLink ID="cmdStartCalendar" resourcekey="Calendar" CssClass="CommandButton"
                runat="server">Calendar</asp:HyperLink>
        </td>
    </tr>
    <tr>
        <td class="SubHead" width="200">
            <dnn:label id="plEndDate" runat="server" controlname="txtEndDate" suffix=":">
            </dnn:label></td>
        <td width="325">
            <asp:TextBox ID="txtEndDate" runat="server" CssClass="NormalTextBox" Width="120"
                Columns="30" MaxLength="11"></asp:TextBox>&nbsp;
            <asp:HyperLink ID="cmdEndCalendar" resourcekey="Calendar" CssClass="CommandButton"
                runat="server">Calendar</asp:HyperLink>
        </td>
    </tr>
    <tr>
        <td class="SubHead" width="200">
            <dnn:label id="plCPC" runat="server" controlname="txtCPC" suffix=":">
            </dnn:label></td>
        <td width="325">
            <asp:TextBox ID="txtCPC" runat="server" MaxLength="7" Columns="30" Width="100" CssClass="NormalTextBox"></asp:TextBox>
            <asp:RequiredFieldValidator ID="valCPC1" resourcekey="CPC.ErrorMessage" runat="server"
                ControlToValidate="txtCPC" ErrorMessage="You Must Enter a Valid CPC" Display="Dynamic"
                CssClass="NormalRed"></asp:RequiredFieldValidator>
            <asp:CompareValidator ID="valCPC2" resourcekey="CPC.ErrorMessage" runat="server"
                ControlToValidate="txtCPC" ErrorMessage="You Must Enter a Valid CPC" Display="Dynamic"
                Type="Double" Operator="DataTypeCheck" CssClass="NormalRed"></asp:CompareValidator>
        </td>
    </tr>
    <tr>
        <td class="SubHead" width="200">
            <dnn:label id="plCPA" runat="server" controlname="txtCPA" suffix=":">
            </dnn:label></td>
        <td width="325">
            <asp:TextBox ID="txtCPA" runat="server" MaxLength="7" Columns="30" Width="100" CssClass="NormalTextBox"></asp:TextBox>
            <asp:RequiredFieldValidator ID="valCPA1" resourcekey="CPA.ErrorMessage" runat="server"
                ControlToValidate="txtCPA" ErrorMessage="You Must Enter a Valid CPA" Display="Dynamic"
                CssClass="NormalRed"></asp:RequiredFieldValidator>
            <asp:CompareValidator ID="valCPA2" resourcekey="CPA.ErrorMessage" runat="server"
                ControlToValidate="txtCPA" ErrorMessage="You Must Enter a Valid CPA" Display="Dynamic"
                Type="Double" Operator="DataTypeCheck" CssClass="NormalRed"></asp:CompareValidator>
        </td>
    </tr>
</table>
<asp:Label ID="lblOptional" resourcekey="Optional" class="SubHead" runat="server">* = Optional</asp:Label>
<p>
    <asp:LinkButton class="CommandButton" ID="cmdUpdate" resourcekey="cmdUpdate" runat="server"
        Text="Update" BorderStyle="none"></asp:LinkButton>&nbsp;
    <asp:LinkButton class="CommandButton" ID="cmdCancel" resourcekey="cmdCancel" runat="server"
        Text="Cancel" BorderStyle="none" CausesValidation="False"></asp:LinkButton>&nbsp;
    <asp:LinkButton class="CommandButton" ID="cmdDelete" resourcekey="cmdDelete" runat="server"
        Text="Delete" BorderStyle="none" CausesValidation="False"></asp:LinkButton>
    <asp:LinkButton class="CommandButton" ID="cmdSend" resourcekey="cmdSend" runat="server"
        Text="Send Notification" BorderStyle="none" CausesValidation="False"></asp:LinkButton>
</p>
