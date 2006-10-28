<%@ Page Language="C#" AutoEventWireup="true" Inherits="DotNetNuke.Services.Exceptions.ErrorPage" CodeFile="ErrorPage.aspx.cs" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">
<HTML LANG="en-US">
    <HEAD>
        <TITLE runat="server" id="Title">Error</TITLE> 
        <LINK id="StyleSheet" runat="server" href="Portals/_default/portal.css" type="text/css" rel="stylesheet"/>
        <script src="js/dnncore.js"></script>
    </HEAD>
    <BODY id="Body" runat="server" onscroll="__dnn_bodyscroll()" bottommargin="0" leftmargin="0"
        topmargin="0" rightmargin="0" marginwidth="0" marginheight="0">
        <FORM id="Form" runat="server" enctype="multipart/form-data">
            <ASP:PLACEHOLDER id="ErrorPlaceHolder" runat="server" />
        </FORM>
    </BODY>
</HTML>
