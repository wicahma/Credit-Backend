using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.DTO.Res.Borrower;
using DAL.DTO.Res.Lender;
using DAL.DTO.Res.User;

namespace DAL.Repository.Interface
{
    public interface ILenderServices
    {
        Task<string> UpdateBalance(string lenderId, decimal amount);

        // tabahapnnya mengambil data dari tabel loan yang statusnya requested, dan return dalam bentuk list
        Task<List<ResGetAllBorrower>> GetAllBorrower();
        
        // tahapannya update status jadi funded, cek saldo lender apakah kurang atau lebih, update saldo lender dan borrower, update ke database terkait (tabel loan dan funding)
        Task<string> AcceptBorrower(string loanId, string lenderId);

        // tahapannya get data dari tabel funding, terus populate data loan pake loan id, dan return semua datanya dalam bentuk list
        Task<List<ResGetLenderHistory>> GetHistoryFunding(string lenderId);
    }
}
