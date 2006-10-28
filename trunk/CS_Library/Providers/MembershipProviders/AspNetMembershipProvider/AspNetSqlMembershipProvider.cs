using System;
using System.Web;
using AspNetSecurity = System.Web.Security;

namespace DotNetNuke.Security.Membership
{
    /// Project:    DotNetNuke
    /// Namespace:  DotNetNuke.Security.Membership
    /// Class:      AspNetSQLMembershipProvider
    /// <summary>
    /// The AspNetSQLMembershipProvider overrides the default SqlMembershipProvider of
    /// the AspNet Membership Component (MemberRole)
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <history>
    ///     [cnurse]	12/08/2005	documented, and renamed to meet new provider mechanism
    /// </history>
    public class AspNetSqlMembershipProvider : System.Web.Security.SqlMembershipProvider
    {
        /// <summary>
        /// The ApplicationName is used by the AspNet Membership Component (MemberRole)
        /// to segregate the users between Applications.
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        ///     [cnurse]	12/08/2005	documented
        /// </history>
        public override string ApplicationName
        {
            get
            {
                if( Convert.ToString( HttpContext.Current.Items["ApplicationName"] ) == "" )
                {
                    return "/";
                }
                else
                {
                    return Convert.ToString( HttpContext.Current.Items["ApplicationName"] );
                }
            }
            set
            {
                HttpContext.Current.Items["ApplicationName"] = value;
            }
        }
    }
}