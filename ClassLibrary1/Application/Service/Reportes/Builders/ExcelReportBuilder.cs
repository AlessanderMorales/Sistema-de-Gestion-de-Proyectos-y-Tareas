using ClosedXML.Excel;
using ServiceProyecto.Domain.Entities;
using ServiceTarea.Application.Service;
using ServiceUsuario.Application.Service;
using ServiceProyecto.Application.Service.Reportes.Builders;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ServiceProyecto.Application.Service.Reportes.Builders
{
    public class ExcelReportBuilder : IReportBuilder
    {
        private readonly TareaService _tareaService;
        private readonly UsuarioService _usuarioService;
        private readonly List<Proyecto> _proyectos = new();
        private string _usuarioNombre = "Sistema";
        private byte[] _result;

        public ExcelReportBuilder(TareaService tareaService, UsuarioService usuarioService)
        {
            _tareaService = tareaService;
            _usuarioService = usuarioService;
        }

        public void Start(string usuarioNombre)
        {
            _usuarioNombre = usuarioNombre ?? "Sistema";
            _proyectos.Clear();
            _result = null;
        }

        public void AddProject(Proyecto proyecto)
        {
            if (proyecto != null) _proyectos.Add(proyecto);
        }

        public void Finish()
        {
            using var wb = new XLWorkbook();
            var wsProy = wb.Worksheets.Add("Proyectos");
            var wsTareas = wb.Worksheets.Add("Tareas");

            // Cabeceras Proyectos
            var proyHeaders = new[] { "ID", "Nombre", "Descripcion", "FechaInicio", "FechaFin", "Estado", "TotalTareas", "EmpleadosAsignados" };
            for (int i = 0; i < proyHeaders.Length; i++) wsProy.Cell(1, i + 1).Value = proyHeaders[i];
            wsProy.Row(1).Style.Font.Bold = true;

            // Cabeceras Tareas
            var tareaHeaders = new[] { "ProyectoID", "ProyectoNombre", "TareaID", "Titulo", "Prioridad", "Estado", "EmpleadosAsignados" };
            for (int i = 0; i < tareaHeaders.Length; i++) wsTareas.Cell(1, i + 1).Value = tareaHeaders[i];
            wsTareas.Row(1).Style.Font.Bold = true;

            int pRow = 2;
            int tRow = 2;

            foreach (var proyecto in _proyectos)
            {
                // Asegurar tareas cargadas
                var tareas = proyecto.Tareas;
                if (tareas == null || !tareas.Any())
                {
                    var todas = _tareaService.ObtenerTodasLasTareas() ?? Enumerable.Empty<ServiceTarea.Domain.Entities.Tarea>();
                    tareas = todas.Where(t => t.IdProyecto == proyecto.Id).ToList();
                }

                // Empleados asignados al proyecto (lista combinada desde tareas)
                var empleados = new List<string>();
                foreach (var tarea in tareas)
                {
                    if (tarea.IdUsuarioAsignado.HasValue)
                    {
                        var u = _usuarioService.ObtenerUsuarioPorId(tarea.IdUsuarioAsignado.Value);
                        if (u != null) empleados.Add($"{u.Nombres} {u.PrimerApellido}".Trim());
                    }

                    var otrosIds = _tareaService.ObtenerIdsUsuariosAsignados(tarea.Id) ?? Enumerable.Empty<int>();
                    foreach (var id in otrosIds)
                    {
                        var u2 = _usuarioService.ObtenerUsuarioPorId(id);
                        if (u2 != null) empleados.Add($"{u2.Nombres} {u2.PrimerApellido}".Trim());
                    }
                }

                var empleadosDistinct = empleados.Distinct().ToList();
                var estadoProy = proyecto.Estado == 1 ? "Activo" : "Inactivo";

                // Rellenar fila proyecto
                wsProy.Cell(pRow, 1).Value = proyecto.Id;
                wsProy.Cell(pRow, 2).Value = proyecto.Nombre ?? "";
                wsProy.Cell(pRow, 3).Value = proyecto.Descripcion ?? "";
                wsProy.Cell(pRow, 4).Value = proyecto.FechaInicio.ToString("dd/MM/yyyy");
                wsProy.Cell(pRow, 5).Value = proyecto.FechaFin.ToString("dd/MM/yyyy");
                wsProy.Cell(pRow, 6).Value = estadoProy;
                wsProy.Cell(pRow, 7).Value = tareas?.Count() ?? 0;
                wsProy.Cell(pRow, 8).Value = empleadosDistinct.Any() ? string.Join(", ", empleadosDistinct) : "*Sin Empleados Asignados*";

                pRow++;

                // Rellenar filas de tareas
                foreach (var tarea in tareas)
                {
                    // Empleados por tarea
                    var empleadosT = new List<string>();
                    if (tarea.IdUsuarioAsignado.HasValue)
                    {
                        var u = _usuarioService.ObtenerUsuarioPorId(tarea.IdUsuarioAsignado.Value);
                        if (u != null) empleadosT.Add($"{u.Nombres} {u.PrimerApellido}".Trim());
                    }
                    var otros = _tareaService.ObtenerIdsUsuariosAsignados(tarea.Id) ?? Enumerable.Empty<int>();
                    foreach (var id in otros)
                    {
                        if (tarea.IdUsuarioAsignado.HasValue && id == tarea.IdUsuarioAsignado.Value) continue;
                        var u2 = _usuarioService.ObtenerUsuarioPorId(id);
                        if (u2 != null) empleadosT.Add($"{u2.Nombres} {u2.PrimerApellido}".Trim());
                    }

                    var estado = MapStatusToFriendly(tarea.Status, tarea.Estado);

                    wsTareas.Cell(tRow, 1).Value = proyecto.Id;
                    wsTareas.Cell(tRow, 2).Value = proyecto.Nombre ?? "";
                    wsTareas.Cell(tRow, 3).Value = tarea.Id;
                    wsTareas.Cell(tRow, 4).Value = tarea.Titulo ?? "";
                    wsTareas.Cell(tRow, 5).Value = tarea.Prioridad ?? "";
                    wsTareas.Cell(tRow, 6).Value = estado;
                    wsTareas.Cell(tRow, 7).Value = empleadosT.Any() ? string.Join(", ", empleadosT.Distinct()) : "*Sin Empleados Asignados*";
                    tRow++;
                }
            }

            // Ajustes estéticos
            wsProy.Columns().AdjustToContents();
            wsTareas.Columns().AdjustToContents();

            // Footer metadata en una hoja pequeña
            var meta = wb.Worksheets.Add("Metadata");
            meta.Cell(1, 1).Value = "GeneradoPor";
            meta.Cell(1, 2).Value = _usuarioNombre;
            meta.Cell(2, 1).Value = "FechaGeneracion";
            meta.Cell(2, 2).Value = DateTime.Now.ToString("dd/MM/yyyy HH:mm");

            using var ms = new MemoryStream();
            wb.SaveAs(ms);
            _result = ms.ToArray();
        }

        public byte[] GetReport() => _result;

        private string MapStatusToFriendly(string status, int estado)
        {
            if (!string.IsNullOrWhiteSpace(status))
            {
                var s = status.Trim().ToLowerInvariant();
                if ((s.Contains("sin") && s.Contains("iniciar")) || s.Contains("sininiciar") || s == "sininiciar") return "Sin iniciar";
                if ((s.Contains("en") && s.Contains("progreso")) || s.Contains("enprogreso") || s.Contains("en progreso")) return "En progreso";
                if (s.Contains("complet") || s.Contains("finaliz") || s.Contains("done")) return "Completado";
                return char.ToUpper(status[0]) + status.Substring(1);
            }
            return estado == 1 ? "Activo" : "Inactivo";
        }
    }
}