using ServiceProyecto.Domain.Entities;
using ServiceProyecto.Application.Service.Reportes.Builders;
using System.Collections.Generic;

namespace ServiceProyecto.Application.Service.Reportes
{
    public class ReporteDirector
    {
        public byte[] BuildGeneral(IReportBuilder builder, IEnumerable<Proyecto> proyectos, string usuarioNombre)
        {
            builder.Start(usuarioNombre);
            foreach (var p in proyectos)
            {
                builder.AddProject(p);
            }
            builder.Finish();
            return builder.GetReport();
        }
    }
}