using MafiaAPI.Data;
using MafiaAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace MafiaAPI.Repositories.EntityFramework
{
    public class EFUserRepository : IUserRepository
    {
        private readonly AppDbContext _context;
        public EFUserRepository(AppDbContext appDbContext)
        {
            _context = appDbContext;
        }
        public async Task Create(User item)
        {
            item.Id = Guid.NewGuid().ToString();
            _context.Users.Add(item);
            await _context.SaveChangesAsync();
        }

        public async Task<User> Delete(string id)
        {
            var user = await Get(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }
            return user;
        }

        public IEnumerable<User> Get()
        {
            return _context.Users;
        }

        public async Task<User> Get(string id)
        {
            return await _context.Users.Include(x => x.PlayerStates).ThenInclude(x => x.Match).FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task Update(User item)
        {
            var user = await Get(item.Id);
            if (user != null)
            {
                user.Name = item.Name;
                user.Password = item.Password;
                user.PlayerStates = item.PlayerStates;

                _context.Users.Update(user);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<User> GetByName(string name)
        {
            return await _context.Users.FirstOrDefaultAsync(x => x.Name == name);
        }
    }
}
