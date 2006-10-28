<%@ Control language="vb" CodeBehind="~/admin/Containers/container.vb" AutoEventWireup="false" Explicit="True" Inherits="DotNetNuke.UI.Containers.Container" %>
<%@ Register TagPrefix="dnn" TagName="ACTIONS" Src="~/Admin/Containers/SolPartActions.ascx" %>
<%@ Register TagPrefix="dnn" TagName="ICON" Src="~/Admin/Containers/Icon.ascx" %>
<%@ Register TagPrefix="dnn" TagName="TITLE" Src="~/Admin/Containers/Title.ascx" %>
<%@ Register TagPrefix="dnn" TagName="VISIBILITY" Src="~/Admin/Containers/Visibility.ascx" %>
<%@ Register TagPrefix="dnn" TagName="ACTIONBUTTON1" Src="~/Admin/Containers/ActionButton.ascx" %>
<%@ Register TagPrefix="dnn" TagName="ACTIONBUTTON2" Src="~/Admin/Containers/ActionButton.ascx" %>
<%@ Register TagPrefix="dnn" TagName="ACTIONBUTTON3" Src="~/Admin/Containers/ActionButton.ascx" %>
<%@ Register TagPrefix="dnn" TagName="ACTIONBUTTON4" Src="~/Admin/Containers/ActionButton.ascx" %>
<%@ Register TagPrefix="dnn" TagName="ACTIONBUTTON5" Src="~/Admin/Containers/ActionButton.ascx" %>
      <TABLE class="containermaster_gray" cellSpacing="0" cellPadding="5" align="center" border="0">
        <TR>
          <TD>
            <TABLE width="100%" border="0" cellpadding="0" cellspacing="0">
              <TR>
                <TD valign="middle" nowrap><dnn:ACTIONS runat="server" id="dnnACTIONS" /></TD>
                <TD valign="middle" nowrap><dnn:ICON runat="server" id="dnnICON" /></TD>
                <TD valign="middle" width="100%" nowrap>&nbsp;<dnn:TITLE runat="server" id="dnnTITLE" /></TD>
                <TD valign="middle" nowrap><dnn:VISIBILITY runat="server" id="dnnVISIBILITY" /><dnn:ACTIONBUTTON5 runat="server" id="dnnACTIONBUTTON5" CommandName="ModuleHelp.Action" DisplayIcon="True" DisplayLink="False" /></TD>
              </TR>
            </TABLE>
            <HR class="containermaster_gray">
          </TD>
        </TR>
        <TR>
          <TD id="ContentPane" runat="server" align="center"></TD>
        </TR>
        <TR>
          <TD>
            <HR class="containermaster_gray">
            <TABLE width="100%" border="0" cellpadding="0" cellspacing="0">
              <TR>
                <TD align="left" valign="middle" nowrap><dnn:ACTIONBUTTON1 runat="server" id="dnnACTIONBUTTON1" CommandName="AddContent.Action" DisplayIcon="True" DisplayLink="True" /></TD>
                <TD align="right" valign="middle" nowrap><dnn:ACTIONBUTTON2 runat="server" id="dnnACTIONBUTTON2" CommandName="SyndicateModule.Action" DisplayIcon="True" DisplayLink="False" />&nbsp;<dnn:ACTIONBUTTON3 runat="server" id="dnnACTIONBUTTON3" CommandName="PrintModule.Action" DisplayIcon="True" DisplayLink="False" />&nbsp;<dnn:ACTIONBUTTON4 runat="server" id="dnnACTIONBUTTON4" CommandName="ModuleSettings.Action" DisplayIcon="True" DisplayLink="False" /></TD>
              </TR>
            </TABLE>
          </TD>
        </TR>
      </TABLE>
      <BR>
