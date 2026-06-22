using Application.DTOs;
using Application.Interfaces.IRepo;
using Application.Interfaces.IServices;
using Application.Services.AuthServices;
using Domain.Entities;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;

namespace UnitTests.Login
{
    public class LoginTests
    {

        [Fact]
        public async Task LoginAsync_ShouldFail_WhenUserDoesNotExist()
        {
            var mockUserRepo = new Mock<IUserRepository>();
            var mockJwt = new Mock<IJwtTokenGenerator>();
            var mockRefreshGen = new Mock<IRefreshTokenGenerator>();
            var mockRefreshRepo = new Mock<IRefreshTokenRepository>();

            mockUserRepo.Setup(x => x.GetByUsernameAsync("test"))
                        .ReturnsAsync((User?)null);

            var service = new AuthService(
                mockUserRepo.Object,
                mockJwt.Object,
                mockRefreshGen.Object,
                mockRefreshRepo.Object);

            var result = await service.LoginAsync(new LoginRequest
            {
                Username = "test",
                Password = "123"
            });

            Assert.False(result.IsSuccess);
            Assert.NotNull(result.Error);

            // No token generation should happen
            mockJwt.Verify(x => x.GenerateToken(It.IsAny<User>()), Times.Never);
        }

        [Fact]
        public async Task LoginAsync_ShouldFail_WhenPasswordIsInvalid()
        {
            var user = new User
            {
                Id = Guid.NewGuid(),
                Username = "test",
                PasswordHash = "correct"
            };

            var mockUserRepo = new Mock<IUserRepository>();
            var mockJwt = new Mock<IJwtTokenGenerator>();
            var mockRefreshGen = new Mock<IRefreshTokenGenerator>();
            var mockRefreshRepo = new Mock<IRefreshTokenRepository>();

            mockUserRepo.Setup(x => x.GetByUsernameAsync("test"))
                        .ReturnsAsync(user);

            var service = new AuthService(
                mockUserRepo.Object,
                mockJwt.Object,
                mockRefreshGen.Object,
                mockRefreshRepo.Object);

            var result = await service.LoginAsync(new LoginRequest
            {
                Username = "test",
                Password = "wrong"
            });

            Assert.False(result.IsSuccess);

            mockJwt.Verify(x => x.GenerateToken(It.IsAny<User>()), Times.Never);
        }

        [Fact]
        public async Task LoginAsync_ShouldReturnTokens_WhenCredentialsAreValid()
        {
            var user = new User
            {
                Id = Guid.NewGuid(),
                Username = "test",
                PasswordHash = "123"
            };

            var mockUserRepo = new Mock<IUserRepository>();
            var mockJwt = new Mock<IJwtTokenGenerator>();
            var mockRefreshGen = new Mock<IRefreshTokenGenerator>();
            var mockRefreshRepo = new Mock<IRefreshTokenRepository>();

            mockUserRepo.Setup(x => x.GetByUsernameAsync("test"))
                        .ReturnsAsync(user);

            mockJwt.Setup(x => x.GenerateToken(user))
                   .Returns("access-token");

            mockRefreshGen.Setup(x => x.Generate())
                          .Returns("refresh-token");

            var service = new AuthService(
                mockUserRepo.Object,
                mockJwt.Object,
                mockRefreshGen.Object,
                mockRefreshRepo.Object);

            var result = await service.LoginAsync(new LoginRequest
            {
                Username = "test",
                Password = "123"
            });

            Assert.True(result.IsSuccess);
            Assert.Equal("access-token", result.Data.AccessToken);
            Assert.Equal("refresh-token", result.Data.RefreshToken);

            // Critical verifications
            mockRefreshRepo.Verify(x => x.AddAsync(It.IsAny<RefreshToken>()), Times.Once);
            mockRefreshRepo.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task RefreshAsync_ShouldFail_WhenTokenNotFound()
        {
            var mockRepo = new Mock<IRefreshTokenRepository>();
            var mockUserRepo = new Mock<IUserRepository>();

            mockRepo.Setup(x => x.GetByTokenAsync("invalid"))
                    .ReturnsAsync((RefreshToken?)null);

            var service = new AuthService(
                mockUserRepo.Object,
                Mock.Of<IJwtTokenGenerator>(),
                Mock.Of<IRefreshTokenGenerator>(),
                mockRepo.Object);

            var result = await service.RefreshAsync(new RefreshTokenRequest
            {
                RefreshToken = "invalid"
            });

            Assert.False(result.IsSuccess);
        }

        [Fact]
        public async Task RefreshAsync_ShouldRotateToken_WhenValid()
        {
            var userId = Guid.NewGuid();

            var storedToken = new RefreshToken
            {
                Token = "old",
                UserId = userId,
                ExpiresAt = DateTime.UtcNow.AddDays(1),
                IsRevoked = false
            };

            var user = new User { Id = userId };

            var mockRepo = new Mock<IRefreshTokenRepository>();
            var mockUserRepo = new Mock<IUserRepository>();
            var mockJwt = new Mock<IJwtTokenGenerator>();
            var mockRefreshGen = new Mock<IRefreshTokenGenerator>();

            mockRepo.Setup(x => x.GetByTokenAsync("old"))
                    .ReturnsAsync(storedToken);

            mockUserRepo.Setup(x => x.GetByIdAsync(userId))
                        .ReturnsAsync(user);

            mockJwt.Setup(x => x.GenerateToken(user))
                   .Returns("new-access");

            mockRefreshGen.Setup(x => x.Generate())
                          .Returns("new-refresh");

            var service = new AuthService(
                mockUserRepo.Object,
                mockJwt.Object,
                mockRefreshGen.Object,
                mockRepo.Object);

            var result = await service.RefreshAsync(new RefreshTokenRequest
            {
                RefreshToken = "old"
            });

            Assert.True(result.IsSuccess);
            Assert.Equal("new-access", result.Data.AccessToken);
            Assert.Equal("new-refresh", result.Data.RefreshToken);

            // Critical: old token revoked
            Assert.True(storedToken.IsRevoked);

            mockRepo.Verify(x => x.AddAsync(It.IsAny<RefreshToken>()), Times.Once);
        }

        [Fact]
        public async Task RefreshAsync_ShouldFail_WhenTokenIsExpired()
        {
            // Arrange
            var expiredToken = new RefreshToken
            {
                Token = "token",
                UserId = Guid.NewGuid(),
                ExpiresAt = DateTime.UtcNow.AddDays(-1),
                IsRevoked = false
            };

            var mockRepo = new Mock<IRefreshTokenRepository>();
            var mockUserRepo = new Mock<IUserRepository>();

            mockRepo.Setup(x => x.GetByTokenAsync("token"))
                    .ReturnsAsync(expiredToken);

            var service = new AuthService(
                mockUserRepo.Object,
                Mock.Of<IJwtTokenGenerator>(),
                Mock.Of<IRefreshTokenGenerator>(),
                mockRepo.Object);

            // Act
            var result = await service.RefreshAsync(new RefreshTokenRequest
            {
                RefreshToken = "token"
            });

            // Assert
            Assert.False(result.IsSuccess);
            Assert.NotNull(result.Error);
        }
    }
}
