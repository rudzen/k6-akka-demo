using Akka.Actor;
using DemoApp.Domains.Demo.Messages;

namespace DemoApp.Domains.Demo.Actors;

public sealed class SmallestMailBoxActor : ReceiveActor
{
    public SmallestMailBoxActor()
    {
        Receive<SmallestMailBoxEvents.SmalledMailBoxMessage>(_ =>
        {
            Sender.Tell(new SmallestMailBoxEvents.SmallesMailBoxResponse(DateTime.UtcNow));
        });
    }
}