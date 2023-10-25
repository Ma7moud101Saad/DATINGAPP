using API.Entites;
using Microsoft.EntityFrameworkCore;
using System;

namespace API.Data
{
    public class DataContext:DbContext
    {
        public DataContext(DbContextOptions<DataContext> options)
    : base(options)
        { }
        public DbSet<AppUser>Users { get; set; }
    }
}
