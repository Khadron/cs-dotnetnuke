<%@ Control Language="C#" AutoEventWireup="true" Explicit="True" Inherits="DotNetNuke.Modules.Admin.Scheduling.ViewScheduleStatus" CodeFile="ViewScheduleStatus.ascx.cs" %>
<table border="0" cellspacing="1" cellpadding="3">
	<tr>
		<td class="SubHead"><asp:label id="lblStatusLabel" resourcekey="lblStatusLabel" runat="server">Current Status:</asp:label></td>
		<td class="Normal"><asp:label cssclass="NormalBold" id="lblStatus" runat="server" /></td>
	</tr>
	<tr>
		<td class="SubHead"><asp:label id="lblMaxThreadsLabel" resourcekey="lblMaxThreadsLabel" runat="server">Max Threads:</asp:label></td>
		<td class="Normal"><asp:label cssclass="NormalBold" id="lblMaxThreads" runat="server" /></td>
	</tr>
	<tr>
		<td class="SubHead"><asp:label id="lblActiveThreadsLabel" resourcekey="lblActiveThreadsLabel" runat="server">Active Threads:</asp:label></td>
		<td class="Normal"><asp:label cssclass="NormalBold" id="lblActiveThreads" runat="server" /></td>
	</tr>
	<tr>
		<td class="SubHead"><asp:label id="lblFreeThreadsLabel" resourcekey="lblFreeThreadsLabel" runat="server">Free Threads:</asp:label></td>
		<td class="Normal"><asp:label cssclass="NormalBold" id="lblFreeThreads" runat="server" /></td>
	</tr>
	<tr>
		<td class="SubHead"><asp:label id="lblCommand" resourcekey="lblCommand" runat="server">Command:</asp:label></td>
		<td class="Normal">
			<asp:linkbutton id="cmdStart" resourcekey="cmdStart" cssclass="CommandButton" runat="server">Start</asp:linkbutton>
			&nbsp;&nbsp;
			<asp:linkbutton id="cmdStop" resourcekey="cmdStop" cssclass="CommandButton" runat="server">Stop</asp:linkbutton>
		</td>
	</tr>
</table>
<br>
<asp:panel id="pnlScheduleProcessing" runat="server">
<asp:label id=lblProcessing runat="server" resourcekey="lblProcessing" cssClass="SubHead">Items Processing</asp:label>
<HR noShade SIZE=1>
<asp:datagrid id=dgScheduleProcessing runat="server" autogeneratecolumns="false" cellpadding="4" datakeyfield="ScheduleID" enableviewstate="false" border="1" summary="This table shows the scheduled tasks that are currently running." alternatingitemstyle-backcolor="#CFCFCF" BorderStyle="None" BorderWidth="0px" GridLines="None">
<AlternatingItemStyle BackColor="#CFCFCF">
</AlternatingItemStyle>

<Columns>
<asp:BoundColumn DataField="ScheduleID" HeaderText="ScheduleID">
<HeaderStyle CssClass="NormalBold">
</HeaderStyle>

<ItemStyle CssClass="Normal">
</ItemStyle>
</asp:BoundColumn>
<asp:BoundColumn DataField="TypeFullName" HeaderText="Type">
<HeaderStyle CssClass="NormalBold">
</HeaderStyle>

<ItemStyle CssClass="Normal">
</ItemStyle>
</asp:BoundColumn>
<asp:BoundColumn DataField="StartDate" HeaderText="Started">
<HeaderStyle CssClass="NormalBold">
</HeaderStyle>

<ItemStyle CssClass="Normal">
</ItemStyle>
</asp:BoundColumn>
<asp:BoundColumn DataField="ElapsedTime" HeaderText="Duration&lt;br&gt;(seconds)">
<HeaderStyle CssClass="NormalBold">
</HeaderStyle>

<ItemStyle CssClass="Normal">
</ItemStyle>
</asp:BoundColumn>
<asp:BoundColumn DataField="ObjectDependencies" HeaderText="ObjectDependencies">
<HeaderStyle CssClass="NormalBold">
</HeaderStyle>

<ItemStyle CssClass="Normal">
</ItemStyle>
</asp:BoundColumn>
<asp:BoundColumn DataField="ScheduleSource" HeaderText="TriggeredBy">
<HeaderStyle CssClass="NormalBold">
</HeaderStyle>

