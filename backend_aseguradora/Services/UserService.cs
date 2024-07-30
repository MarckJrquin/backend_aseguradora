using backend_aseguradora.Data;
using backend_aseguradora.Models;
using backend_aseguradora.Utils;
using Microsoft.EntityFrameworkCore;


namespace backend_aseguradora.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;

        public UserService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<object> GetUserByIdAsync(int id)
        {
            var user = await _context.Users
                                     .Include(u => u.Person)
                                     .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
            {
                return null;
            }

            return new
            {
                user.Id,
                user.Username,
                user.Email,
                user.Person.FirstName,
                user.Person.LastName,
                DateOfBirth = user.Person.DateOfBirth.ToString("yyyy-MM-dd")
            };
        }


        public async Task<string> EditUserAsync(int id, EditUserModel model)
        {
            var user = await _context.Users.Include(u => u.Person).FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
            {
                return "User not found.";
            }

            user.Email = model.Email;
            user.Person.FirstName = model.FirstName;
            user.Person.LastName = model.LastName;
            user.Person.DateOfBirth = model.DateOfBirth;

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return "User data updated successfully.";
        }


        public async Task<string> UpdatePasswordAsync(int id, UpdatePasswordModel model)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
            {
                return "User not found.";
            }

            if (!HashFunction.CheckPassword(model.OldPassword, user.Salt, user.Password))
            {
                return "Old password is incorrect.";
            }

            if (!PasswordValidator.IsValid(model.NewPassword))
            {
                return "Password must contain at least one letter, one number, and one uppercase letter.";
            }

            var (salt, hashedPassword) = HashFunction.CreateHashAndSalt(model.NewPassword);
            user.Password = hashedPassword;
            user.Salt = salt;

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return "Password updated successfully.";
        }

        public async Task<string> DeleteUserAsync(int id)
        {
            var user = await _context.Users.Include(u => u.Person).FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
            {
                return "User not found.";
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return "User deleted successfully.";
        }


        public async Task<IEnumerable<object>> GetAllUsersAsync()
        {
            var users = await _context.Users
                                      .Include(u => u.Person)
                                      .Include(u => u.UserRoles)
                                      .ThenInclude(ur => ur.Role)
                                      .OrderByDescending(u => u.Id)
                                      .Select(u => new
                                      {
                                          u.Id,
                                          u.Person.FirstName,
                                          u.Person.LastName,
                                          u.Username,
                                          u.Email,
                                          Roles = u.UserRoles.Select(ur => new { ur.Role.Id, ur.Role.Name }).ToList(),
                                          u.CreatedAt
                                      })
                                      .ToListAsync();

            return users;
        
        }
    }
}
