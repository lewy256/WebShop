using MediatR;
using Microsoft.AspNetCore.Mvc;
using OrderApi.Commands;
using OrderApi.Queries;
using OrderApi.Shared.OrderDtos;

namespace OrderApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class OrderController : ControllerBase {
    private readonly ISender _sender;
    private readonly IPublisher _publisher;

    public OrderController(ISender sender, IPublisher publisher) {
        _sender = sender;
        _publisher = publisher;
    }


    /*  [HttpGet("{id:Guid}")]
      public async Task<IActionResult> GetOrders(int id, [FromQuery] OrderParameters orderParameters) {
          var pagedResult = await _sender.Send(new GetOrdersQuery());


          *//*   Response.Headers.Add("X-Pagination",
                 JsonSerializer.Serialize(pagedResult.metaData));*//*
          return Ok(pagedResult);
      }*/


    [HttpGet("{id:Guid}", Name = "OrderById")]
    public async Task<IActionResult> GetOrder(Guid id) {
        var order = await _sender.Send(new GetOrderQuery(id));

        return Ok(order);
    }


    [HttpPut("{id:Guid}")]
    public async Task<IActionResult> PutOrder(Guid id, [FromBody] UpdateOrderDto order) {

        if(order is null) {
            return BadRequest("UpdateOrderDto object is null");
        }

        await _sender.Send(new UpdateOrderCommand(id, order));

        return NoContent();
    }

    [HttpDelete("{id:Guid}")]
    public async Task<IActionResult> DeleteOrder(Guid id) {
        await _sender.Send(new DeleteOrderCommand(id));

        return NoContent();
    }

    /*    [HttpPatch("{id:Guid}")]
        public async Task<IActionResult> PatchOrder(Guid id, [FromBody] JsonPatchDocument<UpdateOrderDto> patchDoc) {
            if(patchDoc is null) return BadRequest("patchDoc object sent from client is null.");

            var result = await _sender.Send.PatchOrderAsync(orderId);

            patchDoc.ApplyTo(result.orderDto);

            await await _sender.Send.SaveChangesForPatchAsync(result.orderDto, result.order);

            return NoContent();
        }*/
}