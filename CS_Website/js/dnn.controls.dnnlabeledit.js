//BEGIN [Needed in case scripts load out of order]
if (typeof(dnn_control) == 'undefined')
	eval('function dnn_control() {}')
//END [Needed in case scripts load out of order]

dnn_control.prototype.initLabelEdit = function (oCtl) 
{
	if (oCtl)
	{
		dnn.controls.controls[oCtl.id] = new dnn.controls.DNNLabelEdit(oCtl);
		return dnn.controls.controls[oCtl.id];
	}
}

//------- Constructor -------//
dnn_control.prototype.DNNLabelEdit = function (o)
{
	this.ns = o.id;               //stores namespace for control
	this.control = o;                    //stores control
	this.editWrapper = null;	//stores dnn wrapper for abstracted edit control
	this.editContainer = null; //stores container of the control (necessary for iframe controls)
	this.editControl = null; //stores reference to underlying edit control (input, span, textarea)
	this.prevText = '';	
	
	this.onblurSave = (dnn.dom.getAttr(o, 'blursave', '1') == '1');
	
	this.toolbarId = dnn.dom.getAttr(o, 'tbId', '');
	this.nsPrefix = dnn.dom.getAttr(o, 'nsPrefix', '');
	this.toolbarEventName = dnn.dom.getAttr(o, 'tbEvent', 'onmousemove');
	this.toolbar = null;
	//this.scriptPath = dnn.dom.getScriptPath();
	//var oThisScript = dnn.dom.getScript('dnn.controls.dnnlabeledit.js');
	//if (oThisScript)
	//	this.scriptPath = oThisScript.src.replace('dnn.controls.dnnlabeledit.js', '');
		
	this.css = o.className;	
	this.cssEdit = dnn.dom.getAttr(o, 'cssEdit', '');
	this.cssWork = dnn.dom.getAttr(o, 'cssWork', '');
	this.cssOver = dnn.dom.getAttr(o, 'cssOver', '');
	this.sysImgPath = dnn.dom.getAttr(o, 'sysimgpath', '');
	this.callBack = dnn.dom.getAttr(o, 'callback', '');
	this.callBackStatFunc = dnn.dom.getAttr(o, 'callbackSF', '');
	this.beforeSaveFunc = dnn.dom.getAttr(o, 'beforeSaveF', '');
	this.eventName = dnn.dom.getAttr(o, 'eventName', 'onclick');
	this.editEnabled = dnn.dom.getAttr(o, 'editEnabled', '1') == '1';
	this.multiLineEnabled = dnn.dom.getAttr(o, 'multiline', '0') == '1';
	this.richTextEnabled = dnn.dom.getAttr(o, 'richtext', '0') == '1';
	this.supportsCE = (document.body.contentEditable != null);
	if (dnn.dom.browser.isType(dnn.dom.browser.Safari))		
		this.supportsCE = false;//Safari content editable still buggy...
	this.supportsRichText = (this.supportsCE || (dnn.dom.browser.isType(dnn.dom.browser.Mozilla) && navigator.productSub >= '20050111'));	//i belive firefox only works well with 1.5 or later, need a better way to detect this!
	
	if (this.eventName != 'none')
		dnn.dom.addSafeHandler(o, this.eventName, this, 'performEdit');
	if (this.toolbarId.length > 0)
		dnn.dom.addSafeHandler(o, this.toolbarEventName, this, 'showToolBar');
	dnn.dom.addSafeHandler(o, 'onmousemove', this, 'mouseMove');
	dnn.dom.addSafeHandler(o, 'onmouseout', this, 'mouseOut');
	
}

