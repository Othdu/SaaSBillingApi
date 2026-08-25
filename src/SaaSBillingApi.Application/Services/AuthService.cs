using SaaSBillingApi.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SaaSBillingApi.Application.DTOs;
using SaaSBillingApi.Application.Interfaces;
namespace SaaSBillingApi.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IPasswordHasher _passwordhasher;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly IUnitOfWork _unitOfWork;
        public AuthService(IPasswordHasher passwordhasher, IJwtTokenGenerator jwtTokenGenerator, IUnitOfWork unitOfWork)
        {
            _passwordhasher = passwordhasher;
            _jwtTokenGenerator = jwtTokenGenerator;
            _unitOfWork = unitOfWork;
        }
        public async Task<LoginResponseDto> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken)
        {
            var user = await _unitOfWork.Users.GetByEmailAsync(request.Email, cancellationToken);
            if (user is null || !_passwordhasher.VerifyPassword(user.PasswordHash, request.Password)) //using them both in one error to stop an attacker from knowing if the email is valid or not
                throw new UnauthorizedAccessException("Invalid email or password.");

            var token = _jwtTokenGenerator.GenerateToken(user);
            return new LoginResponseDto
            {
                Token = token,
                Email = user.Email,
                Role = user.Role.ToString()
            };
        }
    }
}
