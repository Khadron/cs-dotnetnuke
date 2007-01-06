<%@ Control Language="C#" CodeFile="User.ascx.cs" AutoEventWireup="true" Inherits="DotNetNuke.Modules.Admin.Users.User" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register TagPrefix="dnn" Assembly="DotNetNuke" Namespace="DotNetNuke.UI.WebControls"%>
<%@ Register TagPrefix="dnn" TagName="SectionHead" Src="~/controls/SectionHeadControl.ascx" %>

<dnn:propertyeditorcontrol id="UserEditor" runat="Server"
	enableClientValidation = "true"
	sortmode="SortOrderAttribute" 
	labelstyle-cssclass="SubHead" 
	helpstyle-cssclass="Help" 
	editcontrolstyle-cssclass="NormalTextBox" 
	labelwidth="125px" 
	editcontrolwidth="150px" 
	width="275px" 
	editmode="Edit" 
	errorstyle-cssclass="NormalRed"/>
<asp:panel id="pnlAddUser" runat="server" visible="False">
	<table id="tblAddUser" runat="server" width="250">
		<tr height="25">
			<td width="125" class="subhead"><dnn:label id="plAuthorize" runat="server" controlname="chkAuthorize" /></td>
			<td width="125" class="normalTextBox"><asp:checkbox id="chkAuthorize" runat="server" checked="True" /></td>
		</tr>
		<tr height="25">
			<td width="125" class="subhead"><dnn:label id="plNotify" runat="server" controlname="chkNotify" /></td>
			<td width="125" class="normalTextBox"><asp:checkbox id="chkNotify" runat="server" checked="True" /></td>
		</tr>
	</table>
	<br/>
	<dnn:sectionhead id="dshPassword" cssclass="Head" runat="server" 
		text="Password" section="tblPassword" resourcekey="Password" 
		isexpanded="True" includerule="True" />
	<table id="tblPassword" runat="server" cellspacing="0" cellpadding="0" width="350" summary="Password Management" border="0">
		<tr><td colspan="2" valign="bottom"><asp:label id="lblPasswordHelp" cssclass="Normal" runat="server"></asp:label></td></tr>
		<tr><td colspan="2" height="10"></td></tr>
		<tr id="trRandom" runat="server">
			<td width="150" class="subhead"><dnn:label id="plRandom" runat="server" controlname="chkRandom" /></td>
			<td width="200" class="normalTextBox"><asp:checkbox id="chkRandom" runat="server" checked="True" /></td>
		</tr>
		<tr height="25">
			<td class="SubHead" width="150"><dnn:label id="plPassword" runat="server" controlname="txtPassword" text="Password:"></dnn:label></td>
			<td width="200"><asp:textbox id="txtPassword" runat="server" cssclass="NormalTextBox" textmode="Password" size="12" maxlength="20"></asp:textbox></td>
		</tr>
		<tr height="25">
			<td class="SubHead" width="150" valign="top"><dnn:label id="plConfirm" runat="server" controlname="txtConfirm" text="Confirm Password:"></dnn:label></td>
			<td width="200">
			    <asp:textbox id="txtConfirm" runat="server" cssclass="NormalTextBox" textmode="Password" size="12" maxlength="20"></asp:textbox>
			    <asp:CustomValidator ID="valPassword" runat="Server" CssClass="NormalRed" />
			</td>
		</tr>
		<tr id="trCaptcha" runat="server">
			<td class="SubHead" width="150" valign="top"><dnn:label id="plCaptcha" controlname="ctlCaptcha" runat="server" text="Password:"></dnn:label></td>
			<td width="200">
				<dnn:captchacontrol id="ctlCaptcha" captchawidth="130" captchaheight="40" cssclass="Normal" ErrorStyle-CssClass="NormalRed" runat="server" />
			</td>
		</tr>
	</table>
</asp:panel>
<asp:panel id="pnlUpdate" runat="server" align="center">
	<dnn:commandbutton id="cmdDelete" runat="server" 
		imageurl="~/images/delete.gif" 
		causesvalidation="False" />
	&nbsp;&nbsp;&nbsp;
	<dnn:commandbutton id="cmdUpdate" runat="server" 
		resourcekey="cmdUpdate" imageurl="~/images/save.gif" 
		causesvalidation="True" />
</asp:panel>
