using MediatR;
using WorldRank.Domain;

namespace WorldRank.Application.Queries.Players
{
    public record GetAllPlayersQuery() : IRequest<IReadOnlyList<Player>>;
}
