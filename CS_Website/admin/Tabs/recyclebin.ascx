<%@ Control Language="C#" AutoEventWireup="true" Inherits="DotNetNuke.Modules.Admin.Tabs.RecycleBin" CodeFile="RecycleBin.ascx.cs" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register TagPrefix="dnn" TagName="HelpButton" Src="~/controls/HelpButtonControl.ascx" %>
<%@ Register TagPrefix="dnn" TagName="SectionHead" Src="~/controls/SectionHeadControl.ascx" %>
<table class="Settings" cellspacing="2" cellpadding="2" summary="Recycle Bin Design Table" border="0">
	<tr>
		<td width="560">
			<asp:panel id="pnlTabs" runat="server" cssclass="WorkPanel" visible="True">
				<dnn:sectionhead id="dshBasic" cssclass="Head" runat="server" includerule="True" resourcekey="Tabs"	section="tblTabs" text="Tabs"></dnn:sectionhead>
				<TABLE id="tblTabs" cellSpacing="0" cellPadding="2" width="525" summary="Tbas Design Table"	border="0" runat="server">
					<TR vAlign="top">
						<TD width="250">
							<asp:listbox id="lstTabs" runat="server" CssClass="Normal" Width="350px" Rows="5" DataTextField="TabName" DataValueField="TabId" SelectionMode="Multiple"></asp:listbox>
						</TD>
						<TD vAlign="top">
							<TABLE summary="Tabs Design Table">
								<TR>
									<TD vAlign="top">
										<asp:imagebutton id="cmdRestoreTab" runat="server" resourcekey="cmdRestoreTab" imageurl="~/images/restore.gif"></asp:imagebutton>
									</TD>
									<TD vAlign="top">
										<dnn:helpbutton id="hbtnRestoreTabHelp" resourcekey="cmdRestoreTab" runat="server" /></dnn:helpbutton>
									</TD>
								</TR>
								<TR>
									<TD vAlign="top">
										<asp:imagebutton id="cmdDeleteTab" runat="server" resourcekey="cmdDeleteTab" imageurl="~/images/delete.gif"></asp:imagebutton></TD>
									<TD vAlign="top">
										<dnn:helpbutton id="hbtnDeleteTabHelp" resourcekey="cmdDeleteTab" runat="server" /></dnn:helpbutton>
									</TD>
								</TR>
							</TABLE>
						</TD>
					</TR>
				</TABLE>
				<BR>
				<dnn:sectionhead id="Sectionhead1" cssclass="Head" runat="server" includerule="True" resourcekey="Modules" section="tblModules" text="Modules"></dnn:sectionhead>
				<TABLE id="tblModules" cellSpacing="0" cellPadding="0" width="525" summary="Basic Settings Design Table" border="0" runat="server">
					<TR vAlign="Top">
						<TD width="250">
							<asp:listbox id="lstModules" runat="server" CssClass="Normal" Width="350px" Rows="5" DataTextField="ModuleTitle" DataValueField="ModuleId" SelectionMode="Multiple"></asp:listbox>
						</TD>
						<TD vAlign="top">
							<TABLE summary="Tabs Design Table">
								<TR>
									<TD vAlign="top">
										<asp:imagebutton id="cmdRestoreModule" runat="server" alternatetext="Restore Module" imageurl="~/images/restore.gif"></asp:imagebutton>
									</TD>
									<TD vAlign="top">
										<dnn:helpbutton id="hbtnRestoreModuleHelp" resourcekey="cmdRestoreModule" runat="server" /></dnn:helpbutton>
									</TD>
								</TR>
								<TR>
									<TD vAlign="top">
										<asp:imagebutton id="cmdDeleteModule" runat="server" alternatetext="Delete Module" imageurl="~/images/delete.gif"></asp:imagebutton>
									</TD>	
									<TD vAlign="top">
										<dnn:helpbutton id="hbtnDeleteModuleHelp" resourcekey="cmdDeleteModule" runat="server" /></dnn:helpbutton>
									</TD>
								</TR>
							</TABLE>
						</TD>
					</TR>
					<TR>
						<TD class="SubHead" align="center"><dnn:label id="plTab" runat="server" resourcekey="Tab" suffix=":" helpkey="TabHelp" controlname="cboTab"></dnn:label></TD>
						<TD>&nbsp;</TD>
					</TR>
					<TR>
						<TD><asp:dropdownlist id="cboTab" cssclass="NormalTextBox" runat="server" width="350" datatextfield="TabName" datavaluefield="TabId"></asp:dropdownlist></TD>
						<TD>&nbsp;</TD>
					</TR>
				</TABLE>
			</asp:panel>
		</td>
		<td width="10">&nbsp;</td>
	</tr>
</table>
<p>
	<asp:linkbutton id="cmdEmpty" resourcekey="cmdEmpty" cssclass="CommandButton" runat="server">Empty Recycle Bin</asp:linkbutton>
</p>
