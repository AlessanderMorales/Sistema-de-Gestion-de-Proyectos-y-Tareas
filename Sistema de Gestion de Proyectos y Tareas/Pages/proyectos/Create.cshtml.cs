using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Sistema_de_Gestion_de_Proyectos_y_Tareas.Domain.Entities;
using Sistema_de_Gestion_de_Proyectos_y_Tareas.Application.Services;
using Sistema_de_Gestion_de_Proyectos_y_Tareas.Helpers;

namespace Sistema_de_Gestion_de_Proyectos_y_Tareas.Pages.Proyectos
{
    public class CreateModel : PageModel
    {
        private readonly ProyectoService _proyectoService;
        [BindProperty]
        public Proyecto Proyecto { get; set; } = new();
        public CreateModel(ProyectoService proyectoService)
        {
            _proyectoService = proyectoService;
        }
        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            // normalize
            Proyecto.Nombre = InputSanitizer.NormalizeSpaces(Proyecto.Nombre) ?? Proyecto.Nombre;
            Proyecto.Descripcion = InputSanitizer.NormalizeSpaces(Proyecto.Descripcion);

            // date validations
            if (Proyecto.FechaFin <= Proyecto.FechaInicio)
            {
                ModelState.AddModelError("Proyecto.FechaFin", "La fecha de finalización debe ser posterior a la fecha de inicio.");
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }
            _proyectoService.CrearNuevoProyecto(Proyecto);

            return RedirectToPage("./Index");
        }
    }
}