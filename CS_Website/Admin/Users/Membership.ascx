<%@ Register TagPrefix="dnn" Assembly="DotNetNuke" Namespace="DotNetNuke.UI.WebControls"%>
<%@ Control Language="C#" CodeFile="Membership.ascx.cs" AutoEventWireup="true" Inherits="DotNetNuke.Modules.Admin.Users.Membership" %>

<dnn:propertyeditorcontrol id="MembershipEditor" runat="Server" 
	editmode="View" width="350px" editcontrolwidth="200px" 
	labelwidth="150px" editcontrolstyle-cssclass="NormalTextBox" 
	helpstyle-cssclass="Help" labelstyle-cssclass="SubHead" 
	namedatafield="Name" valuedatafield="PropertyValue" 
	sortmode="SortOrderAttribute" />
<p align="center">
	<dnn:commandbutton id="cmdAuthorize" runat="server" 
		resourcekey="cmdAuthorize" imageurl="~/images/checked.gif" 
		causesvalidation="False" OnClick="cmdAuthorize_Click" />
	<dnn:commandbutton id="cmdUnAuthorize" runat="server" 
		resourcekey="cmdUnAuthorize" imageurl="~/images/unchecked.gif" 
		causesvalidation="False" OnClick="cmdUnAuthorize_Click" />
	&nbsp;&nbsp;
	<dnn:commandbutton id="cmdUnLock" runat="server" 
		resourcekey="cmdUnLock" imageurl="~/images/icon_securityroles_16px.gif" 
		causesvalidation="False" OnClick="cmdUnLock_Click" />
	&nbsp;&nbsp;
	<dnn:commandbutton id="cmdPassword" runat="server" 
		resourcekey="cmdPassword" imageurl="~/images/icon_securityroles_16px.gif" 
		causesvalidation="False" OnClick="cmdPassword_Click" />
</p>
