using Application.DTOs;
using Application.Interfaces.IRepo;
using Application.Interfaces.IServices;
using Domain.comman;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Services.AuthServices
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly IRefreshTokenGenerator _refreshTokenGenerator;
        private readonly IRefreshTokenRepository _refreshTokenRepository;

        public AuthService(
            IUserRepository userRepository,
            IJwtTokenGenerator jwtTokenGenerator,
            IRefreshTokenGenerator refreshTokenGenerator,
            IRefreshTokenRepository refreshTokenRepository)
        {
            _userRepository = userRepository;
            _jwtTokenGenerator = jwtTokenGenerator;
            _refreshTokenGenerator = refreshTokenGenerator;
            _refreshTokenRepository = refreshTokenRepository;
        }

        public async Task<Result<AuthResponse?>> LoginAsync(LoginRequest request)
        {
            var user = await _userRepository.GetByUsernameAsync(request.Username);
            if (user == null)
                return Result<AuthResponse?>.Failure(Error.Validation("Login.Invalid", "Invalid username or password"));

            // ⚠️ Replace with proper hash verification
            if (user.PasswordHash != request.Password)
                return Result<AuthResponse?>.Failure(Error.Validation("Login.Invalid", "Invalid username or password"));

            var token = _jwtTokenGenerator.GenerateToken(user);

            var refreshToken = new RefreshToken
            {
                Id = Guid.NewGuid(),
                Token = _refreshTokenGenerator.Generate(),
                UserId = user.Id,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                IsRevoked = false
            };

            await _refreshTokenRepository.AddAsync(refreshToken);
            await _refreshTokenRepository.SaveChangesAsync();


            return Result<AuthResponse?>.Success(new AuthResponse
            {
                AccessToken = token,
                RefreshToken = refreshToken.Token
            });
        }

        public async Task<Result<AuthResponse?>> RefreshAsync(RefreshTokenRequest request)
        {
            var storedToken = await _refreshTokenRepository.GetByTokenAsync(request.RefreshToken);

            if (storedToken == null || storedToken.IsRevoked || storedToken.ExpiresAt < DateTime.UtcNow)
                return Result<AuthResponse?>.Failure(
                    Error.Validation("Refresh.Invalid", "Invalid refresh token"));

            var user = await _userRepository.GetByIdAsync(storedToken.UserId);

            if (user == null)
                return Result<AuthResponse?>.Failure(
                    Error.Validation("Refresh.Invalid", "Invalid user"));

            // rotate refresh token (BEST PRACTICE)
            storedToken.IsRevoked = true;

            var newRefreshToken = new RefreshToken
            {
                Id = Guid.NewGuid(),
                Token = _refreshTokenGenerator.Generate(),
                UserId = user.Id,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                IsRevoked = false
            };

            await _refreshTokenRepository.AddAsync(newRefreshToken);
            await _refreshTokenRepository.SaveChangesAsync();

            var newAccessToken = _jwtTokenGenerator.GenerateToken(user);

            return Result<AuthResponse?>.Success(new AuthResponse
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken.Token
            });
        }

    }
}
