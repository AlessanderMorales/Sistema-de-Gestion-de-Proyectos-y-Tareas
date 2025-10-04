namespace Sistema_de_Gestion_de_Proyectos_y_Tareas.Models
{
    public class Tarea
    {
        public int Id { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public string? Prioridad { get; set; }
    }
}