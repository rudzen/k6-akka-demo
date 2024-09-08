using Akka.Actor;

namespace DemoApp.Domains.Demo.Messages;

public static class AggregatorMessageEvents
{
    /// <summary>
    /// The state for the aggregation
    /// </summary>
    /// <param name="Id">Unique ID</param>
    /// <param name="Count">How many results before we are done</param>
    /// <param name="Results">The results themselves</param>
    /// <param name="Origin">The original sender actor where the original request came from</param>
    public sealed record AggregationState(string Id, int Count, List<DateTime> Results, long Start, ICanTell Origin);

    /// <summary>
    /// When we want to compute a result, we send the unique id with it
    /// </summary>
    /// <param name="Id"></param>
    public sealed record AggregateResultRequest(string Id);

    /// <summary>
    /// When we have the result, we make sure to send back the original id so we know where the result belongs
    /// </summary>
    /// <param name="Id">The aggregation ID</param>
    /// <param name="Result">The result we were after</param>
    public sealed record AggregateResultResponse(string Id, DateTime Result);

    /// <summary>
    /// The main entry point into the aggregation actors, contains how many results we are interested in
    /// </summary>
    /// <param name="Count">The number of results we want</param>
    public sealed record AggregateRequest(int Count);

    /// <summary>
    /// The actual response given back to the sender from the aggregation actor
    /// </summary>
    /// <param name="ResultsFormatted">The result themselves</param>
    public sealed record AggregateResponse(string[] ResultsFormatted);

    /// <summary>
    /// We need to do some "final" processing of the result before sending it back to origin
    /// </summary>
    /// <param name="Results">The results we need to process</param>
    /// <param name="Origin">The origin actor waiting for the final result</param>
    public sealed record AggregateFinalize(IReadOnlyList<DateTime> Results, ICanTell Origin);
}