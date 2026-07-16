using MediatR;

namespace WorldRank.Application.Commands.Players
{
    public record CreatePlayerCommand(string Name, int Score) : IRequest<int>;
}
