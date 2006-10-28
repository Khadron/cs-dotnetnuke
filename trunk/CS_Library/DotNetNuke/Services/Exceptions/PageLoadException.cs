using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace DotNetNuke.Services.Exceptions
{
    [Serializable()]
    public class PageLoadException : BasePortalException
    {
        // default constructor
        public PageLoadException()
        {
        }

        //constructor with exception message
        public PageLoadException(string message)
            : base(message)
        {
        }

        // constructor with message and inner exception
        public PageLoadException(string message, Exception inner)
            : base(message, inner)
        {
        }

        protected PageLoadException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            // Serialize this class' state and then call the base class GetObjectData
            base.GetObjectData(info, context);
        }
    }
}