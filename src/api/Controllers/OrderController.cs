using api.Requests;
using Microsoft.AspNetCore.Mvc;
using model.Events;
using service.Interfaces.Infra;
using service.Interfaces.Services;
using System.Runtime.CompilerServices;

namespace api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController(IOrderService orderService, IOrderEventStream eventStream) : ControllerBase
    {
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Create(CreateOrderRequest request, CancellationToken ct)
        {
            await orderService.CreateOrderAsync(request.ToModel());
            return Created();
        }

        [HttpGet("stream")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IAsyncEnumerable<OrderCreatedEvent>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IResult Stream(CancellationToken ct)
        {
            async IAsyncEnumerable<OrderCreatedEvent> StreamOrders([EnumeratorCancellation] CancellationToken cancellationToken)
            {
                await foreach (var order in eventStream.SubscribeAsync(false, cancellationToken))
                {
                    yield return order;
                }
            }
            return TypedResults.ServerSentEvents(StreamOrders(ct), eventType: "order-created");
        }
    }
}
