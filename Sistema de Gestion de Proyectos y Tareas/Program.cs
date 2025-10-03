using Sistema_de_Gestion_de_Proyectos_y_Tareas.Models;
using Sistema_de_Gestion_de_Proyectos_y_Tareas.Repository;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

// ---> INICIO DE NUESTROS SERVICIOS
// 1. La fábrica de conexiones
builder.Services.AddSingleton<IDbConnectionFactory, MySqlConnectionFactory>();

// 2. Los repositorios
builder.Services.AddTransient<IDB<Proyecto>, ProyectoRepository>();
builder.Services.AddTransient<IDB<Usuario>, UsuarioRepository>();
builder.Services.AddTransient<IDB<Tarea>, TareaRepository>();
// ---> FIN DE NUESTROS SERVICIOS

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();