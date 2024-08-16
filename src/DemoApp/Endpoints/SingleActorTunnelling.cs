using Akka.Actor;
using Akka.Hosting;
using Akka.Routing;
using DemoApp.Domains.Demo.Actors;

namespace DemoApp.Endpoints;

public static class SingleActorTunnelling
{
    public static AkkaConfigurationBuilder RegisterAkkaActors(this AkkaConfigurationBuilder builder)
    {
        return builder
               .WithActors((system, registry, _) =>
               {
                   var props = Props.Create<TunnelActor>();
                   var actor = system.ActorOf(props, "tunnel-actor");
                   registry.Register<TunnelActor>(actor);
               })
               .WithActors((system, registry, _) =>
               {
                   var props = Props.Create<SmallestMailBox>().WithRouter(new SmallestMailboxPool(16));
                   var actor = system.ActorOf(props, "router-actor");
                   registry.Register<SmallestMailBox>(actor);
               });
    }
}