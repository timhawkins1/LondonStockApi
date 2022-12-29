using LondonStockApi.Models;
using LondonStockApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace LondonStockApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TradeController : ControllerBase
{
    private readonly ITradeService _tradeService;

    public TradeController(ITradeService tradeService)
    {
        _tradeService = tradeService;
    }

    [HttpPost]
    public async Task<ActionResult<Trade>> Notify(Trade trade)
    {
        await _tradeService.CreateAsync(trade);
        return trade;
    }
}
