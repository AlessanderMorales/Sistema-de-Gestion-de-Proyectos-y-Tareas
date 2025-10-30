using iText.IO.Font.Constants;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Draw;
using iText.Layout;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Properties;
using ServiceProyecto.Application.Service;
using ServiceProyecto.Domain.Entities;
using ServiceTarea.Application.Service;
using ServiceTarea.Domain.Entities;
using ServiceUsuario.Application.Service;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Geom;
using System;

namespace ServiceProyecto.Application.Service.Reportes
{
    public class ReporteService
    {
        private readonly ProyectoService _proyectoService;
        private readonly TareaService _tareaService;
        private readonly UsuarioService _usuarioService;

        // ✅ Fuentes (una regular y una en negrita)
        private readonly PdfFont _fontRegular;
        private readonly PdfFont _fontBold;

        public ReporteService(ProyectoService proyectoService, TareaService tareaService, UsuarioService usuarioService)
        {
            _proyectoService = proyectoService;
            _tareaService = tareaService;
            _usuarioService = usuarioService;
            _fontRegular = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);
            _fontBold = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);
        }

        // -----------------------------
        // Método para 1 proyecto
        // -----------------------------
        public byte[] GenerarReporteProyectoPdf(int idProyecto, string usuarioNombre = "Sistema")
        {
            var proyecto = _proyectoService.ObtenerProyectoConTareas(idProyecto);

            if (proyecto == null) return null;

            // Asegurar que las tareas estén cargadas; si no, intentar obtenerlas desde TareaService
            if (proyecto.Tareas == null || !proyecto.Tareas.Any())
            {
                var todas = _tareaService.ObtenerTodasLasTareas() ?? Enumerable.Empty<Tarea>();
                var tareasFallback = todas.Where(t => t.IdProyecto == proyecto.Id).ToList();

                proyecto.Tareas = tareasFallback;
            }

            using var stream = new MemoryStream();
            using var writer = new PdfWriter(stream);
            using var pdf = new PdfDocument(writer);
            var document = new Document(pdf);

            // Título
            var titulo = new Paragraph($"REPORTE DE PROYECTO: {proyecto.Nombre}")
                .SetFont(_fontBold)
                .SetFontSize(18)
                .SetFontColor(ColorConstants.BLUE)
                .SetTextAlignment(TextAlignment.CENTER);
            document.Add(titulo);

            document.Add(new LineSeparator(new SolidLine()).SetMarginTop(5).SetMarginBottom(10));

            // Info principal
            document.Add(CrearSeccionInfoPrincipal(proyecto));

            // Tareas
            var subtitulo = new Paragraph("Tareas Asignadas")
                .SetFont(_fontBold)
                .SetFontSize(14)
                .SetMarginTop(20);
            document.Add(subtitulo);
            document.Add(CrearTablaTareas(proyecto));

            // Nota
            document.Add(new Paragraph("Integrantes: (Listado consultado desde UsuarioService según tareas asignadas)")
                .SetFont(_fontRegular)
                .SetFontSize(10)
                .SetFontColor(ColorConstants.GRAY));

            // Close document to finalize pages
            document.Close();

            // Post-process bytes to add footers safely
            var bytes = stream.ToArray();
            return AddFootersToPdfBytes(bytes, usuarioNombre);
        }

        // -----------------------------
        // Nuevo método: todos los proyectos (usa ProyectoService para obtenerlos)
        // -----------------------------
        public byte[] GenerarReporteGeneralProyectosPdf(string usuarioNombre = "Sistema")
        {
            var proyectos = _proyectoService.ObtenerTodosLosProyectos()?.ToList() ?? new List<Proyecto>();

            // Asegurarse de que cada proyecto tenga sus tareas cargadas
            var proyectosConTareas = new List<Proyecto>();
            foreach (var p in proyectos)
            {
                var pConTareas = _proyectoService.ObtenerProyectoConTareas(p.Id) ?? p;
                proyectosConTareas.Add(pConTareas);
            }

            return GenerarReporteGeneralProyectosPdf(proyectosConTareas, usuarioNombre);
        }

        // Mantengo el método existente que recibe proyectos (compatibilidad)
        public byte[] GenerarReporteGeneralProyectosPdf(IEnumerable<Proyecto> proyectos, string usuarioNombre = "Sistema")
        {
            using var stream = new MemoryStream();
            using var writer = new PdfWriter(stream);
            using var pdf = new PdfDocument(writer);
            var document = new Document(pdf);

            // Título general
            var tituloGeneral = new Paragraph("REPORTE GENERAL DE PROYECTOS")
                .SetFont(_fontBold)
                .SetFontSize(18)
                .SetFontColor(ColorConstants.BLUE)
                .SetTextAlignment(TextAlignment.CENTER);
            document.Add(tituloGeneral);

            document.Add(new LineSeparator(new SolidLine()).SetMarginTop(5).SetMarginBottom(10));

            foreach (var proyecto in proyectos)
            {
                // Asegurar que las tareas estén cargadas; intentar ObtenerProyectoConTareas y luego fallback a TareaService
                if (proyecto.Tareas == null || !proyecto.Tareas.Any())
                {
                    var pConTareas = _proyectoService.ObtenerProyectoConTareas(proyecto.Id);
                    if (pConTareas?.Tareas != null && pConTareas.Tareas.Any())
                    {
                        proyecto.Tareas = pConTareas.Tareas;
                    }
                    else
                    {
                        var todas = _tareaService.ObtenerTodasLasTareas() ?? Enumerable.Empty<Tarea>();
                        proyecto.Tareas = todas.Where(t => t.IdProyecto == proyecto.Id).ToList();
                    }
                }

                // Info principal
                document.Add(CrearSeccionInfoPrincipal(proyecto));

                // Tareas
                var subtitulo = new Paragraph("Tareas Asignadas")
                    .SetFont(_fontBold)
                    .SetFontSize(14)
                    .SetMarginTop(10);
                document.Add(subtitulo);
                document.Add(CrearTablaTareas(proyecto));

                // Nota
                document.Add(new Paragraph("Integrantes: (Listado consultado desde UsuarioService según tareas asignadas)")
                    .SetFont(_fontRegular)
                    .SetFontSize(10)
                    .SetFontColor(ColorConstants.GRAY));

                // Salto de página si no es el último proyecto
                if (proyecto != proyectos.Last())
                {
                    document.Add(new AreaBreak(AreaBreakType.NEXT_PAGE));
                }
            }

            // Close document to finalize pages
            document.Close();

            // Post-process bytes to add footers safely
            var generated = stream.ToArray();
            return AddFootersToPdfBytes(generated, usuarioNombre);
        }

        // -----------------------------
        // Métodos internos
        // -----------------------------
        private Table CrearSeccionInfoPrincipal(Proyecto proyecto)
        {
            var table = new Table(UnitValue.CreatePercentArray(new float[] { 1, 3 }))
                .UseAllAvailableWidth()
                .SetMarginBottom(15);

            // Use regular cells for label/value rows so headers don't accumulate and break layout
            table.AddCell(CrearCelda("ID Proyecto:", true));
            table.AddCell(CrearCelda(proyecto.Id.ToString()));

            table.AddCell(CrearCelda("Nombre:", true));
            table.AddCell(CrearCelda(proyecto.Nombre));

            table.AddCell(CrearCelda("Descripción:", true));
            table.AddCell(CrearCelda(proyecto.Descripcion ?? "N/A"));

            table.AddCell(CrearCelda("Inicia:", true));
            table.AddCell(CrearCelda(proyecto.FechaInicio.ToShortDateString()));

            table.AddCell(CrearCelda("Finaliza:", true));
            table.AddCell(CrearCelda(proyecto.FechaFin.ToShortDateString()));

            table.AddCell(CrearCelda("Estado:", true));
            table.AddCell(CrearCelda(proyecto.Estado == 1 ? "Activo" : "Inactivo"));

            return table;
        }

        // Ahora incluye columna "Usuarios" con los usuarios asignados a cada tarea (tanto IdUsuarioAsignado como asignaciones en TareaUsuario)
        private Table CrearTablaTareas(Proyecto proyecto)
        {
            var table = new Table(UnitValue.CreatePercentArray(new float[] { 1, 3, 1, 1, 3 }))
                .UseAllAvailableWidth()
                .SetBorder(new SolidBorder(ColorConstants.BLACK, 1));

            // Encabezados
            table.AddHeaderCell(CrearCelda("ID Tarea", true, ColorConstants.LIGHT_GRAY));
            table.AddHeaderCell(CrearCelda("Título", true, ColorConstants.LIGHT_GRAY));
            table.AddHeaderCell(CrearCelda("Prioridad", true, ColorConstants.LIGHT_GRAY));
            table.AddHeaderCell(CrearCelda("Estado", true, ColorConstants.LIGHT_GRAY));
            table.AddHeaderCell(CrearCelda("Usuarios", true, ColorConstants.LIGHT_GRAY));

            if (proyecto.Tareas == null || !proyecto.Tareas.Any())
            {
                table.AddCell(new Cell(1, 5)
                    .Add(new Paragraph("No hay tareas asignadas a este proyecto."))
                    .SetTextAlignment(TextAlignment.CENTER));
            }
            else
            {
                foreach (var tarea in proyecto.Tareas)
                {
                    table.AddCell(CrearCelda(tarea.Id.ToString()));
                    table.AddCell(CrearCelda(tarea.Titulo));
                    table.AddCell(CrearCelda(tarea.Prioridad?.ToString() ?? "N/A"));

                    // Use the human-readable Status string (e.g. "SinIniciar", "EnProgreso", etc.)
                    var estadoTexto = !string.IsNullOrWhiteSpace(tarea.Status) ? tarea.Status : (tarea.Estado.ToString());
                    table.AddCell(CrearCelda(estadoTexto));

                    // Construir lista de usuarios asociados a la tarea
                    var usuariosNombres = new List<string>();

                    // Usuario asignado directamente en la tarea
                    if (tarea.IdUsuarioAsignado.HasValue)
                    {
                        var usuario = _usuarioService.ObtenerUsuarioPorId(tarea.IdUsuarioAsignado.Value);
                        if (usuario != null)
                        {
                            var nombreCompleto = $"{usuario.Nombres} {usuario.PrimerApellido}".Trim();
                            usuariosNombres.Add(nombreCompleto);
                        }
                    }

                    // Usuarios en tabla intermedia (TareaUsuario)
                    var otrosIds = _tareaService.ObtenerIdsUsuariosAsignados(tarea.Id) ?? Enumerable.Empty<int>();
                    foreach (var uid in otrosIds)
                    {
                        // Evitar duplicados con el IdUsuarioAsignado
                        if (tarea.IdUsuarioAsignado.HasValue && uid == tarea.IdUsuarioAsignado.Value) continue;

                        var usuario = _usuarioService.ObtenerUsuarioPorId(uid);
                        if (usuario != null)
                        {
                            var nombreCompleto = $"{usuario.Nombres} {usuario.PrimerApellido}".Trim();
                            usuariosNombres.Add(nombreCompleto);
                        }
                    }

                    var usuariosStr = usuariosNombres.Any()
                        ? string.Join(", ", usuariosNombres.Distinct())
                        : "N/A";

                    table.AddCell(CrearCelda(usuariosStr));
                }
            }

            return table;
        }

        private Cell CrearCelda(string content, bool isHeader = false, iText.Kernel.Colors.Color? bgColor = null)
        {
            var paragraph = new Paragraph(content)
                .SetFont(isHeader ? _fontBold : _fontRegular)
                .SetFontSize(10)
                .SetPadding(5)
                .SetBorder(new SolidBorder(ColorConstants.BLACK, 0.5f));

            if (isHeader)
            {
                paragraph.SetBackgroundColor(bgColor ?? ColorConstants.WHITE);
            }

            return new Cell().Add(paragraph);
        }

        // Post-process PDF bytes and draw footer on each page
        private byte[] AddFootersToPdfBytes(byte[] inputPdfBytes, string usuarioNombre)
        {
            using var inputStream = new MemoryStream(inputPdfBytes);
            using var reader = new PdfReader(inputStream);
            using var outputStream = new MemoryStream();
            using var writer = new PdfWriter(outputStream);
            using var pdfDoc = new PdfDocument(reader, writer);

            // create local fonts for this pdf processing (do not reuse _fontRegular/_fontBold)
            var footerFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);
            var footerFontSmall = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);

            int total = pdfDoc.GetNumberOfPages();
            for (int i = 1; i <= total; i++)
            {
                var page = pdfDoc.GetPage(i);
                var pageSize = page.GetPageSize();
                var canvas = new PdfCanvas(page.NewContentStreamAfter(), page.GetResources(), pdfDoc);
                var layoutCanvas = new iText.Layout.Canvas(canvas, pageSize);

                float x = pageSize.GetRight() - 40;
                float y = pageSize.GetBottom() + 30;

                var pageNumPara = new Paragraph($"Página {i}")
                    .SetFont(footerFont)
                    .SetFontSize(9)
                    .SetFontColor(ColorConstants.GRAY);

                layoutCanvas.ShowTextAligned(pageNumPara, x, y, TextAlignment.RIGHT);

                var generatedAt = DateTime.Now.ToString("dd/MM/yyyy HH:mm");
                var genPara = new Paragraph($"Reporte Generado Por: {usuarioNombre} - {generatedAt}")
                    .SetFont(footerFontSmall)
                    .SetFontSize(8)
                    .SetFontColor(ColorConstants.GRAY);

                layoutCanvas.ShowTextAligned(genPara, x, y - 10, TextAlignment.RIGHT);

                layoutCanvas.Close();
            }

            pdfDoc.Close();
            return outputStream.ToArray();
        }
    }
}