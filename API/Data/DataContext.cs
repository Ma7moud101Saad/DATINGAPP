using API.Entites;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System;
using System.Diagnostics.Contracts;

namespace API.Data
{
    public class DataContext:DbContext
    {
        public DataContext(DbContextOptions<DataContext> options)
    : base(options)
        {
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        }
        public DbSet<AppUser>Users { get; set; }
        public DbSet<UserLike> UserLike { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserLike>().HasKey(u => new { u.SourceUserId,u.TargetUserId});

            modelBuilder.Entity<UserLike>()
              .HasOne(x => x.SourceUser)
              .WithMany(c => c.LikedUsers)
              .HasForeignKey(x => x.SourceUserId)
              .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UserLike>()
             .HasOne(x => x.TargetUser)
             .WithMany(c => c.LikedByUsers)
             .HasForeignKey(x => x.TargetUserId)
             .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
