using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using OrderApi.Intefaces;
using OrderApi.Shared.OrderDtos;

namespace OrderApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class OrderController : ControllerBase {
    private readonly IOrderService _orderService;


    public OrderController(IOrderService orderService) {
        _orderService = orderService;
    }


    /*    [HttpGet("{customerId:int}")]
        public async Task<IActionResult> GetOrders(int customerId, [FromQuery] OrderParameters orderParameters) {
            var pagedResult = await _orderService.GetOrderAsync(customerId, orderParameters);


            Response.Headers.Add("X-Pagination",
                JsonSerializer.Serialize(pagedResult.metaData));
            return Ok(pagedResult.ordersDto);
        }*/


    [HttpGet("{orderId:int}", Name = "OrderById")]
    public async Task<IActionResult> GetOrder(int orderId) {
        var orders = await _orderService.GetOrderByIdAsync(orderId);

        return Ok(orders);
    }


    [HttpPut("{orderId:int}")]
    public async Task<IActionResult> PutOrder(int orderId, [FromBody] UpdateOrderDto order) {
        await _orderService.UpdateOrderAsync(orderId, order);

        return NoContent();
    }

    [HttpDelete("{orderId:int}")]
    public async Task<IActionResult> DeleteOrder(int orderId) {
        await _orderService.DeleteOrderAsync(orderId);

        return NoContent();
    }

    [HttpPatch("{orderId:int}")]
    public async Task<IActionResult> PatchOrder(int orderId, [FromBody] JsonPatchDocument<UpdateOrderDto> patchDoc) {
        if(patchDoc is null) return BadRequest("patchDoc object sent from client is null.");

        var result = await _orderService.PatchOrderAsync(orderId);

        patchDoc.ApplyTo(result.orderDto);

        await _orderService.SaveChangesForPatchAsync(result.orderDto, result.order);

        return NoContent();
    }
}