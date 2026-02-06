using Microsoft.EntityFrameworkCore;
using MyMicroservice.Infrastructure.Data;

namespace MyMicroservice.Infrastructure.Repositories
{

    public interface IRolesRepository
    {
        Task<string> GetUserRoles(int userId);
        Task <int?> GetRoleIdByAlias(string alias);
    }

    public class RolesRepository(ApplicationDbContext _context) : IRolesRepository
    {
        private readonly ApplicationDbContext _context = _context;

        public async Task<string> GetUserRoles(int userId)
        {
            return "todo";
        }

        public async Task<int?> GetRoleIdByAlias(string alias)
        {
            var role = await _context.Roles.FirstOrDefaultAsync(r => r.Name == alias);
            return role?.Id;
        }
    }
}