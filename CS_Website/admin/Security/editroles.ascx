<%@ Control Language="C#" AutoEventWireup="false" Explicit="True" Inherits="DotNetNuke.Modules.Admin.Security.EditRoles" CodeFile="EditRoles.ascx.cs" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register TagPrefix="dnn" TagName="SectionHead" Src="~/controls/SectionHeadControl.ascx" %>
<%@ Register TagPrefix="dnn" TagName="Url" Src="~/controls/UrlControl.ascx" %>
<table class="Settings" cellspacing="2" cellpadding="2" summary="Edit Roles Design Table" border="0">
	<tr>
		<td width="560" valign="top">
			<asp:panel id="pnlBasic" runat="server" cssclass="WorkPanel" visible="True"><dnn:sectionhead id=dshBasic cssclass="Head" runat="server" text="Basic Settings" section="tblBasic" resourcekey="BasicSettings" includerule="True"></dnn:sectionhead>
      <table id=tblBasic cellspacing=0 cellpadding=2 width=525 
      summary="Basic Settings Design Table" border=0 runat="server">
        <tr>
          <td colspan=2><asp:label id=lblBasicSettingsHelp cssclass="Normal" runat="server" resourcekey="BasicSettingsDescription" enableviewstate="False"></asp:label></td></tr>
        <tr valign=top>
          <td class=SubHead width=150><dnn:label id=plRoleName runat="server" resourcekey="RoleName" suffix=":" controlname="txtRoleName"></dnn:label></td>
          <td align=left width=325><asp:textbox id=txtRoleName cssclass="NormalTextBox" runat="server" maxlength="50" columns="30" width="325"></asp:textbox><asp:label id=lblRoleName Visible="False" Runat="server" CssClass="Normal"></asp:label><asp:requiredfieldvalidator id=valRoleName cssclass="NormalRed" runat="server" resourcekey="valRoleName" controltovalidate="txtRoleName" errormessage="<br>You Must Enter a Valid Name" display="Dynamic"></asp:requiredfieldvalidator></td></tr>
        <tr valign=top>
          <td class=SubHead width=150><dnn:label id=plDescription runat="server" resourcekey="Description" suffix=":" controlname="txtDescription"></dnn:label></td>
          <td width=325><asp:textbox id=txtDescription cssclass="NormalTextBox" runat="server" maxlength="1000" columns="30" width="325" textmode="MultiLine" height="84px"></asp:textbox></td></tr>
        <tr>
          <td class=SubHead width=150><dnn:label id=plRoleGroups runat="server" suffix="" controlname="cboRoleGroups"></dnn:label></td>
          <td width=325><asp:dropdownlist id=cboRoleGroups cssclass="NormalTextBox" Runat="server"></asp:dropdownlist></td></tr>
        <tr>
          <td class=SubHead width=150><dnn:label id=plIsPublic runat="server" resourcekey="PublicRole" controlname="chkIsPublic"></dnn:label></td>
          <td width=325><asp:checkbox id=chkIsPublic runat="server"></asp:checkbox></td></tr>
        <tr>
          <td class=SubHead width=150><dnn:label id=plAutoAssignment runat="server" resourcekey="AutoAssignment" controlname="chkAutoAssignment"></dnn:label></td>
          <td width=325><asp:checkbox id=chkAutoAssignment runat="server"></asp:checkbox></td></tr></table><br><dnn:sectionhead id=dshAdvanced cssclass="Head" runat="server" text="Advanced Settings" section="tblAdvanced" resourcekey="AdvancedSettings" includerule="True" isexpanded="False"></dnn:sectionhead>
      <table id=tblAdvanced cellspacing=0 cellpadding=2 width=525 
      summary="Advanced Settings Design Table" border=0 runat="server">
        <tr>
          <td colspan=2><asp:label id=lblAdvancedSettingsHelp cssclass="Normal" runat="server" resourcekey="AdvancedSettingsHelp" enableviewstate="False"></asp:label></td></tr>
        <tr valign=top>
          <td class=SubHead width=150><dnn:label id=plServiceFee runat="server" resourcekey="ServiceFee" suffix=":" controlname="txtServiceFee"></dnn:label></td>
          <td width=325><asp:textbox id=txtServiceFee cssclass="NormalTextBox" runat="server" maxlength="50" columns="30" width="100"></asp:textbox><asp:comparevalidator id=valServiceFee1 cssclass="NormalRed" runat="server" resourcekey="valServiceFee1" controltovalidate="txtServiceFee" errormessage="<br>Service Fee Value Entered Is Not Valid" display="Dynamic" type="Currency" operator="DataTypeCheck"></asp:comparevalidator><asp:comparevalidator id=valServiceFee2 cssclass="NormalRed" runat="server" resourcekey="valServiceFee2" controltovalidate="txtServiceFee" errormessage="<br>Service Fee Must Be Greater Than or Equal to Zero" display="Dynamic" operator="GreaterThanEqual" valuetocompare="0"></asp:comparevalidator></td></tr>
        <tr valign=top>
          <td class=SubHead width=150><dnn:label id=plBillingPeriod runat="server" resourcekey="BillingPeriod" suffix=":" controlname="txtBillingPeriod"></dnn:label></td>
          <td width=325><asp:textbox id=txtBillingPeriod cssclass="NormalTextBox" runat="server" maxlength="50" columns="30" width="100"></asp:textbox>&nbsp;&nbsp; 
