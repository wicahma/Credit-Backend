using System.Security.Claims;
using DAL.DTO.Res;
using DAL.Repository.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BECredit.Controllers
{
    [Route("rest/v1")]
    [ApiController]
    public class LenderController(ILenderServices lenderServices, IBorrowerServices borrowerServices) : ControllerBase
    {
        private readonly ILenderServices _lenderServices = lenderServices;
        private readonly IBorrowerServices _borrowerServices = borrowerServices;

        [HttpPost]
        [Route("lender")]
        [Authorize(Roles = "admin,lender")]

        public async Task<IActionResult> UpdateBalance([FromQuery] int amount)
        {
            try
            {
                var lenderId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new Exception("Lender not found in token!");
                var res = await _lenderServices.UpdateBalance(lenderId, amount);

                return Ok(new ResBaseDto<string>
                {
                    Success = true,
                    Message = res,
                    Data = null,
                });
            }
            catch (Exception ex)
            {
                if (ex.Message == "Lender not found!")
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
            }
        }
    }
}
