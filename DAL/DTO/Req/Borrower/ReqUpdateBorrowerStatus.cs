using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.DTO.Req.Borrower
{
    public interface ReqUpdateBorrowerStatus
    {
        [Required(ErrorMessage = "Status is required")]
        [RegularExpression("^(requested|funded|repaid)$", ErrorMessage = "The status must be either 'repaid', 'requested' or 'funded'.")]
        [MaxLength(10, ErrorMessage = "Status can't be more than 10 characters")]
        public string status { get; set; }

    }
}
