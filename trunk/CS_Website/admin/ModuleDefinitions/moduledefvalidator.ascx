<%@ Control Language="C#" AutoEventWireup="true" Explicit="True" Inherits="DotNetNuke.Modules.Admin.ModuleDefinitions.ModuleDefValidator" CodeFile="ModuleDefValidator.ascx.cs" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<br>
<div align="center">
    <div>
        <asp:label id="lblBrowse" resourcekey="lblBrowse" cssClass="Normal" runat="server">Please select a *.dnn file from your local system for validation.</asp:label>
        <br>
        <label style="DISPLAY: none" for="<%=cmdBrowse.ClientID%>">Browse Files</label>
        <input id="cmdBrowse" type="file" size="50" name="cmdBrowse" runat="server">&nbsp;&nbsp;
        <asp:linkbutton id="lnkValidate" resourcekey="lnkValidate" runat="server" cssclass="CommandButton">Validate</asp:linkbutton>
    </div>
    <br>
    <asp:datalist id="lstResults" runat="server" width="100%" visible="False" borderwidth="0" cellspacing="0" cellpadding="4">
        <headertemplate>
            <asp:label id="lblHeader" resourcekey="lblHeader" cssclass="NormalBold" runat="server">Validation Results</asp:label>
        </headertemplate>
        <itemtemplate>
            <span class="Normal">
                <%# Container.DataItem %>
            </span>
        </itemtemplate>
    </asp:datalist>
</div>
