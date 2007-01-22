<%@ Control Inherits="DotNetNuke.Modules.Admin.PortalManagement.SiteWizard" Language="C#" AutoEventWireup="true"  enableViewState="True" CodeFile="SiteWizard.ascx.cs" %>
<%@ Register TagPrefix="dnn" TagName="Skin" Src="~/controls/SkinThumbNailControl.ascx" %>
<%@ Register TagPrefix="dnn" TagName="url" Src="~/controls/UrlControl.ascx" %>
<%@ Register TagPrefix="dnn" TagName="label" Src="~/controls/LabelControl.ascx" %>
<asp:Wizard ID="Wizard" runat="server" ActiveStepIndex="0" CellPadding="5" CellSpacing="5" CssClass="Wizard" DisplaySideBar="false" FinishCompleteButtonType="Link"
    FinishPreviousButtonType="Link" StartNextButtonType="Link" StepNextButtonType="Link" StepPreviousButtonType="Link">
    <StepStyle VerticalAlign="Top" />
    <NavigationButtonStyle BackColor="Transparent" BorderStyle="None" CssClass="CommandButton" />
    <HeaderTemplate>
        <asp:Label ID="lblTitle" runat="server" CssClass="Head"><% =Localization.GetString(Wizard.ActiveStep.Title + ".Title", this.LocalResourceFile)%></asp:Label><br />
        <br />
        <asp:Label ID="lblTitleHelp" runat="server" CssClass="WizardText"><% =Localization.GetString(Wizard.ActiveStep.Title + ".Help", this.LocalResourceFile)%></asp:Label>
    </HeaderTemplate>
    <WizardSteps>
        <asp:WizardStep ID="wizIntroduction" runat="server" AllowReturn="false" StepType="Start" Title="Introduction">
        </asp:WizardStep>
        <asp:WizardStep ID="wizTemplate" runat="server" Title="Template">
            <table border="0" cellpadding="0" cellspacing="0">
                <tr>
                    <td colspan="3" style="height: 5;">
                    </td>
                </tr>
                <tr>
                    <td colspan="3">
                        <asp:CheckBox ID="chkTemplate" runat="server" AutoPostBack="True" CssClass="WizardText" resourcekey="TemplateDetail" Text="Build your site from a template (below)" />
                    </td>
                </tr>
                <tr>
                    <td colspan="3" style="height: 5;">
                    </td>
                </tr>
                <tr>
                    <td align="center" style="width: 150;">
                        <asp:ListBox ID="lstTemplate" runat="server" AutoPostBack="True" Rows="8" Width="150"></asp:ListBox>
                    </td>
                    <td align="left" colspan="2" valign="top" style="width: 300;">
                        <asp:Label ID="lblTemplateMessage" runat="server" CssClass="NormalRed" Style="overflow: auto; width: 280px; height: 150px"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td colspan="3" style="height: 5;">
                    </td>
                </tr>
                <tr>
                    <td colspan="3">
                        <asp:Label ID="lblMergeTitle" runat="server" CssClass="WizardText" resourcekey="MergeDetail"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td colspan="3" style="height: 5;">
                    </td>
                </tr>
                <tr>
                    <td align="center" colspan="3">
                        <asp:RadioButtonList ID="optMerge" runat="server" CssClass="WizardText" RepeatDirection="Horizontal">
                            <asp:ListItem resourcekey="Ignore" Selected="" Value="Ignore">Ignore</asp:ListItem>
                            <asp:ListItem resourcekey="Replace" Value="Replace">Replace</asp:ListItem>
                            <asp:ListItem resourcekey="Merge" Value="Merge">Merge</asp:ListItem>
                        </asp:RadioButtonList>
                    </td>
                </tr>
                <tr>
                    <td colspan="3" style="height: 5;">
                    </td>
                </tr>
                <tr>
                    <td colspan="3">
                        <asp:Label ID="lblMergeWarning" runat="server" CssClass="WizardText" resourcekey="MergeWarning"></asp:Label>
                    </td>
                </tr>
            </table>
        </asp:WizardStep>
        <asp:WizardStep ID="wizSkin" runat="server" Title="Skin">
            <table border="0" cellpadding="0" cellspacing="0">
                <tr>
                    <td style="height: 5;">
                    </td>
                </tr>
                <tr>
                    <td align="center">
                        <dnn:Skin ID="ctlPortalSkin" runat="server" />
                    </td>
                </tr>
            </table>
        </asp:WizardStep>
        <asp:WizardStep ID="wizContainer" runat="server" Title="Container">
            <table border="0" cellpadding="0" cellspacing="0">
                <tr>
                    <td style="height: 5;">
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:CheckBox ID="chkIncludeAll" runat="server" AutoPostBack="True" CssClass="WizardText" resourcekey="IncludeAll" Text="Show All Containers:" TextAlign="Left" /></td>
                </tr>
                <tr>
                    <td style="height: 5;">
                    </td>
                </tr>
                <tr>
                    <td align="center" colspan="2">
                        <dnn:Skin ID="ctlPortalContainer" runat="server" />
                    </td>
                </tr>
            </table>
        </asp:WizardStep>
        <asp:WizardStep ID="wizDetails" runat="server" Title="Details">
            <table border="0" cellpadding="0" cellspacing="0">
                <tr>
                    <td colspan="2" style="height: 5;">
                    </td>
                </tr>
                <tr>
                    <td class="SubHead" style="width: 150;">
                        <dnn:label ID="lblPortalName" runat="server" ControlName="txtPortalName" Text="Name/Title:" />
                    </td>
                    <td align="left" class="NormalTextBox" valign="top">
                        <asp:TextBox ID="txtPortalName" runat="server" CssClass="NormalTextBox" MaxLength="128" Width="300"></asp:TextBox></td>
                </tr>
                <tr>
                    <td colspan="2" style="height: 5;">
                    </td>
                </tr>
                <tr>
                    <td class="SubHead" valign="top" style="width: 150;">
                        <dnn:label ID="lblDescription" runat="server" Text="Description:" />
                    </td>
                    <td align="left" class="NormalTextBox">
                        <asp:TextBox ID="txtDescription" runat="server" CssClass="NormalTextBox" MaxLength="475" Rows="3" TextMode="MultiLine" Width="300"></asp:TextBox></td>
                </tr>
                <tr>
                    <td colspan="2" style="height: 5;">
                    </td>
                </tr>
                <tr>
                    <td class="SubHead" valign="top" style="width: 150;">
                        <dnn:label ID="lblKeyWords" runat="server" Text="Key Words:" />
                    </td>
                    <td align="left" class="NormalTextBox">
                        <asp:TextBox ID="txtKeyWords" runat="server" CssClass="NormalTextBox" MaxLength="475" Rows="3" TextMode="MultiLine" Width="300"></asp:TextBox></td>
                </tr>
                <tr>
                    <td colspan="2" style="height: 5;">
                    </td>
                </tr>
                <tr>
                    <td class="SubHead" valign="top" style="width: 120;">
                        <dnn:label ID="lblLogo" runat="server" Text="Logo:" />
                    </td>
                    <td align="left" class="NormalTextBox">
                        <dnn:url ID="urlLogo" runat="server" Required="false" ShowLog="False" ShowTabs="False" ShowTrack="false" ShowUrls="False" />
                    </td>
                </tr>
            </table>
        </asp:WizardStep>
        <asp:WizardStep ID="wizComplete" runat="server" StepType="Complete">
            <asp:Label ID="lblWizardTitle" runat="server" CssClass="Head" resourcekey="Complete.Title"></asp:Label><br />
            <br />
            <asp:Label ID="lblHelp" runat="server" CssClass="WizardText" resourcekey="Complete.Help"></asp:Label>
        </asp:WizardStep>
    </WizardSteps>
</asp:Wizard>
