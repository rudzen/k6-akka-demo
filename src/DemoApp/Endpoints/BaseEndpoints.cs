using System.Text;
using Akka.Actor;
using Akka.Hosting;
using DemoApp.Domains.Demo.Actors;
using DemoApp.Domains.Demo.Messages;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace DemoApp.Endpoints;

public static class BaseEndpoints
{
    private sealed record DateTimeResponse(DateTime DateTime);

    public static void MapDateReturnEndpoints(this IEndpointRouteBuilder app)
    {
        var dateTimeFetch = app.MapGroup("/simple/date-time");

        dateTimeFetch.MapGet("/", () =>
                     {
                         var now = DateTime.UtcNow;
                         return Task.FromResult<Results<Ok<DateTimeResponse>, NotFound>>(TypedResults.Ok(new DateTimeResponse(now)));
                     })
                     .WithName("BaseTimeStampReturn")
                     .WithOpenApi();

        // aggregate
        dateTimeFetch.MapGet("/aggregate/{count}", async (int count, [FromServices] IRequiredActor<AggregatorMainActor> reqActor) =>
                     {
                         var actor = await reqActor.GetAsync();
                         var responseContent = await actor.Ask<AggregatorMessageEvents.AggregateResponse>(new AggregatorMessageEvents.AggregateRequest(count));
                         return TypedResults.Ok(responseContent);
                     })
                     .WithName("BaseTimeStampAggregation")
                     .WithOpenApi();

        dateTimeFetch.MapGet("/tunnel", async ([FromServices] IRequiredActor<TunnelActor> reqActor) =>
                     {
                         var actor = await reqActor.GetAsync();
                         var responseContent = await actor.Ask<TunnelEvents.TunnelMessageResponse>(new TunnelEvents.TunnelMessage());
                         return TypedResults.Ok(new DateTimeResponse(responseContent.Now));
                     })
                     .WithName("BaseTimeStampTunneling")
                     .WithOpenApi();

        dateTimeFetch.MapGet("/tunnel-delay", async ([FromServices] IRequiredActor<TunnelActor> reqActor) =>
                     {
                         var actor = await reqActor.GetAsync();
                         var responseContent = await actor.Ask<TunnelEvents.TunnelDelayMessageResponse>(new TunnelEvents.TunnelDelayMessage());
                         return TypedResults.Ok(new DateTimeResponse(responseContent.Now));
                     })
                     .WithName("BaseTimeStampTunnelingDelayed")
                     .WithOpenApi();

        dateTimeFetch.MapGet("/smallest-mail-box", async ([FromServices] IRequiredActor<SmallestMailBoxActor> reqActor) =>
                     {
                         var actor = await reqActor.GetAsync();
                         var responseContent = await actor.Ask<SmallestMailBoxEvents.SmallesMailBoxResponse>(new SmallestMailBoxEvents.SmalledMailBoxMessage());
                         return TypedResults.Ok(new DateTimeResponse(responseContent.Now));
                     })
                     .WithName("BaseTimeStampSmallestMailBox")
                     .WithOpenApi();
    }
}