using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Sistema_de_Gestion_de_Proyectos_y_Tareas.Factories;
using Sistema_de_Gestion_de_Proyectos_y_Tareas.Models;
using Sistema_de_Gestion_de_Proyectos_y_Tareas.Repository;

namespace Sistema_de_Gestion_de_Proyectos_y_Tareas.Pages.Proyectos
{
    public class EditModel : PageModel
    {
        private readonly ProyectoryRepositoryCreator _repositoryCreator;

        public EditModel(ProyectoryRepositoryCreator repositoryCreator)
        {
            _repositoryCreator = repositoryCreator;
        }

        [BindProperty]
        public Proyecto Proyecto { get; set; } = default!;

        // Maneja la solicitud GET para mostrar el formulario de edición (SÍNCRONA)
        public IActionResult OnGet(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            IDB<Proyecto> repo = _repositoryCreator.CreateRepository();
            var proyecto = repo.GetByIdAsync(id.Value); // Llamada síncrona

            if (proyecto == null)
            {
                return NotFound();
            }
            Proyecto = proyecto;
            return Page();
        }

        // OnPostAsync puede seguir siendo async si tu UpdateAsync es async o si quieres usar await para otras cosas
        public async Task<IActionResult> OnPostAsync() // Puedes cambiarlo a 'OnPost()' si todas las operaciones del repo son síncronas
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            IDB<Proyecto> repo = _repositoryCreator.CreateRepository();
            repo.UpdateAsync(Proyecto); // Llamada síncrona (si UpdateAsync en IDB es void)

            return RedirectToPage("./Index");
        }
    }
}