using WorldRank.Application.Interfaces;
using WorldRank.Domain;
using Microsoft.AspNetCore.Mvc;

namespace WorldRank.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PlayersController : ControllerBase
    {

        private readonly IPlayerRepository _playerRepository;
        
        public PlayersController(IPlayerRepository playerRepository)
        {
            _playerRepository = playerRepository;
        }
        [HttpGet]
        public IActionResult GetAll()
        {
            try
            {
                var result = _playerRepository
                    .GetAllPlayers()
                    .ToList();

                if (result.Count == 0)
                    return NotFound();

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("{playerId:int}")]
        public IActionResult GetPlayerById(int playerId)
        {
            try
            {
                var result = _playerRepository.FindPlayer(playerId);
                if (result == null) return NotFound();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
