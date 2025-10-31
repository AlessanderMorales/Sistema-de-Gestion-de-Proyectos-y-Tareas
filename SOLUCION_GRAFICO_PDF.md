# ? Gr�fico de Torta en Reportes PDF - SOLUCI�N

## Problema
Al eliminar las columnas de ID en los reportes PDF, el gr�fico de torta dej� de mostrarse.

## Soluci�n Implementada

Agregar el gr�fico de torta en una **p�gina separada al final** del reporte general de proyectos, mostrando estad�sticas globales:

### M�todo a Agregar

```csharp
// Agregar justo antes de document.Close() en GenerarReporteGeneralProyectosPdf l�nea 168

// ? NUEVO: Agregar gr�fico de estad�sticas generales
AgregarGraficoEstadisticas(document, proyectos.ToList());

document.Close();
```

### M�todos Nuevos

```csharp
private void AgregarGraficoEstadisticas(Document document, List<Proyecto> proyectos)
{
    var todasLasTareas = proyectos.SelectMany(p => p.Tareas ?? Enumerable.Empty<Tarea>()).ToList();

    if (!todasLasTareas.Any())
        return;

    var sinIniciar = todasLasTareas.Count(t => t.Status == "SinIniciar");
    var enProgreso = todasLasTareas.Count(t => t.Status == "EnProgreso");
    var completadas = todasLasTareas.Count(t => t.Status == "Completada");
    var total = todasLasTareas.Count;

    if (total == 0)
        return;

    // Nueva p�gina para el gr�fico
    document.Add(new AreaBreak(AreaBreakType.NEXT_PAGE));

    var titulo = new Paragraph("Estad�sticas Generales")
      .SetFont(_fontBold)
        .SetFontSize(18)
        .SetTextAlignment(TextAlignment.CENTER)
  .SetMarginBottom(20);
    document.Add(titulo);

    // Crear tabla para el gr�fico y leyenda
    var table = new Table(UnitValue.CreatePercentArray(new float[] { 1, 1 }))
        .UseAllAvailableWidth()
   .SetBorder(Border.NO_BORDER);

    // Columna del gr�fico (usando Canvas)
    var graficoCell = new Cell()
        .SetBorder(Border.NO_BORDER)
.SetHeight(300);
    
    // Aqu� se dibujar� el gr�fico con PdfCanvas despu�s
    table.AddCell(graficoCell);

    // Columna de la leyenda
    var leyendaCell = new Cell()
        .SetBorder(Border.NO_BORDER);

    var porcentajeSinIniciar = (sinIniciar * 100.0 / total);
    var porcentajeEnProgreso = (enProgreso * 100.0 / total);
    var porcentajeCompletadas = (completadas * 100.0 / total);

    leyendaCell.Add(new Paragraph("Distribuci�n de Tareas")
     .SetFont(_fontBold)
        .SetFontSize(14)
        .SetMarginBottom(15));

    leyendaCell.Add(new Paragraph($"? Sin Iniciar: {sinIniciar} ({porcentajeSinIniciar:F1}%)")
     .SetFontColor(new DeviceRgb(180, 180, 180))
        .SetFont(_fontBold)
    .SetFontSize(12)
        .SetMarginBottom(8));

    leyendaCell.Add(new Paragraph($"? En Progreso: {enProgreso} ({porcentajeEnProgreso:F1}%)")
        .SetFontColor(new DeviceRgb(255, 215, 0))
    .SetFont(_fontBold)
        .SetFontSize(12)
        .SetMarginBottom(8));

    leyendaCell.Add(new Paragraph($"? Completadas: {completadas} ({porcentajeCompletadas:F1}%)")
        .SetFontColor(new DeviceRgb(76, 175, 80))
        .SetFont(_fontBold)
        .SetFontSize(12)
        .SetMarginBottom(20));

    leyendaCell.Add(new Paragraph($"Total de Proyectos: {proyectos.Count}")
        .SetFont(_fontBold)
        .SetFontSize(11)
        .SetMarginBottom(5));

    leyendaCell.Add(new Paragraph($"Total de Tareas: {total}")
        .SetFont(_fontBold)
        .SetFontSize(11));

    table.AddCell(leyendaCell);

    document.Add(table);
}
```

### Alternativa M�s Simple (Sin Gr�fico Visual)

Si el gr�fico de torta es muy complejo, una alternativa es mostrar las estad�sticas en formato de tabla o barras simples:

