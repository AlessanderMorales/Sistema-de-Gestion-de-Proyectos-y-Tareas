using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using System.Data;

namespace Sistema_de_Gestion_de_Proyectos_y_Tareas.Pages.Tareas
{
    public class IndexModel : PageModel
    // Para solucionar CS0101, asegúrate de que solo exista UNA definición de IndexModel en el espacio de nombres 'Sistema_de_Gestion_de_Proyectos_y_Tareas.Pages.Tareas'.
    // Busca y elimina cualquier clase duplicada 'IndexModel' en este espacio de nombres.
    // Si tienes otra clase IndexModel en otro archivo (por ejemplo, Index.cshtml.cs o IndexModel.cs), elimina o renombra una de ellas.
    // No es necesario modificar el código aquí si esta es la única definición. Si tienes otra definición, por favor indícala o muéstrala para poder sugerir una corrección específica.
    
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