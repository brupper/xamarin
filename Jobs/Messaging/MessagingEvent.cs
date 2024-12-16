using System;

namespace Brupper.Jobs;

public abstract class MessagingEvent : IEvent
{
    public DateTime TriggeredOn { get; protected set; } = DateTime.UtcNow;
}
