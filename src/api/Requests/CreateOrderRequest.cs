using model;

namespace api.Requests
{
    public class CreateOrderRequest
    {    
        public string Product { get; set; }
    }

    public static class CreateOrderRequestExtensions
    {
        public static OrderModel ToModel(this CreateOrderRequest request)
        {
            return new OrderModel
            {
                Product = request.Product
            };
        }
    }
}
