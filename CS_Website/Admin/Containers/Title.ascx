<%@ Control Language="C#" AutoEventWireup="true" Inherits="DotNetNuke.UI.Containers.Title" CodeFile="Title.ascx.cs" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.UI.WebControls" Assembly="DotNetNuke.WebControls" %>
<dnn:DNNLabelEdit id="lblTitle" runat="server" cssclass="Head" enableviewstate="False" MouseOverCssClass="LabelEditOverClass"
	LabelEditCssClass="LabelEditTextClass" EditEnabled="True" OnUpdateLabel="lblTitle_UpdateLabel"></dnn:DNNLabelEdit>