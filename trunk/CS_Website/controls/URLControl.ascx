<%@ Control AutoEventWireup="true" Codebehind="URLControl.ascx.cs" Explicit="True" Inherits="DotNetNuke.UI.UserControls.UrlControl" Language="C#" %>
<table border="0" cellpadding="0" cellspacing="0">
    <tr id="TypeRow" runat="server">
        <td nowrap="nowrap">
            <br>
            <asp:Label ID="lblURLType" runat="server" CssClass="NormalBold" EnableViewState="False" resourcekey="Type"></asp:Label><br>
            <asp:RadioButtonList ID="optType" runat="server" AutoPostBack="True" CssClass="NormalBold" RepeatDirection="Vertical">
            </asp:RadioButtonList><br>
        </td>
    </tr>
    <tr id="URLRow" runat="server">
        <td nowrap="nowrap">
            <asp:Label ID="lblURL" runat="server" CssClass="NormalBold" EnableViewState="False" resourcekey="URL"></asp:Label><asp:DropDownList ID="cboUrls" runat="server" CssClass="NormalTextBox"
                DataTextField="Url" DataValueField="Url" Width="300">
            </asp:DropDownList><asp:TextBox ID="txtUrl" runat="server" CssClass="NormalTextBox" Width="300"></asp:TextBox><br>
            <asp:LinkButton ID="cmdSelect" runat="server" CausesValidation="False" CssClass="CommandButton" resourcekey="Select">Select</asp:LinkButton><asp:LinkButton ID="cmdDelete"
                runat="server" CausesValidation="False" CssClass="CommandButton" resourcekey="Delete">Delete</asp:LinkButton><asp:LinkButton ID="cmdAdd" runat="server" CausesValidation="False"
                    CssClass="CommandButton" resourcekey="Add">Add</asp:LinkButton></td>
    </tr>
    <tr id="TabRow" runat="server">
        <td nowrap="nowrap">
            <asp:Label ID="lblTab" runat="server" CssClass="NormalBold" EnableViewState="False" resourcekey="Tab"></asp:Label><asp:DropDownList ID="cboTabs" runat="server" CssClass="NormalTextBox"
                DataTextField="TabName" DataValueField="TabId" Width="300">
            </asp:DropDownList></td>
    </tr>
    <tr id="FileRow" runat="server">
        <td nowrap="nowrap">
            <asp:Label ID="lblFolder" runat="server" CssClass="NormalBold" EnableViewState="False" resourcekey="Folder"></asp:Label><asp:DropDownList ID="cboFolders" runat="server"
                AutoPostBack="True" CssClass="NormalTextBox" Width="300">
            </asp:DropDownList>
            <asp:Image ID="imgStorageLocationType" runat="server" Visible="False" /><br>
            <asp:Label ID="lblFile" runat="server" CssClass="NormalBold" EnableViewState="False" resourcekey="File"></asp:Label><asp:DropDownList ID="cboFiles" runat="server"
                CssClass="NormalTextBox" DataTextField="Text" DataValueField="Value" Width="300">
            </asp:DropDownList><input id="txtFile" runat="server" name="txtFile" size="30" type="file" width="300">
            <br>
            <asp:LinkButton ID="cmdUpload" runat="server" CausesValidation="False" CssClass="CommandButton" resourcekey="Upload">Upload</asp:LinkButton><asp:LinkButton ID="cmdSave"
                runat="server" CausesValidation="False" CssClass="CommandButton" resourcekey="Save">Save</asp:LinkButton><asp:LinkButton ID="cmdCancel" runat="server" CausesValidation="False"
                    CssClass="CommandButton" resourcekey="Cancel">Cancel</asp:LinkButton></td>
    </tr>
    <tr id="UserRow" runat="server">
        <td nowrap="nowrap">
            <asp:Label ID="lblUser" runat="server" CssClass="NormalBold" EnableViewState="False" resourcekey="User"></asp:Label>
            <asp:TextBox ID="txtUser" runat="server" CssClass="NormalTextBox" Width="300"></asp:TextBox>
        </td>
    </tr>
    <tr id="ErrorRow" runat="server">
        <td>
            <asp:Label ID="lblMessage" runat="server" CssClass="NormalRed" EnableViewState="False"></asp:Label><br>
        </td>
    </tr>
    <tr>
        <td nowrap="nowrap">
            <asp:CheckBox ID="chkTrack" runat="server" CssClass="NormalBold" resourcekey="Track" Text="Track?" TextAlign="Right" /><asp:CheckBox ID="chkLog" runat="server" CssClass="NormalBold"
                resourcekey="Log" Text="Log?" TextAlign="Right" /><asp:CheckBox ID="chkNewWindow" runat="server" CssClass="NormalBold" resourcekey="NewWindow" Text="New Window?"
                    TextAlign="Right" Visible="False" /><br>
        </td>
    </tr>
</table>
