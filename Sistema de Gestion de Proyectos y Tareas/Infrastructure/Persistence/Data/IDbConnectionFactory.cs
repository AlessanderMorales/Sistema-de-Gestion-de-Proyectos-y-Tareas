using System.Data;

namespace Sistema_de_Gestion_de_Proyectos_y_Tareas.Infrastructure.Persistence.Data
{
    public interface IDbConnectionFactory
    {
        IDbConnection CreateConnection();
    }
}