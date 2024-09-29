using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    [Table("trn_repayment")]
    public class TrnRepayment
    {
        [Key]
        [Column("id")]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        
        [Required]
        [ForeignKey("Loans")]
        [Column("loan_id")]
        public string LoanId { get; set; }

        [Column("amount")]
        public decimal Amount { get; set; }

        [Column("repaid_amount")]
        public decimal RepaidAmount { get; set; }

        [Column("balance_amount")]
        public decimal BalanceAmount { get; set; }

        [Column("repaid_status")]
        public string RepaidStatus { get; set; }

        [Column("paid_at")]
        public DateTime PaidAt { get; set; }

        public MstLoans Loans { get; set; }
    }
}
