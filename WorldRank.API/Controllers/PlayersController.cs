using MediatR;
using Microsoft.AspNetCore.Mvc;
using WorldRank.API.Dtos;
using WorldRank.Application.Commands.Players;
using WorldRank.Application.Queries.Players;

namespace WorldRank.API.Controllers
{
    [ApiController]
    [Route("players")]
    public class PlayersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PlayersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreatePlayerRequest request, CancellationToken ct)
        {
            int id;
            try
            {
                id = await _mediator.Send(new CreatePlayerCommand(request.Name, request.Score), ct);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }

            return CreatedAtAction(nameof(GetById), new { id }, null);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id, CancellationToken ct)
        {
            var player = await _mediator.Send(new GetPlayerByIdQuery(id), ct);
            return player is null ? NotFound() : Ok(PlayerResponse.From(player));
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken ct)
        {
            var players = await _mediator.Send(new GetAllPlayersQuery(), ct);
            return Ok(players.Select(PlayerResponse.From));
        }
    }
}
