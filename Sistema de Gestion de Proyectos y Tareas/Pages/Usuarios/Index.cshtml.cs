using Microsoft.AspNetCore.Mvc.RazorPages;
using Sistema_de_Gestion_de_Proyectos_y_Tareas.Factories;
using Sistema_de_Gestion_de_Proyectos_y_Tareas.Models;

namespace Sistema_de_Gestion_de_Proyectos_y_Tareas.Pages.Usuarios
{
    public class IndexModel : PageModel
    {
        private readonly IRepositoryFactory _factory;
        public IEnumerable<Usuario> Usuarios { get; private set; } = new List<Usuario>();

        public IndexModel(IRepositoryFactory factory) => _factory = factory;

        public async Task OnGetAsync()
        {
            var repo = _factory.CreateUsuarioRepository();
            Usuarios = await repo.GetAllAsync();
        }
    }
}