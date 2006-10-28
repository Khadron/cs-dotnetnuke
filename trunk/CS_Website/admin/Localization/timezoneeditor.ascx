<%@ Import namespace="System.Data"%>
<%@ Control Language="C#" AutoEventWireup="true" Inherits="DotNetNuke.Services.Localization.TimeZoneEditor" CodeFile="TimeZoneEditor.ascx.cs" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<P>
	<TABLE id="Table1" cellSpacing="1" cellPadding="1" border="0">
		<TR>
			<TD class="SubHead" valign="top"><dnn:label id="lbLocales" runat="server" controlname="cboLocales" text="Available Locales"></dnn:label></TD>
			<TD valign="top"><asp:DropDownList id="cboLocales" runat="server" Width="300px" DataTextField="name" DataValueField="key"
					AutoPostBack="True"></asp:DropDownList></TD>
		</TR>
	</TABLE>
</P>
<P>
	<asp:Panel id="pnlMissing" runat="server" Visible="False" Wrap="True">
		<asp:Label id="lblMissing" runat="server">System Default resource file contains some entries not present in current localized file. This can lead to some values not being translated.</asp:Label>
		<BR>
		<asp:LinkButton id="cmdAddMissing" runat="server" resourcekey="cmdAddMissing" CssClass="CommandButton"
			CausesValidation="false">Add Missing Entries</asp:LinkButton>
	</asp:Panel>
</P>
<P>
	<asp:DataGrid id="dgEditor" runat="server" AutoGenerateColumns="False" CellPadding="3" CellSpacing="1"
		GridLines="None">
		<ItemStyle VerticalAlign="Top"></ItemStyle>
		<HeaderStyle Font-Bold="True" CssClass="subsubhead" BackColor="Silver"></HeaderStyle>
		<Columns>
			<asp:TemplateColumn HeaderText="Name">
				<ItemTemplate>
					<asp:TextBox id=txtName runat="server" Width="300px" Text='<%# DataBinder.Eval(Container, "DataItem.name") %>'>
					</asp:TextBox>
					<asp:RequiredFieldValidator id="RequiredFieldValidator1" runat="server" ControlToValidate="txtName" Display="Dynamic"
						ErrorMessage="<br>Required Field" resourcekey="RequiredField.ErrorMessage"></asp:RequiredFieldValidator>
				</ItemTemplate>
			</asp:TemplateColumn>
			<asp:BoundColumn DataField="key" ReadOnly="True" HeaderText="Offset">
				<ItemStyle HorizontalAlign="Right" CssClass="Normal"></ItemStyle>
			</asp:BoundColumn>
			<asp:TemplateColumn HeaderText="DefaultValue">
				<ItemTemplate>
					<asp:Label id="Label2" runat="server" CssClass="Normal">
						<%# ((DataRow)Container.DataItem).GetParentRow("defaultvalues")["defaultvalue"] %>
					</asp:Label>
				</ItemTemplate>
			</asp:TemplateColumn>
		</Columns>
	</asp:DataGrid></P>
<P>
	<asp:LinkButton id="cmdUpdate" runat="server" CssClass="CommandButton" resourcekey="cmdUpdate">Update</asp:LinkButton>
	<asp:LinkButton id="cmdCancel" runat="server" CssClass="CommandButton" resourcekey="cmdCancel" CausesValidation="false">Cancel</asp:LinkButton>
</P>
