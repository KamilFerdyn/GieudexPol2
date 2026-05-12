using GieudexPol.Application.Interfaces;
using GieudexPol.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GieudexPol.Infrastructure.Repositories
{
    public class UserAlertRepository : GenericRepository<UserAlert>, IUserAlertRepository
    {
        public UserAlertRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<UserAlert>> GetUserAlertsByUserIdAsync(int userId)
        {
            return await _dbSet.Where(ua => ua.UserId == userId).ToListAsync();
        }
    }
}
