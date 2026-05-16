using GieudexPol.Domain.Auth;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace GieudexPol.Infrastructure.Auth
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserRepository(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<User> GetByEmailAsync(string email)
        {
            var applicationUser = await _userManager.FindByEmailAsync(email);
            if (applicationUser == null)
            {
                return null;
            }
            return new User(Guid.Parse(applicationUser.Id), applicationUser.Email, applicationUser.PasswordHash);
        }

        public async Task AddAsync(User user)
        {
            var applicationUser = new ApplicationUser { UserName = user.Email, Email = user.Email };
            var result = await _userManager.CreateAsync(applicationUser, user.HashedPassword);
            if (!result.Succeeded)
            {
                throw new System.Exception("Failed to create user."); // Obsługa błędów rejestracji
            }
            user.UpdatePassword(applicationUser.PasswordHash); // Aktualizacja hasła po zahashowaniu przez UserManager
        }

        public async Task UpdateAsync(User user)
        {
            var applicationUser = await _userManager.FindByEmailAsync(user.Email);
            if (applicationUser == null)
            {
                throw new UserNotFoundException(user.Email);
            }

            applicationUser.Email = user.Email;
            applicationUser.UserName = user.Email; // Aktualizacja UserName, jeśli zmieniamy email
            applicationUser.PasswordHash = user.HashedPassword; // Zakładamy, że HashedPassword jest już zahashowany

            var result = await _userManager.UpdateAsync(applicationUser);
            if (!result.Succeeded)
            {
                throw new System.Exception("Failed to update user."); // Obsługa błędów aktualizacji
            }
        }
    }
}