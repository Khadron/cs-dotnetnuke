<%@ Import Namespace="DotNetNuke.Services.Localization" %>
<%@ Import Namespace="DotNetNuke.UI.Utilities" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Security.Permissions.Controls"
    Assembly="DotNetNuke" %>
<%@ Control Inherits="DotNetNuke.Modules.Admin.FileSystem.FileManager" Language="C#"
    AutoEventWireup="true" Explicit="True" CodeFile="FileManager.ascx.cs" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register TagPrefix="dnn" TagName="SectionHead" Src="~/controls/SectionHeadControl.ascx" %>
<%@ Register TagPrefix="dnntv" Namespace="DotNetNuke.UI.WebControls" Assembly="DotNetNuke.WebControls" %>

<script language="javascript">
//Localization Vars
var m_sLocaleOk = '<%=ClientAPI.GetSafeJSString(Localization.GetString("Ok", LocalResourceFile))%>';
var m_sLocaleCancel = '<%=ClientAPI.GetSafeJSString(Localization.GetString("Cancel", LocalResourceFile))%>';
var m_sLocaleCopyFiles = '<%=ClientAPI.GetSafeJSString(Localization.GetString("CopyFiles", LocalResourceFile))%>';
var m_sLocaleCancelCopy = '<%=ClientAPI.GetSafeJSString(Localization.GetString("CancelCopy", LocalResourceFile))%>';
var m_sLocaleMoveFiles = '<%=ClientAPI.GetSafeJSString(Localization.GetString("MoveFiles", LocalResourceFile))%>';
var m_sLocaleCancelMove = '<%=ClientAPI.GetSafeJSString(Localization.GetString("CancelMove", LocalResourceFile))%>';
var m_sLocaleNoFilesChecked = '<%=ClientAPI.GetSafeJSString(Localization.GetString("NoFilesChecked", LocalResourceFile))%>';
var m_sLocaleSpecifyFolder = '<%=ClientAPI.GetSafeJSString(Localization.GetString("PleaseSpecifyAFolderNameFirst", LocalResourceFile))%>';
var m_sLocaleCopyCheckedFiles = '<%=ClientAPI.GetSafeJSString(Localization.GetString("CopyCheckedFiles", LocalResourceFile))%>';
var m_sLocaleMoveCheckedFiles = '<%=ClientAPI.GetSafeJSString(Localization.GetString("MoveCheckedFiles", LocalResourceFile))%>';
var m_sLocaleDeleteFolder = '<%=ClientAPI.GetSafeJSString(Localization.GetString("DeleteFolder", LocalResourceFile))%>';
var m_sLocaleMoveSelectedFilesTo = '<%=ClientAPI.GetSafeJSString(Localization.GetString("MoveSelectedFilesTo", LocalResourceFile))%>';
var m_sLocaleCopySelectedFilesTo = '<%=ClientAPI.GetSafeJSString(Localization.GetString("CopySelectedFilesTo", LocalResourceFile))%>';
var m_sLocaleUnzipSelectedFilesTo = '<%=ClientAPI.GetSafeJSString(Localization.GetString("UnzipSelectedFilesTo", LocalResourceFile))%>';
var m_sLocaleSelectAll = '<%=ClientAPI.GetSafeJSString(Localization.GetString("SelectAll", LocalResourceFile))%>';
var m_sLocaleUnSelectAll = '<%=ClientAPI.GetSafeJSString(Localization.GetString("UnSelectAll", LocalResourceFile))%>';

//Localization Vars


var m_bAllFilesChecked;
var previd;
var blpb=true;
var m_sDNNTreeID;
var m_sUCPrefixID;
var m_sUCPrefixName;
var m_arReplaceTitle = new Array(1);
m_arReplaceTitle[0] = '<%=ClientAPI.GetSafeJSString(RootFolderName) %>';

function getLastPath() {return dnn.getVar('LastPath');}
function setLastPath(sValue) {dnn.setVar('LastPath', sValue);}
function getDestPath() {return dnn.getVar('DestPath');}
function setDestPath(sValue) {dnn.setVar('DestPath', sValue);}
function getSourcePath() {return dnn.getVar('SourcePath');}
function setSourcePath(sValue) {dnn.setVar('SourcePath', sValue);}

function getMoveFiles() {return dnn.getVar('MoveFiles');}
function setMoveFiles(sValue) {dnn.setVar('MoveFiles', sValue);}
function getMoveStatus() {return dnn.getVar('MoveStatus');}
function setMoveStatus(sValue) {dnn.setVar('MoveStatus', sValue);}

function addFileToMoveList(strfile, sender, strSelClass, strOrigClass) 
{

	if (sender.tagName.toUpperCase()=="INPUT") 
	{
		/* 1.0 Framework */
		var blvalue = sender.checked;
		var objRow = sender.parentNode.parentNode.parentNode;
	}
	else
	{ /* 1.1 FrameWork */
		var blvalue = sender.children[0].checked;

		var objRow = sender.parentNode.parentNode;
	}

	if(!blvalue)
	{
		//unchecking
		setMoveFiles(getMoveFiles().replace(strfile + ";", ""));
		objRow.className=strOrigClass;
	}
	else 
	{
		//checking
		setMoveFiles(getMoveFiles() + strfile + ";");
		objRow.className = strSelClass;
	}
}

