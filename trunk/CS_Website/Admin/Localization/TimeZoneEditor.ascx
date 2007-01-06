<%@ Import Namespace="System.Data" %>
<%@ Control AutoEventWireup="true" CodeFile="TimeZoneEditor.ascx.cs" Inherits="DotNetNuke.Services.Localization.TimeZoneEditor" Language="C#" %>
<%@ Register Src="~/controls/LabelControl.ascx" TagName="Label" TagPrefix="dnn" %>

<table id="Table1" border="0" cellpadding="1" cellspacing="1">
    <tr>
        <td class="SubHead" valign="top">
            <dnn:Label ID="lbLocales" runat="server" ControlName="cboLocales" Text="Available Locales" />
        </td>
        <td valign="top">
            <asp:DropDownList ID="cboLocales" runat="server" AutoPostBack="True" DataTextField="name" DataValueField="key" OnSelectedIndexChanged="cboLocales_SelectedIndexChanged"
                Width="300px">
            </asp:DropDownList></td>
    </tr>
</table>

<p>
    <asp:Panel ID="pnlMissing" runat="server" Visible="False" Wrap="True">
        <asp:Label ID="lblMissing" runat="server">System Default resource file contains some entries not present in current localized file. This can lead to some values not being translated.</asp:Label>
        <br/>
        <asp:LinkButton ID="cmdAddMissing" runat="server" CausesValidation="false" CssClass="CommandButton" OnClick="cmdAddMissing_Click" resourcekey="cmdAddMissing">Add Missing Entries</asp:LinkButton>
    </asp:Panel>
</p>
<p>
    <asp:DataGrid ID="dgEditor" runat="server" AutoGenerateColumns="False" CellPadding="3" CellSpacing="1" GridLines="None">
        <ItemStyle VerticalAlign="Top" />
        <HeaderStyle BackColor="Silver" CssClass="subsubhead" Font-Bold="True" />
        <Columns>
            <asp:TemplateColumn HeaderText="Name">
                <ItemTemplate>
                    <asp:TextBox ID="txtName" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.name") %>' Width="300px">
                    </asp:TextBox>
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="txtName" Display="Dynamic" ErrorMessage="<br>Required Field" resourcekey="RequiredField.ErrorMessage"></asp:RequiredFieldValidator>
                </ItemTemplate>
            </asp:TemplateColumn>
            <asp:BoundColumn DataField="key" HeaderText="Offset" ReadOnly="True">
                <ItemStyle CssClass="Normal" HorizontalAlign="Right" />
            </asp:BoundColumn>
            <asp:TemplateColumn HeaderText="DefaultValue">
                <ItemTemplate>
                    <asp:Label ID="Label2" runat="server" CssClass="Normal">
						<%# ((DataRow)Container.DataItem).GetParentRow("defaultvalues")["defaultvalue"] %>
                    </asp:Label>
                </ItemTemplate>
            </asp:TemplateColumn>
        </Columns>
    </asp:DataGrid></p>
<p>
    <asp:LinkButton ID="cmdUpdate" runat="server" CssClass="CommandButton" OnClick="cmdUpdate_Click" resourcekey="cmdUpdate">Update</asp:LinkButton>
    <asp:LinkButton ID="cmdCancel" runat="server" CausesValidation="false" CssClass="CommandButton" OnClick="cmdCancel_Click" resourcekey="cmdCancel">Cancel</asp:LinkButton>
</p>
