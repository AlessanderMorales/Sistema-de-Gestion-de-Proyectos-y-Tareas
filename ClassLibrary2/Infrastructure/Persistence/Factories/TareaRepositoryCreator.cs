using ServiceCommon.Infrastructure.Persistence.Data;
using ServiceCommon.Domain.Port.Repositories;
using ServiceTarea.Domain.Entities;
using ServiceTarea.Infrastructure.Persistence.Repositories;

namespace ServiceTarea.Infrastructure.Persistence.Factories
{
    public class TareaRepositoryCreator : MySqlRepositoryFactory<Tarea>
    {
        public TareaRepositoryCreator(MySqlConnectionSingleton connectionFactory) : base(connectionFactory) { }
        public override IDB<Tarea> CreateRepository()
        {
            return new TareaRepository(this._connectionSingleton);
        }
    }
}