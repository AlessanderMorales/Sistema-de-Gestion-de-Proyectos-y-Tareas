using ClosedXML.Excel;
using ServiceProyecto.Application.Service;
using ServiceProyecto.Domain.Entities;
using ServiceTarea.Application.Service;
using ServiceTarea.Domain.Entities;
using ServiceUsuario.Application.Service;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;

namespace ServiceProyecto.Application.Service.Reportes
{
    // Clase que se encarga de generar los reportes en formato Excel (XLSX)
    public class ReporteExcelService
    {
        private readonly ProyectoService _proyectoService;
        private readonly TareaService _tareaService;
        private readonly UsuarioService _usuarioService;

        public ReporteExcelService(ProyectoService proyectoService, TareaService tareaService, UsuarioService usuarioService)
        {
            _proyectoService = proyectoService;
            _tarea_service: _ = tareaService; // no-op to avoid accidental old references
            _tareaService = tareaService;
            _usuarioService = usuarioService;
        }

        // ---------------------------------------------
        // Método principal para el Reporte General de Proyectos en Excel (ClosedXML)
        // ---------------------------------------------
        public byte[] GenerarReporteGeneralProyectosExcel()
        {
            var proyectos = _proyectoService.ObtenerTodosLosProyectos()?.ToList() ?? new List<Proyecto>();

            // Asegurar que cada proyecto tenga sus tareas cargadas
            var proyectosConTareas = new List<Proyecto>();
            foreach (var p in proyectos)
            {
                var pConTareas = _proyectoService.ObtenerProyectoConTareas(p.Id) ?? p;

                if (pConTareas.Tareas == null || !pConTareas.Tareas.Any())
                {
                    // Fallback para cargar tareas si no se cargaron con el proyecto
                    var todas = _tareaService.ObtenerTodasLasTareas() ?? Enumerable.Empty<Tarea>();
                    pConTareas.Tareas = todas.Where(t => t.IdProyecto == pConTareas.Id).ToList();
                }

                proyectosConTareas.Add(pConTareas);
            }

            // Llamada al método que realmente construye el archivo Excel
            return CrearExcel(proyectosConTareas);
        }

        // ---------------------------------------------
        // Método que construye el archivo XLSX usando EPPlus
        // ---------------------------------------------
        private byte[] CrearExcel(IEnumerable<Proyecto> proyectos)
        {
            using var ms = new MemoryStream();
            using var workbook = new XLWorkbook();

            CrearHojaResumenProyectos(workbook, proyectos);
            CrearHojaDetalleTareas(workbook, proyectos);

            workbook.SaveAs(ms);
            return ms.ToArray();
        }

        // ---------------------------------------------
        // Hoja 1: Resumen de Proyectos
        // ---------------------------------------------
        private void CrearHojaResumenProyectos(XLWorkbook workbook, IEnumerable<Proyecto> proyectos)
        {
            var ws = workbook.Worksheets.Add("Resumen Proyectos");

            // Encabezados
            var headers = new List<string> {
                "ID Proyecto", "Nombre", "Descripción", "Fecha Inicio", "Fecha Fin", "Estado", "Total Tareas", "Tareas Completadas"
            };

            for (int i = 0; i < headers.Count; i++)
            {
                ws.Cell(1, i + 1).Value = headers[i];
            }

            var headerRange = ws.Range(1, 1, 1, headers.Count);
            headerRange.Style.Font.SetBold();
            headerRange.Style.Fill.SetBackgroundColor(XLColor.FromColor(Color.FromArgb(200, 200, 200)));

            // Datos
            int row = 2;
            foreach (var proyecto in proyectos)
            {
                int totalTareas = proyecto.Tareas?.Count ?? 0;
                int tareasCompletadas = proyecto.Tareas?.Count(t => MapStatusToFriendly(t.Status, t.Estado) == "Completado") ?? 0;

                ws.Cell(row, 1).Value = proyecto.Id;
                ws.Cell(row, 2).Value = proyecto.Nombre;
                ws.Cell(row, 3).Value = proyecto.Descripcion;
                ws.Cell(row, 4).Value = proyecto.FechaInicio;
                ws.Cell(row, 5).Value = proyecto.FechaFin;
                ws.Cell(row, 6).Value = proyecto.Estado == 1 ? "Activo" : "Inactivo";
                ws.Cell(row, 7).Value = totalTareas;
                ws.Cell(row, 8).Value = tareasCompletadas;

                row++;
            }

            ws.Column(4).Style.DateFormat.SetFormat("dd/MM/yyyy");
            ws.Column(5).Style.DateFormat.SetFormat("dd/MM/yyyy");

            ws.Columns().AdjustToContents();
        }

