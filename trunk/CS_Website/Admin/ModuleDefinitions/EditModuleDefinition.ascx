<%@ Register TagPrefix="Portal" TagName="DualList" Src="~/controls/DualListControl.ascx" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Control Language="C#" CodeFile="EditModuleDefinition.ascx.cs" AutoEventWireup="true" Inherits="DotNetNuke.Modules.Admin.ModuleDefinitions.EditModuleDefinition" %>
<table cellspacing="0" cellpadding="4" border="0" summary="Module Definitions Design Table">
	<tr>
		<td>
			<table id="tabModule" runat="server" cellspacing="0" cellpadding="4" border="0" summary="Module Definitions Design Table">
				<tr id="rowManifest" runat="server">
					<td class="SubHead" width="150"><dnn:label id="plManifest" text="Manifest:" controlname="cboManifest" runat="server" /></td>
					<td>
						<asp:dropdownlist id="cboManifest" runat="server" width="390" cssclass="NormalTextBox"></asp:dropdownlist>
						<asp:linkbutton id="cmdInstall" resourcekey="cmdInstall" text="Install" runat="server" class="CommandButton" borderstyle="none" CausesValidation="False" OnClick="cmdInstall_Click" />
					</td>
				</tr>
				<tr>
					<td colspan="2">&nbsp;</td>
				</tr>
				<tr>
					<td class="SubHead" width="150"><dnn:label id="plModuleName" text="Module Name:" controlname="txtModuleName" runat="server" /></td>
					<td>
						<asp:textbox id="txtModuleName" cssclass="NormalTextBox" width="390" columns="30" maxlength="150"
							runat="server" enabled="False" />
						<asp:requiredfieldvalidator id="valModuleName" display="Dynamic" resourcekey="valModuleName.ErrorMessage" errormessage="<br>You must enter a Name for the Module."
							controltovalidate="txtModuleName" runat="server" />
					</td>
				</tr>
				<tr>
					<td class="SubHead" width="150"><dnn:label id="plFolderName" text="Folder Name:" controlname="txtFolderName" runat="server" /></td>
					<td>
						<asp:textbox id="txtFolderName" cssclass="NormalTextBox" width="390" columns="30" maxlength="150"
							runat="server" enabled="False" />
						<asp:requiredfieldvalidator id="valFolderName" display="Dynamic" resourcekey="valFolderName.ErrorMessage" errormessage="<br>You must enter a Folder Name for the location of the Module's files."
							controltovalidate="txtFolderName" runat="server" Width="335px" />
					</td>
				</tr>
				<tr>
					<td class="SubHead" width="150"><dnn:label id="plFriendlyName" text="Friendly Name:" controlname="txtFriendlyName" runat="server" /></td>
					<td>
						<asp:textbox id="txtFriendlyName" cssclass="NormalTextBox" width="390" columns="30" maxlength="150"
							runat="server" />
					</td>
				</tr>
				<tr>
					<td class="SubHead" width="150" valign="top"><dnn:label id="plDescription" text="Description:" controlname="txtDescription" runat="server" /></td>
					<td><asp:textbox id="txtDescription" cssclass="NormalTextBox" width="390" columns="30" textmode="MultiLine"
							rows="10" maxlength="2000" runat="server" /></td>
				</tr>
				<tr>
					<td class="SubHead" width="150" valign="top"><dnn:label id="plVersion" text="Version:" controlname="txtVersion" runat="server" /></td>
					<td><asp:textbox id="txtVersion" cssclass="NormalTextBox" width="390" columns="30" maxlength="150"
							runat="server" enabled="False" /></td>
				</tr>
				<tr>
					<td class="SubHead" width="150"><dnn:label id="plBusinessClass" text="Business Controller Class:" controlname="txtBusinessClass"
							runat="server" /></td>
					<td>
						<asp:textbox id="txtBusinessClass" cssclass="NormalTextBox" width="390" columns="30" maxlength="150"
							runat="server" enabled="False" />
					</td>
				</tr>
				<tr>
					<td class="SubHead" width="150"><dnn:label id="plSupportedFeatures" text="Supported Features:" controlname="txtSupportedFeatures"
							runat="server" /></td>
					<td class="SubHead" valign="top">
						<asp:checkbox id="chkPortable" runat="server" cssclass="NormalTextBox" AutoPostBack="False" enabled="False" />&nbsp;
						<dnn:label id="plPortable" text="Portable" controlname="txtPortable" runat="server" />
						<asp:checkbox id="chkSearchable" runat="server" cssclass="NormalTextBox" AutoPostBack="False"
							enabled="False" />&nbsp;
						<dnn:label id="plSearchable" text="Searchable" controlname="txtSearchable" runat="server" />
						<asp:checkbox id="chkUpgradeable" runat="server" cssclass="NormalTextBox" AutoPostBack="False"
							enabled="False" />&nbsp;
						<dnn:label id="plUpgradeable" text="Upgradeable" controlname="txtUpgradeable" runat="server" />
					</td>
				</tr>
				<tr>
					<td class="SubHead" width="150" valign="top"><dnn:label id="plPremium" text="Premium?" controlname="chkPremium" runat="server" /></td>
					<td>
						<asp:checkbox id="chkPremium" runat="server" cssclass="NormalTextBox" AutoPostBack="True" OnCheckedChanged="chkPremium_CheckedChanged"></asp:checkbox><br>
						<portal:duallist id="ctlPortals" runat="server" ListBoxWidth="130" ListBoxHeight="130" DataValueField="PortalID"
							DataTextField="PortalName" />
					</td>
				</tr>
			</table>
			<p>
				<asp:linkbutton id="cmdUpdate" resourcekey="cmdUpdate" text="Update" runat="server" class="CommandButton"
					borderstyle="none" OnClick="cmdUpdate_Click" />
				&nbsp;
				<asp:linkbutton id="cmdCancel" resourcekey="cmdCancel" text="Cancel" causesvalidation="False" runat="server"
					class="CommandButton" borderstyle="none" OnClick="cmdCancel_Click" />
				&nbsp;
				<asp:linkbutton id="cmdDelete" resourcekey="cmdDelete" text="Delete" causesvalidation="False" runat="server"
					class="CommandButton" borderstyle="none" OnClick="cmdDelete_Click" />
			</p>
			<hr>
			<table id="tabDefinitions" runat="server" cellspacing="0" cellpadding="4" border="0" summary="Module Definitions Design Table">
				<tr>
					<td class="SubHead" width="150" valign="top"><dnn:label id="plDefinitions" text="Definitions:" controlname="cboDefinitions" runat="server" /></td>
					<td>
						<asp:dropdownlist id="cboDefinitions" runat="server" width="290px" cssclass="NormalTextBox" datatextfield="FriendlyName"
							datavaluefield="ModuleDefId" autopostback="True" OnSelectedIndexChanged="cboDefinitions_SelectedIndexChanged"></asp:dropdownlist>
						&nbsp;&nbsp;
						<asp:linkbutton id="cmdDeleteDefinition" resourcekey="cmdDeleteDefinition" text="Delete Definition"
							runat="server" class="CommandButton" borderstyle="none" OnClick="cmdDeleteDefinition_Click" />
					</td>
				</tr>
				<tr>
					<td class="SubHead" width="150" valign="top"><dnn:label id="plDefinition" text="New Definition:" controlname="txtDefinition" runat="server" /></td>
					<td>
						<asp:textbox id="txtDefinition" cssclass="NormalTextBox" width="290px" columns="30" maxlength="128"
							runat="server" />
						&nbsp;&nbsp;
						<asp:linkbutton id="cmdAddDefinition" resourcekey="cmdAddDefinition" text="Add Definition" runat="server"
							class="CommandButton" borderstyle="none" OnClick="cmdAddDefinition_Click" />
					</td>
				</tr>
			</table>
			<hr>
			<table id="tabCache" runat="server" cellspacing="0" cellpadding="4" border="0" width="100%"
				summary="Module Definitions Design Table">
				<tr>
					<td class="SubHead" width="150" valign="top"><dnn:label id="plCacheTime" text="Default Cache Time:" controlname="txtCacheTime" runat="server" /></td>
					<td>
						<asp:textbox id="txtCacheTime" cssclass="NormalTextBox" width="290px" columns="30" maxlength="128"
							runat="server" />
						&nbsp;&nbsp;
						<asp:linkbutton id="cmdUpdateCacheTime" resourcekey="cmdUpdateCacheTime" text="Update Cache Time"
							runat="server" class="CommandButton" borderstyle="none" OnClick="cmdUpdateCacheTime_Click" />
					</td>
				</tr>
			</table>
			<hr>
			<table id="tabControls" runat="server" cellspacing="0" cellpadding="4" border="0" width="100%"
				summary="Module Definitions Design Table">
				<tr>
					<td colspan="2">
						<asp:datagrid id="grdControls" runat="server" width="100%" border="0" cellspacing="3" autogeneratecolumns="false"
							enableviewstate="true" summary="Module Controls Design Table" GridLines="None" BorderWidth="0px">
							<Columns>
								<asp:TemplateColumn>
									<ItemStyle Width="20px"></ItemStyle>
									<ItemTemplate>
										<asp:HyperLink id=Hyperlink1 runat="server" NavigateUrl='<%# FormatURL("modulecontrolid",DataBinder.Eval(Container.DataItem,"ModuleControlId").ToString()) %>'>
											<asp:image imageurl="~/images/edit.gif" alternatetext="Edit" runat="server" id="Hyperlink1Image" />
										</asp:HyperLink>
									</ItemTemplate>
								</asp:TemplateColumn>
								<asp:BoundColumn DataField="ControlKey" HeaderText="Control">
									<HeaderStyle CssClass="NormalBold"></HeaderStyle>
									<ItemStyle CssClass="Normal"></ItemStyle>
								</asp:BoundColumn>
								<asp:BoundColumn DataField="ControlTitle" HeaderText="Title">
									<HeaderStyle CssClass="NormalBold"></HeaderStyle>
									<ItemStyle CssClass="Normal"></ItemStyle>
								</asp:BoundColumn>
								<asp:BoundColumn DataField="ControlSrc" HeaderText="Source">
									<HeaderStyle CssClass="NormalBold"></HeaderStyle>
									<ItemStyle CssClass="Normal"></ItemStyle>
								</asp:BoundColumn>
							</Columns>
						</asp:datagrid>
						<p>
							<asp:linkbutton id="cmdAddControl" resourcekey="cmdAddControl" text="Add Control" runat="server"
								class="CommandButton" borderstyle="none" OnClick="cmdAddControl_Click" />
						</p>
					</td>
				</tr>
			</table>
		</td>
	</tr>
</table>
