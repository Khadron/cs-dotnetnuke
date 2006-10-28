using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace DotNetNuke.Services.Exceptions
{
    [Serializable()]
    public class SchedulerException : BasePortalException
    {
        // default constructor
        public SchedulerException()
        {
        }

        //constructor with exception message
        public SchedulerException(string message)
            : base(message)
        {
        }

        // constructor with message and inner exception
        public SchedulerException(string message, Exception inner)
            : base(message, inner)
        {
        }

        protected SchedulerException(SerializationInfo info, StreamingContext context)
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