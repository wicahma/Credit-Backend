using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.DTO.Res.Borrower;
using DAL.DTO.Res.Lender;
using DAL.DTO.Res.User;
using DAL.Models;
using DAL.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace DAL.Repository.Services
{
    public class LenderService(CreditContext context) : ILenderServices
    {
        private readonly CreditContext _context = context;
        public async Task<string> AcceptBorrower(string loanId, string lenderId)
        {
            try
            {
                MstLoans loan = _context.MstLoans.Where(x => x.Id == loanId).FirstOrDefault() ?? throw new Exception("Loan not found!");
                
                if (loan.Status != "requested") throw new Exception("Loan already accepted!");

                MstUser lender = _context.MstUsers.Where(x => x.Id == lenderId).FirstOrDefault() ?? throw new Exception("Lender not found!");

                if (lender.Role != "lender") throw new Exception("User is not a lender!");

                if (loan.Amount > lender.Balance) throw new Exception("User balance is insufficient!");

                lender.Balance -= loan.Amount;
                loan.Status = "funded";
                loan.UpdatedAt = DateTime.UtcNow;
                MstUser borrower = _context.MstUsers.Where(x => x.Id == loan.BorrowerId).FirstOrDefault() ?? throw new Exception("Borrower not found!");
                borrower.Balance += loan.Amount;

                TrnFunding funding = new()
                {
                    LenderId = lender.Id,
                    LoanId = loan.Id,
                    Amount = loan.Amount,
                    FundedAt = DateTime.UtcNow
                };

                await _context.TrnFundings.AddAsync(funding);

                _context.MstLoans.Update(loan);
                _context.MstUsers.Update(lender);
                _context.MstUsers.Update(borrower);

                if (await _context.SaveChangesAsync() > 0) return "Loan accepted succesfully!";
                else throw new Exception("Failed to update loan data!");
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task<List<ResGetAllBorrower>> GetAllBorrower()
        {
            try
            {
                return await _context.MstLoans
                    .Include(x => x.User)
                    .Where(x => x.Status == "requested")
                    .Select(loan => new ResGetAllBorrower
                    {
                        Id = loan.Id,   
                        Name = loan.User.Name,
                        Amount = loan.Amount,
                        Interest = loan.InterestRate,
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

        public async Task<List<ResGetLenderHistory>> GetHistoryFunding(string lenderId)
        {
           try
            {
                MstUser lender = _context.MstUsers.Where(x => x.Id == lenderId).FirstOrDefault() ?? throw new Exception("Lender not found!");
                
                return await _context.TrnFundings
                    .Include(x => x.Loans)
                    .ThenInclude(ln => ln.User)
                    .Include(x => x.User)
                    .Where(x => x.LenderId == lender.Id)
                    .Select(funding => new ResGetLenderHistory
                    {
                        Id = funding.Id,
                        LoanId = funding.LoanId,
                        BorrowerName = funding.Loans.User.Name,
                        Amount = funding.Amount,
                        Status = funding.Loans.Status,
                        Duration = funding.Loans.Duration,
                        CreatedAt = funding.Loans.CreatedAt,
                        FundedAt = funding.FundedAt
                    }).ToListAsync();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task<string> UpdateBalance(string lenderId, decimal amount)
        {
            try
            {
                MstUser lender = _context.MstUsers.Where(x => x.Id == lenderId).FirstOrDefault() ?? throw new Exception("Lender not found!");

                lender.Balance += amount;

                _context.MstUsers.Update(lender);

                if (await _context.SaveChangesAsync() > 0) return "Balance updated succesfully!";
                else throw new Exception("Failed to update balance data!");
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}
