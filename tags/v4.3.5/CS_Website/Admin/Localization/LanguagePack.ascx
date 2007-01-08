<%@ Control AutoEventWireup="true" CodeFile="LanguagePack.ascx.cs" Inherits="DotNetNuke.Services.Localization.LanguagePack" Language="C#" %>
<%@ Register Src="~/controls/LabelControl.ascx" TagName="Label" TagPrefix="dnn" %>
<%@ Register Src="~/controls/SectionHeadControl.ascx" TagName="SectionHead" TagPrefix="dnn" %>
<table>
    <tr>
        <td class="SubHead" valign="middle" width="150">
            <dnn:Label ID="lbLocale" runat="server" ControlName="cboLanguage" Text="Resource Locale" />
        </td>
        <td valign="top">
            <asp:DropDownList ID="cboLanguage" runat="server">
            </asp:DropDownList></td>
    </tr>
    <tr>
        <td class="SubHead" valign="middle" width="150">
            <dnn:Label ID="lblType" runat="server" ControlName="cboLanguage" Text="Resource Locale" />
        </td>
        <td valign="top">
            <asp:RadioButtonList ID="rbPackType" runat="server" AutoPostBack="True" CellPadding="0" CellSpacing="0" CssClass="Normal" OnSelectedIndexChanged="rbPackType_SelectedIndexChanged"
                RepeatDirection="Horizontal">
                <asp:ListItem resourcekey="Core.LangPackType" Selected="True" Value="Core">Core</asp:ListItem>
                <asp:ListItem resourcekey="Module.LangPackType" Value="Module">Module</asp:ListItem>
                <asp:ListItem resourcekey="Provider.LangPackType" Value="Provider">Provider</asp:ListItem>
                <asp:ListItem resourcekey="Full.LangPackType" Value="Full">Full</asp:ListItem>
            </asp:RadioButtonList></td>
    </tr>
    <tr id="rowitems" runat="server">
        <td valign="middle" width="150">
        </td>
        <td valign="top">
            <asp:Label ID="lblItems" runat="server" CssClass="SubHead"></asp:Label><br>
            <asp:ListBox ID="lbItems" runat="server" Rows="7" Width="300px"></asp:ListBox></td>
    </tr>
    <tr>
        <td class="SubHead" valign="middle" width="150">
            <dnn:Label ID="lblName" runat="server" ControlName="cboLanguage" Text="Resource Locale" />
        </td>
        <td valign="top">
            <asp:Label ID="Label2" runat="server" CssClass="Normal">ResourcePack.</asp:Label><asp:TextBox ID="txtFileName" runat="server" Width="200px">Core</asp:TextBox>
            <asp:Label ID="lblFilenameFix" runat="server" CssClass="Normal">.&lt;version&gt;.&lt;locale&gt;.zip</asp:Label></td>
    </tr>
    <tr>
        <td class="SubHead" valign="middle" width="150">
        </td>
        <td valign="top">
            <asp:LinkButton ID="cmdCreate" runat="server" CssClass="CommandButton" OnClick="cmdCreate_Click" resourcekey="cmdCreate" Text="Create"></asp:LinkButton>&nbsp;
            <asp:LinkButton ID="cmdCancel" runat="server" CssClass="CommandButton" OnClick="cmdCancel_Click" resourcekey="cmdCancel" Text="Cancel"></asp:LinkButton></td>
    </tr>
</table>
<p>
</p>
<asp:Panel ID="pnlLogs" runat="server" Visible="False">
    <dnn:SectionHead ID="dshBasic" runat="server" CssClass="Head" IncludeRule="True" ResourceKey="LogTitle" Section="divLog" Text="Language Pack Log" />
    <div id="divLog" runat="server">
        <asp:HyperLink ID="hypLink" runat="server" CssClass="CommandButton"></asp:HyperLink>
        <hr/>
        <asp:Label ID="lblMessage" runat="server"></asp:Label></div>
</asp:Panel>

<script type="text/javascript">
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