function canAddFolder() 
{
	var txtNewFolder1 = dnn.dom.getById(m_sUCPrefixID + 'txtNewFolder');
	if (txtNewFolder1.value=='')
	{
		alert(m_sLocaleSpecifyFolder);
		return false;
	}
	else 
		__doPostBack(m_sUCPrefixName + 'lnkAddFolder', '');
}

function cancelMove() 
{
	clearErrorMessage();
	setMoveStatus('');
	blCopying=false;
	CheckAllFiles(false);
	dnn.dom.getById(m_sUCPrefixID + 'lblCurFolder').innerHTML = getSourcePath().replace(m_arReplaceTitle[0],m_arReplaceTitle[1]) + '\\';
}

function clearErrorMessage()
{
	disableButtons(false);
	var tdGrid = dnn.dom.getById('tdGrid');
	tdGrid.style.display='';
	var FileOverlay = dnn.dom.getById('FileGridOverLay');
	FileOverlay.innerHTML='';
	dnn.dom.getById('tdOverLay').style.display='none';
	dnn.setVar('ErrorMessage', '');
}

function confirmMoveFiles(strDestFolder) 
{
	hideDataGrid();

	if (getMoveStatus()=='move') 
		var strConfirmTitle = m_sLocaleMoveSelectedFilesTo;
	else if (getMoveStatus()=='copy') 
		var strConfirmTitle = m_sLocaleCopySelectedFilesTo;
	else if (getMoveStatus()=='unzip') 
		var strConfirmTitle = m_sLocaleUnzipSelectedFilesTo;

	var strConfirmMessage='<table cellspacing=0 cellpadding=0><tr><td class=NormalBold>' + getProperPath(strDestFolder) + '</td></tr>';
	strConfirmMessage+='<tr><td height=15>&nbsp;</td><tr>'
	strConfirmMessage+='<tr><td align=center>';
	strConfirmMessage+='<INPUT id=btnMoveOK style="width:82px;" Class="NormalBold" onclick="__doPostBack(\'' + m_sUCPrefixName + 'lnkMoveFiles' + '\');" type=button value="' + m_sLocaleOk + '">&nbsp;&nbsp;&nbsp;&nbsp;';
	strConfirmMessage+='<INPUT id=btnNoConfirmMove style="width:82px;" Class="NormalBold" onclick=hideDataGrid(); type=button value="' + m_sLocaleCancel + '">';
	strConfirmMessage+='</td></tr></table>';
	showErrorMessage(strConfirmTitle, strConfirmMessage);
}

function copyCheckedFiles()
{
	if (getMoveFiles() =='')
	{
		alert(m_sLocaleNoFilesChecked);
		return false;
	}
	//confirm the copy
	var blconfirm = confirm(m_sLocaleCopyCheckedFiles);
	if (blconfirm) 
	{
		setSourcePath(getLastPath());
		setMoveStatus('copy');
		hideDataGrid();
		return false;
	}
}

function deleteAllChecked()
{
	clearErrorMessage();
	__doPostBack(m_sUCPrefixName + 'lnkDeleteAllCheckedFiles', '');
}

function deleteCheckedFiles()
{
	if (getMoveFiles() =='')
	{
		alert(m_sLocaleNoFilesChecked);
		return false;
	}
  
	var strTitle='<%=ClientAPI.GetSafeJSString(Localization.GetString("DeleteFiles", LocalResourceFile))%>:';
	var strMessage='<table cellspacing=0 cellpadding=0>';
	var strCurFiles = getMoveFiles();
	var arFiles = strCurFiles.split(';');
	for( var i = 0; i < arFiles.length-1; i++ )
		strMessage+='<tr><td class=NormalBold>' + arFiles[i] + '</td></tr>';

	strMessage+='<tr><td height=15>&nbsp;</td><tr>';
	strMessage+='<tr><td colspan=2 align=center>';
	strMessage+='<INPUT id=btnConfirmDel style="WIDTH: 81px;" Class="NormalBold" onclick=deleteAllChecked(); type=button value="' + m_sLocaleOk + '">&nbsp;&nbsp;&nbsp;&nbsp;';
	strMessage+='<INPUT id=btnClearError style="WIDTH: 81px;" Class="NormalBold" onclick=clearErrorMessage(); type=button value="' + m_sLocaleCancel + '">';
	strMessage+='</td></tr></table>';
	showErrorMessage(strTitle, strMessage);
	return false;
}

function deleteFolder() 
{
	var blValue = confirm(m_sLocaleDeleteFolder + ' ' + getProperPath(getLastPath()) + '?');
	if (blValue) {__doPostBack(m_sUCPrefixName + 'lnkDeleteFolder', '');}else{return false;}
}

