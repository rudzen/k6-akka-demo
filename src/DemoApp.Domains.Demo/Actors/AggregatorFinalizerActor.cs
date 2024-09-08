using Akka.Actor;
using DemoApp.Domains.Demo.Messages;

namespace DemoApp.Domains.Demo.Actors;

/// <summary>
/// Creates the final result we want to send back to origin
/// </summary>
public sealed class AggregatorFinalizerActor : ReceiveActor
{
    public AggregatorFinalizerActor()
    {
        Receive<AggregatorMessageEvents.AggregateFinalize>(FinalizeResult);
    }

    private void FinalizeResult(AggregatorMessageEvents.AggregateFinalize finalize)
    {
        // do some work on the incoming data
        var s = new string[finalize.Results.Count];
        for (var i = 0; i < finalize.Results.Count; i++)
            s[i] = finalize.Results[i].ToString("O");
        finalize.Origin.Tell(new AggregatorMessageEvents.AggregateResponse(s), Sender);
    }
}