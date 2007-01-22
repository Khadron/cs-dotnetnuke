#region DotNetNuke License
// DotNetNuke® - http://www.dotnetnuke.com
// Copyright (c) 2002-2006
// by DotNetNuke Corporation
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
// documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
// the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
// to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions 
// of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
// TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
// THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
// CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
// DEALINGS IN THE SOFTWARE.
#endregion
using System;
using System.Collections;
using System.Web;
using System.Web.UI.WebControls;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Framework.Providers;
using DotNetNuke.Security.Authentication;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Localization;
using DotNetNuke.UI.Skins.Controls;

namespace DotNetNuke.Modules.Authentication
{
	
	public partial class AuthenticationSettings : PortalModuleBase
	{
	    protected void Page_Load(Object sender, EventArgs e)
		{
			//Put user code to initialize the page here
			try
			{
				// Obtain PortalSettings from Current Context
				AuthenticationController objAuthenticationController = new AuthenticationController();
				
				// Reset config
				Configuration.ResetConfig();
				Configuration config = Configuration.GetConfig();
				
				if (UserInfo.Username.IndexOf("\\") > 0)
				{
					string strDomain = GetUserDomainName(UserInfo.Username);
					if (strDomain.ToLower() == Request.ServerVariables["SERVER_NAME"].ToLower())
					{						
                        DotNetNuke.UI.Skins.Skin.AddModuleMessage(this, string.Format(Localization.GetString("SameDomainError", this.LocalResourceFile), strDomain, HttpUtility.HtmlEncode(Request.ServerVariables["SERVER_NAME"])), ModuleMessageType.YellowWarning);
						DisableScreen();
						return;
					}
				}
				
				if (! Page.IsPostBack)
				{
					ProviderConfiguration objProviderConfiguration = ProviderConfiguration.GetProviderConfiguration(Configuration.AUTHENTICATION_KEY);
					
					chkAuthentication.Checked = config.WindowsAuthentication;
					chkSynchronizeRole.Checked = config.SynchronizeRole;
					chkSynchronizePassword.Checked = config.SynchronizePassword;
					txtRootDomain.Text = config.RootDomain;
					txtUserName.Text = config.UserName;
					txtEmailDomain.Text = config.EmailDomain;
					
					// Bind Authentication provider list, this allows each portal could use different provider for authentication
					foreach (object _Provider in objProviderConfiguration.Providers)
					{
						DictionaryEntry objProvider = (DictionaryEntry) _Provider;
						string ProviderName = Convert.ToString(objProvider.Key);
						string ProviderType = ((Provider) objProvider.Value).Type;
						
						this.cboProviders.Items.Add(new ListItem(ProviderName, ProviderType));
					}
					
					// Bind AuthenticationTypes list, on first configure, it could obtains only from default authentication provider
					try
					{
						this.cboAuthenticationType.DataSource = objAuthenticationController.AuthenticationTypes();
					}
					catch (TypeInitializationException)
					{
						UI.Skins.Skin.AddModuleMessage(this, Localization.GetString("AuthProviderError", this.LocalResourceFile), ModuleMessageType.YellowWarning);
						DisableScreen();
						return;
					}
					this.cboAuthenticationType.DataBind();
					this.cboAuthenticationType.Items.FindByText(config.AuthenticationType).Selected = true;
					
				}
				
				valConfirm.ErrorMessage = Localization.GetString("PasswordMatchFailure", this.LocalResourceFile);
				
			}
			catch (Exception exc) //Module failed to load
			{
				Exceptions.ProcessModuleLoadException(this, exc);
			}
		}
		
		private static string GetUserDomainName(string UserName)
		{
			string strReturn = "";
			if (UserName.IndexOf("\\") > 0)
			{
				strReturn = UserName.Substring(0, (UserName.IndexOf("\\")));
			}
			return strReturn;
		}
		
		private void DisableScreen()
		{
			chkSynchronizeRole.Enabled = false;
			cmdUpdate.Visible = false;
			chkAuthentication.Enabled = false;
			cboProviders.Enabled = false;
			txtUserName.Enabled = false;
			txtPassword.Enabled = false;
			txtConfirm.Enabled = false;
			valConfirm.Enabled = false;
			cboAuthenticationType.Enabled = false;
			chkSynchronizePassword.Enabled = false;
			txtRootDomain.Enabled = false;
			txtEmailDomain.Enabled = false;
		}

	    protected void cmdAuthenticationUpdate_Click(Object sender, EventArgs e)
		{
			PortalSettings _portalSettings = (PortalSettings) HttpContext.Current.Items["PortalSettings"];
			try
			{
				string providerTypeName = this.cboProviders.SelectedItem.Value;
				string authenticationType = this.cboAuthenticationType.SelectedItem.Value;
				
				Configuration.UpdateConfig(_portalSettings.PortalId, this.chkAuthentication.Checked, this.txtRootDomain.Text, this.txtEmailDomain.Text, this.txtUserName.Text, this.txtPassword.Text, this.chkSynchronizeRole.Checked, this.chkSynchronizePassword.Checked, providerTypeName, authenticationType);
				Configuration.ResetConfig();
				
				AuthenticationController objAuthenticationController = new AuthenticationController();
				string statusMessage = objAuthenticationController.NetworkStatus();
				
				if (statusMessage.ToLower().IndexOf("fail") > - 1)
				{
					MessageCell.Controls.Add(UI.Skins.Skin.GetModuleMessageControl("", LocalizedStatus(statusMessage), ModuleMessageType.RedError));
				}
				else
				{
					MessageCell.Controls.Add(UI.Skins.Skin.GetModuleMessageControl("", LocalizedStatus(statusMessage), ModuleMessageType.GreenSuccess));
				}
				
			}
			catch (Exception exc) //Module failed to load
			{
				Exceptions.ProcessModuleLoadException(this, exc);
			}
		}
		
		private string LocalizedStatus(string InputText)
		{
			//Return InputText
			string strReturn = InputText;
			strReturn = strReturn.Replace("[Global Catalog Status]", Localization.GetString("[Global Catalog Status]", this.LocalResourceFile));
			strReturn = strReturn.Replace("[Root Domain Status]", Localization.GetString("[Root Domain Status]", this.LocalResourceFile));
			strReturn = strReturn.Replace("[LDAP Status]", Localization.GetString("[LDAP Status]", this.LocalResourceFile));
			strReturn = strReturn.Replace("[Network Domains Status]", Localization.GetString("[Network Domains Status]", this.LocalResourceFile));
			strReturn = strReturn.Replace("[LDAP Error Message]", Localization.GetString("[LDAP Error Message]", this.LocalResourceFile));
			strReturn = strReturn.Replace("OK", Localization.GetString("OK", this.LocalResourceFile));
			strReturn = strReturn.Replace("FAIL", Localization.GetString("FAIL", this.LocalResourceFile));
			//
			return strReturn;
			
		}
		
		
	}
	
}

