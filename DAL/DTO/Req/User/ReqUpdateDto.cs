using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.DTO.Req.User
{
    public class ReqUpdateDto
    {
        [Required(ErrorMessage = "Name is required")]
        [MaxLength(30, ErrorMessage = "Name can't be more than 30 characters")]
        public string name { get; set; }

        [Required(ErrorMessage = "Role is required")]
        [RegularExpression("^(lender|borrower)$", ErrorMessage = "The role must be either 'lender' or 'borrower'.")]
        [MaxLength(30, ErrorMessage = "Role can't be more than 30 characters")]
        public string role { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Balance must be a positive number")]
        public decimal? balance { get; set; }
    }
}
