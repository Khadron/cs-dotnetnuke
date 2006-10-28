/*
	DotNetNuke specific enhancements for FreeTextBox
*/

//This method launches the Insert Link Popup
FTB_FreeTextBox.prototype.CreateLink = function() {
	var url = this.imageGalleryUrl.replace('ftb.imagegallery.aspx','ftb.createlink.aspx');
	url = url.replace(/\{0\}/g,url); 
	url += "&ftb=" + this.id;
	var DNNlink = window.open(url,'dnnlink','width=400,height=350,toolbars=0,resizable=1');
	DNNlink.focus();
};

//This method launches the Select Smiley Popup
FTB_FreeTextBox.prototype.SelectSmiley = function() {
	var url = this.imageGalleryUrl.replace('ftb.imagegallery.aspx','ftb.insertsmiley.aspx');
	url = url.replace(/\{0\}/g,url); 
	url += "&ftb=" + this.id;
	var DNNlink = window.open(url,'dnnlink','width=400,height=200,toolbars=0,resizable=1');
	DNNlink.focus();
};

//This method inserts a Link
FTB_FreeTextBox.prototype.InsertLink = function(href, newWindow) {
	if (this.mode != FTB_MODE_DESIGN) return;
	this.designEditor.focus();

	this.designEditor.document.execCommand('createlink', null, href);

	//If new Window set target="_blank" 
	if (newWindow) {
		if (FTB_Browser.isIE) {
			var sel = this.designEditor.document.selection.createRange();
			selection = sel.htmlText; 
		} else {
			selection = this.designEditor.window.getSelection();
		}
		selection = selection.replace(/<A/, "<A target='_blank'");
		this.InsertHtml(selection);
	}

	if (this.clientSideTextChanged)
		this.clientSideTextChanged(this);
};

//This method
FTB_FreeTextBox.prototype.WordClean = function ()	{
    wordContent = this.designEditor.document.body.innerHTML;
    wordContent = String(wordContent).replace(/ class=[^\s|>]*/gi,'');
    wordContent = String(wordContent).replace(/ style=\'[^>]*\'/gi,'');
    wordContent = String(wordContent).replace(/ align=[^\s|>]*/gi,'');
    wordContent = String(wordContent).replace(/<p [^>]*>/gi,'<p>');
    wordContent = String(wordContent).replace(/<b [^>]*>/gi,'<b>');
    wordContent = String(wordContent).replace(/<i [^>]*>/gi,'<i>');
    wordContent = String(wordContent).replace(/<ul [^>]*>/gi,'<ul>');
    wordContent = String(wordContent).replace(/<li [^>]*>/gi,'<li>');
    wordContent = String(wordContent).replace(/<b>/gi,'<strong>');
    wordContent = String(wordContent).replace(/<\/b>/gi,'</strong>');
    wordContent = String(wordContent).replace(/<em>/gi,'<i>');
    wordContent = String(wordContent).replace(/<\/em>/gi,'</i>');
    wordContent = String(wordContent).replace(/<\?xml:[^>]*>/g, '');
    wordContent = String(wordContent).replace(/<\/?st1:[^>]*>/g,'');
    wordContent = String(wordContent).replace(/<\/?[a-z]\:[^>]*>/g,'');
    wordContent = String(wordContent).replace(/<\/?span[^>]*>/gi,'');
    wordContent = String(wordContent).replace(/<\/?div[^>]*>/gi,'');
    wordContent = String(wordContent).replace(/<\/?font[^>]*>/gi,'');
    wordContent = String(wordContent).replace(/<\/?pre[^>]*>/gi,'');
    wordContent = String(wordContent).replace(/<\/?h[1-6][^>]*>/gi,'');
    this.designEditor.document.body.innerHTML = wordContent;
}
