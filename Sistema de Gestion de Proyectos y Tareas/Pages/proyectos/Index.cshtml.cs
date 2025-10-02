using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using System.Data;

namespace Sistema_de_Gestion_de_Proyectos_y_Tareas.Pages.Proyectos
{
    public class IndexModel : PageModel
    {
        public DataTable ProyectosTable { get; set; } = new DataTable();

        private readonly IConfiguration _configuration;

        public IndexModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void OnGet()
        {
            SelectProyectos();
        }

        private void SelectProyectos()
        {
            string connectionString = _configuration.GetConnectionString("MySqlConecction")!;

            string query = @"SELECT id_proyecto, nombre, descripcion, fecha_inicio, fecha_fin 
                             FROM Proyecto
                             WHERE estado = 1
                             ORDER BY nombre";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                MySqlCommand command = new MySqlCommand(query, connection);
                connection.Open();

                MySqlDataAdapter adapter = new MySqlDataAdapter(command);
                adapter.Fill(ProyectosTable);
            }
        }
    }
}