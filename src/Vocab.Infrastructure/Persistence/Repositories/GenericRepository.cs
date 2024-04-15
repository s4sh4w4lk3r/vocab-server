using Microsoft.EntityFrameworkCore;
using Vocab.Application.Abstractions.Repositories;
using Vocab.Application.ValueObjects;
using Vocab.Core;

namespace Vocab.Infrastructure.Persistence.Repositories
{
    internal class GenericRepository<TEntity>(VocabContext context) : IGenericRepository<TEntity>, IAsyncDisposable where TEntity : class, IEntity
    {
        private readonly DbSet<TEntity> _set = context.Set<TEntity>();

        public Task Delete(long id) => _set.Where(e => e.Id == id).ExecuteDeleteAsync();
        public IQueryable<TEntity> Get() => _set.AsQueryable<TEntity>();
        public Task Insert(TEntity entity) => _set.AddAsync(entity).AsTask();
        public void Update(TEntity entity) => _set.Update(entity);

        public Task<Result> Save() => context.TrySaveChangesAsync();
        public ValueTask DisposeAsync() => context.DisposeAsync();
    }
}
