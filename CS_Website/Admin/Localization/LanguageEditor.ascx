<%@ Control AutoEventWireup="true" CodeFile="LanguageEditor.ascx.cs" Inherits="DotNetNuke.Services.Localization.LanguageEditor" Language="C#" %>
<%@ Register Src="~/controls/LabelControl.ascx" TagName="Label" TagPrefix="dnn" %>
<%@ Register Src="~/controls/SectionHeadControl.ascx" TagName="SectionHead" TagPrefix="dnn" %>
<%@ Register Assembly="DotNetNuke.WebControls" Namespace="DotNetNuke.UI.WebControls" TagPrefix="dnntv" %>
<style type="text/css">.Pending { BORDER-LEFT-COLOR: red; BORDER-BOTTOM-COLOR: red; BORDER-TOP-STYLE: solid; BORDER-TOP-COLOR: red; BORDER-RIGHT-STYLE: solid; BORDER-LEFT-STYLE: solid; BORDER-RIGHT-COLOR: red; BORDER-BOTTOM-STYLE: solid }
	</style>
<table id="Table2" border="0" cellspacing="5" width="100%">
    <tr>
        <td nowrap="nowrap" valign="top">
            <p>
                <asp:Panel ID="Panel1" runat="server" Width="195px">
                    <dnntv:DnnTree ID="DNNTree" runat="server" CssClass="Normal" DefaultNodeCssClass="Normal" DefaultNodeCssClassOver="Normal">
                    </dnntv:DnnTree>
                </asp:Panel>
            </p>
        </td>
        <td valign="top">
            <p>
                <table id="Table1" border="0" cellpadding="1" cellspacing="1">
                    <tr>
                        <td valign="top" width="150">
                            <asp:Label ID="lblSelected" runat="server" CssClass="SubHead" resourcekey="SelectedFile">Selected Resource File:</asp:Label></td>
                        <td valign="top">
                            <asp:Label ID="lblResourceFile" runat="server" CssClass="Normal" Font-Bold="True" Text="Selected Resource File:">Selected Resource File:</asp:Label></td>
                    </tr>
                    <tr>
                        <td nowrap="nowrap">
                        </td>
                        <td nowrap="nowrap">
                            <asp:RadioButtonList ID="rbDisplay" runat="server" AutoPostBack="True" CssClass="Normal" RepeatDirection="Horizontal" RepeatLayout="Flow">
                                <asp:ListItem resourcekey="DisplayEnglish" Value="English">English</asp:ListItem>
                                <asp:ListItem resourcekey="DisplayNative" Selected="True" Value="Native">Native</asp:ListItem>
                            </asp:RadioButtonList></td>
                    </tr>
                    <tr>
                        <td class="SubHead" valign="top" width="150">
                            <dnn:Label ID="lbLocales" runat="server" ControlName="cboLocales" Text="Available Locales" />
                        </td>
                        <td valign="top">
                            <asp:DropDownList ID="cboLocales" runat="server" AutoPostBack="True" CssClass="Normal" DataTextField="name" DataValueField="key" Width="300px">
                            </asp:DropDownList></td>
                    </tr>
                    <tr id="rowMode" runat="server">
                        <td class="SubHead" valign="top" width="150">
                            <dnn:Label ID="lbMode" runat="server" ControlName="cboLocales" Text="Available Locales" />
                        </td>
                        <td valign="top">
                            <asp:RadioButtonList ID="rbMode" runat="server" AutoPostBack="True" CssClass="Normal" RepeatColumns="3" RepeatDirection="Horizontal">
                                <asp:ListItem resourcekey="ModeSystem" Selected="True" Value="System">System</asp:ListItem>
                                <asp:ListItem resourcekey="ModeHost" Value="Host">Host</asp:ListItem>
                                <asp:ListItem resourcekey="ModePortal" Value="Portal">Portal</asp:ListItem>
                            </asp:RadioButtonList></td>
                    </tr>
                    <tr>
                        <td class="SubHead" colspan="2" valign="top">
                            <asp:CheckBox ID="chkHighlight" runat="server" AutoPostBack="True" resourcekey="Highlight" Text="Highlight Pending Translations" TextAlign="Left" /></td>
                    </tr>
                </table>
            </p>
            <p>
                <asp:DataGrid ID="dgEditor" runat="server" AutoGenerateColumns="False" CellPadding="3" CssClass="Normal" GridLines="None">
                    <ItemStyle VerticalAlign="Top" />
                    <HeaderStyle Font-Bold="True" />
                    <Columns>
                        <asp:TemplateColumn>
                            <ItemTemplate>
                                <table border="0" cellpadding="0" cellspacing="2" width="100%">
                                    <tr>
                                        <td bgcolor="silver" colspan="3" width="100%">
                                            <asp:Label ID="Label3" runat="server" CssClass="NormalBold" Font-Bold="True" resourcekey="ResourceName">
												Resource name:</asp:Label>
                                            <asp:Label ID="lblName" runat="server" CssClass="Normal">
												<%# DataBinder.Eval(Container, "DataItem.key") %>
                                            </asp:Label></td>
                                    </tr>
                                    <tr>
                                        <td width="300">
                                            <asp:Label ID="Label4" runat="server" CssClass="NormalBold" Font-Bold="True" resourcekey="Value">
												Localized Value</asp:Label></td>
                                        <td>
                                        </td>
                                        <td width="100%">
                                            <table border="0">
                                                <tr>
                                                    <td>
                                                        <dnn:SectionHead ID="dshDef" runat="server" CssClass="Normal" IncludeRule="False" IsExpanded='<%# ExpandDefault((System.Web.UI.Pair)DataBinder.Eval(Container, "DataItem.value"))  %>'
                                                            Section="divDef" Text="" />
                                                    </td>
                                                    <td>
                                                        <asp:Label ID="Label5" runat="server" CssClass="NormalBold" Font-Bold="True" resourcekey="DefaultValue">
												Default Value</asp:Label></td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td valign="top" width="300">
                                            <asp:TextBox ID="txtValue" runat="server" Height="30px" TextMode="MultiLine" Width="300px"></asp:TextBox></td>
                                        <td nowrap="nowrap" valign="top">
                                            <asp:HyperLink ID="lnkEdit" runat="server" CssClass="CommandButton" NavigateUrl='<%# OpenFullEditor(Convert.ToString(DataBinder.Eval(Container, "DataItem.key"))) %> '>
                                                <asp:Image ID="imgEdit" runat="server" AlternateText="Edit" ImageUrl="~/images/uprt.gif" resourcekey="cmdEdit" />
                                            </asp:HyperLink>&nbsp;
                                        </td>
                                        <td valign="top" width="100%">
                                            <table id="divDef" runat="server" border="0" cellpadding="0" cellspacing="0" width="100%">
                                                <tr>
                                                    <td>
                                                        <asp:Label ID="lblDefault" runat="server" CssClass="Normal"></asp:Label></td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                </table>
                            </ItemTemplate>
                        </asp:TemplateColumn>
                        <asp:BoundColumn DataField="key" Visible="False"></asp:BoundColumn>
                    </Columns>
                </asp:DataGrid></p>
            <p>
                <asp:LinkButton ID="cmdUpdate" runat="server" CssClass="CommandButton" resourcekey="cmdUpdate">Update</asp:LinkButton>&nbsp;<asp:LinkButton ID="cmdCancel" runat="server"
                    CausesValidation="false" CssClass="CommandButton" resourcekey="cmdCancel">Cancel</asp:LinkButton>&nbsp;<asp:LinkButton ID="cmdDelete" runat="server" CausesValidation="false"
                        CssClass="CommandButton" resourcekey="cmdDelete">Delete</asp:LinkButton></p>
        </td>
    </tr>
</table>
