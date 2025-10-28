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
using System.Security.Claims;
using System.Linq;

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
        public int UsuarioActualId { get; set; }
        public Usuario JefeProyecto { get; set; }
        public int JefeProyectoId { get; set; }

        [TempData]
        public string? MensajeExito { get; set; }

        [TempData]
        public string? MensajeError { get; set; }

        [BindProperty]
        public int DirigidoAUsuarioId { get; set; }

        // Nueva propiedad para mapeo de tareas a usuarios
        public Dictionary<int, List<Usuario>> TareaUsuariosMap { get; set; }

        public CreateModel(ComentarioService comentarioService, TareaService tareaService, UsuarioService usuarioService)
        {
            _comentarioService = comentarioService;
            _tareaService = tareaService;
            _usuarioService = usuarioService;
        }

        public void OnGet()
        {
            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(idClaim, out var usuarioId))
            {
                UsuarioActualId = usuarioId;
            }

            if (User.IsInRole("Empleado"))
            {
                Tareas = _tareaService.ObtenerTareasPorUsuarioAsignado(UsuarioActualId);
            }
            else
            {
                Tareas = _tareaService.ObtenerTodasLasTareas();
            }

            // Obtener Jefe de Proyecto
            var todosLosUsuarios = _usuarioService.ObtenerTodosLosUsuarios().ToList();
            JefeProyecto = todosLosUsuarios.FirstOrDefault(u => u.Rol == "JefeDeProyecto");
            JefeProyectoId = JefeProyecto?.Id ?? 0;

            // ============================================
            // NUEVO: JEFE Y EMPLEADO USAN EL MISMO SISTEMA DINÁMICO
            // ============================================
            TareaUsuariosMap = new Dictionary<int, List<Usuario>>();

            foreach (var tarea in Tareas)
            {
                var usuariosEnTarea = new List<Usuario>();

                // Obtener IDs de usuarios asignados a esta tarea
                var idsUsuariosEnTarea = _tareaService.ObtenerIdsUsuariosAsignados(tarea.Id).ToList();

                // Filtrar usuarios: solo los asignados a la tarea, excluyendo al usuario actual y SuperAdmin
                var usuariosAsignados = todosLosUsuarios
                    .Where(u => idsUsuariosEnTarea.Contains(u.Id) &&
                                u.Id != UsuarioActualId && // No puede comentarse a sí mismo
                                u.Rol != "SuperAdmin") // No puede comentar al Admin
                    .ToList();

                usuariosEnTarea.AddRange(usuariosAsignados);

                TareaUsuariosMap[tarea.Id] = usuariosEnTarea;
            }

            // Establecer primer destinatario disponible si hay tareas
            if (Tareas.Any() && TareaUsuariosMap.ContainsKey(Tareas.First().Id))
            {
                var primerosUsuarios = TareaUsuariosMap[Tareas.First().Id];
                if (primerosUsuarios.Any())
                {
                    DirigidoAUsuarioId = primerosUsuarios.First().Id;
                }
            }
        }

        public IActionResult OnPost()
        {
            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(idClaim, out var usuarioId))
            {
                UsuarioActualId = usuarioId;
            }

            // SIEMPRE el autor es el usuario actual
            Comentario.IdUsuario = UsuarioActualId;

            // ============================================
            // VALIDACIONES DE NEGOCIO
            // ============================================

            // 1. Validar que DirigidoAUsuarioId esté presente
            if (DirigidoAUsuarioId <= 0)
            {
                TempData["ErrorMessage"] = "Debes seleccionar un destinatario para el comentario.";
                return RedirectToPage("Index");
            }

            // 2. Validar que no te comentes a ti mismo
            if (DirigidoAUsuarioId == UsuarioActualId)
            {
                TempData["ErrorMessage"] = "No puedes crear comentarios dirigidos a ti mismo.";
                return RedirectToPage("Index");
            }

            // 3. Validar que no se comente al Admin
            var usuarioDestinatario = _usuarioService.ObtenerUsuarioPorId(DirigidoAUsuarioId);
            if (usuarioDestinatario == null)
            {
                TempData["ErrorMessage"] = "El usuario destinatario no existe.";
                return RedirectToPage("Index");
            }

            if (usuarioDestinatario.Rol == "SuperAdmin")
            {
                TempData["ErrorMessage"] = "No se pueden crear comentarios para el administrador.";
                return RedirectToPage("Index");
            }

            // 4. Validar que la tarea exista
            var tarea = _tareaService.ObtenerTareaPorId(Comentario.IdTarea);
            if (tarea == null)
            {
                TempData["ErrorMessage"] = "La tarea seleccionada no existe.";
                return RedirectToPage("Index");
            }

            // ============================================
            // NUEVA LÓGICA: LOS COMENTARIOS SON INDEPENDIENTES
            // NO SE VINCULAN CON LA ASIGNACIÓN DE TAREAS
            // ============================================
            
            // Los comentarios son simplemente mensajes entre usuarios sobre una tarea
            // NO modifican quién tiene  asignada la tarea
            
            // El campo "DirigidoAUsuarioId" es solo informativo:
            // - Indica a quién va dirigido el mensaje
            // - NO cambia la asignación de la tarea
            
            // Por lo tanto, NO actualizamos tarea.IdUsuarioAsignado

            // Validar ModelState
             if (!ModelState.IsValid)
            {
                if (User.IsInRole("Empleado"))
                {
                    Tareas = _tareaService.ObtenerTareasPorUsuarioAsignado(UsuarioActualId);
                }
                else
                {
  Tareas = _tareaService.ObtenerTodasLasTareas();
                }

                var todosLosUsuarios = _usuarioService.ObtenerTodosLosUsuarios().ToList();
            JefeProyecto = todosLosUsuarios.FirstOrDefault(u => u.Rol == "JefeDeProyecto");
                JefeProyectoId = JefeProyecto?.Id ?? 0;
                Usuarios = todosLosUsuarios
                    .Where(u => u.Rol != "SuperAdmin" && u.Id != UsuarioActualId)
      .ToList();
                return Page();
            }

            // ============================================
            // GUARDAR COMENTARIO (SIN TOCAR LA TAREA)
            // ============================================

            try
            {
                Comentario.Estado = 1;
                Comentario.Fecha = DateTime.Now;
                _comentarioService.Add(Comentario);

                TempData["SuccessMessage"] = $"Comentario enviado a {usuarioDestinatario.Nombres} {usuarioDestinatario.PrimerApellido}.";
                return RedirectToPage("Index");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error al crear el comentario: {ex.Message}";
                return RedirectToPage("Index");
            }
        }
    }
}
