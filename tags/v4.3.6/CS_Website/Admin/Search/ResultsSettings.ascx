<%@ Control Language="C#" AutoEventWireup="true"  Inherits="DotNetNuke.Modules.SearchResults.ResultsSettings" CodeFile="ResultsSettings.ascx.cs" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<table cellSpacing="0" cellPadding="2" summary="Edit Search Design Table" border="0">
    <tr>
        <td class="SubHead" width="250">
            <dnn:Label ID="plResults" runat="server" ControlName="txtresults" Text="Maximum Search Results:" />
        </td>
        <td class="NormalTextBox">
            <asp:TextBox ID="txtresults" runat="server" CssClass="NormalTextBox" MaxLength="5"></asp:TextBox>
            <asp:CompareValidator ID="CompareValidator5" runat="server" ControlToValidate="txtresults" CssClass="NormalRed" Display="Dynamic" Operator="GreaterThan" resourcekey="Validation.ErrorMessage"
                Type="Integer" ValueToCompare="0"></asp:CompareValidator></td>
    </tr>
    <tr>
        <td class="SubHead" width="250">
            <dnn:Label ID="plPage" runat="server" ControlName="txtPage" Text="Results per Page:" />
        </td>
        <td class="NormalTextBox">
            <asp:TextBox ID="txtPage" runat="server" CssClass="NormalTextBox" MaxLength="5"></asp:TextBox>
            <asp:CompareValidator ID="CompareValidator1" runat="server" ControlToValidate="txtPage" CssClass="NormalRed" Display="Dynamic" Operator="GreaterThan" resourcekey="Validation.ErrorMessage"
                Type="Integer" ValueToCompare="0"></asp:CompareValidator></td>
    </tr>
    <tr>
        <td class="SubHead" width="250">
            <dnn:Label ID="plTitle" runat="server" ControlName="txtTitle" Text="Maximum Title Length:" />
        </td>
        <td class="NormalTextBox">
            <asp:TextBox ID="txtTitle" runat="server" CssClass="NormalTextBox" MaxLength="5"></asp:TextBox>
            <asp:CompareValidator ID="CompareValidator2" runat="server" ControlToValidate="txtTitle" CssClass="NormalRed" Display="Dynamic" Operator="GreaterThan" resourcekey="Validation.ErrorMessage"
                Type="Integer" ValueToCompare="0"></asp:CompareValidator></td>
    </tr>
    <tr>
        <td class="SubHead" width="250">
            <dnn:Label ID="plDescription" runat="server" ControlName="txtdescription" Text="Maximum Description Length:" />
        </td>
        <td class="NormalTextBox">
            <asp:TextBox ID="txtdescription" runat="server" CssClass="NormalTextBox" MaxLength="5"></asp:TextBox>
            <asp:CompareValidator ID="CompareValidator3" runat="server" ControlToValidate="txtdescription" CssClass="NormalRed" Display="Dynamic" Operator="GreaterThan" resourcekey="Validation.ErrorMessage"
                Type="Integer" ValueToCompare="0"></asp:CompareValidator></td>
    </tr>
	<tr>
		<td colSpan="2">&nbsp;</td>
	</tr>
	<tr>
        <td class="SubHead" width="250"><dnn:label id="plShowDescription" runat="server" controlname="chkDescription" text="Show Description?"></dnn:label></td>
		<td class="NormalTextBox"><asp:checkbox id="chkDescription" runat="server" CssClass="NormalTextBox"></asp:checkbox></td>
	</tr>
</table>
