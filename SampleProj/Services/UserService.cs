using SampleProj.Entities.Models;
using SampleProj.Repository;
using SampleProj.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SampleProj.Services
{
    public class UserService : BaseService<User>, IUserService
    {
        private Repository<User> _repository;

        public UserService(RepositoryContext repositoryContext)
            : base(repositoryContext)
        {
            _repository = new Repository<User>(repositoryContext);
        }

        public async Task<User> AuthenticateAsync(string email, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
                return null;

            var user = (await _repository.FindByAsync(u => u.Email == email, i => i.Role)).FirstOrDefault();

            // check if username exists
            if (user == null)
                return null;

            // check if password is correct
            if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
                return null;

            // authentication successful
            return user;
        }

        public async Task<User> CreateAsync(User user, string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                throw new Exception("Password is required");

            if ((await _repository.FindByConditionAsync(u => u.Email == user.Email)).Any())
                throw new Exception($@"Email {user.Email} is already taken");

            if ((await _repository.FindByConditionAsync(u => u.CompanyIdID == user.CompanyIdID)).Any())
                throw new Exception($@"CompanyIdID {user.CompanyIdID} is already taken");

            byte[] passwordHash, passwordSalt;
            CreatePasswordHash(password, out passwordHash, out passwordSalt);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            _repository.Create(user);
            await _repository.SaveAsync();

            return user;
        }

        public async Task<bool> ChangePassword(int userId, string newPassword)
        {
            var user = (await _repository.FindByConditionAsync(x => x.Id == userId)).FirstOrDefault();
            if (user != null)
            {
                byte[] newPasswordHash, newPasswordSalt;
                CreatePasswordHash(newPassword, out newPasswordHash, out newPasswordSalt);

                user.PasswordHash = newPasswordHash;
                user.PasswordSalt = newPasswordSalt;

                await _repository.UpdateEntryAsync(user, x => x.PasswordHash, x => x.PasswordSalt);
                return true;
            }
            else
                return false;
        }

        public async Task<bool> ChangePassword(int userId, string newPassword, string oldPassword)
        {
            var user = (await _repository.FindByConditionAsync(x => x.Id == userId)).FirstOrDefault();
            if (user != null)
            {
                if (VerifyPasswordHash(oldPassword, user.PasswordHash, user.PasswordSalt))
                {
                    byte[] newPasswordHash, newPasswordSalt;
                    CreatePasswordHash(newPassword, out newPasswordHash, out newPasswordSalt);

                    user.PasswordHash = newPasswordHash;
                    user.PasswordSalt = newPasswordSalt;

                    await _repository.UpdateEntryAsync(user, x => x.PasswordHash, x => x.PasswordSalt);
                    return true;
                }
                else return false;
            }
            else
                return false;
        }

        private static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            if (password == null) throw new ArgumentNullException("password");
            if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Value cannot be empty or whitespace only string.", "password");

            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        private static bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt)
        {
            if (password == null) throw new ArgumentNullException("password");
            if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Value cannot be empty or whitespace only string.", "password");
            if (storedHash.Length != 64) throw new ArgumentException("Invalid length of password hash (64 bytes expected).", "passwordHash");
            if (storedSalt.Length != 128) throw new ArgumentException("Invalid length of password salt (128 bytes expected).", "passwordHash");

            using (var hmac = new System.Security.Cryptography.HMACSHA512(storedSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != storedHash[i]) return false;
                }
            }

            return true;
        }

        public override async Task<IEnumerable<User>> GetAllAsync()
        {
            IEnumerable<User> users = await _repository.FindAllAsync((x => x.Role));
            return users;
        }

        public override async Task<IEnumerable<User>> GetAllPerPageAsync(int page, int pageSize)
        {
            return (await _repository.FindByAsync(x => true, x => x.Role)).Skip((page - 1) * pageSize).Take(pageSize);
        }

        public override async Task<User> GetByIdAsync(int id)
        {
            User user = (await _repository.FindByAsync(u => u.Id == id, i => i.Role)).FirstOrDefault();

            return user;
        }

        public override async Task UpdateAsync(User user)
        {
            var oldUserData = (await _repository.FindByConditionAsync(u => u.Id == user.Id)).FirstOrDefault();

            if (oldUserData.Email != user.Email)
                if ((await _repository.FindByConditionAsync(u => u.Email == user.Email)).Any())
                    throw new Exception($@"Email {user.Email} is already taken");

            if (oldUserData.CompanyIdID != user.CompanyIdID)
                if ((await _repository.FindByConditionAsync(u => u.CompanyIdID == user.CompanyIdID)).Any())
                    throw new Exception($@"CompanyIdID {user.CompanyIdID} is already taken");

            await _repository.UpdateEntryAsync(user,
                x => x.FullName,
                x => x.Email,
                x => x.CompanyIdID,
                x => x.RoleId,
                x => x.Status);
        }
    }
}
