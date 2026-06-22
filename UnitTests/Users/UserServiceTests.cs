using Application.DTOs;
using Application.Interfaces.IRepo;
using Application.Services.UserServices;
using Domain.Entities;
using FluentValidation;
using Moq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace UnitTests.Users
{
    public class UserServiceTests
    {



        [Fact]
        public async Task GetUserByIdAsync_ShouldReturnSuccess_WhenUserExists()
        {
            var userId = Guid.NewGuid();

            var mockRepo = new Mock<IUserRepository>();
            var mockValidator = new Mock<IValidator<CreateUserDto>>();

            mockRepo.Setup(x => x.GetByIdAsync(userId))
                    .ReturnsAsync(new User { Id = userId, Username = "test", PasswordHash = "hashedpassword" });

            var service = new UserService(mockRepo.Object, mockValidator.Object);

            var result = await service.GetUserByIdAsync(userId);

            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.Equal(userId, result.Data.Id);
        }

        [Fact]
        public async Task GetUserByIdAsync_ShouldReturnFailure_WhenUserDoesNotExist()
        {
            var userId = Guid.NewGuid();

            var mockRepo = new Mock<IUserRepository>();
            var mockValidator = new Mock<IValidator<CreateUserDto>>();

            mockRepo.Setup(x => x.GetByIdAsync(userId))
                    .ReturnsAsync((User?)null);

            var service = new UserService(mockRepo.Object, mockValidator.Object);

            var result = await service.GetUserByIdAsync(userId);

            Assert.False(result.IsSuccess);
            Assert.NotNull(result.Error);
        }

        [Fact]
        public async Task CreateUser_ShouldReturnValidationError_WhenValidationFails()
        {
            // Arrange
            var dto = new CreateUserDto { Username = "", Password = "" };

            var mockRepo = new Mock<IUserRepository>();
            var mockValidator = new Mock<IValidator<CreateUserDto>>();

            var failures = new List<FluentValidation.Results.ValidationFailure>
            {
                new("Username", "Username is required")
            };

            mockValidator.Setup(v => v.ValidateAsync(dto, default))
                         .ReturnsAsync(new FluentValidation.Results.ValidationResult(failures));

            var service = new UserService(mockRepo.Object, mockValidator.Object);

            // Act
            var result = await service.CreateUser(dto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.NotNull(result.Error);

            // IMPORTANT: Repo should NOT be called
            mockRepo.Verify(r => r.CreateUser(It.IsAny<User>()), Times.Never);
        }
        [Fact]
        public async Task CreateUser_ShouldCreateUser_WhenValidationPasses()
        {
            var dto = new CreateUserDto
            {
                Username = "test",
                Password = "123"
            };

            var mockRepo = new Mock<IUserRepository>();
            var mockValidator = new Mock<IValidator<CreateUserDto>>();

            mockValidator.Setup(v => v.ValidateAsync(dto, default))
                         .ReturnsAsync(new FluentValidation.Results.ValidationResult());

            var userId = Guid.NewGuid();

            mockRepo.Setup(r => r.CreateUser(It.IsAny<User>()))
                    .ReturnsAsync(userId);

            var service = new UserService(mockRepo.Object, mockValidator.Object);

            var result = await service.CreateUser(dto);

            Assert.True(result.IsSuccess);
            Assert.Equal(userId, result.Data.Id);

            // Verify repo was called once
            mockRepo.Verify(r => r.CreateUser(It.IsAny<User>()), Times.Once);
        }

        #region Theory test 


        //Use [Theory] only when:
        //✔ same setup
        //✔ same logic path

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task GetUserByIdAsync_ShouldReturnExpectedResult(bool userExists)
        {
            var userId = Guid.NewGuid();

            var mockRepo = new Mock<IUserRepository>();
            var mockValidator = new Mock<IValidator<CreateUserDto>>();

            User? user = null;

            if (userExists)
            {
                user = new User
                {
                    Id = userId,
                    Username = "test",
                    PasswordHash = "hashedpassword"
                };
            }

            mockRepo.Setup(x => x.GetByIdAsync(userId))
                    .ReturnsAsync(user);

            var service = new UserService(mockRepo.Object, mockValidator.Object);

            var result = await service.GetUserByIdAsync(userId);

            if (userExists)
            {
                Assert.True(result.IsSuccess);
                Assert.NotNull(result.Data);
                Assert.Equal(userId, result.Data.Id);
            }
            else
            {
                Assert.False(result.IsSuccess);
                Assert.NotNull(result.Error);
            }
        }


        [Theory]
        [InlineData("", "123")]
        [InlineData("test", "")]
        [InlineData("", "")]
        public async Task CreateUser_ShouldFail_WhenInputIsInvalid(string username, string password)
        {
            var dto = new CreateUserDto
            {
                Username = username,
                Password = password
            };

            var mockRepo = new Mock<IUserRepository>();
            var mockValidator = new Mock<IValidator<CreateUserDto>>();

            mockValidator.Setup(v => v.ValidateAsync(dto, default))
                         .ReturnsAsync(new FluentValidation.Results.ValidationResult(
                             new List<FluentValidation.Results.ValidationFailure>
                             {
                         new("Username", "Invalid input")
                             }));

            var service = new UserService(mockRepo.Object, mockValidator.Object);

            var result = await service.CreateUser(dto);

            Assert.False(result.IsSuccess);

            mockRepo.Verify(r => r.CreateUser(It.IsAny<User>()), Times.Never);
        }

        #endregion



        #region class Data
        public class CreateUserInvalidData : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                yield return new object[] { "", "123" };
                yield return new object[] { "test", "" };
                yield return new object[] { "", "" };
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        [Theory]
        [ClassData(typeof(CreateUserInvalidData))]
        public async Task CreateUser_ShouldFail_WhenInputIsInvalid_classData(string username, string password)
        {
            var dto = new CreateUserDto
            {
                Username = username,
                Password = password
            };

            var mockRepo = new Mock<IUserRepository>();
            var mockValidator = new Mock<IValidator<CreateUserDto>>();

            mockValidator.Setup(v => v.ValidateAsync(It.IsAny<CreateUserDto>(), default))
                         .ReturnsAsync(new FluentValidation.Results.ValidationResult(
                             new List<FluentValidation.Results.ValidationFailure>
                             {
                                 new("Username", "Invalid input")
                             }));

            var service = new UserService(mockRepo.Object, mockValidator.Object);

            var result = await service.CreateUser(dto);

            Assert.False(result.IsSuccess);

            mockRepo.Verify(r => r.CreateUser(It.IsAny<User>()), Times.Never);
        }



        #endregion

    }
}