<asp:dropdownlist id=cboBillingFrequency cssclass="NormalTextBox" runat="server" width="100px" datavaluefield="value" datatextfield="text"></asp:dropdownlist><asp:comparevalidator id=valBillingPeriod1 cssclass="NormalRed" runat="server" resourcekey="valBillingPeriod1" controltovalidate="txtBillingPeriod" errormessage="<br>Billing Period Value Entered Is Not Valid" display="Dynamic" type="Integer" operator="DataTypeCheck"></asp:comparevalidator><asp:comparevalidator id=valBillingPeriod2 cssclass="NormalRed" runat="server" resourcekey="valBillingPeriod2" controltovalidate="txtBillingPeriod" errormessage="<br>Billing Period Must Be Greater Than or Equal to Zero" display="Dynamic" operator="GreaterThan" valuetocompare="0"></asp:comparevalidator></td></tr>
        <tr valign=top>
          <td class=SubHead width=150><dnn:label id=plTrialFee runat="server" resourcekey="TrialFee" suffix=":" controlname="txtTrialFee"></dnn:label></td>
          <td width=325><asp:textbox id=txtTrialFee cssclass="NormalTextBox" runat="server" maxlength="50" columns="30" width="100"></asp:textbox><asp:comparevalidator id=valTrialFee1 cssclass="NormalRed" runat="server" resourcekey="valTrialFee1" controltovalidate="txtTrialFee" errormessage="<br>Trial Fee Value Entered Is Not Valid" display="Dynamic" type="Currency" operator="DataTypeCheck"></asp:comparevalidator><asp:comparevalidator id=valTrialFee2 cssclass="NormalRed" runat="server" resourcekey="valTrialFee2" controltovalidate="txtTrialFee" errormessage="<br>Trial Fee Must Be Greater Than Zero" display="Dynamic" operator="GreaterThanEqual" valuetocompare="0"></asp:comparevalidator></td></tr>
        <tr valign=top>
          <td class=SubHead width=150><dnn:label id=plTrialPeriod runat="server" resourcekey="TrialPeriod" suffix=":" controlname="txtTrialPeriod"></dnn:label></td>
          <td width=325><asp:textbox id=txtTrialPeriod cssclass="NormalTextBox" runat="server" maxlength="50" columns="30" width="100"></asp:textbox>&nbsp;&nbsp; 
<asp:dropdownlist id=cboTrialFrequency cssclass="NormalTextBox" runat="server" width="100px" datavaluefield="value" datatextfield="text"></asp:dropdownlist><asp:comparevalidator id=valTrialPeriod1 cssclass="NormalRed" runat="server" resourcekey="valTrialPeriod1" controltovalidate="txtTrialPeriod" errormessage="<br>Trial Period Value Entered Is Not Valid" display="Dynamic" type="Integer" operator="DataTypeCheck"></asp:comparevalidator><asp:comparevalidator id=valTrialPeriod2 cssclass="NormalRed" runat="server" resourcekey="valTrialPeriod2" controltovalidate="txtTrialPeriod" errormessage="<br>Trial Period Must Be Greater Than Zero" display="Dynamic" operator="GreaterThan" valuetocompare="0"></asp:comparevalidator></td></tr>
        <tr valign=top>
          <td class=SubHead width=150><dnn:label id=plRSVPCode runat="server" controlname="txtRSVPCode"></dnn:label></td>
          <td width=325><asp:textbox id=txtRSVPCode cssclass="NormalTextBox" runat="server" maxlength="50" columns="30" width="100"></asp:textbox></td>
         </tr>
		<tr>
		  <td class="SubHead" width="150" valign="top"><dnn:label id="plIcon" text="Icon:" runat="server" controlname="ctlIcon"></dnn:label></td>
		  <td width="325"><dnn:url id="ctlIcon" runat="server" width="325" showurls="False" showtabs="False" showlog="False" showtrack="False" required="False" /></td>
		</tr>
        </table>
		</asp:panel>
		</td>
	</tr>
</table>
<p>
	<asp:linkbutton id="cmdUpdate" resourcekey="cmdUpdate" runat="server" cssclass="CommandButton" text="Update" borderstyle="none" />
	&nbsp;
	<asp:linkbutton id="cmdCancel" resourcekey="cmdCancel" runat="server" cssclass="CommandButton" text="Cancel" causesvalidation="False" borderstyle="none" />
	&nbsp;
	<asp:linkbutton id="cmdDelete" resourcekey="cmdDelete" runat="server" cssclass="CommandButton" text="Delete" causesvalidation="False" borderstyle="none" />
	&nbsp;
	<asp:linkbutton id="cmdManage" resourcekey="cmdManage" runat="server" cssclass="CommandButton" text="Manage Users" causesvalidation="False" />
</p>
