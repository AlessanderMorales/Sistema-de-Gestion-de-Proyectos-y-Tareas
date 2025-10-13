// Archivo: Infrastructure/Persistence/Factories/ProyectoryRepositoryCreator.cs

// --- AÑADE ESTOS 'USINGS' ---
// Para que sepa qué es 'Proyecto'
using Sistema_de_Gestion_de_Proyectos_y_Tareas.Domain.Entities;
using Sistema_de_Gestion_de_Proyectos_y_Tareas.Domain.Ports.Repositories;
using Sistema_de_Gestion_de_Proyectos_y_Tareas.Infrastructure.Persistence.Repositories;
using Sistema_de_Gestion_de_Proyectos_y_Tareas.Infrastructure.Persistence.Data;


namespace Sistema_de_Gestion_de_Proyectos_y_Tareas.Infrastructure.Persistence.Factories
{
    public class ProyectoryRepositoryCreator(MySqlConnectionSingleton connectionFactory) : MySqlRepositoryFactory<Proyecto>(connectionFactory)
    {
        public override IDB<Proyecto> CreateRepository()
        {
            return new ProyectoRepository(connectionFactory);
        }
        // Elimina o comenta la siguiente línea, ya que el espacio de nombres 'Sistema_de_Gestion_de_Proyectos_y_Tareas.Repository' no existe y no se utiliza en el archivo.
        // using Sistema_de_Gestion_de_Proyectos_y_Tareas.Repository;
    }
}