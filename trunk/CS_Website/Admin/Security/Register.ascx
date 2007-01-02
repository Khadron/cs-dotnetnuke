<%@ Control Language="C#" CodeFile="Register.ascx.cs" AutoEventWireup="true" Explicit="True" Inherits="DotNetNuke.Modules.Admin.Security.Register" %>
<%@ Register TagPrefix="dnn" TagName="Address" Src="~/controls/Address.ascx"%>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register TagPrefix="dnn" TagName="Sectionhead" Src="~/controls/SectionHeadControl.ascx" %>
<%@ Register TagPrefix="dnn" TagName="User" Src="~/controls/User.ascx"%>
<asp:panel id="UserRow" runat="server">
	<asp:label id="lblRegistration" runat="server" cssclass="Normal" width="600"></asp:label>
	<table cellSpacing="1" cellPadding="0" width="700" summary="Register Design Table" border="0">
		<tr>
			<td colSpan="3">
				<asp:label id="lblRegister" runat="server" cssclass="Normal"></asp:label></td></tr>
		<tr>
			<td class="SubHead" vAlign="top" width="350">
				<dnn:user id="userControl" runat="server"></dnn:user></td>
			<td>&nbsp;</td>
			<td class="NormalBold" vAlign="top" width="350" rowSpan="8">
				<dnn:address id="addressUser" runat="server"></dnn:address></td></tr></table><br>
	<dnn:sectionhead id="dshPreferences" runat="server" cssclass="Head" section="tblPreferences" includerule="True" resourcekey="Preferences" text="Preferences"></dnn:sectionhead>
	<table id="tblPreferences" cellSpacing="1" cellPadding="0" width="600" summary="Preferences" runat="server">
		<tr>
			<td class="SubHead" width="225">
				<dnn:label id="plLocale" runat="server" text="Preferred Language:" controlname="cboLocale"></dnn:label></td>
			<td class="NormalBold" noWrap>
				<asp:dropdownlist id="cboLocale" tabIndex="18" runat="server" cssclass="NormalTextBox" width="300"></asp:dropdownlist></td></tr>
		<tr>
			<td class="SubHead" width="225">
				<dnn:label id="plTimeZone" runat="server" text="Time Zone:" controlname="cboTimeZone"></dnn:label></td>
			<td class="NormalBold" noWrap>
				<asp:dropdownlist id="cboTimeZone" tabIndex="19" runat="server" cssclass="NormalTextBox" width="300"></asp:dropdownlist></td></tr></table>
</asp:panel>
<br>
<asp:panel id="PasswordManagementRow" runat="server">
	<dnn:sectionhead id="dshPassword" runat="server" cssclass="Head" section="tblPassword" includerule="True" resourcekey="ChangePassword" text="Change Password"></dnn:sectionhead>
	<table id="tblPassword" cellSpacing="0" cellPadding="4" width="600" summary="Password Management" border="0" runat="server">
		<tr vAlign="top" height="*">
			<td id="MessageCell" colSpan="2" runat="server"></td></tr>
		<tr>
			<td class="SubHead" width="175">
				<dnn:label id="plOldPassword" runat="server" text="Old Password:" controlname="txtOldPassword"></dnn:label></td>
			<td class="NormalBold" noWrap>
				<asp:textbox id="txtOldPassword" tabIndex="20" runat="server" cssclass="NormalTextBox" textmode="Password" size="25" maxlength="20"></asp:textbox>&nbsp;*</td></tr>
		<tr>
			<td class="SubHead" width="175">
				<dnn:label id="plNewPassword" runat="server" text="New Password:" controlname="txtNewPassword"></dnn:label></td>
			<td class="NormalBold" noWrap>
				<asp:textbox id="txtNewPassword" tabIndex="21" runat="server" cssclass="NormalTextBox" textmode="Password" size="25" maxlength="20"></asp:textbox>&nbsp;*</td></tr>
		<tr>
			<td class="SubHead" width="175">
				<dnn:label id="plNewConfirm" runat="server" text="Confirm New Password:" controlname="txtNewConfirm"></dnn:label></td>
			<td class="NormalBold" noWrap>
				<asp:textbox id="txtNewConfirm" tabIndex="22" runat="server" cssclass="NormalTextBox" textmode="Password" size="25" maxlength="20"></asp:textbox>&nbsp;*</td></tr>
		<tr>
			<td colSpan="2">
				<asp:linkbutton class="CommandButton" id="cmdUpdatePassword" runat="server" resourcekey="cmdUpdatePassword" text="Update Password" OnClick="cmdUpdatePassword_Click"></asp:linkbutton></td></tr></table>
