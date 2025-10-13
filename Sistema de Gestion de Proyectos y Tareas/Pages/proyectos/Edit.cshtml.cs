using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Sistema_de_Gestion_de_Proyectos_y_Tareas.Domain.Entities;
using Sistema_de_Gestion_de_Proyectos_y_Tareas.Domain.Ports.Repositories;
using Sistema_de_Gestion_de_Proyectos_y_Tareas.Infrastructure.Persistence.Factories;

namespace Sistema_de_Gestion_de_Proyectos_y_Tareas.Pages.Proyectos
{
    public class EditModel : PageModel
    {
        private readonly MySqlRepositoryFactory<Proyecto> _repositoryFactory;

        public EditModel(MySqlRepositoryFactory<Proyecto> repositoryFactory)
        {
            _repositoryFactory = repositoryFactory;
        }

        [BindProperty]
        public Proyecto Proyecto { get; set; } = default!;

        public IActionResult OnGet(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            IDB<Proyecto> repo = _repositoryFactory.CreateRepository();
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

            IDB<Proyecto> repo = _repositoryFactory.CreateRepository();
            repo.UpdateAsync(Proyecto); 

            return RedirectToPage("./Index");
        }
    }
}