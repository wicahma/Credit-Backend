using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.DTO.Req.User
{
    public class ReqCreateDto
    {
        [Required(ErrorMessage = "Name is required")]
        [MaxLength(30, ErrorMessage = "Name can't be more than 30 characters")]
        public string name { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [MaxLength(50, ErrorMessage = "Email can't be more than 50 characters")]
        public string email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters")]
        [MaxLength(50, ErrorMessage = "Password can't be more than 50 characters")]
        public string pass { get; set; } = "password1";

        [Required(ErrorMessage = "Role is required")]
        [RegularExpression("^(lender|borrower)$", ErrorMessage = "The role must be either 'lender' or 'borrower'.")]
        [MaxLength(10, ErrorMessage = "Role can't be more than 30 characters")]
        public string role { get; set; }
    }
}
