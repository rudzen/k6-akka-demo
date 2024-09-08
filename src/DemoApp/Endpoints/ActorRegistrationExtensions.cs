using Akka.Actor;
using Akka.Hosting;
using Akka.Routing;
using DemoApp.Domains.Demo.Actors;

namespace DemoApp.Endpoints;

public static class ActorRegistrationExtensions
{
    public static void RegisterAkkaActors(this AkkaConfigurationBuilder builder)
    {
        builder
            .WithActors((system, registry, _) =>
            {
                var props = Props.Create<TunnelActor>();
                var actor = system.ActorOf(props, "tunnel-actor");
                registry.Register<TunnelActor>(actor);
            })
            .WithActors((system, registry, _) =>
            {
                var props = Props.Create<AggregatorMainActor>();
                var actor = system.ActorOf(props, "aggregator-actor");
                registry.Register<AggregatorMainActor>(actor);
            })
            .WithActors((system, registry, _) =>
            {
                var props = Props.Create<SmallestMailBoxActor>().WithRouter(new SmallestMailboxPool(16));
                var actor = system.ActorOf(props, "router-actor");
                registry.Register<SmallestMailBoxActor>(actor);
            });
    }
}