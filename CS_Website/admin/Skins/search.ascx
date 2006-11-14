<%@ Control Language="C#" CodeFile="Search.ascx.cs" AutoEventWireup="true" Explicit="True"
    Inherits="DotNetNuke.UI.Skins.Controls.Search" %>
<asp:TextBox ID="txtSearch" runat="server" CssClass="NormalTextBox" Columns="20"
    MaxLength="255" EnableViewState="False"></asp:TextBox>&nbsp;<asp:LinkButton ID="cmdSearch"
        runat="server" CausesValidation="False" CssClass="SkinObject"></asp:LinkButton>
