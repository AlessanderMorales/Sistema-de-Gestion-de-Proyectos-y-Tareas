// Archivo: Pages/Proyectos/Edit.cshtml.cs (Versión Corregida)

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Sistema_de_Gestion_de_Proyectos_y_Tareas.Domain.Entities;
using Sistema_de_Gestion_de_Proyectos_y_Tareas.Application.Services;

namespace Sistema_de_Gestion_de_Proyectos_y_Tareas.Pages.Proyectos
{
    public class EditModel : PageModel
    {
        private readonly ProyectoService _proyectoService;

        [BindProperty]
        public Proyecto Proyecto { get; set; } = default!;
        public EditModel(ProyectoService proyectoService)
        {
            _proyectoService = proyectoService;
        }

        public IActionResult OnGet(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var proyecto = _proyectoService.ObtenerProyectoPorId(id.Value);

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
            _proyectoService.ActualizarProyecto(Proyecto);

            return RedirectToPage("./Index");
        }
    }
}