dnn_control.prototype.DNNLabelEdit.prototype = 
{

isEditMode: function()
{
	return (this.control.style.display != '')
},

initToolbar: function()
{
	if (this.toolbar == null)
	{
		var sStatus = dnn.dom.scriptStatus('dnn.controls.dnntoolbar.js');
		if (sStatus == 'complete')
		{
			this.toolbar = new dnn.controls.DNNToolBar(this.ns);
			this.toolbar.loadDefinition(this.toolbarId, this.nsPrefix, this.control, this.control.parentNode, this.control, dnn.createDelegate(this, this.toolbarAction));			
			this.handleToolbarDisplay();
		}
		else if (sStatus == '')	//not loaded
			dnn.dom.loadScript(dnn.dom.getScriptPath() + 'dnn.controls.dnntoolbar.js', '', dnn.createDelegate(this, this.initToolbar));
	}

},

toolbarAction: function(btn, src)
{
	var sCA = btn.clickAction;
	if (sCA == 'edit')
		this.performEdit();
	else if (sCA == 'save')
	{
		this.persistEdit();
		this.toolbar.hide();
	}
	else if (sCA == 'cancel')
	{
		this.cancelEdit();
		this.toolbar.hide();	
	}	
	else if (this.isFormatButton(sCA))
	{
		if (this.editWrapper)
		{
			var s;
			if (sCA == 'createlink' && dnn.dom.browser.isType(dnn.dom.browser.InternetExplorer) == false)
				s = prompt(btn.tooltip);
				
			this.editWrapper.focus();
			this.editWrapper.execCommand(sCA, null, s);
		}
	}
		
},

performEdit: function () 
{
	if (this.toolbar)
		this.toolbar.hide();
	this.initEditWrapper();
	if (this.editContainer != null)
	{
		if (dnn.dom.browser.isType(dnn.dom.browser.Mozilla))
			this.control.style.display = '-moz-inline-box';
		this.editContainer.style.height = dnn.dom.positioning.elementHeight(this.control) + 4;
		this.editContainer.style.width = dnn.dom.positioning.elementWidth(this.control.parentNode) //'100%';
		this.editContainer.style.display = '';
		//this.editContainer.style.visibility = '';	//firefox workaround... can't do display none
		this.editContainer.style.overflow = 'auto';
		this.editContainer.style.overflowX = 'hidden';

		this.prevText = this.control.innerHTML;
		if (dnn.dom.browser.isType(dnn.dom.browser.Safari) && this.control.innerText)		//safari gets strange chars... use innerText
			this.prevText = this.control.innerText;
		this.editWrapper.setText(this.prevText);
		this.initEditControl();
		this.control.style.display = 'none';
		this.handleToolbarDisplay();
	}
},

showToolBar: function ()
{
	this.initToolbar();
	if (this.toolbar)
		this.toolbar.show(true);	
},

mouseMove: function () 
{
	if (this.toolbarId.length > 0 && this.toolbarEventName == 'onmousemove')
		this.showToolBar();
	this.control.className = this.css + ' ' + this.cssOver;
},

mouseOut: function () 
{
	//this.initToolbar();
	if (this.toolbar)
		this.toolbar.beginHide();
	this.control.className = this.css;
},


initEditWrapper: function()
{
	if (this.editWrapper == null)
	{
		var bRichText = (this.richTextEnabled && this.supportsRichText);
		var sScript = (bRichText ? 'dnn.controls.dnnrichtext.js' : 'dnn.controls.dnninputtext.js');
		
		var sStatus = dnn.dom.scriptStatus(sScript);
		if (sStatus == 'complete')
		{
			var oTxt;
			if (this.richTextEnabled && this.supportsRichText)
			{
				var func = dnn.dom.getObjMethRef(this, 'initEditControl');
				oTxt = new dnn.controls.DNNRichText(func);
			}
			else
				oTxt = new dnn.controls.DNNInputText(this.multiLineEnabled);
					
			this.editWrapper = oTxt;
			this.editContainer = this.editWrapper.container;
			//this.control.parentNode.appendChild(this.editContainer);
			this.control.parentNode.insertBefore(this.editContainer, this.control);
			if (this.richTextEnabled && this.supportsCE)	//control is instantly available (not an iframe)
				this.initEditControl();
		}
		else if (sStatus == '') //not loaded
			dnn.dom.loadScript(dnn.dom.getScriptPath() + sScript, '', dnn.createDelegate(this, this.performEdit));		//should call self or performEdit?
	}
},

initEditControl: function() 
{
	if (this.editWrapper.initialized)
	{
		this.editControl = this.editWrapper.control;
		this.editControl.className = this.control.className + ' ' + this.cssEdit;
		this.editWrapper.focus();
		if (this.editWrapper.supportsCE || this.editWrapper.isRichText == false)	//if browser supports contentEditable or is a simple INPUT control
		{
			if (this.onblurSave)
				dnn.dom.addSafeHandler(this.editContainer, 'onblur', this, 'persistEdit');	
			dnn.dom.addSafeHandler(this.editControl, 'onkeypress', this, 'handleKeyPress');	
			dnn.dom.addSafeHandler(this.editControl, 'onmousemove', this, 'mouseMove');	
			dnn.dom.addSafeHandler(this.editControl, 'onmouseout', this, 'mouseOut');	
		}
		else	//IFRAME event handlers
		{
			if (this.onblurSave)
				dnn.dom.attachEvent(this.editContainer.contentWindow.document, 'onblur', dnn.dom.getObjMethRef(this, 'persistEdit'));	
			dnn.dom.attachEvent(this.editContainer.contentWindow.document, 'onkeypress', dnn.dom.getObjMethRef(this, 'handleKeyPress'));			
			dnn.dom.attachEvent(this.editContainer.contentWindow.document, 'onmousemove', dnn.dom.getObjMethRef(this, 'mouseMove'));	
			dnn.dom.attachEvent(this.editContainer.contentWindow.document, 'onmouseout', dnn.dom.getObjMethRef(this, 'mouseOut'));	
		}
	}
},

persistEdit: function() 
{
	if (this.editWrapper.getText() != this.prevText)
	{
		if (this.raiseEvent('beforeSaveFunc', null, this))
		{			
			this.editControl.className = this.control.className + ' ' + this.cssWork;
			eval(this.callBack.replace('[TEXT]', dnn.escapeForEval(this.editWrapper.getText())));
		}
	}
	else
		this.showLabel();
},

raiseEvent: function(sFunc, evt, element)
{
	if (this[sFunc].length > 0)
	{
		var oPtr = eval(this[sFunc]);
		return oPtr(evt, element) != false;
	}
	return true;
},

cancelEdit: function () 
{
	this.editWrapper.setText(this.prevText);
	this.showLabel();
},

callBackStatus: function (result, ctx) 
{
	var oLbl = ctx;
	if (oLbl.callBackStatFunc != null && oLbl.callBackStatFunc.length > 0)
	{
		var oPointerFunc = eval(oLbl.callBackStatFunc);
		oPointerFunc(result, ctx);	
	}	
},

callBackSuccess: function (result, ctx) 
{
	ctx.callBackStatus(result, ctx);
	ctx.showLabel();
},

handleToolbarDisplay: function()
{
	if (this.toolbar)
	{
		var bInEdit = this.isEditMode();
		
		for (var sKey in this.toolbar.buttons)
		{
			if (sKey == 'edit')
				this.toolbar.buttons[sKey].visible = !bInEdit;
			else if (this.isFormatButton(sKey))
				this.toolbar.buttons[sKey].visible = (bInEdit && this.editWrapper && this.editWrapper.isRichText);
			else
				this.toolbar.buttons[sKey].visible = bInEdit;					
		
		}
		this.toolbar.refresh();
	}
},

isFormatButton: function(sKey)
{
	return '~bold~italic~underline~justifyleft~justifycenter~justifyright~insertorderedlist~insertunorderedlist~outdent~indent~createlink~'.indexOf('~' + sKey + '~') > -1;
},

showLabel: function () 
{
	this.control.innerHTML = this.editWrapper.getText();
	this.control.style.display = '';
	this.control.className = this.css;
	//this.editContainer.style.width = 0; //firefox workaround
	//this.editContainer.style.visibility = 'hidden';	//firefox workaround
	this.editContainer.style.display = 'none';
	this.handleToolbarDisplay();
},

callBackFail: function (result, ctx) 
{
	alert(result);
	ctx.cancelEdit();
},

handleKeyPress: function (e) 
{
	if (e == null)
	{
		if (dnn.dom.event != null)	//mini hack
			e = dnn.dom.event.object;
		else
			e = this.editWrapper.container.contentWindow.event;
	}	
	if (e.keyCode == 13 && this.editWrapper.supportsMultiLine == false)
	{
		this.persistEdit();
		return false;
	}	
	else if (e.keyCode == 27)
		this.cancelEdit();		
}
}


function __dl_getCSS()	//probably a better way to handle this...
{
	var arr = dnn.dom.getByTagName('link');
	var s = '';
	for (var i=0; i< arr.length; i++)
	{
		s+= '<LINK href="' + arr[i].href + '" type=text/css rel=stylesheet>';
	}
	return s;
}

//BEGIN [Needed in case scripts load out of order]
if (typeof(dnn_controls) != 'undefined')
{
	dnn.extend(dnn_controls.prototype, dnn_control.prototype);
	dnn.controls = new dnn_controls();
}
//END [Needed in case scripts load out of order]