<%@ Control Language="C#" AutoEventWireup="true" Explicit="True" Inherits="DotNetNuke.UI.UserControls.TextEditor" %>
<table cellSpacing="2" cellPadding="2" summary="Edit HTML Design Table" border="0">
	<tr vAlign="top">
		<td align="center"><asp:panel id="pnlOption" Visible="True" Runat="server">
				<asp:RadioButtonList id="optView" Runat="server" AutoPostBack="True" RepeatDirection="Horizontal" CssClass="NormalTextBox"></asp:RadioButtonList>
			</asp:panel></td>
	</tr>
	<tr vAlign="top">
		<td><asp:panel id="pnlBasicTextBox" Visible="False" Runat="server">
				<asp:TextBox id="txtDesktopHTML" CssClass="NormalTextBox" runat="server" textmode="multiline"
					rows="12" width="600" columns="75"></asp:TextBox>
				<BR>
				<asp:Panel id="pnlBasicRender" Runat="server" Visible="True">
					<asp:RadioButtonList id="optRender" Runat="server" AutoPostBack="True" RepeatDirection="Horizontal" CssClass="NormalTextBox"></asp:RadioButtonList>
				</asp:Panel>
			</asp:panel><asp:panel id="pnlRichTextBox" Visible="False" Runat="server">
				<asp:PlaceHolder id="plcEditor" runat="server"></asp:PlaceHolder>
			</asp:panel></td>
	</tr>
</table>
