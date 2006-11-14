<%@ Page Language="C#" ValidateRequest="false" Trace="false" AutoEventWireup="true" Inherits="DotNetNuke.HtmlEditor.FTBImageGallery" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.HtmlEditor" Assembly="DotNetNuke.Ftb3HtmlEditorProvider" %>
<html>
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
