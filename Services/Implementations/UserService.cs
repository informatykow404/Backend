using Backend.Data.Models;
using Backend.DTOs.Auth;
using Backend.Repositories.Implementations;
using Backend.Repositories.Interfaces;
using Backend.Services.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace Backend.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly UserManager<User> _userManager;
        public UserService(IUserRepository userRepository, UserManager<User> userManager)
        {
            _userRepository = userRepository;
            _userManager = userManager;
        }
        public async Task<User> CreateAsync(User user, CancellationToken ct = default)
        {
            await _userRepository.AddAsync(user, ct);
            await _userRepository.SaveChangesAsync(ct);
            return user;
        }

        public async Task<bool> DeleteAsync(string guid, CancellationToken ct = default)
        {
            var existing = await _userRepository.GetByGuidAsync(guid, ct);
            if (existing is null) return false;

            _userRepository.Remove(existing);
            await _userRepository.SaveChangesAsync(ct);
            return true;
        }

        public async Task<IEnumerable<User>> GetAllAsync(CancellationToken ct = default)
        {
            return await _userRepository.GetAllAsync(ct);
        }

        public async Task<User?> GetByGuidAsync(string guid, CancellationToken ct = default)
        {
            return await _userRepository.GetByGuidAsync(guid, ct);
        }

        public async Task<bool> UpdateAsync(string guid, User user, CancellationToken ct = default)
        {
            var existing = await _userRepository.GetByGuidAsync(guid, ct);
            if (existing is null) return false;

            existing.Name = user.Name;
            existing.Surname = user.Surname;

            _userRepository.Update(existing);
            await _userRepository.SaveChangesAsync(ct);
            return true;
        }

        public async Task<GetInfoAboutUser?> GetDataAboutUser(string username, CancellationToken ct = default)
        {
            
            try
            {
                //empty
                ct.ThrowIfCancellationRequested();
                if (string.IsNullOrEmpty(username)) return null;
                
                var userinfo = await _userRepository.GetByUsernameAsync(username, ct);
                var roles = await _userManager.GetRolesAsync(userinfo);
                var role = roles.FirstOrDefault();
                if (userinfo is null) return null;
                ct.ThrowIfCancellationRequested();
                

                return new GetInfoAboutUser
                {
                    Id = userinfo.Id,
                    PhoneNumber = userinfo.PhoneNumber,
                    Email = userinfo.Email,
                    UserName = userinfo.UserName,
                    Role = role


                };

            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception)
            {
                
                return null;
            }
            
            
            
            
            
            
        }
        
        
    }
}
