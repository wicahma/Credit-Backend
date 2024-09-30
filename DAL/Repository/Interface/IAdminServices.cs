using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.DTO.Req.Auth;
using DAL.DTO.Req.User;
using DAL.DTO.Res.User;

namespace DAL.Repository.Interface
{
    public interface IAdminServices
    {
        Task<string> UpdateUserAsync(ReqUpdateDto bodyUser, string userId);
        Task<string> CreateAdminAsync(ReqRegisterAdminDto bodyUser);
        Task<string> DeleteUserAsync(string userId);
        Task<string> CreateUserAsync(ReqCreateDto bodyUser);
        Task<List<ResDto>> GetUsersAsync();
    }
}
