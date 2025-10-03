using Sistema_de_Gestion_de_Proyectos_y_Tareas.Models;

public interface IDB<T>
{
    Task<IEnumerable<T>> GetAllAsync();
    Task<T?> GetByIdAsync(int id);
    Task AddAsync(T proyecto);
    Task UpdateAsync(T proyecto);
    Task DeleteAsync(int id);
}