using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Sistema_de_Gestion_de_Proyectos_y_Tareas.Models;

namespace Sistema_de_Gestion_de_Proyectos_y_Tareas.Pages.Proyectos
{
    public class CreateModel : PageModel
    {
        private readonly IDB<Proyecto> _proyectoRepository;
        [BindProperty]
        public Proyecto Proyecto { get; set; } = new();

        public CreateModel(IDB<Proyecto> proyectoRepository)
        {
            _proyectoRepository = proyectoRepository;
        }

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();
            await _proyectoRepository.AddAsync(Proyecto);
            return RedirectToPage("./Index");
        }
    }
}