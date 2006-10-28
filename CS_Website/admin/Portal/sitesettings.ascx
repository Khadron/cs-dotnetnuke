<%@ Register TagPrefix="Portal" TagName="DualList" Src="~/controls/DualListControl.ascx" %>
<%@ Register TagPrefix="Portal" TagName="URL" Src="~/controls/URLControl.ascx" %>
<%@ Register TagPrefix="dnn" TagName="Skin" Src="~/controls/SkinControl.ascx" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register TagPrefix="dnn" TagName="SectionHead" Src="~/controls/SectionHeadControl.ascx" %>
<%@ Control Inherits="DotNetNuke.Modules.Admin.PortalManagement.SiteSettings" Language="C#" AutoEventWireup="false" Explicit="True" enableViewState="True" debug="False" CodeFile="SiteSettings.ascx.cs" %>
<!-- Settings Tables -->
<table class="Settings" cellspacing="2" cellpadding="2" width="560" summary="Site Settings Design Table"
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
								enableviewstate="False">In this section, you can set up the basic settings for your site.</asp:label></TD>
					</TR>
					<TR>
						<TD width="25"></TD>
						<TD vAlign="top" width="475">
							<dnn:sectionhead id="dshSite" cssclass="Head" runat="server" text="Site Details" section="tblSite"
								resourcekey="SiteDetails"></dnn:sectionhead>
							<TABLE id="tblSite" cellSpacing="2" cellPadding="2" summary="Site Details Design Table"
								border="0" runat="server">
								<TR>
									<TD class="SubHead" width="150">
										<dnn:label id="plPortalName" runat="server" text="Title:" controlname="txtPortalName"></dnn:label></TD>
									<TD class="NormalTextBox" vAlign="top" width="325">
										<asp:textbox id="txtPortalName" cssclass="NormalTextBox" runat="server" width="325" maxlength="128"></asp:textbox></TD>
								</TR>
								<TR>
									<TD class="SubHead" vAlign="top" width="150">
										<dnn:label id="plDescription" runat="server" text="Description:" controlname="txtDescription"></dnn:label></TD>
									<TD class="NormalTextBox" width="325">
										<asp:textbox id="txtDescription" cssclass="NormalTextBox" runat="server" width="325" maxlength="475"
											rows="3" textmode="MultiLine"></asp:textbox></TD>
								</TR>
								<TR>
									<TD class="SubHead" vAlign="top" width="150">
										<dnn:label id="plKeyWords" runat="server" text="Key Words:" controlname="txtKeyWords"></dnn:label></TD>
									<TD class="NormalTextBox" width="325">
										<asp:textbox id="txtKeyWords" cssclass="NormalTextBox" runat="server" width="325" maxlength="475"
											rows="3" textmode="MultiLine"></asp:textbox></TD>
								</TR>
							</TABLE>
							<BR>
							<dnn:sectionhead id="dshAppearance" cssclass="Head" runat="server" text="Appearance" section="tblAppearance"
								resourcekey="Appearance"></dnn:sectionhead>
							<TABLE id="tblAppearance" cellSpacing="2" cellPadding="2" summary="Appearance Design Table"
								border="0" runat="server">
								<TR>
									<TD class="SubHead" vAlign="top" width="150">
										<dnn:label id="plLogo" runat="server" controlname="ctlLogo" suffix=""></dnn:label></TD>
									<TD width="325">
										<portal:url id="ctlLogo" runat="server" width="325" ShowUrls="False" ShowTabs="False" ShowLog="False"
											ShowTrack="False" Required="False"></portal:url></TD>
								</TR>
								<TR>
									<TD class="SubHead" vAlign="top" width="150">
										<dnn:label id="plBackground" runat="server" text="Body Background:" controlname="cboBackground"></dnn:label></TD>
									<TD width="325">
										<portal:url id="ctlBackground" runat="server" width="325" ShowUrls="False" ShowTabs="False"
											ShowLog="False" ShowTrack="False" Required="False"></portal:url></TD>
								</TR>
								<TR>
									<TD class="SubHead" width="150">
										<dnn:label id="plPortalSkin" runat="server" text="Portal Skin:" controlname="ctlPortalSkin"></dnn:label></TD>
									<TD vAlign="top" width="325">
										<dnn:skin id="ctlPortalSkin" runat="server"></dnn:skin></TD>
								</TR>
								<TR>
									<TD class="SubHead" width="150">
										<dnn:label id="plPortalContainer" runat="server" text="Portal Container:" controlname="ctlPortalContainer"></dnn:label></TD>
									<TD vAlign="top" width="325">
										<dnn:skin id="ctlPortalContainer" runat="server"></dnn:skin></TD>
								</TR>
								<TR>
									<TD class="SubHead" width="150">
										<dnn:label id="plAdminSkin" runat="server" text="Admin Skin:" controlname="ctlAdminSkin"></dnn:label></TD>
									<TD vAlign="top" width="325">
										<dnn:skin id="ctlAdminSkin" runat="server"></dnn:skin></TD>
								</TR>
								<TR>
									<TD class="SubHead" width="150">
										<dnn:label id="plAdminContainer" runat="server" text="Admin Container:" controlname="ctlAdminContainer"></dnn:label></TD>
									<TD vAlign="top" width="325">
										<dnn:skin id="ctlAdminContainer" runat="server"></dnn:skin></TD>
								</TR>
								<TR>
									<TD align="center" colSpan="2"><BR>
										<asp:HyperLink id="lnkUploadSkin" runat="server" resourcekey="SkinUpload" CssClass="CommandButton">Upload 
                  Skin</asp:HyperLink>&nbsp;&nbsp;
										<asp:HyperLink id="lnkUploadContainer" runat="server" resourcekey="ContainerUpload" CssClass="CommandButton">Upload 
                  Container</asp:HyperLink></TD>
								</TR>
							</TABLE>
						</TD>
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
								enableviewstate="False">In this section, you can set up more advanced settings for your site.</asp:label></TD>
					</TR>
					<TR>
						<TD width="25"></TD>
						<TD vAlign="top" width="475">
							<dnn:sectionhead id="dhsSecurity" cssclass="Head" runat="server" text="Security Settings" section="tblSecurity"
								resourcekey="SecuritySettings"></dnn:sectionhead>
							<TABLE id="tblSecurity" cellSpacing="2" cellPadding="2" summary="Security Settings Design Table"
								border="0" runat="server">
								<TR>
									<TD class="SubHead" width="150">
										<dnn:label id="plUserRegistration" runat="server" text="User Registration:" controlname="optUserRegistration"></dnn:label></TD>
									<TD class="NormalTextBox" vAlign="top" width="325">
										<asp:radiobuttonlist id="optUserRegistration" cssclass="Normal" runat="server" enableviewstate="False"
											repeatdirection="Horizontal">
											<asp:listitem value="0" resourcekey="None">None</asp:listitem>
											<asp:listitem value="1" resourcekey="Private">Private</asp:listitem>
											<asp:listitem value="2" resourcekey="Public">Public</asp:listitem>
											<asp:listitem value="3" resourcekey="Verified">Verified</asp:listitem>
										</asp:radiobuttonlist></TD>
								</TR>
							</TABLE>
							<BR>
							<dnn:sectionhead id="dshPages" cssclass="Head" runat="server" text="Page Management" section="tblPages"
								resourcekey="Pages"></dnn:sectionhead>
							<TABLE id="tblPages" cellSpacing="2" cellPadding="2" summary="Page Management Design Table"
								border="0" runat="server">
								<TR>
									<TD class="SubHead" width="150">
										<dnn:label id="plSplashTabId" runat="server" text="Splash Page:" controlname="cboSplashTabId"></dnn:label></TD>
									<TD class="NormalTextBox" vAlign="top" width="325">
										<asp:dropdownlist id="cboSplashTabId" cssclass="NormalTextBox" runat="server" width="325" datatextfield="TabName"
											datavaluefield="TabId"></asp:dropdownlist></TD>
								</TR>
								<TR>
									<TD class="SubHead" width="150">
										<dnn:label id="plHomeTabId" runat="server" text="Home Page:" controlname="cboHomeTabId"></dnn:label></TD>
									<TD class="NormalTextBox" vAlign="top" width="325">
										<asp:dropdownlist id="cboHomeTabId" cssclass="NormalTextBox" runat="server" width="325" datatextfield="TabName"
											datavaluefield="TabId"></asp:dropdownlist></TD>
								</TR>
								<TR>
									<TD class="SubHead" width="150">
										<dnn:label id="plLoginTabId" runat="server" text="Login Page:" controlname="cboLoginTabId"></dnn:label></TD>
									<TD class="NormalTextBox" vAlign="top" width="325">
										<asp:dropdownlist id="cboLoginTabId" cssclass="NormalTextBox" runat="server" width="325" datatextfield="TabName"
											datavaluefield="TabId"></asp:dropdownlist></TD>
								</TR>
								<TR>
									<TD class="SubHead" width="150">
										<dnn:label id="plUserTabId" runat="server" text="User Page:" controlname="cboUserTabId"></dnn:label></TD>
									<TD class="NormalTextBox" vAlign="top" width="325">
										<asp:dropdownlist id="cboUserTabId" cssclass="NormalTextBox" runat="server" width="325" datatextfield="TabName"
											datavaluefield="TabId"></asp:dropdownlist></TD>
								</TR>
								<TR>
									<TD class="SubHead" vAlign="top" width="150">
										<dnn:label id="plHomeDirectory" runat="server" text="Home Directory:" controlname="txtHomeDirectory"></dnn:label></TD>
									<TD class="NormalTextBox" width="325">
										<asp:textbox id="txtHomeDirectory" cssclass="NormalTextBox" runat="server" width="325" maxlength="100"
											enabled="False"></asp:textbox></TD>
								</TR>
							</TABLE>
							<BR>
							<dnn:sectionhead id="dshPayment" cssclass="Head" runat="server" text="Payment Settings" section="tblPayment"
								resourcekey="Payment" isexpanded="False"></dnn:sectionhead>
							<TABLE id="tblPayment" cellSpacing="2" cellPadding="2" summary="Payment Setttings Design Table"
								border="0" runat="server">
								<TR>
									<TD class="SubHead" width="150">
										<dnn:label id="plCurrency" runat="server" text="Currency:" controlname="cboCurrency"></dnn:label></TD>
									<TD class="NormalTextBox" vAlign="top" width="325">
										<asp:dropdownlist id="cboCurrency" cssclass="NormalTextBox" runat="server" width="325" datatextfield="text"
											datavaluefield="value"></asp:dropdownlist></TD>
								</TR>
								<TR>
									<TD class="SubHead" vAlign="top" width="150">
										<dnn:label id="plProcessor" runat="server" text="Payment Processor:" controlname="cboProcessor"></dnn:label></TD>
									<TD class="NormalTextBox" vAlign="top" width="325">
										<asp:dropdownlist id="cboProcessor" cssclass="NormalTextBox" runat="server" width="325" datatextfield="value"
											datavaluefield="text"></asp:dropdownlist><BR>
										<asp:linkbutton id="cmdProcessor" cssclass="CommandButton" runat="server" resourcekey="ProcessorWebSite"
											enableviewstate="False">Go To Payment Processor Website</asp:linkbutton></TD>
								</TR>
								<TR>
									<TD class="SubHead" width="150">
										<dnn:label id="plUserId" runat="server" text="Processor UserId:" controlname="txtUserId"></dnn:label></TD>
									<TD class="NormalTextBox" vAlign="top" width="325">
										<asp:textbox id="txtUserId" cssclass="NormalTextBox" runat="server" width="325" maxlength="50"></asp:textbox></TD>
								</TR>
								<TR>
									<TD class="SubHead" width="150">
										<dnn:label id="plPassword" runat="server" text="Processor Password:" controlname="txtPassword"></dnn:label></TD>
									<TD class="NormalTextBox" vAlign="top" width="325">
										<asp:textbox id="txtPassword" cssclass="NormalTextBox" runat="server" width="325" maxlength="50" textmode="Password"></asp:textbox></TD>
								</TR>
							</TABLE>
							<BR>
							<dnn:sectionhead id="dsOther" cssclass="Head" runat="server" text="Other Settings" section="tblOther"
								resourcekey="Other" isexpanded="False"></dnn:sectionhead>
							<TABLE id="tblOther" cellSpacing="2" cellPadding="2" summary="Other Setttings Design Table"
								border="0" runat="server">
								<TR>
									<TD class="SubHead" width="150">
										<dnn:label id="plFooterText" runat="server" text="Copyright:" controlname="txtFooterText"></dnn:label></TD>
									<TD class="NormalTextBox" width="325">
										<asp:textbox id="txtFooterText" cssclass="NormalTextBox" runat="server" width="300" maxlength="100"></asp:textbox></TD>
								</TR>
								<TR>
									<TD class="SubHead" width="150">
										<dnn:label id="plBannerAdvertising" runat="server" text="Banner Advertising:" controlname="optBannerAdvertising"></dnn:label></TD>
									<TD class="NormalTextBox" width="325">
										<asp:radiobuttonlist id="optBannerAdvertising" cssclass="Normal" runat="server" enableviewstate="False"
											repeatdirection="Horizontal">
											<asp:listitem value="0" resourcekey="None">None</asp:listitem>
											<asp:listitem value="1" resourcekey="Site">Site</asp:listitem>
											<asp:listitem value="2" resourcekey="Host">Host</asp:listitem>
										</asp:radiobuttonlist></TD>
								</TR>
								<TR>
									<TD class="SubHead" width="150">
										<dnn:label id="plAdministrator" runat="server" text="Administrator:" controlname="cboAdministratorId"></dnn:label></TD>
									<TD class="NormalTextBox" width="325">
										<asp:dropdownlist id="cboAdministratorId" cssclass="NormalTextBox" runat="server" width="300" datatextfield="FullName"
											datavaluefield="UserId"></asp:dropdownlist></TD>
								</TR>
								<TR>
									<TD class="SubHead" width="150">
										<dnn:label id="plDefaultLanguage" runat="server" text="Default Language:" controlname="cboDefaultLanguage"></dnn:label></TD>
									<TD width="325">
										<asp:dropdownlist id="cboDefaultLanguage" cssclass="NormalTextBox" runat="server" width="300"></asp:dropdownlist></TD>
								</TR>
								<TR>
									<TD class="SubHead" width="150">
										<dnn:label id="plTimeZone" runat="server" text="Portal TimeZone:" controlname="cboTimeZone"></dnn:label></TD>
									<TD width="325">
										<asp:dropdownlist id="cboTimeZone" cssclass="NormalTextBox" runat="server" width="300"></asp:dropdownlist></TD>
								</TR>
							</TABLE>
							<BR>
							<dnn:sectionhead id="dshHost" cssclass="Head" runat="server" text="Host Settings" section="tblHost"
								resourcekey="HostSettings" isexpanded="False"></dnn:sectionhead>
							<TABLE id="tblHost" cellSpacing="2" cellPadding="2" summary="Host Settings Design Table"
								border="0" runat="server">
								<TR>
									<TD class="SubHead" width="150">
										<dnn:label id="plExpiryDate" runat="server" text="Expiry Date:" controlname="txtExpiryDate"></dnn:label></TD>
									<TD class="NormalTextBox" width="325">
										<asp:textbox id="txtExpiryDate" cssclass="NormalTextBox" runat="server" width="150" maxlength="15"></asp:textbox>
										<asp:hyperlink id="cmdExpiryCalendar" cssclass="CommandButton" runat="server" resourcekey="Calendar">Calendar</asp:hyperlink>
										<asp:comparevalidator id="valExpiryDate" cssclass="NormalRed" runat="server" controltovalidate="txtExpiryDate"
											errormessage="<br>Invalid expiry date!" operator="DataTypeCheck" type="Date" display="Dynamic"></asp:comparevalidator></TD>
								</TR>
								<TR>
									<TD class="SubHead" width="150">
										<dnn:label id="plHostFee" runat="server" text="Hosting Fee:" controlname="txtHostFee"></dnn:label></TD>
									<TD class="NormalTextBox" width="325">
										<asp:textbox id="txtHostFee" cssclass="NormalTextBox" runat="server" width="300" maxlength="10"></asp:textbox></TD>
								</TR>
								<TR>
									<TD class="SubHead" width="150">
										<dnn:label id="plHostSpace" runat="server" text="Disk Space:" controlname="txtHostSpace"></dnn:label></TD>
									<TD class="NormalTextBox" width="325">
										<asp:textbox id="txtHostSpace" cssclass="NormalTextBox" runat="server" maxlength="6" width="300"></asp:textbox></TD>
								</TR>
								<TR>
									<TD class="SubHead" width="150">
										<dnn:label id="plSiteLogHistory" runat="server" text="Site Log History:" controlname="txtSiteLogHistory"></dnn:label></TD>
									<TD class="NormalTextBox" width="325">
										<asp:textbox id="txtSiteLogHistory" cssclass="NormalTextBox" runat="server" width="300" maxlength="3"></asp:textbox></TD>
								</TR>
								<TR>
									<TD class="SubHead" vAlign="top" width="150"><BR>
										<BR>
										<dnn:label id="plDesktopModules" runat="server" text="Premium Modules:" controlname="ctlDesktopModules"></dnn:label></TD>
									<TD class="NormalTextBox" width="325">
										<portal:duallist id="ctlDesktopModules" runat="server" ListBoxWidth="130" ListBoxHeight="130" DataValueField="DesktopModuleID"
											DataTextField="FriendlyName"></portal:duallist></TD>
								</TR>
							</TABLE>
						</TD>
					</TR>
				</TABLE>
				<BR>
				<dnn:sectionhead id="dshStylesheet" cssclass="Head" runat="server" text="Stylesheet Editor" section="tblStylesheet"
					resourcekey="StylesheetEditor" includerule="True" isexpanded="False"></dnn:sectionhead>
				<TABLE id="tblStylesheet" cellSpacing="0" cellPadding="2" width="525" summary="Stylesheet Editor Design Table"
					border="0" runat="server">
					<TR>
						<TD>
							<asp:textbox id="txtStyleSheet" cssclass="NormalTextBox" runat="server" rows="20" textmode="MultiLine"
								wrap="False" columns="100"></asp:textbox></TD>
					</TR>
					<TR>
						<TD>
							<asp:linkbutton id="cmdSave" cssclass="CommandButton" runat="server" resourcekey="SaveStyleSheet"
								enableviewstate="False">Save Style Sheet</asp:linkbutton>&nbsp;&nbsp;
							<asp:linkbutton id="cmdRestore" cssclass="CommandButton" runat="server" resourcekey="RestoreDefaultStyleSheet"
								enableviewstate="False">Restore Default Style Sheet</asp:linkbutton></TD>
					</TR>
				</TABLE>
			</asp:panel>
		</td>
	</tr>
</table>
<p>
	<asp:linkbutton cssclass="CommandButton" id="cmdUpdate" resourcekey="cmdUpdate" runat="server" text="Update"></asp:linkbutton>&nbsp;&nbsp;
	<asp:linkbutton cssclass="CommandButton" id="cmdCancel" resourcekey="cmdCancel" runat="server" text="Cancel" causesvalidation="False" borderstyle="none"></asp:linkbutton>&nbsp;&nbsp;
	<asp:linkbutton cssclass="CommandButton" id="cmdDelete" resourcekey="cmdDelete" runat="server" text="Delete" causesvalidation="False" borderstyle="none"></asp:linkbutton>&nbsp;&nbsp;
	<asp:linkbutton cssclass="CommandButton" id="cmdGoogle" resourcekey="cmdGoogle" runat="server" text="Submit Site To Google"></asp:linkbutton>
</p>
