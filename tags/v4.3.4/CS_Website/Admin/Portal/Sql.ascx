<%@ Control Language="C#" AutoEventWireup="true"  Inherits="DotNetNuke.Modules.Admin.SQL.SQL" CodeFile="SQL.ascx.cs" %>
<br/>
<asp:TextBox ID="txtQuery" Runat="server" TextMode="MultiLine" Columns="50" Rows="10" EnableViewState="False"></asp:TextBox>
<br/>
<asp:linkbutton id="cmdExecute" resourcekey="cmdExecute" EnableViewState="False" CssClass="CommandButton" runat="server" ToolTip="can include {directives} and /*comments*/" Width="120px" OnClick="cmdExecute_Click">Execute</asp:linkbutton>
<asp:checkbox id="chkRunAsScript" resourcekey="chkRunAsScript" CssClass="SubHead" runat="server" text="Run as Script" textalign="Left" tooltip="include 'GO' directives; for testing &amp; update scripts"></asp:checkbox>
<br/>
<br/>
<asp:Label id="lblMessage" Runat="server" CssClass="NormalRed" EnableViewState="False"></asp:Label>
<asp:DataGrid ID="grdResults" Runat="server" AutoGenerateColumns="True" HeaderStyle-CssClass="SubHead"
	ItemStyle-CssClass="Normal" summary="SQL Design Table" EnableViewState="False">
	<ItemStyle CssClass="Normal"></ItemStyle>
	<HeaderStyle CssClass="SubHead"></HeaderStyle>
</asp:DataGrid>
