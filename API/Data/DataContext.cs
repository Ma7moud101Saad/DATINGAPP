using API.Entites;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System;
using System.Diagnostics.Contracts;

namespace API.Data
{
    public class DataContext: IdentityDbContext<AppUser, AppRole, int,
        IdentityUserClaim<int>, AppUserRole, IdentityUserLogin<int>,
        IdentityRoleClaim<int>, IdentityUserToken<int>>
    {
        public DataContext(DbContextOptions<DataContext> options)
    : base(options)
        {
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        }
        public DbSet<UserLike> UserLike { get; set; }
        public DbSet<Message> Message { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<AppUser>()
               .HasMany(c => c.UserRoles)
               .WithOne(x => x.AppUser)
               .HasForeignKey(x => x.UserId)
               .IsRequired();


            modelBuilder.Entity<AppRole>()
               .HasMany(c => c.UserRoles)
               .WithOne(x => x.AppRole)
               .HasForeignKey(x => x.RoleId)
               .IsRequired();

            modelBuilder.Entity<UserLike>().HasKey(u => new { u.SourceUserId,u.TargetUserId});

            modelBuilder.Entity<UserLike>()
              .HasOne(x => x.SourceUser)
              .WithMany(c => c.LikedUsers)
              .HasForeignKey(x => x.SourceUserId)
              .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserLike>()
             .HasOne(x => x.TargetUser)
             .WithMany(c => c.LikedByUsers)
             .HasForeignKey(x => x.TargetUserId)
             .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Message>()
            .HasOne(x => x.Sender)
            .WithMany(c => c.MessagesSent)
            .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Message>()
           .HasOne(x => x.Recipient)
           .WithMany(c => c.MessagesReceived)
           .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
