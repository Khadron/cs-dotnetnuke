<%@ Page Language="C#" ValidateRequest="false" Trace="false" AutoEventWireup="true" Inherits="DotNetNuke.HtmlEditor.FTBImageGallery" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.HtmlEditor" Assembly="DotNetNuke.Ftb3HtmlEditorProvider" %>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title>
		<%=Title%>
	</title>
</head>
<body>
    <form id="Form1" runat="server" enctype="multipart/form-data">  
		<dnn:DNNImageGallery id="imgGallery" runat="Server" />
	</form>
</body>
</html>
