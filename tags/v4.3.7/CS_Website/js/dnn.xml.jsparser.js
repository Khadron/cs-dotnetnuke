
//dnn.xml.JsDocument object ---------------------------------------------------------------------------------------------------------
	dnn_xml.prototype.JsDocument = function ()
	{
		this.root = new dnn.xml.JsXmlNode(this, '__root');
		this.childNodes = this.root.childNodes;
		this.currentHashCode=0;
	}

	dnn_xml.prototype.JsDocument.prototype.hasChildNodes = function ()
	{
		return this.childNodes.length > 0;
	}
	
	dnn_xml.prototype.JsDocument.prototype.loadXml = function (sXml)
	{
		var oParser = new dnn.xml.JsParser();
		oParser.parse(sXml, this.root);
		return true;
	}

	dnn_xml.prototype.JsDocument.prototype.getXml = function()
	{
		return this.root.getXml();
	}	

	dnn_xml.prototype.JsDocument.prototype.findNode = function (oParent, sNodeName, sAttr, sValue)
	{
		//primitive for now...  
		for (var i=0; i < oParent.childNodes.length; i++)
		{
			oNode = oParent.childNodes[i];

			//if (oNode.nodeType != 3)  //exclude nodeType of Text (Netscape/Mozilla) issue!
			if (oNode.nodeName == sNodeName)
			{
				if (sAttr == null)
					return oNode;
				else
				{
					if (oNode.getAttribute(sAttr) == sValue)
						return oNode;
				}
			}
			if (oNode.childNodes.length > 0)
			{
				var o = this.findNode(oNode, sNodeName, sAttr, sValue);
				if (o != null)
					return o;
			}
		}
	}

	dnn_xml.prototype.JsDocument.prototype.getNextHashCode = function ()
	{
		this.currentHashCode++;
		return this.currentHashCode;
	}
	
/*
	function dnn_xml_ChildNodes(oParent)
	{
		var ary = new Array();
		for (var i=0; i< oParent.childNodes.length; i++)
			ary[ary.length] = new dnn_xml_XmlNode(oParent.childNodes[i]);
		return ary;
	}
*/

//dnn.xml.JsXmlNode Object ---------------------------------------------------------------------------------------------------------

	dnn_xml.prototype.JsXmlNode = function (ownerDocument, name)
	{
		this.ownerDocument = ownerDocument;
		this.nodeName = name;
		this.text = '';
		this.childNodes = new Array();
		this.attributes = new Array();
		this.parentNode = null;
		this.hashCode = this.ownerDocument.getNextHashCode();
		this.nodeType = 0;
		//this.xml = this.getXml;
	}

	dnn_xml.prototype.JsXmlNode.prototype.appendChild = function(oNode)
	{
		this.childNodes[this.childNodes.length] = oNode;
		oNode.parentNode = this;
	}

	dnn_xml.prototype.JsXmlNode.prototype.removeChild = function(oNode)
	{
		var oParent = this;
		var iHash = oNode.hashCode;
		var bFound = false;
		for (var i=0; i<oParent.childNodes.length; i++)
		{
			if (bFound == false)
			{
				if (oParent.childNodes[i].hashCode == iHash)
					bFound = true;
			}
			if (bFound)
				oParent.childNodes[i] = oParent.childNodes[i+1];
		}
		if (bFound)
			oParent.childNodes.length = oParent.childNodes.length - 1; //remove last node
		return oNode;
	}

	dnn_xml.prototype.JsXmlNode.prototype.hasChildNodes = function ()
	{
		return this.childNodes.length > 0;
	}
	
	dnn_xml.prototype.JsXmlNode.prototype.getXml = function (oNode)
	{
		if (oNode == null)
			oNode = this;

		var sXml = '';
		
		if (oNode.nodeName != '__root')
			sXml =  '<' + oNode.nodeName + this.getAttributes(oNode) + '>';
		
		for (var i=0; i<oNode.childNodes.length; i++)
		{
			sXml += this.getXml(oNode.childNodes[i]) + oNode.childNodes[i].text;				
		}	
		if (oNode.nodeName != '__root')
			sXml = sXml + '</' + oNode.nodeName + '>';
		return sXml;
	}

	dnn_xml.prototype.JsXmlNode.prototype.getAttributes = function (oNode)
	{
		var sRet = '';
		for (var sAttr in oNode.attributes)
			sRet += ' ' + sAttr + '="' + dnn.encodeHTML(oNode.attributes[sAttr]) + '"';					
		return sRet;
	}
	
	dnn_xml.prototype.JsXmlNode.prototype.getAttribute = function (sAttr)
	{
		return this.attributes[sAttr];
	}

	dnn_xml.prototype.JsXmlNode.prototype.setAttribute = function (sAttr, sVal)
	{
		this.attributes[sAttr] = sVal;
	}

	dnn_xml.prototype.JsXmlNode.prototype.removeAttribute = function (sAttr)
	{
		delete this.attributes[sAttr];
	}
	
