if (typeof(__dnn_m_aNamespaces) == 'undefined')	//include in each DNN ClientAPI namespace file for dependency loading
	var __dnn_m_aNamespaces = new Array();

function __dnn_getParser()
{
	if (dnn.dom.browser.isType(dnn.dom.browser.InternetExplorer))
		return 'MSXML';
	else if (dnn.dom.browser.isType(dnn.dom.browser.Netscape,dnn.dom.browser.Mozilla))
		return 'DOMParser';
	else
		return 'JS';
	
}

//dnn.xml Namespace ---------------------------------------------------------------------------------------------------------
function dnn_xml()
{
	this.pns = 'dnn';
	this.ns = 'xml';
	this.dependencies = 'dnn,dnn.dom'.split(',');
	this.isLoaded = false;
	this.parserName = null;
}

dnn_xml.prototype.init = function ()
{
	this.parserName = __dnn_getParser();
	if (this.parserName == 'MSXML')	
	{
		dnn_xml.prototype.createDocument = function()
		{
			var o = new ActiveXObject('MSXML.DOMDocument');
			o.async = false;
			return new dnn.xml.documentObject(o); 
		}

		dnn_xml.prototype.documentObject.prototype.getXml = function(sXml)
		{
			return this._doc.xml;
		}
		
		dnn_xml.prototype.documentObject.prototype.loadXml = function (sXml)
		{
			return this._doc.loadXML(sXml);
		}
			
	}
	else if (this.parserName == 'DOMParser')
	{
		dnn_xml.prototype.createDocument = function()
		{
			return new dnn.xml.documentObject(document.implementation.createDocument("", "", null)); 
		}
		
		dnn_xml.prototype.documentObject.prototype.getXml = function (sXml)
		{
			return this._doc.xml;
		}
		
		dnn_xml.prototype.documentObject.prototype.loadXml = function (sXml)
		{
			// parse the string to a new doc
			var oDoc = (new DOMParser()).parseFromString(sXml, "text/xml");
			    
			// remove all initial children
			while (this._doc.hasChildNodes())
				this._doc.removeChild(this._doc.lastChild);

			// insert and import nodes
			for (var i = 0; i < oDoc.childNodes.length; i++) 
				this._doc.appendChild(this._doc.importNode(oDoc.childNodes[i], true));
		}
		function __dnn_getNodeXml() 
		{
			//create a new XMLSerializer
			var oXmlSerializer = new XMLSerializer;
			    
			//get the XML string
			var sXml = oXmlSerializer.serializeToString(this);
			    
			//return the XML string
			return sXml;
		}
		Node.prototype.__defineGetter__("xml", __dnn_getNodeXml);
	}
	else
	{
		dnn_xml.prototype.createDocument = function()
		{
			return new dnn.xml.documentObject(new dnn.xml.JsDocument()); 
		}
		
		dnn_xml.prototype.documentObject.prototype.getXml = function ()
		{
			return this._doc.getXml();	//wish other browsers supported getters/setters	
		}
		
		dnn_xml.prototype.documentObject.prototype.loadXml = function (sXml)
		{
			return this._doc.loadXml(sXml);
		}
	}
}

//dnn.xml.documentObject Object ---------------------------------------------------------------------------------------------------------
dnn_xml.prototype.documentObject = function(oDoc)
{
	this._doc = oDoc;
}

dnn_xml.prototype.documentObject.prototype.childNodes = function (iIndex)
{
	if (this._doc.childNodes[iIndex] != null)
		return new dnn.xml.XmlNode(this._doc.childNodes[iIndex]);
}

dnn_xml.prototype.documentObject.prototype.findNode = function (sNodeName, sAttr, sValue)
{
	return this.childNodes(0).findNode(sNodeName, sAttr, sValue);
}

dnn_xml.prototype.documentObject.prototype.childNodeCount = function ()
{
	return this._doc.childNodes.length;
}

dnn_xml.prototype.documentObject.prototype.rootNode = function ()
{
	var oNode;
	for (var i=0; i<this.childNodeCount(); i++)
	{
		if (this.childNodes(i).nodeType != 7)
		{
			oNode = this.childNodes(i);
			break;
		}
	}
	return oNode;
}

