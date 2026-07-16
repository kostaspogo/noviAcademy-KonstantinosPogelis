using Microsoft.Extensions.DependencyInjection;
using WorldRank.Gateway.Ecb;

namespace WorldRank.Gateway
{
    public static class GatewayServiceCollectionExtensions
    {
        public static IServiceCollection AddEcbGateway(this IServiceCollection services)
        {
            services.AddHttpClient<IEcbRatesClient, EcbHttpClient>(client =>
            {
                client.BaseAddress = new Uri("https://www.ecb.europa.eu");
            });

            return services;
        }
    }
}
