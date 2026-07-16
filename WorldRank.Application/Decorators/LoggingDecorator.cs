using MediatR;
using Microsoft.Extensions.Logging;

namespace WorldRank.Application.Decorators
{
    public class LoggingDecorator<TRequest, TResult> : IRequestHandler<TRequest, TResult>
        where TRequest : IRequest<TResult>
    {
        private readonly IRequestHandler<TRequest, TResult> _inner;
        private readonly ILogger<LoggingDecorator<TRequest, TResult>> _logger;

        public LoggingDecorator(
            IRequestHandler<TRequest, TResult> inner,
            ILogger<LoggingDecorator<TRequest, TResult>> logger)
        {
            _inner = inner;
            _logger = logger;
        }

        public async Task<TResult> Handle(TRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Request started {Name}", typeof(TRequest).Name);
            var result = await _inner.Handle(request, cancellationToken);
            _logger.LogInformation("Request finished {Name}", typeof(TRequest).Name);
            return result;
        }
    }
}
