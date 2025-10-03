namespace Sistema_de_Gestion_de_Proyectos_y_Tareas.Repository
{

    using System.Collections.Generic;
    using System.Threading.Tasks;
    public interface IDB<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T?> GetByIdAsync(int id);
        Task AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(int id);
    }
}
