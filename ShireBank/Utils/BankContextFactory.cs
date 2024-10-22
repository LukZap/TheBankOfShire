﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using ShireBank.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShireBank.Utils
{
    public class BankContextFactory : IDesignTimeDbContextFactory<BankContext>
    {
        public BankContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<BankContext>();
            // TODO find way to use config value here
            builder.UseSqlite(@"Filename=C:\Users\lukasz.zaparucha\Documents\TheBankOfShire\ShireBank\ShireBank.db"); 

            return new BankContext(builder.Options);
        }
    }
}
