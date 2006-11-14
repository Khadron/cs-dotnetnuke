<%@ Control Language="C#" AutoEventWireup="true" Inherits="DotNetNuke.Modules.Admin.ModuleDefinitions.PrivateAssembly" CodeFile="PrivateAssembly.ascx.cs" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register TagPrefix="dnn" TagName="SectionHead" Src="~/controls/SectionHeadControl.ascx" %>
<table>
    <tr>
        <td class="SubHead" width="150"><dnn:label id="plFileName" text="File Name:" controlname="txtFileName" runat="server" /></td>
        <td>
            <asp:textbox id="txtFileName" cssclass="NormalTextBox" width="390" columns="30" maxlength="150" runat="server"/>
        </td>
    </tr>
	<tr>
		<td colspan="2">
			<asp:linkbutton id="cmdCreate" runat="server" resourcekey="cmdCreate" Text="Create" CssClass="CommandButton"></asp:linkbutton>&nbsp;
			<asp:linkbutton id="cmdCancel" runat="server" resourcekey="cmdCancel" Text="Cancel" CssClass="CommandButton"></asp:linkbutton>
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
