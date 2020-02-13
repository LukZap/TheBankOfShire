using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShireBank.Models
{
    public class BankContext : DbContext
    {
        public DbSet<Account> Accounts { get; set; }
        public DbSet<AccountHistory> AccountHistories { get; set; }

        public BankContext(DbContextOptions<BankContext> options) : base(options)
        {
        }

        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    modelBuilder.Entity<AccountHistory>()
        //        .HasOne<Account>()
        //        .WithMany()
        //        .HasForeignKey(a => a.AccountId)
        //        .IsRequired()
        //        .OnDelete(DeleteBehavior.Cascade);

        //    base.OnModelCreating(modelBuilder);
        //}
    }
}
