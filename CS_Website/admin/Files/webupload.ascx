<%@ Control Language="C#" AutoEventWireup="false" Explicit="True" Inherits="DotNetNuke.Modules.Admin.FileSystem.WebUpload" CodeFile="WebUpload.ascx.cs" %>
<table class="Settings" cellspacing="2" cellpadding="2" summary="Web Upload Design Table">
    <tr>
        <td valign="top" width="560">
            <asp:Panel ID="pnlUpload" Visible="True" CssClass="WorkPanel" runat="server">
                <table class="Settings" id="tblUpload" cellspacing="2" cellpadding="2"
                    summary="Web Upload Design Table" runat="server">
                    <tr>
                        <td class="Head" colspan="2" align="center">
                            <asp:Label ID="lblUploadType" runat="server"></asp:Label><br>
                            <hr>
                        </td>
                    </tr>
                    <tr id="trRoot" runat="server" visible="false">
                        <td width="100">
                            <asp:Label ID="lblRootType" runat="server" CssClass="SubHead"></asp:Label></td>
                        <td width="425">
                            <asp:Label ID="lblRootFolder" runat="server" CssClass="Normal"></asp:Label></td>
                    </tr>
                    <tr>
                        <td colspan="2">
                            &nbsp;</td>
                    </tr>
                    <tr>
                        <td align="center" colspan="2">
                            <label style="display: none" for="<%=cmdBrowse.ClientID%>">Browse Files</label>
                            <input id="cmdBrowse" type="file" size="50" name="cmdBrowse" runat="server">&nbsp;&nbsp;
                            <asp:LinkButton ID="cmdAdd" runat="server" CssClass="CommandButton" Resourcekey="cmdUpload">Add</asp:LinkButton>
                        </td>
                    </tr>
                    <tr id="trFolders" runat="server" visible="false">
                        <td align="center" colspan="2">
                            <asp:DropDownList ID="ddlFolders" runat="server" Width="525px">
                                <asp:ListItem Value="Root">Root</asp:ListItem>
                                <asp:ListItem Value="Files\">Files</asp:ListItem>
                                <asp:ListItem Value="Files\SubFolder\">Files\SubFolder</asp:ListItem>
                            </asp:DropDownList></td>
                    </tr>
                    <tr id="trUnzip" runat="server" visible="false">
                        <td colspan="2">
                            <asp:CheckBox ID="chkUnzip" runat="server" CssClass="Normal" TextAlign="Right" Text="Decompress ZIP Files?"
                                resourcekey="Decompress"></asp:CheckBox></td>
                    </tr>
                    <tr>
                        <td align="left" colspan="2">
                            <asp:Label ID="lblMessage" runat="server" CssClass="Normal" Width="500px" EnableViewState="False"></asp:Label></td>
                    </tr>
                </table>
                <br>
                <table id="tblLogs" cellspacing="0" cellpadding="0" summary="Resource Upload Logs Table"
                    runat="server" visible="False">
                    <tr>
                        <td>
                            <asp:Label ID="lblLogTitle" runat="server" resourcekey="LogTitle">Resource Upload Logs</asp:Label></td>
                    </tr>
                    <tr>
                        <td>
                            <asp:LinkButton ID="cmdReturn1" runat="server" CssClass="CommandButton" resourcekey="cmdReturn">Return to File Manager</asp:LinkButton></td>
                    </tr>
                    <tr>
                        <td>
                            &nbsp;</td>
                    </tr>
                    <tr>
                        <td>
                            <asp:PlaceHolder ID="phPaLogs" runat="server"></asp:PlaceHolder>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            &nbsp;</td>
                    </tr>
                    <tr>
                        <td>
                            <asp:LinkButton ID="cmdReturn2" runat="server" CssClass="CommandButton" resourcekey="cmdReturn">Return to File Manager</asp:LinkButton></td>
                    </tr>
                </table>
            </asp:Panel>
        </td>
    </tr>
</table>
