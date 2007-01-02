<%@ Control Language="C#" AutoEventWireup="true" Explicit="True" Inherits="DotNetNuke.Modules.Admin.ModuleDefinitions.EditModuleControl" CodeFile="EditModuleControl.ascx.cs" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<table cellspacing="0" cellpadding="4" border="0" summary="Module Controls Design Table">
	<tr>
		<td class="SubHead" width="150"><dnn:label id="plModule" text="Module:" controlname="txtModule" runat="server" /></td>
		<td><asp:textbox id="txtModule" cssclass="NormalTextBox" width="390" columns="30" maxlength="150"
				runat="server" enabled="False" /></td>
	</tr>
	<tr>
		<td class="SubHead" width="150"><dnn:label id="plDefinition" text="Definition:" controlname="txtDefinition" runat="server" /></td>
		<td><asp:textbox id="txtDefinition" cssclass="NormalTextBox" width="390" columns="30" maxlength="150"
				runat="server" enabled="False" /></td>
	</tr>
	<tr>
		<td class="SubHead" width="150"><dnn:label id="plKey" text="Key:" controlname="txtKey" runat="server" /></td>
		<td><asp:textbox id="txtKey" cssclass="NormalTextBox" width="390" columns="30" maxlength="50" runat="server" /></td>
	</tr>
	<tr>
		<td class="SubHead" width="150"><dnn:label id="plTitle" text="Title:" controlname="txtTitle" runat="server" /></td>
		<td><asp:textbox id="txtTitle" cssclass="NormalTextBox" width="390" columns="30" maxlength="50" runat="server" /></td>
	</tr>
	<tr>
		<td class="SubHead" width="150"><dnn:label id="plSource" text="Source:" controlname="cboSource" runat="server" /></td>
		<td><asp:dropdownlist id="cboSource" runat="server" width="390" cssclass="NormalTextBox" autopostback="True" OnSelectedIndexChanged="cboSource_SelectedIndexChanged"></asp:dropdownlist></td>
	</tr>
	<tr>
		<td class="SubHead" width="150"><dnn:label id="plType" text="Type:" controlname="cboType" runat="server" /></td>
		<td>
			<asp:dropdownlist id="cboType" runat="server" width="390" cssclass="NormalTextBox">
				<asp:listitem resourcekey="Skin" value="-2">Skin Object</asp:listitem>
				<asp:listitem resourcekey="Anonymous" value="-1">Anonymous</asp:listitem>
				<asp:listitem resourcekey="View" value="0">View</asp:listitem>
				<asp:listitem resourcekey="Edit" value="1">Edit</asp:listitem>
				<asp:listitem resourcekey="Admin" value="2">Admin</asp:listitem>
				<asp:listitem resourcekey="Host" value="3">Host</asp:listitem>
			</asp:dropdownlist>
		</td>
	</tr>
	<tr>
		<td class="SubHead" width="150"><dnn:label id="plViewOrder" text="View Order:" controlname="txtViewOrder" runat="server" /></td>
		<td><asp:textbox id="txtViewOrder" cssclass="NormalTextBox" width="390" columns="30" maxlength="2"
				runat="server" /></td>
	</tr>
	<tr>
		<td class="SubHead" width="150"><dnn:label id="plIcon" text="Icon:" controlname="cboIcon" runat="server" /></td>
		<td height="23"><asp:dropdownlist id="cboIcon" runat="server" width="390" cssclass="NormalTextBox" datavaluefield="Value"
				datatextfield="Text"></asp:dropdownlist></td>
	</tr>
	<tr>
		<td class="SubHead" width="150"><dnn:label id="plHelpURL" text="Help URL:" controlname="txtHelpURL" runat="server" /></td>
		<td><asp:textbox id="txtHelpURL" runat="server" maxlength="200" columns="30" width="390" cssclass="NormalTextBox"></asp:textbox></td>
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
