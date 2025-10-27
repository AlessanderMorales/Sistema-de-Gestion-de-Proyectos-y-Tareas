using ServiceCommon.Infrastructure.Persistence.Data;
using ServiceProyecto.Domain.Entities;
using ServiceProyecto.Infrastructure.Persistence.Repositories;
using ServiceCommon.Domain.Port.Repositories;

namespace ServiceProyecto.Infrastructure.Persistence.Factories
{
    public class ProyectoryRepositoryCreator(MySqlConnectionSingleton connectionFactory) : MySqlRepositoryFactory<Proyecto>(connectionFactory)
    {
        public override IDB<Proyecto> CreateRepository()
        {
            return new ProyectoRepository(connectionFactory);
        }

    }
}