using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ServiceComentario.Application.Service;
using ServiceComentario.Domain.Entities;

namespace Sistema_de_Gestion_de_Proyectos_y_Tareas.Pages.Comentarios
{
    [Authorize]
    public class EditModel : PageModel
    {
        private readonly ComentarioService _comentarioService;

        [BindProperty]
        public Comentario Comentario { get; set; }

        public EditModel(ComentarioService comentarioService)
        {
            _comentarioService = comentarioService;
        }

        public void OnGet(int id)
        {
            Comentario = _comentarioService.GetById(id);
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
                return Page();

            // ? Aplicar trim automático
            if (!string.IsNullOrEmpty(Comentario.Contenido))
            {
                Comentario.Contenido = TrimExtraSpaces(Comentario.Contenido);
            }

            _comentarioService.Update(Comentario);
            TempData["SuccessMessage"] = "Comentario actualizado exitosamente.";
            return RedirectToPage("Index");
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
