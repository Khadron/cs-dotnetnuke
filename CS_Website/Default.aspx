<%@ Page Language="C#" AutoEventWireup="true" Inherits="DotNetNuke.Framework.DefaultPage" CodeFile="Default.aspx.cs" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Common.Controls" Assembly="DotNetNuke" %>
<asp:literal id="skinDocType" runat="server"></asp:literal>

<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head" runat="server">
    <meta id="MetaRefresh" runat="server" name="Refresh" content="" />
    <meta id="MetaDescription" runat="server" name="DESCRIPTION" content="" />
    <meta id="MetaKeywords" runat="server" name="KEYWORDS" content="" />
    <meta id="MetaCopyright" runat="server" name="COPYRIGHT" content="" />
    <meta id="MetaGenerator" runat="server" name="GENERATOR" content="" />
    <meta id="MetaAuthor" runat="server" name="AUTHOR" content="" />
    <meta name="RESOURCE-TYPE" content="DOCUMENT" />
    <meta name="DISTRIBUTION" content="GLOBAL" />
    <meta name="ROBOTS" content="INDEX, FOLLOW" />
    <meta name="REVISIT-AFTER" content="1 DAYS" />
    <meta name="RATING" content="GENERAL" />
    <meta http-equiv="PAGE-ENTER" content="RevealTrans(Duration=0,Transition=1)"/>
    <style type="text/css" id="StylePlaceholder" runat="server"></style>    
    <asp:placeholder id="CSS" runat="server"></asp:placeholder>
    <title>Default Page</title>
</head>
<body id="Body" runat="server" bottommargin="0" leftmargin="0" topmargin="0" rightmargin="0" marginwidth="0" marginheight="0">
    <noscript></noscript>    
    <dnn:Form id="Form" runat="server" ENCTYPE="multipart/form-data" style="height: 100%;">    
        <asp:Label ID="SkinError" runat="server" CssClass="NormalRed" Visible="False"></asp:Label>
        <asp:PlaceHolder ID="SkinPlaceHolder" runat="server" />
        <input id="ScrollTop" runat="server" name="ScrollTop" type="hidden"/>
        <input id="__dnnVariable" runat="server" name="__dnnVariable" type="hidden"/>
    </dnn:Form>
</body>
</html>
