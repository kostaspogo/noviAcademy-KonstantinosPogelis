using Microsoft.AspNetCore.Mvc;
using WorldRank.API.Dtos;
using WorldRank.Application.Interfaces;
using WorldRank.Domain;

namespace WorldRank.API.Controllers
{
    [ApiController]
    [Route("players")]
    public class PlayersController : ControllerBase
    {
        private readonly IPlayerService _players;

        public PlayersController(IPlayerService players)
        {
            _players = players;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreatePlayerRequest request, CancellationToken ct)
        {
            Player player;
            try
            {
                player = await _players.CreateAsync(request.Name, request.Score, ct);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }

            return CreatedAtAction(nameof(GetById), new { id = player.Id }, PlayerResponse.From(player));
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id, CancellationToken ct)
        {
            var player = await _players.GetByIdAsync(id, ct);
            return player is null ? NotFound() : Ok(PlayerResponse.From(player));
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken ct)
        {
            var players = await _players.GetAllAsync(ct);
            return Ok(players.Select(PlayerResponse.From));
        }
    }
}