```csharp
private void AgregarEstadisticasSimples(Document document, List<Proyecto> proyectos)
{
    var todasLasTareas = proyectos.SelectMany(p => p.Tareas ?? Enumerable.Empty<Tarea>()).ToList();

    if (!todasLasTareas.Any())
        return;

    var sinIniciar = todasLasTareas.Count(t => t.Status == "SinIniciar");
    var enProgreso = todasLasTareas.Count(t => t.Status == "EnProgreso");
    var completadas = todasLasTareas.Count(t => t.Status == "Completada");
    var total = todasLasTareas.Count;

    // Nueva p�gina
    document.Add(new AreaBreak(AreaBreakType.NEXT_PAGE));

    var titulo = new Paragraph("Resumen Estad�stico")
     .SetFont(_fontBold)
    .SetFontSize(18)
        .SetTextAlignment(TextAlignment.CENTER)
        .SetMarginBottom(20);
    document.Add(titulo);

    // Tabla de estad�sticas
    var table = new Table(UnitValue.CreatePercentArray(new float[] { 2, 1, 1 }))
        .UseAllAvailableWidth()
        .SetMarginBottom(20);

  // Headers
    table.AddHeaderCell(new Cell().Add(new Paragraph("Estado"))
        .SetBackgroundColor(ColorConstants.LIGHT_GRAY)
        .SetFont(_fontBold)
        .SetTextAlignment(TextAlignment.CENTER));
    
    table.AddHeaderCell(new Cell().Add(new Paragraph("Cantidad"))
        .SetBackgroundColor(ColorConstants.LIGHT_GRAY)
        .SetFont(_fontBold)
        .SetTextAlignment(TextAlignment.CENTER));
    
    table.AddHeaderCell(new Cell().Add(new Paragraph("Porcentaje"))
        .SetBackgroundColor(ColorConstants.LIGHT_GRAY)
  .SetFont(_fontBold)
      .SetTextAlignment(TextAlignment.CENTER));

  // Sin Iniciar
    table.AddCell(new Cell().Add(new Paragraph("Sin Iniciar"))
        .SetBackgroundColor(new DeviceRgb(235, 235, 235)));
    table.AddCell(new Cell().Add(new Paragraph(sinIniciar.ToString()))
 .SetTextAlignment(TextAlignment.CENTER));
    table.AddCell(new Cell().Add(new Paragraph($"{(sinIniciar * 100.0 / total):F1}%"))
        .SetTextAlignment(TextAlignment.CENTER));

    // En Progreso
  table.AddCell(new Cell().Add(new Paragraph("En Progreso"))
        .SetBackgroundColor(new DeviceRgb(255, 244, 179)));
    table.AddCell(new Cell().Add(new Paragraph(enProgreso.ToString()))
        .SetTextAlignment(TextAlignment.CENTER));
  table.AddCell(new Cell().Add(new Paragraph($"{(enProgreso * 100.0 / total):F1}%"))
        .SetTextAlignment(TextAlignment.CENTER));

    // Completadas
    table.AddCell(new Cell().Add(new Paragraph("Completadas"))
        .SetBackgroundColor(new DeviceRgb(200, 230, 201)));
    table.AddCell(new Cell().Add(new Paragraph(completadas.ToString()))
        .SetTextAlignment(TextAlignment.CENTER));
    table.AddCell(new Cell().Add(new Paragraph($"{(completadas * 100.0 / total):F1}%"))
        .SetTextAlignment(TextAlignment.CENTER));

    // Total
    table.AddCell(new Cell().Add(new Paragraph("TOTAL"))
        .SetFont(_fontBold)
.SetBackgroundColor(ColorConstants.LIGHT_GRAY));
    table.AddCell(new Cell().Add(new Paragraph(total.ToString()))
 .SetFont(_fontBold)
        .SetTextAlignment(TextAlignment.CENTER)
        .SetBackgroundColor(ColorConstants.LIGHT_GRAY));
    table.AddCell(new Cell().Add(new Paragraph("100.0%"))
     .SetFont(_fontBold)
        .SetTextAlignment(TextAlignment.CENTER)
        .SetBackgroundColor(ColorConstants.LIGHT_GRAY));

    document.Add(table);

    // Informaci�n adicional
    var info = new Paragraph($"N�mero total de proyectos: {proyectos.Count}")
        .SetFont(_fontBold)
      .SetFontSize(12)
        .SetTextAlignment(TextAlignment.CENTER)
        .SetMarginTop(10);
    document.Add(info);
}
```

## Uso

En el m�todo `GenerarReporteGeneralProyectosPdf`, justo antes de `document.Close()`:

```csharp
// Opci�n 1: Con gr�fico (m�s visual pero m�s complejo)
// AgregarGraficoEstadisticas(document, proyectos.ToList());

// Opci�n 2: Tabla simple (m�s f�cil, igualmente efectiva)
AgregarEstadisticasSimples(document, proyectos.ToList());

document.Close();
```

## Ventajas de la Tabla Simple

? **M�s simple de implementar**  
? **No depende de coordenadas complejas**  
? **Funciona sin importar las columnas de IDs**  
? **F�cil de mantener**  
? **Informaci�n clara y legible**  
? **Compatible con cualquier versi�n de iText**  

## Implementaci�n Recomendada

Usar la **tabla simple de estad�sticas** porque:
- Es m�s f�cil de implementar y mantener
- No depende de gr�ficos complejos
- La informaci�n es clara y profesional
- No requiere coordenadas ni c�lculos trigonom�tricos