</asp:panel>
<br>
<asp:panel id="ServicesRow" runat="server">
	<dnn:sectionhead id="dshServices" runat="server" cssclass="Head" section="tblServices" includerule="True" resourcekey="Services" text="Membership Services" isExpanded="False"></dnn:sectionhead>
	<table id="tblServices" cellSpacing="0" cellPadding="4" width="600" summary="Register Design Table" border="0" runat="server">
		<tr>
			<td class=SubHead width=150><dnn:label id=plRSVPCode runat="server" controlname="txtRSVPCode"></dnn:label></td>
			<td width=325>
				<asp:textbox id=txtRSVPCode cssclass="NormalTextBox" runat="server" maxlength="50" columns="30" width="100"></asp:textbox>
          		<asp:linkbutton class="CommandButton" id="cmdRSVP" runat="server" resourcekey="cmdRSVP" text="Subscribe" OnClick="cmdRSVP_Click"></asp:linkbutton>
			</td>
		</tr>
		<tr>
			<td colspan="2">
				<asp:datagrid id="grdServices" runat="server" summary="Register Design Table" border="0" cellpadding="5" cellspacing="5" autogeneratecolumns="false" enableviewstate="true">
					<columns>
						<asp:templatecolumn>
							<itemtemplate>
								<asp:HyperLink Text='<%# ServiceText(Convert.ToBoolean(DataBinder.Eval(Container.DataItem,"Subscribed"))) %>' CssClass="CommandButton" NavigateUrl='<%# ServiceURL("RoleID",Convert.ToString(DataBinder.Eval(Container.DataItem,"RoleID")),DataBinder.Eval(Container.DataItem,"ServiceFee"),Convert.ToBoolean(DataBinder.Eval(Container.DataItem,"Subscribed"))) %>' runat="server" ID="Hyperlink1"/>
							</itemtemplate>
						</asp:templatecolumn>
						<asp:boundcolumn headertext="Name" datafield="RoleName" itemstyle-cssclass="Normal" headerstyle-cssclass="NormalBold" />
						<asp:boundcolumn headertext="Description" datafield="Description" itemstyle-cssclass="Normal" headerstyle-cssclass="NormalBold" />
						<asp:templatecolumn headertext="Fee" headerstyle-cssclass="NormalBold">
							<itemtemplate>
								<asp:Label runat="server" Text='<%#FormatPrice(Convert.ToSingle(DataBinder.Eval(Container.DataItem, "ServiceFee"))) %>' CssClass="Normal" ID="Label2"/>
							</itemtemplate>
						</asp:templatecolumn>
						<asp:templatecolumn headertext="Every" headerstyle-cssclass="NormalBold">
							<itemtemplate>
								<asp:Label runat="server" Text='<%#FormatPeriod(Convert.ToInt32(DataBinder.Eval(Container.DataItem, "BillingPeriod"))) %>' CssClass="Normal" ID="Label3"/>
							</itemtemplate>
						</asp:templatecolumn>
						<asp:boundcolumn headertext="Period" datafield="BillingFrequency" itemstyle-cssclass="Normal" headerstyle-cssclass="NormalBold" />
						<asp:templatecolumn headertext="Trial" headerstyle-cssclass="NormalBold">
							<itemtemplate>
								<asp:Label runat="server" Text='<%#FormatPrice(Convert.ToSingle(DataBinder.Eval(Container.DataItem, "TrialFee"))) %>' CssClass="Normal" ID="Label4"/>
							</itemtemplate>
						</asp:templatecolumn>
						<asp:templatecolumn headertext="Every" headerstyle-cssclass="NormalBold">
							<itemtemplate>
								<asp:Label runat="server" Text='<%#FormatPeriod(Convert.ToInt32(DataBinder.Eval(Container.DataItem, "TrialPeriod"))) %>' CssClass="Normal" ID="Label5"/>
							</itemtemplate>
						</asp:templatecolumn>
						<asp:boundcolumn headertext="Period" datafield="TrialFrequency" itemstyle-cssclass="Normal" headerstyle-cssclass="NormalBold" />
						<asp:templatecolumn headertext="ExpiryDate" headerstyle-cssclass="NormalBold">
							<itemtemplate>
								<asp:Label runat="server" Text='<%#FormatExpiryDate(Convert.ToDateTime(DataBinder.Eval(Container.DataItem, "ExpiryDate"))) %>' CssClass="Normal" ID="Label1"/>
							</itemtemplate>
						</asp:templatecolumn>
					</columns>
				</asp:datagrid>
				<asp:label id="lblServices" runat="server" cssclass="Normal" visible="False"></asp:label></td></tr></table>
</asp:panel>
<p>
	<asp:linkbutton class="CommandButton" id="cmdRegister" runat="server" text="Register" OnClick="cmdRegister_Click"></asp:linkbutton>&nbsp;&nbsp;
	<asp:linkbutton class="CommandButton" id="cmdCancel" resourcekey="cmdCancel" runat="server" causesvalidation="False" text="Cancel" borderstyle="none" OnClick="cmdCancel_Click"></asp:linkbutton>&nbsp;&nbsp;
	<asp:linkbutton class="CommandButton" id="cmdUnregister" resourcekey="cmdUnregister" runat="server" text="Unregister" OnClick="cmdUnregister_Click"></asp:linkbutton>
</p>