function disableButtons(blvalue)
{

	var objNewFolderID = dnn.dom.getById(m_sUCPrefixID + 'txtNewFolder');
	if (objNewFolderID) 
	{
		objNewFolderID.disabled=blvalue;
		enableDisableCtl('lnkAddFolderIMG', blvalue);
		enableDisableCtl('lnkDelFolderIMG', blvalue);
	}
	dnn.dom.getById(m_sUCPrefixID + 'txtFilter').disabled=blvalue;
	enableDisableCtl('lnkCopy', blvalue);
	enableDisableCtl('lnkDelete', blvalue);
	enableDisableCtl('lnkMove', blvalue);
	enableDisableCtl('lnkUploadIMG', blvalue);
	enableDisableCtl('lnkFilterIMG', blvalue);
	//enableDisableCtl('lnkRefreshIMG', blvalue);
	enableDisableCtl('lnkFolderPropertiesIMG', blvalue);

	obj=dnn.dom.getById(m_sUCPrefixID + 'tblMessagePager')
	if (obj)
	{
		if (blvalue)
			obj.style.display='none';
		else
			obj.style.display='';
	}
	else
	{
	}
	obj=dnn.dom.getById(m_sUCPrefixID + 'selPageSize')
	if (obj)
		obj.disabled=blvalue;
}

function enableDisableCtl(sID, blvalue)
{
	var obj = dnn.dom.getById(sID);
	var strCursor = 'hand';
	var strSearch='Disabled';
	var strReplace='Enabled';
	var strStyle='';
	if (blvalue) 
	{
		strCursor = 'default';
		strSearch='Enabled';
		strReplace='Disabled';
		strStyle='none';
	}

	if (obj)
	{
		obj.style.cursor=strCursor;
		obj.disabled=blvalue;
		obj.parentNode.disabled=blvalue;
		obj.src=obj.src.replace(strSearch, strReplace);
	}
}

function fldScroll() 
{
	setFolderScrollPos(dnn.dom.getById(m_sUCPrefixID + 'pnlFolders').scrollTop);
}

function getFolderID(fldname) 
{
	var arvalues=fldname.split('\\');
	return arvalues[0];
}

function getFolderScrollPos()
{
	var i = dnn.getVar('folderScrollPos');
	if (i != null)
		return i;
	else
		return 0;
}

function getProperPath(s)
{
	s=s.replace('\/', '\\');
	var sFldID=getFolderID(s);
	return s.replace(sFldID,m_arReplaceTitle[sFldID]);
}

function getSelectedTreeNode()
{
	if (dnn.controls != null)
		return dnn.controls.controls[m_sDNNTreeID].selTreeNode;	
	else
	{
		eval('var oNode = ' + dnn.getVar(m_sDNNTreeID + '_selNode'));
		return oNode;
	}
}

function hideDataGrid() 
{

	//used when moving/copying files...
	dnn.dom.getById('tdOverLay').style.display='';
	var tdGrid = dnn.dom.getById('tdGrid');
	tdGrid.style.display='none';
	var FileOverlay = dnn.dom.getById('FileGridOverLay');
	FileOverlay.style.display='';

	var strCurFiles = getMoveFiles();
	
	var arFiles = strCurFiles.split(';');
	var strMessage='<table cellspacing=0 cellpadding=0>';
	for( var i = 0; i < arFiles.length-1; i++ )
		strMessage+='<tr><td class=NormalBold>' + arFiles[i] + '</td></tr>';

	strMessage=strMessage.replace(arFiles[arFiles.length-2]+',',arFiles[arFiles.length-2]);
	strMessage+= '<tr><td height=15>&nbsp;</td><tr>'
	strMessage+= '<tr><td class=NormalRed><%=ClientAPI.GetSafeJSString(Localization.GetString("FromFolder", LocalResourceFile))%>' + getProperPath(getLastPath());  + '</td></tr>'
	strMessage+= '<tr><td height=15>&nbsp;</td><tr>'
	strMessage+= '<tr><td class=NormalRed><%=ClientAPI.GetSafeJSString(Localization.GetString("ToFolder", LocalResourceFile))%></td></tr>'
	strMessage+= '<tr><td height=15>&nbsp;</td><tr>'
	
	if (getMoveStatus()=='move') 
	{
		var strTitle='<%=ClientAPI.GetSafeJSString(Localization.GetString("MovingFiles", LocalResourceFile))%>';
	}
	else if (getMoveStatus()=='copy') 
	{
		var strTitle='<%=ClientAPI.GetSafeJSString(Localization.GetString("CopyingFiles", LocalResourceFile))%>';
	}
	else if (getMoveStatus()=='unzip')
	{ 
		var strTitle='<%=ClientAPI.GetSafeJSString(Localization.GetString("UnzippingFile", LocalResourceFile))%>';
	}
	strMessage+= '<tr><td align=center><INPUT id=btnCancelMove style="width:82px;" class="NormalBold" onclick=cancelMove(); type=button value="' + m_sLocaleCancel + '"></td></tr>';
	strMessage+= '</table>'
	
	showErrorMessage(strTitle, strMessage);
}

