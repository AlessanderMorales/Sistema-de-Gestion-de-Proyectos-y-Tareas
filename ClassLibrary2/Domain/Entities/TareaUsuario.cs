using System;

namespace ServiceTarea.Domain.Entities
{
    /// <summary>
    /// Representa la relación muchos-a-muchos entre Tareas y Usuarios
    /// </summary>
    public class TareaUsuario
    {
        public int Id { get; set; }
        public int IdTarea { get; set; }
        public int IdUsuario { get; set; }
        public DateTime FechaAsignacion { get; set; }
        public int Estado { get; set; }
        
        // Propiedad de navegación
        public Tarea Tarea { get; set; }
    }
}
