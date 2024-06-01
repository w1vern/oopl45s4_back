using MafiaAPI.Data;
using MafiaAPI.Models;

namespace MafiaAPI.Repositories
{
    public class EFMatchRepository : IMatchRepository
    {
        private readonly AppDbContext _context;
        public EFMatchRepository(AppDbContext appDbContext)
        {
            _context = appDbContext;
        }

        public async void Create(Match item)
        {
            item.Id = Guid.NewGuid().ToString();
            _context.Matches.Add(item);
            await _context.SaveChangesAsync();
        }

        public async Task<Match> Delete(string id)
        {
            var match = Get(id);
            if (match != null)
            {
                _context.Matches.Remove(match.Result);
                await _context.SaveChangesAsync();
            }
            return match.Result;
        }

        public IEnumerable<Match> Get()
        {
            return _context.Matches;
        }

        public async Task<Match> Get(string id)
        {
            return await _context.Matches.FindAsync(id);
        }

        public async void Update(Match item)
        {
            var match = Get(item.Id);
            if (match != null)
            {
                match.Result.MatchStart = item.MatchStart;
                match.Result.MatchEnd = item.MatchEnd;
                match.Result.WebsocketURL = item.WebsocketURL;
                match.Result.PlayerStates = item.PlayerStates;

                _context.Matches.Update(match.Result);
                await _context.SaveChangesAsync();
            }
        }
    }
}
