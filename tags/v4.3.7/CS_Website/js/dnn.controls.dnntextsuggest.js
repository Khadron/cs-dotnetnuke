//BEGIN [Needed in case scripts load out of order]
if (typeof(dnn_control) == 'undefined')
	eval('function dnn_control() {}')
//END [Needed in case scripts load out of order]

dnn_control.prototype.initTextSuggest = function (oCtl) 
{
	//TODO:DETERMINE WHEN TO LOAD THIS
	//Extends is better than inherits cause inherits overwrites any functions we may have defined specific to DNNTextSuggestNode
	dnn.extend(dnn.controls.DNNTextSuggestNode.prototype, new dnn.controls.DNNNode);
	//dnn.controls.DNNTextSuggestNode.prototype = new dnn.controls.DNNNode;
	
	dnn.controls.controls[oCtl.id] = new dnn.controls.DNNTextSuggest(oCtl);
	return dnn.controls.controls[oCtl.id];
}

//------- Constructor -------//
dnn_control.prototype.DNNTextSuggest = function (o)
{
	this.ns = o.id;               //stores namespace for menu
	this.container = o;                    //stores container
	this.resultCtr = null;
	this.DOM = null;
	//--- Appearance Properties ---//
	this.tscss = dnn.dom.getAttr(o, 'tscss', '');
	this.css = dnn.dom.getAttr(o, 'css', '');
	this.cssChild = dnn.dom.getAttr(o, 'csschild', '');
	this.cssHover = dnn.dom.getAttr(o, 'csshover', '');
	this.cssSel = dnn.dom.getAttr(o, 'csssel', '');
	this.cssIcon = dnn.dom.getAttr(o, 'cssicon', '');

	this.sysImgPath = dnn.dom.getAttr(o, 'sysimgpath', '');
	//this.imageList = dnn.dom.getAttr(o, 'imagelist', '').split(',');
	this.workImg = 'dnnanim.gif';
		
	this.target = dnn.dom.getAttr(o, 'target', '');	
	this.defaultJS = dnn.dom.getAttr(o, 'js', '');	
	
	this.postBack = dnn.dom.getAttr(o, 'postback', '');
	this.callBack = dnn.dom.getAttr(o, 'callback', '');
	this.callBackStatFunc = dnn.dom.getAttr(o, 'callbackSF', '');
	//if (this.callBackStatFunc != null)
	//	this.callBackStatFunc = eval(this.callBackStatFunc);
		
	this.rootNode=null;
	this.selNode=null;  
	this.selIndex=-1;
	//this.delay = new Array();
	this.lookupDelay = dnn.dom.getAttr(o, 'ludelay', '500');

	this.anim = dnn.dom.getAttr(o, 'anim', '');	//expand
	this.inAnimObj = null;
	this.inAnimType = null;
	this.prevText = '';	
	dnn.dom.addSafeHandler(o, 'onkeyup', this, 'keyUp');
	//dnn.dom.addSafeHandler(o, 'onkeydown', this, 'keyDown');
	dnn.dom.addSafeHandler(o, 'onkeypress', this, 'keyPress');

	o.setAttribute('autocomplete', 'off');	
	this.delimiter = dnn.dom.getAttr(o, 'del', '');
	this.idtoken = dnn.dom.getAttr(o, 'idtok', '');
	this.maxRows = new Number(dnn.dom.getAttr(o, 'maxRows', '10'));
	if (this.maxRows == 0)
		this.maxRows = 9999;
	this.minChar = new Number(dnn.dom.getAttr(o, 'minChar', '1'));
	this.caseSensitive = dnn.dom.getAttr(o, 'casesens', '0') == '1';

	this.prevLookupText = '';
	this.prevLookupOffset = 0;	
}

