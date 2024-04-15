using Vocab.Core.Entities;

namespace Vocab.Application.Abstractions.Repositories
{
    public interface IUserRepository
    {
        Task<User> Get(Guid guid);
        Task Insert(User user);
        Task Delete(Guid guid);
        Task Update(User user);
    }
}
