using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using TestApi.AzureSQL.Models;
using TestApi.CosmosDB.Models;

namespace TestApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class OrderController : ControllerBase {
    private readonly CosmosContext _cosmosContext;
    private readonly AzureSQLContext _azureSQLContext;

    public OrderController(CosmosContext cosmosContext, AzureSQLContext azureSQLContext) {
        _cosmosContext = cosmosContext;
        _azureSQLContext = azureSQLContext;
    }
    //bnechamrking and  stockwatch
    [HttpGet("cosmosdb/{categoryId}")]
    public IActionResult GetCosmosDB(int categoryId) {
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();
        var customers = _cosmosContext.Product.AsNoTracking().Where(c => c.ProductCategoryID == categoryId).Take(50);
        stopwatch.Stop();
        Console.WriteLine("CosmosDB runtime: " + stopwatch.ElapsedMilliseconds + " ms");
        return Ok(customers);
    }

    [HttpGet("azuresql/{categoryId}")]
    public IActionResult GetAzureSQL(int categoryId) {
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();
        var customers = _azureSQLContext.Product.AsNoTracking().Where(c => c.ProductCategoryId == categoryId).Take(50);
        stopwatch.Stop();
        Console.WriteLine("Azure SQL runtime: " + stopwatch.ElapsedMilliseconds + " ms");
        return Ok(customers);
    }
}
