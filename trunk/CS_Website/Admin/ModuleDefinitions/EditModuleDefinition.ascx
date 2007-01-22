<%@ Register TagPrefix="Portal" TagName="DualList" Src="~/controls/DualListControl.ascx" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Control Language="C#" CodeFile="EditModuleDefinition.ascx.cs" AutoEventWireup="true" Inherits="DotNetNuke.Modules.Admin.ModuleDefinitions.EditModuleDefinition" %>
<table cellspacing="0" cellpadding="4" border="0" summary="Module Definitions Design Table">
	<tr>
		<td>
			<table id="tabManifest" runat="server" border="0" cellpadding="4" cellspacing="0" summary="Manifest Design Table">
				<tr>
					<td class="SubHead" style="width: 150;"><dnn:label id="plManifest" text="Manifest:" controlname="cboManifest" runat="server" /></td>
					<td>
						<asp:dropdownlist id="cboManifest" runat="server" width="390" cssclass="NormalTextBox"></asp:dropdownlist>
						<asp:LinkButton ID="cmdInstallManifest" runat="server" BorderStyle="none" CausesValidation="False" class="CommandButton" OnClick="cmdInstallManifest_Click" resourcekey="cmdInstall"
                            Text="Install">
                        </asp:LinkButton>
					</td>
				</tr>
				<tr>
					<td colspan="2"><hr /></td>
				</tr>
            </table>
            <table id="tabModule" runat="server" border="0" cellpadding="4" cellspacing="0" summary="Module Definitions Design Table">
                <tr>
					<td class="SubHead" style="width: 150;"><dnn:label id="plModuleName" text="Module Name:" controlname="txtModuleName" runat="server" /></td>
					<td>
						<asp:textbox id="txtModuleName" cssclass="NormalTextBox" width="390" columns="30" maxlength="150"
							runat="server" enabled="False" />
						<asp:requiredfieldvalidator id="valModuleName" display="Dynamic" resourcekey="valModuleName.ErrorMessage" errormessage="<br>You must enter a Name for the Module."
							controltovalidate="txtModuleName" runat="server" />
					</td>
				</tr>
				<tr>
					<td class="SubHead" style="width: 150;"><dnn:label id="plFolderName" text="Folder Name:" controlname="txtFolderName" runat="server" /></td>
					<td>
						<asp:textbox id="txtFolderName" cssclass="NormalTextBox" width="390" columns="30" maxlength="150"
							runat="server" enabled="False" />
						<asp:requiredfieldvalidator id="valFolderName" display="Dynamic" resourcekey="valFolderName.ErrorMessage" errormessage="<br>You must enter a Folder Name for the location of the Module's files."
							controltovalidate="txtFolderName" runat="server" Width="335px" />
					</td>
				</tr>
				<tr>
					<td class="SubHead" style="width: 150;"><dnn:label id="plFriendlyName" text="Friendly Name:" controlname="txtFriendlyName" runat="server" /></td>
					<td>
						<asp:textbox id="txtFriendlyName" cssclass="NormalTextBox" width="390" columns="30" maxlength="150"
							runat="server" />
                        <asp:RequiredFieldValidator ID="valFriendlyName" runat="server" ControlToValidate="txtFriendlyName" Display="Dynamic" ErrorMessage="<br>A Module must have a Friendly Name" resourcekey="valFriendlyName.ErrorMessage">
                        </asp:RequiredFieldValidator>
                    </td>
				</tr>
				<tr>
					<td class="SubHead" style="width: 150;" valign="top"><dnn:label id="plDescription" text="Description:" controlname="txtDescription" runat="server" /></td>
					<td><asp:textbox id="txtDescription" cssclass="NormalTextBox" width="390" columns="30" textmode="MultiLine"
							rows="10" maxlength="2000" runat="server" /></td>
				</tr>
				<tr>
					<td class="SubHead" style="width: 150;" valign="top"><dnn:label id="plVersion" text="Version:" controlname="txtVersion" runat="server" /></td>
                    <td>
                        <asp:TextBox ID="txtVersion" runat="server" Columns="30" CssClass="NormalTextBox" Enabled="False" MaxLength="150" Width="390">
                        </asp:TextBox>
                        <asp:RequiredFieldValidator ID="valVersion1" runat="server" ControlToValidate="txtVersion" Display="Dynamic" ErrorMessage="<br>A Module must have a Version number in the form of ##.##.##" resourcekey="valVersion.ErrorMessage">
                        </asp:RequiredFieldValidator>
                        <asp:RegularExpressionValidator ID="valVersion2" runat="server" ControlToValidate="txtVersion" Display="dynamic" ErrorMessage="<br>A Module must have a Version number in the form of ##.##.##" resourceKey="valVersion.ErrorMesage" ValidationExpression="[0-9]{1}[0-9]{1}.[0-9]{1}[0-9]{1}.[0-9]{1}[0-9]{1}">
                        </asp:RegularExpressionValidator>
                    </td>
				</tr>
                <tr>
                    <td class="SubHead" valign="top" style="width:150;">
                        <dnn:Label ID="plCompatibleVersions" runat="server" ControlName="txtCompatibleVersions" Text="Compatibility:" />
                    </td>
                    <td>
                        <asp:TextBox ID="txtCompatibleVersions" runat="server" Columns="30" CssClass="NormalTextBox" Enabled="False" MaxLength="500" Rows="2" TextMode="MultiLine" Width="390">
                        </asp:TextBox></td>
                </tr>
				<tr>
					<td class="SubHead" style="width: 150;"><dnn:label id="plBusinessClass" text="Business Controller Class:" controlname="txtBusinessClass"
							runat="server" /></td>
					<td>
						<asp:textbox id="txtBusinessClass" cssclass="NormalTextBox" width="390" columns="30" maxlength="150"
							runat="server" enabled="False" />
					</td>
				</tr>
				<tr>
					<td class="SubHead" style="width: 150;"><dnn:label id="plSupportedFeatures" text="Supported Features:" controlname="txtSupportedFeatures"
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
					<td class="SubHead" style="width: 150;" valign="top"><dnn:label id="plPremium" text="Premium?" controlname="chkPremium" runat="server" /></td>
					<td>
						<asp:checkbox id="chkPremium" runat="server" cssclass="NormalTextBox" AutoPostBack="True" OnCheckedChanged="chkPremium_CheckedChanged"></asp:checkbox><br>
						<portal:duallist id="ctlPortals" runat="server" ListBoxWidth="130" ListBoxHeight="130" DataValueField="PortalID"
							DataTextField="PortalName" />
					</td>
				</tr>
			</table>
			<p>
				<asp:linkbutton id="cmdUpdate" resourcekey="cmdUpdate" text="Update" runat="server" cssclass="CommandButton"
					borderstyle="none" OnClick="cmdUpdate_Click" />
				&nbsp;
				<asp:linkbutton id="cmdCancel" resourcekey="cmdCancel" text="Cancel" causesvalidation="False" runat="server"
					cssclass="CommandButton" borderstyle="none" OnClick="cmdCancel_Click" />
				&nbsp;
				<asp:linkbutton id="cmdDelete" resourcekey="cmdDelete" text="Delete" causesvalidation="False" runat="server"
					cssclass="CommandButton" borderstyle="none" OnClick="cmdDelete_Click" />
				&nbsp;
                <asp:CheckBox ID="chkDelete" runat="server" CssClass="SubHead" resourcekey="chkDelete" Text="Delete Files?" TextAlign="Right" />
			</p>
			<hr/>
			<table id="tabDefinitions" runat="server" cellspacing="0" cellpadding="4" border="0" summary="Module Definitions Design Table">
				<tr>
					<td class="SubHead" style="width: 150;" valign="top"><dnn:label id="plDefinitions" text="Definitions:" controlname="cboDefinitions" runat="server" /></td>
					<td>
						<asp:dropdownlist id="cboDefinitions" runat="server" width="290px" cssclass="NormalTextBox" datatextfield="FriendlyName"
							datavaluefield="ModuleDefId" autopostback="True" OnSelectedIndexChanged="cboDefinitions_SelectedIndexChanged"></asp:dropdownlist>
						&nbsp;&nbsp;
						<asp:linkbutton id="cmdDeleteDefinition" resourcekey="cmdDeleteDefinition" text="Delete Definition"
							runat="server" cssclass="CommandButton" borderstyle="none" OnClick="cmdDeleteDefinition_Click" />
					</td>
				</tr>
				<tr>
					<td class="SubHead" style="width: 150;" valign="top"><dnn:label id="plDefinition" text="New Definition:" controlname="txtDefinition" runat="server" /></td>
					<td>
						<asp:textbox id="txtDefinition" cssclass="NormalTextBox" width="290px" columns="30" maxlength="128"
							runat="server" />
						&nbsp;&nbsp;
						<asp:linkbutton id="cmdAddDefinition" resourcekey="cmdAddDefinition" text="Add Definition" runat="server"
							cssclass="CommandButton" borderstyle="none" OnClick="cmdAddDefinition_Click" />
					</td>
				</tr>
			</table>
			<hr/>
			<table id="tabCache" runat="server" cellspacing="0" cellpadding="4" border="0" width="100%"
				summary="Module Definitions Design Table">
				<tr>
					<td class="SubHead" style="width: 150;" valign="top"><dnn:label id="plCacheTime" text="Default Cache Time:" controlname="txtCacheTime" runat="server" /></td>
					<td>
						<asp:textbox id="txtCacheTime" cssclass="NormalTextBox" width="290px" columns="30" maxlength="128"
							runat="server" />
						&nbsp;&nbsp;
						<asp:linkbutton id="cmdUpdateCacheTime" resourcekey="cmdUpdateCacheTime" text="Update Cache Time"
							runat="server" cssclass="CommandButton" borderstyle="none" OnClick="cmdUpdateCacheTime_Click" />
					</td>
				</tr>
			</table>
			<hr/>
			<table id="tabControls" runat="server" cellspacing="0" cellpadding="4" border="0" width="100%"
				summary="Module Definitions Design Table">
				<tr>
					<td colspan="2">
						<asp:datagrid id="grdControls" runat="server" width="100%" cellspacing="3" autogeneratecolumns="false"
							enableviewstate="true" summary="Module Controls Design Table" GridLines="None" BorderWidth="0px">
							<Columns>
								<asp:TemplateColumn>
									<ItemStyle Width="20px"></ItemStyle>
									<ItemTemplate>
										<asp:HyperLink id="Hyperlink1" runat="server" NavigateUrl='<%# FormatURL("modulecontrolid",DataBinder.Eval(Container.DataItem,"ModuleControlId").ToString()) %>'>
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
								cssclass="CommandButton" borderstyle="none" OnClick="cmdAddControl_Click" />
						</p>
					</td>
				</tr>
			</table>
		</td>
	</tr>
</table>
