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

        public IActionResult OnGet(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            IDB<Proyecto> repo = _repositoryCreator.CreateRepository();
            var proyecto = repo.GetByIdAsync(id.Value);

            if (proyecto == null)
            {
                return NotFound();
            }
            Proyecto = proyecto;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync() 
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            IDB<Proyecto> repo = _repositoryCreator.CreateRepository();
            repo.UpdateAsync(Proyecto); 

            return RedirectToPage("./Index");
        }
    }
}