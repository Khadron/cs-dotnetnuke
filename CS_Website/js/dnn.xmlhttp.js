if (typeof(__dnn_m_aNamespaces) == 'undefined')	//include in each DNN ClientAPI namespace file for dependency loading
	var __dnn_m_aNamespaces = new Array();

function __dnn_getParser()
{
	if (dnn_xmlhttp.JsXmlHttpRequest != null)
		return 'JS';
	if (dnn.dom.browser.isType(dnn.dom.browser.InternetExplorer))
		return 'ActiveX'; //'ActiveX';
	else if (typeof(XMLHttpRequest) != "undefined") //(dnn.dom.browser.isType(dnn.dom.browser.Netscape) || dnn.dom.browser.isType(dnn.dom.browser.Mozilla)) //(typeof XMLHttpRequest != "undefined");
		return 'Native'; //'Native';
	else
		return 'JS';
	
}

function __dnn_cleanupxmlhttp()
{
	for (var i=0; i<dnn.xmlhttp.requests.length;i++)
	{
		if (dnn.xmlhttp.requests[i] != null)
		{
			if (dnn.xmlhttp.requests[i].completed)
			{
				dnn.xmlhttp.requests[i].dispose();
				if (dnn.xmlhttp.requests.length == 1)
				    dnn.xmlhttp.requests = new Array();
				else
				    dnn.xmlhttp.requests.splice(i,i);
			}
		}
	}
	//window.status = dnn.xmlhttp.requests.length + ' ' + new Date();
}

//dnn.xmlhttp Namespace ---------------------------------------------------------------------------------------------------------
function dnn_xmlhttp()
{
	this.pns = 'dnn';
	this.ns = 'xmlhttp';
	this.dependencies = 'dnn,dnn.dom'.split(',');
	this.isLoaded = false;
	this.parserName = null;
	this.contextId = 0;
	this.requests = new Array();
	this.cleanUpTimer = null;
}

dnn_xmlhttp.prototype.init = function ()
{
	this.parserName = __dnn_getParser();
}

dnn_xmlhttp.prototype.doCallBack = function(sControlId, sArg, pSuccessFunc, sContext, pFailureFunc, pStatusFunc, bAsync, sPostChildrenId, iType)
{
	var oReq = dnn.xmlhttp.createRequestObject();
	var sURL = document.location.href;
	oReq.successFunc = pSuccessFunc;
	oReq.failureFunc = pFailureFunc;
	oReq.statusFunc = pStatusFunc;
	oReq.context = sContext;
	if (bAsync == null)
		bAsync = true;
	
	if (sURL.indexOf('.aspx') == -1)	//fix this for url's that dont have page name in them...  quickfix for now...
		sURL += 'default.aspx';
	
	if (sURL.indexOf('?') == -1)
		sURL += '?';
	else
		sURL += '&';

	
	//sURL += '__DNNCAPISCI=' + sControlId + '&__DNNCAPISCP=' + encodeURIComponent(sArg);
	
	oReq.open('POST', sURL, bAsync);
	//oReq.send();
	
	sArg = dnn.encode(sArg);
		
	if (sPostChildrenId)
		sArg += '&' + dnn.dom.getFormPostString($(sPostChildrenId));

	if (iType != 0)
		sArg += '&__DNNCAPISCT=' + iType;
		
	oReq.send('__DNNCAPISCI=' + sControlId + '&__DNNCAPISCP=' + sArg);

	return oReq; //1.3
}

dnn_xmlhttp.prototype.createRequestObject = function()
{
	if (this.parserName == 'ActiveX')
	{
		var o = new ActiveXObject('Microsoft.XMLHTTP');
		dnn.xmlhttp.requests[dnn.xmlhttp.requests.length] = new dnn.xmlhttp.XmlHttpRequest(o);
		return dnn.xmlhttp.requests[dnn.xmlhttp.requests.length-1]; 
	}
	else if (this.parserName == 'Native')
	{
		return new dnn.xmlhttp.XmlHttpRequest(new XMLHttpRequest()); 
	}
	else
	{
		var oReq = new dnn.xmlhttp.XmlHttpRequest(new dnn.xmlhttp.JsXmlHttpRequest());
		dnn.xmlhttp.requests[oReq._request.contextId] = oReq;
		return oReq; 
	}	
}

