using Sistema_de_Gestion_de_Proyectos_y_Tareas.Models;
using Sistema_de_Gestion_de_Proyectos_y_Tareas.Repository;
using Sistema_de_Gestion_de_Proyectos_y_Tareas.Factories;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddRazorPages();

builder.Services.AddSingleton<IDbConnectionSingleton, MySqlConnectionFactory>();


builder.Services.AddSingleton<IRepositoryFactory, MySqlRepositoryFactory>();


builder.Services.AddTransient<IDB<Proyecto>, ProyectoRepository>();
builder.Services.AddTransient<IDB<Usuario>, UsuarioRepository>();
builder.Services.AddTransient<IDB<Tarea>, TareaRepository>();

var app = builder.Build();

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