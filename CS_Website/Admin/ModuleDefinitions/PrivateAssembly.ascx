<%@ Control Language="C#" AutoEventWireup="true" Inherits="DotNetNuke.Modules.Admin.ModuleDefinitions.PrivateAssembly" CodeFile="PrivateAssembly.ascx.cs" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register TagPrefix="dnn" TagName="SectionHead" Src="~/controls/SectionHeadControl.ascx" %>
<table>
    <tr>
        <td class="SubHead" style="width: 150;"><dnn:label id="plFileName" text="File Name:" controlname="txtFileName" runat="server" /></td>
        <td>
            <asp:textbox id="txtFileName" cssclass="NormalTextBox" width="390" columns="30" maxlength="150" runat="server"/>
        </td>
    </tr>
    <tr>
        <td class="SubHead" valign="top" width="150">
            <dnn:Label ID="plManifest" runat="server" ControlName="chkManifest" Text="Create Manifest File (.dnn)?" />
        </td>
        <td>
            <asp:CheckBox ID="chkManifest" runat="server" CssClass="NormalTextBox" /><br>
        </td>
    </tr>
    <tr>
        <td class="SubHead" valign="top" width="150">
            <dnn:Label ID="plPrivate" runat="server" ControlName="chkPrivate" Text="Supports Private Assembly Folder?" />
        </td>
        <td>
            <asp:CheckBox ID="chkPrivate" runat="server" CssClass="NormalTextBox" /><br>
        </td>
    </tr>
    <tr id="rowSource" runat="server" visible="false">
        <td class="SubHead" valign="top" style="width: 150;">
            <dnn:Label ID="plSource" runat="server" ControlName="chkSource" Text="Include Source?" />
        </td>
        <td>
            <asp:CheckBox ID="chkSource" runat="server" CssClass="NormalTextBox" /><br/>
        </td>
    </tr>
	<tr>
		<td colspan="2">
			<asp:linkbutton id="cmdCreate" runat="server" resourcekey="cmdCreate" Text="Create" CssClass="CommandButton" OnClick="cmdCreate_Click"></asp:linkbutton>&nbsp;
			<asp:linkbutton id="cmdCancel" runat="server" resourcekey="cmdCancel" Text="Cancel" CssClass="CommandButton" OnClick="cmdCancel_Click"></asp:linkbutton>
		</td>
	</tr>
</table>
<p></p>
<asp:panel id="pnlLogs" runat="server" Visible="False">
	<dnn:sectionhead id="dshBasic" text="Language Pack Log" runat="server" resourcekey="LogTitle" section="divLog"
		includerule="True" cssclass="Head"></dnn:sectionhead>
	<div id="divLog" runat="server">
		<asp:Label id="lblLink" Runat="server" CssClass="Normal"></asp:Label>
		<hr/>
		<asp:Label id="lblMessage" runat="server"></asp:Label>
	</div>
</asp:panel>
