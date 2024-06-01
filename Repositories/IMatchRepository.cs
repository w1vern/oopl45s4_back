using MafiaAPI.Models;

namespace MafiaAPI.Repositories
{
    public interface IMatchRepository
    {
        IEnumerable<Match> Get();
        Task<Match> Get(string id);
        Task Create(Match item);
        Task Update(Match item);
        Task<Match> Delete(string id);
    }
}