function initFileManager()
{
	if (dnn.getVar('IsRefresh') == '0')	
		dnn.dom.getById(m_sUCPrefixID + 'pnlFolders').scrollTop=getFolderScrollPos();

	m_sUCPrefixID = dnn.getVar('UCPrefixID');
	m_sUCPrefixName = dnn.getVar('UCPrefixName');
	m_sDNNTreeID = m_sUCPrefixID + 'DNNTree';

	var oFileOverlay = dnn.dom.getById('FileGridOverLay');
	if (oFileOverlay.innerHTML.length == 0)
		dnn.dom.getById('tdOverLay').style.display='none';

	if (dnn.getVar('DisabledButtons') != '0')
		disableButtons(true);

	if (dnn.getVar('ErrorMessage') != null && dnn.getVar('ErrorMessage').length > 0)
		showErrorMessage('<%=ClientAPI.GetSafeJSString(Localization.GetString("ErrorOccurred", LocalResourceFile))%>', dnn.getVar('ErrorMessage'));

}

function moveFiles() 
{

	if (getMoveFiles() =='')
	{
		alert(m_sLocaleNoFilesChecked);
		return false;
	}
	var blconfirm = confirm(m_sLocaleMoveCheckedFiles);
	if (blconfirm) 
	{
		setSourcePath(getLastPath());
		setMoveStatus('move');
		hideDataGrid();
		return false;
	}
}

function nodeSelected()
{
	var oNode = getSelectedTreeNode();
	
	if (oNode != null)
	{
		var sKey = oNode.key;

		var sIsMoving = getMoveStatus();
		if ((sIsMoving=='copy')||(sIsMoving=='move')||(sIsMoving=='unzip'))
		{
			setDestPath(sKey);
			confirmMoveFiles(sKey);
			return false; //cancel postback
		}
		else
		{
			setLastPath(sKey);
			setDestPath(sKey);
			return true; //do postback
		}
	}
}

function setFolderScrollPos(sValue)
{
	dnn.setVar('folderScrollPos', sValue);
}

function showErrorMessage(strTitle, strMessage)
{
	var strOutput='<table width=80% cellpadding=0 cellspacing=0 style="border: 1px solid black">'
	strOutput+='<tr><td height=15></td></tr>'
	strOutput+='<tr><td class=Head align=center><u>' + strTitle + '</u></td></tr>'
	strOutput+='<tr><td height=15></td></tr>'
	strOutput+='<tr><td align=center valign=middle>' + strMessage + '</td></tr>'
	strOutput+='<tr><td height=15></td></tr></table>'

	disableButtons(true);
	dnn.dom.getById('tdOverLay').style.display='';
	var tdGrid = dnn.dom.getById('tdGrid');
	tdGrid.style.display='none';
	var oFileOverlay = dnn.dom.getById('FileGridOverLay');
	oFileOverlay.innerHTML=strOutput;
}

function unzipFile(FileName) 
{
	setMoveFiles(FileName + ";");
	var blconfirm = confirm('UnZip File(s): ' + FileName + '?');
	if (blconfirm) 
	{
		setSourcePath(getLastPath());
		setMoveStatus('unzip');
		hideDataGrid();
		return false;
	}
}

function gridCheckAll(sender) 
{
	m_bAllFilesChecked=!m_bAllFilesChecked;
	CheckAllFiles(m_bAllFilesChecked);
	if (!m_bAllFilesChecked) 
	{
		sender.src=sender.src.replace('checked', 'unchecked');
		sender.alt = m_sLocaleSelectAll;
	} 
	else 
	{
		sender.src=sender.src.replace('unchecked', 'checked');
		sender.alt = m_sLocaleUnSelectAll;
	}

	return false;
}


