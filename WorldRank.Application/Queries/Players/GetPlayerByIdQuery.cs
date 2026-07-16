using MediatR;
using WorldRank.Domain;

namespace WorldRank.Application.Queries.Players
{
    public record GetPlayerByIdQuery(int Id) : IRequest<Player?>;
}
