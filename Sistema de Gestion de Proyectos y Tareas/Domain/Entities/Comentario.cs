using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Sistema_de_Gestion_de_Proyectos_y_Tareas.Domain.Entities
{
    public class Comentario
    {
        public int Id { get; set; }
        public string Contenido { get; set; }
        public DateTime Fecha { get; set; }
        public int Estado { get; set; }

        public int IdTarea { get; set; }
        public int IdUsuario { get; set; }

        [ValidateNever]
        public Usuario Usuario { get; set; }
    }
}
