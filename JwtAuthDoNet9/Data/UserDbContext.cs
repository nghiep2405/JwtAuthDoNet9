using Microsoft.EntityFrameworkCore;
using JwtAuthDoNet9.Entities;
namespace JwtAuthDoNet9.Data
{
    public class UserDbContext(DbContextOptions<UserDbContext> options) : DbContext(options)
    {
        public DbSet<User> Users { get; set; }  // create sql table
    }   
}