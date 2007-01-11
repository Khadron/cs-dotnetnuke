FreeTextBox v3.0.1 Localization
-------------------------------
FreeTextBox.dll contains all the language resources listed in the Languages.xml file. Supported languages do not need to be listed in Languages.xml. Languages.xml is just a list of language resources embedded in the dll.

The way the localization works is:
1 - FreeTextBox first loads english from the internal resource to populate all the needed values

2 - It will look for a physical language file correspoding with the selected language
2.1 - If there is no physical file it will look for an internal resource.

3 - If there is a valid language file in (2) or (2.1) then FreeTextBox will load all available values. If a value is missing, the original (1) internal English value will be used.

For more information www.freetextbox.com

DotNetNuke will use the current language for the user to set the localized language in FreeTextBox.

DotNetNuke Core Team