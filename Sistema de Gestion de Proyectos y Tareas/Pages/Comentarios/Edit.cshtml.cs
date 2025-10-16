using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Sistema_de_Gestion_de_Proyectos_y_Tareas.Application.Services;
using Sistema_de_Gestion_de_Proyectos_y_Tareas.Domain.Entities;

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

            _comentarioService.Update(Comentario);
            return RedirectToPage("Index");
        }
    }
}
