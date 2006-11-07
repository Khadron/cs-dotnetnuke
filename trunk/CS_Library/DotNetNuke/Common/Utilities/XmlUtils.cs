#region DotNetNuke License
// DotNetNuke® - http://www.dotnetnuke.com
// Copyright (c) 2002-2006
// by Perpetual Motion Interactive Systems Inc. ( http://www.perpetualmotion.ca )
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
using System.IO;
using System.Net;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.Xsl;

namespace DotNetNuke.Common.Utilities
{
    /// <Summary>
    /// The XmlUtils class provides Shared/Static methods for manipulating xml files
    /// </Summary>
    public class XmlUtils
    {
        public static XmlAttribute CreateAttribute(XmlDocument objDoc, string attName, string attValue)
        {
            XmlAttribute attribute = objDoc.CreateAttribute(attName);
            attribute.Value = attValue;

            return attribute;
        }

        public static XmlElement CreateElement(XmlDocument objDoc, string NodeName, string NodeValue)
        {
            XmlElement element = objDoc.CreateElement(NodeName);
            element.InnerText = NodeValue;

            return element;
        }

        public static object Deserialize(string xmlObject, Type type)
        {
            XmlSerializer ser = new XmlSerializer(type);
            StringReader sr = new StringReader(xmlObject);

            return ser.Deserialize(sr);
        }

        /// <summary>
        /// Gets the value of node
        /// </summary>
        /// <param name="objNode">Parent node</param>
        /// <param name="NodeName">Child node to look for</param>
        /// <param name="DefaultValue">Default value to return</param>
        /// <returns></returns>
        /// <remarks>
        /// If the node does not exist or it causes any error the default value will be returned.
        /// </remarks>
        public static string GetNodeValue(XmlNode objNode, string NodeName, string DefaultValue)
        {
            string strValue = DefaultValue;

            try
            {
                strValue = objNode[NodeName].InnerText;

                if (strValue == "" && DefaultValue != "")
                {
                    strValue = DefaultValue;
                }
            }
            catch
            {
                // node does not exist - legacy issue
            }

            return strValue;
        }

        /// <summary>
        /// Gets the value of node
        /// </summary>
        /// <param name="objNode">Parent node</param>
        /// <param name="NodeName">Child node to look for</param>
        /// <param name="DefaultValue">Default value to return</param>
        /// <returns></returns>
        /// <remarks>
        /// If the node does not exist or it causes any error the default value will be returned.
        /// </remarks>
        public static bool GetNodeValueBoolean(XmlNode objNode, string NodeName, bool DefaultValue)
        {
            bool bValue = DefaultValue;

            try
            {
                bValue = Convert.ToBoolean(objNode[NodeName].InnerText);
            }
            catch
            {
                // node does not exist / data conversion error - legacy issue: use default value
            }

            return bValue;
        }

        /// <summary>
        /// Gets the value of node
        /// </summary>
        /// <param name="objNode">Parent node</param>
        /// <param name="NodeName">Child node to look for</param>
        /// <param name="DefaultValue">Default value to return</param>
        /// <returns></returns>
        /// <remarks>
        /// If the node does not exist or it causes any error the default value will be returned.
        /// </remarks>
        public static DateTime GetNodeValueDate(XmlNode objNode, string NodeName, DateTime DefaultValue)
        {
            DateTime dateValue = DefaultValue;

            try
            {
                if (objNode[NodeName].InnerText != "")
                {
                    dateValue = Convert.ToDateTime(objNode[NodeName].InnerText);
                    if (dateValue.Date.Equals(Null.NullDate.Date))
                    {
                        dateValue = Null.NullDate;
                    }
                }
            }
            catch
            {
                // node does not exist / data conversion error - legacy issue: use default value
            }

            return dateValue;
        }

        /// <summary>
        /// Gets the value of node
        /// </summary>
        /// <param name="objNode">Parent node</param>
        /// <param name="NodeName">Child node to look for</param>
        /// <param name="DefaultValue">Default value to return</param>
        /// <returns></returns>
        /// <remarks>
        /// If the node does not exist or it causes any error the default value will be returned.
        /// </remarks>
        public static int GetNodeValueInt(XmlNode objNode, string NodeName, int DefaultValue)
        {
            int intValue = DefaultValue;

            try
            {
                intValue = Convert.ToInt32(objNode[NodeName].InnerText);
            }
            catch
            {
                // node does not exist / data conversion error - legacy issue: use default value
            }

            return intValue;
        }

