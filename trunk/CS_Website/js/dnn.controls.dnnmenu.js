//BEGIN [Needed in case scripts load out of order]
if (typeof(dnn_control) == 'undefined')
	eval('function dnn_control() {}')
//END [Needed in case scripts load out of order]

dnn_control.prototype.initMenu = function (oCtl) 
{
	dnn.extend(dnn.controls.DNNMenuNode.prototype, new dnn.controls.DNNNode);

	dnn.controls.controls[oCtl.id] = new dnn.controls.DNNMenu(oCtl);
	dnn.controls.controls[oCtl.id].generateMenuHTML();
	return dnn.controls.controls[oCtl.id];
}

//------- Constructor -------//
dnn_control.prototype.DNNMenu = function (o)
{
	this.ns = o.id;               //stores namespace for menu
	this.container = o;                    //stores container

	//--- Data Properties ---//  
	this.DOM = new dnn.xml.createDocument();
	this.DOM.loadXml(dnn.getVar(o.id + '_xml'));

	//--- Appearance Properties ---//
	this.mbcss = dnn.dom.getAttr(o, 'mbcss', '');
	this.mcss = dnn.dom.getAttr(o, 'mcss', '');
	this.css = dnn.dom.getAttr(o, 'css', '');
	this.cssChild = dnn.dom.getAttr(o, 'csschild', '');
	this.cssHover = dnn.dom.getAttr(o, 'csshover', '');
	this.cssSel = dnn.dom.getAttr(o, 'csssel', '');
	this.cssIcon = dnn.dom.getAttr(o, 'cssicon', '');

	this.sysImgPath = dnn.dom.getAttr(o, 'sysimgpath', '');
	this.imagePaths = dnn.dom.getAttr(o, 'imagepaths', '').split(',');
	this.imageList = dnn.dom.getAttr(o, 'imagelist', '').split(',');
	for (var i=0; i<this.imageList.length; i++)
	{
		var iPos = this.imageList[i].indexOf(']');
		if (iPos > -1)
			this.imageList[i] = this.imagePaths[this.imageList[i].substring(1, iPos)] + this.imageList[i].substring(iPos+1);
	}
	this.urlList = dnn.dom.getAttr(o, 'urllist', '').split(',');
	
	this.workImg = dnn.dom.getAttr(o, 'workimg', 'dnnanim.gif');	
	this.rootArrow = dnn.dom.getAttr(o, 'rarrowimg', '');
	this.childArrow = dnn.dom.getAttr(o, 'carrowimg', '');
	this.target = dnn.dom.getAttr(o, 'target', '');	
	this.defaultJS = dnn.dom.getAttr(o, 'js', '');	
	this.postBack = dnn.dom.getAttr(o, 'postback', '');
	this.callBack = dnn.dom.getAttr(o, 'callback', '');
	this.callBackStatFunc = dnn.dom.getAttr(o, 'callbacksf', '');
	this.selMenuNode=null;  
	this.rootNode = null;	
	this.orient = new Number(dnn.dom.getAttr(o, 'orient', dnn.controls.orient.horizontal));
	
	this.openMenus = new Array();
	this.moutDelay = dnn.dom.getAttr(o, 'moutdelay', '1000');
	this.minDelay = new Number(dnn.dom.getAttr(o, 'mindelay', 250));

	this.anim = dnn.dom.getAttr(o, 'anim', '');	//expand
	this.useTables = (dnn.dom.getAttr(o, 'usetables', '1') == '1');
	this.enablePostbackState = (dnn.dom.getAttr(o, 'enablepbstate', '0') == '1');
	this.enablePostbackState = true;//F5 in FireFox seems to need this on...  for now always set to true until can provide a workaround.
	this.podInProgress = false;
	
	this.keyboardAccess = (dnn.dom.getAttr(o, 'kbaccess', '1') == '1');
	
	this.childControls = null;

	this.hoverMenuNode = null;
	this.selMenuNode=null;  
	this.rootNode = null;	
	if (this.keyboardAccess)
	{
	
		if (this.container.tabIndex <= 0)
		{
			this.container.tabIndex = 0;
			dnn.dom.addSafeHandler(this.container, 'onkeydown', this, 'keyboardHandler');
			dnn.dom.addSafeHandler(this.container, 'onfocus', this, 'focusHandler');
			dnn.dom.addSafeHandler(this.container, 'onblur', this, 'blurHandler');
		}
		else
		{
			var oTxt = document.createElement('input');
			oTxt.type = 'text';
			oTxt.style.width = 0;
			oTxt.style.height = 0;
			oTxt.style.background = 'transparent';
			oTxt.style.border = 0;
			oTxt.style.positioning = 'absolute';	
			if (dnn.dom.browser.isType(dnn.dom.browser.Safari))
			{
				oTxt.style.width = 1;	//safari doesn't like zero
				oTxt.style.height = 1;	
				dnn.dom.addSafeHandler(oTxt, 'onkeydown', this, 'keyboardHandler'); //'keydownHandler'); 
				dnn.dom.addSafeHandler(this.container.parentNode, 'onkeypress', this, 'safariKeyHandler');	//in order to cancel RETURN (if attach keypress to oTxt, Safari crashes...
			}
			else
				dnn.dom.addSafeHandler(oTxt, 'onkeypress', this, 'keyboardHandler');	
			dnn.dom.addSafeHandler(oTxt, 'onfocus', this, 'focusHandler');
			dnn.dom.addSafeHandler(oTxt, 'onblur', this, 'blurHandler');

			this.container.parentNode.appendChild(oTxt);
		}
	}
}

