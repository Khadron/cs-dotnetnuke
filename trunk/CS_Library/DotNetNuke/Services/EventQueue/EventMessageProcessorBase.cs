namespace DotNetNuke.Services.EventQueue
{
    public abstract class EventMessageProcessorBase
    {
        public abstract bool ProcessMessage( EventMessage message );
    }
}