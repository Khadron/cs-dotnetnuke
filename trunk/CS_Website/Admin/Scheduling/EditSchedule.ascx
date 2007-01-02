<%@ Control AutoEventWireup="true" CodeFile="EditSchedule.ascx.cs" Explicit="True" Inherits="DotNetNuke.Modules.Admin.Scheduling.EditSchedule" Language="C#" %>
<%@ Register Src="~/controls/LabelControl.ascx" TagName="Label" TagPrefix="dnn" %>
<asp:Panel ID="pnlScheduleItem" runat="server">
    <table border="0" cellpadding="3" cellspacing="0" summary="Edit Schedule" width="750">
        <tr valign="top">
            <td class="SubHead" width="150">
                <dnn:Label ID="plType" runat="server" ControlName="txtType" Text="Full Class Name and Assembly:" />
            </td>
            <td class="Normal">
                <asp:TextBox ID="txtType" runat="server" Width="450"></asp:TextBox></td>
        </tr>
        <tr valign="top">
            <td class="SubHead" width="150">
                <dnn:Label ID="plEnabled" runat="server" ControlName="chkEnabled" Text="Schedule Enabled:" />
            </td>
            <td class="Normal">
                <asp:CheckBox ID="chkEnabled" runat="server" resourcekey="Yes" Text="Yes" /></td>
        </tr>
        <tr valign="top">
            <td class="SubHead" width="150">
                <dnn:Label ID="plTimeLapse" runat="server" ControlName="txtTimeLapse" Text="Time Lapse:" />
            </td>
            <td class="Normal">
                <asp:TextBox ID="txtTimeLapse" runat="server" CssClass="NormalTextBox" MaxLength="10" Width="50"></asp:TextBox>
                <asp:DropDownList ID="ddlTimeLapseMeasurement" runat="server">
                    <asp:ListItem resourcekey="Seconds" Value="s">Seconds</asp:ListItem>
                    <asp:ListItem resourcekey="Minutes" Value="m">Minutes</asp:ListItem>
                    <asp:ListItem resourcekey="Hours" Value="h">Hours</asp:ListItem>
                    <asp:ListItem resourcekey="Days" Value="d">Days</asp:ListItem>
                </asp:DropDownList></td>
        </tr>
        <tr valign="top">
            <td class="SubHead" width="150">
                <dnn:Label ID="plRetryTimeLapse" runat="server" ControlName="txtRetryTimeLapse" Text="Retry Frequency:" />
            </td>
            <td class="Normal">
                <asp:TextBox ID="txtRetryTimeLapse" runat="server" CssClass="NormalTextBox" MaxLength="10" Width="50"></asp:TextBox>
                <asp:DropDownList ID="ddlRetryTimeLapseMeasurement" runat="server">
                    <asp:ListItem resourcekey="Seconds" Value="s">Seconds</asp:ListItem>
                    <asp:ListItem resourcekey="Minutes" Value="m">Minutes</asp:ListItem>
                    <asp:ListItem resourcekey="Hours" Value="h">Hours</asp:ListItem>
                    <asp:ListItem resourcekey="Days" Value="d">Days</asp:ListItem>
                </asp:DropDownList></td>
        </tr>
        <tr valign="top">
            <td class="SubHead" width="150">
                <dnn:Label ID="plRetainHistoryNum" runat="server" ControlName="ddlRetainHistoryNum" Text="Retain Schedule History:" />
            </td>
            <td class="Normal">
                <asp:DropDownList ID="ddlRetainHistoryNum" runat="server">
                    <asp:ListItem Value="0">None</asp:ListItem>
                    <asp:ListItem Value="1">1</asp:ListItem>
                    <asp:ListItem Value="5">5</asp:ListItem>
                    <asp:ListItem Value="10">10</asp:ListItem>
                    <asp:ListItem Value="25">25</asp:ListItem>
                    <asp:ListItem Value="50">50</asp:ListItem>
                    <asp:ListItem Value="100">100</asp:ListItem>
                    <asp:ListItem Value="250">250</asp:ListItem>
                    <asp:ListItem Value="500">500</asp:ListItem>
                    <asp:ListItem Value="-1">All</asp:ListItem>
                </asp:DropDownList></td>
        </tr>
        <tr valign="top">
            <td class="SubHead" width="150">
                <dnn:Label ID="plAttachToEvent" runat="server" ControlName="ddlAttachToEvent" Text="Run on Event:" />
            </td>
            <td class="Normal">
                <asp:DropDownList ID="ddlAttachToEvent" runat="server" CssClass="NormalTextBox">
                    <asp:ListItem resourcekey="None" Value="">None</asp:ListItem>
                    <asp:ListItem resourcekey="APPLICATION_START" Value="APPLICATION_START">Application Start</asp:ListItem>
                </asp:DropDownList></td>
        </tr>
        <tr valign="top">
            <td class="SubHead" width="150">
                <dnn:Label ID="plCatchUpEnabled" runat="server" ControlName="chkCatchUpEnabled" Text="Catch Up Enabled:" />
            </td>
            <td class="Normal">
                <asp:CheckBox ID="chkCatchUpEnabled" runat="server" resourcekey="Yes" Text="Yes" /></td>
        </tr>
        <tr valign="top">
            <td class="SubHead" width="150">
                <dnn:Label ID="plObjectDependencies" runat="server" ControlName="txtObjectDependencies" Text="Object Dependencies:" />
            </td>
            <td class="Normal">
                <asp:TextBox ID="txtObjectDependencies" runat="server" CssClass="NormalTextBox" MaxLength="150" Width="390"></asp:TextBox></td>
        </tr>
        <tr valign="top">
            <td class="SubHead" width="150">
                <dnn:Label ID="plServers" runat="server" ControlName="txtServers" Text="Run on Servers:" />
            </td>
            <td class="Normal">
                <asp:TextBox ID="txtServers" runat="server" CssClass="NormalTextBox" MaxLength="150" Width="390"></asp:TextBox></td>
        </tr>
    </table>
    <p>
        <asp:LinkButton ID="cmdUpdate" runat="server" CssClass="CommandButton" OnClick="cmdUpdate_Click" resourcekey="cmdUpdate">Update</asp:LinkButton>&nbsp;
        <asp:LinkButton ID="cmdDelete" runat="server" CssClass="CommandButton" OnClick="cmdDelete_Click" resourcekey="cmdDelete">Delete</asp:LinkButton>&nbsp;
        <asp:LinkButton ID="cmdCancel" runat="server" CssClass="CommandButton" OnClick="cmdCancel_Click" resourcekey="cmdCancel">Cancel</asp:LinkButton></p>
</asp:Panel>
