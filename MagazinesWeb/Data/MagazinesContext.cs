using MagazinesWeb.Models;
using Microsoft.EntityFrameworkCore;

namespace MagazinesWeb.Data
{
    public class MagazinesContext : DbContext
    {
        public MagazinesContext(DbContextOptions<MagazinesContext> options) : base(options)
        {
        }

        public DbSet<Magazine> Magazines { get; set; }
    }
}
