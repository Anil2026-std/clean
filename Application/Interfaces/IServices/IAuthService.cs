using Application.DTOs;
using Domain.comman;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces.IServices
{
    public interface IAuthService
    {
        Task<Result<AuthResponse?>> LoginAsync(LoginRequest request);
        Task<Result<AuthResponse?>> RefreshAsync(RefreshTokenRequest request);
    }
}
