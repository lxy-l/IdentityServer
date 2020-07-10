using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using WebTest.Models;

namespace WebTest.Data
{
    public class UserContext:DbContext
    {
        public UserContext() : base(){ }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // 从 appsetting.json 中获取配置信息
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();
            if (Convert.ToBoolean(config["ConnectionStrings:UseMysql"]))
            {
                optionsBuilder.UseMySql(config.GetConnectionString("MysqlConnection"));
            }
            else
            {
                optionsBuilder.UseSqlServer(config.GetConnectionString("SQLServerConnection"));
            }

        }
        public DbSet<User> Users{ get; set; }
        public DbSet<Role> Roles{ get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
