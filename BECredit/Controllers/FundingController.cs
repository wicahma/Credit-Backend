using System.Security.Claims;
using DAL.DTO.Res;
using DAL.DTO.Res.Funding;
using DAL.DTO.Res.Lender;
using DAL.Repository.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BECredit.Controllers
{
    [Route("rest/v1")]
    [ApiController]
    public class FundingController(ILenderServices lenderServices, IBorrowerServices borrowerServices) : ControllerBase
    {
        private readonly ILenderServices _lenderServices = lenderServices;
        private readonly IBorrowerServices _borrowerServices = borrowerServices;

        [HttpGet]
        [Route("funding/detail")]
        [Authorize]
        public async Task<IActionResult> GetFundingDetail([FromQuery] string loanId)
        {
            try
            {
                var res = await _borrowerServices.GetFundingDetail(loanId);

                return Ok(new ResBaseDto<ResGetFundingDetail>
                {
                    Success = true,
                    Message = "Funding Detail",
                    Data = res,
                });
            }
            catch (Exception ex)
            {
                if (ex.Message == "Funding not found!")
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

        [HttpGet]
        [Route("funding/lender-history")]
        [Authorize(Roles = "admin,lender")]
        public async Task<IActionResult> GetLenderFundingHistory()
        {
            try
            {
                var lenderId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new Exception("Lender not found in token!");

                var res = await _lenderServices.GetHistoryFunding(lenderId);

                return Ok(new ResBaseDto<List<ResGetLenderHistory>>
                {
                    Success = true,
                    Message = "Lender Funding History",
                    Data = res,
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
