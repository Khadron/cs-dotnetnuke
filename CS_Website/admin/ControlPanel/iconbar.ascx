<%@ Control Language="C#" AutoEventWireup="false" Explicit="True" Inherits="DotNetNuke.UI.ControlPanels.IconBar" CodeFile="IconBar.ascx.cs" %>
<table class="ControlPanel" cellspacing="0" cellpadding="0" border="0">
	<tr>
		<td>
			<table cellspacing="0" cellpadding="2" width="100%">
				<tr>
					<td align="center" class="SubHead"><asp:Label ID="lblPageFunctions" Runat="server" CssClass="SubHead" enableviewstate="False"><font size="1">Page 
								Functions</font></asp:Label></td>
					<td rowspan="2" align="center" width="1"><div class="ControlPanel"></div>
					</td>
					<td align="center" class="SubHead">
						<asp:radiobuttonlist id="optModuleType" cssclass="SubHead" runat="server" repeatdirection="Horizontal"
							repeatlayout="Flow" autopostback="True">
							<asp:listitem value="0" resourcekey="optModuleTypeNew">Add New Module</asp:listitem>
							<asp:listitem value="1" resourcekey="optModuleTypeExisting">Add Existing Module</asp:listitem>
						</asp:radiobuttonlist>
					</td>
					<td rowspan="2" align="center" width="1"><div class="ControlPanel"></div>
					</td>
					<td align="center" class="SubHead"><asp:Label ID="lblCommonTasks" Runat="server" CssClass="SubHead" enableviewstate="False"><font size="1">Common 
								Tasks</font></asp:Label></td>
				</tr>
				<tr>
					<td align="center" valign="top">
						<table cellspacing="0" cellpadding="2" border="0">
							<tr valign="bottom" height="24">
								<td width="35" align="center"><asp:LinkButton ID="cmdAddTabIcon" Runat="server" CssClass="CommandButton" CausesValidation="False">
										<asp:Image ID="imgAddTabIcon" Runat="server" ImageUrl="~/admin/ControlPanel/images/iconbar_addtab.gif"></asp:Image>
									</asp:LinkButton></td>
								<td width="35" align="center"><asp:LinkButton ID="cmdEditTabIcon" Runat="server" CssClass="CommandButton" CausesValidation="False">
										<asp:Image ID="imgEditTabIcon" Runat="server" ImageUrl="~/admin/ControlPanel/images/iconbar_edittab.gif"></asp:Image>
									</asp:LinkButton></td>
								<td width="35" align="center"><asp:LinkButton ID="cmdDeleteTabIcon" Runat="server" CssClass="CommandButton" CausesValidation="False">
										<asp:Image ID="imgDeleteTabIcon" Runat="server" ImageUrl="~/admin/ControlPanel/images/iconbar_deletetab.gif"></asp:Image>
									</asp:LinkButton></td>
								<td width="35" align="center"><asp:LinkButton ID="cmdCopyTabIcon" Runat="server" CssClass="CommandButton" CausesValidation="False">
										<asp:Image ID="imgCopyTabIcon" Runat="server" ImageUrl="~/admin/ControlPanel/images/iconbar_copytab.gif"></asp:Image>
									</asp:LinkButton></td>
								<td width="35" align="center"><asp:LinkButton ID="cmdPreviewTabIcon" Runat="server" CssClass="CommandButton" CausesValidation="False">
										<asp:Image ID="imgPreviewTabIcon" Runat="server" ImageUrl="~/admin/ControlPanel/images/iconbar_previewtab.gif"></asp:Image>
									</asp:LinkButton></td>
							</tr>
							<tr valign="bottom">
								<td width="35" align="center" class="Normal"><asp:LinkButton ID="cmdAddTab" Runat="server" CssClass="CommandButton" CausesValidation="False">Add</asp:LinkButton></td>
								<td width="35" align="center" class="Normal"><asp:LinkButton ID="cmdEditTab" Runat="server" CssClass="CommandButton" CausesValidation="False">Settings</asp:LinkButton></td>
								<td width="35" align="center" class="Normal"><asp:LinkButton ID="cmdDeleteTab" Runat="server" CssClass="CommandButton" CausesValidation="False">Delete</asp:LinkButton></td>
								<td width="35" align="center" class="Normal"><asp:LinkButton ID="cmdCopyTab" Runat="server" CssClass="CommandButton" CausesValidation="False">Copy</asp:LinkButton></td>
								<td width="35" align="center" class="Normal"><asp:LinkButton ID="cmdPreviewTab" Runat="server" CssClass="CommandButton" CausesValidation="False">Preview</asp:LinkButton></td>
							</tr>
						</table>
					</td>
					<td align="center" valign="top" height="100%">
						<table cellspacing="1" cellpadding="0" border="0" height="100%">
							<tr>
								<td align="center">
									<table cellspacing="1" cellpadding="0" border="0" height="100%">
										<tr valign="bottom">
											<td class="SubHead" align="right" nowrap><asp:Label ID="lblModule" Runat="server" CssClass="SubHead" enableviewstate="False">Module:</asp:Label>&nbsp;</td>
											<td nowrap><asp:dropdownlist id="cboTabs" runat="server" cssclass="NormalTextBox" Width="140" datavaluefield="TabID"
													datatextfield="TabName" visible="False" autopostback="True"></asp:dropdownlist><asp:dropdownlist id="cboDesktopModules" runat="server" cssclass="NormalTextBox" Width="140" datavaluefield="DesktopModuleID"
													datatextfield="FriendlyName"></asp:dropdownlist>&nbsp;&nbsp;</td>
											<td class="SubHead" align="right" nowrap><asp:Label ID="lblPane" Runat="server" CssClass="SubHead" enableviewstate="False">Pane:</asp:Label>&nbsp;</td>
											<td nowrap><asp:dropdownlist id="cboPanes" runat="server" cssclass="NormalTextBox" Width="110"></asp:dropdownlist>&nbsp;&nbsp;</td>
											<td align="center" nowrap><asp:LinkButton id="cmdAddModuleIcon" runat="server" cssclass="CommandButton" CausesValidation="False">
													<asp:Image runat="server" EnableViewState="False" ID="imgAddModuleIcon" ImageUrl="~/admin/ControlPanel/images/iconbar_addmodule.gif"></asp:Image>
												</asp:LinkButton></td>
										</tr>
										<tr valign="bottom">
											<td class="SubHead" align="right" nowrap><asp:Label ID="lblTitle" Runat="server" CssClass="SubHead" enableviewstate="False">Title:</asp:Label>&nbsp;</td>
											<td nowrap><asp:dropdownlist id="cboModules" runat="server" cssclass="NormalTextBox" Width="140" datavaluefield="ModuleID"
													datatextfield="ModuleTitle" visible="False"></asp:dropdownlist><asp:TextBox ID="txtTitle" Runat="server" CssClass="NormalTextBox" Width="140"></asp:TextBox>&nbsp;&nbsp;</td>
											<td class="SubHead" align="right" nowrap><asp:Label ID="lblPosition" Runat="server" CssClass="SubHead" resourcekey="Position" enableviewstate="False">Insert:</asp:Label>&nbsp;</td>
											<td nowrap>
												<asp:dropdownlist id="cboPosition" runat="server" CssClass="NormalTextBox" Width="110">
													<asp:ListItem Value="0" resourcekey="Top">Top</asp:ListItem>
													<asp:ListItem Value="-1" resourcekey="Bottom">Bottom</asp:ListItem>
												</asp:dropdownlist>&nbsp;&nbsp;
											</td>
											<td align="center" class="Normal" nowrap><asp:linkbutton id="cmdAddModule" runat="server" cssclass="CommandButton" CausesValidation="False">Add</asp:linkbutton></td>
										</tr>
										<tr valign="bottom">
											<td class="SubHead" align="right" nowrap><asp:Label ID="lblPermission" Runat="server" CssClass="SubHead" resourcekey="Permission" enableviewstate="False">Visibility:</asp:Label>&nbsp;</td>
											<td nowrap>
												<asp:dropdownlist id="cboPermission" runat="server" CssClass="NormalTextBox" Width="140">
													<asp:ListItem Value="0" resourcekey="PermissionView">Same As Page</asp:ListItem>
													<asp:ListItem Value="1" resourcekey="PermissionEdit">Page Editors Only</asp:ListItem>
												</asp:dropdownlist>&nbsp;&nbsp;
											</td>
											<td class="SubHead" align="right" nowrap><asp:Label ID="lblAlign" Runat="server" CssClass="SubHead" enableviewstate="False">Align:</asp:Label>&nbsp;</td>
											<td nowrap>
												<asp:dropdownlist id="cboAlign" runat="server" CssClass="NormalTextBox" Width="110">
													<asp:ListItem Value="left" resourcekey="Left">Left</asp:ListItem>
													<asp:ListItem Value="center" resourcekey="Center">Center</asp:ListItem>
													<asp:ListItem Value="right" resourcekey="Right">Right</asp:ListItem>
												</asp:dropdownlist>&nbsp;&nbsp;
											</td>
											<td align="center" nowrap>&nbsp;</td>
										</tr>
									</table>
								</td>
							</tr>
						</table>
					</td>
					<td align="center" valign="top">
						<table cellspacing="0" cellpadding="2" border="0">
							<tr valign="bottom" height="24">
								<td width="35" align="center"><asp:LinkButton ID="cmdWizardIcon" Runat="server" CssClass="CommandButton" CausesValidation="False">
										<asp:Image ID="imgWizardIcon" Runat="server" ImageUrl="~/admin/ControlPanel/images/iconbar_wizard.gif"></asp:Image>
									</asp:LinkButton></td>
								<td width="35" align="center"><asp:LinkButton ID="cmdSiteIcon" Runat="server" CssClass="CommandButton" CausesValidation="False">
										<asp:Image ID="imgSiteIcon" Runat="server" ImageUrl="~/admin/ControlPanel/images/iconbar_site.gif"></asp:Image>
									</asp:LinkButton></td>
								<td width="35" align="center"><asp:LinkButton ID="cmdUsersIcon" Runat="server" CssClass="CommandButton" CausesValidation="False">
										<asp:Image ID="imgUsersIcon" Runat="server" ImageUrl="~/admin/ControlPanel/images/iconbar_users.gif"></asp:Image>
									</asp:LinkButton></td>
								<td width="35" align="center"><asp:LinkButton ID="cmdFilesIcon" Runat="server" CssClass="CommandButton" CausesValidation="False">
										<asp:Image ID="imgFilesIcon" Runat="server" ImageUrl="~/admin/ControlPanel/images/iconbar_files.gif"></asp:Image>
									</asp:LinkButton></td>
								<td width="35" align="center"><asp:Hyperlink ID="cmdHelpIcon" Runat="server" CssClass="CommandButton" CausesValidation="False"
										Target="_new">
										<asp:Image ID="imgHelpIcon" Runat="server" ImageUrl="~/admin/ControlPanel/images/iconbar_help.gif"></asp:Image>
									</asp:Hyperlink></td>
							</tr>
							<tr valign="bottom">
								<td width="35" align="center" class="Normal"><asp:LinkButton ID="cmdWizard" Runat="server" CssClass="CommandButton" CausesValidation="False">Wizard</asp:LinkButton></td>
								<td width="35" align="center" class="Normal"><asp:LinkButton ID="cmdSite" Runat="server" CssClass="CommandButton" CausesValidation="False">Site</asp:LinkButton></td>
								<td width="35" align="center" class="Normal"><asp:LinkButton ID="cmdUsers" Runat="server" CssClass="CommandButton" CausesValidation="False">Users</asp:LinkButton></td>
								<td width="35" align="center" class="Normal"><asp:LinkButton ID="cmdFiles" Runat="server" CssClass="CommandButton" CausesValidation="False">Files</asp:LinkButton></td>
								<td width="35" align="center" class="Normal"><asp:Hyperlink ID="cmdHelp" Runat="server" CssClass="CommandButton" CausesValidation="False" Target="_new">Help</asp:Hyperlink></td>
							</tr>
						</table>
					</td>
				</tr>
			</table>
		</td>
	</tr>
</table>
