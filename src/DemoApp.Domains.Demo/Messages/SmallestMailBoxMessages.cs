namespace DemoApp.Domains.Demo.Messages;

public static class SmallestMailBoxEvents
{
    public sealed record SmalledMailBoxMessage;
    public sealed record SmallesMailBoxResponse(DateTime Now);
}