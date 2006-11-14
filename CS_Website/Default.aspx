<%@ Page Language="C#" AutoEventWireup="true" Explicit="true" Inherits="DotNetNuke.Framework.DefaultPage" CodeFile="Default.aspx.cs" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Common.Controls" Assembly="DotNetNuke" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.1//EN""http://www.w3.org/TR/xhtml11/DTD/xhtml11.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head" runat="server">
    <meta id="metaRefresh" runat="server" name="Refresh" />
    <meta id="metaDescription" runat="server" name="DESCRIPTION" />
    <meta id="metaKeywords" runat="server" name="KEYWORDS" />
    <meta id="metaCopyright" runat="server" name="COPYRIGHT" />
    <meta id="metaGenerator" runat="server" name="GENERATOR" />
    <meta id="metaAuthor" runat="server" name="AUTHOR" />
    <meta name="RESOURCE-TYPE" content="DOCUMENT">
    <meta name="DISTRIBUTION" content="GLOBAL">
    <meta name="ROBOTS" content="INDEX, FOLLOW">
    <meta name="REVISIT-AFTER" content="1 DAYS">
    <meta name="RATING" content="GENERAL">
    <meta http-equiv="PAGE-ENTER" content="RevealTrans(Duration=0,Transition=1)">
    <style id="StylePlaceholder" runat="server"></style>
    <asp:placeholder id="CSS" runat="server"></asp:placeholder>
</head>
<body id="Body" runat="server" bottommargin="0" leftmargin="0" topmargin="0" rightmargin="0" marginwidth="0" marginheight="0">
    <noscript></noscript>    
    <dnn:Form id="FormDefault" runat="server" ENCTYPE="multipart/form-data" style="height: 100%;">
        <asp:Label ID="SkinError" runat="server" CssClass="NormalRed" Visible="False"></asp:Label>
        <asp:PlaceHolder ID="SkinPlaceHolder" runat="server" />
        <input id="ScrollTop" runat="server" name="ScrollTop" type="hidden">
        <input id="__dnnVariable" runat="server" name="__dnnVariable" type="hidden">
    </dnn:Form>
</body>
</html>
