namespace Sistema_de_Gestion_de_Proyectos_y_Tareas.Domain.Ports.Repositories
{
    public interface IDB<T> 
    {
        IEnumerable<T> GetAllAsync();
        T GetByIdAsync(int id);
        void AddAsync(T entity);
        void UpdateAsync(T entity);
        void DeleteAsync(int id);
        void DeleteAsync(T entity);
    }
}