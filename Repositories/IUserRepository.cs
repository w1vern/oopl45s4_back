using MafiaAPI.Models;

namespace MafiaAPI.Repositories
{
    public interface IUserRepository
    {
        IEnumerable<User> Get();
        Task<User> Get(string id);
        void Create(User item);
        void Update(User item);
        Task<User> Delete(string id);
        Task<User> GetByName(string name);
    }
}
