using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using DAL.DTO.Req;
using DAL.DTO.Req.Auth;
using DAL.DTO.Res.Auth;
using DAL.DTO.Res.User;
using DAL.Models;
using DAL.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace DAL.Repository.Services
{
    public class UserService(CreditContext context, IConfiguration configuration) : IUserServices
    {
        private readonly CreditContext _context = context;
        private readonly IConfiguration _configuration = configuration;

        private string GenerateJwtToken(MstUser user)
        {
            try
            {

                var jwtOptions = _configuration.GetSection("jwtSettings");
                var secretKey = jwtOptions["secretKey"];
                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
                var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

                var claims = new[]
                {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

                var token = new JwtSecurityToken(
                    issuer: jwtOptions["validIssuer"],
                    audience: jwtOptions["validAudience"],
                    claims: claims,
                    expires: DateTime.Now.AddHours(3),
                    signingCredentials: credentials
                    );

                return new JwtSecurityTokenHandler().WriteToken(token);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task<ResLoginDto> LoginUserAsync(ReqLoginDto bodyUser)
        {
            try
            {

                MstUser user = await _context.MstUsers.SingleOrDefaultAsync(x => x.Email == bodyUser.email) ?? throw new Exception("Email or Password is Wrong!");

                bool isPasswordValid = BCrypt.Net.BCrypt.Verify(bodyUser.pass, user.Pass);

                if (!isPasswordValid)
                {
                    throw new Exception("Email or Password is Wrong!");
                }

                var token = GenerateJwtToken(user);

                var loginResp = new ResLoginDto
                {
                    id = user.Id,
                    email = user.Email,
                    name = user.Name,
                    role = user.Role,
                    balance = user.Balance,
                    token = token,
                };

                return loginResp;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task<ResDto> GetUserAsync(string userId)
        {
            try
            {

                MstUser user = await _context.MstUsers.Where(x => x.Id == userId).SingleOrDefaultAsync() ?? throw new Exception("User not found!");

                var newUser = new ResDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    Name = user.Name,
                    Role = user.Role,
                    Balance = user.Balance
                };

                return newUser;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}
