<%@ Control Language="C#" AutoEventWireup="false" CodeFile="LanguageEditor.ascx.cs" Inherits="DotNetNuke.Services.Localization.LanguageEditor" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register TagPrefix="dnn" TagName="SectionHead" Src="~/controls/SectionHeadControl.ascx" %>
<%@ Register TagPrefix="dnntv" Namespace="DotNetNuke.UI.WebControls" Assembly="DotNetNuke.WebControls" %>
<style type="text/css">.Pending { BORDER-LEFT-COLOR: red; BORDER-BOTTOM-COLOR: red; BORDER-TOP-STYLE: solid; BORDER-TOP-COLOR: red; BORDER-RIGHT-STYLE: solid; BORDER-LEFT-STYLE: solid; BORDER-RIGHT-COLOR: red; BORDER-BOTTOM-STYLE: solid }
	</style>
<TABLE id="Table2" cellSpacing="5" width="100%" border="0">
	<TR>
		<TD vAlign="top" noWrap>
			<P><asp:panel id="Panel1" Width="195px" runat="server">
					<dnntv:DNNTree id="DNNTree" runat="server" DefaultNodeCssClassOver="Normal" CssClass="Normal" DefaultNodeCssClass="Normal"></dnntv:DNNTree>
				</asp:panel></P>
		</TD>
		<TD vAlign="top">
			<P>
				<TABLE id="Table1" cellSpacing="1" cellPadding="1" border="0">
					<TR>
						<TD width="150" vAlign="top"><asp:label id="lblSelected" runat="server" CssClass="SubHead" resourcekey="SelectedFile">Selected Resource File:</asp:label></TD>
						<TD vAlign="top"><asp:label id="lblResourceFile" runat="server" CssClass="Normal" Font-Bold="True" text="Selected Resource File:">Selected Resource File:</asp:label></TD>
					</TR>
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
						<TD width="150" class="SubHead" vAlign="top"><dnn:label id="lbLocales" runat="server" text="Available Locales" controlname="cboLocales"></dnn:label></TD>
						<TD vAlign="top"><asp:dropdownlist id="cboLocales" Width="300px" runat="server" AutoPostBack="True" DataValueField="key"
								DataTextField="name" CssClass="Normal"></asp:dropdownlist></TD>
					</TR>
					<TR id="rowMode" runat="server">
						<TD width="150" class="SubHead" vAlign="top"><dnn:label id="lbMode" runat="server" text="Available Locales" controlname="cboLocales"></dnn:label></TD>
						<TD vAlign="top"><asp:radiobuttonlist id="rbMode" runat="server" CssClass="Normal" AutoPostBack="True" RepeatColumns="3"
								RepeatDirection="Horizontal">
								<asp:ListItem resourcekey="ModeSystem" Value="System" Selected="True">System</asp:ListItem>
								<asp:ListItem resourcekey="ModeHost" Value="Host">Host</asp:ListItem>
								<asp:ListItem resourcekey="ModePortal" Value="Portal">Portal</asp:ListItem>
							</asp:radiobuttonlist></TD>
					</TR>
					<tr>
						<td class="SubHead" vAlign="top" colSpan="2"><asp:checkbox id="chkHighlight" runat="server" resourcekey="Highlight" AutoPostBack="True" TextAlign="Left"
								Text="Highlight Pending Translations"></asp:checkbox></td>
					</tr>
				</TABLE>
			</P>
			<P><asp:datagrid id="dgEditor" runat="server" CssClass="Normal" GridLines="None" CellPadding="3"
					AutoGenerateColumns="False">
					<ItemStyle VerticalAlign="Top"></ItemStyle>
					<HeaderStyle Font-Bold="True"></HeaderStyle>
					<Columns>
						<asp:TemplateColumn>
							<ItemTemplate>
								<TABLE cellSpacing="2" cellPadding="0" width="100%" border="0">
									<TR>
										<TD width="100%" bgColor="silver" colSpan="3">
											<asp:label id="Label3" runat="server" CssClass="NormalBold" resourcekey="ResourceName" Font-Bold="True">
												Resource name:</asp:label>
											<asp:Label id="lblName" runat="server" CssClass="Normal">
												<%# DataBinder.Eval(Container, "DataItem.key") %>
											</asp:Label></TD>
									</TR>
									<TR>
										<TD width="300">
											<asp:label id="Label4" runat="server" CssClass="NormalBold" resourcekey="Value" Font-Bold="True">
												Localized Value</asp:label></TD>
										<TD></TD>
										<TD width="100%">
											<TABLE border="0">
												<TR>
													<TD>
														<dnn:sectionhead id="dshDef" runat="server" text="" includerule="False" section="divDef" cssclass="Normal" IsExpanded='<%# ExpandDefault((System.Web.UI.Pair)DataBinder.Eval(Container, "DataItem.value"))  %>'>
														</dnn:sectionhead></TD>
													<TD>
														<asp:label id="Label5" runat="server" CssClass="NormalBold" resourcekey="DefaultValue" Font-Bold="True">
												Default Value</asp:label></TD>
												</TR>
											</TABLE>
										</TD>
									</TR>
									<TR>
										<TD vAlign="top" width="300">
											<asp:TextBox id="txtValue" Width="300px" runat="server" TextMode="MultiLine" Height="30px"></asp:TextBox></TD>
										<TD vAlign="top" noWrap>
											<asp:HyperLink id=lnkEdit runat="server" CssClass="CommandButton" NavigateUrl='<%# OpenFullEditor(Convert.ToString(DataBinder.Eval(Container, "DataItem.key"))) %> '>
												<asp:Image runat="server" AlternateText="Edit" ID="imgEdit" ImageUrl="~/images/uprt.gif" resourcekey="cmdEdit"></asp:Image>
											</asp:HyperLink>&nbsp;
										</TD>
										<TD vAlign="top" width="100%">
											<TABLE id="divDef" cellSpacing="0" cellPadding="0" width="100%" border="0" runat="server">
												<TR>
													<TD>
														<asp:Label id="lblDefault" runat="server" CssClass="Normal"></asp:Label></TD>
												</TR>
											</TABLE>
										</TD>
									</TR>
								</TABLE>
							</ItemTemplate>
						</asp:TemplateColumn>
						<asp:BoundColumn Visible="False" DataField="key"></asp:BoundColumn>
					</Columns>
				</asp:datagrid></P>
			<P><asp:linkbutton id="cmdUpdate" runat="server" CssClass="CommandButton" resourcekey="cmdUpdate">Update</asp:linkbutton>&nbsp;<asp:linkbutton id="cmdCancel" runat="server" CssClass="CommandButton" resourcekey="cmdCancel" CausesValidation="false">Cancel</asp:linkbutton>&nbsp;<asp:linkbutton id="cmdDelete" runat="server" CssClass="CommandButton" resourcekey="cmdDelete" CausesValidation="false">Delete</asp:linkbutton></P>
		</TD>
	</TR>
</TABLE>
