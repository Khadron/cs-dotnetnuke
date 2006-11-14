<%@ Control Language="C#" AutoEventWireup="true" Inherits="DotNetNuke.Services.Localization.Languages" CodeFile="Languages.ascx.cs" %>
<%@ Register TagPrefix="dnn" TagName="SectionHead" Src="~/controls/SectionHeadControl.ascx" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<dnn:sectionhead id="dshBasic" runat="server" cssclass="Head" text="Suported Locales" section="tblBasic"
	resourcekey="SupportedLocales" includerule="False"></dnn:sectionhead><br>
<TABLE id="tblBasic" cellSpacing="1" cellPadding="1" border="0" runat="server">
	<TR>
		<TD noWrap><asp:datagrid id="dgLocales" runat="server" CssClass="Normal" AutoGenerateColumns="False" CellPadding="4"
				GridLines="None">
				<AlternatingItemStyle Wrap="False"></AlternatingItemStyle>
				<ItemStyle Wrap="False"></ItemStyle>
				<HeaderStyle Font-Bold="True" Wrap="False"></HeaderStyle>
				<Columns>
					<asp:BoundColumn DataField="name" ReadOnly="True" HeaderText="Name"></asp:BoundColumn>
					<asp:BoundColumn DataField="key" ReadOnly="True" HeaderText="Key"></asp:BoundColumn>
					<asp:TemplateColumn HeaderText="Status">
						<ItemTemplate>
							<asp:Label id="lblStatus" runat="server" CssClass="Normal"></asp:Label>
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn>
						<ItemTemplate>
							<asp:LinkButton id="cmdDisable" runat="server" CssClass="CommandButton" Text="Disable" CausesValidation="false"
								CommandName="Disable"></asp:LinkButton>
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn>
						<ItemTemplate>
							<asp:LinkButton id=cmdDelete runat="server" resourcekey="cmdDelete" CssClass="CommandButton" Text="Delete" CommandName="Delete" CausesValidation="False" Visible="<%# UserInfo.IsSuperUser %>">Delete</asp:LinkButton>
						</ItemTemplate>
					</asp:TemplateColumn>
				</Columns>
			</asp:datagrid>
			<P><asp:checkbox id="chkDeleteFiles" runat="server" resourcekey="DeleteFiles" CssClass="Normal" Text="Delete all resources of this locale, too"></asp:checkbox></P>
		</TD>
	</TR>
</TABLE>
<asp:panel id="pnlAdd" runat="server">
	<P>
		<dnn:sectionhead id="dshAdd" runat="server" cssclass="Head" text="Add New Locale" section="tblAdd"
			resourcekey="AddNewLocale" includerule="False"></dnn:sectionhead><BR>
		<TABLE id="tblAdd" cellSpacing="1" cellPadding="1" border="0" runat="server">
			<TR>
				<TD noWrap></TD>
				<TD noWrap>
					<asp:RadioButtonList id="rbDisplay" runat="server" CssClass="Normal" RepeatDirection="Horizontal" RepeatLayout="Flow"
						AutoPostBack="True">
						<asp:ListItem Value="English" resourcekey="DisplayEnglish">English</asp:ListItem>
						<asp:ListItem Value="Native" resourcekey="DisplayNative" Selected="True">Native</asp:ListItem>
					</asp:RadioButtonList></TD>
			</TR>
			<TR>
				<TD class="SubHead" vAlign="top" noWrap width="150">
					<dnn:label id="lbLocale" runat="server" text="Name" controlname="txtName"></dnn:label></TD>
				<TD vAlign="top" noWrap>
					<asp:dropdownlist id="cboLocales" runat="server"></asp:dropdownlist></TD>
			</TR>
			<TR>
				<TD noWrap></TD>
				<TD noWrap>
					<asp:linkbutton id="cmdAdd" runat="server" resourcekey="Add" CssClass="CommandButton">Add</asp:linkbutton></TD>
			</TR>
		</TABLE>
	</P>
</asp:panel>
