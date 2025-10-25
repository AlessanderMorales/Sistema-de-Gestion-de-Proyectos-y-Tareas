using ServiceCommon.Domain.Port.Repositories;
using ServiceCommon.Infrastructure.Persistence.Data;
using ServiceComentario.Domain.Entities;

namespace ServiceComentario.Application.Service
{
    public class ComentarioService
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
    }
}
