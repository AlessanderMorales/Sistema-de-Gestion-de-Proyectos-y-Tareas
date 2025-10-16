using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Sistema_de_Gestion_de_Proyectos_y_Tareas.Common.Services;
using Sistema_de_Gestion_de_Proyectos_y_Tareas.Domain.Entities;

namespace Sistema_de_Gestion_de_Proyectos_y_Tareas.Pages.Tareas
{
    [Authorize(Policy = "OnlyJefe")]
    public class AsignarModel : PageModel
    {
        private readonly TareaService _tareaService;
        private readonly UsuarioService _usuarioService;

        [BindProperty]
        public int TareaId { get; set; }

        [BindProperty]
        public int UsuarioId { get; set; }

        public SelectList UsuariosDisponibles { get; set; }

        public AsignarModel(TareaService tareaService, UsuarioService usuarioService)
        {
            _tareaService = tareaService;
            _usuarioService = usuarioService;
        }

        public void OnGet(int id)
        {
            TareaId = id;
            var usuarios = _usuarioService.ObtenerTodosLosUsuarios();
            UsuariosDisponibles = new SelectList(usuarios, "Id", "PrimerNombre");
        }

        public IActionResult OnPost()
        {
            if (UsuarioId <= 0 || TareaId <= 0) return Page();
            _tareaService.AsignarTareaAUsuario(TareaId, UsuarioId);
            return RedirectToPage("Index");
        }
    }
}
