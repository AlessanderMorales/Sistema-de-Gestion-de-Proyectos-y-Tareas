using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using System.Data;

namespace Sistema_de_Gestion_de_Proyectos_y_Tareas.Pages.Usuarios
{
    public class IndexModel : PageModel
    {
        public DataTable UsuariosTable { get; set; } = new DataTable();
        private readonly IConfiguration _configuration;

        public IndexModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void OnGet()
        {
            SelectUsuarios();
        }

        private void SelectUsuarios()
        {
            string connectionString = _configuration.GetConnectionString("MySqlConecction")!;
            string query = @"SELECT id_usuario, primer_nombre, segundo_nombre, apellidos, rol 
                             FROM Usuario
                             WHERE estado = 1
                             ORDER BY apellidos, primer_nombre";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                MySqlCommand command = new MySqlCommand(query, connection);
                connection.Open();
                MySqlDataAdapter adapter = new MySqlDataAdapter(command);
                adapter.Fill(UsuariosTable);
            }
        }
    }
}