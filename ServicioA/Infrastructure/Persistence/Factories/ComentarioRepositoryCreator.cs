using ServiceCommon.Domain.Port.Repositories;
using ServiceCommon.Infrastructure.Persistence.Data;
using ServiceComentario.Domain.Entities;
using ServiceComentario.Infrastructure.Persistence.Repositories;

namespace ServiceComentario.Infrastructure.Persistence.Factories
{
    public class ComentarioRepositoryCreator(MySqlConnectionSingleton connectionFactory)
        : MySqlRepositoryFactory<Comentario>(connectionFactory)
    {
        public override IDB<Comentario> CreateRepository()
        {
            return new ComentarioRepository(connectionFactory);
        }
    }
}
