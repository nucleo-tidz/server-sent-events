using model.Events;
using service.Interfaces.Infra;
using System.Runtime.CompilerServices;
using System.Threading.Channels;

namespace infrastructure
{
    public sealed class InMemoryOrderEventStream : IOrderEventStream
    {
        private readonly Channel<OrderCreatedEvent> _channel = Channel.CreateUnbounded<OrderCreatedEvent>();

        public ValueTask PublishAsync(OrderCreatedEvent order, CancellationToken ct = default)
            => _channel.Writer.WriteAsync(order, ct);

        public async IAsyncEnumerable<OrderCreatedEvent> SubscribeAsync(
            bool replay = false,
            [EnumeratorCancellation] CancellationToken ct = default)
        {
            await foreach (var order in _channel.Reader.ReadAllAsync(ct))
            {
                yield return order;
            }
        }
    }
}



