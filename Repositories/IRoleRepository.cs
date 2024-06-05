using MafiaAPI.Models;

namespace MafiaAPI.Repositories
{
    public interface IRoleRepository
    {
        IEnumerable<Role> Get();
        Task<Role> Get(string id);
        Task Create(Role item);
        Task Update(Role item);
        Task<Role> Delete(string id);
        Task<Role> GetByName(string name);
    }
}
