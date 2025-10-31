using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ServiceProyecto.Application.Service;
using ServiceProyecto.Domain.Entities;

namespace Sistema_de_Gestion_de_Proyectos_y_Tareas.Pages.Proyectos
{
    [Authorize]
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

            // ? Aplicar trim automático a campos de texto
            if (!string.IsNullOrEmpty(Proyecto.Nombre))
            {
                Proyecto.Nombre = TrimExtraSpaces(Proyecto.Nombre);
            }

            if (!string.IsNullOrEmpty(Proyecto.Descripcion))
            {
                Proyecto.Descripcion = TrimExtraSpaces(Proyecto.Descripcion);
            }
            
            _proyectoService.ActualizarProyecto(Proyecto);

            TempData["SuccessMessage"] = "Proyecto actualizado correctamente.";
            return RedirectToPage("./Index");
        }

        /// <summary>
        /// Elimina espacios al inicio, al final y múltiples espacios consecutivos en el medio
        /// </summary>
        private string TrimExtraSpaces(string input)
        {
            if (string.IsNullOrEmpty(input)) return input;
            
            // Eliminar espacios al inicio y al final
            input = input.Trim();
     
            // Reemplazar múltiples espacios consecutivos por uno solo
            return System.Text.RegularExpressions.Regex.Replace(input, @"\s+", " ");
        }
    }
}