//dnn.xmlhttp.XmlHttpRequest Object ---------------------------------------------------------------------------------------------------------
dnn_xmlhttp.prototype.XmlHttpRequest = function(o)
{
	this._request = o;
	this.successFunc = null;
	this.failureFunc = null;
	this.statusFunc = null;
	//this._request.onreadystatechange = dnn.dom.getObjMethRef(this, 'readyStateChange');
	this._request.onreadystatechange = dnn.dom.getObjMethRef(this, 'onreadystatechange');
	this.context = null;
	this.completed = false;
	//this.childNodes = this._doc.childNodes;
}

dnn_xmlhttp.prototype.XmlHttpRequest.prototype.dispose = function ()
{
	if (this._request != null)
	{
		this._request.onreadystatechange = new function() {};//stop IE memory leak.  Not sure why can't set to null;
		this._request.abort();
		this._request = null;
		this.successFunc = null;
		this.failureFunc = null;
		this.statusFunc = null;
		this.context = null;
		this.completed = null;
		this.postData = null;	//1.3
	}
}

dnn_xmlhttp.prototype.XmlHttpRequest.prototype.open = function (sMethod, sURL, bAsync)
{
	this._request.open(sMethod, sURL, bAsync);
	if (typeof(this._request.setRequestHeader) != 'undefined')
		this._request.setRequestHeader("Content-type", "application/x-www-form-urlencoded; charset=UTF-8");
	return true;
} 

dnn_xmlhttp.prototype.XmlHttpRequest.prototype.send = function (postData)
{
	//this._request.onreadystatechange = this.complete;
	this.postData = postData;
	if (dnn.xmlhttp.parserName == 'ActiveX')	
		this._request.send(postData);
	else
		this._request.send(postData);
	return true;
}

dnn_xmlhttp.prototype.XmlHttpRequest.prototype.onreadystatechange = function ()
{
	if (this.statusFunc != null)
		this.statusFunc(this._request.readyState, this.context, this); //1.3
		
	if (this._request.readyState == '4')
	{
		this.complete(this._request.responseText);
		if (dnn.xmlhttp.parserName == 'ActiveX')
			window.setTimeout(__dnn_cleanupxmlhttp, 1);	//cleanup xmlhttp object
	}
}

dnn_xmlhttp.prototype.XmlHttpRequest.prototype.complete = function (sRes)
{
	var sStatusCode = this.getResponseHeader('__DNNCAPISCSI');
	this.completed=true;

	if (sStatusCode == '200')	
		this.successFunc(sRes, this.context, this);	//1.3
	else
	{
		var sStatusDesc = this.getResponseHeader('__DNNCAPISCSDI');
		if (this.failureFunc != null)
			this.failureFunc(sStatusCode + ' - ' + sStatusDesc, this.context, this); //1.3
		else
			alert(sStatusCode + ' - ' + sStatusDesc);
	}
}

dnn_xmlhttp.prototype.XmlHttpRequest.prototype.getResponseHeader = function (sKey)
{
	return this._request.getResponseHeader(sKey);
}


dnn_xmlhttp.prototype.dependenciesLoaded = function()
{
	return (typeof(dnn) != 'undefined' && typeof(dnn.dom) != 'undefined');
}

dnn_xmlhttp.prototype.loadNamespace = function ()
{
	if (this.isLoaded == false)
	{
		if (this.dependenciesLoaded())
		{
			dnn.xmlhttp = this; 
			this.isLoaded = true;
			dnn.loadDependencies(this.pns, this.ns);
			dnn.xmlhttp.init();
		}
	}	
}

__dnn_m_aNamespaces[__dnn_m_aNamespaces.length] = new dnn_xmlhttp();

for (var i=__dnn_m_aNamespaces.length-1; i>=0; i--)
	__dnn_m_aNamespaces[i].loadNamespace();
