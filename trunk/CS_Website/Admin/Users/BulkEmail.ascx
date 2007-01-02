<%@ Register Src="~/controls/TextEditor.ascx" TagName="TextEditor" TagPrefix="dnn" %>
<%@ Register Src="~/controls/SectionHeadControl.ascx" TagName="SectionHead" TagPrefix="dnn" %>
<%@ Register Src="~/controls/LabelControl.ascx" TagName="Label" TagPrefix="dnn" %>
<%@ Control AutoEventWireup="true" CodeFile="BulkEmail.ascx.cs" Explicit="True" Inherits="DotNetNuke.Modules.Admin.Users.BulkEmail" Language="C#" %>
<%@ Register Src="~/controls/URLControl.ascx" TagName="URLControl" TagPrefix="dnn" %>
<table border="0" cellpadding="2" cellspacing="2" class="Settings" summary="Edit Roles Design Table">
    <tr>
        <td valign="top" width="560">
            <asp:Panel ID="pnlSettings" runat="server" CssClass="WorkPanel" Visible="True">
                <dnn:SectionHead ID="dshBasic" runat="server" CssClass="Head" IncludeRule="True" ResourceKey="BasicSettings" Section="tblBasic" Text="Basic Settings" />
                <table id="tblBasic" runat="server" border="0" cellpadding="2" cellspacing="0" summary="Basic Settings Design Table" width="525">
                    <tr>
                        <td colspan="2">
                            <asp:Label ID="lblBasicSettingsHelp" runat="server" CssClass="Normal" EnableViewState="False" resourcekey="BasicSettingsDescription"></asp:Label></td>
                    </tr>
                    <tr>
                        <td class="SubHead" width="150">
                            <dnn:Label ID="plRoles" runat="server" ControlName="chkRoles" Suffix=":" />
                        </td>
                        <td align="center" width="325">
                            <asp:CheckBoxList ID="chkRoles" runat="server" CssClass="Normal" DataTextField="RoleName" DataValueField="RoleName" RepeatColumns="2" Width="325">
                            </asp:CheckBoxList></td>
                    </tr>
                    <tr valign="top">
                        <td class="SubHead" width="150">
                            <dnn:Label ID="plEmail" runat="server" ControlName="txtEmail" Suffix=":" />
                        </td>
                        <td align="center" width="325">
                            <asp:TextBox ID="txtEmail" runat="server" CssClass="NormalTextBox" Rows="3" TextMode="MultiLine" Width="325"></asp:TextBox></td>
                    </tr>
                    <tr valign="top">
                        <td class="SubHead" width="150">
                            <dnn:Label ID="plFrom" runat="server" ControlName="txtFrom" Suffix=":" />
                        </td>
                        <td width="325">
                            <asp:TextBox ID="txtFrom" runat="server" Columns="40" CssClass="NormalTextBox" MaxLength="100" Width="325"></asp:TextBox>
                            <asp:RegularExpressionValidator ID="revEmailAddress" runat="server" ControlToValidate="txtFrom" CssClass="NormalRed" ErrorMessage="RegularExpressionValidator" resourcekey="revEmailAddress.ErrorMessage"
                                ValidationExpression="\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"></asp:RegularExpressionValidator></td>
                    </tr>
                    <tr valign="top">
                        <td class="SubHead" width="150">
                            <dnn:Label ID="plSubject" runat="server" ControlName="txtSubject" Suffix=":" />
                        </td>
                        <td width="325">
                            <asp:TextBox ID="txtSubject" runat="server" Columns="40" CssClass="NormalTextBox" MaxLength="100" Width="325"></asp:TextBox></td>
                    </tr>
                </table>
                <br>
                <dnn:SectionHead ID="dshMessage" runat="server" CssClass="Head" IncludeRule="True" ResourceKey="Message" Section="tblMessage" Text="Message" />
                <table id="tblMessage" runat="server" border="0" cellpadding="2" cellspacing="0" summary="Message Design Table" width="525">
                    <tr>
                        <td colspan="2">
                            <asp:Label ID="lblMessageHelp" runat="server" CssClass="Normal" EnableViewState="False" resourcekey="MessageDescription"></asp:Label></td>
                    </tr>
                    <tr valign="top">
                        <td colspan="2">
                            <dnn:TextEditor ID="teMessage" runat="server" ChooseMode="True" ChooseRender="False" DefaultMode="Rich" Height="350" HtmlEncode="False" TextRenderMode="Raw" Width="550" />
                        </td>
                    </tr>
                </table>
                <br>
                <dnn:SectionHead ID="dshAdvanced" runat="server" CssClass="Head" IncludeRule="True" IsExpanded="False" ResourceKey="AdvancedSettings" Section="tblAdvanced" Text="Advanced Settings" />
                <table id="tblAdvanced" runat="server" border="0" cellpadding="2" cellspacing="0" summary="Message Design Table" width="525">
                    <tr>
                        <td colspan="2">
                            <asp:Label ID="lblAdvancedSettingsHelp" runat="server" CssClass="Normal" EnableViewState="False" resourcekey="AdvancedSettingsHelp"></asp:Label></td>
                    </tr>
                    <tr valign="top">
                        <td class="SubHead" width="150">
                            <dnn:Label ID="plAttachment" runat="server" ControlName="cboAttachment" Suffix=":" />
                        </td>
                        <td width="325">
                            <dnn:URLControl ID="ctlAttachment" runat="server" Required="False" ShowLog="False" ShowTabs="False" ShowTrack="False" ShowUpLoad="true" ShowUrls="False" />
                        </td>
                    </tr>
                    <tr valign="top">
                        <td class="SubHead" width="150">
                            <dnn:Label ID="plPriority" runat="server" ControlName="cboPriority" Suffix=":" />
                        </td>
                        <td width="325">
                            <asp:DropDownList ID="cboPriority" runat="server" CssClass="NormalTextBox" Width="100">
                                <asp:ListItem resourcekey="High" Value="1">High</asp:ListItem>
                                <asp:ListItem resourcekey="Normal" Selected="True" Value="2">Normal</asp:ListItem>
                                <asp:ListItem resourcekey="Low" Value="3">Low</asp:ListItem>
                            </asp:DropDownList></td>
                    </tr>
                    <tr valign="top">
                        <td class="SubHead" width="150">
                            <dnn:Label ID="plSendMethod" runat="server" ControlName="cboSendMethod" Suffix=":" />
                        </td>
                        <td width="325">
                            <asp:DropDownList ID="cboSendMethod" runat="server" CssClass="NormalTextBox" Width="325px">
                                <asp:ListItem resourcekey="SendTo" Selected="True" Value="TO">TO: One Message Per Email Address ( Personalized )</asp:ListItem>
                                <asp:ListItem resourcekey="SendBCC" Value="BCC">BCC: One Email To Blind Distribution List ( Not Personalized )</asp:ListItem>
                            </asp:DropDownList></td>
                    </tr>
                    <tr valign="top">
                        <td class="SubHead" width="150">
                            <dnn:Label ID="plSendAction" runat="server" ControlName="optSendAction" Suffix=":" />
                        </td>
                        <td width="325">
                            <asp:RadioButtonList ID="optSendAction" runat="server" CssClass="Normal" RepeatDirection="Horizontal">
                                <asp:ListItem resourcekey="Synchronous" Value="S">Synchronous</asp:ListItem>
                                <asp:ListItem resourcekey="Asynchronous" Selected="True" Value="A">Asynchronous</asp:ListItem>
                            </asp:RadioButtonList></td>
                    </tr>
                </table>
            </asp:Panel>
        </td>
    </tr>
</table>
<p>
    <asp:LinkButton ID="cmdSend" runat="server" CssClass="CommandButton" OnClick="cmdSend_Click" resourcekey="cmdSend" Text="Send Email">
    </asp:LinkButton>
</p>
