using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Akka.Actor;
using Akka.Event;
using Akka.Routing;
using DemoApp.Domains.Demo.Messages;

namespace DemoApp.Domains.Demo.Actors;

/// <summary>
/// Will aggregate results using simplistic state and return when all results has been gathered
/// </summary>
public sealed class AggregatorMainActor : ReceiveActor
{
    private const int DefaultCapacity = 1024;

    private readonly Dictionary<string, AggregatorMessageEvents.AggregationState> _inProcess;
    private readonly ILoggingAdapter _logger;

    private readonly IActorRef _aggregatorWorkerActor;
    private readonly IActorRef _aggregatorFinalizerActor;

    public AggregatorMainActor()
    {
        _logger = Context.GetLogger();
        _inProcess = new Dictionary<string, AggregatorMessageEvents.AggregationState>(DefaultCapacity);

        var aggregatorWorkerProps = Props.Create<AggregatorWorkerActor>();
        _aggregatorWorkerActor = Context.ActorOf(aggregatorWorkerProps.WithRouter(new SmallestMailboxPool(4)));

        var aggregatorFinalizerProps = Props.Create<AggregatorFinalizerActor>();
        _aggregatorFinalizerActor = Context.ActorOf(aggregatorFinalizerProps.WithRouter(new SmallestMailboxPool(4)));

        Receive<AggregatorMessageEvents.AggregateRequest>(Aggregate);
        Receive<AggregatorMessageEvents.AggregateResultResponse>(Aggregate);
    }

    private void Aggregate(AggregatorMessageEvents.AggregateRequest request)
    {
        // set up state
        var id = Guid.NewGuid().ToString();
        var state = new AggregatorMessageEvents.AggregationState(
            Id: id,
            Count: request.Count,
            Results: new List<DateTime>(request.Count),
            Start: Stopwatch.GetTimestamp(),
            Origin: Sender);

        if (!_inProcess.TryAdd(id, state))
        {
            _logger.Error("Another aggregator has already been registered with this actor");
            return;
        }

        _logger.Info($"Aggregating {id}...");

        for (var i = 0; i < request.Count; ++i)
            _aggregatorWorkerActor.Tell(new AggregatorMessageEvents.AggregateResultRequest(id));
    }

    private void Aggregate(AggregatorMessageEvents.AggregateResultResponse resultResponse)
    {
        ref var state = ref CollectionsMarshal.GetValueRefOrNullRef(_inProcess, resultResponse.Id);
        if (Unsafe.IsNullRef(ref state))
        {
            _logger.Error("Unable to find matching aggregation state");
            return;
        }

        state.Results.Add(resultResponse.Result);

        if (state.Results.Count == state.Count)
        {
            var end = Stopwatch.GetElapsedTime(state.Start);
            _aggregatorFinalizerActor.Tell(new AggregatorMessageEvents.AggregateFinalize(state.Results, state.Origin));
            _inProcess.Remove(state.Id);
            _logger.Info($"State completed. time={end}, id={resultResponse.Id}");
        }
    }
}