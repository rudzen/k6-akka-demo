using System.Runtime.CompilerServices;
using Akka.Actor;
using DemoApp.Domains.Demo.Messages;

namespace DemoApp.Domains.Demo.Actors;

public sealed class TunnelActor : ReceiveActor
{
    private enum DelayDirection
    {
        Increase,
        Decrease
    }

    private long _delay;
    private DelayDirection _delayDirection;

    public TunnelActor()
    {
        _delay = 25;
        _delayDirection = DelayDirection.Decrease;
        Receive<TunnelEvents.TunnelMessage>(TunnelMessage);
        ReceiveAsync<TunnelEvents.TunnelDelayMessage>(TunnelDelayMessage);
    }

    private void TunnelMessage(TunnelEvents.TunnelMessage _)
    {
        Sender.Tell(new TunnelEvents.TunnelMessageResponse(DateTime.UtcNow));
    }

    private Task TunnelDelayMessage(TunnelEvents.TunnelDelayMessage _)
    {
        return UpdateDelay().PipeTo(Sender);
    }

    private async Task<TunnelEvents.TunnelDelayMessageResponse> UpdateDelay()
    {
        if (_delayDirection == DelayDirection.Decrease)
            DecrementDelay();
        else
            IncrementDelay();

        await Task.Delay(TimeSpan.FromMilliseconds(_delay));
        return new TunnelEvents.TunnelDelayMessageResponse(DateTime.UtcNow);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void DecrementDelay()
    {
        _delay--;
        if (_delay == 0)
            _delayDirection = DelayDirection.Increase;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void IncrementDelay()
    {
        _delay++;
        if (_delay == 1000)
            _delayDirection = DelayDirection.Decrease;
    }
}