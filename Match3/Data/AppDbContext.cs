using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Match3.Models;
using Microsoft.EntityFrameworkCore;

namespace Match3.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string dbPath = Path.Combine(FileSystem.AppDataDirectory, "match3.db");
            optionsBuilder.UseSqlite($"Filename={dbPath}");
        }

        public DbSet<Player> Players { get; set; }
        public DbSet<HighScore> HighScores { get; set; }
    }
}
