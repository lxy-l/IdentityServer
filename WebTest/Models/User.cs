using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WebTest.Models
{
    public class User
    {
        /// <summary>
        /// 主键
        /// </summary>
        [Key]
        [Column("Uid")]
        public int Id { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        [Required]
        [StringLength(255)]
        public string Name { get; set; }
        
        /// <summary>
        /// 密码
        /// </summary>
        [Required]
        [StringLength(255)]
        public string PasswordHash { get; set; }

        /// <summary>
        /// 并发标记
        /// </summary>
        [Timestamp]
        public byte[] RowVersion { get; set; }

        /// <summary>
        /// 一个用户多个角色
        /// </summary>
        public virtual List<UserRole> UserRoles { get; set; }
    }
}
