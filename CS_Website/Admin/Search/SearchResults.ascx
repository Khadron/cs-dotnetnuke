<%@ Control AutoEventWireup="true" CodeFile="SearchResults.ascx.cs" Explicit="True" Inherits="DotNetNuke.Modules.SearchResults.SearchResults" Language="C#" %>
<asp:DataGrid ID="dgResults" runat="server" AllowPaging="True" AutoGenerateColumns="False" BorderStyle="None" CellPadding="4" GridLines="None" OnPageIndexChanged="dgResults_PageIndexChanged"
    PagerStyle-CssClass="NormalBold" ShowHeader="False">
    <Columns>
        <asp:TemplateColumn>
            <ItemTemplate>
                <asp:Label ID="lblNo" runat="server" CssClass="SubHead" Text='<%# Convert.ToInt32(DataBinder.Eval(Container, "ItemIndex")) + 1 %>'>
                </asp:Label>
            </ItemTemplate>
        </asp:TemplateColumn>
        <asp:TemplateColumn>
            <ItemTemplate>
                <asp:HyperLink ID="lnkTitle" runat="server" CssClass="SubHead" NavigateUrl='<%# FormatURL(Convert.ToInt32(DataBinder.Eval(Container.DataItem,"TabId")),Convert.ToString(DataBinder.Eval(Container.DataItem,"Guid"))) %>'
                    Text='<%# DataBinder.Eval(Container.DataItem, "Title") %>'>
                </asp:HyperLink>&nbsp;-
                <asp:Label ID="lblRelevance" runat="server" CssClass="Normal" Text='<%# FormatRelevance(Convert.ToInt32(DataBinder.Eval(Container.DataItem, "Relevance"))) %>'>
                </asp:Label><br>
                <asp:Label ID="lblSummary" runat="server" CssClass="Normal" Text='<%# Convert.ToString(DataBinder.Eval(Container.DataItem, "Description")) + "<br>" %>' Visible="<%# Convert.ToBoolean(ShowDescription()) %>">
                </asp:Label>
                <asp:HyperLink ID="lnkLink" runat="server" CssClass="CommandButton" NavigateUrl='<%# FormatURL(Convert.ToInt32(DataBinder.Eval(Container.DataItem,"TabId")),Convert.ToString(DataBinder.Eval(Container.DataItem,"Guid"))) %>'
                    Text='<%# FormatURL(Convert.ToInt32(DataBinder.Eval(Container.DataItem,"TabId")),Convert.ToString(DataBinder.Eval(Container.DataItem,"Guid"))) %>'>
                </asp:HyperLink>&nbsp;-
                <asp:Label ID="lblPubDate" runat="server" CssClass="Normal" Text='<%# FormatDate(Convert.ToDateTime(DataBinder.Eval(Container.DataItem, "PubDate"))) %>'>
                </asp:Label>
            </ItemTemplate>
        </asp:TemplateColumn>
    </Columns>
    <PagerStyle CssClass="NormalBold" Mode="NumericPages" />
</asp:DataGrid>
