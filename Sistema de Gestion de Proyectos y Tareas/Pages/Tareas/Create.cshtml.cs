using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;

namespace Sistema_de_Gestion_de_Proyectos_y_Tareas.Pages.Tareas
{
    public class CreateModel : PageModel
    {
        private readonly IConfiguration _configuration;

        [BindProperty]
        public TareaInfo Tarea { get; set; } = new TareaInfo();
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
            if (string.IsNullOrEmpty(Tarea.Titulo) || string.IsNullOrEmpty(Tarea.Prioridad))
            {
                ErrorMessage = "El título y la prioridad son campos obligatorios.";
                return Page();
            }

            try
            {
                string connectionString = _configuration.GetConnectionString("MySqlConecction")!;
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    string query = @"INSERT INTO Tareas (titulo, descripcion, prioridad) 
                                     VALUES (@titulo, @descripcion, @prioridad);";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@titulo", Tarea.Titulo);
                        command.Parameters.AddWithValue("@descripcion", string.IsNullOrEmpty(Tarea.Descripcion) ? (object)DBNull.Value : Tarea.Descripcion);
                        command.Parameters.AddWithValue("@prioridad", Tarea.Prioridad);

                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                return Page();
            }

            return RedirectToPage("/Tareas/Index");
        }
    }

    public class TareaInfo
    {
        public string? Titulo { get; set; }
        public string? Descripcion { get; set; }
        public string? Prioridad { get; set; }
    }
}