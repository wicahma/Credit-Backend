using DAL.DTO.Res;
using System.Text;
using DAL.Repository.Interface;
using DAL.Repository.Services;
using Microsoft.AspNetCore.Mvc;
using DAL.DTO.Req;
using DAL.DTO.Res.User;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace BECredit.Controllers
{
    [Route("rest/v1/auth")]
    [ApiController]
    public class AuthController(IUserServices userServices, IAdminServices adminServices) : ControllerBase
    {
        private readonly IUserServices _userServices = userServices;
        private readonly IAdminServices _adminServices = adminServices;

        [HttpPost]
        [Route("register-admin")]
        public async Task<IActionResult> Register(ReqRegisterAdminDto register)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Where(x => x.Value.Errors.Any())
                        .Select(x =>
                        new {
                            Field = x.Key,
                            Messages = x.Value.Errors.Select(e => e.ErrorMessage).ToList()
                        }).ToList();
                    var errorMessage = new StringBuilder("Validate errors occured!");

                    return BadRequest(new ResBaseDto<object>
                    {
                        Success = false,
                        Message = errorMessage.ToString(),
                        Data = errors
                    });
                }
                var res = await _adminServices.CreateAdminAsync(register);

                return Ok(new ResBaseDto<string>
                {
                    Success = true,
                    Message = "User Registered!",
                    Data = res,
                });
            }
            catch (Exception ex)
            {
                if (ex.Message == "Email already used!")
                {
                    return BadRequest(new ResBaseDto<object>
                    {
                        Success = false,
                        Message = ex.Message,
                        Data = null,
                    });
                }
                return StatusCode(StatusCodes.Status500InternalServerError, new ResBaseDto<string>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null,
                });
            };
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login(ReqLoginDto login)
        {
            try
            {
                var res = await _userServices.LoginUserAsync(login);
                return Ok(new ResBaseDto<object>
                {
                    Success = true,
                    Message = "User Logged in!",
                    Data = res,
                });
            }
            catch (Exception ex)
            {
                if (ex.Message == "Email or Password is Wrong!")
                {
                    return BadRequest(new ResBaseDto<object>
                    {
                        Success = false,
                        Message = ex.Message,
                        Data = null,
                    });
                }
                return StatusCode(StatusCodes.Status500InternalServerError, new ResBaseDto<string>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null,
                });
            };
        }

        [HttpGet]
        [Route("user")]
        [Authorize]
        public async Task<IActionResult> GetUser([FromQuery] string id)
        {
            try
            {
                var lenderId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new Exception("Lender not found in token!");
                var role = User.FindFirst(ClaimTypes.Role)?.Value ?? throw new Exception("Role not found in token!");

                var res = await _userServices.GetUserAsync(role != "admin" ? lenderId : id);

                return Ok(new ResBaseDto<ResDto>
                {
                    Success = true,
                    Message = "User found!",
                    Data = res,
                });
            }
            catch (Exception ex)
            {
                if (ex.Message == "User not found!")
                {
                    return BadRequest(new ResBaseDto<object>
                    {
                        Success = false,
                        Message = ex.Message,
                        Data = null,
                    });
                }
                return StatusCode(StatusCodes.Status500InternalServerError, new ResBaseDto<string>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null,
                });
            };
        }

    }
}
