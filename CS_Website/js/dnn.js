var dnn;	//should really make this m_dnn... but want to treat like namespace

var DNN_HIGHLIGHT_COLOR = '#9999FF';
var COL_DELIMITER = String.fromCharCode(18);
var ROW_DELIMITER = String.fromCharCode(17);
var QUOTE_REPLACEMENT = String.fromCharCode(19);
var KEY_LEFT_ARROW = 37;
var KEY_UP_ARROW = 38;
var KEY_RIGHT_ARROW = 39;
var KEY_DOWN_ARROW = 40;
var KEY_RETURN = 13;
var KEY_ESCAPE = 27;

if (typeof(__dnn_m_aNamespaces) == 'undefined')	//include in each DNN ClientAPI namespace file for dependency loading
	var __dnn_m_aNamespaces = new Array();

//NameSpace DNN
	function __dnn()
	{
		this.apiversion = .2;
		this.pns = '';
		this.ns = 'dnn';
		this.diagnostics = null;
		this.vars = null;
		this.dependencies = new Array();
		this.isLoaded = false;

		this.delay = new Array();
	}
	
__dnn.prototype = 
{
	getVars: function()
	{
		if (this.vars == null)
		{
			this.vars = new Array();
			var oCtl = dnn.dom.getById('__dnnVariable');
			if (oCtl != null)
			{
				if (oCtl.value.indexOf('__scdoff') != -1)
				{
					//browsers like MacIE don't support char(18) very well... need to use multichars
					COL_DELIMITER = '~|~';
					ROW_DELIMITER = '~`~';
					QUOTE_REPLACEMENT = '~!~';
				}
			
				var aryItems = oCtl.value.split(ROW_DELIMITER);
				for (var i=0; i<aryItems.length; i++)
				{
					var aryItem = aryItems[i].split(COL_DELIMITER);
					
					if (aryItem.length == 2)
						this.vars[aryItem[0]] = aryItem[1];
				}
			}
		}
		return this.vars;	
	},

	getVar: function(sKey)
	{
		if (this.getVars()[sKey] != null)
		{
			var re = eval('/' + QUOTE_REPLACEMENT + '/g');
			return this.getVars()[sKey].replace(re, '"');
		}
	},

	setVar: function(sKey, sVal)
	{			
		if (this.vars == null)
			this.getVars();			
		this.vars[sKey] = sVal;
		var oCtl = dnn.dom.getById('__dnnVariable');
		if (oCtl == null)
		{
			oCtl = dnn.dom.createElement('INPUT');
			oCtl.type = 'hidden';
			oCtl.id = '__dnnVariable';
			dnn.dom.appendChild(dnn.dom.getByTagName("body")[0], oCtl);		
		}
		var sVals = '';
		var s;
		var re = eval('/"/g');
		for (s in this.vars)
			sVals += ROW_DELIMITER + s + COL_DELIMITER + this.vars[s].toString().replace(re, QUOTE_REPLACEMENT);

		oCtl.value = sVals;
		return true;
	},

	callPostBack: function(sAction)
	{
		var sPostBack = dnn.getVar('__dnn_postBack');
		var sData = '';
		if (sPostBack.length > 0)
		{
			sData += sAction;
			for (var i=1; i<arguments.length; i++)
			{
				var aryParam = arguments[i].split('=');
				sData += COL_DELIMITER + aryParam[0] + COL_DELIMITER + aryParam[1];
			}
			eval(sPostBack.replace('[DATA]', sData));
			return true;
		}
		return false;
	},

    createDelegate: function(oThis, pFunc) 
    {
        return function() {pFunc.apply(oThis, arguments);};
    },

	doDelay: function(sType, iTime, pFunc, oContext) 
	{
		if (this.delay[sType] == null)
		{
			this.delay[sType] = new dnn.delayObject(pFunc, oContext, sType);
			//this.delay[sType].num = window.setTimeout(dnn.dom.getObjMethRef(this.delay[sType], 'complete'), iTime);
			this.delay[sType].num = window.setTimeout(dnn.createDelegate(this.delay[sType], this.delay[sType].complete), iTime);
		}
	},

	cancelDelay: function(sType) 
	{
		if (this.delay[sType] != null)
		{
			window.clearTimeout(this.delay[sType].num);
			this.delay[sType] = null;
		}
	},

	decodeHTML: function(s)	
	{
		return s.toString().replace(/&amp;/g,"&").replace(/&lt;/g,"<").replace(/&gt;/g,">").replace(/&quot;/g,'"');
	},

	encode: function(sArg)
	{
		if (encodeURIComponent)
			return encodeURIComponent(sArg);
		else
			return escape(sArg);
	},

	encodeHTML: function(s)	
	{
		return s.toString().replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;").replace(/'/g, "&apos;").replace(/\"/g, "&quot;");
	},

	escapeForEval: function(s)	//needs work...
	{
		return s.replace(/\\/g, '\\\\').replace(/\'/g, "\\'").replace(/\r/g, '').replace(/\n/g, '\\n').replace(/\./, '\\.');
	},

	extend: function(dest, src) 
	{
		for (s in src)
			dest[s] = src[s];
		return dest;
	},

	dependenciesLoaded: function()
	{
		return true;
	},

	loadNamespace: function()
	{
		if (this.isLoaded == false)
		{
			if (this.dependenciesLoaded())
			{
				dnn = this; 
				this.isLoaded = true;
				this.loadDependencies(this.pns, this.ns);
			}
		}	
	},

	loadDependencies: function(sPNS, sNS)
	{
		for (var i=0; i<__dnn_m_aNamespaces.length; i++)
		{
			for (var iDep=0; iDep<__dnn_m_aNamespaces[i].dependencies.length; iDep++)
			{
				if (__dnn_m_aNamespaces[i].dependencies[iDep] == sPNS + (sPNS.length>0 ? '.': '') + sNS)
					__dnn_m_aNamespaces[i].loadNamespace();
			}
		}
	}
}	

	__dnn.prototype.delayObject = function(pFunc, oContext, sType)
	{
		this.num = null;
		this.pfunc = pFunc;
		this.context = oContext;
		this.type = sType;
	}

	__dnn.prototype.delayObject.prototype =
	{
		complete: function()
		{
			dnn.delay[this.type] = null;
			this.pfunc(this.context);
		}
	}

	//--- dnn.dom
		function dnn_dom()
		{
			this.pns = 'dnn';
			this.ns = 'dom';
			this.dependencies = 'dnn'.split(',');
			this.isLoaded = false;
			this.browser = new this.browserObject();
			this.__leakEvts = new Array();			
		}

dnn_dom.prototype =
{
		appendChild: function(oParent, oChild) 
		{
			if (oParent.appendChild) 
				return oParent.appendChild(oChild);
			else 
				return null;
		},

		attachEvent: function(oCtl, sType, fHandler) 
		{
			if (dnn.dom.browser.isType(dnn.dom.browser.InternetExplorer) == false)
			{
				var sName = sType.substring(2);
				oCtl.addEventListener(sName, function(evt) {dnn.dom.event = new dnn.dom.eventObject(evt, evt.target); return fHandler();}, false);
			}
			else
				oCtl.attachEvent(sType, function() {dnn.dom.event = new dnn.dom.eventObject(window.event, window.event.srcElement); return fHandler();});
			return true;
		},		

		createElement: function(sTagName) 
		{
			if (document.createElement) 
				return document.createElement(sTagName.toLowerCase());
			else 
				return null;
		},

		cursorPos: function(oCtl)
		{			
			// empty control means the cursor is at 0
			if (oCtl.value.length == 0)
				return 0;
			
			// -1 for unknown
			var iPos = -1;

			if (oCtl.selectionStart)	// Moz - Opera
				iPos = oCtl.selectionStart;
			else if ( oCtl.createTextRange )// IE
			{
				var oSel = window.document.selection.createRange();
				var oRange = oCtl.createTextRange();
				
				// if the current selection is within the edit control			
				if (oRange == null || oSel == null || (( oSel.text != "" ) && oRange.inRange(oSel) == false))
					return -1;
				
				if (oSel.text == "")
				{
					if (oRange.boundingLeft == oSel.boundingLeft)
						iPos = 0;
					else
					{
						var sTagName = oCtl.tagName.toLowerCase();
						// Handle inputs.
						if (sTagName == "input")
						{
							var sText = oRange.text;
							var i = 1;
							while (i < sText.length)
							{
								oRange.findText(sText.substring(i));
								if (oRange.boundingLeft == oSel.boundingLeft)
									break;
								
								i++;
							}
						}
						// Handle text areas.
						else if (sTagName == "textarea")
						{
							var i = oCtl.value.length + 1;
							var oCaret = document.selection.createRange().duplicate();
							while (oCaret.parentElement() == oCtl && oCaret.move("character",1) == 1)
								--i;
							
							if (i == oCtl.value.length + 1)
								i = -1;
						}
						iPos = i;
					}
				}
				else
					iPos = oRange.text.indexOf(oSel.text);
			}
			return iPos;
		},

		cancelCollapseElement: function(oCtl)
		{
			dnn.cancelDelay(oCtl.id + 'col');
			oCtl.style.display = 'none';
		},
		
		collapseElement: function(oCtl, iNum, pCallBack) 
		{
			if (iNum == null)
				iNum = 10;
			oCtl.style.overflow = 'hidden';
			var oContext = new Object();
			oContext.num = iNum;
			oContext.ctl = oCtl;
			oContext.pfunc = pCallBack;
			oCtl.origHeight = oCtl.offsetHeight;
			dnn.dom.__collapseElement(oContext);
		},
		
		__collapseElement: function(oContext) 
		{
			var iNum = oContext.num;
			var oCtl = oContext.ctl;
			
			var iStep = oCtl.origHeight / iNum;
			if (oCtl.offsetHeight - (iStep*2) > 0)
			{
				oCtl.style.height = oCtl.offsetHeight - iStep;
				dnn.doDelay(oCtl.id + 'col', 10, dnn.dom.__collapseElement, oContext);
			}
			else
			{
				oCtl.style.display = 'none';
				if (oContext.pfunc != null)
					oContext.pfunc();
			}
		},

		cancelExpandElement: function(oCtl)
		{
			dnn.cancelDelay(oCtl.id + 'exp');
			oCtl.style.overflow = '';
			oCtl.style.height = '';			
		},
		
		expandElement: function(oCtl, iNum, pCallBack) 
		{
			if (iNum == null)
				iNum = 10;
			
			if (oCtl.style.display == 'none' && oCtl.origHeight == null)
			{
				oCtl.style.display = '';
				oCtl.style.overflow = '';
				oCtl.origHeight = oCtl.offsetHeight;
				oCtl.style.overflow = 'hidden';
				oCtl.style.height = 1;
			}
			oCtl.style.display = '';

			var oContext = new Object();
			oContext.num = iNum;
			oContext.ctl = oCtl;
			oContext.pfunc = pCallBack;
			dnn.dom.__expandElement(oContext);
		},

		__expandElement: function(oContext) 
		{
			var iNum = oContext.num;
			var oCtl = oContext.ctl;
			var iStep = oCtl.origHeight / iNum;
			if (oCtl.offsetHeight + iStep < oCtl.origHeight)
			{
				oCtl.style.height = oCtl.offsetHeight + iStep;
				dnn.doDelay(oCtl.id + 'exp', 10, dnn.dom.__expandElement, oContext);
			}
			else
			{
				oCtl.style.overflow = '';
				oCtl.style.height = '';
				if (oContext.pfunc != null)
					oContext.pfunc();
			}				
		},
		
		deleteCookie: function(sName, sPath, sDomain) 
		{
			if (this.getCookie(sName)) 
			{
				this.setCookie(sName, '', -1, sPath, sDomain);
				return true;
			}
			return false;
		},

		getAttr: function(oNode, sAttr, sDef)
		{
			if (oNode.getAttribute == null)
				return sDef;
			var sVal = oNode.getAttribute(sAttr);
			
			if (sVal == null || sVal == '')
				return sDef;
			else
				return sVal;
		},

		getById: function(sID, oCtl)
		{
			if (oCtl == null)
				oCtl = document;
			if (oCtl.getElementById) //(dnn.dom.browser.isType(dnn.dom.browser.InternetExplorer) == false)
				return oCtl.getElementById(sID);
			else if (oCtl.all)
				return oCtl.all(sID);
			else
				return null;
		},

		getByTagName: function(sTag, oCtl)
		{
			if (oCtl == null)
				oCtl = document;
			if (oCtl.getElementsByTagName) //(dnn.dom.browser.type == dnn.dom.browser.InternetExplorer)
				return oCtl.getElementsByTagName(sTag);
			else if (oCtl.all && oCtl.all.tags)
				return oCtl.all.tags(sTag);
			else
				return null;
		},

		getParentByTagName: function(oCtl, sTag)
		{
			var oP = oCtl.parentNode;
			sTag = sTag.toLowerCase();
			while (oP!= null)
			{
				if  (oP.tagName && oP.tagName.toLowerCase() == sTag)
					return oP;
				oP = oP.parentNode;
			}
			return null;
		},

		getCookie: function(sName) 
		{
			var sCookie = " " + document.cookie;
			var sSearch = " " + sName + "=";
			var sStr = null;
			var iOffset = 0;
			var iEnd = 0;
			if (sCookie.length > 0) 
			{
				iOffset = sCookie.indexOf(sSearch);
				if (iOffset != -1) 
				{
					iOffset += sSearch.length;
					iEnd = sCookie.indexOf(";", iOffset)
					if (iEnd == -1) 
						iEnd = sCookie.length;
					sStr = unescape(sCookie.substring(iOffset, iEnd));
				}
			}
			return(sStr);
		},

		getNonTextNode: function(oNode)
		{
			if (this.isNonTextNode(oNode))	
				return oNode;
			
			while (oNode != null && this.isNonTextNode(oNode))
			{
				oNode = this.getSibling(oNode, 1);
			}
			return oNode;
		},

		__leakEvt: function(sName, oCtl, oPtr)
		{
			this.name = sName;
			this.ctl = oCtl;
			this.ptr = oPtr;
		},
		
		addSafeHandler: function(oDOM, sEvent, oObj, sMethod)
		{
			oDOM[sEvent] = this.getObjMethRef(oObj, sMethod);			

			if (dnn.dom.browser.isType(dnn.dom.browser.InternetExplorer))	//handle IE memory leaks with closures
			{
				if (this.__leakEvts.length == 0)
					dnn.dom.attachEvent(window, 'onunload', dnn.dom.destroyHandlers);

				this.__leakEvts[this.__leakEvts.length] = new dnn.dom.__leakEvt(sEvent, oDOM, oDOM[sEvent]);
			}
		},
		
		destroyHandlers: function()	//handle IE memory leaks with closures
		{
			var iCount = dnn.dom.__leakEvts.length-1;
			for (var i=iCount; i>=0; i--)
			{
				var oEvt = dnn.dom.__leakEvts[i];
				oEvt.ctl.detachEvent(oEvt.name, oEvt.ptr);
				oEvt.ctl[oEvt.name] = null;
				dnn.dom.__leakEvts.length = dnn.dom.__leakEvts.length - 1;
			}
		},
		
		//http://jibbering.com/faq/faq_notes/closures.html (associateObjWithEvent)
		getObjMethRef: function(obj, methodName)
		{
			return (function(e)	{e = e||window.event; return obj[methodName](e, this); } );
		},

		getSibling: function(oCtl, iOffset)
		{
			if (oCtl != null && oCtl.parentNode != null)
			{
				for (var i=0; i<oCtl.parentNode.childNodes.length; i++)
				{
					if (oCtl.parentNode.childNodes[i].id == oCtl.id)
					{
						if (oCtl.parentNode.childNodes[i + iOffset] != null)
							return oCtl.parentNode.childNodes[i + iOffset];
					}
				}
			}
			return null;
		},
	
		isNonTextNode: function(oNode)
		{
			return (oNode.nodeType != 3 && oNode.nodeType != 8); //exclude nodeType of Text (Netscape/Mozilla) issue!
		},
		
		navigate: function(sURL, sTarget)
		{
			if (sTarget != null && sTarget.length > 0)
			{
				if (sTarget == '_blank')	//todo: handle more
					window.open(sURL);
				else
					document.frames[sTarget].location.href = sURL;
			}
			else
				window.location.href = sURL;
			return false;
		},
		
		removeChild: function(oChild) 
		{
			if (oChild.parentNode.removeChild) 
				return oChild.parentNode.removeChild(oChild);
			else 
				return null;
		},

		setCookie: function(sName, sVal, iDays, sPath, sDomain, bSecure) 
		{
			var sExpires;
			if (iDays)
			{
				sExpires = new Date();
				sExpires.setTime(sExpires.getTime()+(iDays*24*60*60*1000));
			}
			document.cookie = sName + "=" + escape(sVal) + ((sExpires) ? "; expires=" + sExpires : "") + 
				((sPath) ? "; path=" + sPath : "") + ((sDomain) ? "; domain=" + sDomain : "") + ((bSecure) ? "; secure" : "");
			
			if (document.cookie.length > 0)
				return true;
		},

		getCurrentStyle: function(oNode, prop) 
		{
			if (document.defaultView) 
			{
				if (oNode.nodeType != oNode.ELEMENT_NODE) return null;
				return document.defaultView.getComputedStyle(oNode,'').getPropertyValue(prop.split('-').join(''));
			}
			if (oNode.currentStyle) 
				return oNode.currentStyle[prop.split('-').join('')];
			if (oNode.style) 
				return oNode.style.getAttribute(prop.split('-').join(''));  // We need to get rid of slashes
			return null;
		},

		dependenciesLoaded: function()
		{
			return (typeof(dnn) != 'undefined');
		},

		loadNamespace: function()
		{
			if (this.isLoaded == false)
			{
				if (this.dependenciesLoaded())
				{
					dnn.dom = this; 
					this.isLoaded = true;
					dnn.loadDependencies(this.pns, this.ns);
				}
			}	
		},

		getFormPostString: function(oCtl)
		{
			var sRet = '';
			if (oCtl != null)
			{
				if (oCtl.tagName.toLowerCase() == 'form')	//if form, faster to loop elements collection
				{
					for (var i=0; i<oCtl.elements.length; i++)
						sRet += this.getElementPostString(oCtl.elements[i]);					
				}
				else
				{
					sRet = this.getElementPostString(oCtl);
					for (var i=0; i<oCtl.childNodes.length; i++)
						sRet += this.getElementPostString(oCtl.childNodes[i]);
				}
			}
			return sRet;		
		},
		
		getElementPostString: function(oCtl)
		{
			var sTagName;
			if (oCtl.tagName)
				sTagName = oCtl.tagName.toLowerCase();
				
			if (sTagName == 'input') 
			{
				var sType = oCtl.type.toLowerCase();
				if (sType == 'text' || sType == 'password' || sType == 'hidden' || ((sType == 'checkbox' || sType == 'radio') && oCtl.checked)) 
					return oCtl.name + '=' + dnn.encode(oCtl.value) + '&';
			}
			else if (sTagName == 'select') 
			{
				for (var i=0; i<oCtl.options.length; i++) 
				{
					if (oCtl.options[i].selected) 
						return oCtl.name + '=' + dnn.encode(oCtl.options[i].value) + '&';
				}
			}
			else if (sTagName == 'textarea') 
					return oCtl.name + '=' + dnn.encode(oCtl.value) + '&';
			return '';
		}
}

		dnn_dom.prototype.eventObject = function(e, srcElement)
		{
			this.object = e;
			this.srcElement = srcElement;
		}
		
		//--- dnn.dom.browser
		dnn_dom.prototype.browserObject = function()
		{
			this.InternetExplorer = 'ie';
			this.Netscape = 'ns';
			this.Mozilla = 'mo';
			this.Opera = 'op';
			this.Safari = 'safari';
			this.Konqueror = 'kq';
			this.MacIE = 'macie';
			
			//Please offer a better solution if you have one!
			var sType;
			var agt=navigator.userAgent.toLowerCase();

			if (agt.indexOf('konqueror') != -1) 
				sType = this.Konqueror;
			else if (agt.indexOf('opera') != -1) 
				sType = this.Opera;
			else if (agt.indexOf('netscape') != -1) 
				sType = this.Netscape;
			else if (agt.indexOf('msie') != -1)
			{
				if (agt.indexOf('mac') != -1)
					sType = this.MacIE;
				else
					sType = this.InternetExplorer;
			}
			else if (agt.indexOf('safari') != -1)
				sType = 'safari';
			
			if (sType == null)
				sType = this.Mozilla;  
			
			this.type = sType;
			this.version = parseFloat(navigator.appVersion);
			
			var sAgent = navigator.userAgent.toLowerCase();
			if (this.type == this.InternetExplorer)
			{
				var temp=navigator.appVersion.split("MSIE");
				this.version=parseFloat(temp[1]);
			}
			if (this.type == this.Netscape)
			{
				var temp=sAgent.split("netscape");
				this.version=parseFloat(temp[1].split("/")[1]);	
			}

			//this.majorVersion = null;
			//this.minorVersion = null;
		}
		
dnn_dom.prototype.browserObject.prototype =
{
		toString: function()
		{
			return this.type + ' ' + this.version;
		},
		
		isType: function()
		{
			for (var i=0; i<arguments.length; i++)
			{
				if (dnn.dom.browser.type == arguments[i])
					return true;
			}
			return false;
		}
}	
		//--- End dnn.dom.browser
						
	//--- End dnn.dom

	//--- dnn.controls - not enough here to justify separate js file
	//if (typeof(dnn_controltree) != 'undefined')
	//	dnn_controls.prototype = new dnn_controltree();
	//if (typeof(dnn_control) != 'undefined')
	//	dnn_controls.prototype = new dnn_control;
	
	function dnn_controls()
	{
		this.pns = 'dnn';
		this.ns = 'controls';
		this.dependencies = 'dnn,dnn.dom,dnn.xml'.split(',');
		this.isLoaded = false;
		this.controls = new Array();
		
		this.orient = new Object();
		this.orient.horizontal = 0;
		this.orient.vertical = 1;
		
		this.action = new Object();
		this.action.postback = 0;
		this.action.expand = 1;
		this.action.none = 2;
		this.action.nav = 3;
	}

dnn_controls.prototype = 
{
	dependenciesLoaded: function()
	{
		return (typeof(dnn) != 'undefined' && typeof(dnn.dom) != 'undefined' && typeof(dnn.xml) != 'undefined');
	},

	loadNamespace: function()
	{
		if (this.isLoaded == false)
		{
			if (this.dependenciesLoaded())
			{				
				if (typeof(dnn_control) != 'undefined')
					dnn.extend(dnn_controls.prototype, new dnn_control);

				dnn.controls = new dnn_controls(); 	
				this.isLoaded = true;
				dnn.loadDependencies(this.pns, this.ns);
			}
		}	
	}
}
	dnn_controls.prototype.DNNNode = function(oNode)
	{
		if (oNode != null)
		{
			this.node = oNode; 
			this.id = oNode.getAttribute('id', '');
			this.key = oNode.getAttribute('key', '');
			this.text = oNode.getAttribute('txt', '');
			this.url = oNode.getAttribute('url', '');
			this.js = oNode.getAttribute('js', '');
			this.target = oNode.getAttribute('tar', '');
			this.toolTip = oNode.getAttribute('tTip', '');
			this.enabled = oNode.getAttribute('enabled', '1') != '0';
			this.css = oNode.getAttribute('css', '');
			this.cssSel = oNode.getAttribute('cssSel', '');
			this.cssHover = oNode.getAttribute('cssHover', '');
			this.cssIcon = oNode.getAttribute('cssIcon', '');
			this.hasNodes = oNode.childNodeCount() > 0;	
			this.hasPendingNodes = (oNode.getAttribute('hasNodes', '0') == '1' && this.hasNodes == false);	
			this.imageIndex = new Number(oNode.getAttribute('imgIdx', '-1')); 
			this.image = oNode.getAttribute('img', '');
			this.level = this.getNodeLevel();	//cache
		}
	}
dnn_controls.prototype.DNNNode.prototype = 
{
	childNodeCount: function()
	{
		return this.node.childNodes.length;
	},
	getNodeLevel: function()
	{
		var i=0;
		var oNode = this.node;
		while (oNode != null)
		{
			oNode = oNode.parentNode();
			if (oNode == null || oNode.nodeName() == 'root')
				break;
			i++;
		}	
		return i;
	},
	update: function(sProp)
	{
		if (sProp != null)
		{
			var sType = typeof(this[sProp]);
			
			if (sType == 'string' || sType == 'number' || this[sProp] == null)
				this.node.setAttribute(sProp, this[sProp]);
			else if (sType == 'boolean')
				this.node.setAttribute(sProp, new Number(this[sProp]));
		}
		else
		{
			for (sProp in this)
				this.update(sProp);
		}
	}
}//END DNNNode Methods
	
//--- End dnn.controls


	//--- dnn.utilities
	function dnn_util()
	{
		this.pns = 'dnn';
		this.ns = 'utilities';
		this.dependencies = 'dnn,dnn.dom'.split(',');
		this.isLoaded = false;
	}

	dnn_util.prototype.dependenciesLoaded = function()
	{
		return (typeof(dnn) != 'undefined' && typeof(dnn.dom) != 'undefined');
	}

	dnn_util.prototype.loadNamespace = function()
	{
		if (this.isLoaded == false)
		{
			if (this.dependenciesLoaded())
			{				
				if (typeof(dnn_utility) != 'undefined')
					dnn.extend(dnn_util.prototype, new dnn_utility);

				dnn.util = new dnn_util(); 	
				this.isLoaded = true;
				dnn.loadDependencies(this.pns, this.ns);
			}
		}	
	}
	//--- End dnn.utilities
	
//--- End dnn

//-- prototype/atlas shorthand functions
function $() 
{
  var ary = new Array();
  for (var i=0; i<arguments.length; i++) 
  {
    var arg = arguments[i];
    var ctl;
    if (typeof arg == 'string')
      ctl = dnn.dom.getById(arg);
    else
      ctl = arg;

    if (ctl != null && typeof(Element) != 'undefined' && typeof(Element.extend) != 'undefined')   //if prototype loaded, we must extend the object
        Element.extend(ctl);
        
    if (arguments.length == 1)
      return ctl;

    ary[ary.length] = ctl;
  }
  return ary;
}

//load namespaces
__dnn_m_aNamespaces[__dnn_m_aNamespaces.length] = new dnn_util();
__dnn_m_aNamespaces[__dnn_m_aNamespaces.length] = new dnn_controls();
__dnn_m_aNamespaces[__dnn_m_aNamespaces.length] = new dnn_dom();
__dnn_m_aNamespaces[__dnn_m_aNamespaces.length] = new __dnn();
for (var i=__dnn_m_aNamespaces.length-1; i>=0; i--)
	__dnn_m_aNamespaces[i].loadNamespace();
