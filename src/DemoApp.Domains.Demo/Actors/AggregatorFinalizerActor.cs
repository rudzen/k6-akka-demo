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
        if (finalize.Results.Count == 1)
        {
            finalize.Origin.Tell(new AggregatorMessageEvents.AggregateResponse([finalize.Results[0].ToString("O")]), Sender);
        }
        else
        {
            var s = new string[finalize.Results.Count];
            for (var i = 0; i < finalize.Results.Count; i++)
                s[i] = finalize.Results[i].ToString("O");
            finalize.Origin.Tell(new AggregatorMessageEvents.AggregateResponse(s), Sender);
        }
    }
}