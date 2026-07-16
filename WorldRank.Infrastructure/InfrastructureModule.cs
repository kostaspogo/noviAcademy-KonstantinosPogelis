using Autofac;
using WorldRank.Application.Interfaces;
using WorldRank.Infrastructure.Reading;

namespace WorldRank.Infrastructure
{
    public class InfrastructureModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<PlayerReader>().As<IPlayerReader>().InstancePerLifetimeScope();

            builder.RegisterDecorator<CachingPlayerReaderDecorator, IPlayerReader>();
        }
    }
}
