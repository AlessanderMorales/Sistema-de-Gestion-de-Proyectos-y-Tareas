

using Sistema_de_Gestion_de_Proyectos_y_Tareas.Domain.Entities;
using Sistema_de_Gestion_de_Proyectos_y_Tareas.Infrastructure.Persistence.Factories;
using System.Collections.Generic;
using System.Linq;
using System;

namespace Sistema_de_Gestion_de_Proyectos_y_Tareas.Application.Services
{
    public class UsuarioService
    {
        private readonly MySqlRepositoryFactory<Usuario> _usuarioFactory;

        public UsuarioService(MySqlRepositoryFactory<Usuario> usuarioFactory)
        {
            _usuarioFactory = usuarioFactory;
        }

        public IEnumerable<Usuario> ObtenerTodosLosUsuarios()
        {
            var repo = _usuarioFactory.CreateRepository();
            return repo.GetAllAsync();
        }

        public Usuario ObtenerUsuarioPorId(int id)
        {
            var repo = _usuarioFactory.CreateRepository();
            return repo.GetByIdAsync(id);
        }

        public void CrearNuevoUsuario(Usuario usuario)
        {
            var repo = _usuarioFactory.CreateRepository();
            repo.AddAsync(usuario);
        }

        public void ActualizarUsuario(Usuario usuario)
        {
            var repo = _usuarioFactory.CreateRepository();
            repo.UpdateAsync(usuario);
        }

        public void EliminarUsuario(int id)
        {
            var repo = _usuarioFactory.CreateRepository();
            repo.DeleteAsync(id);
        }

        public Usuario ValidarUsuario(string email, string password)
        {
            var repo = _usuarioFactory.CreateRepository();
            var usuario = repo.GetAllAsync().FirstOrDefault(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));

            if (usuario != null && usuario.Contraseña == password)
            {
                return usuario;
            }
            return null;
        }
    }
}