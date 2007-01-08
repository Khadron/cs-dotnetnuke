<%@ Control Language="C#" AutoEventWireup="true" Inherits="DotNetNuke.Services.Wizards.WizardSuccess" TargetSchema="http://schemas.microsoft.com/intellisense/ie3-2nav3-0" %>
<table cellspacing="0" cellpadding="0" border="0">
	<tr><td><asp:label id="lblTitle" runat="server" cssclass="SubHead">Congratulations</asp:label></td></tr>
	<tr><td height="15"></td></tr>
	<tr>
		<td>
			<asp:label id="lblDetail" runat="server" cssclass="WizardText">
				<p>
					Congratulations.  The Wizard completed successfully.
				</p>
			</asp:label>
		</td>
	</tr>
	<tr><td height="15"></td></tr>
	<tr><td><asp:label id="lblMessage" runat="server"></asp:label></td></tr>
</table>
