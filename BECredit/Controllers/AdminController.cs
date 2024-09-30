using DAL.DTO.Req;
using DAL.DTO.Res;
using System.Text;
using DAL.Repository.Interface;
using Microsoft.AspNetCore.Mvc;
using DAL.DTO.Req.User;
using Microsoft.AspNetCore.Authorization;
using DAL.DTO.Res.User;

namespace BECredit.Controllers
{
    [Route("rest/v1")]
    [ApiController]
    [Authorize(Roles = "admin")]
    public class AdminController(IAdminServices adminServices) : ControllerBase
    {
        private readonly IAdminServices _adminServices = adminServices;

        [HttpPost]
        [Route("admin")]
        public async Task<IActionResult> CreateUser(ReqCreateDto userBody)
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

                var res = await _adminServices.CreateUserAsync(userBody);

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

        [HttpPut]
        [Route("admin")]
        public async Task<IActionResult> UpdateUser(ReqUpdateDto userBody, [FromQuery] string id)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Where(x => x.Value.Errors.Any())
                        .Select(x =>
                        new
                        {
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

                var res = await _adminServices.UpdateUserAsync(userBody, id);

                return Ok(new ResBaseDto<string>
                {
                    Success = true,
                    Message = "User Registered!",
                    Data = res,
                });
            }
            catch (Exception ex)
            {
                if (ex.Message == "User not found!" || ex.Message == "Fail to create new user!")
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
        [Route("admin/users")]
        public async Task<IActionResult> GetUsers()
        {
            try
            {
                var res = await _adminServices.GetUsersAsync();

                return Ok(new ResBaseDto<List<ResDto>>
                {
                    Success = true,
                    Message = "Users found!",
                    Data = res,
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ResBaseDto<string>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null,
                });
            };
        }

        [HttpDelete]
        [Route("admin")]
        public async Task<IActionResult> DeleteUser([FromQuery] string id)
        {
            try
            {
                var res = await _adminServices.DeleteUserAsync(id);

                return Ok(new ResBaseDto<string>
                {
                    Success = true,
                    Message = "User deleted!",
                    Data = res,
                });
            }
            catch (Exception ex)
            {
                if (ex.Message == "User not found!" || ex.Message == "Cannot delete admin!")
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
