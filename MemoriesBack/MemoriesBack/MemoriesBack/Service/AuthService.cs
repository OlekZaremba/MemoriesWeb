
using System;
using System.Linq; 
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using MemoriesBack.Entities;
using MemoriesBack.Repository; 
using MemoriesBack.DTO;

namespace MemoriesBack.Service
{
    public class AuthService
    {
        private readonly SensitiveDataRepository _sensitiveDataRepository;
        private readonly UserRepository _userRepository;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly UserGroupRepository _userGroupRepository;
        private readonly GroupMemberRepository _groupMemberRepository;
        
        private readonly IGroupMemberClassRepository _groupMemberClassRepository;

        public AuthService(
            SensitiveDataRepository sensitiveDataRepository,
            UserRepository userRepository,
            IPasswordHasher<User> passwordHasher,
            UserGroupRepository userGroupRepository,
            GroupMemberRepository groupMemberRepository,
            
            IGroupMemberClassRepository groupMemberClassRepository)
        {
            _sensitiveDataRepository = sensitiveDataRepository;
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _userGroupRepository = userGroupRepository;
            _groupMemberRepository = groupMemberRepository;
            _groupMemberClassRepository = groupMemberClassRepository;
        }

        public async Task<LoginResponse> Login(string login, string password)
        {
            var data = await _sensitiveDataRepository.GetByLoginAsync(login)
                       ?? throw new ArgumentException("Nieprawidłowy login");

            var user = data.User ?? throw new InvalidOperationException("User data not found for sensitive data entry.");
            var result = _passwordHasher.VerifyHashedPassword(user, data.Password, password);
            if (result != PasswordVerificationResult.Success)
                throw new ArgumentException("Nieprawidłowe hasło");

            var members = await _groupMemberRepository.GetAllByUserIdAsync(user.Id); 
            
            string? className = null;
            int? groupId = null;

            if (members != null && members.Any())
            {
                var firstMember = members.First();
                
                var userGroup = firstMember.UserGroup; 
                if (userGroup == null && firstMember.UserGroupId != 0) 
                {
                    
                    
                    
                    
                    var tempGroup = await _userGroupRepository.GetByIdAsync(firstMember.UserGroupId);
                    className = tempGroup?.GroupName;
                }
                else
                {
                    className = userGroup?.GroupName;
                }
                groupId = firstMember.UserGroupId;
            }

            string imageBase64 = string.IsNullOrWhiteSpace(user.Image) ? "" : user.Image;

            return new LoginResponse(
                user.Id,
                user.Name,
                user.Surname,
                user.UserRole,
                imageBase64,
                className, 
                groupId    
            );
        }

        public async Task RegisterUser(RegisterUserRequest request)
        {
            var existing = await _sensitiveDataRepository.GetByLoginAsync(request.Login);
            if (existing != null)
                throw new ArgumentException("Podany login jest już zajęty");

            var user = new User
            {
                Name = request.Name,
                Surname = request.Surname,
                UserRole = request.Role
            };

            await _userRepository.AddAsync(user); 

            var sensitive = new SensitiveData
            {
                Login = request.Login,
                Email = request.Email, 
                Password = _passwordHasher.HashPassword(user, request.Password),
                UserId = user.Id 
            };

            await _sensitiveDataRepository.AddAsync(sensitive);

            if (request.GroupId != 0) 
            {
                var group = await _userGroupRepository.GetByIdAsync(request.GroupId)
                    ?? throw new ArgumentException("Nie ma takiej grupy: " + request.GroupId);

                var gm = new GroupMember
                {
                    UserId = user.Id, 
                    UserGroupId = group.Id 
                };

                await _groupMemberRepository.AddAsync(gm);
            }
        }
    }
}
