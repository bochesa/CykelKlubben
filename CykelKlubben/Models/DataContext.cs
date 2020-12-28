using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CykelKlubben.Models
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }
        //protected override void OnModelCreating(ModelBuilder builder)
        //{
        //    builder.Entity<byte[]>()
        //        .HasMany(c => c.Products)
        //        .WithOne(p => p.Category)
        //        .HasForeignKey(p => p.CategoryId);
        //}
        public DbSet<Bicycle> Bicycles { get; set; }
        public DbSet<Experience> Experiences { get; set; }
        public DbSet<Picture> ExperiencePictures { get; set; }
    }
}
