using Sistema_de_Gestion_de_Proyectos_y_Tareas.Models;
using Sistema_de_Gestion_de_Proyectos_y_Tareas.Repository; 

namespace Sistema_de_Gestion_de_Proyectos_y_Tareas.Factories
{
    public interface IRepositoryFactory
    {
        IDB<Proyecto> CreateProyectoRepository();
        IDB<Usuario> CreateUsuarioRepository();
        IDB<Tarea> CreateTareaRepository();
    }
}