using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.DTO.Res.Funding
{
    public class ResGetFundingDetail
    {
        public string Id { get; set; }
        public string LenderName { get; set; }
        public string Status { get; set; }
        public decimal Amount { get; set; }
        public decimal InterestRate { get; set; }
        public int Duration { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime FundedAt { get; set; }
    }
}
