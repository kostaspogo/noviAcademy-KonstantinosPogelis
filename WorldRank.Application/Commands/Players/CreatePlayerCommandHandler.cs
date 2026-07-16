using MediatR;
using WorldRank.Application.Interfaces;
using WorldRank.Domain;

namespace WorldRank.Application.Commands.Players
{
    public class CreatePlayerCommandHandler : IRequestHandler<CreatePlayerCommand, int>
    {
        private readonly IPlayerRepository _players;

        public CreatePlayerCommandHandler(IPlayerRepository players)
        {
            _players = players;
        }

        public async Task<int> Handle(CreatePlayerCommand request, CancellationToken cancellationToken)
        {
            var all = await _players.GetAllPlayersAsync(cancellationToken);
            var nextId = all.Count == 0 ? 1 : all.Max(p => p.Id) + 1;

            var player = new Player(nextId, request.Name);
            player.AddScore(request.Score);

            await _players.AddPlayerAsync(player, cancellationToken);
            return player.Id;
        }
    }
}
