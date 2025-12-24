using model.Events;
using service.Interfaces.Infra;
using StackExchange.Redis;
using System.Runtime.CompilerServices;

namespace infrastructure
{
    public sealed class RedisOrderEventStream : IOrderEventStream
    {
        private const string StreamKey = "orders:stream";
        private readonly IDatabase _db;

        public RedisOrderEventStream(IConnectionMultiplexer redis)
        {
            _db = redis.GetDatabase();
        }
        public async ValueTask PublishAsync(OrderCreatedEvent order, CancellationToken ct = default)
        {
            await _db.StreamAddAsync(
                StreamKey,
                new NameValueEntry[]
                {
                    new("orderId", order.OrderId),
                    new("product", order.Product),
                    new("createdAt", order.CreatedAt.ToString("O"))
                },
                maxLength: 4,
                useApproximateMaxLength: true
            );
        }
        public async IAsyncEnumerable<OrderCreatedEvent> SubscribeAsync(
            bool replay = false, [EnumeratorCancellation] CancellationToken ct = default)
        {
            if (replay)
            {
                await foreach (var order in SubscribeWithReplayAsync(ct))
                {
                    yield return order;
                }
            }
            else
            {
                await foreach (var order in SubscribeWithoutReplayAsync(ct))
                {
                    yield return order;
                }
            }
        }
        private async IAsyncEnumerable<OrderCreatedEvent> SubscribeWithoutReplayAsync(
            [EnumeratorCancellation] CancellationToken ct = default)
        {
            RedisValue position;
            var latest = await _db.StreamRangeAsync(
                StreamKey,
                minId: "-",
                maxId: "+",
                count: 1,
                Order.Descending
            );

            position = latest.Length > 0 ? latest[0].Id : "0-0";
            while (!ct.IsCancellationRequested)
            {
                var entries = await _db.StreamReadAsync(
                    StreamKey,
                    position,
                    count: 10
                );

                foreach (var entry in entries)
                {
                    position = entry.Id;
                    yield return new OrderCreatedEvent(
                        OrderId: (int)entry["orderId"],
                        Product: entry["product"]!,
                        CreatedAt: DateTime.Parse(entry["createdAt"]!)
                    );
                }
                await Task.Delay(300, ct);
            }
        }
        private async IAsyncEnumerable<OrderCreatedEvent> SubscribeWithReplayAsync(
            [EnumeratorCancellation] CancellationToken ct = default)
        {
            var connectedAt = DateTime.UtcNow;
            RedisValue position = "0-0";
            while (!ct.IsCancellationRequested)
            {
                var entries = await _db.StreamReadAsync(
                    StreamKey,
                    position,
                    count: 10
                );
                foreach (var entry in entries)
                {
                    position = entry.Id;
                    var order = new OrderCreatedEvent(
                        OrderId: (int)entry["orderId"],
                        Product: entry["product"]!,
                        CreatedAt: DateTime.Parse(entry["createdAt"]!)
                    );
                    if (order.CreatedAt < connectedAt)
                        continue;
                    yield return order;
                }
                await Task.Delay(300, ct);
            }
        }
    }
}
