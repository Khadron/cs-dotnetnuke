<%@ Control Language="C#" AutoEventWireup="false" Explicit="True" Inherits="DotNetNuke.Modules.Admin.Scheduling.EditSchedule" CodeFile="EditSchedule.ascx.cs" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<asp:panel id="pnlScheduleItem" runat="server">
	<TABLE cellSpacing="0" cellPadding="3" width="750" summary="Edit Schedule" border="0">
		<TR vAlign="top">
			<TD class="SubHead" width="150">
				<dnn:label id="plType" runat="server" controlname="txtType" text="Full Class Name and Assembly:"></dnn:label></TD>
			<TD class="Normal">
				<asp:TextBox id="txtType" Width="450" runat="server"></asp:TextBox></TD>
		</TR>
		<TR vAlign="top">
			<TD class="SubHead" width="150">
				<dnn:label id="plEnabled" runat="server" controlname="chkEnabled" text="Schedule Enabled:"></dnn:label></TD>
			<TD class="Normal">
				<asp:checkbox id="chkEnabled" runat="server" text="Yes" resourcekey="Yes"></asp:checkbox></TD>
		</TR>
		<TR vAlign="top">
			<TD class="SubHead" width="150">
				<dnn:label id="plTimeLapse" runat="server" controlname="txtTimeLapse" text="Time Lapse:"></dnn:label></TD>
			<TD class="Normal">
				<asp:textbox id="txtTimeLapse" runat="server" maxlength="10" width="50" cssclass="NormalTextBox"></asp:textbox>
				<asp:dropdownlist id="ddlTimeLapseMeasurement" runat="server">
					<asp:listitem resourcekey="Seconds" value="s">Seconds</asp:listitem>
					<asp:listitem resourcekey="Minutes" value="m">Minutes</asp:listitem>
					<asp:listitem resourcekey="Hours" value="h">Hours</asp:listitem>
					<asp:listitem resourcekey="Days" value="d">Days</asp:listitem>
				</asp:dropdownlist></TD>
		</TR>
		<TR vAlign="top">
			<TD class="SubHead" width="150">
				<dnn:label id="plRetryTimeLapse" runat="server" controlname="txtRetryTimeLapse" text="Retry Frequency:"></dnn:label></TD>
			<TD class="Normal">
				<asp:textbox id="txtRetryTimeLapse" runat="server" maxlength="10" width="50" cssclass="NormalTextBox"></asp:textbox>
				<asp:dropdownlist id="ddlRetryTimeLapseMeasurement" runat="server">
					<asp:listitem resourcekey="Seconds" value="s">Seconds</asp:listitem>
					<asp:listitem resourcekey="Minutes" value="m">Minutes</asp:listitem>
					<asp:listitem resourcekey="Hours" value="h">Hours</asp:listitem>
					<asp:listitem resourcekey="Days" value="d">Days</asp:listitem>
				</asp:dropdownlist></TD>
		</TR>
		<TR vAlign="top">
			<TD class="SubHead" width="150">
				<dnn:label id="plRetainHistoryNum" runat="server" controlname="ddlRetainHistoryNum" text="Retain Schedule History:"></dnn:label></TD>
			<TD class="Normal">
				<asp:dropdownlist id="ddlRetainHistoryNum" runat="server">
					<asp:listitem value="0">None</asp:listitem>
					<asp:listitem value="1">1</asp:listitem>
					<asp:listitem value="5">5</asp:listitem>
					<asp:listitem value="10">10</asp:listitem>
					<asp:listitem value="25">25</asp:listitem>
					<asp:listitem value="50">50</asp:listitem>
					<asp:listitem value="100">100</asp:listitem>
					<asp:listitem value="250">250</asp:listitem>
					<asp:listitem value="500">500</asp:listitem>
					<asp:listitem value="-1">All</asp:listitem>
				</asp:dropdownlist></TD>
		</TR>
		<TR vAlign="top">
			<TD class="SubHead" width="150">
				<dnn:label id="plAttachToEvent" runat="server" controlname="ddlAttachToEvent" text="Run on Event:"></dnn:label></TD>
			<TD class="Normal">
				<asp:dropdownlist id="ddlAttachToEvent" runat="server" cssclass="NormalTextBox">
					<asp:listitem resourcekey="None" value="">None</asp:listitem>
					<asp:listitem resourcekey="APPLICATION_START" value="APPLICATION_START">Application Start</asp:listitem>
				</asp:dropdownlist></TD>
		</TR>
		<TR vAlign="top">
			<TD class="SubHead" width="150">
				<dnn:label id="plCatchUpEnabled" runat="server" controlname="chkCatchUpEnabled" text="Catch Up Enabled:"></dnn:label></TD>
			<TD class="Normal">
				<asp:checkbox id="chkCatchUpEnabled" runat="server" text="Yes" resourcekey="Yes"></asp:checkbox></TD>
		</TR>
		<TR vAlign="top">
			<TD class="SubHead" width="150">
				<dnn:label id="plObjectDependencies" runat="server" controlname="txtObjectDependencies" text="Object Dependencies:"></dnn:label></TD>
			<TD class="Normal">
				<asp:textbox id="txtObjectDependencies" runat="server" maxlength="150" width="390" cssclass="NormalTextBox"></asp:textbox></TD>
		</TR>
		<TR vAlign="top">
			<TD class="SubHead" width="150">
				<dnn:label id="plServers" runat="server" controlname="txtServers" text="Run on Servers:"></dnn:label></TD>
			<TD class="Normal">
				<asp:textbox id="txtServers" runat="server" maxlength="150" width="390" cssclass="NormalTextBox"></asp:textbox></TD>
		</TR>
	</TABLE>
	<P>
		<asp:linkbutton id="cmdUpdate" runat="server" resourcekey="cmdUpdate" cssclass="CommandButton">Update</asp:linkbutton>&nbsp;
		<asp:linkbutton id="cmdDelete" runat="server" resourcekey="cmdDelete" cssclass="CommandButton">Delete</asp:linkbutton>&nbsp;
		<asp:linkbutton id="cmdCancel" runat="server" resourcekey="cmdCancel" cssclass="CommandButton">Cancel</asp:linkbutton></P>
</asp:panel>
