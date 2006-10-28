<%@ Control Language="C#" AutoEventWireup="false" Inherits="DotNetNuke.Services.Localization.LanguagePack"
    CodeFile="LanguagePack.ascx.cs" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register TagPrefix="dnn" TagName="SectionHead" Src="~/controls/SectionHeadControl.ascx" %>
<table>
	<tr>
		<td class="SubHead" vAlign="middle" width="150"><dnn:label id="lbLocale" text="Resource Locale" controlname="cboLanguage" runat="server"></dnn:label></td>
		<td vAlign="top"><asp:dropdownlist id="cboLanguage" runat="server"></asp:dropdownlist></td>
	</tr>
	<TR>
		<TD class="SubHead" vAlign="middle" width="150"><dnn:label id="lblType" text="Resource Locale" controlname="cboLanguage" runat="server"></dnn:label></TD>
		<TD vAlign="top"><asp:radiobuttonlist id="rbPackType" runat="server" AutoPostBack="True" CellSpacing="0" CellPadding="0"
				RepeatDirection="Horizontal" CssClass="Normal">
				<asp:ListItem resourcekey="Core.LangPackType" Value="Core" Selected="True">Core</asp:ListItem>
				<asp:ListItem resourcekey="Module.LangPackType" Value="Module">Module</asp:ListItem>
				<asp:ListItem resourcekey="Provider.LangPackType" Value="Provider">Provider</asp:ListItem>
				<asp:ListItem resourcekey="Full.LangPackType" Value="Full">Full</asp:ListItem>
			</asp:radiobuttonlist></TD>
	</TR>
	<TR id="rowitems" runat="server">
		<TD vAlign="middle" width="150"></TD>
		<TD vAlign="top"><asp:label id="lblItems" runat="server" CssClass="SubHead"></asp:label><BR>
			<asp:listbox id="lbItems" runat="server" Rows="7" Width="300px"></asp:listbox></TD>
	</TR>
	<TR>
		<TD class="SubHead" vAlign="middle" width="150"><dnn:label id="lblName" text="Resource Locale" controlname="cboLanguage" runat="server"></dnn:label></TD>
		<TD vAlign="top">
			<asp:Label id="Label2" runat="server" CssClass="Normal">ResourcePack.</asp:Label><asp:textbox id="txtFileName" runat="server" Width="200px">Core</asp:textbox>
			<asp:Label id="lblFilenameFix" runat="server" CssClass="Normal">.&lt;version&gt;.&lt;locale&gt;.zip</asp:Label></TD>
	</TR>
	<TR>
		<TD class="SubHead" vAlign="middle" width="150"></TD>
		<TD vAlign="top"><asp:linkbutton id="cmdCreate" runat="server" CssClass="CommandButton" resourcekey="cmdCreate" Text="Create"></asp:linkbutton>&nbsp;
			<asp:linkbutton id="cmdCancel" runat="server" CssClass="CommandButton" resourcekey="cmdCancel" Text="Cancel"></asp:linkbutton></TD>
	</TR>
</table>
<P></P>
<asp:panel id="pnlLogs" runat="server" Visible="False">
	<dnn:sectionhead id="dshBasic" runat="server" text="Language Pack Log" resourcekey="LogTitle" cssclass="Head"
		includerule="True" section="divLog"></dnn:sectionhead>
	<DIV id="divLog" runat="server">
		<asp:HyperLink id="hypLink" runat="server" CssClass="CommandButton"></asp:HyperLink>
		<HR>
		<asp:Label id="lblMessage" runat="server"></asp:Label></DIV>
</asp:panel>
<script language="javascript">
function changeItem(l,t)
{
	var sel=document.getElementById(l);
	var tex=document.getElementById(t);
	var f=0;
	for(var i=0; i<sel.length; i++)
	{
		var o = sel.options[i];
		if(o.selected) f++;
	}
	if(f==1)
	{
		var o = sel.options[sel.selectedIndex];
		tex.value = o.text.replace(" [",".");
		tex.value = tex.value.replace("]","");
	}
}
</script>
