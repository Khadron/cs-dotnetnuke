<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Control Language="C#" AutoEventWireup="true" Explicit="True" Inherits="DotNetNuke.Modules.Admin.Skins.EditSkins" CodeFile="EditSkins.ascx.cs" %>
<center>
	<table cellSpacing="0" cellPadding="0" width="500" border="0">
		<tr>
			<td colSpan="4">&nbsp;</td>
		</tr>
		<tr id="typeRow" runat="server">
			<td class="SubHead" vAlign="middle" align="right" colspan="2"><dnn:label id="plType" text="Skin Type:" runat="server"></dnn:label></td>
			<td align="left" colSpan="2">&nbsp;&nbsp;
				<asp:checkbox id="chkHost" CssClass="SubHead" Runat="server" resourcekey="Host" AutoPostBack="True"
					Checked="True" Text="Host" OnCheckedChanged="chkHost_CheckedChanged"></asp:checkbox>&nbsp;&nbsp;
				<asp:checkbox id="chkSite" CssClass="SubHead" Runat="server" resourcekey="Site" AutoPostBack="True"
					Checked="True" Text="Site" OnCheckedChanged="chkSite_CheckedChanged"></asp:checkbox>
			</td>
		</tr>
		<tr>
			<td colSpan="4">&nbsp;</td>
		</tr>
		<tr>
			<td class="SubHead" vAlign="middle"><dnn:label id="plSkins" suffix=":" controlname="cboSkins" runat="server"></dnn:label></td>
			<td><asp:dropdownlist id="cboSkins" Runat="server" AutoPostBack="True" OnSelectedIndexChanged="cboSkins_SelectedIndexChanged"></asp:dropdownlist></td>
			<td class="SubHead" vAlign="middle"><dnn:label id="plContainers" suffix=":" controlname="cboContainers" runat="server"></dnn:label></td>
			<td><asp:dropdownlist id="cboContainers" Runat="server" AutoPostBack="True" OnSelectedIndexChanged="cboContainers_SelectedIndexChanged"></asp:dropdownlist></td>
		</tr>
		<tr>
			<td colSpan="4">&nbsp;</td>
		</tr>
		<tr>
			<td align="center" colSpan="4"><asp:linkbutton id="cmdRestore" CssClass="CommandButton" Runat="server" resourcekey="cmdRestore" OnClick="cmdRestore_Click">Restore Default Skin</asp:linkbutton></td>
		</tr>
		<tr>
			<td colSpan="4">&nbsp;</td>
		</tr>
		<tr>
			<td colSpan="4"><asp:label id="lblGallery" runat="server"></asp:label></td>
		</tr>
		<tr>
			<td class="SubHead" align="center" colSpan="4"><asp:panel id="pnlSkin" Runat="server" Visible="False">
<asp:label id="lblApply" runat="server" resourcekey="ApplyTo">Apply To</asp:label>: 
      &nbsp;&nbsp; 
<asp:CheckBox id="chkPortal" Text="Portal" Checked="True" resourcekey="Portal" Runat="server"
						CssClass="SubHead"></asp:CheckBox>&nbsp;&nbsp; 
<asp:CheckBox id="chkAdmin" Text="Admin" Checked="True" resourcekey="Admin" Runat="server" CssClass="SubHead"></asp:CheckBox><BR><BR>
<asp:LinkButton id="cmdParse" resourcekey="cmdParse" Runat="server" CssClass="CommandButton" OnClick="cmdParse_Click">Parse Skin Package</asp:LinkButton>&nbsp;&nbsp; 
<asp:LinkButton id="cmdDelete" resourcekey="cmdDelete" Runat="server" CssClass="CommandButton" OnClick="cmdDelete_Click">Delete Skin Package</asp:LinkButton></asp:panel></td>
		</tr>
		<tr>
			<td colSpan="4">&nbsp;</td>
		</tr>
		<tr>
			<td align="center" colSpan="4"><asp:panel id="pnlParse" Runat="server" Visible="False">
					<TABLE cellSpacing="0" cellPadding="0" border="0">
						<TR>
							<TD class="SubHead">
								<asp:label id="lblParseOptions" runat="server" resourcekey="ParseOptions">Parse Options</asp:label>:</TD>
							<TD>
								<asp:RadioButtonList id="optParse" Runat="server" CssClass="SubHead" RepeatDirection="Horizontal">
									<asp:ListItem resourcekey="Localized" Value="L" Selected="True">Localized</asp:ListItem>
									<asp:ListItem resourcekey="Portable" Value="P">Portable</asp:ListItem>
								</asp:RadioButtonList></TD>
						</TR>
					</TABLE>
				</asp:panel></td>
		</tr>
		<tr>
			<td colSpan="4"><asp:label id="lblOutput" CssClass="Normal" Runat="server" EnableViewState="False"></asp:label></td>
		</tr>
	</table>
</center>
