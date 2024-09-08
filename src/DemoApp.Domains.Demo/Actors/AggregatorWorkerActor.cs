using Akka.Actor;
using DemoApp.Domains.Demo.Messages;

namespace DemoApp.Domains.Demo.Actors;

public sealed class AggregatorWorkerActor : ReceiveActor
{
    public AggregatorWorkerActor()
    {
        ReceiveAsync<AggregatorMessageEvents.AggregateResultRequest>(DoWork);
    }

    private Task DoWork(AggregatorMessageEvents.AggregateResultRequest aggregateResult)
    {
        // can be more elaborate, but this illustrates the point nicely

        // get the result, since we are working with some Task based stuff, we use .PipeTo here instead of .Tell
        return GetNow(aggregateResult.Id).PipeTo(Sender);
    }

    private static ValueTask<AggregatorMessageEvents.AggregateResultResponse> GetNow(string id)
    {
        var now = DateTime.UtcNow;
        return ValueTask.FromResult(new AggregatorMessageEvents.AggregateResultResponse(id, now));
    }
}