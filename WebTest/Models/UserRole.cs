using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WebTest.Models
{
    public class UserRole
    {
        [Key]
        public int Id { get; set; }
        public int Uid { get; set; }
        [ForeignKey("Uid")]
        public User User { get; set; }
        
        public int Rid { get; set; }
        [ForeignKey("Rid")]
        public Role Role { get; set; }
    }
}
