<%@ Control Language="C#" CodeFile="MemberServices.ascx.cs" AutoEventWireup="true" Explicit="True" Inherits="DotNetNuke.Modules.Admin.Security.MemberServices" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<table id="tblServices" cellspacing="0" cellpadding="4" width="600" summary="Register Design Table" border="0" runat="server">
	<tr>
		<td class="SubHead" width="150"><dnn:label id="plRSVPCode" runat="server" controlname="txtRSVPCode"></dnn:label></td>
		<td width="325"><asp:textbox id="txtRSVPCode" runat="server" width="100" cssclass="NormalTextBox" maxlength="50" columns="30"></asp:textbox><asp:linkbutton class="CommandButton" id="cmdRSVP" runat="server" text="Subscribe" resourcekey="cmdRSVP"></asp:linkbutton></td></tr>
	<tr>
		<td colspan="2">
			<asp:datagrid id="grdServices" runat="server" GridLines="None" enableviewstate="true" 
				autogeneratecolumns="false" cellpadding="2" border="0" summary="Register Design Table">
				<columns>
					<asp:templatecolumn>
						<itemtemplate>
							<asp:hyperlink text='<%# ServiceText(Convert.ToBoolean(DataBinder.Eval(Container.DataItem,"Subscribed"))) %>' cssclass="CommandButton" navigateurl='<%# ServiceURL("RoleID",Convert.ToString(DataBinder.Eval(Container.DataItem,"RoleID")),DataBinder.Eval(Container.DataItem,"ServiceFee"),Convert.ToBoolean(DataBinder.Eval(Container.DataItem,"Subscribed"))) %>' runat="server" id="Hyperlink1"/>
						</itemtemplate>
					</asp:templatecolumn>
					<asp:boundcolumn headertext="Name" datafield="RoleName" itemstyle-cssclass="Normal" headerstyle-cssclass="NormalBold" />
					<asp:boundcolumn headertext="Description" datafield="Description" itemstyle-cssclass="Normal" headerstyle-cssclass="NormalBold" />
					<asp:templatecolumn headertext="Fee" headerstyle-cssclass="NormalBold">
						<itemtemplate>
							<asp:label runat="server" text='<%#FormatPrice(Convert.ToSingle(DataBinder.Eval(Container.DataItem, "ServiceFee"))) %>' cssclass="Normal" id="Label2"/>
						</itemtemplate>
					</asp:templatecolumn>
					<asp:templatecolumn headertext="Every" headerstyle-cssclass="NormalBold">
						<itemtemplate>
							<asp:label runat="server" text='<%#FormatPeriod(Convert.ToInt32(DataBinder.Eval(Container.DataItem, "BillingPeriod"))) %>' cssclass="Normal" id="Label3"/>
						</itemtemplate>
					</asp:templatecolumn>
					<asp:boundcolumn headertext="Period" datafield="BillingFrequency" itemstyle-cssclass="Normal" headerstyle-cssclass="NormalBold" />
					<asp:templatecolumn headertext="Trial" headerstyle-cssclass="NormalBold">
						<itemtemplate>
							<asp:label runat="server" text='<%#FormatPrice(Convert.ToSingle(DataBinder.Eval(Container.DataItem, "TrialFee"))) %>' cssclass="Normal" id="Label4"/>
						</itemtemplate>
					</asp:templatecolumn>
					<asp:templatecolumn headertext="Every" headerstyle-cssclass="NormalBold">
						<itemtemplate>
							<asp:label runat="server" text='<%#FormatPeriod(Convert.ToInt32(DataBinder.Eval(Container.DataItem, "TrialPeriod"))) %>' cssclass="Normal" id="Label5"/>
						</itemtemplate>
					</asp:templatecolumn>
					<asp:boundcolumn headertext="Period" datafield="TrialFrequency" itemstyle-cssclass="Normal" headerstyle-cssclass="NormalBold" />
					<asp:templatecolumn headertext="ExpiryDate" headerstyle-cssclass="NormalBold">
						<itemtemplate>
							<asp:label runat="server" text='<%#FormatExpiryDate(Convert.ToDateTime(DataBinder.Eval(Container.DataItem, "ExpiryDate"))) %>' cssclass="Normal" id="Label1"/>
						</itemtemplate>
					</asp:templatecolumn>
				</columns>
			</asp:datagrid>
			<asp:label id="lblServices" runat="server" cssclass="Normal" visible="False"></asp:label>
		</td>
	</tr>
</table>
