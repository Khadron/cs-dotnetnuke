<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.UI.WebControls" Assembly="DotNetNuke" %>
<%@ Control Inherits="DotNetNuke.Modules.Admin.Users.UserAccounts" CodeFile="Users.ascx.cs" Language="C#" AutoEventWireup="true" %>
<table width="475" border="0">
    <tr>
        <td align="left" width="75">
            <asp:Label ID="lblSearch" CssClass="SubHead" resourcekey="Search" runat="server">Search:</asp:Label></td>
        <td class="Normal" align="left" width="*">
            <asp:TextBox ID="txtSearch" runat="server"></asp:TextBox>
            <asp:DropDownList ID="ddlSearchType" runat="server" />
            <asp:ImageButton ID="btnSearch" runat="server" ImageUrl="~/images/icon_search_16px.gif" OnClick="btnSearch_Click">
            </asp:ImageButton>
        </td>
    </tr>
    <tr>
        <td colspan="3" height="15">
        </td>
    </tr>
</table>
<asp:Panel ID="plLetterSearch" runat="server" HorizontalAlign="Center">
    <asp:Repeater ID="rptLetterSearch" runat="server">
        <ItemTemplate>
            <asp:HyperLink runat="server" CssClass="CommandButton" NavigateUrl='<%# FilterURL(Container.DataItem.ToString(),"1") %>'
                Text='<%# Container.DataItem %>'>
            </asp:HyperLink>&nbsp;&nbsp;
        </ItemTemplate>
    </asp:Repeater>
</asp:Panel>
<asp:DataGrid ID="grdUsers" AutoGenerateColumns="false" Width="100%" CellPadding="2"
    GridLines="None" CssClass="DataGrid_Container" runat="server" OnDeleteCommand="grdUsers_DeleteCommand" OnItemDataBound="grdUsers_ItemDataBound">
    <HeaderStyle CssClass="NormalBold" VerticalAlign="Top" HorizontalAlign="Center" />
    <ItemStyle CssClass="Normal" HorizontalAlign="Left" />
    <AlternatingItemStyle CssClass="Normal" />
    <EditItemStyle CssClass="NormalTextBox" />
    <SelectedItemStyle CssClass="NormalRed" />
    <FooterStyle CssClass="DataGrid_Footer" />
    <PagerStyle CssClass="DataGrid_Pager" />
    <Columns>
        <dnn:imagecommandcolumn CommandName="Edit" ImageUrl="~/images/edit.gif" EditMode="URL"
            KeyField="UserID" />
        <dnn:imagecommandcolumn commandname="Delete" imageurl="~/images/delete.gif" keyfield="UserID" />
        <dnn:imagecommandcolumn CommandName="UserRoles" ImageUrl="~/images/icon_securityroles_16px.gif"
            EditMode="URL" KeyField="UserID" />
        <asp:TemplateColumn>
            <ItemTemplate>
                <asp:Image ID="imgOnline" runat="Server" ImageUrl="~/images/userOnline.gif" />
            </ItemTemplate>
        </asp:TemplateColumn>
        <dnn:textcolumn datafield="UserName" headertext="Username" />
        <dnn:textcolumn datafield="FirstName" headertext="FirstName" />
        <dnn:textcolumn datafield="LastName" headertext="LastName" />
        <dnn:textcolumn datafield="DisplayName" headertext="DisplayName" />
        <asp:TemplateColumn HeaderText="Address">
            <ItemTemplate>
                <asp:Label ID="lblAddress" runat="server" Text='<%# DisplayAddress(((UserInfo)Container.DataItem).Profile.Unit, 
                ((UserInfo)Container.DataItem).Profile.Street, 
                ((UserInfo)Container.DataItem).Profile.City, 
                ((UserInfo)Container.DataItem).Profile.Region, 
                ((UserInfo)Container.DataItem).Profile.Country, 
                ((UserInfo)Container.DataItem).Profile.PostalCode) %>'>
                </asp:Label>
            </ItemTemplate>
        </asp:TemplateColumn>
        <asp:TemplateColumn HeaderText="Telephone">
            <ItemTemplate>
                <asp:Label ID="Label4" runat="server" Text='<%# DisplayEmail(((UserInfo)Container.DataItem).Profile.Telephone) %>'>
                </asp:Label>
            </ItemTemplate>
        </asp:TemplateColumn>
        <asp:TemplateColumn HeaderText="Email">
            <ItemTemplate>
                <asp:Label ID="lblEmail" runat="server" Text='<%# DisplayEmail(((UserInfo)Container.DataItem).Membership.Email) %>'>
                </asp:Label>
            </ItemTemplate>
        </asp:TemplateColumn>
        <asp:TemplateColumn HeaderText="CreatedDate">
            <ItemTemplate>
                <asp:Label ID="lblLastLogin" runat="server" Text='<%# DisplayDate(((UserInfo)Container.DataItem).Membership.CreatedDate) %>'>
                </asp:Label>
            </ItemTemplate>
        </asp:TemplateColumn>
        <asp:TemplateColumn HeaderText="LastLogin">
            <ItemTemplate>
                <asp:Label ID="Label7" runat="server" Text='<%# DisplayDate(((UserInfo)Container.DataItem).Membership.LastLoginDate) %>'>
                </asp:Label>
            </ItemTemplate>
        </asp:TemplateColumn>
        <asp:TemplateColumn HeaderText="Authorized">
            <ItemTemplate>
                <asp:Image runat="server" ID="imgApproved" ImageUrl="~/images/checked.gif" Visible="<%# ((UserInfo)Container.DataItem).Membership.Approved=true%>" />
                <asp:Image runat="server" ID="imgNotApproved" ImageUrl="~/images/unchecked.gif" Visible="<%# ((UserInfo)Container.DataItem).Membership.Approved=false%>" />
            </ItemTemplate>
        </asp:TemplateColumn>
    </Columns>
</asp:DataGrid>
<br/>
<br/>
<dnn:PagingControl ID="ctlPagingControl" runat="server"></dnn:PagingControl>
