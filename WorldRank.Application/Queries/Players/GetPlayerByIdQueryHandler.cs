using MediatR;
using WorldRank.Application.Interfaces;
using WorldRank.Domain;

namespace WorldRank.Application.Queries.Players
{
    public class GetPlayerByIdQueryHandler : IRequestHandler<GetPlayerByIdQuery, Player?>
    {
        private readonly IPlayerReader _reader;

        public GetPlayerByIdQueryHandler(IPlayerReader reader)
        {
            _reader = reader;
        }

        public async Task<Player?> Handle(GetPlayerByIdQuery request, CancellationToken cancellationToken)
        {
            return await _reader.GetByIdAsync(request.Id, cancellationToken);
        }
    }
}
