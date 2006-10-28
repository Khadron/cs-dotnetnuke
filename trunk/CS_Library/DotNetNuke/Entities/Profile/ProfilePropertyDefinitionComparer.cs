using System.Collections;

namespace DotNetNuke.Entities.Profile
{
    /// <Summary>
    /// The ProfilePropertyDefinitionComparer class provides an implementation of
    /// IComparer to sort the ProfilePropertyDefinitionCollection by ViewOrder
    /// </Summary>
    public class ProfilePropertyDefinitionComparer : IComparer
    {
        /// <Summary>Compares two ProfilePropertyDefinition objects</Summary>
        /// <Param name="x">A ProfilePropertyDefinition object</Param>
        /// <Param name="y">A ProfilePropertyDefinition object</Param>
        /// <Returns>
        /// An integer indicating whether x greater than y, x=y or x less than y
        /// </Returns>
        public virtual int Compare( object x, object y )
        {
            return ( (ProfilePropertyDefinition)x ).ViewOrder.CompareTo( ( (ProfilePropertyDefinition)y ).ViewOrder );
        }
    }
}