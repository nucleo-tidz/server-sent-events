using model;

namespace service.Interfaces.Services
{
    public interface IOrderService
    {
        Task CreateOrderAsync(OrderModel order);
    }
}
