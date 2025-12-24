using model.Events;

namespace service.Interfaces.Infra
{
    public interface IOrderEventStream
    {
        ValueTask PublishAsync(OrderCreatedEvent order, CancellationToken ct = default);
        IAsyncEnumerable<OrderCreatedEvent> SubscribeAsync(bool Replay = false, CancellationToken ct = default);
    }
}
