using model;
using model.Events;
using service.Interfaces.Infra;
using service.Interfaces.Services;

namespace service
{
    internal class OrderService(IOrderEventStream eventStream) : IOrderService
    {
        public async Task CreateOrderAsync(OrderModel order)
        {
            var orderCreated = new OrderCreatedEvent(
                OrderId: Random.Shared.Next(1, 100_000),
                Product: order.Product,
                CreatedAt: DateTime.UtcNow
            );
            await eventStream.PublishAsync(orderCreated, CancellationToken.None);
            await Task.CompletedTask;
        }
    }
}