dnn_control.prototype.DNNMenu.prototype = 
{

//--- Generates menu HTML ---//
getXml: function () 
{
	return this.DOM.getXml();
},

generateMenuHTML: function() 
{
	this.childControls = new Array();
	this.rootNode = this.DOM.rootNode();
	this.container.className = this.mbcss;	

	for (var i=0; i<this.rootNode.childNodeCount(); i++)
		this.renderNode(this.rootNode.childNodes(i), this.container);

	this.update();		
},

renderNode: function(oNode, oCtr)
{
	var oMNode = new dnn.controls.DNNMenuNode(oNode);
	
	if (oMNode.selected)
		this.selMenuNode = oMNode;
	
	var oMBuilder;	
	if (this.useTables && oMNode.level > 0)
		oMBuilder = new dnn.controls.DNNMenuTableBuilder(this, oMNode, oCtr);		
	else
		oMBuilder = new dnn.controls.DNNMenuBuilder(this, oMNode, oCtr);

	if (this.isNodeVertical(oMNode))
		oMBuilder.newRow();

	oMBuilder.newCont();

	if (oMNode.lhtml.length > 0)
		oMBuilder.appendChild(this.renderCustomHTML(oMNode.lhtml));

	var oIcn = this.renderIcon(oMNode);
	oMBuilder.appendChild(oIcn);	//render icon
	if (this.useTables == false || oMNode.level == 0)
		oIcn.className = (oMNode.cssIcon.length > 0 ? oMNode.cssIcon : this.cssIcon);
	else	
		oMBuilder.subcont.className = (oMNode.cssIcon.length > 0 ? oMNode.cssIcon : this.cssIcon);	//assign style to container of icon

	if (oMNode.isBreak == false)
		oMBuilder.appendChild(this.renderText(oMNode), true);	//render text
		
	oMBuilder.newCell();			
	this.renderArrow(oMNode, oMBuilder.subcont);

	if (oMNode.rhtml.length > 0)
		oMBuilder.appendChild(this.renderCustomHTML(oMNode.rhtml));
				
	if (oMNode.toolTip.length > 0)
		oMBuilder.row.title = oMNode.toolTip;

	this.assignCss(oMNode);
	
	if (oMNode.enabled)
		dnn.dom.addSafeHandler(oMBuilder.row, 'onclick', this, 'nodeTextClick');
	
	dnn.dom.addSafeHandler(oMBuilder.row, 'onmouseover', this, 'nodeMOver');
	dnn.dom.addSafeHandler(oMBuilder.row, 'onmouseout', this, 'nodeMOut');
	dnn.dom.addSafeHandler(oMBuilder.container, 'onmouseover', this, 'menuMOver');
	dnn.dom.addSafeHandler(oMBuilder.container, 'onmouseout', this, 'menuMOut');

	if (oMNode.hasNodes || oMNode.hasPendingNodes)	//if node has children render container and hide if necessary
	{
		var oSub = this.renderSubMenu(oMNode);
		this.container.appendChild(oSub);
		
		for (var i=0; i<oNode.childNodeCount(); i++)	//recursively call child rendering
			this.renderNode(oNode.childNodes(i), oSub);
	}				
},

renderCustomHTML: function (sHTML) 
{
	var oCtr = dnn.dom.createElement('span');
	oCtr.innerHTML = sHTML;
	return oCtr;	
},

renderIcon: function (oMNode) 
{
	var oCtr = dnn.dom.createElement('span');
	if (oMNode.imageIndex > -1 || oMNode.image != '')
	{
		var oImg = this.createChildControl('img', oMNode.id, 'icn');
		oImg.src = (oMNode.image.length > 0 ? oMNode.image : this.imageList[oMNode.imageIndex]);
		oCtr.appendChild(oImg);
	}
	
	return oCtr;
},

renderArrow: function (oMNode, oCont) 
{
	if (oMNode.hasNodes || oMNode.hasPendingNodes)
	{
		var sImg = (oMNode.level == 0 ? this.rootArrow : this.childArrow);
		if (sImg.length > 0)
		{
			if (this.useTables && oMNode.level > 0)	//do not require tables to need special padding to properly show arrow, place a real image there and have browser space it appropriately
			{
				var oImg = dnn.dom.createElement('img');
				oImg.src = sImg;
				oCont.appendChild(oImg);			
			}
			else
			{
				oCont.style.backgroundImage = 'url(' + sImg + ')';
				oCont.style.backgroundRepeat = 'no-repeat';
				oCont.style.backgroundPosition = 'right';			
			}
		}
	}
},

renderText: function (oMNode) 
{
	var oCtr = this.createChildControl('span', oMNode.id, 't');
	oCtr.innerHTML = oMNode.text;	
	oCtr.style.cursor = 'pointer';		
	return oCtr;
},

renderSubMenu: function(oMNode)
{
	var oMBuilder;
	if (this.useTables)
		oMBuilder = new dnn.controls.DNNMenuTableBuilder(this, oMNode);		
	else
		oMBuilder = new dnn.controls.DNNMenuBuilder(this, oMNode);
	
	var oSub = oMBuilder.createSubMenu();
	oSub.style.position = 'absolute';
	oSub.style.display = 'none';
	oSub.className = this.mcss;
	return oSub;
},

//---- Methods ---//
hoverNode: function (oMNode) 
{
	if (this.hoverMenuNode != null)
	{
		this.hoverMenuNode.hover = false;
		this.assignCss(this.hoverMenuNode);
	}
	if (oMNode != null)
	{
		oMNode.hover = true;
		this.assignCss(oMNode);
	}
	this.hoverMenuNode = oMNode;
},

__expandNode: function (oContext) 
{
	this.expandNode(oContext, true);
},

expandNode: function (oMNode, bForce) 
{
	dnn.cancelDelay(this.ns + 'min');
		
	if (oMNode.hasPendingNodes)
	{
		if (this.podInProgress == false)
		{
			this.podInProgress = true;
			this.showWorkImage(oMNode, true);
			oMNode.menu = this;	//need to give reference back to self
			
			if (this.callBack.indexOf('[NODEXML]') > -1)
				eval(this.callBack.replace('[NODEXML]', dnn.escapeForEval(oMNode.node.getXml())));
			else
				eval(this.callBack.replace('[NODEID]', oMNode.id));
		}
	}
	else
	{
		if (this.minDelay == 0 || bForce)	
		{
			this.hideMenus(new dnn.controls.DNNMenuNode(oMNode.node.parentNode()));	//MinDelay???
			var oSub = this.getChildControl(oMNode.id, 'sub');
			if (oSub != null)
			{
				this.positionMenu(oMNode, oSub);
				this.showSubMenu(oSub, true);				
				this.openMenus[this.openMenus.length] = oMNode;
			}
		}
		else
			dnn.doDelay(this.ns + 'min', this.minDelay, dnn.createDelegate(this, this.__expandNode), oMNode);
	}
	return true;
},

showSubMenu: function(oSub, bShow)
{
	oSub.style.display = (bShow ? '' : 'none');
	dnn.dom.positioning.placeOnTop(oSub, bShow, this.sysImgPath + 'spacer.gif');
},

showWorkImage: function (oMNode, bShow)
{
	if (this.workImg != null)
	{
		var oIcn = this.getChildControl(oMNode.id, 'icn');	
		if (oIcn != null)
		{
			if (bShow)
				oIcn.src = this.sysImgPath + this.workImg;
			else
				oIcn.src = (oMNode.image.length > 0 ? oMNode.image : this.imageList[oMNode.imageIndex]);
		}
	}

},

isNodeVertical: function (oMNode)
{
	return (this.orient == dnn.controls.orient.vertical || oMNode.level > 0);
},

hideMenus: function (oMNode) 
{
	for (var i=this.openMenus.length-1; i>=0; i--)
	{
		if (oMNode != null && this.openMenus[i].id == oMNode.id)
			break;
		this.collapseNode(this.openMenus[i]);
		this.openMenus.length = this.openMenus.length-1;
	}
},

collapseNode: function (oMNode) 
{
	var oSub = this.getChildControl(oMNode.id, 'sub');
	if (oSub != null)
	{
		this.showSubMenu(oSub, false);
		oMNode.expanded = null;
		oMNode.update();
		this.update();
		return true;
	}
},

positionMenu: function (oMNode, oMenu)
{
	var oPCtl = this.getChildControl(oMNode.id, 'ctr');
	if (oPCtl.tagName == 'TR' && oPCtl.childNodes.length > 0)
		oPCtl = oPCtl.childNodes[oPCtl.childNodes.length-1];	//fix for Safari and Opera... use TD instead of TR
		
	var oPDims = new dnn.dom.positioning.dims(oPCtl);
	var oMDims = new dnn.dom.positioning.dims(oMenu);
	var iScrollLeft = dnn.dom.positioning.bodyScrollLeft();
	var iScrollTop = dnn.dom.positioning.bodyScrollTop()
	//Max = ViewPort + Scroll - Menu's relative offset
	var iMaxTop = dnn.dom.positioning.viewPortHeight() + iScrollTop - oPDims.rot;	
	var iMaxLeft = dnn.dom.positioning.viewPortWidth() + iScrollLeft - oPDims.rol;	
	var iNewTop = oPDims.t;
	var iNewLeft = oPDims.l;
	var iStartTop = oPDims.t;
	var iStartLeft = oPDims.l;

	if (this.isNodeVertical(oMNode))
	{
		iNewLeft = oPDims.l + oPDims.w;
		iStartTop = iMaxTop;
	}
	else
	{
		iNewTop = oPDims.t + oPDims.h;
		iStartLeft = iMaxLeft;
	}	
		
	if (iNewTop + oMDims.h >= iMaxTop)	//if menu doesn't fit below...
	{
		if (oPDims.rot + iStartTop - oMDims.h > iScrollTop)	//see if it fits above
			iNewTop = iStartTop - oMDims.h;
		//else						//cause menu to scroll...
	}
	
	if (iNewLeft + oMDims.w > iMaxLeft)	//if menu doesn't fit to right
	{
		if (oPDims.rol + iStartLeft - oMDims.w > iScrollLeft)  //see if it fits to left
			iNewLeft = iStartLeft - oMDims.w;
	}

	oMenu.style.top = iNewTop + 'px';
	oMenu.style.left = iNewLeft + 'px';
},

selectNode: function (oMNode) 
{		
	if (this.selMenuNode != null)	//unselect previously selected node
	{
		this.selMenuNode.selected = null;
		this.selMenuNode.update('selected');
		this.assignCss(this.selMenuNode);
	}		
	
	oMNode.selected = (oMNode.selected ? null : true);
	oMNode.update('selected');
	this.assignCss(oMNode);
	
	this.selMenuNode = oMNode;
	this.update();

	if (oMNode.hasNodes || oMNode.hasPendingNodes)
		this.expandNode(oMNode, true);	//force display

	if (oMNode.selected)
	{
		var sJS = this.defaultJS;
		if (oMNode.js.length > 0)
			sJS = oMNode.js;
		
		this.enablePostbackState = true; 
		this.update();	//update xml even if enablePostbackState = false
		
		if (sJS.length > 0)
		{
			if (eval(sJS) == false)
				return;	//don't do postback if returns false
		}
		
		if (oMNode.clickAction == dnn.controls.action.postback)
			eval(this.postBack.replace('[NODEID]', oMNode.id));
		else if (oMNode.clickAction == dnn.controls.action.nav)
			dnn.dom.navigate(oMNode.getUrl(this), oMNode.target.length > 0 ? oMNode.target : this.target);
	}
	return true;		
},

assignCss: function (oMNode)
{
	var oCtr = this.getChildControl(oMNode.id, 'ctr');		
	var sCss = this.css;

	if (oMNode.level > 0 && this.cssChild.length > 0)
		sCss = this.cssChild;

	if (oMNode.css.length > 0)
		sCss = oMNode.css;

	if (oMNode.hover)
		sCss += ' ' + (oMNode.cssHover.length > 0 ? oMNode.cssHover : this.cssHover);
	if (oMNode.selected)
		sCss += ' ' + (oMNode.cssSel.length > 0 ? oMNode.cssSel : this.cssSel);
	
	oCtr.className = sCss;
},

update: function (bForce) 
{
	if (this.enablePostbackState || bForce)
		dnn.setVar(this.ns + '_xml', this.DOM.getXml());
	else
		dnn.setVar(this.ns + '_xml', '');
	return true;
},

//--- Event Handlers ---//
focusHandler: function (e) 
{
	var oMNode = this.hoverMenuNode;
	if (oMNode == null)
		oMNode = new dnn.controls.DNNMenuNode(this.rootNode.childNodes(0));
	this.hoverNode(oMNode);
	this.container.onfocus = null;
},

blurHandler: function (e)
{
	if (this.hoverMenuNode != null)
		this.hoverNode(null);
	this.hideMenus();
},

safariKeyHandler: function (e) 
{
	if (e.keyCode == KEY_RETURN)
	{
		if (this.hoverMenuNode != null && this.hoverMenuNode.enabled)
			this.selectNode(this.hoverMenuNode);
		return false;
	}
},

keyboardHandler: function (e) 
{
	if (e.keyCode == KEY_RETURN)
	{
		if (this.hoverMenuNode != null && this.hoverMenuNode.enabled)
			this.selectNode(this.hoverMenuNode);
		return false;
	}
	
	if (e.keyCode == KEY_ESCAPE)
	{
		this.blurHandler();
		return false;
	}
	
	if (e.keyCode >= KEY_LEFT_ARROW && e.keyCode <= KEY_DOWN_ARROW)
	{
		var iDir = (e.keyCode == KEY_UP_ARROW || e.keyCode == KEY_LEFT_ARROW) ? -1 : 1;
		var sAxis = (e.keyCode == KEY_UP_ARROW || e.keyCode == KEY_DOWN_ARROW) ? 'y' : 'x';

		var oMNode = this.hoverMenuNode;
		var oNewMNode;
		if (oMNode == null)
			oMNode = new dnn.controls.DNNMenuNode(this.rootNode.childNodes(0));
		
		var bHorRoot = (oMNode.level == 0 && this.orient == dnn.controls.orient.horizontal);
		if ((sAxis == 'y' && !bHorRoot) || (bHorRoot && sAxis == 'x'))
		{
			this.hideMenus(new dnn.controls.DNNMenuNode(oMNode.node.parentNode()));
			oNewMNode = this.__getNextNode(oMNode, iDir);
		}		
		else 
		{
			if (iDir == -1)
			{
				oNewMNode = new dnn.controls.DNNMenuNode(oMNode.node.parentNode());
				if (oNewMNode.level == 0 && this.orient == dnn.controls.orient.horizontal)
					oNewMNode = this.__getNextNode(new dnn.controls.DNNMenuNode(oMNode.node.parentNode()), iDir);					
				this.hideMenus(oNewMNode);	
					
			}
			else if (iDir == 1)
			{
				if (oMNode.hasNodes || oMNode.hasPendingNodes)
				{
					if (oMNode.expanded != true)
					{
						this.expandNode(oMNode);
						if (this.podInProgress == false)
							oNewMNode = new dnn.controls.DNNMenuNode(oMNode.node.childNodes(0));
					}
				}
				else
				{
					var oNode = oMNode.node;
					while (oNode.parentNode().nodeName() != 'root')
						oNode = oNode.parentNode();
					oNewMNode = new dnn.controls.DNNMenuNode(oNode);
					oNewMNode = this.__getNextNode(oNewMNode, iDir);
					this.hideMenus(new dnn.controls.DNNMenuNode(oNewMNode.node.parentNode()));
				}
			}
		}
		if (oNewMNode != null && oNewMNode.node.nodeName() != 'root')
			this.hoverNode(oNewMNode);
		
		return false;
	}
	
},

__getNextNode: function (oMNode, iDir) 
{
	var oNode;
	var oParentNode = oMNode.node.parentNode();
	var iNodeIndex = oMNode.node.getNodeIndex('id');
	if (iNodeIndex + iDir < 0)	//if first node was selected and going left, select last node
		oNode = oParentNode.childNodes(oParentNode.childNodeCount()-1);
	else if (iNodeIndex + iDir > oParentNode.childNodeCount()-1)
		oNode = oParentNode.childNodes(0);
	else
		oNode = oParentNode.childNodes(iNodeIndex + iDir);
	
	var oRetNode = new dnn.controls.DNNMenuNode(oNode);	
	if (oRetNode.isBreak)
	{
		iNodeIndex += iDir;	//check next one
		if (iNodeIndex + iDir < 0)
			oNode = oParentNode.childNodes(oParentNode.childNodeCount()-1);
		else if (iNodeIndex + iDir > oParentNode.childNodeCount()-1)
			oNode = oParentNode.childNodes(0);
		else
			oNode = oParentNode.childNodes(iNodeIndex + iDir);
		return new dnn.controls.DNNMenuNode(oNode);
	}
	else
		return oRetNode;
},

callBackStatus: function (result, ctx) 
{
	var oMNode = ctx;
	var oMenu = oMNode.menu;
	
	if (oMenu.callBackStatFunc != null && oMenu.callBackStatFunc.length > 0)
	{
		var oPtr = eval(oMenu.callBackStatFunc);
		oPtr(result, ctx);	
	}
},

callBackSuccess: function (result, ctx) 
{
	var oMNode = ctx;
	var oNode = oMNode.node;
	var oMenu = oMNode.menu;
	
	oMenu.showWorkImage(oMNode, false);
	oNode.appendXml(result);

	var oSub = oMenu.getChildControl(oMNode.id, 'sub');	
	for (var i=0; i<oNode.childNodeCount(); i++)	
		oMenu.renderNode(oNode.childNodes(i), oSub);
	
	oMNode.hasPendingNodes = false;
	oMNode.hasNodes = true;

	oMenu.update();

	oSub = oMenu.getChildControl(oMNode.id, 'sub');
	oMenu.expandNode(new dnn.controls.DNNMenuNode(oNode));

	oMenu.callBackStatus(result, ctx);
	oMenu.podInProgress = false;
},

callBackFail: function (result, ctx) 
{
	alert(result);
},

nodeTextClick: function(evt, element)
{
	var oNode = this.DOM.findNode('n', 'id', element.nodeid);
	if (oNode != null)
		this.selectNode(new dnn.controls.DNNMenuNode(oNode));
},

menuMOver: function(evt, element)
{
	dnn.cancelDelay(this.ns + 'mout');
},

menuMOut: function(evt, element)
{
	dnn.cancelDelay(this.ns + 'min');
	if (this.moutDelay > 0)
		dnn.doDelay(this.ns + 'mout', this.moutDelay, dnn.createDelegate(this, this.hideMenus));
	else
		this.hideMenus();
},

nodeMOver: function(evt, element)
{
	var oNode = this.DOM.findNode('n', 'id', element.nodeid);
	if (oNode != null)
	{
		var oMNode = new dnn.controls.DNNMenuNode(oNode);
		//this.hideMenus(new dnn.controls.DNNMenuNode(oNode.parentNode())); //MinDelay???
		oMNode.hover = true;
		this.assignCss(oMNode);
		this.expandNode(oMNode);
	}
},

nodeMOut: function(evt, element)
{
	var oNode = this.DOM.findNode('n', 'id', element.nodeid);
	if (oNode != null)
	{
		var oMNode = new dnn.controls.DNNMenuNode(oNode);
		this.assignCss(oMNode);
	}
},

createChildControl: function (sTag, sNodeID, sPrefix)
{
	var oCtl = dnn.dom.createElement(sTag);
	oCtl.ns = this.ns;
	oCtl.nodeid = sNodeID;
	oCtl.id = this.ns + sPrefix + sNodeID;
	this.childControls[oCtl.id] = oCtl; //cache the control for quicker lookups
	return oCtl;
}, 

getChildControl: function (sNodeID, sPrefix)
{
	var sId = this.ns + sPrefix + sNodeID;
	if (this.childControls[sId] != null)	//retrive from cache if available
		return this.childControls[sId];
	else
		return $(sId);
}
}

