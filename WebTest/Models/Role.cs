using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WebTest.Models
{
    public class Role
    {
        /// <summary>
        /// 主键
        /// </summary>
        [Key]
        [Column("Rid")]
        public int Id { get; set; }
        /// <summary>
        /// 角色名
        /// </summary>
        [Required]
        [StringLength(255)]
        public string RoleName { get; set; }

        /// <summary>
        /// 一个角色多个用户
        /// </summary>
        public virtual ICollection<User> Users { get; set; }
    }
}
