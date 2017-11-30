using ConsoleAppCore.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleAppCore.DB
{
    public class ConfigurationContext : DbContext
    {
        public ConfigurationContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<ConfigurationValue> Values { get; set; }
    }
}
