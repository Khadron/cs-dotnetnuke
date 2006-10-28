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
		<asp:BoundColumn DataField="IsPremium" HeaderText="Premium">
			<HeaderStyle HorizontalAlign="Center" CssClass="NormalBold"></HeaderStyle>
			<ItemStyle HorizontalAlign="Center" CssClass="Normal"></ItemStyle>
		</asp:BoundColumn>
	</Columns>
</asp:datagrid>
