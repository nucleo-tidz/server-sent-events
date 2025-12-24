using model.Events;

namespace service.Interfaces.Infra
{
    public interface IOrderEventStream
    {
        ValueTask PublishAsync(OrderCreatedEvent order, CancellationToken ct = default);
        IAsyncEnumerable<OrderCreatedEvent> SubscribeAsync(CancellationToken ct = default);
    }
}
