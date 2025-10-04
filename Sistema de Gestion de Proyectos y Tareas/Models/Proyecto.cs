using Microsoft.AspNetCore.Mvc.ModelBinding;
namespace Sistema_de_Gestion_de_Proyectos_y_Tareas.Models
{
    public class Proyecto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public byte Estado { get; set; }

        //@Nombre, @Descripcion, @FechaInicio, @FechaFin
    }
}