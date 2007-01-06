<%@ Control Language="C#" AutoEventWireup="true"  Inherits="DotNetNuke.UI.UserControls.Help" %>
<div align="left">
	<asp:Label ID="lblHelp" Runat="server" CssClass="Normal" Width="100%" enableviewstate="False"></asp:Label>
    <br><br>
    <asp:linkbutton id="cmdCancel" runat="server" class="CommandButton" resourcekey="cmdCancel" borderstyle="none" text="Cancel" causesvalidation="False" enableviewstate="False"></asp:linkbutton>
    &nbsp;&nbsp;
    <asp:HyperLink id="cmdHelp" Runat="server" CssClass="CommandButton" resourcekey="cmdHelp" Target="_new" Text="View Online Help" enableviewstate="False"></asp:HyperLink>
</div>
