<%@ Control Language="C#" AutoEventWireup="true" Inherits="DotNetNuke.Modules.Admin.Vendors.Banners" CodeFile="Banners.ascx.cs" %>
<asp:DataGrid ID="grdBanners" runat="server" Width="100%" Border="0" CellSpacing="3"
    AutoGenerateColumns="false" EnableViewState="true" summary="Edit Vendors Design Table"
    BorderStyle="None" BorderWidth="0px" GridLines="None">
    <Columns>
        <asp:TemplateColumn>
            <ItemStyle Width="20px"></ItemStyle>
            <ItemTemplate>
                <asp:HyperLink NavigateUrl='<%# FormatURL("BannerId",DataBinder.Eval(Container.DataItem,"BannerId").ToString()) %>'
                    runat="server" ID="Hyperlink1">
                    <asp:Image ImageUrl="~/images/edit.gif" resourcekey="Edit" AlternateText="Edit" runat="server"
                        ID="Hyperlink1Image" />
                </asp:HyperLink>
            </ItemTemplate>
        </asp:TemplateColumn>
        <asp:BoundColumn DataField="BannerName" HeaderText="Banner">
            <HeaderStyle CssClass="NormalBold"></HeaderStyle>
            <ItemStyle CssClass="Normal"></ItemStyle>
        </asp:BoundColumn>
        <asp:TemplateColumn HeaderText="Type">
            <HeaderStyle CssClass="NormalBold"></HeaderStyle>
            <ItemStyle CssClass="Normal"></ItemStyle>
            <ItemTemplate>
                <asp:Label ID="lblType" runat="server" Text='<%# DisplayType(Convert.ToInt32(DataBinder.Eval(Container.DataItem, "BannerTypeId"))) %>'></asp:Label>
            </ItemTemplate>
        </asp:TemplateColumn>
        <asp:BoundColumn DataField="GroupName" HeaderText="Group">
            <HeaderStyle CssClass="NormalBold"></HeaderStyle>
            <ItemStyle CssClass="Normal"></ItemStyle>
        </asp:BoundColumn>
        <asp:BoundColumn DataField="Impressions" HeaderText="Impressions">
            <HeaderStyle HorizontalAlign="Center" CssClass="NormalBold"></HeaderStyle>
            <ItemStyle HorizontalAlign="Center" CssClass="Normal"></ItemStyle>
        </asp:BoundColumn>
        <asp:BoundColumn DataField="CPM" HeaderText="CPM" DataFormatString="{0:#,##0.00}">
            <HeaderStyle HorizontalAlign="Center" CssClass="NormalBold"></HeaderStyle>
            <ItemStyle HorizontalAlign="Center" CssClass="Normal"></ItemStyle>
        </asp:BoundColumn>
        <asp:BoundColumn DataField="Views" HeaderText="Views">
            <HeaderStyle HorizontalAlign="Center" CssClass="NormalBold"></HeaderStyle>
            <ItemStyle HorizontalAlign="Center" CssClass="Normal"></ItemStyle>
        </asp:BoundColumn>
        <asp:BoundColumn DataField="ClickThroughs" HeaderText="Clicks">
            <HeaderStyle HorizontalAlign="Center" CssClass="NormalBold"></HeaderStyle>
            <ItemStyle HorizontalAlign="Center" CssClass="Normal"></ItemStyle>
        </asp:BoundColumn>
        <asp:TemplateColumn HeaderText="Start">
            <HeaderStyle CssClass="NormalBold"></HeaderStyle>
            <ItemStyle CssClass="Normal"></ItemStyle>
            <ItemTemplate>
                <asp:Label ID="lblStartDate" runat="server" Text='<%# DisplayDate(Convert.ToDateTime(DataBinder.Eval(Container.DataItem, "StartDate"))) %>'></asp:Label>
            </ItemTemplate>
        </asp:TemplateColumn>
        <asp:TemplateColumn HeaderText="End">
            <HeaderStyle CssClass="NormalBold"></HeaderStyle>
            <ItemStyle CssClass="Normal"></ItemStyle>
            <ItemTemplate>
                <asp:Label ID="lblEndDate" runat="server" Text='<%# DisplayDate(Convert.ToDateTime(DataBinder.Eval(Container.DataItem, "EndDate"))) %>'></asp:Label>
            </ItemTemplate>
        </asp:TemplateColumn>
    </Columns>
</asp:DataGrid>
<br>
<asp:HyperLink CssClass="CommandButton" ID="cmdAdd" resourcekey="cmdAdd" runat="server"
    BorderStyle="none">Create New Banner</asp:HyperLink>
