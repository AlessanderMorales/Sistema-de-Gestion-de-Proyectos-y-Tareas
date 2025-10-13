using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Sistema_de_Gestion_de_Proyectos_y_Tareas.Infrastructure.Persistence.Factories;
using Sistema_de_Gestion_de_Proyectos_y_Tareas.Models;
using Sistema_de_Gestion_de_Proyectos_y_Tareas.Repository;

namespace Sistema_de_Gestion_de_Proyectos_y_Tareas.Pages.Tareas
{
    public class EditModel : PageModel
    {
        private readonly MySqlRepositoryFactory<Tarea> _repositoryFactory;

        public EditModel(MySqlRepositoryFactory<Tarea> repositoryFactory)
        {
            _repositoryFactory = repositoryFactory;
        }

        [BindProperty]
        public Tarea Tarea { get; set; } = default!;

        public IActionResult OnGet(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            IDB<Tarea> repo = _repositoryFactory.CreateRepository();
            var tarea = repo.GetByIdAsync(id.Value);

            if (tarea == null)
            {
                return NotFound();
            }
            Tarea = tarea;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            IDB<Tarea> repo = _repositoryFactory.CreateRepository();
            repo.UpdateAsync(Tarea);

            return RedirectToPage("./Index");
        }
    }
}