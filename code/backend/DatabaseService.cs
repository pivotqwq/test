using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace backend.Services
{
    public class DatabaseService
    {
        private readonly backend.Data.ApplicationDbContext _context;

        public DatabaseService(backend.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<string> GetCurrentDatabaseNameAsync()
        {
            // 使用 FromSqlRaw 查询当前数据库名称
            var databaseName = await _context.Users
                .FromSqlRaw("SELECT current_database()")
                .Select(u => u.username) // 假设 Users 表有一个 username 字段
                .FirstOrDefaultAsync();

            return databaseName;
        }
    }
}
