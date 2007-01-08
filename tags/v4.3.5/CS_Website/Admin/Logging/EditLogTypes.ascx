<%@ Control AutoEventWireup="true" CodeFile="EditLogTypes.ascx.cs" Inherits="DotNetNuke.Modules.Admin.Log.EditLogTypes" Language="C#" %>
<%@ Register Src="~/controls/LabelControl.ascx" TagName="Label" TagPrefix="dnn" %>
<%@ Register Src="~/controls/SectionHeadControl.ascx" TagName="SectionHead" TagPrefix="dnn" %>
<asp:Panel ID="pnlLogTypeConfigInfo" runat="server">
    <asp:DataGrid ID="dgLogTypeConfigInfo" runat="server" AutoGenerateColumns="false" border="0" CellPadding="4" DataKeyField="ID" OnEditCommand="dgLogTypeConfigInfo_EditCommand"
        summary="This table shows the log configuration entries.">
        <ItemStyle HorizontalAlign="Left" />
        <Columns>
            <asp:EditCommandColumn ButtonType="LinkButton" EditText="Edit" HeaderStyle-CssClass="NormalBold" ItemStyle-CssClass="Normal"></asp:EditCommandColumn>
            <asp:BoundColumn DataField="LogTypeKey" HeaderStyle-CssClass="NormalBold" HeaderText="LogType" ItemStyle-CssClass="Normal"></asp:BoundColumn>
            <asp:BoundColumn DataField="LogTypePortalID" HeaderStyle-CssClass="NormalBold" HeaderText="Portal" ItemStyle-CssClass="Normal"></asp:BoundColumn>
            <asp:BoundColumn DataField="LoggingIsActive" HeaderStyle-CssClass="NormalBold" HeaderText="Active" ItemStyle-CssClass="Normal"></asp:BoundColumn>
            <asp:BoundColumn DataField="LogFilename" HeaderStyle-CssClass="NormalBold" HeaderText="FileName" ItemStyle-CssClass="Normal"></asp:BoundColumn>
        </Columns>
    </asp:DataGrid>
    <p>
        <asp:LinkButton ID="cmdReturn" runat="server" BorderStyle="none" CausesValidation="False" class="CommandButton" OnClick="cmdReturn_Click" resourcekey="cmdReturn"
            Text="Return">Return</asp:LinkButton></p>
