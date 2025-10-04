using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using Sistema_de_Gestion_de_Proyectos_y_Tareas.Factories;
using Sistema_de_Gestion_de_Proyectos_y_Tareas.Models;

namespace Sistema_de_Gestion_de_Proyectos_y_Tareas.Pages.Usuarios
{
    public class CreateModel : PageModel
    {
        private readonly IConfiguration _configuration;

        [BindProperty]
        public UsuarioInfo Usuario { get; set; } = new UsuarioInfo();
        public string ErrorMessage { get; set; } = "";

        public CreateModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void OnGet()
        {
            using Microsoft.AspNetCore.Mvc;
            using Microsoft.AspNetCore.Mvc.RazorPages;
            using Sistema_de_Gestion_de_Proyectos_y_Tareas.Factories;
            using Sistema_de_Gestion_de_Proyectos_y_Tareas.Models;

namespace Sistema_de_Gestion_de_Proyectos_y_Tareas.Pages.Usuarios
    {
        public class CreateModel : PageModel
        {
            private readonly IRepositoryFactory _factory;
            [BindProperty]
            public Usuario Usuario { get; set; } = new();

            public CreateModel(IRepositoryFactory factory) => _factory = factory;

            public void OnGet() { }

            public async Task<IActionResult> OnPostAsync()
            {
                if (!ModelState.IsValid) return Page();
                var repo = _factory.CreateUsuarioRepository();
                await repo.AddAsync(Usuario);
                return RedirectToPage("./Index");
            }
        }
    }
}

        public IActionResult OnPost()
        {
            if (string.IsNullOrEmpty(Usuario.PrimerNombre) ||
                string.IsNullOrEmpty(Usuario.Apellidos) ||
                string.IsNullOrEmpty(Usuario.Contraseña) ||
                string.IsNullOrEmpty(Usuario.Rol))
            {
                ErrorMessage = "Primer nombre, apellidos, contraseña y rol son campos obligatorios.";
                return Page();
            }

            

            try
            {
                string connectionString = _configuration.GetConnectionString("MySqlConecction")!;
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    string query = @"INSERT INTO Usuario (primer_nombre, segundo_nombre, apellidos, contraseña, rol) 
                                     VALUES (@primer_nombre, @segundo_nombre, @apellidos, @contraseña, @rol);";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@primer_nombre", Usuario.PrimerNombre);
                        command.Parameters.AddWithValue("@segundo_nombre", string.IsNullOrEmpty(Usuario.SegundoNombre) ? (object)DBNull.Value : Usuario.SegundoNombre);
                        command.Parameters.AddWithValue("@apellidos", Usuario.Apellidos);
                        command.Parameters.AddWithValue("@contraseña", Usuario.Contraseña); 
                        command.Parameters.AddWithValue("@rol", Usuario.Rol);

                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                return Page();
            }

            return RedirectToPage("/Usuarios/Index");
        }
    }

    public class UsuarioInfo
    {
        public string? PrimerNombre { get; set; }
        public string? SegundoNombre { get; set; }
        public string? Apellidos { get; set; }
        public string? Contraseña { get; set; }
        public string? Rol { get; set; }
    }
}