</script>
<asp:panel id="pnlMainScripts" Runat="server"></asp:panel>
<div style="DISPLAY: none"><asp:literal id="ctrlScripts1" Runat="server" EnableViewState="true"></asp:literal><asp:linkbutton id="lnkSelectFolder" Runat="server" EnableViewState="False"></asp:linkbutton><asp:textbox id="txtCurFolderID" Runat="server" EnableViewState="true"></asp:textbox><asp:textbox id="txtFldScrollPos" EnableViewState="true" runat="server"></asp:textbox></div>
<asp:panel id="pnlScripts" Runat="server"></asp:panel>
<table class="FileManager" cellSpacing="0" cellPadding="0" width="760" align="center" border="0">
	<!-- ToolBar Row Begin -->
	<tr>
		<td vAlign="top" colSpan="3">
			<table class="FileManager_ToolBar" cellSpacing="0" cellPadding="0" width="100%" border="0">
				<tr>
					<td width="5"></td>
					<td width="60"><asp:label id="lblFolderBar" Runat="server" CssClass="SubHead" resourcekey="FolderBar"></asp:label></td>
					<td width="160"><asp:dropdownlist id="ddlStorageLocation" Runat="server" CssClass="NormalTextBox"></asp:dropdownlist></td>
					<td vAlign="middle" width="100"><asp:textbox id="txtNewFolder" Runat="server" EnableViewState="False" CssClass="NormalTextBox"
							Width="100"></asp:textbox></td>
					<td vAlign="middle" width="100">
						<asp:linkbutton id="lnkAddFolder" EnableViewState="False" runat="server" Visible="False"></asp:linkbutton>
						<span style="CURSOR: pointer" onclick="return canAddFolder();">
							<asp:image id="lnkAddFolderIMG" Runat="server" resourcekey="AddFolderImg.AlternateText" AlternateText="Add Folder"
								ImageUrl="~/images/FileManager/ToolBarAddFolderEnabled.gif" name="lnkAddFolderIMG"></asp:image>
							<asp:label id="Label2" EnableViewState="False" runat="server" CssClass="Normal" resourcekey="AddFolder">Add Folder</asp:label>
						</span>
					</td>
					<td vAlign="middle" width="120">
						<asp:linkbutton id="lnkDeleteFolder" EnableViewState="False" runat="server" Visible="False"></asp:linkbutton>
						<span style="CURSOR: pointer" onclick="return deleteFolder();">
							<asp:image id="lnkDelFolderIMG" Runat="server" resourcekey="DeleteFolderImg.AlternateText"
								AlternateText="Delete Folder" ImageUrl="~/images/FileManager/ToolBarDelFolderEnabled.gif"
								name="lnkDelFolderIMG"></asp:image>
							<asp:label id="Label1" EnableViewState="False" runat="server" CssClass="Normal" resourcekey="DeleteFolderLabel">Delete Folder</asp:label>
						</span>
					</td>
					<td align="right" vAlign="middle" width="210">
						<asp:linkbutton id="lnkSyncFolder" EnableViewState="False" runat="server" Visible="False"></asp:linkbutton>
						<span style="CURSOR: pointer" onclick="__doPostBack(m_sUCPrefixName + 'lnkSyncFolder', '');">
							<asp:image id="lnkSyncFolderIMG" Runat="server" resourcekey="lnkSyncFolderIMG.AlternateText"
								AlternateText="Synchronize Folder" ImageUrl="~/images/FileManager/ToolBarSynchronize.gif"
								name="lnkSyncFolderIMG"></asp:image>
							<asp:label id="Label3" EnableViewState="False" runat="server" CssClass="Normal" resourcekey="SynchronizeFolder">Synchronize Folder</asp:label>
						</span>
						<asp:CheckBox ID="chkRecursive" Runat="server" resourcekey="Recursive" CssClass="Normal"></asp:CheckBox>
					</td>
					<td width="5"></td>
				</tr>
			</table>
			<table class="FileManager_ToolBar" cellSpacing="0" cellPadding="0" width="100%" border="0">
				<tr>
					<td width="5"></td>
					<td width="60"><asp:label id="lblFileBar" Runat="server" CssClass="SubHead" resourcekey="FileBar"></asp:label></td>
					<td vAlign="middle">
						<asp:linkbutton id="lnkRefresh" EnableViewState="False" runat="server" Visible="False"></asp:linkbutton>
						<span style="CURSOR: pointer" onclick="__doPostBack(m_sUCPrefixName + 'lnkRefresh', '');">
							<asp:image id="lnkRefreshIMG" Runat="server" resourcekey="RefreshImg.AlternateText" AlternateText="Refresh"
								ImageUrl="~/images/FileManager/ToolBarRefreshEnabled.gif" name="lnkRefreshIMG"></asp:image>
							<asp:label id="Label6" EnableViewState="False" runat="server" CssClass="Normal" resourcekey="Refresh">Refresh</asp:label>
						</span>
					</td>
					<td vAlign="middle"><span style="CURSOR: pointer" onclick="copyCheckedFiles();"><asp:image id="lnkCopy" Runat="server" resourcekey="CopyImg.AlternateText" AlternateText="Copy"
								ImageUrl="~/images/FileManager/ToolBarCopyEnabled.gif" name="lnkCopy"></asp:image><asp:label id="lblCopy" EnableViewState="False" runat="server" CssClass="Normal" resourcekey="CopyFiles">Copy Files</asp:label></span></td>
					<td vAlign="middle"><span style="CURSOR: pointer" onclick="moveFiles();"><asp:image id="lnkMove" Runat="server" resourcekey="MoveImg.AlternateText" AlternateText="Move"
								ImageUrl="~/images/FileManager/ToolBarMoveEnabled.gif" name="lnkMove"></asp:image><asp:label id="lblMove" EnableViewState="False" runat="server" CssClass="Normal" resourcekey="MoveFiles">Move Files</asp:label>&nbsp;
						</span>
					</td>
					<td vAlign="middle"><asp:linkbutton id="lnkUpload" EnableViewState="False" runat="server" Visible="False"></asp:linkbutton><span style="CURSOR: pointer" onclick="__doPostBack(m_sUCPrefixName + 'lnkUpload', '');"><asp:image id="lnkUploadIMG" Runat="server" resourcekey="UploadImg.AlternateText" AlternateText="Upload"
								ImageUrl="~/images/FileManager/ToolBarUploadEnabled.gif" name="lnkUploadIMG"></asp:image><asp:label id="lblUpload" EnableViewState="False" runat="server" CssClass="Normal" resourcekey="Upload">Upload</asp:label>&nbsp;
						</span>
					</td>
					<td vAlign="middle"><span style="CURSOR: pointer" onclick="deleteCheckedFiles();"><asp:image id="lnkDelete" Runat="server" resourcekey="DeleteImg.AlternateText" AlternateText="Delete"
								ImageUrl="~/images/FileManager/ToolBarDeleteEnabled.gif" name="lnkDelete"></asp:image><asp:label id="lblDelete" EnableViewState="False" runat="server" CssClass="Normal" resourcekey="DeleteFiles">Delete Files </asp:label>&nbsp;
						</span>
					</td>
					<td vAlign="middle"><asp:image id="lnkFilterIMG" style="CURSOR: pointer" onclick="__doPostBack(m_sUCPrefixName + 'lnkFilter', '');;"
							Runat="server" resourcekey="FilterImg.AlternateText" AlternateText="Filter" ImageUrl="~/images/FileManager/ToolBarFilterEnabled.gif"
							name="lnkFilterIMG"></asp:image></td>
					<td vAlign="middle"><asp:linkbutton id="lnkFilter" EnableViewState="False" runat="server" Visible="False"></asp:linkbutton>&nbsp;</td>
					<td vAlign="middle"><asp:textbox id="txtFilter" EnableViewState="False" runat="server" CssClass="NormalTextBox"></asp:textbox>&nbsp;</td>
				</tr>
			</table>
		</td>
	</tr>
	<!-- ToolBar Row End -->
	<!-- Main Row Begin -->
	<tr>
		<td vAlign="top" width="205" bgColor="#ffffff">
			<table class="FileManager_Explorer" cellSpacing="0" cellPadding="0" width="100%" border="0">
				<tr>
					<td class="FileManager_Header" width="100%"><asp:label id="lblFolders" Runat="server" EnableViewState="False" resourcekey="Folders" Width="100%">Folders</asp:label></td>
				</tr>
				<tr>
					<td vAlign="top"><span id="pnlFolders" style="WIDTH: 200px; HEIGHT: 300px; BACKGROUND-COLOR: #ffffff" onscroll="fldScroll();"
							runat="server"><asp:panel id="pnlTreeInitScripts" style="MARGIN-LEFT: 2px" Runat="server" width="200" Height="300">
								<dnntv:DNNTree id="DNNTree" runat="server" DefaultNodeCssClassSelected="FileManagerTreeNodeSelected"
									DefaultNodeCssClass="FileManagerTreeNode" DefaultChildNodeCssClass="FileManagerTreeNode"></dnntv:DNNTree>
							</asp:panel></span></td>
				</tr>
			</table>
		</td>
		<td width="5"></td>
		<td vAlign="top" width="100%">
			<table class="FileManager_FileList" cellSpacing="0" cellPadding="0" width="100%" border="0">
				<tr>
					<td class="FileManager_MessageBox" id="tdOverLay" style="DISPLAY: none" vAlign="top"
						width="100%">
						<div id="FileGridOverLay"></div>
					</td>
					<td id="tdGrid" vAlign="top" width="100%"><asp:datagrid id="dgFileList" runat="server" Width="100%" GridlInes="Horizontal" AllowPaging="True"
							AutoGenerateColumns="False" PageSize="10" CellPadding="0" AllowSorting="True" HeaderStyle-CssClass="FileManager_Header" ItemStyle-CssClass="FileManager_Item"
							EditItemStyle-CssClass="Normal" AlternatingItemStyle-CssClass="FileManager_AltItem" SelectedItemStyle-CssClass="FileManager_SelItem">
							<Columns>
								<asp:TemplateColumn>
									<ItemTemplate>
										<asp:Image ID="imgFileICO" Runat="server" Extention='<%# DataBinder.Eval(Container, "DataItem.Extension")%>' ImageUrl='<%# GetImageUrl(Convert.ToString(DataBinder.Eval(Container, "DataItem.Extension"))) %>' Width="16" Height="16"/>
									</ItemTemplate>
									<EditItemTemplate>
										<asp:Image ID="imgFileEditICO" Runat="server" Extention='<%# DataBinder.Eval(Container, "DataItem.Extension")%>' ImageUrl='<%# GetImageUrl(Convert.ToString(DataBinder.Eval(Container, "DataItem.Extension"))) %>' Width="16" Height="16" />
									</EditItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn HeaderStyle-HorizontalAlign="Left" SortExpression="FileName" HeaderText="FileName">
									<ItemTemplate>
										<asp:LinkButton CssClass="Normal" Runat="server" OnCommand="lnkDLFile_Command" ID="lnkDLFile" CommandArgument='<%# DataBinder.Eval(Container, "DataItem.FileId")%>'>
											<%# DataBinder.Eval(Container, "DataItem.FileName")%>
										</asp:LinkButton>
									</ItemTemplate>
									<EditItemTemplate>
										<asp:TextBox Runat="server" CssClass="Normal" id="txtEditFileName" Width="95%" Text='<%# DataBinder.Eval(Container, "DataItem.FileName")%>' />
									</EditItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn HeaderStyle-HorizontalAlign="Left" HeaderText="Date" SortExpression="DateModified">
									<ItemTemplate>
										<%# DataBinder.Eval(Container, "DataItem.DateModified")%>
									</ItemTemplate>
									<EditItemTemplate>
										<asp:CheckBox CssClass="Normal" id="chkReadOnly" original='<%# DataBinder.Eval(Container, "DataItem.ReadOnly")%>' Checked='<%# DataBinder.Eval(Container, "DataItem.ReadOnly")%>' runat="server" Text="R:" Visible='<%# (Convert.ToInt32(DataBinder.Eval(Container, "DataItem.StorageLocation")) < 2) %>' />
										<asp:CheckBox CssClass="Normal" id="chkHidden" original='<%# DataBinder.Eval(Container, "DataItem.Hidden")%>' Checked='<%# DataBinder.Eval(Container, "DataItem.Hidden")%>' runat="server" Text="H:" Visible='<%# (Convert.ToInt32(DataBinder.Eval(Container, "DataItem.StorageLocation")) < 2) %>' />
										<asp:CheckBox CssClass="Normal" id="chkSystem" original='<%# DataBinder.Eval(Container, "DataItem.System")%>' Checked='<%# DataBinder.Eval(Container, "DataItem.System")%>' runat="server" Text="S:" Visible='<%# (Convert.ToInt32(DataBinder.Eval(Container, "DataItem.StorageLocation")) < 2) %>' />
										<asp:CheckBox CssClass="Normal" id="chkArchive" original='<%# DataBinder.Eval(Container, "DataItem.Archive")%>' Checked='<%# DataBinder.Eval(Container, "DataItem.Archive")%>' runat="server" Text="A:" Visible='<%# (Convert.ToInt32(DataBinder.Eval(Container, "DataItem.StorageLocation")) < 2) %>' />
									</EditItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn HeaderStyle-HorizontalAlign="Center" SortExpression="DateModified">
									<ItemTemplate>
										<%# DataBinder.Eval(Container, "DataItem.AttributeString")%>
									</ItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn HeaderStyle-HorizontalAlign="Right" SortExpression="intFileSize" HeaderText="Size">
									<ItemStyle HorizontalAlign="Right" CssClass="Normal"></ItemStyle>
									<ItemTemplate>
										<%# DataBinder.Eval(Container, "DataItem.FileSize")%>
									</ItemTemplate>
									<EditItemTemplate>
										<%# DataBinder.Eval(Container, "DataItem.FileSize")%>
									</EditItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn>
									<ItemStyle HorizontalAlign="Right" Width="1%"></ItemStyle>
									<ItemTemplate>
										<asp:ImageButton Runat="server" ID="lnkEditFile" OnCommand="lnkEditFile_Command" AlternateText="Rename File"
											resourcekey="RenameFileImg.AlternateText" ImageUrl="~/images/FileManager/DNNExplorer_Edit.gif" />
									</ItemTemplate>
									<EditItemTemplate>
										<asp:ImageButton Runat="server" ID="lnkOkRename" OnCommand="lnkOkRename_Command" AlternateText="Save Changes" resourcekey="SaveChangesImg.AlternateText" ImageUrl="~/images/FileManager/DNNExplorer_OK.gif" CommandArgument='<%# DataBinder.Eval(Container, "DataItem.FileName")%>' />
									</EditItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn>
									<ItemStyle HorizontalAlign="Right" Width="1%"></ItemStyle>
									<ItemTemplate>
										<asp:ImageButton Runat="server" ID="lnkDeleteFile" OnCommand="lnkDeleteFile_Command" AlternateText="Delete File" resourcekey="DeleteFileImg.AlternateText" ImageUrl="~/images/FileManager/DNNExplorer_trash.gif" CommandName='<%# DataBinder.Eval(Container, "DataItem.FileName")%>' />
									</ItemTemplate>
									<EditItemTemplate>
										<asp:ImageButton Runat="server" ID="lnkCancelRename" OnCommand="lnkCancelRename_Command" AlternateText="Cancel Rename" resourcekey="CancelRenameImg.AlternateText" ImageUrl="~/images/FileManager/DNNExplorer_Cancel.gif" CommandArgument='<%# DataBinder.Eval(Container, "DataItem.FileName")%>' />
									</EditItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn>
									<ItemStyle HorizontalAlign="Right" Width="1%"></ItemStyle>
									<ItemTemplate>
										<asp:image id="lnkUnzip" style="CURSOR: pointer" runat="server" name="lnkMove" ImageUrl="~/images/FileManager/DNNExplorer_Unzip.gif" AlternateText="Unzip File" resourcekey="UnzipFileImg.AlternateText" filename='<%# DataBinder.Eval(Container, "DataItem.FileName")%>' extension='<%# DataBinder.Eval(Container, "DataItem.Extension")%>'>
										</asp:image>
									</ItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn HeaderStyle-HorizontalAlign="center">
									<ItemStyle HorizontalAlign="center" Width="30px"></ItemStyle>
									<HeaderTemplate>
										<input onclick='return gridCheckAll(this);' type=image src='<%# Page.ResolveUrl("~/images/FileManager/unchecked.gif") %>' alt='<%=Localization.GetString("SelectAll", LocalResourceFile)%>'>
									</HeaderTemplate>
									<ItemTemplate>
										<asp:CheckBox ID=chkFile Runat="server" filename='<%# DataBinder.Eval(Container, "DataItem.FileName")%>' />
									</ItemTemplate>
									<EditItemTemplate>
										<asp:CheckBox ID="chkFile" Enabled="False" Runat="server" filename='<%# DataBinder.Eval(Container, "DataItem.FileName")%>' />
									</EditItemTemplate>
								</asp:TemplateColumn>
							</Columns>
							<PagerStyle Visible="False"></PagerStyle>
						</asp:datagrid></td>
				</tr>
				<tr vAlign="bottom">
					<td colSpan="2">
						<table class="FileManager_Pager" id="tblMessagePager" cellSpacing="0" cellPadding="0" width="100%"
							border="0" runat="server">
							<tr>
								<td align="left">&nbsp;&nbsp;<asp:label id="lblCurPage" Runat="server" CssClass="NormalBold"></asp:label></td>
								<td vAlign="bottom" align="right" style="padding-right:5px">
									<table cellSpacing="2" cellPadding="2" border="0">
										<tr vAlign="middle">
											<td width="19"><asp:linkbutton id="lnkMoveFirst" runat="server"></asp:linkbutton></td>
											<td width="19"><asp:linkbutton id="lnkMovePrevious" runat="server"></asp:linkbutton></td>
											<td width="19"><asp:linkbutton id="lnkMoveNext" runat="server"></asp:linkbutton></td>
											<td width="19"><asp:linkbutton id="lnkMoveLast" runat="server"></asp:linkbutton></td>
										</tr>
									</table>
								</td>
							</tr>
						</table>
					</td>
				</tr>
			</table>
		</td>
		<!-- Main Row End --></tr>
	<!-- Status Bar Row Begin -->
	<tr>
		<td class="FileManager_StatusBar" colSpan="3">
			<table cellSpacing="0" cellPadding="0" width="100%" border="1">
				<tr bgColor="#ece9d8">
					<td align="left" width="50%">
						<div style="OVERFLOW: visible; WIDTH: 100%">&nbsp;<asp:label id="lblCurFolder" Runat="server" CssClass="NormalBold"></asp:label></div>
					</td>
					<td width="25%"><asp:label id="lblFileSpace" Runat="server" CssClass="NormalBold"></asp:label></td>
					<td class="NormalBold" align="right" width="25%">&nbsp;
						<asp:label id="lblItemsPerPage" runat="server" CssClass="NormalBold" resourcekey="ItemsPerPage">Items Per Page:</asp:label>&nbsp;
						<asp:dropdownlist id="selPageSize" Runat="server" CssClass="Normal" AutoPostBack="True">
							<asp:ListItem Selected="True" Value="10">10</asp:ListItem>
							<asp:ListItem Value="15">15</asp:ListItem>
							<asp:ListItem Value="20">20</asp:ListItem>
							<asp:ListItem Value="30">30</asp:ListItem>
							<asp:ListItem Value="40">40</asp:ListItem>
							<asp:ListItem Value="50">50</asp:ListItem>
						</asp:dropdownlist>&nbsp;&nbsp;
					</td>
				</tr>
			</table>
		</td>
	</tr>
	<!-- Status Bar Row End --></table>
