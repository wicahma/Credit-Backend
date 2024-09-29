using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.DTO.Req;
using DAL.DTO.Req.User;
using DAL.DTO.Res;
using DAL.DTO.Res.User;

namespace DAL.Repository.Interface
{
    public interface IUserServices
    {
        Task<ResLoginDto> LoginUserAsync(ReqLoginDto bodyUser);
        Task<ResDto> GetUserAsync(string userId);

    }
}
