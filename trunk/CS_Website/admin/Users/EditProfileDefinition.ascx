<%@ Control Inherits="DotNetNuke.Modules.Admin.Users.EditProfileDefinition" CodeFile="EditProfileDefinition.ascx.cs" Language="C#" AutoEventWireup="true" %>
<%@ Register TagPrefix="dnn" Assembly="DotNetNuke" Namespace="DotNetNuke.UI.WebControls"%>

<dnn:propertyeditorcontrol id="Properties" runat="Server"
    SortMode="SortOrderAttribute"
	labelstyle-cssclass="SubHead" 
	helpstyle-cssclass="Help" 
	editcontrolstyle-cssclass="NormalTextBox" 
	labelwidth="200px" 
	editcontrolwidth="250px" 
	width="450px"/>
<p>
	<dnn:commandbutton class="CommandButton" id="cmdUpdate" imageUrl="~/images/save.gif" resourcekey="cmdUpdate" runat="server" text="Update"/>&nbsp;
	<dnn:commandbutton class="CommandButton" id="cmdCancel" imageUrl="~/images/lt.gif" resourcekey="cmdCancel" runat="server" text="Cancel" causesvalidation="False" />&nbsp;
	<dnn:commandbutton class="CommandButton" id="cmdDelete" imageUrl="~/images/delete.gif" resourcekey="cmdDelete" runat="server" text="Delete" causesvalidation="False" />
</p>
