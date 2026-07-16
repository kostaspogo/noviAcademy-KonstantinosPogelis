using MediatR;
using WorldRank.Application.Interfaces;
using WorldRank.Domain;

namespace WorldRank.Application.Queries.Players
{
    public class GetAllPlayersQueryHandler : IRequestHandler<GetAllPlayersQuery, IReadOnlyList<Player>>
    {
        private readonly IPlayerRepository _players;

        public GetAllPlayersQueryHandler(IPlayerRepository players)
        {
            _players = players;
        }

        public async Task<IReadOnlyList<Player>> Handle(GetAllPlayersQuery request, CancellationToken cancellationToken)
        {
            return await _players.GetAllPlayersAsync(cancellationToken);
        }
    }
}
