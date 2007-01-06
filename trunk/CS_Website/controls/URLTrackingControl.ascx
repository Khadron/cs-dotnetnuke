<%@ Control Language="C#" AutoEventWireup="true"  Inherits="DotNetNuke.UI.UserControls.URLTrackingControl" %>
<table width="750" cellSpacing="0" cellPadding="2" summary="URL Tracking Design Table" border="0">
	<tr>
		<td class="SubHead" valign="middle" width="150"><asp:Label id="Label1" resourcekey="Url" runat="server" enableviewstate="False">URL</asp:Label>:</td>
		<td nowrap><asp:label id="lblURL" CssClass="Normal" Runat="server" Width="300"></asp:label>
			<asp:label id="lblLogURL" Runat="server" CssClass="Normal" Visible="False"></asp:label></td>
	</tr>
	<tr>
		<td class="SubHead" valign="middle" width="150"><asp:Label id="Label3" resourcekey="Created" runat="server" enableviewstate="False">Created</asp:Label>:</td>
		<td><asp:label id="lblCreatedDate" CssClass="Normal" Runat="server" Width="300"></asp:label></td>
	</tr>
</table>
<asp:Panel id="pnlTrack" runat="server" visible="False">
	<br>
	<table width="750" cellSpacing="0" cellPadding="2" summary="URL Tracking Design Table" border="0">
		<tr>
			<td class="SubHead" valign="middle" width="150"><asp:Label id="Label2" resourcekey="TrackingUrl" runat="server" enableviewstate="False">Tracking URL</asp:Label>:</td>
			<td nowrap><asp:label id="lblTrackingURL" CssClass="Normal" Runat="server" Width="300"></asp:label></td>
		</tr>
		<tr>
			<td class="SubHead" valign="middle" width="150"><asp:Label id="Label4" resourcekey="Clicks" runat="server" enableviewstate="False">Clicks</asp:Label>:</td>
			<td><asp:label id="lblClicks" CssClass="Normal" Runat="server" Width="300"></asp:label></td>
		</tr>
		<tr>
			<td class="SubHead" valign="middle" width="150"><asp:Label id="Label5" resourcekey="LastClick" runat="server" enableviewstate="False">Last Click</asp:Label>:</td>
			<td><asp:label id="lblLastClick" CssClass="Normal" Runat="server" Width="300"></asp:label></td>
		</tr>
	</table>
</asp:Panel>
<asp:Panel id="pnlLog" runat="server" visible="False">
	<BR>
	<TABLE cellSpacing="0" cellPadding="2" width="750" summary="URL Log Criteria Design Table"
		border="0">
		<TR>
			<TD class="SubHead" vAlign="middle" width="150">
			<LABEL for="<%= txtStartDate.ClientID%>">
				<asp:Label id="Label6" runat="server" resourcekey="StartDate" enableviewstate="False">Start Date</asp:Label>:
			</LABEL>
			</TD>
			<TD>
				<asp:TextBox id="txtStartDate" runat="server" CssClass="NormalTextBox" width="120" Columns="20"></asp:TextBox>&nbsp;
				<asp:HyperLink id="cmdStartCalendar" resourcekey="Calendar" Runat="server" CssClass="CommandButton" enableviewstate="False">Calendar</asp:HyperLink>
			</TD>
		</TR>
		<TR>
			<TD class="SubHead" vAlign="middle" width="150">
			<LABEL for="<%=txtEndDate.ClientID%>">
				<asp:Label id="Label7" runat="server" resourcekey="EndDate" enableviewstate="False">End Date</asp:Label>:
			</LABEL>
			</TD>
			<TD>
				<asp:TextBox id="txtEndDate" runat="server" CssClass="NormalTextBox" width="120" Columns="20"></asp:TextBox>&nbsp;
				<asp:HyperLink id="cmdEndCalendar" resourcekey="Calendar" Runat="server" CssClass="CommandButton" enableviewstate="False">Calendar</asp:HyperLink>
			</TD>
		</TR>
	</TABLE>
	<P>
	<asp:LinkButton id="cmdDisplay" runat="server" resourcekey="cmdDisplay" cssclass="CommandButton" Text="Display" enableviewstate="False"></asp:LinkButton></P>
	<asp:datagrid id="grdLog" runat="server" CellPadding="4" Summary="URL Log Design Table" EnableViewState="false" AutoGenerateColumns="false" CellSpacing="3" Border="0">
		<Columns>
			<asp:BoundColumn HeaderText="Date" DataField="ClickDate" ItemStyle-CssClass="Normal" HeaderStyle-Cssclass="NormalBold" />
			<asp:BoundColumn HeaderText="User" DataField="FullName" ItemStyle-CssClass="Normal" HeaderStyle-Cssclass="NormalBold" />
		</Columns>
	</asp:datagrid>
</asp:Panel>
