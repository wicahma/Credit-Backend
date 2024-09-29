
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using DAL.DTO.Req;
using DAL.DTO.Req.User;
using DAL.DTO.Res;
using DAL.DTO.Res.User;
using DAL.Models;
using DAL.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Win32;

namespace DAL.Repository.Services
{
    public class AdminService(CreditContext context, IConfiguration configuration) : IAdminServices
    {
        private readonly CreditContext _context = context;
        private readonly IConfiguration _configuration = configuration;

        public async Task<string> CreateAdminAsync(ReqRegisterAdminDto bodyUser)
        {
            try
            {
                var isAnyEmail = await _context.MstUsers.SingleOrDefaultAsync(x => x.Email == bodyUser.email);
                if (isAnyEmail != null)
                {
                    throw new Exception("Email already used!");
                }
                var user = new MstUser
                {
                    Name = bodyUser.name,
                    Email = bodyUser.email,
                    Pass = BCrypt.Net.BCrypt.HashPassword(bodyUser.pass),
                    Role = "admin",
                    Balance = 0,
                };

                await _context.MstUsers.AddAsync(user);
                return await _context.SaveChangesAsync() > 0 ? "Admin created succesfully!" : throw new Exception("Failed to Create admin data!");
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task<string> CreateUserAsync(ReqCreateDto bodyUser)
        {
            try
            {
                var isAnyEmail = await _context.MstUsers.SingleOrDefaultAsync(x => x.Email == bodyUser.email);
                if (isAnyEmail != null)
                {
                    throw new Exception("Email already used!");
                }
                var user = new MstUser
                {
                    Name = bodyUser.name,
                    Email = bodyUser.email,
                    Pass = BCrypt.Net.BCrypt.HashPassword(bodyUser.pass),
                    Role = bodyUser.role,
                    Balance = 0,
                };

                await _context.MstUsers.AddAsync(user);
                return await _context.SaveChangesAsync() > 0 ? "User created succesfully!" : throw new Exception("Failed to create user data!");
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task<string> DeleteUserAsync(string userId)
        {
            try
            {
                MstUser user = await _context.MstUsers.SingleOrDefaultAsync(e => e.Id == userId) ?? throw new Exception("User not found!");

                _context.MstUsers.Remove(user);

                return await _context.SaveChangesAsync() > 0 ? "User deleted succesfully!" : throw new Exception("Failed to delete user!");
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task<List<ResDto>> GetUsersAsync()
        {
            try
            {

                return await _context.MstUsers.Select(user => new ResDto
                {
                    Id = user.Id,
                    Name = user.Name,
                    Email = user.Email,
                    Role = user.Role,
                    Balance = user.Balance
                }).ToListAsync();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task<string> UpdateUserAsync(ReqUpdateDto bodyUser, string userId)
        {
            try
            {
                MstUser user = await _context.MstUsers.SingleOrDefaultAsync(x => x.Id == userId) ?? throw new Exception("User not found!");
                user.Name = bodyUser.name;
                user.Role = bodyUser.role;
                user.Balance = bodyUser.balance ?? 0;
                MstUser? newUser = _context.MstUsers.Update(user).Entity;
                _context.SaveChanges();

                return newUser == null ? throw new Exception("Fail to create new user!") : "User Created Succesfully!";
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}
