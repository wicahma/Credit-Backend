using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.DTO.Req.Loan
{
    public class ReqCreateLoan
    {
        [Required(ErrorMessage = "Amount is required!")]
        [Range(0, double.MaxValue, ErrorMessage = "Amount must be a positive number!")]
        public decimal amount { get; set; }

        [Required(ErrorMessage = "Amount is required!")]
        [Range(0, double.MaxValue, ErrorMessage = "Interest must be a positive number!")]
        public decimal interest { get; set; } = 2.5m;

        [Range(0, double.MaxValue, ErrorMessage = "Duration must be a positive number that represent month!")]
        public int duration { get; set; } = 12;

    }
}
