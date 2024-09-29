using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.DTO.Req.Loan;
using DAL.DTO.Res.Funding;
using DAL.DTO.Res.Lender;
using DAL.DTO.Res.Loan;
using DAL.DTO.Res.Repayment;
using DAL.DTO.Res.User;
using DAL.Models;
using DAL.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace DAL.Repository.Services
{
    public class BorrowerService(CreditContext context, IConfiguration configuration) : IBorrowerServices
    {
        private readonly CreditContext _context = context;
        private readonly IConfiguration _configuration = configuration;

        public async Task<string> CreateLoan(ReqCreateLoan bodyBorrower, string borrowerId)
        {
            Console.WriteLine("CreateLoan");
            Console.WriteLine(borrowerId);
            try
            {
                MstLoans loan = new()
                {
                    Amount = bodyBorrower.amount,
                    BorrowerId = borrowerId,
                    Duration = bodyBorrower.duration,
                    InterestRate = bodyBorrower.interest,
                    Status = "requested",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                };

                await _context.MstLoans.AddAsync(loan);
                return await _context.SaveChangesAsync() > 0 ?
                    "Loan created succesfully!" :
                    throw new Exception("Failed to create loan data!");
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task<List<ResGetBorrowerHistory>> GetBorrowerHistory(string type = "all")
        {
            try
            {
                return await _context.MstLoans
                    .Include(x => x.User)
                    .Where(x => type == x.Status || type == "all")
                    .OrderByDescending(x => x.UpdatedAt)
                    .Select(loan => new ResGetBorrowerHistory
                    {
                        Id = loan.Id,
                        BorrowerName = loan.User.Name,
                        Amount = loan.Amount,
                        InterestRate = loan.InterestRate,
                        Duration = loan.Duration,
                        Status = loan.Status,
                        CreatedAt = loan.CreatedAt,
                        UpdatedAt = loan.UpdatedAt
                    }).ToListAsync();

            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public Task<ResGetFundingDetail> GetFundingDetail(string loanId)
        {
            try
            {
                TrnFunding funding = _context.TrnFundings
                    .Include(x => x.Loans)
                    .Include(x => x.User)
                    .Where(x => x.LoanId == loanId)
                    .SingleOrDefault() ?? throw new Exception("Funding not found!");

                return Task.FromResult(new ResGetFundingDetail
                {
                    Id = funding.Id,
                    Status = funding.Loans.Status,
                    Amount = funding.Amount,
                    LenderName = funding.User.Name,
                    Duration = funding.Loans.Duration,
                    CreatedAt = funding.Loans.CreatedAt,
                    FundedAt = funding.FundedAt,
                    InterestRate = funding.Loans.InterestRate,
                });
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task<ResGetRepaymentDetail> GetRepaymentDetail(string loanId)
        {
            try
            {
                TrnFunding funding = _context.TrnFundings
                    .Include(x => x.Loans)
                    .Include(x => x.User)
                    .Where(x => x.LoanId == loanId)
                    .SingleOrDefault() ?? throw new Exception("Funding not found!");

                List<TrnRepayment> repayment = await _context.TrnRepayments
                    .Include(x => x.Loans)
                    .Where(x => x.LoanId == loanId)
                    .OrderByDescending(x => x.PaidAt).ToListAsync();

                return await Task.FromResult(new ResGetRepaymentDetail
                {
                    Amount = funding.Amount,
                    Duration = funding.Loans.Duration,
                    InterestRate = funding.Loans.InterestRate,
                    FundedAt = funding.FundedAt,
                    LastStatus = funding.Loans.Status,
                    RepaymentHistory = repayment.Select(x => new RepaymentHistory
                    {
                        Id = x.Id,
                        Amount = x.Amount,
                        RepaidAmount = x.RepaidAmount,
                        BalanceAmount = x.BalanceAmount,
                        RepaidStatus = x.RepaidStatus,
                        PaidAt = x.PaidAt
                    }).ToList()
                });

                throw new NotImplementedException();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task<ResPayLoan> PayLoan(ReqPayLoan bodyLoan, string borrowerId, string loanId)
        {
            try
            {
                MstLoans loan = _context.MstLoans
                    .Include(x => x.User)
                    .Where(x => x.Id == loanId)
                    .SingleOrDefault() ?? throw new Exception("Loan not found!");

                if (loan.BorrowerId != borrowerId) throw new Exception("You are not authorized to pay this loan!");

                List<TrnRepayment> oldRepayment = await _context.TrnRepayments
                    .Include(x => x.Loans)
                    .Where(x => x.LoanId == loanId)
                    .OrderByDescending(x => x.PaidAt).ToListAsync();

                if (oldRepayment.Count >= loan.Duration) throw new Exception("Loan already paid!");
                if (oldRepayment.Count + bodyLoan.monthPaid > loan.Duration) throw new Exception("Invalid month paid!");

                loan.Status = "repaid";
                decimal interest = (loan.InterestRate / 100);
                decimal interestAmount = interest * loan.Amount;
                decimal totalDebt = loan.Amount + interestAmount;
                decimal monthlyRepayment = totalDebt / loan.Duration;

                for (int i = oldRepayment.Count; i < oldRepayment.Count + bodyLoan.monthPaid; i++)
                {
                    decimal balanceAmount = loan.Amount - (monthlyRepayment * (i + 1));
                    string repaidStatus = balanceAmount <= 0 ? "done" : "on repay";
                    
                    if (balanceAmount <= 0) loan.Status = "done";

                    TrnRepayment repayment = new()
                    {
                        LoanId = loanId,
                        Amount = totalDebt,
                        RepaidAmount = monthlyRepayment,
                        BalanceAmount = balanceAmount,
                        RepaidStatus = repaidStatus,
                        PaidAt = DateTime.UtcNow
                    };
                    await _context.TrnRepayments.AddAsync(repayment);
                }

                _context.Update(loan);

                if (await _context.SaveChangesAsync() <= 0) throw new Exception("Failed to pay loan!");

                return await Task.FromResult(new ResPayLoan
                {
                    RemainDebt = loan.Amount - (monthlyRepayment * bodyLoan.monthPaid),
                });
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task<string> DeleteLoan(string loanId)
        {
            try
            {
                MstLoans loan = await _context.MstLoans.SingleOrDefaultAsync(x => x.Id == loanId) ?? throw new Exception("Loan not found!");

                if (loan.Status != "requested") throw new Exception("Loan already accepted, you cannot delete it!");

                _context.MstLoans.Remove(loan);

                return await _context.SaveChangesAsync() > 0 ? "Loan deleted succesfully!" : throw new Exception("Failed to delete loan!");
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}
