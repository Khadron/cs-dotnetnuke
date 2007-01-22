//this script enables any element on the page to have a toolbar associated.
//the RegisterToolBar method is used on the server-side to do the association.

function __dnn_toolbarHandler(sToolBarId, sCtlId, sNsPrefix, fHandler, sEvt, sHideEvt)
{
	var sStatus = dnn.dom.scriptStatus('dnn.controls.dnntoolbar.js');
	if (sStatus == 'complete')
	{
		var oTB = new dnn.controls.DNNToolBar(sCtlId);
		dnn.controls.controls[sToolBarId] = oTB;
		var oCtl = $(sCtlId);
		oTB.loadDefinition(sToolBarId, sNsPrefix, oCtl, oCtl.parentNode, oCtl, fHandler);
		dnn.dom.addSafeHandler(oCtl, sEvt, oTB, 'show');
		dnn.dom.addSafeHandler(oCtl, sHideEvt, oTB, 'beginHide');
		oTB.show();
	}
	else if (sStatus == '')	//not loaded
		dnn.dom.loadScript(dnn.dom.getScriptPath() + 'dnn.controls.dnntoolbar.js', '', function () {__dnn_toolbarHandler(sToolBarId, sCtlId, sNsPrefix, fHandler, sEvt, sHideEvt)});
}
