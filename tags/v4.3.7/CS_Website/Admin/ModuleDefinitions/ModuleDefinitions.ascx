<%@ Control CodeFile="ModuleDefinitions.ascx.cs" Language="C#" AutoEventWireup="true" Inherits="DotNetNuke.Modules.Admin.ModuleDefinitions.ModuleDefinitions" %>
<center>
    <table id="tabFeatures" runat="server" border="0" cellpadding="4" cellspacing="0" summary="Module Definitions Design Table" width="500px">
        <tr>
            <td align="center" width="100%">
                <asp:Label ID="lblInstall" runat="server" CssClass="Normal" resourceKey="lblInstall">Installing Application Extensions</asp:Label>
            </td>
        </tr>
        <tr>
            <td align="center" width="100%">
                <asp:Label ID="lblSite" runat="server" CssClass="NormalBold" resourceKey="lblSite">Site:</asp:Label>
                <asp:DropDownList ID="cboSites" runat="server" CssClass="NormalTextBox">
                </asp:DropDownList>
                <asp:LinkButton ID="cmdGo" runat="server" CssClass="CommandButton" OnClick="cmdGo_Click" resourcekey="cmdGo">Go</asp:LinkButton>
            </td>
        </tr>
    </table>
    <hr />
    <table id="tabUpgrade" runat="server" border="0" cellpadding="4" cellspacing="0" summary="Module Definitions Design Table" width="500px">
        <tr>
            <td align="center" width="100%">
                <asp:Label ID="lblUpdate" runat="server" CssClass="Normal" resourceKey="lblUpdate">Updating Modules</asp:Label>
            </td>
        </tr>
    </table>
<asp:datagrid id="grdDefinitions" BorderWidth="0" BorderStyle="None" CellPadding="4" cellspacing="4"
	AutoGenerateColumns="false" EnableViewState="false" runat="server" summary="Module Defs Design Table"
	GridLines="None">
	<Columns>
		<asp:TemplateColumn>
			<ItemStyle Width="20px"></ItemStyle>
			<ItemTemplate>
				<asp:HyperLink NavigateUrl='<%# EditUrl("desktopmoduleid", Convert.ToString(DataBinder.Eval(Container.DataItem,"DesktopModuleId"))) %>' Visible="<%# IsEditable %>" runat="server" ID="Hyperlink1">
					<asp:Image ImageUrl="~/images/edit.gif" AlternateText="Edit" Visible="<%# IsEditable %>" runat="server" ID="Hyperlink1Image" resourcekey="Edit"/>
				</asp:HyperLink>
			</ItemTemplate>
		</asp:TemplateColumn>
		<asp:BoundColumn DataField="FriendlyName" HeaderText="ModuleName">
			<HeaderStyle Wrap="False" CssClass="NormalBold"></HeaderStyle>
			<ItemStyle Wrap="False" CssClass="Normal"></ItemStyle>
		</asp:BoundColumn>
		<asp:BoundColumn DataField="Description" HeaderText="Description">
			<HeaderStyle CssClass="NormalBold"></HeaderStyle>
			<ItemStyle CssClass="Normal"></ItemStyle>
		</asp:BoundColumn>
		<asp:BoundColumn DataField="Version" HeaderText="Version">
			<HeaderStyle HorizontalAlign="Center" CssClass="NormalBold"></HeaderStyle>
			<ItemStyle HorizontalAlign="Center" CssClass="Normal"></ItemStyle>
		</asp:BoundColumn>
        <asp:TemplateColumn HeaderText="Upgrade">
            <HeaderStyle CssClass="NormalBold" HorizontalAlign="Center" Wrap="False" />
            <ItemStyle CssClass="Normal" HorizontalAlign="Center" />
            <ItemTemplate>
                <asp:HyperLink ID="HyperLink2" runat="server" ImageUrl='<%# UpgradeStatusURL((string)DataBinder.Eval(Container.DataItem,"Version"),(string)DataBinder.Eval(Container.DataItem,"ModuleName")) %>'
                    NavigateUrl='<%# UpgradeURL((string)DataBinder.Eval(Container.DataItem,"ModuleName")) %>' Target="_new" ToolTip="Click Here To Get The Upgrade">
                </asp:HyperLink>
            </ItemTemplate>
        </asp:TemplateColumn>
	</Columns>
</asp:datagrid>
</center>