<div style="DISPLAY: none"><asp:linkbutton id="lnkMoveFiles" Runat="server" EnableViewState="False"></asp:linkbutton><asp:linkbutton id="lnkGetMoreNodes" Runat="server" EnableViewState="False">GetMoreNodes</asp:linkbutton><asp:textbox id="txtMailData" Runat="server" EnableViewState="False"></asp:textbox><asp:textbox id="txtMailText" Runat="server" EnableViewState="False"></asp:textbox><asp:linkbutton id="lnkDeleteAllCheckedFiles" Runat="server" EnableViewState="False"></asp:linkbutton><asp:textbox id="txtLastPath" Runat="server" EnableViewState="False"></asp:textbox><asp:linkbutton id="lnkCancelMoveFiles" Runat="server" EnableViewState="False"></asp:linkbutton><asp:hyperlink id="lnkUploadRedir" Runat="server" EnableViewState="False" Visible="False"></asp:hyperlink></div>
<asp:panel id="pnlSecurity" Runat="server" Visible="False">
<dnn:sectionhead id="dshSecurity" runat="server" resourcekey="Security" cssclass="Head" text="Security Settings"
		section="tblSecurity"></dnn:sectionhead>
<TABLE id="tblSecurity" cellSpacing="2" cellPadding="2" summary="Security Details Design Table"
		border="0" runat="server">
		<TR>
			<TD class="SubHead" vAlign="top" width="150"><BR>
				<dnn:label id="plPermissions" runat="server" text="Permissions:" controlname="ctlPermissions"></dnn:label></TD>
			<TD>
				<TABLE cellSpacing="0" cellPadding="0" border="0">
					<TR>
						<TD>
							<dnn:folderpermissionsgrid id="dgPermissions" runat="server"></dnn:folderpermissionsgrid></TD>
					</TR>
				</TABLE>
			</TD>
		</TR>
	</TABLE>
<asp:linkbutton class="CommandButton" id="cmdUpdate" runat="server" resourcekey="cmdUpdate" text="Update"></asp:linkbutton>&nbsp;&nbsp; 
<asp:linkbutton class="CommandButton" id="cmdCancel" runat="server" resourcekey="cmdCancel" text="Cancel"></asp:linkbutton>&nbsp;&nbsp; 
</asp:panel>
<asp:panel id="pnlScripts2" Runat="server" EnableViewState="False"></asp:panel>
