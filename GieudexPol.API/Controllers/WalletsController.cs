using GieudexPol.Application.Interfaces;
using GieudexPol.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GieudexPol.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WalletsController : ControllerBase
    {
        private readonly IWalletService _walletService;

        public WalletsController(IWalletService walletService)
        {
            _walletService = walletService;
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetUserWallets(int userId)
        {
            var wallets = await _walletService.GetUserWalletsAsync(userId);
            if (wallets == null)
            {
                return NotFound();
            }
            return Ok(wallets);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetWalletById(int id)
        {
            var wallet = await _walletService.GetByIdAsync(id);
            if (wallet == null)
            {
                return NotFound();
            }
            return Ok(wallet);
        }

        [HttpPost]
        public async Task<IActionResult> CreateWallet([FromBody] Wallet wallet)
        {
            await _walletService.AddAsync(wallet);
            return CreatedAtAction(nameof(GetWalletById), new { id = wallet.Id }, wallet);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateWallet(int id, [FromBody] Wallet wallet)
        {
            if (id != wallet.Id)
            {
                return BadRequest();
            }
            await _walletService.UpdateAsync(wallet);
            return NoContent();
        }

        /// <summary>
        /// Executes a trade transaction by debiting the source wallet and crediting the destination wallet.
        /// </summary>
        [HttpPost("trade")]
        public async Task<IActionResult> ExecuteTrade([FromQuery] int userId, [FromBody] TradeRequest request)
        {
            try
            {
                await _walletService.ExecuteTradeTransactionAsync(
                    userId, 
                    request.FromCurrencyId, 
                    request.AmountFrom, 
                    request.ToCurrencyId, 
                    request.AmountTo
                );
                return Ok("Trade executed successfully.");
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("Niewystarczające środki"))
            {
                return BadRequest(new { error = "Transaction failed", message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Internal Server Error", message = ex.Message });
            }
        }
    }

    // Ta klasa musi być tutaj, aby kontroler widział strukturę przesyłanego obiektu JSON
    public class TradeRequest
    {
        public int FromCurrencyId { get; set; }
        public decimal AmountFrom { get; set; }
        public int ToCurrencyId { get; set; }
        public decimal AmountTo { get; set; }
    }
}