//DNNMenuBuilder object
dnn_control.prototype.DNNMenuBuilder = function (oMenu, oMNode, oCont)
{
	this.menu = oMenu;
	this.menuNode = oMNode;
	this.isVertical = oMenu.isNodeVertical(oMNode);
	this.container = oCont;	
	this.row = null;
	this.subcont = null;
}

//DNNMenuBuilder specific methods
dnn_control.prototype.DNNMenuBuilder.prototype = 
{
appendChild: function(oCtl, bNewCell)
{
	this.subcont.appendChild(oCtl);	
},

newCell: function() {},

newCont: function()
{
	if (this.isVertical)
		this.row = this.menu.createChildControl('div', this.menuNode.id, 'ctr');	//container for Node
	else
		this.row = this.menu.createChildControl('span', this.menuNode.id, 'ctr');	//container for Node
	this.subcont = this.row;
	this.container.appendChild(this.subcont);
},

newRow: function()
{
	//if (this.container.childNodes.length > 0)
	//	this.container.appendChild(document.createElement('br'));
},

createSubMenu: function()
{
	return this.menu.createChildControl('DIV', this.menuNode.id, 'sub');	//Not using SPAN due to FireFox bug...
}
}

//DNNMenuTableBuilder object inherits DNNMenuBuilder
dnn_control.prototype.DNNMenuTableBuilder = function (oMenu, oMNode, oCont)
{
	this.base = dnn.controls.DNNMenuBuilder;
	this.base(oMenu, oMNode, oCont);	//invoke base class constructor
	//RootTable???
	/*if (oCont != null && oCont.rows.length > 0)
		this.row = oCont.rows[oCont.rows.length-1];*/
}

