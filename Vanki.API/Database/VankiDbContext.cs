using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata;
using Vanki.API.Models;

namespace Vanki.API.Database
{
    public class VankiDbContext : DbContext
    {
        public DbSet<Card> Cards { get; set; }
        public DbSet<Deck> Decks { get; set; }

        public string DbPath { get; }

        public VankiDbContext(DbContextOptions<VankiDbContext> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