//dnn.xml.XmlNode ---------------------------------------------------------------------------------------------------------
	//function dnn_xml_XmlNode(oNode)
	dnn_xml.prototype.XmlNode = function (oNode)
	{
		this.node = oNode;
		this.ownerDocument = this.node.ownerDocument;
		this.nodeType = this.node.nodeType;
	}
	
	dnn_xml.prototype.XmlNode.prototype.parentNode = function ()
	{
		if (this.node.parentNode != null)
			return new dnn.xml.XmlNode(this.node.parentNode);
		else
			return null;
	}
	
	dnn_xml.prototype.XmlNode.prototype.childNodes = function (iIndex)
	{
		if (this.node.childNodes[iIndex] != null)
			return new dnn.xml.XmlNode(this.node.childNodes[iIndex]);
	}

	dnn_xml.prototype.XmlNode.prototype.childNodeCount = function ()
	{
		return this.node.childNodes.length;
	}

	dnn_xml.prototype.XmlNode.prototype.nodeName = function ()
	{
		return this.node.nodeName;
	}
	
	dnn_xml.prototype.XmlNode.prototype.getAttribute = function (sAttr, sDef)
	{
		var sRet = this.node.getAttribute(sAttr);
		if (sRet == null)
			sRet = sDef;
		return sRet;
	}

	dnn_xml.prototype.XmlNode.prototype.setAttribute = function (sAttr, sVal)
	{
		if (sVal == null)
			return this.node.removeAttribute(sAttr);
		else
			return this.node.setAttribute(sAttr, sVal);
	}
	
	dnn_xml.prototype.XmlNode.prototype.getXml = function()
	{
		if (this.node.xml != null)
			return this.node.xml;
		else
			return this.node.getXml();	
	}	

	dnn_xml.prototype.XmlNode.prototype.getDocumentXml = function()
	{
		if (this.node.ownerDocument.xml != null)
			return this.node.ownerDocument.xml;
		else
			return this.node.ownerDocument.getXml();
	}

	dnn_xml.prototype.XmlNode.prototype.appendXml = function (sXml)
	{
		var oDoc = dnn.xml.createDocument();
		oDoc.loadXml('<___temp>' + sXml + '</___temp>');	//need to guarantee a single root
		var aNodes = new Array();

		for (var i=0; i<oDoc.childNodes(0).childNodeCount(); i++)	//appending child actually removes node from document, so get references then do append
			aNodes[aNodes.length] = oDoc.childNodes(0).childNodes(i).node;	//use real underlying node object
		for (var i=0; i<aNodes.length; i++)
			this.node.appendChild(aNodes[i]);	//surprised I don't need importNode
		
		return true;
	}

	dnn_xml.prototype.XmlNode.prototype.getNodeIndex = function (sIDName)
	{
		var oParent = this.parentNode();
		var sID = this.getAttribute(sIDName);
		for (var i=0; i<oParent.childNodeCount(); i++)
		{
			if (oParent.childNodes(i).getAttribute(sIDName) == sID)
				return i;
		}
		return -1;
	}

	dnn_xml.prototype.XmlNode.prototype.findNode = function (sNodeName, sAttr, sValue)
	{
		var sXPath = "//" + sNodeName + "[@" + sAttr + "='" + sValue + "']";
		var oNode;
		if (typeof(this.node.selectSingleNode) != 'undefined')
			oNode = this.node.selectSingleNode(sXPath);
		else if (typeof(this.node.ownerDocument.evaluate) != 'undefined')
		{
			var oNodeList = (this.node.ownerDocument.evaluate(sXPath, this.node, null, 0, null));
			if (oNodeList != null)
				oNode = oNodeList.iterateNext();
		}
		else
			oNode = this.node.ownerDocument.findNode(this.node, sNodeName, sAttr, sValue);
		
		if (oNode != null)
			return new dnn.xml.XmlNode(oNode);
			
	}

	dnn_xml.prototype.XmlNode.prototype.removeChild = function(oNode)
	{
		return this.node.removeChild(oNode.node);
	}

//---------------------------------------------------------------------------------------------------------

dnn_xml.prototype.dependenciesLoaded = function()
{
	return (typeof(dnn) != 'undefined' && typeof(dnn.dom) != 'undefined');
}

dnn_xml.prototype.loadNamespace = function ()
{
	if (this.isLoaded == false)
	{
		if (this.dependenciesLoaded())
		{
			dnn.xml = this; 
			this.isLoaded = true;
			dnn.loadDependencies(this.pns, this.ns);
			dnn.xml.init();
		}
	}	
}

__dnn_m_aNamespaces[__dnn_m_aNamespaces.length] = new dnn_xml();
for (var i=__dnn_m_aNamespaces.length-1; i>=0; i--)
	__dnn_m_aNamespaces[i].loadNamespace();
