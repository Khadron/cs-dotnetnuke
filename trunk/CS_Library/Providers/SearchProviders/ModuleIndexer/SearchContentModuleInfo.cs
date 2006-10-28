 //using Entities.Modules;

using DotNetNuke.Entities.Modules;

namespace DotNetNuke.Services.Search
{
    /// <summary>
    /// The SearchContentModuleInfo class represents an extendension (by containment)
    /// of ModuleInfo to add a parametere that determines whether a module is Searchable
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <history>
    ///		[cnurse]	11/15/2004	documented
    /// </history>
    public class SearchContentModuleInfo
    {
        protected ModuleInfo m_ModInfo;
        protected ISearchable m_ModControllerType;

        public ISearchable ModControllerType
        {
            get
            {
                return m_ModControllerType;
            }
            set
            {
                m_ModControllerType = value;
            }
        }

        public ModuleInfo ModInfo
        {
            get
            {
                return m_ModInfo;
            }
            set
            {
                m_ModInfo = value;
            }
        }
    }
}