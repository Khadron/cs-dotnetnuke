<%@ Control Inherits="DotNetNuke.Modules.Admin.PortalManagement.SiteWizard" Language="C#" AutoEventWireup="false" Explicit="True" enableViewState="True" debug="False" CodeFile="SiteWizard.ascx.cs" %>
<%@ Register TagPrefix="dnn" TagName="Skin" Src="~/controls/SkinThumbNailControl.ascx" %>
<%@ Register TagPrefix="dnn" TagName="url" Src="~/controls/UrlControl.ascx" %>
<%@ Register TagPrefix="dnn" TagName="label" Src="~/controls/LabelControl.ascx" %>
<table id="Wizard" cellspacing="0" cellpadding="0" border="0" runat="server">
	<tr>
		<td id="WizardBody" runat="server" class="WizardBody" valign="top">
			<asp:panel id="pnlTemplate" runat="server">
				<table cellspacing="0" cellpadding="0" border="0">
					<tr>
						<td colspan="3" class="SubHead"><dnn:label id="lblTemplateTitle" runat="server" text="Choose a Template" /></td>
					</tr>
					<tr>
						<td colspan="3" height="15"></td>
					</tr>
					<tr>
						<td colspan="3"><asp:checkbox id="chkTemplate" runat="server" cssclass="WizardText" autopostback="True" resourcekey="TemplateDetail"
								Text="Build your site from a template (below)"></asp:checkbox></td>
					</tr>
					<tr>
						<td colspan="3" height="10"></td>
					</tr>
					<tr>
						<td width="150" aliign="center"><asp:listbox id="lstTemplate" runat="server" width="150" rows="8" autopostback="True"></asp:listbox></td>
						<td colspan="2" valign="top" align="left" width="300"><asp:label id="lblTemplateMessage" runat="server" cssclass="NormalRed" style="overflow:auto; width:280px; height:150px"></asp:label></td>
					</tr>
					<tr>
						<td colspan="3" height="15"></td>
					</tr>
					<tr>
						<td colspan="3">
							<asp:label id="lblMergeTitle" runat="server" resourcekey="MergeDetail" cssclass="WizardText">
								<p>
									If you elect to build your site using a template, you need to choose how to 
									deal with duplicate Modules (Modules that are in the Template and also already 
									on your site).
								</p>
							</asp:label>
						</td>
					</tr>
					<tr>
						<td colspan="3" height="15"></td>
					</tr>
					<tr>
						<td colspan="3" align="center">
							<asp:radiobuttonlist id="optMerge" cssclass="WizardText" runat="server" repeatdirection="Horizontal">
								<asp:listitem selected value="Ignore" resourcekey="Ignore">Ignore</asp:listitem>
								<asp:listitem value="Replace" resourcekey="Replace">Replace</asp:listitem>
								<asp:listitem value="Merge" resourcekey="Merge">Merge</asp:listitem>
							</asp:radiobuttonlist>
						</td>
					</tr>
					<tr>
						<td colspan="3">
							<asp:label id="lblMergeWarning" runat="server" resourcekey="MergeWarning" cssclass="WizardText">
								<p class="NormalRed">
									Note: If you choose "Replace", all existing content on pages that are also in 
									the template will be lost.
								</p>
							</asp:label>
						</td>
					</tr>
				</table>
			</asp:panel>
			<asp:panel id="pnlSkin" runat="server">
				<table cellspacing="0" cellpadding="0" border="0">
					<tr>
						<td class="SubHead"><dnn:label id="lblSkinTitle" runat="server" text="Select a Skin:" /></td>
					</tr>
					<tr>
						<td height="15"></td>
					</tr>
					<tr>
						<td>
							<asp:label id="lblSkinDetail" runat="server" resourcekey="SkinDetail" cssclass="WizardText">Select a Skin from the Skin Viewer (below)</asp:label>
						</td>
					</tr>
					<tr>
						<td height="15"></td>
					</tr>
					<tr>
						<td align="center"><dnn:skin id="ctlPortalSkin" runat="server"></dnn:skin></td>
					</tr>
				</table>
			</asp:panel>
			<asp:panel id="pnlContainer" runat="server">
				<table cellspacing="0" cellpadding="0" border="0">
					<tr>
						<td colspan="2" class="SubHead"><dnn:label id="lblContainerTitle" runat="server" text="Select a Container:" /></td>
					</tr>
					<tr>
						<td height="15"></td>
					</tr>
					<tr>
						<td><asp:label id="lblContainerDetail" runat="server" resourcekey="ContainerDetail" cssclass="WizardText">Select a Container from Viewer:</asp:label></td>
						<td align="center"><asp:CheckBox ID="chkIncludeAll" CssClass="WizardText" Runat="server" resourcekey="IncludeAll" TextAlign="Left" Text="Show All Containers:" AutoPostBack="True"></asp:CheckBox></td>
					</tr>
					<tr>
						<td height="15"></td>
					</tr>
					<tr>
						<td align="center"colspan="2" ><dnn:skin id="ctlPortalContainer" runat="server"></dnn:skin></td>
					</tr>
				</table>
			</asp:panel>
			<asp:panel id="pnlDetails" runat="server">
				<table cellspacing="0" cellpadding="0" border="0">
					<tr>
						<td colspan="2" class="SubHead"><dnn:label id="lblDetailsTitle" runat="server" text="Site Details" /></td>
					</tr>
					<tr>
						<td colspan="2" height="15"></td>
					</tr>
					<tr>
						<td colspan="2"><asp:label id="lblDetailsDetail" runat="server" resourcekey="DetailsDetail" cssclass="WizardText">Provide some basic information about your site.</asp:label></td>
					</tr>
					<tr>
						<td colspan="2" height="15"></td>
					</tr>
					<tr>
						<td class="SubHead" width="150"><dnn:label id="lblPortalName" runat="server" text="Name/Title:" controlname="txtPortalName" /></td>
						<td class="NormalTextBox" valign="top" align="left"><asp:textbox id="txtPortalName" cssclass="NormalTextBox" runat="server" width="300" maxlength="128"></asp:textbox></td>
					</tr>
					<tr>
						<td colspan="2" height="15"></td>
					</tr>
					<tr>
						<td class="SubHead" valign="top" width="150"><dnn:label id="lblDescription" runat="server" text="Description:" /></td>
						<td class="NormalTextBox" align="left"><asp:textbox id="txtDescription" cssclass="NormalTextBox" runat="server" width="300" maxlength="475" rows="3" textmode="MultiLine"></asp:textbox></td>
					</tr>
					<tr>
						<td colspan="2" height="15"></td>
					</tr>
					<tr>
						<td class="SubHead" valign="top" width="150"><dnn:label id="lblKeyWords" runat="server" text="Key Words:" /></td>
						<td class="NormalTextBox" align="left"><asp:textbox id="txtKeyWords" cssclass="NormalTextBox" runat="server" width="300" maxlength="475" rows="3" textmode="MultiLine"></asp:textbox></td>
					</tr>
				</table>
			</asp:panel>
			<asp:panel id="pnlLogo" runat="server">
				<table cellspacing="0" cellpadding="0" border="0">
					<tr>
						<td colspan="2" class="SubHead"><dnn:label id="lblLogoTitle" runat="server" text="Choose a Logo:" /></td>
					</tr>
					<tr>
						<td height="15"></td>
					</tr>
					<tr>
						<td class="SubHead" valign="top" width="120"><dnn:label id="lblLogo" runat="server" text="Logo:" /></td>
						<td class="NormalTextBox" align="left">
							<dnn:url id="urlLogo" runat="server" showLog="False" showTabs="False" showUrls="False" showTrack="false" required="false"/>
						</td>
					</tr>
				</table>
			</asp:panel>
		</td>
	</tr>
</table>
