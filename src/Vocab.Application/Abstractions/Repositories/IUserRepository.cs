using Vocab.Core.Entities;

namespace Vocab.Application.Abstractions.Repositories
{
    public interface IUserRepository
    {
        Task<User> GetAsync(Guid guid);
        Task CreateAsync(User user);
        Task DeleteAsync(Guid guid);
        Task UpdateAsync(User user);
    }
}
