﻿using Backend.Data.Models;
using Backend.Repositories.Implementations;
using Backend.Repositories.Interfaces;
using Backend.Services.Interfaces;

namespace Backend.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
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
    }
}
