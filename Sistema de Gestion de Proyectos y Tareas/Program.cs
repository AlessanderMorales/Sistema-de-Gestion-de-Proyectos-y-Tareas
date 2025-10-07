using Sistema_de_Gestion_de_Proyectos_y_Tareas.Models;
using Sistema_de_Gestion_de_Proyectos_y_Tareas.Repository;
using Sistema_de_Gestion_de_Proyectos_y_Tareas.Factories;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddRazorPages();

builder.Services.AddSingleton<MySqlConnectionSingleton>();

builder.Services.AddScoped<ProyectoryRepositoryCreator>();
builder.Services.AddScoped<UsuarioRepositoryCreator>();

builder.Services.AddScoped<TareaRepositoryCreator>();

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