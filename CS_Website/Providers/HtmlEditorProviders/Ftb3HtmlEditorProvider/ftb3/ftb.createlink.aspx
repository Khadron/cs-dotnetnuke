<%@ Page Language="C#" ValidateRequest="false" Trace="false" AutoEventWireup="true" Inherits="DotNetNuke.HtmlEditor.FTBCreateLink" %>
<%@ Register TagPrefix="Portal" TagName="URL" Src="~/controls/URLControl.ascx" %>
<%@ Import Namespace="DotNetNuke.UI.Utilities" %>
<%@ Import Namespace="DotNetNuke.Services.Localization" %>
<html xmlns="http://www.w3.org/1999/xhtml">
	<head runat="server">
		<title><%= Title %></title>
		<link rel="stylesheet" type="text/css" href="FTBPopUp.css" />
		<script type='text/javascript'>
			
		function insertLink() {
			ftb = document.getElementById('TargetFreeTextBox').value;
			myOption = -1;
			var x=document.getElementsByName("ctlURL:optType");
			for (i=x.length-1; i > -1; i--) {
				if (x[i].checked) {
					myOption = i;
				}
			}
			switch(myOption)
			{
				case 0:
					URLexisting = document.getElementById('ctlURL_cboUrls');
					if (!URLexisting)
						href = document.getElementById('ctlURL_txtUrl').value;
					else
						href = document.getElementById('ctlURL_cboUrls').value;
					break    
				case 1:
					href = document.getElementById('DNNDomainNameTabid').value + document.getElementById('ctlURL_cboTabs').value;
					break
				case 2:
					var mylist=document.getElementById("ctlURL_cboFiles")
					href = document.getElementById('DNNDomainNameFilePath').value + document.getElementById('ctlURL_cboFolders').value + mylist.options[mylist.selectedIndex].text;
					break
				default:
					alert('<%=ClientAPI.GetSafeJSString(Localization.GetString("InsertLink", LocalResourceFile))%>');
					return false;
			}
			if (href == '' || href == 'http://') {		
					alert('<%=ClientAPI.GetSafeJSString(Localization.GetString("InsertLink", LocalResourceFile))%>');
				return false;	
			}
			window.opener.FTB_API[ftb].InsertLink(href, document.getElementById('ctlURL_chkNewWindow').checked);	
			window.close();
		}
		</script>
	</head>
	<body>
		<form id="Form1" runat="server" enctype="multipart/form-data">
			<asp:placeholder id="phHidden" runat="server"></asp:placeholder>
			<h3><asp:label id="lblHeader" runat="server" resourcekey="LinkEditor" /></h3>
			<fieldset><legend><asp:label id="lblLegend" runat="server" resourcekey="LinkProperties"/></legend>
			<portal:url id="ctlURL" runat="server" width="250" 
				ShowNewWindow="True" ShowLog="False" ShowTrack="False" ShowUpLoad="False"
			/>
			</fieldset>
			<div class='footer'>
				<button type='button' name='insertLinkButton' id='insertLinkButton' onclick='insertLink();window.close();'><%=Localization.GetString("Ok", LocalResourceFile)%></button>
				<button type='button' name='cancel' id='cancelButton' onclick='window.close();'><%=Localization.GetString("Cancel", LocalResourceFile)%></button></div>
		</form>
		<script type='text/javascript'>
			function load() {
				ftb = window.launchParameters['ftb'];
			}
		</script>
	</body>
</html>