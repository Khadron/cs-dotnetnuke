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
using System.Web.UI.HtmlControls;
using DotNetNuke.Services.Localization;

namespace DotNetNuke.Modules.Admin.ResourceInstaller
{
    public class PaLogger : ResourceInstallerBase
    {
        private string _ErrorClass;
        private string _HighlightClass;
        private string _NormalClass;
        private ArrayList m_Logs;
        private bool m_Valid;

        public string ErrorClass
        {
            get
            {
                if (_ErrorClass == "")
                {
                    _ErrorClass = "NormalRed";
                }
                return _ErrorClass;
            }
            set
            {
                _ErrorClass = value;
            }
        }

        public string HighlightClass
        {
            get
            {
                if (_HighlightClass == "")
                {
                    _HighlightClass = "NormalBold";
                }
                return _HighlightClass;
            }
            set
            {
                _HighlightClass = value;
            }
        }

        public ArrayList Logs
        {
            get
            {
                return m_Logs;
            }
        }

        public string NormalClass
        {
            get
            {
                if (_NormalClass == "")
                {
                    _NormalClass = "Normal";
                }
                return _NormalClass;
            }
            set
            {
                _NormalClass = value;
            }
        }

        public bool Valid
        {
            get
            {
                return m_Valid;
            }
        }

        public PaLogger()
        {
            m_Logs = new ArrayList();
            m_Valid = true;
        } //New

        /// <summary>
        /// GetLogsTable formats log entries in an HtmlTable
        /// </summary>
        public HtmlTable GetLogsTable()
        {
            HtmlTable arrayTable = new HtmlTable();

            PaLogEntry LogEntry;

            foreach (PaLogEntry tempLoopVar_LogEntry in Logs)
            {
                LogEntry = tempLoopVar_LogEntry;
                HtmlTableRow tr = new HtmlTableRow();
                HtmlTableCell tdType = new HtmlTableCell();
                tdType.InnerText = Localization.GetString("LOG.PALogger." + LogEntry.Type.ToString());
                HtmlTableCell tdDescription = new HtmlTableCell();
                tdDescription.InnerText = LogEntry.Description;
                tr.Cells.Add(tdType);
                tr.Cells.Add(tdDescription);
                switch (LogEntry.Type)
                {
                    case PaLogType.Failure:
                        tr.Attributes.Add("class", ErrorClass);
                        break;

                    case PaLogType.Warning:

                        tr.Attributes.Add("class", ErrorClass);
                        break;
                    case PaLogType.StartJob:
                        tr.Attributes.Add("class", HighlightClass);
                        break;

                    case PaLogType.EndJob:

                        tr.Attributes.Add("class", HighlightClass);
                        break;
                    case PaLogType.Info:

                        tr.Attributes.Add("class", NormalClass);
                        break;
                }
                arrayTable.Rows.Add(tr);
                if (LogEntry.Type == PaLogType.EndJob)
                {
                    HtmlTableRow SpaceTR = new HtmlTableRow();
                    HtmlTableCell SpaceTD = new HtmlTableCell();
                    SpaceTD.ColSpan = 2;
                    SpaceTD.InnerHtml = "&nbsp;";
                    SpaceTR.Cells.Add(SpaceTD);
                    arrayTable.Rows.Add(SpaceTR);
                }
            }

            return arrayTable;
        }

        public void Add(Exception ex)
        {
            AddFailure(EXCEPTION + ex.ToString());
        }

        public void AddFailure(string failure)
        {
            m_Logs.Add(new PaLogEntry(PaLogType.Failure, failure));
            m_Valid = false;
        }

        public void AddInfo(string info)
        {
            m_Logs.Add(new PaLogEntry(PaLogType.Info, info));
        }

        public void AddWarning(string warning)
        {
            m_Logs.Add(new PaLogEntry(PaLogType.Warning, warning));
        }

        public void EndJob(string job)
        {
            m_Logs.Add(new PaLogEntry(PaLogType.EndJob, job));
        }

        public void StartJob(string job)
        {
            m_Logs.Add(new PaLogEntry(PaLogType.StartJob, job));
        }
    } 
}