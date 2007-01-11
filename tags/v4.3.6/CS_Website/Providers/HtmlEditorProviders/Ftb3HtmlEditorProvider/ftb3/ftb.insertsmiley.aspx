<%@ Page Language="C#" ValidateRequest="false" Trace="false" AutoEventWireup="true" Inherits="DotNetNuke.HtmlEditor.FTBInsertSmiley" %>
<%@ Import Namespace="DotNetNuke.UI.Utilities" %>
<%@ Import Namespace="DotNetNuke.Services.Localization" %>
<html xmlns="http://www.w3.org/1999/xhtml">
	<head runat="server">
		<title><%= Title %></title>
		<link rel="stylesheet" type="text/css" href="FTBPopUp.css" />
		<script type='text/javascript'>
			
		function insertSmiley(imageUrl) {
			ftb = document.getElementById('TargetFreeTextBox').value;
			src = imageUrl;
			img = '<img src="' + src + '" border="0" />';
			window.opener.FTB_API[ftb].InsertHtml(img);
			window.close();
		}
		</script>
	</head>
	<body>
		<form id="Form1" runat="server" enctype="multipart/form-data">
			<asp:placeholder id="phHidden" runat="server"></asp:placeholder>
			<h3><asp:label id="lblHeader" runat="server" resourcekey="InsertSmiley" /></h3>
			<div class="smileyTable">
				<asp:dataList id="lstSmileys" runat="server" datafield="Name"
						RepeatColumns="12" RepeatDirection="Horizontal" RepeatLayout="Table">
					<ItemStyle CssClass="smiley"></ItemStyle>
					<ItemTemplate>
						<img src='<%# FormatUrl(Convert.ToString(DataBinder.Eval(Container.DataItem, "Name"))) %>' 
							onclick="insertSmiley('<%# FormatUrl(Convert.ToString(DataBinder.Eval(Container.DataItem, "Name"))) %>'); window.close();"
							style="cursor:pointer;" alt="FTBInsertSmiley"
							/>
					</ItemTemplate>
				</asp:datalist>
			</div>
		</form>
		<script type='text/javascript'>
			function load() {
				ftb = window.launchParameters['ftb'];
			}
		</script>
	</body>
</html>