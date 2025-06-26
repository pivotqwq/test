using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore.PostgreSQL.Query.Expressions.Internal;
using System.ComponentModel.DataAnnotations;
using static backend.Data.ApplicationDbContext;

namespace backend.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }

        public class patients
        {
            public string? id { get; set; }
            public string? medical_record_no { get; set; }
            public string? name { get; set; }
            public string? gender { get; set; }
            public string? birth_date { get; set; }
            public string? address { get; set; }
        }
        public DbSet<patients> Patients { get; set; }
        public class Memo
        {
            public int id { get; set; }
            public string title { get; set; }
            public string content { get; set; }
            public DateTime createdAt { get; set; } = DateTime.Now;
        }
        public DbSet<Memo> Memos { get; set; }
    }

    public class User
    {
        [Key]
        [MaxLength(36)]  // UUID 字符串长度为36
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public string? username { get; set; }
        public string? password { get; set; }
        public string? email { get; set; }
        public string? urlBase64 { get; set; }
        public string? phone { get; set; }
        public string? name {get; set; }
        public string? profession { get; set; }
    }

    
}
