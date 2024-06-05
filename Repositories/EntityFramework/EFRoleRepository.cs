using MafiaAPI.Data;
using MafiaAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace MafiaAPI.Repositories.EntityFramework
{
    public class EFRoleRepository : IRoleRepository
    {
        private readonly AppDbContext _context;
        public EFRoleRepository(AppDbContext appDbContext)
        {
            _context = appDbContext;
        }

        public async Task Create(Role item)
        {
            item.Id = Guid.NewGuid().ToString();
            _context.Roles.Add(item);
            await _context.SaveChangesAsync();
        }

        public async Task<Role> Delete(string id)
        {
            var role = await Get(id);
            if (role != null)
            {
                _context.Roles.Remove(role);
                await _context.SaveChangesAsync();
            }
            return role;
        }

        public IEnumerable<Role> Get()
        {
            return _context.Roles;
        }

        public async Task<Role> Get(string id)
        {
            return await _context.Roles.FindAsync(id);
        }

        public async Task<Role> GetByName(string name)
        {
            return await _context.Roles.FirstOrDefaultAsync(x => x.Name == name);
        }

        public async Task Update(Role item)
        {
            var role = await Get(item.Id);
            if (role != null)
            {
                role.Name = item.Name;
                role.Description = item.Description;

                _context.Roles.Update(role);
                await _context.SaveChangesAsync();
            }
        }
    }
}
