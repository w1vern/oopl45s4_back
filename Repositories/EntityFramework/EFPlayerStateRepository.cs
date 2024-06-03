using MafiaAPI.Data;
using MafiaAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace MafiaAPI.Repositories.EntityFramework
{
    public class EFPlayerStateRepository : IPlayerStateRepository
    {
        private readonly AppDbContext _context;
        public EFPlayerStateRepository(AppDbContext appDbContext)
        {
            _context = appDbContext;
        }

        public async Task Create(PlayerState item)
        {
            item.Id = Guid.NewGuid().ToString();
            _context.PlayerStates.Add(item);
            await _context.SaveChangesAsync();
        }

        public async Task<PlayerState> Delete(string id)
        {
            var playerState = Get(id);
            if (playerState != null)
            {
                _context.PlayerStates.Remove(playerState.Result);
                await _context.SaveChangesAsync();
            }
            return playerState.Result;
        }

        public IEnumerable<PlayerState> Get()
        {
            return _context.PlayerStates;
        }

        public async Task<PlayerState> Get(string id)
        {
            return await _context.PlayerStates.FindAsync(id);
        }

        public async Task Update(PlayerState item)
        {
            var playerState = await Get(item.Id);
            if (playerState != null)
            {
                playerState.Role = item.Role;
                playerState.IsAlive = item.IsAlive;
                playerState.UserId = item.UserId;
                playerState.User = item.User;
                playerState.MatchId = item.MatchId;
                playerState.Match = item.Match;

                _context.PlayerStates.Update(playerState);
                await _context.SaveChangesAsync();
            }
        }
    }
}
