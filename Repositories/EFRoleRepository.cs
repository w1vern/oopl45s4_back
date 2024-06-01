using MafiaAPI.Data;
using MafiaAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace MafiaAPI.Repositories
{
    public class EFRoleRepository : IRoleRepository
    {
        private readonly AppDbContext _context;
        public EFRoleRepository(AppDbContext appDbContext)
        {
            _context = appDbContext;
        }

        public async void Create(PlayerState item)
        {
            item.Id = Guid.NewGuid().ToString();
            _context.Roles.Add(item);
            await _context.SaveChangesAsync();
        }

        public async Task<PlayerState> Delete(string id)
        {
            var match = Get(id);
            if (match != null)
            {
                _context.Roles.Remove(match.Result);
                await _context.SaveChangesAsync();
            }
            return match.Result;
        }

        public IEnumerable<PlayerState> Get()
        {
            return _context.Roles;
        }

        public async Task<PlayerState> Get(string id)
        {
            return await _context.Roles.FindAsync(id);
        }

        public async void Update(PlayerState item)
        {
            var match = Get(item.Id);
            if (match != null)
            {
                match.Result.Name = item.Name;

                _context.Roles.Update(match.Result);
                await _context.SaveChangesAsync();
            }
        }
    }
}
