using ServiceProyecto.Domain.Entities;
using System.Collections.Generic;

namespace ServiceProyecto.Application.Service.Reportes.Builders
{
    public interface IReportBuilder
    {
        void Start(string usuarioNombre);
        void AddProject(Proyecto proyecto);
        void Finish();
        byte[] GetReport();
    }
}