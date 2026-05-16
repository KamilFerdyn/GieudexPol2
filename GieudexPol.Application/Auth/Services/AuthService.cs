using GieudexPol.Application.Auth.Commands;
using GieudexPol.Application.Auth.DTOs;
using GieudexPol.Domain.Auth;
using GieudexPol.Infrastructure.Auth;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace GieudexPol.Application.Auth.Services
{
    public class AuthService : 
        IRequestHandler<RegisterUserCommand, AuthResponse>,
        IRequestHandler<LoginUserCommand, AuthResponse>
    {
        private readonly IUserRepository _userRepository;
        private readonly JwtService _jwtService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AuthService(IUserRepository userRepository, JwtService jwtService, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userRepository = userRepository;
            _jwtService = jwtService;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<AuthResponse> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            var existingUser = await _userRepository.GetByEmailAsync(request.RegisterRequest.Email);
            if (existingUser != null)
            {
                throw new UserAlreadyExistsException(request.RegisterRequest.Email);
            }

            var user = new User(Guid.NewGuid(), request.RegisterRequest.Email, request.RegisterRequest.Password);
            await _userRepository.AddAsync(user);

            var token = _jwtService.GenerateToken(user.Id.ToString(), user.Email);

            return new AuthResponse { Token = token, Email = user.Email };
        }

        public async Task<AuthResponse> Handle(LoginUserCommand request, CancellationToken cancellationToken)
        {
            var applicationUser = await _userManager.FindByEmailAsync(request.LoginRequest.Email);
            if (applicationUser == null)
            {
                throw new InvalidCredentialsException();
            }

            var result = await _signInManager.CheckPasswordSignInAsync(applicationUser, request.LoginRequest.Password, false);
            if (!result.Succeeded)
            {
                throw new InvalidCredentialsException();
            }

            var user = await _userRepository.GetByEmailAsync(request.LoginRequest.Email);
            var token = _jwtService.GenerateToken(user.Id.ToString(), user.Email);

            return new AuthResponse { Token = token, Email = user.Email };
        }
    }
}