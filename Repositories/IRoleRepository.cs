using MafiaAPI.Models;

namespace MafiaAPI.Repositories
{
    public interface IRoleRepository
    {
        IEnumerable<PlayerState> Get();
        Task<PlayerState> Get(string id);
        void Create(PlayerState item);
        void Update(PlayerState item);
        Task<PlayerState> Delete(string id);
    }
}
