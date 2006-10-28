<%@ Control Language="C#" AutoEventWireup="false" Explicit="True" Inherits="DotNetNuke.Modules.Admin.Portals.Portals" CodeFile="Portals.ascx.cs" %>
<asp:datagrid id="grdPortals" runat="server" Width="100%" EnableViewState="false" AutoGenerateColumns="false" CellSpacing="0" CellPadding="4" Border="0" summary="Portals Design Table" BorderStyle="None" BorderWidth="0px" GridLines="None">
<Columns>
<asp:TemplateColumn>
<ItemStyle Width="20px">
</ItemStyle>

<ItemTemplate>
				<asp:HyperLink NavigateUrl='<%# GetEditURL(Convert.ToInt32(DataBinder.Eval(Container.DataItem,"PortalID"))) %>' runat="server" ID="lnkEdit">
<asp:Image ImageUrl="~/images/edit.gif" resourcekey="Edit" AlternateText="Edit this Portal" runat="server" ID="imgEdit"/></asp:HyperLink>
			
</ItemTemplate>
</asp:TemplateColumn>
<asp:TemplateColumn HeaderText="Title">
<HeaderStyle CssClass="NormalBold">
</HeaderStyle>

<ItemStyle CssClass="Normal">
</ItemStyle>

<ItemTemplate>
				<asp:Label ID="lblPortal" Runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "PortalName") %>'></asp:Label>
			
</ItemTemplate>
</asp:TemplateColumn>
<asp:TemplateColumn HeaderText="Portal Aliases">
<HeaderStyle CssClass="NormalBold">
</HeaderStyle>

<ItemStyle CssClass="Normal">
</ItemStyle>

<ItemTemplate>
				<asp:Label ID="lblPortalAliases" Runat="server" Text='<%# FormatPortalAliases(Convert.ToInt32(DataBinder.Eval(Container.DataItem, "PortalID"))) %>'></asp:Label>
			
</ItemTemplate>
</asp:TemplateColumn>
<asp:BoundColumn DataField="Users" HeaderText="Users">
<HeaderStyle HorizontalAlign="Center" CssClass="NormalBold">
</HeaderStyle>

<ItemStyle HorizontalAlign="Center" CssClass="Normal">
</ItemStyle>
</asp:BoundColumn>
<asp:BoundColumn DataField="HostSpace" HeaderText="DiskSpace">
<HeaderStyle CssClass="NormalBold">
</HeaderStyle>

<ItemStyle CssClass="Normal">
</ItemStyle>
</asp:BoundColumn>
<asp:BoundColumn DataField="HostFee" HeaderText="HostingFee" DataFormatString="{0:0.00}">
<HeaderStyle CssClass="NormalBold">
</HeaderStyle>

<ItemStyle CssClass="Normal">
</ItemStyle>
</asp:BoundColumn>
<asp:TemplateColumn HeaderText="Expires">
<HeaderStyle CssClass="NormalBold">
</HeaderStyle>

<ItemTemplate>
				<asp:Label runat="server" Text='<%#FormatExpiryDate(Convert.ToDateTime(DataBinder.Eval(Container.DataItem, "ExpiryDate"))) %>' CssClass="Normal" ID="Label1"/>
			
</ItemTemplate>
</asp:TemplateColumn>
</Columns>
</asp:datagrid>