//DNNMenuTableBuilder specific methods
dnn_control.prototype.DNNMenuTableBuilder.prototype = 
{
appendChild: function(oCtl, bNewCell)
{
	if (bNewCell)
		this.newCell();
	this.subcont.appendChild(oCtl);	
},

newCont: function()
{
	this.subcont = this.newCell();	//TD	
},

newCell: function()
{
	var  oTD = dnn.dom.createElement('td');
	this.row.appendChild(oTD);	
	this.subcont = oTD;//
	return oTD;
},

newRow: function()
{
	this.row = this.menu.createChildControl('tr', this.menuNode.id, 'ctr');	//TR
	var oTBs = dnn.dom.getByTagName('TBODY', this.container);
	oTBs[0].appendChild(this.row);
},

createSubMenu: function()
{		
	var oSub = this.menu.createChildControl('table', this.menuNode.id, 'sub');
	oSub.border = 0;
	oSub.cellPadding = 0;
	oSub.cellSpacing = 0;
	oSub.appendChild(dnn.dom.createElement('tbody'));
	return oSub;
}
}

dnn_control.prototype.DNNMenuNode = function (oNode)
{
	this.base = dnn.controls.DNNNode;
	this.base(oNode);	//invoke base class constructor
	
	//menu specific attributes
	this.hover = false;
	this.expanded = oNode.getAttribute('expanded', '0') == '1' ? true : null;
	this.selected = oNode.getAttribute('selected', '0') == '1' ? true : null;
	this.clickAction = oNode.getAttribute('ca', dnn.controls.action.postback);
	this.imageIndex = new Number(oNode.getAttribute('iIdx', '-1')); 
	this.urlIndex = new Number(oNode.getAttribute('uIdx', '-1')); 
	this.isBreak = oNode.getAttribute('break', '0') == '1' ? true : false;	//probably move to base DNNNode
	this.lhtml = oNode.getAttribute('lhtml', '');
	this.rhtml = oNode.getAttribute('rhtml', '');
}

//DNNMenuNode specific methods
dnn_control.prototype.DNNMenuNode.prototype = 
{
childNodes: function (iIndex)
{
	if (this.node.childNodes[iIndex] != null)
		return new dnn.controls.DNNMenuNode(this.node.childNodes[iIndex]);
},
getUrl: function (oMenu)
{
	if (this.urlIndex > -1)
		return oMenu.urlList[this.urlIndex] + this.url;
	else
		return this.url;
}
}

//BEGIN [Needed in case scripts load out of order]
if (typeof(dnn_controls) != 'undefined')
{
	dnn.extend(dnn_controls.prototype, dnn_control.prototype);
	dnn.controls = new dnn_controls();
}
//END [Needed in case scripts load out of order]