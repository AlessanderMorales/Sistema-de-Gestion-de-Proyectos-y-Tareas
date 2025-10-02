using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using System.Data;

namespace Sistema_de_Gestion_de_Proyectos_y_Tareas.Pages.Tareas
{
    public class IndexModel : PageModel
    {
        public DataTable TareasTable { get; set; } = new DataTable();
        private readonly IConfiguration _configuration;

        public IndexModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void OnGet()
        {
            SelectTareas();
        }

        private void SelectTareas()
        {
            string connectionString = _configuration.GetConnectionString("MySqlConecction")!;
            string query = @"SELECT id_tarea, titulo, descripcion, prioridad, fechaRegistro 
                             FROM Tareas
                             WHERE estado = 1
                             ORDER BY prioridad, titulo";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                MySqlCommand command = new MySqlCommand(query, connection);
                connection.Open();
                MySqlDataAdapter adapter = new MySqlDataAdapter(command);
                adapter.Fill(TareasTable);
            }
        }
    }
}