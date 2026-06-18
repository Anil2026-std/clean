using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces.IServices
{
    public interface IJwtTokenGenerator
    {
        string GenerateToken(User user);
    }
    public interface IRefreshTokenGenerator
    {
        string Generate();
    }
}
