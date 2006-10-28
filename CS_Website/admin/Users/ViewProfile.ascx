<%@ Control Language="C#" CodeFile="ViewProfile.ascx.cs" AutoEventWireup="true" Inherits="DotNetNuke.Modules.Admin.Users.ViewProfile" %>
<%@ Register TagPrefix="dnn" TagName="Profile" Src="~/Admin/Users/ProfileModule.ascx" %>
<dnn:Profile id="ctlProfile" runat="server" EditorMode="View" ShowUpdate="False" />
