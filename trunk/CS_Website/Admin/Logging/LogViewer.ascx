<%@ Register Assembly="DotNetNuke" Namespace="DotNetNuke.UI.WebControls" TagPrefix="dnnsc" %>
<%@ Register Src="~/controls/SectionHeadControl.ascx" TagName="SectionHead" TagPrefix="dnn" %>
<%@ Register Src="~/controls/LabelControl.ascx" TagName="Label" TagPrefix="dnn" %>
<%@ Control AutoEventWireup="true" CodeFile="LogViewer.ascx.cs"  Inherits="DotNetNuke.Modules.Admin.Log.LogViewer" Language="C#" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<style type="text/css">  
	.Exception { COLOR: #ffffff; BACKGROUND-COLOR: #ff1414 }  
	.ItemCreated { COLOR: #ffffff; BACKGROUND-COLOR: #009900 }  
	.ItemUpdated { COLOR: #ffffff; BACKGROUND-COLOR: #009999 }  
	.ItemDeleted { COLOR: #000000; BACKGROUND-COLOR: #14ffff }  
	.OperationSuccess { COLOR: #ffffff; BACKGROUND-COLOR: #999900 }  
	.OperationFailure { COLOR: #ffffff; BACKGROUND-COLOR: #990000 }  
	.GeneralAdminOperation { COLOR: #ffffff; BACKGROUND-COLOR: #4d0099 }  
	.AdminAlert { COLOR: #ffffff; BACKGROUND-COLOR: #148aff }  
	.HostAlert { COLOR: #ffffff; BACKGROUND-COLOR: #ff8a14 }  
	.SecurityException { COLOR: #ffffff; BACKGROUND-COLOR: #000000 }  
	#floater { PADDING-RIGHT: 0px; PADDING-LEFT: 0px; BACKGROUND: #ffffff; VISIBILITY: hidden; PADDING-BOTTOM: 0px; MARGIN: 0px; WIDTH: 150px; COLOR: #ffffff; PADDING-TOP: 0px; POSITION: absolute; HEIGHT: auto }  
</style>
<!--
Other colors that match the scheme above:
#990000
#994D00
#999900
#4D9900
#99004D
#D60000
#FF1414
#009900
#990099
#14FFFF
#00D6D6
#00994D
#4D0099
#000099
#004D99
#009999
-->

<script language="JavaScript" type="text/javascript">
<!--
function CheckExceptions()
{
    var j,isChecked = false;
    if (document.forms[0].item("Exception").length)
        {
            j=document.forms[0].item("Exception").length;
            for (var i=0;i<j;i++)
                {
                    if (document.forms[0].item("Exception")(i).checked==true)
                    {
                        isChecked = true;
                    }
                }
            if (isChecked!=true)
                {
                    alert('Please select at least one exception.');
                }
            return isChecked;
        }
    else 
        {
            if (document.forms[0].item("Exception").checked)
                return true;
            else
                {
                alert('Please select at least one exception.');
                return false;
                }
        }
}

function flipFlop(eTarget) {
    if (document.getElementById(eTarget).style.display=='')
    {
    	document.getElementById(eTarget).style.display='none';
    }
    else
    {
    	document.getElementById(eTarget).style.display='';
    }
}
  
//-->
</script>

<asp:Panel ID="pnlOptions" runat="server">
    <table border="0" width="100%">
        <tr>
            <td valign="top">
                <dnn:SectionHead ID="dshSettings" runat="server" CssClass="Head" ResourceKey="Settings" Section="tblSettings" Text="Viewer Settings" />
                <table id="tblSettings" runat="server" border="0" cellpadding="2" cellspacing="2">
                    <tr>
                        <td align="left" class="SubHead" nowrap="nowrap" width="110">
                            <dnn:Label ID="plPortalID" runat="server" ControlName="ddlPortalid" Suffix=":" />
                        </td>
                        <td width="60">
                            <asp:DropDownList ID="ddlPortalid" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlPortalID_SelectedIndexChanged">
                            </asp:DropDownList></td>
                        <td width="25">
                            &nbsp;</td>
                        <td align="left" class="SubHead" width="100">
                            <dnn:Label ID="plLogType" runat="server" ControlName="ddlLogType" Suffix=":" />
                        </td>
                        <td width="100">
                            <asp:DropDownList ID="ddlLogType" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlLogType_SelectedIndexChanged">
                            </asp:DropDownList></td>
                    </tr>
                    <tr>
                        <td align="left" class="SubHead" nowrap="nowrap">
                            <dnn:Label ID="plRecordsPage" runat="server" cssclass="SubHead" ResourceKey="Recordsperpage" Suffix=":" />
                        </td>
                        <td width="25">
                            <asp:DropDownList ID="ddlRecordsPerPage" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlRecordsPerPage_SelectedIndexChanged">
                                <asp:ListItem Value="10">10</asp:ListItem>
                                <asp:ListItem Value="25">25</asp:ListItem>
                                <asp:ListItem Value="50">50</asp:ListItem>
                                <asp:ListItem Value="100">100</asp:ListItem>
                                <asp:ListItem Value="250">250</asp:ListItem>
                            </asp:DropDownList></td>
                        <td width="25">
                            &nbsp;</td>
                        <td class="SubHead" colspan="2">
                            <asp:CheckBox ID="chkColorCoding" runat="server" AutoPostBack="true" resourcekey="ColorCoding" Text="Color Coding On" OnCheckedChanged="chkColorCoding_CheckedChanged" /></td>
                        <td width="*">
                            &nbsp;</td>
                    </tr>
                    <tr>
                        <td colspan="5">
                            <table id="tblInstructions" runat="server" border="0" width="100%">
                                <tr height="10">
                                    <td>
                                    </td>
                                </tr>
                                <tr>
                                    <td align="center" class="Normal">
                                        <asp:Label ID="lbClickRow" runat="server" resourcekey="ClickRow">Click on a row for details.</asp:Label><br />
                                    </td>
                                </tr>
                                <tr>
                                    <td align="center" class="NormalBold">
                                        <asp:Label ID="txtShowing" runat="server"></asp:Label></td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </td>
            <td width="*">
                &nbsp;</td>
            <td align="left" class="SubHead" rowspan="3" valign="top" width="230">
                <asp:Panel ID="pnlLegend" runat="server" HorizontalAlign="Center">
                    <dnn:SectionHead ID="dshLegend" runat="server" CssClass="Head" IsExpanded="False" ResourceKey="Legend" Section="tblLegend" Text="Color Coding Legend" />
                    <table id="tblLegend" runat="server" bgcolor="#000000" border="0" cellpadding="2" cellspacing="2">
                        <tr>
                            <td bgcolor="#ffffff">
                                <table border="0">
                                    <tr>
                                        <td>
                                            <table bgcolor="#000000" border="0" cellpadding="3" cellspacing="1">
                                                <tr>
                                                    <td bgcolor="#ff1414" class="Normal">
                                                        <img height="5" src="images/spacer.gif" width="5" /></td>
                                                </tr>
                                            </table>
                                        </td>
                                        <td class="Normal">
                                            <asp:Label ID="Label1" runat="server" resourcekey="ExceptionCode">Exception</asp:Label></td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <table bgcolor="#000000" border="0" cellpadding="3" cellspacing="1">
                                                <tr>
                                                    <td bgcolor="#009900" class="Normal">
                                                        <img height="5" src="images/spacer.gif" width="5" /></td>
                                                </tr>
                                            </table>
                                        </td>
                                        <td class="Normal">
                                            <asp:Label ID="Label2" runat="server" resourcekey="ItemCreatedCode">Item Created</asp:Label></td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <table bgcolor="#000000" border="0" cellpadding="3" cellspacing="1">
                                                <tr>
                                                    <td bgcolor="#009999" class="Normal">
                                                        <img height="5" src="images/spacer.gif" width="5" /></td>
                                                </tr>
                                            </table>
                                        </td>
                                        <td class="Normal">
                                            <asp:Label ID="Label3" runat="server" resourcekey="ItemUpdatedCode">Item Updated</asp:Label></td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <table bgcolor="#000000" border="0" cellpadding="3" cellspacing="1">
                                                <tr>
                                                    <td bgcolor="#14ffff" class="Normal">
                                                        <img height="5" src="images/spacer.gif" width="5" /></td>
                                                </tr>
                                            </table>
                                        </td>
                                        <td class="Normal">
                                            <asp:Label ID="Label4" runat="server" resourcekey="ItemDeletedCode">Item Deleted</asp:Label></td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <table bgcolor="#000000" border="0" cellpadding="3" cellspacing="1">
                                                <tr>
                                                    <td bgcolor="#999900" class="Normal">
                                                        <img height="5" src="images/spacer.gif" width="5" /></td>
                                                </tr>
                                            </table>
                                        </td>
                                        <td class="Normal">
                                            <asp:Label ID="Label5" runat="server" resourcekey="SuccessCode">Operation Success</asp:Label></td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <table bgcolor="#000000" border="0" cellpadding="3" cellspacing="1">
                                                <tr>
                                                    <td bgcolor="#990000" class="Normal">
                                                        <img height="5" src="images/spacer.gif" width="5" /></td>
                                                </tr>
                                            </table>
                                        </td>
                                        <td class="Normal">
                                            <asp:Label ID="Label6" runat="server" resourcekey="FailureCode">Operation Failure</asp:Label></td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <table bgcolor="#000000" border="0" cellpadding="3" cellspacing="1">
                                                <tr>
                                                    <td bgcolor="#4d0099" class="Normal">
                                                        <img height="5" src="images/spacer.gif" width="5" /></td>
                                                </tr>
                                            </table>
                                        </td>
                                        <td class="Normal">
                                            <asp:Label ID="Label7" runat="server" resourcekey="AdminOpCode">General Admin Operation</asp:Label></td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <table bgcolor="#000000" border="0" cellpadding="3" cellspacing="1">
                                                <tr>
                                                    <td bgcolor="#148aff" class="Normal">
                                                        <img height="5" src="images/spacer.gif" width="5" /></td>
                                                </tr>
                                            </table>
                                        </td>
                                        <td class="Normal">
                                            <asp:Label ID="Label8" runat="server" resourcekey="AdminAlertCode">Admin Alert</asp:Label></td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <table bgcolor="#000000" border="0" cellpadding="3" cellspacing="1">
                                                <tr>
                                                    <td bgcolor="#ff8a14" class="Normal">
                                                        <img height="5" src="images/spacer.gif" width="5" /></td>
                                                </tr>
                                            </table>
                                        </td>
                                        <td class="Normal">
                                            <asp:Label ID="Label9" runat="server" resourcekey="HostAlertCode">Host Alert</asp:Label></td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <table bgcolor="#000000" border="0" cellpadding="3" cellspacing="1">
                                                <tr>
                                                    <td bgcolor="#000000" class="Normal">
                                                        <img height="5" src="images/spacer.gif" width="5" /></td>
                                                </tr>
                                            </table>
                                        </td>
                                        <td class="Normal">
                                            <asp:Label ID="Label10" runat="server" resourcekey="SecurityException">Security Exception</asp:Label></td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </table>
                </asp:Panel>
            </td>
        </tr>
    </table>
    <br />
</asp:Panel>
<br />
<asp:DataList ID="dlLog" runat="server" BackColor="#CFCFCF" CellPadding="0" CellSpacing="1" EnableViewState="False" Width="100%">
    <ItemStyle BorderWidth="0" />
    <HeaderStyle BackColor="#CFCFCF" BorderWidth="0" />
    <HeaderTemplate>
        <span class="NormalBold" style="width: 20; background: #CFCFCF;">&nbsp;</span>
        <asp:Label ID="lblDateHeader" runat="server" class="NormalBold" resourcekey="Date" Style="width: 150; background: #CFCFCF;">&nbsp;Date</asp:Label>
        <asp:Label ID="lblTypeHeader" runat="server" class="NormalBold" resourcekey="Type" Style="width: 200; background: #CFCFCF;">&nbsp;Log Type</asp:Label>
        <asp:Label ID="lblUserHeader" runat="server" class="NormalBold" resourcekey="Username" Style="width: 100; background: #CFCFCF;">&nbsp;Username</asp:Label>
        <asp:Label ID="lblPortalHeader" runat="server" class="NormalBold" resourcekey="Portal" Style="width: 150; background: #CFCFCF;">&nbsp;Portal</asp:Label>
        <asp:Label ID="lblSummaryHeader" runat="server" class="NormalBold" resourcekey="Summary" Style="width: 270; background: #CFCFCF;">&nbsp;Summary</asp:Label>
    </HeaderTemplate>
    <ItemTemplate>
        <table border="0" cellpadding="0" cellspacing="0" width="100%">
            <tr class='<%# GetMyLogType(Convert.ToString(DataBinder.Eval(Container.DataItem,"LogTypeKey"))).LogTypeCSSClass %>'>
                <td align="center" valign="middle" width="20">
                    <input name="Exception" type="checkbox" value='<%# ((DotNetNuke.Services.Log.EventLog.LogInfo)Container.DataItem).LogGUID %>|<%# ((DotNetNuke.Services.Log.EventLog.LogInfo)Container.DataItem).LogFileID %>' /></td>
                <td nowrap="nowrap" onclick="flipFlop('<%# ((DotNetNuke.Services.Log.EventLog.LogInfo)Container.DataItem).LogGUID %>')">
                    <span class="Normal" style="width: 150; overflow: hidden;">&nbsp;
                        <asp:Label ID="lblDate" runat="server" EnableViewState="False" Text='<%# DataBinder.Eval(Container.DataItem,"LogCreateDate") %>'></asp:Label></span> <span class="Normal"
                            style="width: 200; overflow: hidden;">&nbsp;
                            <asp:Label ID="lblType" runat="server" EnableViewState="False" Text='<%# GetMyLogType(Convert.ToString(DataBinder.Eval(Container.DataItem,"LogTypeKey"))).LogTypeFriendlyName %>'>
                            </asp:Label></span> <span class="Normal" style="width: 100; overflow: hidden;">&nbsp;
                                <asp:Label ID="lblUserName" runat="server" EnableViewState="False" Text='<%# DataBinder.Eval(Container.DataItem,"LogUserName") %>'></asp:Label></span>
                    <span class="Normal" style="width: 150; overflow: hidden;">&nbsp;
                        <asp:Label ID="lblPortal" runat="server" EnableViewState="False" Text='<%# DataBinder.Eval(Container.DataItem,"LogPortalName") %>'></asp:Label></span> <span class="Normal"
                            style="width: 280; overflow: hidden;">&nbsp;
                            <asp:Label ID="lblSummary" runat="server" EnableViewState="False" Text='<%# ((DotNetNuke.Services.Log.EventLog.LogInfo)Container.DataItem).LogProperties.Summary %>'>
                            </asp:Label>&nbsp;...</span>
                </td>
            </tr>
            <tr id='<%# ((DotNetNuke.Services.Log.EventLog.LogInfo)Container.DataItem).LogGUID%>' style="display: none;">
                <td bgcolor="#FFFFFF" colspan="2">
                    <table bgcolor="#000000" border="0" cellpadding="5" cellspacing="1" width="100%">
                        <tr>
                            <td bgcolor="#CFCFCF">
                                <asp:Label ID="lblException" runat="server" class="Normal" EnableViewState="False" Text='<%# GetPropertiesText(Container.DataItem) %>'></asp:Label>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
    </ItemTemplate>
</asp:DataList>
<dnnsc:PagingControl ID="ctlPagingControlBottom" runat="server" />
<p>
    <asp:LinkButton ID="btnDelete" runat="server" CssClass="CommandButton" resourcekey="btnDelete" OnClick="btnDelete_Click">Delete Selected Exceptions</asp:LinkButton>
    &nbsp;&nbsp;
    <asp:LinkButton ID="btnClear" runat="server" CssClass="CommandButton" resourcekey="btnClear" OnClick="btnClear_Click">Clear Log</asp:LinkButton>
</p>
<asp:Panel ID="pnlSendExceptions" runat="server" CssClass="Normal">
    <dnn:SectionHead ID="dshSendExceptions" runat="server" CssClass="Head" IncludeRule="True" IsExpanded="False" ResourceKey="SendExceptions" Section="tblSendExceptions"
        Text="Send Exceptions" />
    <table id="tblSendExceptions" runat="server" border="0" cellpadding="2" cellspacing="2" width="560">
        <tr>
            <td colspan="3">
                <asp:Label ID="lblExceptionsWarning" runat="server" class="normal" resourcekey="ExceptionsWarning">
          <B>Please note</B>: By using these features 
            below, you <I>may</I> be sending sensitive data over the Internet in clear 
            text (<I>not</I> encrypted). Before sending your exception submission, 
            please review the contents of your exception log to verify that no 
            sensitive data is contained within it. Only the log entries checked above 
            will be sent.
                </asp:Label>
                <hr size="1" />
                <asp:RadioButtonList ID="optEmailType" runat="server" AutoPostBack="False" CssClass="Normal" RepeatDirection="Horizontal">
                    <asp:ListItem resourcekey="ToEmail.Text" Selected="True" Value="Email">To Specified Email Address</asp:ListItem>
                </asp:RadioButtonList></td>
        </tr>
        <tr>
            <td align="left" class="SubHead" width="200">
                <dnn:Label ID="plEmailAddress" runat="server" ControlName="txtEmailAddress" Suffix=":" />
            </td>
            <td width="200">
                <asp:TextBox ID="txtEmailAddress" runat="server"></asp:TextBox></td>
            <td width="*">
                &nbsp;</td>
        </tr>
        <tr>
            <td align="left" class="SubHead" width="200">
                <dnn:Label ID="plMessage" runat="server" ControlName="txtMessage" ResourceKey="SendMessage" Suffix=":" />
            </td>
            <td width="200">
                <asp:TextBox ID="txtMessage" runat="server" Columns="25" Rows="6" TextMode="MultiLine"></asp:TextBox></td>
            <td width="*">
                &nbsp;</td>
        </tr>
        <tr>
            <td colspan="3">
                <asp:LinkButton ID="btnEmail" runat="server" CssClass="CommandButton" resourcekey="btnEmail" OnClick="btnEmail_Click">Send Selected Exceptions</asp:LinkButton></td>
        </tr>
    </table>
</asp:Panel>
