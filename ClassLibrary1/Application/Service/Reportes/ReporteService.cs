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
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace ServiceProyecto.Application.Service.Reportes
{
    public class ReporteService
    {
        private readonly ProyectoService _proyectoService;

        // ✅ Fuentes (una regular y una en negrita)
        private readonly PdfFont _fontRegular;
        private readonly PdfFont _fontBold;

        public ReporteService(ProyectoService proyectoService)
        {
            _proyectoService = proyectoService;
            _fontRegular = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);
            _fontBold = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);
        }

        // -----------------------------
        // Método para 1 proyecto
        // -----------------------------
        public byte[] GenerarReporteProyectoPdf(int idProyecto)
        {
            var proyecto = _proyectoService.ObtenerProyectoConTareas(idProyecto);

            if (proyecto == null) return null;

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
            document.Add(new Paragraph("Integrantes: (Deducidos de las tareas - Falta lógica de UsuarioService)")
                .SetFont(_fontRegular)
                .SetFontSize(10)
                .SetFontColor(ColorConstants.GRAY));

            document.Close();
            return stream.ToArray();
        }

        // -----------------------------
        // Nuevo método: todos los proyectos
        // -----------------------------
        public byte[] GenerarReporteGeneralProyectosPdf(IEnumerable<Proyecto> proyectos)
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
                document.Add(new Paragraph("Integrantes: (Deducidos de las tareas - Falta lógica de UsuarioService)")
                    .SetFont(_fontRegular)
                    .SetFontSize(10)
                    .SetFontColor(ColorConstants.GRAY));

                // Salto de página si no es el último proyecto
                if (proyecto != proyectos.Last())
                {
                    document.Add(new AreaBreak(AreaBreakType.NEXT_PAGE));
                }
            }

            document.Close();
            return stream.ToArray();
        }

        // -----------------------------
        // Métodos internos
        // -----------------------------
        private Table CrearSeccionInfoPrincipal(Proyecto proyecto)
        {
            var table = new Table(UnitValue.CreatePercentArray(new float[] { 1, 3 }))
                .UseAllAvailableWidth()
                .SetMarginBottom(15);

            table.AddHeaderCell(CrearCelda("ID Proyecto:", true));
            table.AddCell(CrearCelda(proyecto.Id.ToString()));

            table.AddHeaderCell(CrearCelda("Nombre:", true));
            table.AddCell(CrearCelda(proyecto.Nombre));

            table.AddHeaderCell(CrearCelda("Descripción:", true));
            table.AddCell(CrearCelda(proyecto.Descripcion ?? "N/A"));

            table.AddHeaderCell(CrearCelda("Inicia:", true));
            table.AddCell(CrearCelda(proyecto.FechaInicio.ToShortDateString()));

            table.AddHeaderCell(CrearCelda("Finaliza:", true));
            table.AddCell(CrearCelda(proyecto.FechaFin.ToShortDateString()));

            table.AddHeaderCell(CrearCelda("Estado:", true));
            table.AddCell(CrearCelda(proyecto.Estado == 1 ? "Activo" : "Inactivo"));

            return table;
        }

        private Table CrearTablaTareas(Proyecto proyecto)
        {
            var table = new Table(UnitValue.CreatePercentArray(new float[] { 1, 3, 1, 1 }))
                .UseAllAvailableWidth()
                .SetBorder(new SolidBorder(ColorConstants.BLACK, 1));

            // Encabezados
            table.AddHeaderCell(CrearCelda("ID Tarea", true, ColorConstants.LIGHT_GRAY));
            table.AddHeaderCell(CrearCelda("Título", true, ColorConstants.LIGHT_GRAY));
            table.AddHeaderCell(CrearCelda("Prioridad", true, ColorConstants.LIGHT_GRAY));
            table.AddHeaderCell(CrearCelda("Estado", true, ColorConstants.LIGHT_GRAY));

            if (!proyecto.Tareas.Any())
            {
                table.AddCell(new Cell(1, 4)
                    .Add(new Paragraph("No hay tareas asignadas a este proyecto."))
                    .SetTextAlignment(TextAlignment.CENTER));
            }
            else
            {
                foreach (var tarea in proyecto.Tareas)
                {
                    table.AddCell(CrearCelda(tarea.Id.ToString()));
                    table.AddCell(CrearCelda(tarea.Titulo));
                    table.AddCell(CrearCelda(tarea.Prioridad.ToString()));
                    table.AddCell(CrearCelda(tarea.Estado.ToString()));
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
    }
}

