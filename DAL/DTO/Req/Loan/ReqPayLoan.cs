using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.DTO.Req.Loan
{
    public class ReqPayLoan
    {
        [Required(ErrorMessage = "Month Paid is required!")]
        [Range(0, double.MaxValue, ErrorMessage = "Month Paid must be a positive number!")]
        public int monthPaid { get; set; }
    }
}
