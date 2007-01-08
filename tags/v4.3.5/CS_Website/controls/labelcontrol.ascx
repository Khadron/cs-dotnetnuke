<%@ Control Language="C#" AutoEventWireup="true" Inherits="DotNetNuke.UI.UserControls.LabelControl" targetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<label id=label runat="server">
  <asp:linkbutton id=cmdHelp tabindex="-1" runat="server" CausesValidation="False" enableviewstate="False">
    <asp:image id="imgHelp" tabindex="-1" runat="server" imageurl="~/images/help.gif" enableviewstate="False"></asp:image>
  </asp:linkbutton>
  <asp:label id=lblLabel runat="server" enableviewstate="False"></asp:label>
</label>
<br>
<asp:panel id=pnlHelp runat="server" cssClass="Help" enableviewstate="False">
  <asp:label id=lblHelp runat="server" enableviewstate="False"></asp:label>
</asp:panel>
