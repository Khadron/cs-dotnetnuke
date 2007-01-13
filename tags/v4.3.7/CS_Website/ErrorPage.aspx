<%@ Page AutoEventWireup="true" CodeFile="ErrorPage.aspx.cs" Inherits="DotNetNuke.Services.Exceptions.ErrorPage" Language="C#" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">
<html lang="en-US">
<head>
    <title id="errorTitle" runat="server">Error</title>
    <link id="StyleSheet" runat="server" href="Portals/_default/portal.css" rel="stylesheet" type="text/css" />

    <script type="text/javascript" src="js/dnncore.js"></script>

</head>
<body id="Body" runat="server" bottommargin="0" leftmargin="0" marginheight="0" marginwidth="0" onscroll="__dnn_bodyscroll()" rightmargin="0" topmargin="0">
    <form id="errorForm" runat="server" enctype="multipart/form-data">
        <asp:PlaceHolder ID="ErrorPlaceHolder" runat="server"></asp:PlaceHolder>
    </form>
</body>
</html>