</asp:Panel>
<asp:Panel ID="pnlEditLogTypeConfigInfo" runat="server">
    <dnn:SectionHead ID="dshSettings" runat="server" CssClass="Head" IncludeRule="True" ResourceKey="Settings" Section="tblSettings" Text="Logging Settings" />
    <table id="tblSettings" runat="server" border="0" cellpadding="2" cellspacing="2">
        <tr>
            <td class="SubHead" width="150">
                <dnn:Label ID="plIsActive" runat="server" ControlName="chkIsActive" Suffix=":" />
            </td>
            <td class="Normal">
                <asp:CheckBox ID="chkIsActive" runat="server" AutoPostBack="True" OnCheckedChanged="chkIsActive_CheckedChanged" /></td>
        </tr>
        <tr>
            <td class="SubHead" width="150">
                <dnn:Label ID="plLogTypeKey" runat="server" ControlName="ddlLogTypeKey" Suffix=":" />
            </td>
            <td class="Normal">
                <asp:DropDownList ID="ddlLogTypeKey" runat="server">
                </asp:DropDownList></td>
        </tr>
        <tr>
            <td class="SubHead" width="150">
                <dnn:Label ID="plLogTypePortalID" runat="server" ControlName="ddlLogTypePortalID" Suffix=":" />
            </td>
            <td class="Normal">
                <asp:DropDownList ID="ddlLogTypePortalID" runat="server">
                </asp:DropDownList></td>
        </tr>
        <tr>
            <td class="SubHead" width="150">
                <dnn:Label ID="plKeepMostRecent" runat="server" ControlName="ddlKeepMostRecent" Suffix=":" />
            </td>
            <td class="Normal">
                <asp:DropDownList ID="ddlKeepMostRecent" runat="server">
                </asp:DropDownList></td>
        </tr>
        <tr>
            <td class="SubHead" width="150">
                <dnn:Label ID="plFileName" runat="server" ControlName="txtFileName" Suffix=":" />
            </td>
            <td class="Normal">
                <asp:TextBox ID="txtFileName" runat="server" Width="300"></asp:TextBox></td>
        </tr>
    </table>
    <br>
    <dnn:SectionHead ID="dshEmailSettings" runat="server" CssClass="Head" IncludeRule="True" ResourceKey="EmailSettings" Section="tblEmailSettings" Text="Email Notification Settings" />
    <table id="tblEmailSettings" runat="server" border="0" cellpadding="2" cellspacing="2">
        <tr>
            <td class="SubHead" width="180">
                <dnn:Label ID="plEmailNotificationStatus" runat="server" ControlName="chkEmailNotificationStatus" Suffix=":" />
            </td>
            <td class="Normal">
                <asp:CheckBox ID="chkEmailNotificationStatus" runat="server" AutoPostBack="True" OnCheckedChanged="chkEmailNotificationStatus_CheckedChanged" /></td>
        </tr>
        <tr>
            <td class="SubHead" width="180">
                <dnn:Label ID="plThreshold" runat="server" ControlName="ddlThreshold" Suffix=":" />
            </td>
            <td class="Normal">
                <asp:DropDownList ID="ddlThreshold" runat="server">
                </asp:DropDownList>&nbsp;
                <asp:Label ID="lblIn" runat="server" Font-Bold="True" resourcekey="In">in</asp:Label>&nbsp;
                <asp:DropDownList ID="ddlThresholdNotificationTime" runat="server">
                    <asp:ListItem Value="1">
                    </asp:ListItem>
                    <asp:ListItem Value="2">
                    </asp:ListItem>
                    <asp:ListItem Value="3">
                    </asp:ListItem>
                    <asp:ListItem Value="4">
                    </asp:ListItem>
                    <asp:ListItem Value="5">
                    </asp:ListItem>
                    <asp:ListItem Value="6">
                    </asp:ListItem>
                    <asp:ListItem Value="7">
                    </asp:ListItem>
                    <asp:ListItem Value="8">
                    </asp:ListItem>
                    <asp:ListItem Value="9">
                    </asp:ListItem>
                    <asp:ListItem Value="10">
                    </asp:ListItem>
                    <asp:ListItem Value="20">
                    </asp:ListItem>
                    <asp:ListItem Value="30">
                    </asp:ListItem>
                    <asp:ListItem Value="60">
                    </asp:ListItem>
                    <asp:ListItem Value="90">
                    </asp:ListItem>
                    <asp:ListItem Value="120">
                    </asp:ListItem>
                </asp:DropDownList>
                <asp:DropDownList ID="ddlThresholdNotificationTimeType" runat="server">
                    <asp:ListItem resourcekey="Seconds" Value="1">Seconds</asp:ListItem>
                    <asp:ListItem resourcekey="Minutes" Value="2">Minutes</asp:ListItem>
                    <asp:ListItem resourcekey="Hours" Value="3">Hours</asp:ListItem>
                    <asp:ListItem resourcekey="Days" Value="4">Days</asp:ListItem>
                </asp:DropDownList></td>
        </tr>
        <tr>
            <td class="SubHead" width="180">
                <dnn:Label ID="plMailFromAddress" runat="server" ControlName="txtMailFromAddress" Suffix=":" />
            </td>
            <td class="Normal">
                <asp:TextBox ID="txtMailFromAddress" runat="server" Width="300"></asp:TextBox></td>
        </tr>
        <tr>
            <td class="SubHead" width="180">
                <dnn:Label ID="plMailToAddress" runat="server" ControlName="txtMailToAddress" Suffix=":" />
            </td>
            <td class="Normal">
                <asp:TextBox ID="txtMailToAddress" runat="server" Width="300"></asp:TextBox></td>
        </tr>
    </table>
    <p>
        <asp:LinkButton ID="cmdUpdate" runat="server" CssClass="CommandButton" OnClick="cmdUpdate_Click" resourcekey="cmdUpdate" Text="Update"></asp:LinkButton>&nbsp;
        <asp:LinkButton ID="cmdCancel" runat="server" BorderStyle="none" CausesValidation="False" CssClass="CommandButton" OnClick="cmdCancel_Click" resourcekey="cmdCancel"
            Text="Cancel"></asp:LinkButton>&nbsp;
        <asp:LinkButton ID="cmdDelete" runat="server" CausesValidation="False" CssClass="CommandButton" OnClick="cmdDelete_Click" resourcekey="cmdDelete" Text="Delete"></asp:LinkButton>&nbsp;
    </p>
</asp:Panel>
