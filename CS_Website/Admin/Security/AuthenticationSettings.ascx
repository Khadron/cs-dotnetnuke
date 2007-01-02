<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Control Language="C#" CodeFile="AuthenticationSettings.ascx.cs" AutoEventWireup="true" Inherits="DotNetNuke.Modules.Authentication.AuthenticationSettings" %>
<table cellSpacing="2" cellPadding="2" width="800" summary="Authentication Settings Design Table"
	border="0">
	<tr vAlign="top" height="*">
		<td id="MessageCell" align="center" colSpan="3" runat="server"></td>
	</tr>
	<tr>
		<td vAlign="top" width="760">
			<table id="tblSettings" cellSpacing="0" cellPadding="1" border="0">
				<tr>
					<td class="SubHead" width="200"><dnn:label id="plAuthentication" runat="server" controlname="chkAuthentication" text="Windows Authentication?"></dnn:label></td>
					<td vAlign="top"><asp:checkbox id="chkAuthentication" runat="server" cssclass="NormalTextBox"></asp:checkbox></td>
				</tr>
				<tr>
					<td class="SubHead" width="200"><dnn:label id="plSynchronizeRole" runat="server" controlname="chkSynchronizeRole" text="Synchronize Role?"></dnn:label></td>
					<td vAlign="top"><asp:checkbox id="chkSynchronizeRole" runat="server" cssclass="NormalTextBox"></asp:checkbox></td>
				</tr>
				<tr id="rowSynchornizePassword" runat="server" visible="False">
					<td class="SubHead" width="200"><dnn:label id="plSynchornizePassword" runat="server" controlname="chkSynchronizePassword" text="Synchronize Password?"></dnn:label></td>
					<td vAlign="top"><asp:checkbox id="chkSynchronizePassword" runat="server" cssclass="NormalTextBox"></asp:checkbox></td>
				</tr>
				<tr>
					<td class="SubHead" width="200"><dnn:label id="plProvider" runat="server" controlname="cboProviders" text="Provider"></dnn:label></td>
					<td vAlign="top"><asp:dropdownlist id="cboProviders" runat="server" cssclass="NormalTextBox" width="300"></asp:dropdownlist></td>
				</tr>
				<tr>
					<td class="SubHead" width="200"><dnn:label id="plAuthenticationType" runat="server" controlname="cboAuthenticationType" text="Authentication Type"></dnn:label></td>
					<td vAlign="top"><asp:dropdownlist id="cboAuthenticationType" runat="server" cssclass="NormalTextBox" width="300"></asp:dropdownlist></td>
				</tr>
				<tr id="rowRootDomain" runat="server">
					<td class="SubHead" width="200"><dnn:label id="plRootDomain" runat="server" controlname="txtRootDomain" text="Root Domain:"></dnn:label></td>
					<td vAlign="top" noWrap><asp:textbox id="txtRootDomain" runat="server" cssclass="NormalTextBox" Width="300px"></asp:textbox></td>
				</tr>
				<tr id="rowUserName" runat="server">
					<td class="SubHead" width="200"><dnn:label id="plUserName" runat="server" controlname="txtUserName" text="User Name:"></dnn:label></td>
					<td vAlign="top" noWrap><asp:textbox id="txtUserName" runat="server" cssclass="NormalTextBox" Width="300px"></asp:textbox></td>
				</tr>
				<tr id="rowPassword" runat="server">
					<td class="SubHead" width="200"><dnn:label id="plPassword" runat="server" controlname="txtPassword" text="Password:"></dnn:label></td>
					<td vAlign="top" noWrap><asp:textbox id="txtPassword" runat="server" cssclass="NormalTextBox" Width="300px" TextMode="Password"></asp:textbox></td>
				</tr>
				<tr id="rowConfirm" runat="server">
					<td class="SubHead" width="200"><dnn:label id="plConfirm" runat="server" controlname="chkAuthentication" text="Confirm Password:"></dnn:label></td>
					<td vAlign="top" noWrap><asp:textbox id="txtConfirm" runat="server" cssclass="NormalTextBox" Width="300px" TextMode="Password"></asp:textbox><asp:comparevalidator id="valConfirm" runat="server" ControlToCompare="txtPassword" ControlToValidate="txtConfirm"
							ErrorMessage="<br>Password Values Entered Do Not Match." Display="Dynamic" CssClass="NormalRed"></asp:comparevalidator></td>
				</tr>
				<tr id="rowEmailDomain" runat="server">
					<td class="SubHead" width="200"><dnn:label id="plEmailDomain" runat="server" controlname="txtEmailDomain" text="Email Domain:"></dnn:label></td>
					<td vAlign="top" noWrap><asp:textbox id="txtEmailDomain" runat="server" cssclass="NormalTextBox" Width="300px"></asp:textbox></td>
				</tr>
			</table>
			<P><asp:linkbutton class="CommandButton" id="cmdUpdate" runat="server" text="Update" resourcekey="cmdUpdate" OnClick="cmdAuthenticationUpdate_Click"></asp:linkbutton></P>
		</td>
	</tr>
</table>
