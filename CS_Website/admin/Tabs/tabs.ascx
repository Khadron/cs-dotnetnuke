<%@ Control Inherits="DotNetNuke.Modules.Admin.Tabs.Tabs" Language="C#" AutoEventWireup="true" Explicit="True" CodeFile="Tabs.ascx.cs" %>
<%@ Register TagPrefix="dnn" TagName="HelpButton" Src="~/controls/HelpButtonControl.ascx" %>
<table class="Settings" cellspacing="2" cellpadding="2" summary="Tabs Design Table" border="0">
	<tr>
		<td width="560">
			<asp:panel id="pnlTabs" runat="server" cssclass="WorkPanel" visible="True">
				<table cellspacing="0" cellpadding="0" border="0" summary="Tabs Design Table">
					<tr valign="top">
						<td width="400">
							<label style="DISPLAY:none" for="<%=lstTabs.ClientID%>">First Tabs</label>
							<asp:listbox id="lstTabs" runat="server" rows="22" datatextfield="TabName" datavaluefield="TabId" cssclass="NormalTextBox" width="400px"></asp:listbox>
						</td>
						<td>&nbsp;</td>
						<td>
							<table summary="Tabs Design Table">
								<tr>
									<td colspan="2" valign="top" class="SubHead">
										<asp:label id="lblMovePage" runat="server" resourcekey="MovePage">Move Page</asp:label>
										<hr noshade size="1">
									</td>
								</tr>
								<tr>
									<td valign="top" width="10%">
										<asp:imagebutton id="cmdUp" resourcekey="cmdUp.Help" runat="server" alternatetext="Move Tab Up In Current Level" commandname="up" imageurl="~/images/up.gif"></asp:imagebutton>
									</td>
									<td valign="top" width="90%">
										<dnn:HelpButton id="hbtnUpHelp" resourcekey="cmdUp" runat="server" /></dnn:helpbutton>
									</td>
								</tr>
								<tr>
									<td valign="top" width="10%">
										<asp:imagebutton id="cmdDown" resourcekey="cmdDown.Help" runat="server" alternatetext="Move Tab Down In Current Level" commandname="down" imageurl="~/images/dn.gif"></asp:imagebutton>
									</td>
									<td valign="top" width="90%">
										<dnn:helpbutton id="hbtnDownHelp" resourcekey="cmdDown" runat="server" /></dnn:helpbutton>
									</td>
								</tr>
								<tr>
									<td valign="top" width="10%">
										<asp:imagebutton id="cmdLeft" resourcekey="cmdLeft.Help" runat="server" alternatetext="Move Tab Up One Hierarchical Level" commandname="left" imageurl="~/images/lt.gif"></asp:imagebutton>
									</td>
									<td valign="top" width="90%">
										<dnn:helpbutton id="hbtnLeftHelp" resourcekey="cmdLeft" runat="server" /></dnn:helpbutton>
									</td>
								</tr>
								<tr>
									<td valign="top" width="10%">
										<asp:imagebutton id="cmdRight" resourcekey="cmdRight.Help" runat="server" alternatetext="Move Tab Down One Hierarchical Level" commandname="right" imageurl="~/images/rt.gif"></asp:imagebutton>
									</td>
									<td valign="top" width="90%">
										<dnn:helpbutton id="hbtnRightHelp" resourcekey="cmdRight" runat="server" /></dnn:helpbutton>
									</td>
								</tr>
								<tr>
									<td colspan="2" height="25">&nbsp;</td>
								</tr>
								<tr>
									<td colspan="2" valign="top" class="SubHead">
										<asp:label id="lblActions" runat="server" resourcekey="Actions">Actions</asp:label>
										<hr noshade size="1">
									</td>
								</tr>
								<tr>
									<td valign="top" width="10%">
										<asp:imagebutton id="cmdEdit" resourcekey="cmdEdit.Help" runat="server" alternatetext="Edit Tab" imageurl="~/images/edit.gif"></asp:imagebutton>
									</td>
									<td valign="top" width="90%">
										<dnn:helpbutton id="hbtnEditHelp" resourcekey="cmdEdit" runat="server" /></dnn:helpbutton>
									</td>
								</tr>
								<tr>
									<td valign="top" width="10%">
										<asp:imagebutton id="cmdView" resourcekey="cmdView.Help" runat="server" alternatetext="View Tab" imageurl="~/images/view.gif"></asp:imagebutton>
									</td>
									<td valign="top" width="90%">
										<dnn:helpbutton id="hbtnViewHelp" resourcekey="cmdView" runat="server" /></dnn:helpbutton>
									</td>
								</tr>
							</table>
						</td>
					</tr>
				</table>
			</asp:panel>
		<td width="10">&nbsp;</td>
	</tr>
</table>
