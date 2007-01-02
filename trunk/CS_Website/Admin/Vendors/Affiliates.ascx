<%@ Control Language="C#" AutoEventWireup="true" Inherits="DotNetNuke.Modules.Admin.Vendors.Affiliates" CodeFile="Affiliates.ascx.cs" %>
<asp:DataGrid ID="grdAffiliates" runat="server" Width="100%" Border="0" CellSpacing="3"
    AutoGenerateColumns="false" EnableViewState="true" BorderStyle="None" BorderWidth="0px"
    GridLines="None">
    <Columns>
        <asp:TemplateColumn>
            <ItemStyle Width="20px"></ItemStyle>
            <ItemTemplate>
                <asp:HyperLink NavigateUrl='<%# FormatURL("AffilId",DataBinder.Eval(Container.DataItem,"AffiliateId").ToString()) %>'
                    runat="server" ID="Hyperlink1">
                    <asp:Image ImageUrl="~/images/edit.gif" resourcekey="Edit" AlternateText="Edit" runat="server"
                        ID="Hyperlink1Image" />
                </asp:HyperLink>
            </ItemTemplate>
        </asp:TemplateColumn>
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
        <asp:BoundColumn DataField="CPC" HeaderText="CPC" DataFormatString="{0:#,##0.#####}">
            <HeaderStyle HorizontalAlign="Center" CssClass="NormalBold"></HeaderStyle>
            <ItemStyle HorizontalAlign="Center" CssClass="Normal"></ItemStyle>
        </asp:BoundColumn>
        <asp:BoundColumn DataField="Clicks" HeaderText="Clicks">
            <HeaderStyle HorizontalAlign="Center" CssClass="NormalBold"></HeaderStyle>
            <ItemStyle HorizontalAlign="Center" CssClass="Normal"></ItemStyle>
        </asp:BoundColumn>
        <asp:BoundColumn DataField="CPCTotal" HeaderText="Total" DataFormatString="{0:#,##0.#####}">
            <HeaderStyle HorizontalAlign="Center" CssClass="NormalBold"></HeaderStyle>
            <ItemStyle HorizontalAlign="Center" CssClass="Normal"></ItemStyle>
        </asp:BoundColumn>
        <asp:BoundColumn DataField="CPA" HeaderText="CPA" DataFormatString="{0:#,##0.#####}">
            <HeaderStyle HorizontalAlign="Center" CssClass="NormalBold"></HeaderStyle>
            <ItemStyle HorizontalAlign="Center" CssClass="Normal"></ItemStyle>
        </asp:BoundColumn>
        <asp:BoundColumn DataField="Acquisitions" HeaderText="Acquisitions">
            <HeaderStyle HorizontalAlign="Center" CssClass="NormalBold"></HeaderStyle>
            <ItemStyle HorizontalAlign="Center" CssClass="Normal"></ItemStyle>
        </asp:BoundColumn>
        <asp:BoundColumn DataField="CPATotal" HeaderText="Total" DataFormatString="{0:#,##0.#####}">
            <HeaderStyle HorizontalAlign="Center" CssClass="NormalBold"></HeaderStyle>
            <ItemStyle HorizontalAlign="Center" CssClass="Normal"></ItemStyle>
        </asp:BoundColumn>
    </Columns>
</asp:DataGrid>
<br/>
<asp:HyperLink CssClass="CommandButton" ID="cmdAdd" resourcekey="cmdAdd" runat="server"
    BorderStyle="None">Create New Affiliate</asp:HyperLink>
