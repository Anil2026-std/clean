using Application.DTOs;
using Application.Interfaces.IRepo;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;
        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetByIdAsync(Guid id)
        {

            return await _context.Users.FirstOrDefaultAsync(x => x.Id == id);

            //return await _context.Users
            //  .Where(x => x.Id == id)
            //  .Select(x => new UserDto
            //  {
            //      Id = x.Id,
            //      Username = x.Username,
            //  })
            //   .FirstOrDefaultAsync();
        }

        public async Task<User?> GetByUsernameAsync(string username)
        {
            return await _context.Users.FirstOrDefaultAsync(x => x.Username == username);
        }

        public async Task<Guid> CreateUser(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            return user.Id;
        }



        public async Task<IEnumerable<UserDto>> GetListUser()
        {
            return await _context.Users
                   .Select(u => new UserDto
                   {
                       Id = u.Id,
                       Username = u.Username,
                   }).ToListAsync();
        }
    }
}
