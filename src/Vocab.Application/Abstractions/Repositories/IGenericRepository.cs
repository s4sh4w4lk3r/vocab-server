using Vocab.Application.ValueObjects;

namespace Vocab.Application.Abstractions.Repositories
{
    public interface IGenericRepository<TEntity>
    {
        public IQueryable<TEntity> Get();
        public Task Insert(TEntity entity);
        public void Update(TEntity entity);
        public Task Delete(long id);
        public Task<Result> Save();
    }
}
