<%@ Control CodeFile="ModuleDefinitions.ascx.cs" Language="C#" AutoEventWireup="true" Inherits="DotNetNuke.Modules.Admin.ModuleDefinitions.ModuleDefinitions" %>
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
        <asp:TemplateColumn HeaderText="Upgraded">
            <HeaderStyle CssClass="NormalBold" HorizontalAlign="Center" Wrap="False" />
            <ItemStyle CssClass="Normal" HorizontalAlign="Center" />
            <ItemTemplate>
                <asp:Image ID="Image1" runat="server" AlternateText="Upgraded?" ImageUrl='<%# UpgradeURL((string)DataBinder.Eval(Container.DataItem,"Version"),(string)DataBinder.Eval(Container.DataItem,"ModuleName")) %>'
                    resourcekey="Upgraded.Header" />
            </ItemTemplate>
        </asp:TemplateColumn>
	</Columns>
</asp:datagrid>