        // ---------------------------------------------
        // Hoja 2: Detalle de Tareas con Asignaciones
        // ---------------------------------------------
        private void CrearHojaDetalleTareas(XLWorkbook workbook, IEnumerable<Proyecto> proyectos)
        {
            var ws = workbook.Worksheets.Add("Detalle Tareas");

            // Encabezados
            var headers = new List<string> {
                "ID Tarea", "Proyecto", "Título Tarea", "Prioridad", "Estado", "Usuarios Asignados", "Fecha Creación", "ID Proyecto"
            };

            for (int i = 0; i < headers.Count; i++)
            {
                ws.Cell(1, i + 1).Value = headers[i];
            }

            var headerRange = ws.Range(1, 1, 1, headers.Count);
            headerRange.Style.Font.SetBold();
            headerRange.Style.Fill.SetBackgroundColor(XLColor.FromColor(Color.FromArgb(179, 217, 255)));

            // Datos
            int row = 2;
            foreach (var proyecto in proyectos)
            {
                if (proyecto.Tareas == null) continue;

                foreach (var tarea in proyecto.Tareas)
                {
                    // 1. Obtener la lista de usuarios
                    var usuariosNombres = ObtenerNombresUsuarios(tarea);
                    var usuariosStr = usuariosNombres.Any() ? string.Join(", ", usuariosNombres.Distinct()) : "*Sin Asignar*";

                    // 2. Obtener el estado amigable y su color
                    var estadoTexto = MapStatusToFriendly(tarea.Status, tarea.Estado);
                    var color = GetStatusColor(estadoTexto);

                    ws.Cell(row, 1).Value = tarea.Id;
                    ws.Cell(row, 2).Value = proyecto.Nombre;
                    ws.Cell(row, 3).Value = tarea.Titulo;
                    ws.Cell(row, 4).Value = tarea.Prioridad ?? "N/A";
                    ws.Cell(row, 5).Value = estadoTexto;
                    ws.Cell(row, 6).Value = usuariosStr;
                    ws.Cell(row, 8).Value = proyecto.Id;

                    // 4. Aplicar color de fondo a la celda de Estado
                    if (color.HasValue)
                    {
                        ws.Cell(row, 5).Style.Fill.SetBackgroundColor(XLColor.FromColor(color.Value));
                    }

                    row++;
                }
            }

            ws.Column(7).Style.DateFormat.SetFormat("dd/MM/yyyy");
            ws.Columns().AdjustToContents();
        }

        // ---------------------------------------------
        // Métodos de ayuda
        // ---------------------------------------------

        // Reutilización del mapeo de estado
        private string MapStatusToFriendly(string status, int estado)
        {
            if (!string.IsNullOrWhiteSpace(status))
            {
                var s = status.Trim().ToLowerInvariant();
                if ((s.Contains("sin") && s.Contains("iniciar")) || s.Contains("sininiciar") || s == "sininiciar") return "Sin iniciar";
                if ((s.Contains("en") && s.Contains("progreso")) || s.Contains("enprogreso") || s.Contains("en progreso")) return "En progreso";
                if (s.Contains("complet") || s.Contains("finaliz") || s.Contains("done")) return "Completado";
                // default: capitalize first letter
                return char.ToUpper(status[0]) + status.Substring(1);
            }

            // fallback by numeric Estado
            return estado == 1 ? "Activo" : "Inactivo";
        }

        // Modificado para usar System.Drawing.Color (EPPlus) en lugar de iText.Kernel.Colors.Color
        private Color? GetStatusColor(string friendlyStatus)
        {
            if (string.IsNullOrWhiteSpace(friendlyStatus)) return null;
            var s = friendlyStatus.ToLowerInvariant();

            if (s.Contains("sin")) return Color.FromArgb(235, 235, 235);
            if (s.Contains("en progreso") || s.Contains("enprogreso") || s.Contains("progreso")) return Color.FromArgb(255, 244, 179);
            if (s.Contains("complet") || s.Contains("finaliz") || s.Contains("done")) return Color.FromArgb(200, 230, 201);
            return null;
        }

        // Lógica de obtención de usuarios para una tarea
        private List<string> ObtenerNombresUsuarios(Tarea tarea)
        {
            var usuariosNombres = new List<string>();

            // Usuario asignado directamente en la tarea
            if (tarea.IdUsuarioAsignado.HasValue)
            {
                var usuario = _usuarioService.ObtenerUsuarioPorId(tarea.IdUsuarioAsignado.Value);
                if (usuario != null)
                {
                    usuariosNombres.Add(($"{usuario.Nombres} {usuario.PrimerApellido}").Trim());
                }
            }

            // Usuarios en tabla intermedia (TareaUsuario)
            var otrosIds = _tareaService.ObtenerIdsUsuariosAsignados(tarea.Id) ?? Enumerable.Empty<int>();
            foreach (var uid in otrosIds)
            {
                // Evitar duplicados
                if (tarea.IdUsuarioAsignado.HasValue && uid == tarea.IdUsuarioAsignado.Value) continue;

                var usuario = _usuarioService.ObtenerUsuarioPorId(uid);
                if (usuario != null)
                {
                    usuariosNombres.Add(($"{usuario.Nombres} {usuario.PrimerApellido}").Trim());
                }
            }

            return usuariosNombres.Distinct().ToList();
        }
    }
}