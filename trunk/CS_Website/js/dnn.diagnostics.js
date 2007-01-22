//--- dnn.diagnostics

if (typeof(__dnn_m_aNamespaces) == 'undefined')	//include in each DNN ClientAPI namespace file for dependency loading
	var __dnn_m_aNamespaces = new Array();

function dnn_diagnostics(bVerbose)
{
	this.pns = 'dnn';
	this.ns = 'diagnostics';
	this.dependencies = 'dnn'.split(',');
	this.isLoaded = false;
	this.verbose = bVerbose;
	this.debugCtl = null;
	this.debugWait = (document.all != null);//(dnn.dom.browser.type == 'ie');
	this.debugArray = new Array();
}

var __dnn_m_aryHandled=new Array();
function dnn_diagnosticTests(oParent)
{
	if (oParent.ns == 'dnn')
		dnn.diagnostics.clearDebug();
	if (typeof(oParent.UnitTests) == 'function')
	{
		dnn.diagnostics.displayDebug('------- Starting ' + oParent.pns + '.' + oParent.ns + ' tests (v.' + (oParent.apiversion ? oParent.apiversion : dnn.apiversion) + ') ' + new Date().toString() + ' -------');
		oParent.UnitTests();
	}
	
	for (var obj in oParent)
	{
		if (oParent[obj] != null && typeof(oParent[obj]) == 'object' && __dnn_m_aryHandled[obj] == null)
		{
			//if (obj != 'debugCtl')	//what is this IE object???
			if (oParent[obj].pns != null)
				dnn_diagnosticTests(oParent[obj]);
		}
		//__dnn_m_aryHandled[obj] = true;
	}
}

function __dnn_documentLoaded()
{
	dnn.diagnostics.debugWait = false;
	dnn.diagnostics.displayDebug('document loaded... avoiding Operation Aborted IE bug');
	dnn.diagnostics.displayDebug(dnn.diagnostics.debugArray.join('\n'));
	
}

dnn_diagnostics.prototype.clearDebug = function()
{
	if (this.debugCtl != null)
	{
		this.debugCtl.value = '';
		return true;
	}
	return false;
}

dnn_diagnostics.prototype.displayDebug = function(sText)
{
	if (this.debugCtl == null)
	{
		if (dnn.dom.browser.type == dnn.dom.browser.InternetExplorer)
		{
			var oBody = dnn.dom.getByTagName("body")[0];
			if (this.debugWait && oBody.readyState != 'complete')
			{
				dnn.debugWait = true;
				this.debugArray[this.debugArray.length] = sText;
				//document.attachEvent('onreadystate', __dnn_documentLoaded);
				if (oBody.onload == null || oBody.onload.toString().indexOf('__dnn_documentLoaded') == -1)
					oBody.onload = dnn.dom.appendFunction(oBody.onload, '__dnn_documentLoaded()');
				return;
			}
		}
		this.debugCtl = dnn.dom.getById('__dnnDebugOutput');
		if (this.debugCtl == null)
		{
			this.debugCtl = dnn.dom.createElement('TEXTAREA');
			this.debugCtl.id = '__dnnDebugOutput';
			this.debugCtl.rows=10;
			this.debugCtl.cols=100;
			dnn.dom.appendChild(oBody, this.debugCtl);
		}
		this.debugCtl.style.display = 'block';
	}
	
	if (dnn.diagnostics.debugCtl == null)
		alert(sText);
	else
		dnn.diagnostics.debugCtl.value += sText + '\n';
	
	return true;
}

dnn_diagnostics.prototype.assertCheck = function (sCom, bVal, sMsg)
{
	if (!bVal)
		this.displayDebug(sCom + ' - FAILED (' + sMsg + ')');
	else if (this.verbose)
		this.displayDebug(sCom + ' - PASSED');
}

dnn_diagnostics.prototype.assert = function (sCom, bVal) 
{
  this.assertCheck(sCom, bVal == true, 'Testing assert(boolean) for true');
}

dnn_diagnostics.prototype.assertTrue = function (sCom, bVal)
{
  this.assertCheck(sCom, bVal == true, 'Testing assert(boolean) for true');
}

dnn_diagnostics.prototype.assertFalse = function (sCom, bVal)
{
  this.assertCheck(sCom, bVal == false, 'Testing assert(boolean) for false');
}

dnn_diagnostics.prototype.assertEquals = function (sCom, sVal1, sVal2)
{
  this.assertCheck(sCom, sVal1 == sVal2, 'Testing Equals: ' + __dnn_safeString(sVal1) + ' (' + typeof(sVal1) + ') != ' + __dnn_safeString(sVal2) + ' (' + typeof(sVal2) + ')');
}

dnn_diagnostics.prototype.assertNotEquals = function (sCom, sVal1, sVal2)
{
  this.assertCheck(sCom, sVal1 != sVal2, 'Testing NotEquals: ' + __dnn_safeString(sVal1) + ' (' + typeof(sVal1) + ') == ' + __dnn_safeString(sVal2) + ' (' + typeof(sVal2) + ')');
}

dnn_diagnostics.prototype.assertNull = function (sCom, sVal1)
{
	this.assertCheck(sCom, sVal1 == null, 'Testing null: ' + __dnn_safeString(sVal1) + ' (' + typeof(sVal1) + ') != null');
}

dnn_diagnostics.prototype.assertNotNull = function (sCom, sVal1)
{
	this.assertCheck(sCom, sVal1 != null, 'Testing for null: ' + __dnn_safeString(sVal1) + ' (' + typeof(sVal1) + ') == null');
}

dnn_diagnostics.prototype.assertStringLength = function (sCom, sVal1)
{
	this.assertCheck(sCom, ((sVal1 == null) ? false : sVal1.length > 0), 'Testing for string length: ' + __dnn_safeString(sVal1) + ' (' + ((sVal1 == null) ? 'null' : sVal1.length) + ')');
}

dnn_diagnostics.prototype.assertNaN = function (sCom, sVal1)
{
	this.assertCheck(sCom, isNaN(sVal1), 'Testing for NaN: ' + __dnn_safeString(sVal1) + ' (' + typeof(sVal1) + ') is a number');
}

dnn_diagnostics.prototype.assertNotNaN = function (sCom, sVal1)
{
	this.assertCheck(sCom, isNaN(sVal1) == false, 'Testing for NotNaN: ' + __dnn_safeString(sVal1) + ' (' + typeof(sVal1) + ') is NOT a number');
}

dnn_diagnostics.prototype.dependenciesLoaded = function()
{
	return (typeof(dnn) != 'undefined');
}

dnn_diagnostics.prototype.loadNamespace = function ()
{
	if (this.isLoaded == false)
	{
		if (this.dependenciesLoaded())
		{
			dnn.diagnostics = this; 
			this.isLoaded = true;
			dnn.loadDependencies(this.pns, this.ns);
		}
	}	
}


function __dnn_safeString(s)
{
	if (typeof(s) == 'string' || typeof(s) == 'number')
		return s;
	else
		return typeof(s);
}

__dnn_m_aNamespaces[__dnn_m_aNamespaces.length] = new dnn_diagnostics(true);
for (var i=__dnn_m_aNamespaces.length-1; i>=0; i--)
	__dnn_m_aNamespaces[i].loadNamespace();


//--- End dnn.diagnostics
//dnn_diagnosticTests(dnn);

dnn.dom.setScriptLoaded('dnn.diagnostics.js');	//callback for dynamically loaded script