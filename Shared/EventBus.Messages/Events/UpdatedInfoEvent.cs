namespace Shared.EventBus.Messages.Events
{
    public class UpdatedInfoEvent : IntegrationBaseEvent
    {
        public List<string> UserIds { get; set; }
        public Dictionary<string, string> UpdatedProperties { get; set; }
    }
}
