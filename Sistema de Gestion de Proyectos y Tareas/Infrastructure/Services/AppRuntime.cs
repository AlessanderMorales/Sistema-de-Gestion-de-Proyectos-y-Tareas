namespace Sistema_de_Gestion_de_Proyectos_y_Tareas.Infrastructure.Services
{
    public class AppRuntime
    {
        public string InstanceId { get; }
        public AppRuntime()
        {
            InstanceId = System.Guid.NewGuid().ToString();
        }
    }
}