//primitive Js Xml Parser ---------------------------------------------------------------------------------------------------------
//sure a regular expression guru could make better
	dnn_xml.prototype.JsParser = function ()
	{
		this.pos = null;
		this.xmlArray = null;
		this.root = null;
	}
	
	dnn_xml.prototype.JsParser.prototype.parse = function(sXml, oRoot)
	{
		this.pos = 0;
		this.xmlArray = sXml.split('>');
		this.processXml(oRoot);
	}
	
	dnn_xml.prototype.JsParser.prototype.getProcessString = function ()
	{
		var s = this.xmlArray[this.pos];
		if (s == null)
			s = '';
		return s.replace(/^\s*/, "").replace(/\s*$/, ""); //trim off spaces on both sides
	}
	
	dnn_xml.prototype.JsParser.prototype.processXml = function (oParent)
	{
		var oNewParent = oParent;
		var bClose = this.isCloseTag();
		var bOpen = this.isOpenTag();
		while ((bClose == false || (bClose && bOpen)) && this.getProcessString().length > 0)
		{
			if (bClose)
			{
				this.processOpenTag(oParent);
				this.pos +=1;
			}
			else
			{
				oNewParent = this.processOpenTag(oParent);
				this.pos +=1;
				this.processXml(oNewParent);
			}					

			bClose = this.isCloseTag();
			bOpen = this.isOpenTag();
		}
		var s = this.getProcessString();		
		if (bClose && s.substr(0,1) != '<')
			oParent.text = s.substr(0,s.indexOf('<'));
		this.pos +=1;
	}

	dnn_xml.prototype.JsParser.prototype.isCloseTag = function()
	{
		var s = this.getProcessString();
		if (s.substr(0, 1) == '/' || s.indexOf('</') > -1 || s.substr(s.length-1) == '/')
			return true;
		else
			return false;
	}
	
	dnn_xml.prototype.JsParser.prototype.isOpenTag = function()
	{
		var s = this.getProcessString();
		if (s.substr(0, 1) == '<' && s.substr(0, 2) != '</' && s.substr(0,2) != '<?')
			return true;
		else
			return false;
	}

	dnn_xml.prototype.JsParser.prototype.processOpenTag = function(oParent)
	{		
		if (this.isOpenTag(this.getProcessString()))
		{
			var sArr = this.getProcessString().split(' ');
			var oNode = new dnn.xml.JsXmlNode(oParent.ownerDocument);
			
			oNode.nodeName = sArr[0].substr(1).replace('/', '');
			oNode.parentNode = oParent;
			this.processAttributes(oNode);
			oParent.appendChild(oNode);
			oParent = oNode;
		}
		return oParent
	}

	dnn_xml.prototype.JsParser.prototype.processAttributes = function(oNode)
	{
		var s = this.getProcessString();
		if (s.indexOf(' ') > -1)
			s = s.substr(s.indexOf(' ') + 1);
		
		if (s.indexOf('=') > -1)
		{
			var bValue=false;
			var sName='';
			var sValue='';
			var sChar;
			for (var i=0; i<s.length; i++)
			{
				sChar = s.substr(i, 1);
				if (sChar == '"')
				{
					if (bValue)
					{
						//need to escape out special characters
						oNode.attributes[sName] = dnn.decodeHTML(sValue);
						sName = '';
						sValue = '';
						i++; //skip space
					}						
					bValue = !bValue;
				}
				else if (sChar != '=' || bValue==true)	//if inside value then allow =
				{
					if (bValue)
						sValue += sChar;
					else
						sName += sChar;
				}
			}
		}
	}

