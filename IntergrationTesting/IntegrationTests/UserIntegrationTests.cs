using Application.DTOs;
using CleanArchitecture.Contracts;
using IntergrationTesting.CustomWebAppFactorys;
using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;

namespace IntergrationTesting.IntegrationTests
{
    public class UserIntegrationTests : IClassFixture<CustomWebAppFactory>
    {
        private readonly HttpClient _client;

        public UserIntegrationTests(CustomWebAppFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task CreateUser_ShouldReturnSuccess()
        {
            var request = new CreateUserDto
            {
                Username = "johns",
                Password = "Test@123"
            };

            var response = await _client.PostAsJsonAsync("/api/user", request);

            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<ApiResponse<UserDto>>();

            Assert.NotNull(result);
            Assert.Equal("johns", result.Data.Username);
        }
        [Fact]
        public async Task GetUser_ShouldReturnUser_WhenExists()
        {
            // Arrange - create first
            var createResponse = await _client.PostAsJsonAsync("/api/user",
                new CreateUserDto
                {
                    Username = "johny",
                    Password = "Test@123"
                });

            var created = await createResponse.Content.ReadFromJsonAsync<ApiResponse<UserDto>>();

            // Act
            var response = await _client.GetAsync($"/api/User/getbyid?id={created.Data.Id}");

            // Assert
            response.EnsureSuccessStatusCode();

            var user = await response.Content.ReadFromJsonAsync<ApiResponse<UserDto>>();

            Assert.Equal("johny", user.Data.Username);
        }

        [Fact]
        public async Task CreateUser_ShouldFail_WhenUsernameAlreadyExists()
        {
            await _client.PostAsJsonAsync("/api/user", new CreateUserDto
            {
                Username = "john",
                Password = "Test@123"
            });

            var response = await _client.PostAsJsonAsync("/api/user", new CreateUserDto
            {
                Username = "john",
                Password = "Test@456"
            });

            Assert.False(response.IsSuccessStatusCode);
        }
    }
}
