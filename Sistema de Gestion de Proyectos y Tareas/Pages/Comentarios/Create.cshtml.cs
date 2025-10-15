using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Sistema_de_Gestion_de_Proyectos_y_Tareas.Application.Services;
using Sistema_de_Gestion_de_Proyectos_y_Tareas.Domain.Entities;

namespace Sistema_de_Gestion_de_Proyectos_y_Tareas.Pages.Comentarios
{
    public class CreateModel : PageModel
    {
        private readonly ComentarioService _comentarioService;

        [BindProperty]
        public Comentario Comentario { get; set; }

        public CreateModel(ComentarioService comentarioService)
        {
            _comentarioService = comentarioService;
        }

        public void OnGet() { }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
                return Page();

            Comentario.Estado = 1;
            Comentario.Fecha = DateTime.Now;
            _comentarioService.Add(Comentario);

            return RedirectToPage("Index");
        }
    }
}
