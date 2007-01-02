<%@ Control Language="C#" AutoEventWireup="true" Explicit="True" Inherits="DotNetNuke.UI.ControlPanels.Classic" CodeFile="Classic.ascx.cs" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<table width="100%" border="0" summary="Table for design" id="Table2" style="BORDER-RIGHT:#cccccc 1px dotted; BORDER-TOP:#cccccc 1px dotted; BORDER-LEFT:#cccccc 1px dotted; BORDER-BOTTOM:#cccccc 1px dotted">
    <tr valign="top">
        <td align="left" valign="bottom" width="20%" nowrap>
            <table border="0">
                <tr>
                    <td nowrap>
                        <asp:label id="lblAdmin" runat="server" cssclass="SubHead" enableviewstate="False">Page Admin:</asp:label>&nbsp;&nbsp;
                        <asp:hyperlink id="cmdAddTab" runat="server" cssclass="CommandButton" enableviewstate="False">Add</asp:hyperlink>&nbsp;&nbsp;
                        <asp:hyperlink id="cmdEditTab" runat="server" cssclass="CommandButton" enableviewstate="False">Edit</asp:hyperlink>&nbsp;&nbsp;
                        <asp:hyperlink id="cmdCopyTab" runat="server" cssclass="CommandButton" enableviewstate="False">Copy</asp:hyperlink>
                    </td>
                </tr>
            </table>
        </td>
        <td align="center" valign="top" nowrap cssClass="SubHead">
			<table cellspacing="1" cellpadding="0" border="0" height="100%">
				<tr valign="middle">
				    <td>
                        <asp:imagebutton id="cmdHelpShow" alternatetext="Show Module Info" runat="server" visible="False" borderwidth="0" enableviewstate="False"></asp:imagebutton>
                        <asp:imagebutton id="cmdHelpHide" alternatetext="Hide Module Info" runat="server" visible="False" borderwidth="0" enableviewstate="False"></asp:imagebutton>
				    </td>
				    <td>&nbsp;</td>
					<td class="SubHead" align="right"><asp:label id="lblModule" runat="server" cssclass="SubHead" enableviewstate="False">Module:</asp:label>&nbsp;</td>
					<td><asp:dropdownlist id="cboDesktopModules" runat="server" cssclass="NormalTextBox" width="150" datavaluefield="DesktopModuleID" datatextfield="FriendlyName"></asp:dropdownlist>&nbsp;&nbsp;</td>
					<td class="SubHead" align="right"><asp:label id="lblPane" runat="server" cssclass="SubHead" enableviewstate="False">Pane:</asp:label>&nbsp;</td>
					<td><asp:dropdownlist id="cboPanes" runat="server" cssclass="NormalTextBox" width="100"></asp:dropdownlist>&nbsp;&nbsp;</td>
					<td class="SubHead" align="right"><asp:label id="lblAlign" runat="server" cssclass="SubHead" enableviewstate="False">Align:</asp:label>&nbsp;</td>
					<td>
						<asp:dropdownlist id="cboAlign" runat="server" cssclass="NormalTextBox" width="100">
							<asp:ListItem Value="left" resourcekey="Left">Left</asp:ListItem>
							<asp:ListItem Value="center" resourcekey="Center">Center</asp:ListItem>
							<asp:ListItem Value="right" resourcekey="Right">Right</asp:ListItem>
						</asp:dropdownlist>&nbsp;&nbsp;
					</td>
					<td width="35" align="center" class="Normal"><asp:linkbutton id="cmdAdd" runat="server" cssclass="CommandButton" causesvalidation="False" enableviewstate="False">Add</asp:linkbutton></td>
				</tr>
			</table>
        </td>
        <td align="right" valign="bottom" width="20%" nowrap>
            <asp:checkbox id="chkContent" runat="server" text="Content:" cssclass="SubHead" autopostback="True" textalign="Left"></asp:checkbox>
            <asp:checkbox id="chkPreview" runat="server" text="Preview:" cssclass="SubHead" autopostback="True" textalign="Left"></asp:checkbox>
        </td>
    </tr>
    <tr>
        <td></td>
        <td align="center" valign="top"><asp:label id="lblDescription" runat="server" cssclass="Normal" enableviewstate="False"></asp:label></td>
        <td></td>
    </tr>
</table>
