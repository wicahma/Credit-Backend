using System.Security.Claims;
using DAL.DTO.Req.Loan;
using DAL.DTO.Res;
using DAL.DTO.Res.Borrower;
using DAL.DTO.Res.Lender;
using DAL.DTO.Res.Loan;
using DAL.Repository.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BECredit.Controllers
{
    [Route("rest/v1")]
    [ApiController]
    public class LoanController(ILenderServices lenderServices, IBorrowerServices borrowerServices) : ControllerBase
    {
        private readonly ILenderServices _lenderServices = lenderServices;
        private readonly IBorrowerServices _borrowerServices = borrowerServices;

        [HttpDelete]
        [Route("loan")]
        [Authorize(Roles = "admin,borrower")]
        public async Task<IActionResult> DeleteLoan([FromQuery] string id)
        {
            try
            {
                var res = await _borrowerServices.DeleteLoan(id);

                return Ok(new ResBaseDto<string>
                {
                    Success = true,
                    Message = res,
                    Data = null,
                });
            }
            catch (Exception ex)
            {
                if (ex.Message == "Loan not found!" || ex.Message == "Loan already accepted, you cannot delete it!")
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

        [HttpPost]
        [Route("loan")]
        [Authorize(Roles = "admin,borrower")]
        public async Task<IActionResult> CreateLoan([FromBody] ReqCreateLoan bodyBorrower)
        {
            try
            {
                var borrowerId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new Exception("Borrower not found in token!");
                var res = await _borrowerServices.CreateLoan(bodyBorrower, borrowerId);

                return Ok(new ResBaseDto<string>
                {
                    Success = true,
                    Message = res,
                    Data = null,
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
            }
        }

        [HttpPut]
        [Route("loan/pay")]
        [Authorize(Roles = "admin,borrower")]
        public async Task<IActionResult> PayLoan([FromBody] ReqPayLoan body, [FromQuery] string loanId)
        {
            try
            {
                var borrowerId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new Exception("Borrower not found in token!");

                var res = await _borrowerServices.PayLoan(body, borrowerId, loanId);

                return Ok(new ResBaseDto<ResPayLoan>
                {
                    Success = true,
                    Message = "Loan paid!",
                    Data = res,
                });
            }
            catch (Exception ex)
            {
                if (ex.Message == "Loan not found!" || ex.Message == "You are not authorized to pay this loan!" || ex.Message == "Loan already paid!" || ex.Message == "Invalid month paid!")
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
        [Route("loan/borrower-history")]
        [Authorize(Roles = "admin,borrower")]
        public async Task<IActionResult> GetBorrowerHistory([FromQuery] string type)
        {
            try
            {
                var res = await _borrowerServices.GetBorrowerHistory(type);

                return Ok(new ResBaseDto<List<ResGetBorrowerHistory>>
                {
                    Success = true,
                    Message = "Borrower history found!",
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
            }
        }

        [HttpGet]
        [Route("loan/accept")]
        [Authorize(Roles = "admin,lender")]
        public async Task<IActionResult> AcceptLoan([FromQuery] string loanId)
        {
            try
            {
                var lenderId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new Exception("Lender not found in token!");

                var res = await _lenderServices.AcceptBorrower(loanId, lenderId);

                return Ok(new ResBaseDto<string>
                {
                    Success = true,
                    Message = res,
                    Data = null,
                });
            }
            catch (Exception ex)
            {
                if (ex.Message == "Loan not found!" || ex.Message == "Loan already accepted!" || ex.Message == "Lender not found!" || ex.Message == "User is not a lender!" || ex.Message == "User balance is insufficient!" || ex.Message == "Borrower not found!")
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
        [Route("loan/requested")]
        [Authorize(Roles = "admin,lender")]
        public async Task<IActionResult> GetAllBorrower()
        {
            try
            {
                var res = await _lenderServices.GetAllBorrower();

                return Ok(new ResBaseDto<List<ResGetAllBorrower>>
                {
                    Success = true,
                    Message = "All borrower found!",
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
            }
        }
    }
}
