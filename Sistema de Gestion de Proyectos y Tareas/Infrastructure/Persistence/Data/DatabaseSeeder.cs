using System;
using Sistema_de_Gestion_de_Proyectos_y_Tareas.Application.Services;
using Sistema_de_Gestion_de_Proyectos_y_Tareas.Domain.Entities;
using Microsoft.Extensions.DependencyInjection;
using Sistema_de_Gestion_de_Proyectos_y_Tareas.Infrastructure.Persistence.Data;
using Dapper;
using System.Data;

namespace Sistema_de_Gestion_de_Proyectos_y_Tareas.Infrastructure.Persistence.Data
{
    public static class DatabaseSeeder
    {
        public static void Seed(IServiceProvider services)
        {
            using var scope = services.CreateScope();
            var usuarioService = scope.ServiceProvider.GetService<UsuarioService>();
            var connectionSingleton = scope.ServiceProvider.GetService<MySqlConnectionSingleton>();

            // Try to adjust schema if possible (add username column, enlarge contraseña)
            if (connectionSingleton != null)
            {
                try
                {
                    using var conn = connectionSingleton.CreateConnection();
                    // attempt to add username column if missing
                    try
                    {
                        conn.Execute("ALTER TABLE Usuario ADD COLUMN username VARCHAR(30) NULL;");
                    }
                    catch { /* ignore if column exists or SQL not supported */ }

                    // attempt to set username from primer_nombre where username is null
                    try
                    {
                        conn.Execute("UPDATE Usuario SET username = primer_nombre WHERE username IS NULL OR username = ''; ");
                    }
                    catch { }

                    // attempt to enlarge contraseña column to support bcrypt hashes
                    try
                    {
                        conn.Execute("ALTER TABLE Usuario MODIFY `contraseña` VARCHAR(100) NOT NULL;");
                    }
                    catch { }
                }
                catch
                {
                    // ignore schema adjustments if connection fails
                }
            }

            if (usuarioService == null) return;

            try
            {
                // Admin
                var existsAdmin = usuarioService.ObtenerUsuarioPorUsername("admin");
                if (existsAdmin == null)
                {
                    var admin = new Usuario
                    {
                        PrimerNombre = "admin",
                        SegundoNombre = null,
                        Apellidos = "admin",
                        Username = "admin",
                        Contraseña = "Admin123!",
                        Rol = "admin",
                        Estado = 1
                    };
                    usuarioService.CrearNuevoUsuario(admin);
                    Console.WriteLine("Usuario admin creado.");
                }

                // Supervisor
                var existsSupervisor = usuarioService.ObtenerUsuarioPorUsername("supervisor");
                if (existsSupervisor == null)
                {
                    var sup = new Usuario
                    {
                        PrimerNombre = "supervisor",
                        SegundoNombre = null,
                        Apellidos = "supervisor",
                        Username = "supervisor",
                        Contraseña = "Supervisor123!",
                        Rol = "supervisor",
                        Estado = 1
                    };
                    usuarioService.CrearNuevoUsuario(sup);
                    Console.WriteLine("Usuario supervisor creado.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Seed error: {ex.Message}");
            }
        }
    }
}
