<%@ Control Language="C#" AutoEventWireup="true" Inherits="DotNetNuke.UI.Skins.Controls.Banner" CodeFile="Banner.ascx.cs" %>
<asp:DataList id="lstBanners" runat="server">
	<ItemStyle HorizontalAlign="Center" Width="100%" BorderColor="#000000"></ItemStyle>
	<ItemTemplate>
		<asp:Label ID="lblItem" Runat="server" Text='<%# FormatItem(Convert.ToInt32(DataBinder.Eval(Container.DataItem,"VendorId")),Convert.ToInt32(DataBinder.Eval(Container.DataItem,"BannerId")),Convert.ToInt32(DataBinder.Eval(Container.DataItem,"BannerTypeId")),Convert.ToString(DataBinder.Eval(Container.DataItem,"BannerName")),
		Convert.ToString(DataBinder.Eval(Container.DataItem,"ImageFile")),Convert.ToString(DataBinder.Eval(Container.DataItem,"Description")),
		Convert.ToString(DataBinder.Eval(Container.DataItem,"Url")),Convert.ToInt32(DataBinder.Eval(Container.DataItem,"Width")),
		Convert.ToInt32(DataBinder.Eval(Container.DataItem,"Height"))) %>'></asp:Label>
	</ItemTemplate>
</asp:DataList>
