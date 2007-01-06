<%@ Control Language="C#" AutoEventWireup="true"  Inherits="DotNetNuke.UI.UserControls.TextEditor" %>
<table cellSpacing="2" cellPadding="2" summary="Edit HTML Design Table" border="0">
	<tr vAlign="top">
		<td align="center"><asp:panel id="pnlOption" Visible="True" Runat="server">
				<asp:RadioButtonList ID="optView" runat="server" AutoPostBack="True" CssClass="NormalTextBox" OnSelectedIndexChanged="optView_SelectedIndexChanged" RepeatDirection="Horizontal"></asp:RadioButtonList>
			</asp:panel></td>
	</tr>
	<tr vAlign="top">
		<td><asp:panel id="pnlBasicTextBox" Visible="False" Runat="server">
				<asp:TextBox id="txtDesktopHTML" CssClass="NormalTextBox" runat="server" textmode="multiline" rows="12" width="600" columns="75"></asp:TextBox>
				<br/>
				<asp:Panel id="pnlBasicRender" Runat="server" Visible="True">
					<asp:RadioButtonList ID="optRender" runat="server" AutoPostBack="True" CssClass="NormalTextBox" OnSelectedIndexChanged="optRender_SelectedIndexChanged" RepeatDirection="Horizontal"></asp:RadioButtonList>
				</asp:Panel>
			</asp:panel><asp:panel id="pnlRichTextBox" Visible="False" Runat="server">
				<asp:PlaceHolder id="plcEditor" runat="server"></asp:PlaceHolder>
			</asp:panel></td>
	</tr>
</table>
