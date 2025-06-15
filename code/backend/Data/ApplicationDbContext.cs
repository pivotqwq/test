using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace backend.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
    }

    public class User
    {
        [Key]
        [MaxLength(36)]  // UUID 字符串长度为36
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public string username { get; set; }
        public string password { get; set; }
        public string email { get; set; }
        public string? urlBase64 { get; set; }
        public string? phone { get; set; }
    }
}
