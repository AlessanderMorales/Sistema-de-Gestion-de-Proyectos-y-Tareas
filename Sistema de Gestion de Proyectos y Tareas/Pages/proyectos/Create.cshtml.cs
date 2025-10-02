using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;

namespace Sistema_de_Gestion_de_Proyectos_y_Tareas.Pages.Proyectos
{
    public class CreateModel : PageModel
    {
        private readonly IConfiguration _configuration;

        [BindProperty]
        public ProyectoInfo Proyecto { get; set; } = new ProyectoInfo();

        public string ErrorMessage { get; set; } = "";

        public CreateModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            if (string.IsNullOrEmpty(Proyecto.Nombre) || Proyecto.FechaInicio == default || Proyecto.FechaFin == default)
            {
                ErrorMessage = "El nombre, la fecha de inicio y la fecha de fin son obligatorios.";
                return Page(); 
            }

            if (Proyecto.FechaFin < Proyecto.FechaInicio)
            {
                ErrorMessage = "La fecha de fin no puede ser anterior a la fecha de inicio.";
                return Page();
            }

            try
            {
                string connectionString = _configuration.GetConnectionString("MySqlConecction")!;
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    string query = @"INSERT INTO Proyecto (nombre, descripcion, fecha_inicio, fecha_fin) 
                                     VALUES (@nombre, @descripcion, @fecha_inicio, @fecha_fin);";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@nombre", Proyecto.Nombre);
                        command.Parameters.AddWithValue("@descripcion", string.IsNullOrEmpty(Proyecto.Descripcion) ? (object)DBNull.Value : Proyecto.Descripcion);
                        command.Parameters.AddWithValue("@fecha_inicio", Proyecto.FechaInicio);
                        command.Parameters.AddWithValue("@fecha_fin", Proyecto.FechaFin);

                        command.ExecuteNonQuery(); 
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                return Page(); 
            }

            return RedirectToPage("/Proyectos/Index");
        }
    }

    public class ProyectoInfo
    {
        public string? Nombre { get; set; }
        public string? Descripcion { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
    }
}