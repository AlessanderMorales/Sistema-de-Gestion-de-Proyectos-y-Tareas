using System;

namespace ServiceTarea.Domain.Entities
{
    public class TareaUsuario
    {
        public int Id { get; set; }
        public int IdTarea { get; set; }
        public int IdUsuario { get; set; }
        public DateTime FechaAsignacion { get; set; }
        public int Estado { get; set; }
        public Tarea Tarea { get; set; }
    }
}