<ItemStyle CssClass="Normal">
</ItemStyle>
</asp:BoundColumn>
<asp:BoundColumn DataField="ThreadID" HeaderText="Thread">
<HeaderStyle CssClass="NormalBold">
</HeaderStyle>

<ItemStyle CssClass="Normal">
</ItemStyle>
</asp:BoundColumn>
<asp:BoundColumn DataField="LogNotes" HeaderText="Notes">
<HeaderStyle CssClass="NormalBold">
</HeaderStyle>

<ItemStyle CssClass="Normal">
</ItemStyle>
</asp:BoundColumn>
<asp:TemplateColumn HeaderText="Servers">
<HeaderStyle CssClass="NormalBold">
</HeaderStyle>

<ItemStyle CssClass="Normal">
</ItemStyle>

<ItemTemplate>
					<%# DataBinder.Eval(Container.DataItem,"Servers") %>
				
</ItemTemplate>
</asp:TemplateColumn>
</Columns>
</asp:datagrid>
</asp:panel>
<br>
<br>
<asp:panel id="pnlScheduleQueue" runat="server">
<asp:label id=lblQueue runat="server" resourcekey="lblQueue" cssclass="SubHead">Items Processing</asp:label>
<HR noShade SIZE=1>
<asp:datagrid id=dgScheduleQueue runat="server" autogeneratecolumns="false" cellpadding="4" datakeyfield="ScheduleID" enableviewstate="false" border="1" summary="This table shows the tasks that are queued up in the schedule." alternatingitemstyle-backcolor="#CFCFCF" BorderStyle="None" BorderWidth="0px" GridLines="None">
<AlternatingItemStyle BackColor="#CFCFCF">
</AlternatingItemStyle>

<Columns>
<asp:BoundColumn DataField="ScheduleID" HeaderText="ScheduleID">
<HeaderStyle CssClass="NormalBold">
</HeaderStyle>

<ItemStyle CssClass="Normal">
</ItemStyle>
</asp:BoundColumn>
<asp:BoundColumn DataField="TypeFullName" HeaderText="Type">
<HeaderStyle CssClass="NormalBold">
</HeaderStyle>

<ItemStyle CssClass="Normal">
</ItemStyle>
</asp:BoundColumn>
<asp:BoundColumn DataField="NextStart" HeaderText="NextStart">
<HeaderStyle CssClass="NormalBold">
</HeaderStyle>

<ItemStyle CssClass="Normal">
</ItemStyle>
</asp:BoundColumn>
<asp:TemplateColumn HeaderText="Overdue">
<HeaderStyle CssClass="NormalBold">
</HeaderStyle>

<ItemStyle CssClass="Normal">
</ItemStyle>

<ItemTemplate>
					<%# GetOverdueText(Convert.ToDouble(DataBinder.Eval(Container.DataItem,"OverdueBy"))) %>
				
</ItemTemplate>
</asp:TemplateColumn>
<asp:BoundColumn DataField="RemainingTime" HeaderText="TimeRemaining">
<HeaderStyle CssClass="NormalBold">
</HeaderStyle>

<ItemStyle CssClass="Normal">
</ItemStyle>
</asp:BoundColumn>
<asp:BoundColumn DataField="ObjectDependencies" HeaderText="ObjectDependencies">
<HeaderStyle CssClass="NormalBold">
</HeaderStyle>

<ItemStyle CssClass="Normal">
</ItemStyle>
</asp:BoundColumn>
<asp:BoundColumn DataField="ScheduleSource" HeaderText="TriggeredBy">
<HeaderStyle CssClass="NormalBold">
</HeaderStyle>

<ItemStyle CssClass="Normal">
</ItemStyle>
</asp:BoundColumn>
<asp:TemplateColumn HeaderText="Servers">
<HeaderStyle CssClass="NormalBold">
</HeaderStyle>

<ItemStyle CssClass="Normal">
</ItemStyle>

<ItemTemplate>
					<%# DataBinder.Eval(Container.DataItem,"Servers") %>
				
</ItemTemplate>
</asp:TemplateColumn>
</Columns>
</asp:datagrid>
</asp:panel>
<P>
	<asp:linkbutton id="cmdCancel" runat="server" resourcekey="cmdCancel" cssclass="CommandButton">Cancel</asp:linkbutton></P>
