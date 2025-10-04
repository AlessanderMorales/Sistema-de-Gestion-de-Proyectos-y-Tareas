namespace Sistema_de_Gestion_de_Proyectos_y_Tareas.Models
{

    public class Usuario
    {
        public int Id { get; set; }
        public string PrimerNombre { get; set; } = string.Empty;
        public string? SegundoNombre { get; set; }
        public string? Apellidos { get; set; }
        public string Contraseña { get; set; } = string.Empty;
        public string? Rol { get; set; }
    }
}