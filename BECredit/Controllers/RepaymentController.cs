using DAL.DTO.Res;
using DAL.DTO.Res.Repayment;
using DAL.Repository.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BECredit.Controllers
{
    [Route("rest/v1")]
    [ApiController]
    public class RepaymentController(ILenderServices lenderServices, IBorrowerServices borrowerServices) : ControllerBase
    {
        private readonly ILenderServices _lenderServices = lenderServices;
        private readonly IBorrowerServices _borrowerServices = borrowerServices;

        [HttpGet]
        [Route("repayment/detail")]
        [Authorize]
        public async Task<IActionResult> GetRepaymentDetail([FromQuery] string loanId)
        {
            try
            {
                var res = await _borrowerServices.GetRepaymentDetail(loanId);

                return Ok(new ResBaseDto<ResGetRepaymentDetail>
                {
                    Success = true,
                    Message = "Repayment Detail",
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
    }
}
