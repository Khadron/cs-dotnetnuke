<%@ Control Language="C#" AutoEventWireup="true"  Inherits="DotNetNuke.Modules.Admin.Portals.Portals" CodeFile="Portals.ascx.cs" %>
<%@ Register Assembly="DotNetNuke" Namespace="DotNetNuke.UI.WebControls" TagPrefix="dnn" %>
<asp:Panel ID="plLetterSearch" runat="server" HorizontalAlign="Center">
    <asp:Repeater ID="rptLetterSearch" runat="server">
        <ItemTemplate>
            <asp:HyperLink ID="HyperLink1" runat="server" CssClass="CommandButton" NavigateUrl='<%# FilterURL((string)Container.DataItem,"1") %>' Text='<%# Container.DataItem %>'>
            </asp:HyperLink>&nbsp;&nbsp;
        </ItemTemplate>
    </asp:Repeater>
</asp:Panel>
<asp:DataGrid ID="grdPortals" runat="server" AutoGenerateColumns="false" CellPadding="2" CssClass="DataGrid_Container" GridLines="None" Width="100%" OnDeleteCommand="grdPortals_DeleteCommand">
    <HeaderStyle CssClass="NormalBold" HorizontalAlign="Center" VerticalAlign="Top" />
    <ItemStyle CssClass="Normal" HorizontalAlign="Center" />
    <AlternatingItemStyle CssClass="Normal" />
    <EditItemStyle CssClass="NormalTextBox" />
    <SelectedItemStyle CssClass="NormalRed" />
    <FooterStyle CssClass="DataGrid_Footer" />
    <PagerStyle CssClass="DataGrid_Pager" />
    <Columns>
        <dnn:imagecommandcolumn commandname="Edit" editmode="URL" imageurl="~/images/edit.gif" keyfield="PortalID">
</dnn:imagecommandcolumn>
        <dnn:imagecommandcolumn commandname="Delete" imageurl="~/images/delete.gif" keyfield="PortalID">
</dnn:imagecommandcolumn>
        <asp:TemplateColumn HeaderText="Title">
            <ItemStyle HorizontalAlign="Left" />
            <ItemTemplate>
                <asp:Label ID="lblPortal" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "PortalName") %>'></asp:Label>
            </ItemTemplate>
        </asp:TemplateColumn>
        <asp:TemplateColumn HeaderText="Portal Aliases">
            <ItemStyle HorizontalAlign="Left" />
            <ItemTemplate>
                <asp:Label ID="lblPortalAliases" runat="server" Text='<%# FormatPortalAliases(Convert.ToInt32(DataBinder.Eval(Container.DataItem, "PortalID"))) %>'></asp:Label>
            </ItemTemplate>
        </asp:TemplateColumn>
        <dnn:textcolumn datafield="Users" headertext="Users">
</dnn:textcolumn>
        <dnn:textcolumn datafield="Pages" headertext="Pages">
</dnn:textcolumn>
        <dnn:textcolumn datafield="HostSpace" headertext="DiskSpace">
</dnn:textcolumn>
        <asp:BoundColumn DataField="HostFee" DataFormatString="{0:0.00}" HeaderText="HostingFee"></asp:BoundColumn>
        <asp:TemplateColumn HeaderText="Expires">
            <HeaderStyle CssClass="NormalBold" />
            <ItemTemplate>
                <asp:Label ID="Label1" runat="server" CssClass="Normal" Text='<%#FormatExpiryDate(Convert.ToDateTime( DataBinder.Eval(Container.DataItem, "ExpiryDate"))) %>'></asp:Label>
            </ItemTemplate>
        </asp:TemplateColumn>
    </Columns>
</asp:DataGrid>
<br>
<br>
<dnn:pagingcontrol id="ctlPagingControl" runat="server"></dnn:pagingcontrol>
