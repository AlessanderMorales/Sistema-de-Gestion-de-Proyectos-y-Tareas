namespace Sistema_de_Gestion_de_Proyectos_y_Tareas.Repository
{
    using Dapper;
    // using Mysqlx.Crud; // Esta importación sigue siendo probablemente innecesaria
    using Sistema_de_Gestion_de_Proyectos_y_Tareas.Models;
    using System.Collections.Generic; // Para IEnumerable
    // using System.Threading.Tasks; // Ya no necesario para GetByIdAsync síncrono

    public class ProyectoRepository : IDB<Proyecto>
    {
        private readonly MySqlConnectionSingleton _connectionSignleton;

        public ProyectoRepository(MySqlConnectionSingleton mySqlConnectionSingleton)
        {
            _connectionSignleton = mySqlConnectionSingleton;
        }

        public IEnumerable<Proyecto> GetAllAsync() // Aunque dice "Async", la implementación es síncrona aquí
        {
            string query = @"SELECT id_proyecto AS Id, nombre, descripcion, fecha_inicio AS FechaInicio, fecha_fin AS FechaFin ,estado as Estado
                              FROM Proyecto WHERE estado = 1 ORDER BY nombre";
            return _connectionSignleton.ExcuteCommandWithDataReturn<Proyecto>(query);
        }

        public void AddAsync(Proyecto entity) // Aunque dice "Async", la implementación es síncrona
        {
            string query = @"INSERT INTO Proyecto (nombre, descripcion, fecha_inicio, fecha_fin)
                             VALUES (@Nombre, @Descripcion, @FechaInicio, @FechaFin);";

            _connectionSignleton.ExcuteCommand<Proyecto>(query, entity);
        }

        // Implementación corregida para GetByIdAsync (SÍNCRONA)
        public Proyecto GetByIdAsync(int id)
        {
            string query = @"SELECT id_proyecto AS Id, nombre, descripcion, fecha_inicio AS FechaInicio, fecha_fin AS FechaFin, estado as Estado
                             FROM Proyecto
                             WHERE id_proyecto = @Id AND estado = 1;"; // Asumo que quieres proyectos activos

            var parameters = new { Id = id }; // Objeto anónimo para los parámetros

            // ¡Usar el nuevo método QueryFirstOrDefault del singleton!
            return _connectionSignleton.QueryFirstOrDefault<Proyecto>(query, parameters);
        }

        public void UpdateAsync(Proyecto entity) // Aunque dice "Async", la implementación es síncrona
        {
            string query = @"UPDATE Proyecto
                             SET nombre = @Nombre,
                             descripcion = @Descripcion,
                             fecha_inicio = @FechaInicio,
                             fecha_fin = @FechaFin
                             WHERE id_proyecto = @Id;";

            _connectionSignleton.ExcuteCommand<Proyecto>(query, entity);
        }

        // Implementación explícita para IDB<Proyecto>.DeleteAsync(int id)
        void IDB<Proyecto>.DeleteAsync(int id)
        {
            string query = @"UPDATE Proyecto SET Estado = 0 WHERE id_proyecto = @Id;";
            _connectionSignleton.ExcuteCommand<dynamic>(query, new { Id = id }); // Pasa el ID como objeto anónimo
        }

        public void DeleteAsync(Proyecto entity)
        {
            string query = @"UPDATE Proyecto
                            SET Estado = 0
                            WHERE id_proyecto = @Id;";
            _connectionSignleton.ExcuteCommand<Proyecto>(query, entity);
        }
    }
}