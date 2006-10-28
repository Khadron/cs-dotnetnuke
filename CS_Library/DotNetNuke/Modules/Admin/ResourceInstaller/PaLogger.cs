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