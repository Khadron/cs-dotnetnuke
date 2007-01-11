//BEGIN [Needed in case scripts load out of order]
if (typeof(dnn_control) == 'undefined')
	eval('function dnn_control() {}')
//END [Needed in case scripts load out of order]

dnn_control.prototype.initLabelEdit = function (oCtl) 
{
	dnn.controls.controls[oCtl.id] = new dnn.controls.DNNLabelEdit(oCtl);
	return dnn.controls.controls[oCtl.id];
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

	this.css = o.className;	
	this.cssEdit = dnn.dom.getAttr(o, 'cssEdit', '');
	this.cssWork = dnn.dom.getAttr(o, 'cssWork', '');
	this.cssOver = dnn.dom.getAttr(o, 'cssOver', '');
	this.sysImgPath = dnn.dom.getAttr(o, 'sysimgpath', '');
	this.callBack = dnn.dom.getAttr(o, 'callback', '');
	this.callBackStatFunc = dnn.dom.getAttr(o, 'callbackSF', '');
	this.eventName = dnn.dom.getAttr(o, 'eventName', 'onclick');
	this.editEnabled = dnn.dom.getAttr(o, 'editEnabled', '1') == '1';
	this.multiLineEnabled = dnn.dom.getAttr(o, 'multiline', '0') == '1';
	this.richTextEnabled = dnn.dom.getAttr(o, 'richtext', '0') == '1';
	this.supportsCE = (document.body.contentEditable != null);
	if (dnn.dom.browser.isType(dnn.dom.browser.Safari))		
		this.supportsCE = false;//Safari content editable still buggy...
	this.supportsRichText = (this.supportsCE || (dnn.dom.browser.isType(dnn.dom.browser.Mozilla) && navigator.productSub >= '20050111'));	//i belive firefox only works well with 1.5 or later, need a better way to detect this!
	
	dnn.dom.addSafeHandler(o, this.eventName, this, 'performEdit');
	dnn.dom.addSafeHandler(o, 'onmouseover', this, 'mouseOver');
	dnn.dom.addSafeHandler(o, 'onmouseout', this, 'mouseOut');
	
}

dnn_control.prototype.DNNLabelEdit.prototype = 
{
//--- Event Handlers ---//
performEdit: function () 
{
	this.initEditWrapper();
	this.editContainer.style.height = dnn.dom.positioning.elementHeight(this.control) + 4;
	this.editContainer.style.width = dnn.dom.positioning.elementWidth(this.control.parentNode) //'100%';
	this.editContainer.style.display = '';
	//this.editContainer.style.visibility = '';	//firefox workaround... can't do display none
	this.editContainer.style.overflow = 'auto';
	this.editContainer.style.overflowX = 'hidden';

	
	this.prevText = this.control.innerHTML;
	this.editWrapper.setText(this.prevText);
	this.initEditControl();
	this.control.style.display = 'none';
},

mouseOver: function () 
{
	this.control.className = this.css + ' ' + this.cssOver;
},

mouseOut: function () 
{
	this.control.className = this.css;
},

initEditWrapper: function()
{
	if (this.editWrapper == null)
	{
		var oTxt;
		if (this.richTextEnabled && this.supportsRichText)	//disabling firefox for now
		{
			var func = dnn.dom.getObjMethRef(this, 'initEditControl');
			oTxt = new dnn.controls.DNNRichText(func);
		}
		else
			oTxt = new dnn.controls.DNNInputText(this.multiLineEnabled);
				
		this.editWrapper = oTxt;
		this.editContainer = this.editWrapper.container;
		this.control.parentNode.appendChild(this.editContainer);
		if (this.richTextEnabled && this.supportsCE)	//control is instantly available (not an iframe)
			this.initEditControl();
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
			dnn.dom.addSafeHandler(this.editContainer, 'onblur', this, 'persistEdit');	
			dnn.dom.addSafeHandler(this.editControl, 'onkeypress', this, 'handleKeyPress');	
		}
		else	//IFRAME event handlers
		{
			dnn.dom.attachEvent(this.editContainer.contentWindow.document, 'onblur', dnn.dom.getObjMethRef(this, 'persistEdit'));	
			dnn.dom.attachEvent(this.editContainer.contentWindow.document, 'onkeypress', dnn.dom.getObjMethRef(this, 'handleKeyPress'));			
		}
	}
},

