﻿using Microsoft.EntityFrameworkCore;

namespace CsvToDb
{
    public class MagazinesContext : DbContext
    {
        public DbSet<Magazine> Magazines { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySQL("server=localhost;database=Magazines;user=root;password=root");
        }
    }
}
