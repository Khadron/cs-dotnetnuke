<%@ Control Language="C#" AutoEventWireup="true"  Inherits="DotNetNuke.UI.Skins.SkinControl" %>
<asp:RadioButton ID="optHost" resourcekey="Host" Runat="server" Text="Host" CssClass="SubHead" Checked="True" AutoPostBack="True" GroupName="SkinControl"></asp:RadioButton>&nbsp;&nbsp;<asp:RadioButton ID="optSite" resourcekey="Site" Runat="server" Text="Site" CssClass="SubHead" Checked="False" AutoPostBack="True" GroupName="SkinControl"></asp:RadioButton>
<br/>
<asp:DropDownList id="cboSkin" runat="server" cssclass="NormalTextBox"></asp:DropDownList>&nbsp;&nbsp;<asp:LinkButton ID="cmdPreview" Runat="server" CssClass="CommandButton" EnableViewState="False" resourcekey="Preview">Preview</asp:LinkButton> 

