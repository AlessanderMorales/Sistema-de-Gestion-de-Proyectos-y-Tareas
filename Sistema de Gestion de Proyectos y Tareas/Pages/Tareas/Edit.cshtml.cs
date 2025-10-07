using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Sistema_de_Gestion_de_Proyectos_y_Tareas.Factories;
using Sistema_de_Gestion_de_Proyectos_y_Tareas.Models;
using Sistema_de_Gestion_de_Proyectos_y_Tareas.Repository;

namespace Sistema_de_Gestion_de_Proyectos_y_Tareas.Pages.Tareas
{
    public class EditModel : PageModel
    {
        private readonly TareaRepositoryCreator _repositoryCreator;

        public EditModel(TareaRepositoryCreator repositoryCreator)
        {
            _repositoryCreator = repositoryCreator;
        }

        [BindProperty]
        public Tarea Tarea { get; set; } = default!;

        public IActionResult OnGet(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Busca la Tarea por ID
            IDB<Tarea> repo = _repositoryCreator.CreateRepository();
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

            // Actualiza la Tarea
            IDB<Tarea> repo = _repositoryCreator.CreateRepository();
            repo.UpdateAsync(Tarea);

            return RedirectToPage("./Index");
        }
    }
}