
using Sistema_de_Gestion_de_Proyectos_y_Tareas.Domain.Entities;
using Sistema_de_Gestion_de_Proyectos_y_Tareas.Infrastructure.Persistence.Data; 
using Sistema_de_Gestion_de_Proyectos_y_Tareas.Infrastructure.Persistence.Factories;

using Sistema_de_Gestion_de_Proyectos_y_Tareas.Application.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();

builder.Services.AddSingleton<MySqlConnectionSingleton>();
builder.Services.AddScoped<MySqlRepositoryFactory<Proyecto>, ProyectoryRepositoryCreator>();
builder.Services.AddScoped<MySqlRepositoryFactory<Usuario>, UsuarioRepositoryCreator>();
builder.Services.AddScoped<MySqlRepositoryFactory<Tarea>, TareaRepositoryCreator>();
builder.Services.AddScoped<ProyectoService>();


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