using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ServiceComentario.Application.Service;
using ServiceComentario.Domain.Entities;
using ServiceTarea.Application.Service;
using ServiceTarea.Domain.Entities;
using ServiceUsuario.Application.Service;
using ServiceUsuario.Domain.Entities;
using System.Collections.Generic;
using System;

namespace Sistema_de_Gestion_de_Proyectos_y_Tareas.Pages.Comentarios
{
    [Authorize]
    public class CreateModel : PageModel
    {
        private readonly ComentarioService _comentarioService;
        private readonly TareaService _tareaService;
        private readonly UsuarioService _usuarioService;

        [BindProperty]
        public Comentario Comentario { get; set; }

        public IEnumerable<Tarea> Tareas { get; set; }
        public IEnumerable<Usuario> Usuarios { get; set; }

        public CreateModel(ComentarioService comentarioService, TareaService tareaService, UsuarioService usuarioService)
        {
            _comentarioService = comentarioService;
            _tareaService = tareaService;
            _usuarioService = usuarioService;
        }

        public void OnGet()
        {
            Tareas = _tareaService.ObtenerTodasLasTareas();
            Usuarios = _usuarioService.ObtenerTodosLosUsuarios();
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                Tareas = _tareaService.ObtenerTodasLasTareas();
                Usuarios = _usuarioService.ObtenerTodosLosUsuarios();
                return Page();
            }

            Comentario.Estado = 1;
            Comentario.Fecha = DateTime.Now;

            _comentarioService.Add(Comentario);

            return RedirectToPage("Index");
        }
    }
}
