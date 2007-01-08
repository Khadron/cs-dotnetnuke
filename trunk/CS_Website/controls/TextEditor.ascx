<%@ Control Language="C#" AutoEventWireup="true"  Inherits="DotNetNuke.UI.UserControls.TextEditor" %>
<table id="tblTextEditor" runat="server" border="0" cellpadding="2" cellspacing="2" summary="Edit HTML Design Table">
	<tr vAlign="top">
		<td align="center"><asp:panel id="pnlOption" Visible="True" Runat="server">
				<asp:RadioButtonList ID="optView" runat="server" AutoPostBack="True" CssClass="NormalTextBox" OnSelectedIndexChanged="optView_SelectedIndexChanged" RepeatDirection="Horizontal"></asp:RadioButtonList>
			</asp:panel></td>
	</tr>
	<tr vAlign="top">
        <td id="celTextEditor" runat="Server">
            <asp:Panel ID="pnlBasicTextBox" runat="server" Visible="False">
				<asp:TextBox id="txtDesktopHTML" CssClass="NormalTextBox" runat="server" textmode="multiline" rows="12" width="600" columns="75"></asp:TextBox>
				<br/>
				<asp:Panel id="pnlBasicRender" Runat="server" Visible="True">
					<asp:RadioButtonList ID="optRender" runat="server" AutoPostBack="True" CssClass="NormalTextBox" OnSelectedIndexChanged="optRender_SelectedIndexChanged" RepeatDirection="Horizontal"></asp:RadioButtonList>
				</asp:Panel>
			</asp:Panel><asp:panel id="pnlRichTextBox" Visible="False" Runat="server">
				<asp:PlaceHolder id="plcEditor" runat="server"></asp:PlaceHolder>
			</asp:panel></td>
	</tr>
</table>
