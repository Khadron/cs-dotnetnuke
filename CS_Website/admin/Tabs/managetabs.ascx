<%@ Register TagPrefix="dnn" TagName="SectionHead" Src="~/controls/SectionHeadControl.ascx" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register TagPrefix="dnn" TagName="URL" Src="~/controls/URLControl.ascx" %>
<%@ Register TagPrefix="Portal" Namespace="DotNetNuke.Security.Permissions.Controls" Assembly="DotNetNuke" %>
<%@ Register TagPrefix="dnn" TagName="Skin" Src="~/controls/SkinControl.ascx" %>
<%@ Control Language="C#" AutoEventWireup="false" Explicit="True" CodeFile="ManageTabs.ascx.cs" Inherits="DotNetNuke.Modules.Admin.Tabs.ManageTabs"%>
<table class="Settings" cellspacing="2" cellpadding="2" summary="Manage Tabs Design Table"
	border="0">
	<tr>
		<td width="560" valign="top">
			<asp:panel id="pnlSettings" runat="server" cssclass="WorkPanel" visible="True">
				<dnn:sectionhead id="dshBasic" cssclass="Head" runat="server" text="Basic Settings" section="tblBasic"
					resourcekey="BasicSettings" includerule="True"></dnn:sectionhead>
				<TABLE id="tblBasic" cellSpacing="0" cellPadding="2" width="525" summary="Basic Settings Design Table"
					border="0" runat="server">
					<TR>
						<TD colSpan="2">
							<asp:label id="lblBasicSettingsHelp" cssclass="Normal" runat="server" resourcekey="BasicSettingsHelp"
								enableviewstate="False"></asp:label></TD>
					</TR>
					<TR>
						<TD width="25"></TD>
						<TD vAlign="top" width="475">
							<dnn:sectionhead id="dshPage" cssclass="Head" runat="server" text="Page Details" section="tblPage"
								resourcekey="PageDetails"></dnn:sectionhead>
							<TABLE id="tblPage" cellSpacing="2" cellPadding="2" summary="Site Details Design Table"
								border="0" runat="server">
								<TR>
									<TD class="SubHead" width="150">
										<dnn:label id="plTabName" runat="server" resourcekey="TabName" suffix=":" helpkey="TabNameHelp"
											controlname="txtTabName"></dnn:label></TD>
									<TD width="325">
										<asp:textbox id="txtTabName" cssclass="NormalTextBox" runat="server" maxlength="50" width="300"></asp:textbox>
										<asp:requiredfieldvalidator id="valTabName" cssclass="NormalRed" runat="server" resourcekey="valTabName.ErrorMessage"
											display="Dynamic" errormessage="<br>Tab Name Is Required" controltovalidate="txtTabName"></asp:requiredfieldvalidator></TD>
								</TR>
								<TR>
									<TD class="SubHead" width="150">
										<dnn:label id="plTitle" runat="server" resourcekey="Title" suffix=":" helpkey="TitleHelp" controlname="txtTitle"></dnn:label></TD>
									<TD>
										<asp:textbox id="txtTitle" cssclass="NormalTextBox" runat="server" maxlength="200" width="300"></asp:textbox></TD>
								</TR>
								<TR>
									<TD class="SubHead" width="150">
										<dnn:label id="plDescription" runat="server" resourcekey="Description" suffix=":" helpkey="DescriptionHelp"
											controlname="txtDescription"></dnn:label></TD>
									<TD class="NormalTextBox" width="325">
										<asp:textbox id="txtDescription" cssclass="NormalTextBox" runat="server" maxlength="500" width="300"
											textmode="MultiLine" rows="3"></asp:textbox></TD>
								</TR>
								<TR>
									<TD class="SubHead" width="150">
										<dnn:label id="plKeywords" runat="server" resourcekey="KeyWords" suffix=":" helpkey="KeyWordsHelp"
											controlname="txtKeyWords"></dnn:label></TD>
									<TD class="NormalTextBox" width="325">
										<asp:textbox id="txtKeyWords" cssclass="NormalTextBox" runat="server" maxlength="500" width="300"
											textmode="MultiLine" rows="3"></asp:textbox></TD>
								</TR>
								<TR>
									<TD class="SubHead" width="150">
										<dnn:label id="plParentTab" runat="server" resourcekey="ParentTab" suffix=":" helpkey="ParentTabHelp"
											controlname="cboTab"></dnn:label></TD>
									<TD width="325">
										<asp:dropdownlist id="cboTab" cssclass="NormalTextBox" runat="server" width="300" datatextfield="TabName"
											datavaluefield="TabId"></asp:dropdownlist></TD>
								</TR>
								<TR id="rowTemplate" runat="server">
									<TD class="SubHead" width="150">
										<dnn:label id="plTemplate" runat="server" resourcekey="Template" suffix=":" helpkey="TemplateHelp"
											controlname="cboTemplate"></dnn:label></TD>
									<TD width="325">
										<asp:dropdownlist id="cboTemplate" cssclass="NormalTextBox" runat="server" width="300" DataValueField="Value" DataTextField="Text"></asp:dropdownlist></TD>
								</TR>
								<TR>
									<TD class="SubHead" vAlign="top" width="150"><BR>
										<BR>
										<dnn:label id="plPermissions" runat="server" resourcekey="Permissions" suffix=":" helpkey="PermissionsHelp"
											controlname="dgPermissions"></dnn:label></TD>
									<TD width="325">
										<Portal:TabPermissionsGrid id="dgPermissions" runat="server"></Portal:TabPermissionsGrid></TD>
								</TR>
							</TABLE>
						</TD>
					</TR>
				</TABLE>
				<BR>
				<dnn:sectionhead id="dshCopy" cssclass="Head" runat="server" text="Copy Page" section="tblCopy" resourcekey="Copy"
					includerule="True"></dnn:sectionhead>
				<TABLE id="tblCopy" cellSpacing="0" cellPadding="2" width="525" summary="Copy Tab Design Table"
					border="0" runat="server">
					<TR>
						<TD width="25"></TD>
						<TD class="SubHead" width="150">
							<dnn:label id="plCopyPage" runat="server" resourcekey="CopyModules" suffix=":" helpkey="CopyModulesHelp"
								controlname="cboCopyPage"></dnn:label></TD>
						<TD width="325">
							<asp:dropdownlist id="cboCopyPage" cssclass="NormalTextBox" runat="server" width="300" datatextfield="TabName"
								datavaluefield="TabId" AutoPostBack="True"></asp:dropdownlist></TD>
					</TR>
					<TR id="rowModules" runat="server">
						<TD width="25"></TD>
						<TD class="SubHead" colSpan="2">
							<dnn:label id="plModules" runat="server" resourcekey="CopyContent" suffix=":" helpkey="CopyContentHelp"
								controlname="grdModules"></dnn:label><BR>
							<asp:DataGrid id="grdModules" runat="server" DataKeyField="ModuleID" ShowHeader="False" ItemStyle-CssClass="Normal"
								GridLines="None" BorderWidth="0px" BorderStyle="None" AutoGenerateColumns="False" CellSpacing="0"
								CellPadding="0" Width="100%">
								<Columns>
									<asp:TemplateColumn>
										<ItemTemplate>
											<asp:CheckBox ID="chkModule" runat="server" CssClass="NormalTextBox" Checked="True" />
										</ItemTemplate>
									</asp:TemplateColumn>
									<asp:TemplateColumn>
										<ItemTemplate>
											<asp:TextBox ID="txtCopyTitle" Width="150" runat="server" CssClass="NormalTextBox" Text='<%# DataBinder.Eval(Container.DataItem,"ModuleTitle")%>'>
											</asp:TextBox>
										</ItemTemplate>
										<ItemStyle Wrap="False"></ItemStyle>
									</asp:TemplateColumn>
									<asp:BoundColumn runat="server" DataField="PaneName" />
									<asp:TemplateColumn>
										<ItemTemplate>
											<asp:RadioButton ID="optNew" Runat="server" CssClass="NormalBold" GroupName="Copy" Text="New" resourcekey="ModuleNew"
												Checked="True"></asp:RadioButton>
											<asp:RadioButton ID="optCopy" Runat="server" CssClass="NormalBold" GroupName="Copy" Text="Copy" resourcekey="ModuleCopy" Enabled='<%# DataBinder.Eval(Container.DataItem, "IsPortable") %>'>
											</asp:RadioButton>
											<asp:RadioButton ID="optReference" Runat="server" CssClass="NormalBold" GroupName="Copy" Text="Reference" resourcekey="ModuleReference" Enabled='<%# Convert.ToInt32(DataBinder.Eval(Container.DataItem, "ModuleID")) != -1  %>'>
											</asp:RadioButton>
										</ItemTemplate>
										<ItemStyle Wrap="False"></ItemStyle>
									</asp:TemplateColumn>
								</Columns>
							</asp:DataGrid></TD>
					</TR>
				</TABLE>
				<BR>
				<dnn:sectionhead id="dshAdvanced" cssclass="Head" runat="server" text="Advanced Settings" section="tblAdvanced"
					resourcekey="AdvancedSettings" includerule="True" isexpanded="False"></dnn:sectionhead>
				<TABLE id="tblAdvanced" cellSpacing="0" cellPadding="2" width="525" summary="Advanced Settings Design Table"
					border="0" runat="server">
					<TR>
						<TD colSpan="2">
							<asp:label id="lblAdvancedSettingsHelp" cssclass="Normal" runat="server" resourcekey="AdvancedSettingsHelp"
								enableviewstate="False"></asp:label></TD>
					</TR>
					<TR>
						<TD width="25"></TD>
						<TD vAlign="top" width="475">
							<dnn:sectionhead id="dhsAppearance" cssclass="Head" runat="server" text="Appearance" section="tblAppearance"
								resourcekey="Appearance"></dnn:sectionhead>
							<TABLE id="tblAppearance" cellSpacing="2" cellPadding="2" summary="Appearance Design Table"
								border="0" runat="server">
								<TR>
									<TD class="SubHead" vAlign="top" width="150">
										<dnn:label id="plIcon" runat="server" resourcekey="Icon" suffix=":" helpkey="IconHelp" controlname="ctlIcon"></dnn:label></TD>
									<TD width="325">
										<dnn:url id="ctlIcon" runat="server" width="300" showlog="False"></dnn:url></TD>
								</TR>
								<TR>
									<TD class="SubHead" vAlign="top" width="150">
										<dnn:label id="plSkin" runat="server" resourcekey="TabSkin" suffix=":" helpkey="TabSkinHelp"
											controlname="ctlSkin"></dnn:label></TD>
									<TD vAlign="top" width="325">
										<dnn:skin id="ctlSkin" runat="server"></dnn:skin></TD>
								</TR>
								<TR>
									<TD class="SubHead" vAlign="top" width="150">
										<dnn:label id="plContainer" runat="server" resourcekey="TabContainer" suffix=":" helpkey="TabContainerHelp"
											controlname="ctlContainer"></dnn:label></TD>
									<TD vAlign="top" width="325">
										<dnn:skin id="ctlContainer" runat="server"></dnn:skin></TD>
								</TR>
								<TR>
									<TD class="SubHead" vAlign="top" width="150">
										<dnn:label id="plHidden" runat="server" resourcekey="Hidden" suffix=":" helpkey="HiddenHelp"
											controlname="chkHidden"></dnn:label></TD>
									<TD width="325">
										<asp:checkbox id="chkHidden" runat="server" font-size="8pt" font-names="Verdana,Arial"></asp:checkbox></TD>
								</TR>
								<TR>
									<TD class="SubHead" vAlign="top" width="150">
										<dnn:label id="plDisable" runat="server" resourcekey="Disabled" suffix=":" helpkey="DisabledHelp"
											controlname="chkDisableLink"></dnn:label></TD>
									<TD width="325">
										<asp:checkbox id="chkDisableLink" runat="server" font-size="8pt" font-names="Verdana,Arial"></asp:checkbox></TD>
								</TR>
								<TR>
									<TD class="SubHead" width="150">
										<dnn:label id="plRefreshInterval" runat="server" resourcekey="RefreshInterval" suffix=":" helpkey="RefreshInterval.Help"
											controlname="cboRefreshInterval" ></dnn:label></TD>
									<TD width="325">
										<asp:TextBox id="txtRefreshInterval" cssclass="NormalTextBox" runat="server"></asp:TextBox></TD>
								</TR>
								<TR>
									<TD class="SubHead" vAlign="top" width="150">
										<dnn:label id="plPageHeadText" runat="server" resourcekey="PageHeadText" suffix=":" helpkey="PageHeadText.Help"
											controlname="txtPageHeadText"></dnn:label></TD>
									<TD class="NormalTextBox" width="325">
										<asp:TextBox id="txtPageHeadText" cssclass="NormalTextBox" runat="server" textmode="MultiLine"
											rows="4" columns="50"></asp:TextBox></TD>
								</TR>
							</TABLE>
							<BR>
							<dnn:sectionhead id="dshOther" cssclass="Head" runat="server" text="Other Settings" section="tblOther"
								resourcekey="OtherSettings"></dnn:sectionhead>
							<TABLE id="tblOther" cellSpacing="2" cellPadding="2" summary="Appearance Design Table"
								border="0" runat="server">
								<TR>
									<TD class="SubHead" vAlign="top" width="150">
										<dnn:label id="plStartDate" runat="server" text="Start Date:" controlname="txtStartDate"></dnn:label></TD>
									<TD>
										<asp:textbox id="txtStartDate" cssclass="NormalTextBox" runat="server" maxlength="11" width="120"
											columns="30"></asp:textbox>&nbsp;
										<asp:hyperlink id="cmdStartCalendar" cssclass="CommandButton" runat="server" resourcekey="Calendar">Calendar</asp:hyperlink>
										<asp:CompareValidator id="valtxtStartDate" resourcekey="valStartDate.ErrorMessage" Operator="DataTypeCheck"
											Type="Date" ErrorMessage="<br>Invalid Start Date" Runat="server" Display="Dynamic" ControlToValidate="txtStartDate"></asp:CompareValidator></TD>
								</TR>
								<TR>
									<TD class="SubHead" vAlign="top" width="150">
										<dnn:label id="plEndDate" runat="server" text="End Date:" controlname="txtEndDate"></dnn:label></TD>
									<TD>
										<asp:textbox id="txtEndDate" cssclass="NormalTextBox" runat="server" maxlength="11" width="120"
											columns="30"></asp:textbox>&nbsp;
										<asp:hyperlink id="cmdEndCalendar" cssclass="CommandButton" runat="server" resourcekey="Calendar">Calendar</asp:hyperlink>
										<asp:CompareValidator id="valtxtEndDate" resourcekey="valEndDate.ErrorMessage" Operator="DataTypeCheck"
											Type="Date" ErrorMessage="<br>Invalid End Date" Runat="server" Display="Dynamic" ControlToValidate="txtEndDate"></asp:CompareValidator></TD>
								</TR>
								<TR>
									<TD class="SubHead" vAlign="top" width="150">
										<dnn:label id="plURL" runat="server" resourcekey="Url" suffix=":" helpkey="UrlHelp" controlname="ctlURL"></dnn:label></TD>
									<TD class="NormalTextBox" width="325">
										<dnn:url id="ctlURL" runat="server" width="300" showlog="False" shownone="True" showtrack="False"></dnn:url></TD>
								</TR>
							</TABLE>
						</TD>
					</TR>
				</TABLE>
			</asp:panel>
		</td>
	</tr>
</table>
<p>
	<asp:linkbutton cssclass="CommandButton" id="cmdUpdate" resourcekey="cmdUpdate" runat="server" Text="Update"></asp:linkbutton>&nbsp;&nbsp;
	<asp:linkbutton cssclass="CommandButton" id="cmdCancel" resourcekey="cmdCancel" runat="server" Text="Cancel"
		CausesValidation="False" BorderStyle="none"></asp:linkbutton>&nbsp;&nbsp;
	<asp:linkbutton cssclass="CommandButton" id="cmdDelete" resourcekey="cmdDelete" runat="server" Text="Delete"
		CausesValidation="False" BorderStyle="none"></asp:linkbutton>&nbsp;&nbsp;
	<asp:linkbutton cssclass="CommandButton" id="cmdGoogle" resourcekey="SubmitToGoogle" runat="server"
		text="Submit Tab To Google"></asp:linkbutton>
</p>
