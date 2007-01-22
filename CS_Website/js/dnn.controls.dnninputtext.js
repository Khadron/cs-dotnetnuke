//DNNInputText is a dynamically loaded script used by the DNNLabelEdit control
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

ltrim: function (s) 
{ 
	return s.replace(/^\s*/, "");
},

rtrim: function (s) 
{ 
	return s.replace(/\s*$/, "");
},

getText: function ()
{
	return this.control.value;
},

setText: function (s)
{
	this.control.value = this.rtrim(this.ltrim(s));
}
}
dnn.extend(dnn_controls.prototype, dnn_control.prototype);
//dnn.controls = new dnn_controls();

dnn.dom.setScriptLoaded('dnn.controls.dnninputtext.js');	//callback for dynamically loaded script