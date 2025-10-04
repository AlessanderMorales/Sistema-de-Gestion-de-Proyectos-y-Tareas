namespace Sistema_de_Gestion_de_Proyectos_y_Tareas.Repository
{
    using Dapper;
    using Mysqlx.Crud;
    using Sistema_de_Gestion_de_Proyectos_y_Tareas.Models;
    

    
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

            _connectionSignleton.ExcuteCommand<Proyecto>(query, entity);
        }
       

        Proyecto IDB<Proyecto>.GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        void IDB<Proyecto>.UpdateAsync(Proyecto entity)
        {
            throw new NotImplementedException();
        }

        void IDB<Proyecto>.DeleteAsync(int id)
        {
            
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


