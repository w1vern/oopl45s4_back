using MafiaAPI.Models;

namespace MafiaAPI.Repositories
{
    public interface IMatchRepository
    {
        IEnumerable<Match> Get();
        Task<Match> Get(string id);
        void Create(Match item);
        void Update(Match item);
        Task<Match> Delete(string id);
    }
}
