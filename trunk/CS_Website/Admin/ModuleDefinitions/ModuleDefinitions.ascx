<%@ Control CodeFile="ModuleDefinitions.ascx.cs" Language="C#" AutoEventWireup="true" Inherits="DotNetNuke.Modules.Admin.ModuleDefinitions.ModuleDefinitions" %>
<%@ Register Assembly="DotNetNuke" Namespace="DotNetNuke.UI.WebControls" TagPrefix="dnn" %>
<%@ Register Src="~/controls/SectionHeadControl.ascx" TagName="SectionHead" TagPrefix="dnn" %>
<table border="0" cellpadding="2" cellspacing="2" summary="Module Definitions Design Table">
    <tr id="rowFeatures" runat="server">
        <td align="left" width="100%">
            <dnn:SectionHead ID="dshFeatures" runat="server" CssClass="Head" IncludeRule="True" ResourceKey="Features" Section="divFeatures" Text="Features" />
            <div id="divFeatures" runat="Server">
                <asp:Label ID="lblInstall" runat="server" CssClass="Normal" resourceKey="lblInstall">Installing Application Extensions</asp:Label>
                <br />
                <br />
                <asp:Label ID="lblSite" runat="server" CssClass="NormalBold" resourceKey="lblSite">Site:</asp:Label>
                <asp:DropDownList ID="cboSites" runat="server" CssClass="NormalTextBox">
                </asp:DropDownList>
                <asp:LinkButton ID="cmdGo" runat="server" CssClass="CommandButton" resourcekey="cmdGo" OnClick="cmdGo_Click">Go</asp:LinkButton>
            </div>
        </td>
    </tr>
    <tr>
        <td>
            <br />
            <dnn:SectionHead ID="dshInstalled" runat="server" CssClass="Head" IncludeRule="True" ResourceKey="Installed" Section="divInstalled" Text="Installed Modules" />
            <div id="divInstalled" runat="Server">
                <asp:Label ID="lblUpdate" runat="server" CssClass="Normal" resourceKey="lblUpdate"></asp:Label>
                <br />
                <asp:DataGrid ID="grdDefinitions" runat="server" AutoGenerateColumns="false" BorderStyle="None" BorderWidth="0" CellPadding="4" CellSpacing="4" EnableViewState="false" GridLines="None" summary="Module Defs Design Table">
                    <HeaderStyle CssClass="NormalBold" Wrap="False" />
                    <ItemStyle CssClass="Normal" Wrap="False" />
                    <Columns>
                        <asp:TemplateColumn>
                            <ItemStyle Width="20px" />
                            <ItemTemplate>
                                <asp:HyperLink ID="Hyperlink1" runat="server" NavigateUrl='<%# EditUrl("desktopmoduleid",(string)DataBinder.Eval(Container.DataItem,"DesktopModuleId")) %>' Visible="<%# IsEditable %>">
                                    <asp:Image ID="Hyperlink1Image" runat="server" AlternateText="Edit" ImageUrl="~/images/edit.gif" resourcekey="Edit" Visible="<%# IsEditable %>" />
                                </asp:HyperLink>
                            </ItemTemplate>
                        </asp:TemplateColumn>
                        <asp:BoundColumn DataField="FriendlyName" HeaderText="ModuleName">
                            <ItemStyle CssClass="Normal" Wrap="False" />
                        </asp:BoundColumn>
                        <asp:BoundColumn DataField="Description" HeaderText="Description">
                            <ItemStyle CssClass="Normal" />
                        </asp:BoundColumn>
                        <asp:BoundColumn DataField="Version" HeaderText="Version">
                            <HeaderStyle HorizontalAlign="Center" />
                            <ItemStyle CssClass="Normal" HorizontalAlign="Center" />
                        </asp:BoundColumn>
                        <asp:TemplateColumn HeaderText="Upgrade">
                            <HeaderStyle HorizontalAlign="Center" Wrap="False" />
                            <ItemStyle CssClass="Normal" HorizontalAlign="Center" />
                            <ItemTemplate>
                                <asp:HyperLink ID="HyperLink2" runat="server" ImageUrl='<%# UpgradeStatusURL((string)DataBinder.Eval(Container.DataItem,"Version"),(string)DataBinder.Eval(Container.DataItem,"ModuleName")) %>' NavigateUrl='<%# UpgradeURL((string)DataBinder.Eval(Container.DataItem,"ModuleName")) %>' Target="_new" ToolTip="Click Here To Get The Upgrade">
                                </asp:HyperLink>
                            </ItemTemplate>
                        </asp:TemplateColumn>
                    </Columns>
                </asp:DataGrid>
            </div>
        </td>
    </tr>
    <tr>
        <td>
            <br />
            <dnn:SectionHead ID="dshAvailable" runat="server" CssClass="Head" IncludeRule="True" ResourceKey="Available" Section="divAvailable" Text="Available Modules" />
            <div id="divAvailable" runat="server">
                <asp:Label ID="lblAvailable" runat="server" CssClass="Normal" resourceKey="lblAvailable"></asp:Label>
                <br />
                <br />
                <asp:CheckBoxList ID="lstModules" runat="server" CssClass="Normal" RepeatColumns="3" RepeatDirection="Horizontal">
                </asp:CheckBoxList>
                <br />
                <dnn:CommandButton ID="cmdInstall" runat="server" CausesValidation="False" ImageUrl="~/images/register.gif" ResourceKey="cmdInstall" />
                <br />
                <asp:PlaceHolder ID="phPaLogs" runat="server"></asp:PlaceHolder>
                <br />
                <dnn:CommandButton ID="cmdRefresh" runat="server" CausesValidation="False" ImageUrl="~/images/refresh.gif" ResourceKey="cmdRefresh" Visible="false" />
            </div>
        </td>
    </tr>
</table>
