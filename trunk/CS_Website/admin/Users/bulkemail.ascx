<%@ Register TagPrefix="dnn" TagName="TextEditor" Src="~/controls/TextEditor.ascx"%>
<%@ Register TagPrefix="dnn" TagName="SectionHead" Src="~/controls/SectionHeadControl.ascx" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Control Inherits="DotNetNuke.Modules.Admin.Users.BulkEmail" CodeFile="BulkEmail.ascx.cs" Language="C#" AutoEventWireup="true" Explicit="True" %>
<%@ Register TagPrefix="dnn" TagName="URLControl" Src="~/controls/URLControl.ascx" %>
<table class="Settings" cellspacing="2" cellpadding="2" summary="Edit Roles Design Table"
	border="0">
	<tr>
		<td width="560" valign="top">
			<asp:panel id="pnlSettings" runat="server" cssclass="WorkPanel" visible="True">
				<dnn:sectionhead id="dshBasic" cssclass="Head" runat="server" includerule="True" resourcekey="BasicSettings"
					section="tblBasic" text="Basic Settings"></dnn:sectionhead>
				<TABLE id="tblBasic" cellSpacing="0" cellPadding="2" width="525" summary="Basic Settings Design Table"
					border="0" runat="server">
					<TR>
						<TD colSpan="2">
							<asp:label id="lblBasicSettingsHelp" cssclass="Normal" runat="server" resourcekey="BasicSettingsDescription"
								enableviewstate="False"></asp:label></TD>
					</TR>
					<TR>
						<TD class="SubHead" width="150">
							<dnn:label id="plRoles" runat="server" controlname="chkRoles" suffix=":"></dnn:label></TD>
						<TD align="center" width="325">
							<asp:checkboxlist id="chkRoles" cssclass="Normal" runat="server" width="325" datatextfield="RoleName"
								datavaluefield="RoleName" repeatcolumns="2"></asp:checkboxlist></TD>
					</TR>
					<TR vAlign="top">
						<TD class="SubHead" width="150">
							<dnn:label id="plEmail" runat="server" controlname="txtEmail" suffix=":"></dnn:label></TD>
						<TD align="center" width="325">
							<asp:textbox id="txtEmail" cssclass="NormalTextBox" runat="server" width="325" textmode="MultiLine"
								rows="3"></asp:textbox></TD>
					</TR>
					<TR vAlign="top">
						<TD class="SubHead" width="150">
							<dnn:label id="plFrom" runat="server" controlname="txtFrom" suffix=":"></dnn:label></TD>
						<TD width="325">
							<asp:textbox id="txtFrom" cssclass="NormalTextBox" runat="server" width="325" columns="40" maxlength="100"></asp:textbox>
							<asp:RegularExpressionValidator id="revEmailAddress" runat="server" resourcekey="revEmailAddress.ErrorMessage" ErrorMessage="RegularExpressionValidator" CssClass="NormalRed"
								ControlToValidate="txtFrom" ValidationExpression="\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"></asp:RegularExpressionValidator></TD>
					</TR>
					<TR vAlign="top">
						<TD class="SubHead" width="150">
							<dnn:label id="plSubject" runat="server" controlname="txtSubject" suffix=":"></dnn:label></TD>
						<TD width="325">
							<asp:textbox id="txtSubject" cssclass="NormalTextBox" runat="server" width="325" columns="40"
								maxlength="100"></asp:textbox></TD>
					</TR>
				</TABLE>
				<BR>
				<dnn:sectionhead id="dshMessage" cssclass="Head" runat="server" includerule="True" resourcekey="Message"
					section="tblMessage" text="Message"></dnn:sectionhead>
				<TABLE id="tblMessage" cellSpacing="0" cellPadding="2" width="525" summary="Message Design Table"
					border="0" runat="server">
					<TR>
						<TD colSpan="2">
							<asp:label id="lblMessageHelp" cssclass="Normal" runat="server" resourcekey="MessageDescription"
								enableviewstate="False"></asp:label></TD>
					</TR>
					<TR vAlign="top">
						<TD colSpan="2">
							<dnn:texteditor id="teMessage" runat="server" width="550" textrendermode="Raw" htmlencode="False"
								defaultmode="Rich" height="350" choosemode="True" chooserender="False"></dnn:texteditor></TD>
					</TR>
				</TABLE>
				<BR>
				<dnn:sectionhead id="dshAdvanced" cssclass="Head" runat="server" includerule="True" resourcekey="AdvancedSettings"
					section="tblAdvanced" text="Advanced Settings" isexpanded="False"></dnn:sectionhead>
				<TABLE id="tblAdvanced" cellSpacing="0" cellPadding="2" width="525" summary="Message Design Table"
					border="0" runat="server">
					<TR>
						<TD colSpan="2">
							<asp:label id="lblAdvancedSettingsHelp" cssclass="Normal" runat="server" resourcekey="AdvancedSettingsHelp"
								enableviewstate="False"></asp:label></TD>
					</TR>
					<TR vAlign="top">
						<TD class="SubHead" width="150">
							<dnn:label id="plAttachment" runat="server" controlname="cboAttachment" suffix=":"></dnn:label></TD>
						<TD width="325">
							<dnn:URLControl id="ctlAttachment" runat="server" required="False" ShowUpload="true" ShowTrack="False"
								ShowLog="False" ShowTabs="False" ShowUrls="False"></dnn:URLControl></TD>
					</TR>
					<TR vAlign="top">
						<TD class="SubHead" width="150">
							<dnn:label id="plPriority" runat="server" controlname="cboPriority" suffix=":"></dnn:label></TD>
						<TD width="325">
							<asp:dropdownlist id="cboPriority" cssclass="NormalTextBox" runat="server" width="100">
								<asp:listitem resourcekey="High" value="1">High</asp:listitem>
								<asp:listitem resourcekey="Normal" value="2" selected="True">Normal</asp:listitem>
								<asp:listitem resourcekey="Low" value="3">Low</asp:listitem>
							</asp:dropdownlist></TD>
					</TR>
					<TR vAlign="top">
						<TD class="SubHead" width="150">
							<dnn:label id="plSendMethod" runat="server" controlname="cboSendMethod" suffix=":"></dnn:label></TD>
						<TD width="325">
							<asp:dropdownlist id="cboSendMethod" cssclass="NormalTextBox" runat="server" width="325px">
								<asp:listitem resourcekey="SendTo" value="TO" selected="True">TO: One Message Per Email Address ( Personalized )</asp:listitem>
								<asp:listitem resourcekey="SendBCC" value="BCC">BCC: One Email To Blind Distribution List ( Not Personalized )</asp:listitem>
							</asp:dropdownlist></TD>
					</TR>
					<TR vAlign="top">
						<TD class="SubHead" width="150">
							<dnn:label id="plSendAction" runat="server" controlname="optSendAction" suffix=":"></dnn:label></TD>
						<TD width="325">
							<asp:radiobuttonlist id="optSendAction" cssclass="Normal" runat="server" repeatdirection="Horizontal">
								<asp:listitem resourcekey="Synchronous" value="S">Synchronous</asp:listitem>
								<asp:listitem resourcekey="Asynchronous" value="A" selected="True">Asynchronous</asp:listitem>
							</asp:radiobuttonlist></TD>
					</TR>
				</TABLE>
			</asp:panel>
		</td>
	</tr>
</table>
<p>
	<asp:linkbutton id="cmdSend" resourcekey="cmdSend" text="Send Email" runat="server" cssclass="CommandButton" />
</p>
