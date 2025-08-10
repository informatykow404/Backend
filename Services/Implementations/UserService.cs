using Backend.Data.Models;
using Backend.DTOs.Auth;
using Backend.EntityFramework.Contexts;
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
        
        public async Task<(bool, string)> ReplaceData(DataUpdateDTO data, string username)
        {
            var message = string.Empty;
            var user = await _userManager.FindByNameAsync(username);
            if (user == null) return (false, "Couldn't find user");
            
            if (string.IsNullOrWhiteSpace(data.Username) == false && _userManager.FindByNameAsync(data.Username).Result == null) {
                await _userManager.SetUserNameAsync(user, data.Username);
                message = message + "Username updated.";
            }
            else if (string.IsNullOrWhiteSpace(data.Username) == false) message = message + "This user name already exists.";

            if (string.IsNullOrWhiteSpace(data.Email) == false && _userManager.FindByNameAsync(data.Email).Result == null) {
                await _userManager.SetEmailAsync(user, data.Email);
                message = message + " Email updated.";
            }
            else if (string.IsNullOrWhiteSpace(data.Email) == false) message = message + " This email already exists.";
            
            if (string.IsNullOrWhiteSpace(data.PhoneNumber) == false) {
                await _userManager.SetPhoneNumberAsync(user, data.PhoneNumber);
                message = message + " Phone number updated.";
            }
            
            if (string.IsNullOrWhiteSpace(data.Name) == false) {
                user.Name = data.Name;
                message = message + " Name updated.";
            }
            
            if (string.IsNullOrWhiteSpace(data.Surname) == false) {
                user.Surname = data.Surname;
                message = message + " Surname updated.";
            }
            
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded) return (false, "Couldn't update user");
            return (true, message);
        }
        
    }
}
