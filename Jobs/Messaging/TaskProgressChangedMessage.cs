using CommunityToolkit.Mvvm.Messaging.Messages;

namespace Brupper.Jobs;

public class TaskProgressChangedMessage : ValueChangedMessage<int>
{
    public TaskProgressChangedMessage(int sender)
        : base(sender) { }
}