        /// <summary>
        /// Gets the value of node
        /// </summary>
        /// <param name="objNode">Parent node</param>
        /// <param name="NodeName">Child node to look for</param>
        /// <param name="DefaultValue">Default value to return</param>
        /// <returns></returns>
        /// <remarks>
        /// If the node does not exist or it causes any error the default value will be returned.
        /// </remarks>
        public static float GetNodeValueSingle(XmlNode objNode, string NodeName, float DefaultValue)
        {
            float sValue = DefaultValue;

            try
            {
                sValue = Convert.ToSingle(objNode[NodeName].InnerText);
            }
            catch
            {
                // node does not exist / data conversion error - legacy issue: use default value
            }

            return sValue;
        }

        /// <summary>
        /// GetXMLContent loads the xml content into an Xml Document
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="ContentUrl">The url to the xml text</param>
        /// <returns>An XmlDocument</returns>
        public static XmlDocument GetXMLContent(string ContentUrl)
        {
            XmlDocument returnValue;
            //This function reads an Xml document via a Url and returns it as an XmlDocument object

            returnValue = new XmlDocument();
            WebRequest req = WebRequest.Create(ContentUrl);
            WebResponse result = req.GetResponse();
            Stream ReceiveStream = result.GetResponseStream();
            XmlReader objXmlReader = new XmlTextReader(result.GetResponseStream());
            returnValue.Load(objXmlReader);

            return returnValue;
        }

        /// <summary>
        /// GetXSLContent loads the xsl content into an Xsl Transform
        /// </summary>
        /// <remarks>
        /// Even though this method uses and returns the deprecated class XslTransform,
        /// it has been retained for backwards compatability.
        /// </remarks>
        /// <param name="ContentURL">The url to the xsl text</param>
        /// <returns>An XslTransform</returns>
        public static XslTransform GetXSLContent(string ContentURL)
        {
            XslTransform returnValue;

            returnValue = new XslTransform();
            WebRequest req = WebRequest.Create(ContentURL);
            WebResponse result = req.GetResponse();
            Stream ReceiveStream = result.GetResponseStream();
            XmlReader objXSLTransform = new XmlTextReader(result.GetResponseStream());
            returnValue.Load(objXSLTransform, null, null);

            return returnValue;
        }

        /// <summary>
        /// Serializes an object to Xml using the XmlAttributes
        /// </summary>
        /// <param name="obj">The object to Serialize</param>
        /// <returns></returns>
        /// <remarks>
        /// An Xml representation of the object.
        /// </remarks>
        public static string Serialize(object obj)
        {
            XmlDocument xmlDoc = new XmlDocument();
            XmlSerializer xser = new XmlSerializer(obj.GetType());
            StringWriter sw = new StringWriter();

            xser.Serialize(sw, obj);

            xmlDoc.LoadXml(sw.GetStringBuilder().ToString());
            XmlNode xmlDocEl = xmlDoc.DocumentElement;
            xmlDocEl.Attributes.Remove(xmlDocEl.Attributes["xmlns:xsd"]);
            xmlDocEl.Attributes.Remove(xmlDocEl.Attributes["xmlns:xsi"]);

            return xmlDocEl.OuterXml;
        }

        /// <summary>
        /// Xml Encodes HTML
        /// </summary>
        /// <param name="HTML">The HTML to encode</param>
        /// <returns></returns>
        public static string XMLEncode(string HTML)
        {
            return "<![CDATA[" + HTML + "]]>";
        }

        public static void AppendElement(ref XmlDocument objDoc, XmlNode objNode, string attName, string attValue, bool includeIfEmpty)
        {
            if (attValue == "" && !includeIfEmpty)
            {
                return;
            }
            objNode.AppendChild(CreateElement(objDoc, attName, attValue));
        }

        public static void XSLTransform(XmlDocument doc, ref StreamWriter writer, string xsltUrl)
        {
            XslCompiledTransform xslt = new XslCompiledTransform();
            xslt.Load(xsltUrl);
            //Transform the file.
            xslt.Transform(doc, null, writer);
        }
    }
}