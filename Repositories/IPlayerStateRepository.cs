using MafiaAPI.Models;

namespace MafiaAPI.Repositories
{
    public interface IPlayerStateRepository
    {
        IEnumerable<PlayerState> Get();
        Task<PlayerState> Get(string id);
        Task Create(PlayerState item);
        Task Update(PlayerState item);
        Task<PlayerState> Delete(string id);
    }
}
