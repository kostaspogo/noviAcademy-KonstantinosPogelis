using Microsoft.AspNetCore.Mvc;
using WorldRank.API.Dtos;
using WorldRank.Application.Interfaces;
using WorldRank.Domain;
using WorldRank.Domain.Exceptions;

namespace WorldRank.API.Controllers
{
    [ApiController]
    [Route("wallets")]
    public class WalletsController : ControllerBase
    {
        private readonly IWalletService _wallets;

        public WalletsController(IWalletService wallets)
        {
            _wallets = wallets;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateWalletRequest request, CancellationToken ct)
        {
            Wallet wallet;
            try
            {
                wallet = await _wallets.CreateAsync(request.PlayerId, request.Currency, request.Balance, ct);
            }
            catch (WalletException ex)
            {
                return BadRequest(new { error = ex.Message });
            }

            return CreatedAtAction(nameof(GetById), new { id = wallet.Id }, WalletResponse.From(wallet));
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id, CancellationToken ct)
        {
            var wallet = await _wallets.GetByIdAsync(id, ct);
            return wallet is null ? NotFound() : Ok(WalletResponse.From(wallet));
        }

        [HttpPost("{id:int}/deposit")]
        public async Task<IActionResult> Deposit(int id, [FromBody] DepositRequest request, CancellationToken ct)
        {
            try
            {
                var wallet = await _wallets.DepositAsync(id, request.Amount, ct);
                return wallet is null ? NotFound() : Ok(WalletResponse.From(wallet));
            }
            catch (WalletException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}
