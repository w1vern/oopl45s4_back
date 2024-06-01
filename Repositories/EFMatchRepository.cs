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

        public async Task Create(Match item)
        {
            item.Id = Guid.NewGuid().ToString();
            _context.Matches.Add(item);
            await _context.SaveChangesAsync();
        }

        public async Task<Match> Delete(string id)
        {
            var match = await Get(id);
            if (match != null)
            {
                _context.Matches.Remove(match);
                await _context.SaveChangesAsync();
            }
            return match;
        }

        public IEnumerable<Match> Get()
        {
            return _context.Matches;
        }

        public async Task<Match> Get(string id)
        {
            return await _context.Matches.FindAsync(id);
        }

        public async Task Update(Match item)
        {
            var match = await Get(item.Id);
            if (match != null)
            {
                match.MatchStart = item.MatchStart;
                match.MatchEnd = item.MatchEnd;
                match.WebsocketURL = item.WebsocketURL;
                match.PlayerStates = item.PlayerStates;

                _context.Matches.Update(match);
                await _context.SaveChangesAsync();
            }
        }
    }
}