persistEdit: function() 
{
	if (this.editWrapper.getText() != this.prevText)
	{
		this.editControl.className = this.control.className + ' ' + this.cssWork;
		eval(this.callBack.replace('[TEXT]', dnn.escapeForEval(this.editWrapper.getText())));
	}
	else
		this.showLabel();
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

showLabel: function () 
{
	this.control.innerHTML = this.editWrapper.getText();
	this.control.style.display = '';
	this.control.className = this.css;
	//this.editContainer.style.width = 0; //firefox workaround
	//this.editContainer.style.visibility = 'hidden';	//firefox workaround
	this.editContainer.style.display = 'none';
},

callBackFail: function (result, ctx) 
{
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

//DNNRichText
dnn_control.prototype.DNNRichText = function (fInit)
{
	this.supportsCE = (document.body.contentEditable != null);
	this.text = '';
	this.supportsMultiLine = true;
	this.document = null;
	this.control = null;
	this.initialized = false;
	this.isRichText = true;

	if (this.supportsCE)
	{
		this.document = document;
		this.container = document.createElement('span');
		this.container.contentEditable = true;	//ie doesn't need no stinkin' iframe
		this.control = this.container;
		this.initialized = true;
	}
	else
	{
		this.container = document.createElement('iframe');
		this.container.src = '';
		this.container.style.border = '0';
		this.initFunc = fInit;	//pointer to function to call when iframe completely loads
		dnn.doDelay(this.container.id + 'initEdit', 10, dnn.dom.getObjMethRef(this, 'initDocument'));	//onreadystate and onload not completely reliable
	}
}

dnn_control.prototype.DNNRichText.prototype = 
{
focus: function()
{
	if (this.supportsCE)
	{
		this.control.focus();
		//this.execCommand('selectall');
	}
	else
		this.container.contentWindow.focus();
},

execCommand: function(cmd)
{
	this.document.execCommand(cmd, false, ';');	
},

getText: function()
{
		return this.control.innerHTML;
},

setText: function (s)
{
	if (this.initialized)
		this.control.innerHTML = s;		
	else
		this.text = s;
},

//method continually called until iframe is completely loaded
initDocument: function ()
{
	if (this.container.contentDocument != null)
	{
		if (this.document == null)	//iframe loaded, now write some HTML, thus causing it to not be loaded again
		{
			this.container.contentDocument.designMode = 'on';
			this.document = this.container.contentWindow.document;
			this.document.open();
			dnn.dom.addSafeHandler(this.container, 'onload', this, 'initDocument');
			this.document.write('<HEAD>' + __dl_getCSS() + '</HEAD><BODY id="__dnn_body"></BODY>');
			this.document.close();
		}
		else if (this.control == null && this.document.getElementById('__dnn_body') != null)	//iframe loaded, now check if body is loaded
		{
			this.control = this.document.getElementById('__dnn_body');
			this.control.style.margin = 0;			
			this.control.tabIndex = 0;
			this.initialized = true;
			this.setText(this.text);
			this.initFunc();		
		}
	}
	if (this.initialized == false)	//iframe and body not loaded, call ourselves until it is
		dnn.doDelay(this.container.id + 'initEdit', 10, dnn.dom.getObjMethRef(this, 'initDocument'));
}
}

//DNNInputText
dnn_control.prototype.DNNInputText = function (bMultiLine)
{
	if (bMultiLine)
		this.control = document.createElement('textarea');	
	else
	{
		this.control = document.createElement('input');
		this.control.type = 'text';
	}
	this.container = this.control;
	this.initialized = true;
	this.supportsMultiLine = bMultiLine;
	this.isRichText = false;

}

dnn_control.prototype.DNNInputText.prototype = 
{
focus: function ()
{
	this.control.focus();
	var iChars = this.getText().length;
	if (this.control.createTextRange)
	{
		var oRange = this.control.createTextRange();
		oRange.moveStart('character', iChars);
		oRange.moveEnd('character', iChars);
		oRange.collapse();
		oRange.select();
	}
	else
	{
		this.control.selectionStart = iChars;
		this.control.selectionEnd = iChars;
	}
	//this.control.select();
},

getText: function ()
{
	return this.control.value;
},

setText: function (s)
{
	this.control.value = s;
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