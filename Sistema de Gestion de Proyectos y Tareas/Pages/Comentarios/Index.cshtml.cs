using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Sistema_de_Gestion_de_Proyectos_y_Tareas.Application.Services;
using Sistema_de_Gestion_de_Proyectos_y_Tareas.Domain.Entities;
using System.Collections.Generic;

namespace Sistema_de_Gestion_de_Proyectos_y_Tareas.Pages.Comentarios
{
    [Authorize]
    public class IndexModel : PageModel
    {

        private readonly ComentarioService _comentarioService;

        public IndexModel(ComentarioService comentarioService)
        {
            _comentarioService = comentarioService;
        }

        public IEnumerable<Comentario> Comentarios { get; set; } = new List<Comentario>();

        public void OnGet()
        {
            Comentarios = _comentarioService.GetAll();
        }

        public IActionResult OnPostDelete(int id)
        {
            _comentarioService.Delete(id);
            return RedirectToPage("/Comentarios/Index");
        }
    }
}