dnn_control.prototype.DNNTextSuggest.prototype = 
{
//--- Event Handlers ---//

keyPress: function (e, element) 
{
	var KEY_RETURN = 13;
	if(e.keyCode == KEY_RETURN)	//stop from posting
		return false;
},

keyUp: function (e, element) 
{
	var KEY_UP_ARROW = 38;
	var KEY_DOWN_ARROW = 40;
	var KEY_RETURN = 13;
	var KEY_ESCAPE = 27;
	dnn.cancelDelay(this.ns + 'kd');
	dnn.doDelay(this.ns + 'kd', this.lookupDelay, dnn.dom.getObjMethRef(this, 'doLookup'));
	this.prevText = this.container.value;
	if (e.keyCode == KEY_UP_ARROW)
		this.setNodeIndex(this.selIndex - 1);
	else if(e.keyCode == KEY_DOWN_ARROW)
		this.setNodeIndex(this.selIndex + 1);
	else if(e.keyCode == KEY_RETURN)
	{
		if (this.selIndex > -1)
		{
			this.selectNode(new dnn.controls.DNNTextSuggestNode(this.rootNode.childNodes(this.selIndex)));
			this.clearResults();
		}
	}
	else if(e.keyCode == KEY_ESCAPE)
		this.clear();
		
},

nodeMOver: function(evt, element)
{
	var oNode = this.DOM.findNode('n', 'id', element.nodeid);
	if (oNode != null)
	{
		var oTSNode = new dnn.controls.DNNTextSuggestNode(oNode);
		oTSNode.hover = true;
		this.assignCss(oTSNode);
	}
},

nodeMOut: function(evt, element)
{
	var oNode = this.DOM.findNode('n', 'id', element.nodeid);
	if (oNode != null)
	{
		var oTSNode = new dnn.controls.DNNTextSuggestNode(oNode);
		oTSNode.hover = false;
		this.assignCss(oTSNode);
	}
},

nodeClick: function(evt, element)
{
	var oNode = this.DOM.findNode('n', 'id', element.nodeid);
	if (oNode != null)
	{
		var oTSNode = new dnn.controls.DNNTextSuggestNode(oNode);
		this.selectNode(oTSNode);
	}
},

// Methods
getTextOffset: function () 
{
	var iOffset = 0;
	if (this.delimiter.length > 0)
	{
		var ary = this.container.value.split(this.delimiter);
		var iPos = dnn.dom.cursorPos(this.container);
		var iLen = 0;
		for (iOffset=0; iOffset<ary.length-1; iOffset++)
		{
			iLen += ary[iOffset].length + 1;
			if (iLen > iPos)
				break;
		}
	}
	return iOffset;
},

setText: function (s, id) 
{
	if (this.idtoken.length > 0)
		s += ' ' + this.idtoken.replace('~', id);
		
	if (this.delimiter.length > 0)
	{
		var ary = this.container.value.split(this.delimiter);
		ary[this.getTextOffset()] = s;

		this.container.value = ary.join(this.delimiter);
		if (this.container.value.lastIndexOf(';') != this.container.value.length - 1)
			this.container.value += ';';
		
	}
	else
		this.container.value = s;

	this.prevText = this.container.value;
},

getText: function () 
{
	if (this.delimiter.length > 0 && this.container.value.indexOf(this.delimiter) > -1)
	{
		var ary = this.container.value.split(this.delimiter);
		return ary[this.getTextOffset()];
	}
	else
		return this.container.value;
},

formatText: function (s) 
{
	if (this.caseSensitive)
		return s;
	else
		return s.toLowerCase();
},

highlightNode: function(iIndex, bHighlight)
{
	if (iIndex > -1)
	{
		var oTSNode = new dnn.controls.DNNTextSuggestNode(this.rootNode.childNodes(this.selIndex));
		oTSNode.hover = bHighlight;
		this.assignCss(oTSNode);				
	}
},

setNodeIndex: function(iIndex)
{
	if (iIndex > -1 && iIndex < this.rootNode.childNodeCount())
	{
		this.highlightNode(this.selIndex, false);
		this.selIndex = iIndex;
		this.highlightNode(this.selIndex, true);
	}
},

selectNode: function (oTSNode) 
{		
	if (this.selNode != null)
	{
		//dnn.dom.getById(__dm_getControlID(this.ns, this.selNode.id, 't')).className = this.nodeCss;
		this.selNode.selected = null;
		this.assignCss(this.selNode);
	}		
	
	if (oTSNode.selected)
	{
		oTSNode.selected = null;
		this.assignCss(oTSNode);
	}
	else
	{
		oTSNode.selected = true;
		this.assignCss(oTSNode);
	}
	
	this.selNode = oTSNode;

	if (oTSNode.selected)
	{
		this.setText(oTSNode.text, oTSNode.id);
		var sJS = '';
		if (this.defaultJS.length > 0)
			sJS = this.defaultJS;
		if (oTSNode.js.length > 0)
			sJS = oTSNode.js;
		
		if (sJS.length > 0)
		{
			if (eval(sJS) == false)
				return;	//don't do postback if returns false
		}
		
		if (oTSNode.clickAction == null || oTSNode.clickAction == dnn.controls.action.postback)
			eval(this.postBack.replace('[TEXT]', this.getText()));
		else if (oTSNode.clickAction == dnn.controls.action.nav)
			dnn.dom.navigate(oTSNode.url, oTSNode.target.length > 0 ? oTSNode.target : this.target);
	}
	return true;		
},

positionMenu: function ()
{
	var oPDims = new dnn.dom.positioning.dims(this.container);
	this.resultCtr.style.left = oPDims.l - dnn.dom.positioning.bodyScrollLeft();			
	this.resultCtr.style.top = oPDims.t + oPDims.h;
},

clearResults: function () 
{
	if (this.resultCtr != null)
		this.resultCtr.innerHTML = '';
	this.selIndex = -1;
	this.selNode = null;
		
},

clear: function () 
{
	this.clearResults();
	this.setText('', '');
},

doLookup: function () 
{
	if (this.getText().length >= this.minChar)
	{
		if (this.needsLookup())
		{
			this.prevLookupOffset = this.getTextOffset();
			this.prevLookupText = this.formatText(this.getText());
			eval(this.callBack.replace('[TEXT]', this.prevLookupText));
		}
		else
			this.renderResults(null);
	}
	else
		this.clearResults();
},

needsLookup: function () 
{
	if (this.DOM == null)
		return true;

	if (this.prevLookupOffset != this.getTextOffset() || this.rootNode == null)
		return true;

	if (this.formatText(this.getText()).indexOf(this.prevLookupText) == 0)	//if starts with previous lookup
	{
		if (this.rootNode.childNodeCount() < this.maxRows)	//if rows are less than max, don't need lookup
			return false;
		
		var oNode;
		var bOneMatch = false;
		var sText = this.getText();
		for (var i=0; i<this.maxRows; i++)
		{
			oNode = new dnn.controls.DNNTextSuggestNode(this.rootNode.childNodes(i));
			if (this.formatText(oNode.text).indexOf(sText) == 0)	
			{
				if (i==0)	//if first shown node hasn't changed
					return false;
				else 
					bOneMatch = true;
			}
			else if (bOneMatch)	//if found match and a row following has no match then we dont need lookup
				return false;
		}		
	}

	return true;

},

renderResults: function (sXML) 
{
	if (sXML != null)
	{
		this.DOM = new dnn.xml.createDocument();
		this.DOM.loadXml(sXML);
	}
	this.rootNode = this.DOM.rootNode();
	if (this.rootNode != null)
	{
		if (this.resultCtr == null)
			this.renderContainer();
			
		this.clearResults();
		for (var i=0; i<this.rootNode.childNodeCount(); i++)
			this.renderNode(this.rootNode.childNodes(i), this.resultCtr);
	}
},

renderContainer: function () 
{
	this.resultCtr = document.createElement('DIV');
	this.container.parentNode.appendChild(this.resultCtr);

	this.resultCtr.className = this.tscss;
	this.resultCtr.style.position = 'absolute';
	this.positionMenu();
},

renderNode: function (oNode, oCont) 
{
	var oTSNode;		
	oTSNode = new dnn.controls.DNNTextSuggestNode(oNode);
	//text must be prefixed by value we are looking for and we must be under the maxRows
	if (this.formatText(oTSNode.text).indexOf(this.formatText(this.getText())) == 0 && oCont.childNodes.length < this.maxRows)
	{
		var oNewContainer = this.createChildControl('DIV', oTSNode.id, 'ctr'); //container for Node
		oNewContainer.appendChild(this.renderText(oTSNode));	//render text

		if (oTSNode.enabled)
		{
			dnn.dom.addSafeHandler(oNewContainer, 'onclick', this, 'nodeClick');
			dnn.dom.addSafeHandler(oNewContainer, 'onmouseover', this, 'nodeMOver');
			dnn.dom.addSafeHandler(oNewContainer, 'onmouseout', this, 'nodeMOut');
		}

		if (oTSNode.toolTip.length > 0)
			oNewContainer.title = oTSNode.toolTip;
			
		oCont.appendChild(oNewContainer);
		this.assignCss(oTSNode);
	}
},

renderText: function (oTSNode) 
{
	var oSpan = this.createChildControl('SPAN', oTSNode.id, 't'); //document.createElement('SPAN');
	oSpan.innerHTML = oTSNode.text;	
	oSpan.style.cursor = 'pointer';
	
	return oSpan;
},

assignCss: function (oTSNode)
{
	var oCtr = this.getChildControl(oTSNode.id, 'ctr'); //dnn.dom.getById(__dm_getControlID(this.ns, oTSNode.id, 'ctr'));//, this.container);
	var sNodeCss = this.css;

	if (oTSNode.css.length > 0)
		sNodeCss = oTSNode.css;

	if (oTSNode.hover)
		sNodeCss += ' ' + (oTSNode.cssHover.length > 0 ? oTSNode.cssHover : this.cssHover);
	if (oTSNode.selected)
		sNodeCss += ' ' + (oTSNode.cssSel.length > 0 ? oTSNode.cssSel : this.cssSel);
	
	oCtr.className = sNodeCss;
},

callBackStatus: function (result, ctx) 
{
	var oText = ctx;
	
	if (oText.callBackStatFunc != null && oText.callBackStatFunc.length > 0)
	{
		var oPointerFunc = eval(oText.callBackStatFunc);
		oPointerFunc(result, ctx);	
	}
	
},

callBackSuccess: function (result, ctx) 
{
	var oText = ctx;
	if (oText.callBackStatFunc != null && oText.callBackStatFunc.length > 0)
	{
		var oPointerFunc = eval(oText.callBackStatFunc);
		oPointerFunc(result, ctx);	
	}
	oText.renderResults(result);

},

callBackFail: function (result, ctx) 
{
	alert(result);
},

createChildControl: function (sTag, sNodeID, sPrefix)
{
	var oCtl = document.createElement(sTag);
	oCtl.ns = this.ns;
	oCtl.nodeid = sNodeID;
	oCtl.id = this.ns + sPrefix + sNodeID; //__dm_getControlID(oCtl.ns, oCtl.nodeid, sPrefix);	
	return oCtl;
}, 

getChildControl: function (sNodeID, sPrefix)
{
	return dnn.dom.getById(this.ns + sPrefix + sNodeID);
}

}//end DNNTextSuggest prototype


dnn_control.prototype.DNNTextSuggestNode = function (oNode)
{
	this.base = dnn.controls.DNNNode;
	this.base(oNode);	//invoke base class constructor

	//textsuggest specific attributes
	this.hover = false;
	this.selected = oNode.getAttribute('selected', '0') == '1' ? true : null;
	this.clickAction = oNode.getAttribute('ca', dnn.controls.action.none);
}

//DNNTextSuggestNode specific methods
dnn_control.prototype.DNNTextSuggestNode.prototype = 
{
childNodes: function (iIndex)
{
	if (this.node.childNodes[iIndex] != null)
		return new dnn.controls.DNNTextSuggestNode(this.node.childNodes[iIndex]);
}
}

//BEGIN [Needed in case scripts load out of order]
if (typeof(dnn_controls) != 'undefined')
{
	dnn.extend(dnn_controls.prototype, dnn_control.prototype);
	dnn.controls = new dnn_controls();
}
//END [Needed in case scripts load out of order]