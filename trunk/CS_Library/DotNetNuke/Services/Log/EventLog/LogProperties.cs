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
using System.Text;
using System.Xml.Serialization;
using System.IO;
using System.Text;
using System.Xml;

namespace DotNetNuke.Services.Log.EventLog
{
    public class LogProperties : ArrayList
    {
        const int MAX_LEN = 75;

        public string Summary
        {
            get
            {
                string summary = this.ToString();
                if(String.IsNullOrEmpty(summary))
                {
                    return "Empty Summary";
                }
                else if (summary.Length > MAX_LEN)
                {
                    return summary.Substring( 0, MAX_LEN );
                }
                else
                {
                    return summary;
                }
            }
        }

        public void Deserialize(string content)
        {
            using (XmlReader reader = XmlReader.Create(new StringReader(content)))
            {
                reader.ReadStartElement("LogProperties");
                if (reader.ReadState != ReadState.EndOfFile & reader.NodeType != XmlNodeType.None & reader.LocalName != "")
                {
                    ReadXml(reader);
                }
                reader.Close();
            }
        }

        public void ReadXml(XmlReader reader)
        {
            do
            {
                reader.ReadStartElement("LogProperty");

                //Create new LogDetailInfo object
                LogDetailInfo logDetail = new LogDetailInfo();

                //Load it from the Xml
                logDetail.ReadXml(reader);

                //Add to the collection
                this.Add(logDetail);

            } while (reader.ReadToNextSibling("LogProperty"));
        }

        public string Serialize()
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.ConformanceLevel = ConformanceLevel.Fragment;
            settings.OmitXmlDeclaration = true;

            StringBuilder sb = new StringBuilder();

            XmlWriter writer = XmlWriter.Create(sb, settings);
            WriteXml(writer);
            writer.Close();
            return sb.ToString();
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (LogDetailInfo logDetail in this)
            {
                sb.Append(logDetail.ToString());
            }
            return sb.ToString();
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement("LogProperties");
            foreach (LogDetailInfo logDetail in this)
            {
                logDetail.WriteXml(writer);
            }
            writer.WriteEndElement();
        }
    }
}