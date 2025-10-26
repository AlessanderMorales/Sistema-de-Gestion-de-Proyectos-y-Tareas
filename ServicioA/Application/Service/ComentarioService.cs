using ServiceCommon.Domain.Port;
using ServiceCommon.Domain.Port.Repositories;
using ServiceCommon.Infrastructure.Persistence.Data;
using ServiceComentario.Domain.Entities;
using System.Collections.Generic;
using System.Linq;

namespace ServiceComentario.Application.Service
{

    public class ComentarioService : IComentarioManager
    {
        private readonly IDB<Comentario> _comentarioRepository;

        public ComentarioService(MySqlRepositoryFactory<Comentario> repositoryFactory)
        {
            _comentarioRepository = repositoryFactory.CreateRepository();
        }

        public IEnumerable<Comentario> GetAll()
        {
            return _comentarioRepository.GetAllAsync();
        }

        public Comentario GetById(int id)
        {
            return _comentarioRepository.GetByIdAsync(id);
        }

        public void Add(Comentario comentario)
        {
            _comentarioRepository.AddAsync(comentario);
        }

        public void Update(Comentario comentario)
        {
            _comentarioRepository.UpdateAsync(comentario);
        }

        public void Delete(int id)
        {
            _comentarioRepository.DeleteAsync(id);
        }
        public void DesactivarPorTareaId(int idTarea)
        {
            var comentarios = _comentarioRepository.GetAllAsync()
                .Where(c => c.IdTarea == idTarea);

            foreach (var c in comentarios)
            {
                c.Estado = 0;
                _comentarioRepository.UpdateAsync(c);
            }
        }
    }
}