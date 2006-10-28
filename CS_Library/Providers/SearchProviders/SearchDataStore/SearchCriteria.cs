namespace DotNetNuke.Services.Search
{
    /// <summary>
    /// The SearchCriteria represents a search criterion
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <history>
    ///		[cnurse]	11/15/2004	documented
    /// </history>
    public class SearchCriteria
    {
        private string m_Criteria;
        private bool m_Required;
        private bool m_Excluded;

        public string Criteria
        {
            get
            {
                return m_Criteria;
            }
            set
            {
                m_Criteria = value;
            }
        }

        public bool MustExclude
        {
            get
            {
                return m_Excluded;
            }
            set
            {
                m_Excluded = value;
            }
        }

        public bool MustInclude
        {
            get
            {
                return m_Required;
            }
            set
            {
                m_Required = value;
            }
        }
    }
}