using MafiaAPI.Models;

namespace MafiaAPI.Repositories
{
    public interface IUserRepository
    {
        IEnumerable<User> Get();
        Task<User> Get(string id);
        Task Create(User item);
        Task Update(User item);
        Task<User> Delete(string id);
        Task<User> GetByName(string name);
    }
}
