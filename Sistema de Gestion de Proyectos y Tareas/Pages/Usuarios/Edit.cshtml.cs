using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Sistema_de_Gestion_de_Proyectos_y_Tareas.Models;
using Sistema_de_Gestion_de_Proyectos_y_Tareas.Infrastructure.Persistence.Factories;

namespace Sistema_de_Gestion_de_Proyectos_y_Tareas.Pages.Usuarios
{
    public class EditModel : PageModel
    {
        private readonly MySqlRepositoryFactory<Usuario> _repositoryFactory;
        [BindProperty]
        public Usuario Usuario { get; set; } = default!;

        public EditModel(MySqlRepositoryFactory<Usuario> repositoryFactory)
        {
            _repositoryFactory = repositoryFactory;
        }

        public IActionResult OnGet(int? id)
        {
            if (id == null) return NotFound();
            var repo = _repositoryFactory.CreateRepository();
            var usuario = repo.GetByIdAsync(id.Value);
            if (usuario == null) return NotFound();
            Usuario = usuario;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();
            var repo = _repositoryFactory.CreateRepository();
            repo.UpdateAsync(Usuario);
            return RedirectToPage("./Index");
        }
    }
}