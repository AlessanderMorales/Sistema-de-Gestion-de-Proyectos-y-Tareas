namespace Sistema_de_Gestion_de_Proyectos_y_Tareas.Infrastructure.Persistence.Repositories
{
    using Dapper;
    using Sistema_de_Gestion_de_Proyectos_y_Tareas.Domain.Entities;
    using Sistema_de_Gestion_de_Proyectos_y_Tareas.Domain.Ports.Repositories;
    using System.Collections.Generic;
    using Sistema_de_Gestion_de_Proyectos_y_Tareas.Infrastructure.Persistence.Data;

    public class ProyectoRepository : IDB<Proyecto>
    {
        private readonly MySqlConnectionSingleton _connectionSignleton;

        public ProyectoRepository(MySqlConnectionSingleton mySqlConnectionSingleton)
        {
            _connectionSignleton = mySqlConnectionSingleton;
        }

        public IEnumerable<Proyecto> GetAllAsync() 
        {
            string query = @"SELECT id_proyecto AS Id, nombre, descripcion, fecha_inicio AS FechaInicio, fecha_fin AS FechaFin ,estado as Estado
                              FROM Proyecto WHERE estado = 1 ORDER BY nombre";
            return _connectionSignleton.ExcuteCommandWithDataReturn<Proyecto>(query);
        }

        public void AddAsync(Proyecto entity) 
        {
            string query = @"INSERT INTO Proyecto (nombre, descripcion, fecha_inicio, fecha_fin)
                             VALUES (@Nombre, @Descripcion, @FechaInicio, @FechaFin);";

            _connectionSignleton.ExcuteCommand(query, entity);
        }

        public Proyecto GetByIdAsync(int id)
        {
            string query = @"SELECT id_proyecto AS Id, nombre, descripcion, fecha_inicio AS FechaInicio, fecha_fin AS FechaFin, estado as Estado
                             FROM Proyecto
                             WHERE id_proyecto = @Id AND estado = 1;"; 

            var parameters = new { Id = id };
            return _connectionSignleton.QueryFirstOrDefault<Proyecto>(query, parameters);
        }

        public void UpdateAsync(Proyecto entity)
        {
            string query = @"UPDATE Proyecto
                             SET nombre = @Nombre,
                             descripcion = @Descripcion,
                             fecha_inicio = @FechaInicio,
                             fecha_fin = @FechaFin
                             WHERE id_proyecto = @Id;";

            _connectionSignleton.ExcuteCommand(query, entity);
        }
        void IDB<Proyecto>.DeleteAsync(int id)
        {
            string query = @"UPDATE Proyecto SET Estado = 0 WHERE id_proyecto = @Id;";
            _connectionSignleton.ExcuteCommand<dynamic>(query, new { Id = id });

            // also deactivate related tasks
            string query2 = @"UPDATE Tareas SET estado = 0 WHERE id_proyecto = @Id;";
            _connectionSignleton.ExcuteCommand<dynamic>(query2, new { Id = id });
        }

        public void DeleteAsync(Proyecto entity)
        {
            string query = @"UPDATE Proyecto
                            SET Estado = 0
                            WHERE id_proyecto = @Id;";
            _connectionSignleton.ExcuteCommand(query, entity);

            // also deactivate related tasks
            string query2 = @"UPDATE Tareas SET estado = 0 WHERE id_proyecto = @Id;";
            _connectionSignleton.ExcuteCommand(query2, entity);
        }

        public void DeactivateByProjectId(int idProyecto)
        {
            // Deactivate project and its tasks
            string query = @"UPDATE Proyecto SET Estado = 0 WHERE id_proyecto = @IdProyecto;";
            _connectionSignleton.ExcuteCommand<dynamic>(query, new { IdProyecto = idProyecto });

            string query2 = @"UPDATE Tareas SET estado = 0 WHERE id_proyecto = @IdProyecto;";
            _connectionSignleton.ExcuteCommand<dynamic>(query2, new { IdProyecto = idProyecto });
        }


        public Proyecto GetByIdConTareas(int idProyecto)
        {
            using var connection = _connectionSignleton.CreateConnection();
            var sql = @"
        SELECT 
            p.*, -- Seleccionamos todos los campos de Proyecto con sus nombres originales
            t.*  -- Seleccionamos todos los campos de Tareas con sus nombres originales
        FROM Proyecto p
        LEFT JOIN Tareas t ON p.id_proyecto = t.id_proyecto
        WHERE p.id_proyecto = @Id;";

            var proyectoDictionary = new Dictionary<int, Proyecto>();

            var proyectos = connection.Query<Proyecto, Tarea, Proyecto>(
                sql,
                (proyecto, tarea) =>
                {
                    if (!proyectoDictionary.TryGetValue(proyecto.Id, out var proyectoActual))
                    {
                        proyectoActual = proyecto;
                        proyectoActual.Tareas = new List<Tarea>();
                        proyectoDictionary.Add(proyectoActual.Id, proyectoActual);
                    }

                    if (tarea != null)
                    {
                        proyectoActual.Tareas.Add(tarea);
                    }

                    return proyectoActual;
                },
                new { Id = idProyecto },
                splitOn: "id_tarea"
            ).Distinct().ToList();

            return proyectos.FirstOrDefault();
        }



    }
}