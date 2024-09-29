using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    [Table("mst_user")]

    public partial class MstUser
    {
        [Key]
        [Column("id")]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        [Column("name")]
        public string Name { get; set; } = null!;
        [Column("email")]
        public string Email { get; set; } = null!;
        [Column("pass")]
        public string Pass { get; set; } = null!;
        [Column("role")]
        public string Role { get; set; } = null!;
        [Column("balance")]
        public decimal? Balance { get; set; }

        public List<MstLoans> MstLoans { get; set; } = new List<MstLoans>();
    }
}
