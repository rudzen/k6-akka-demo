namespace DemoApp.Domains.Demo.Messages;

public static class TunnelEvents
{
    public sealed record TunnelMessage;
    public sealed record TunnelDelayMessage;
    public sealed record TunnelMessageResponse(DateTime Now);
    public sealed record TunnelDelayMessageResponse(DateTime Now);
}