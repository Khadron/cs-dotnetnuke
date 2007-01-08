<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Control Inherits="DotNetNuke.Modules.Admin.SiteLog.SiteLog" Language="C#" AutoEventWireup="true"  CodeFile="SiteLog.ascx.cs" %>
<table class="Settings" cellspacing="2" cellpadding="2" summary="Site Log Design Table" border="0" width="450">
	<tr vAlign="top">
    <td class="SubHead" width="150"><dnn:label id="plReportType" runat="server" controlname="cboReportType" suffix=":"></dnn:label></td>
		<td class="NormalBold" align="left" width="325">
			<asp:DropDownList ID="cboReportType" Runat="server" DataValueField="value" DataTextField="text" CssClass="NormalTextBox"></asp:DropDownList>
		</td>
	</tr>
	<tr>
    <td class="SubHead" width="150"><dnn:label id="plStartDate" runat="server" controlname="txtStartDate" suffix=":"></dnn:label></td>
		<td class="NormalBold" align="left" width="325">
			<asp:TextBox id="txtStartDate" CssClass="NormalTextBox" runat="server" width="120" Columns="20"></asp:TextBox>&nbsp;
			<asp:HyperLink id="cmdStartCalendar" resourcekey="Calendar" Runat="server" CssClass="CommandButton">Calendar</asp:HyperLink>
		</td>
	</tr>
	<tr>
    <td class="SubHead" width="150"><dnn:label id="plEndDate" runat="server" controlname="txtEndDate" suffix=":"></dnn:label></td>
		<td class="NormalBold" align="left" width="325">
			<asp:TextBox id="txtEndDate" CssClass="NormalTextBox" runat="server" width="120" Columns="20"></asp:TextBox>&nbsp;
			<asp:HyperLink id="cmdEndCalendar" resourcekey="Calendar" Runat="server" CssClass="CommandButton">Calendar</asp:HyperLink>
		</td>
	</tr>
	<tr>
		<td class="NormalBold" vAlign="top" align="center" colspan="2">
			<asp:LinkButton id="cmdDisplay" resourcekey="cmdDisplay" cssclass="CommandButton" Text="Display" runat="server" OnClick="cmdDisplay_Click"  />&nbsp;&nbsp;
			<asp:LinkButton id="cmdCancel" resourcekey="cmdCancel" CssClass="CommandButton" runat="server" OnClick="cmdCancel_Click">Cancel</asp:LinkButton>
		</td>
	</tr>
</table>
<br/>
<asp:DataGrid ID="grdLog" runat="server" AutoGenerateColumns="true" Border="0" BorderStyle="None" BorderWidth="0px" CellPadding="4" CellSpacing="4" GridLines="None"
    HeaderStyle-CssClass="NormalBold" HeaderStyle-HorizontalAlign="Center" ItemStyle-CssClass="Normal" ItemStyle-HorizontalAlign="Center" Width="750">
</asp:DataGrid>
<br/>
