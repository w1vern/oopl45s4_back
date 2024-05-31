using MafiaAPI.Data;
using MafiaAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace MafiaAPI.Repositories
{
    public class EFUserRepository : IUserRepository
    {
        private readonly AppDbContext _context;
        public EFUserRepository(AppDbContext appDbContext) {
            _context = appDbContext;
        }
        public async void Create(User item)
        {
            item.Id = Guid.NewGuid().ToString();
            _context.Users.Add(item);
            await _context.SaveChangesAsync();
        }

        public async Task<User> Delete(string id)
        {
            var user = Get(id);
            if(user != null)
            {
                _context.Users.Remove(user.Result);
                await _context.SaveChangesAsync();
            }
            return user.Result;
        }

        public IEnumerable<User> Get()
        {
            return _context.Users;
        }

        public async Task<User> Get(string id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async void Update(User item)
        {
            var user = Get(item.Id);
            if(user != null)
            {
                user.Result.Name = item.Name;
                user.Result.Password = item.Password;

                _context.Users.Update(user.Result);
                _context.SaveChanges();
            }
        }

        public async Task<User> DeleteByName(string name)
        {
            var user = GetByName(name);
            if (user != null)
            {
                _context.Users.Remove(user.Result);
                await _context.SaveChangesAsync();
            }
            return user.Result;
        }

        public async Task<User> GetByName(string name)
        {
            return await _context.Users.FirstOrDefaultAsync(x => x.Name == name);
        }

        public async Task<User> GetByToken(string id, string secret)
        {
            var user = Get(id);
            if (user != null && user.Result.Secret == secret)
            {
                return user.Result;
            }
            return null;
        }
    }
}
