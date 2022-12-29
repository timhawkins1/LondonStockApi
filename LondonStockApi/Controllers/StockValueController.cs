using LondonStockApi.Models;
using LondonStockApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace LondonStockApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class StockValueController : ControllerBase
{
    private readonly IStockValueService _stockValueService;

    public StockValueController(IStockValueService stockValueService)
    {
        _stockValueService = stockValueService;
    }

    [HttpGet("getAll")]
    public async Task<ActionResult<IEnumerable<StockValue>>> GetAll()
    {
        return await _stockValueService.GetAllAsync();
    }

    [HttpGet("getOne/{ticker}")]
    public async Task<ActionResult<StockValue>> GetOne(string ticker)
    {
        return await _stockValueService.GetOneAsync(ticker);
    }

    [HttpPost("getRange")]
    public async Task<ActionResult<IEnumerable<StockValue>>> GetRange([FromBody]IEnumerable<string> tickers)
    {
        return await _stockValueService.GetRangeAsync(tickers);
    }
}

