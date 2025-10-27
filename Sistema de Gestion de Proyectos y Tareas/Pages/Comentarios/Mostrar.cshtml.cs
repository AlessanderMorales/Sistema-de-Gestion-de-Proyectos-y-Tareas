using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using ServiceComentario.Application.Service;
using ServiceComentario.Domain.Entities;

namespace Sistema_de_Gestion_de_Proyectos_y_Tareas.Pages.Comentarios
{
    [Authorize]
    public class MostrarModel : PageModel
    {
        private readonly ComentarioService _comentarioService;

        public Comentario Comentario { get; set; }

        public MostrarModel(ComentarioService comentarioService)
        {
            _comentarioService = comentarioService;
        }

        public void OnGet(int id)
        {
            Comentario = _comentarioService.GetById(id);
        }
    }
}
