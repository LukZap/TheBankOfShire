using Microsoft.EntityFrameworkCore;
using ShireBank.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShireBank.Utils
{
    public static class BankContextOptionsFactory
    {
        public static DbContextOptions<BankContext> Build()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["SqliteConnectionString"].ConnectionString;

            var builder = new DbContextOptionsBuilder<BankContext>();
            builder.UseSqlite(connectionString);

            return builder.Options;
        }
    }
}
