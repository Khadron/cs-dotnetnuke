<%@ Control Language="C#" AutoEventWireup="true" Inherits="DotNetNuke.UI.UserControls.HelpButtonControl" targetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<asp:linkbutton id=cmdHelp tabindex="-1" runat="server" CausesValidation="False" enableviewstate="False">
  <asp:image id="imgHelp" tabindex="-1" runat="server" imageurl="~/images/help.gif" enableviewstate="False"></asp:image>
</asp:linkbutton>
<br>
<asp:panel id=pnlHelp runat="server" cssClass="Help" enableviewstate="False">
  <asp:label id=lblHelp runat="server" enableviewstate="False"></asp:label>
</asp:panel>
