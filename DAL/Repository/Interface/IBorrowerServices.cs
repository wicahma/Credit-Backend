using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.DTO.Req.Loan;
using DAL.DTO.Res.Borrower;
using DAL.DTO.Res.Funding;
using DAL.DTO.Res.Loan;
using DAL.DTO.Res.Repayment;
using DAL.DTO.Res.User;

namespace DAL.Repository.Interface
{
    public interface IBorrowerServices
    {
        Task<String> CreateLoan(ReqCreateLoan bodyBorrower, string borrowerId);
        Task<List<ResGetBorrowerHistory>> GetBorrowerHistory(string type = "all");
        Task<ResGetFundingDetail> GetFundingDetail(string loanId);
        Task<ResGetRepaymentDetail> GetRepaymentDetail(string loanId);
        Task<ResPayLoan> PayLoan(ReqPayLoan bodyLoan, string borrowerId, string loanId);
        Task<string> DeleteLoan(string loanId);

    }
}
