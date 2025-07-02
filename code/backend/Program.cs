using backend.Data;
using backend.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Npgsql;
using System.Text; // 确保已安装 Npgsql 包

Console.Clear(); // 清除控制台所有内容
var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
Console.WriteLine($"🔗 实际连接字符串: {connectionString}");

// 注册 CORS 服务（可命名策略或使用默认策略）
builder.Services.AddCors(options => {
options.AddPolicy("AllowFrontend", builder => {
builder
    .WithOrigins("http://localhost:8080", "http://localhost:5173") // 允许的前端域名/端口（开发环境）
    .AllowAnyMethod() // 允许所有 HTTP 方法（GET/POST 等）
    .AllowAnyHeader(); // 允许所有请求头
});

// 开发环境临时允许所有来源（生产环境不建议）
// options.AddPolicy("AllowAll", builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

// 添加JWT配置
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
            ValidateIssuer = false, // 简化开发，生产环境建议开启
            ValidateAudience = false
        };
    });
// 添加配置
builder.Configuration.AddJsonFile("appsettings.json");
try
{
    using var connection = new NpgsqlConnection(connectionString);
    await connection.OpenAsync(); // 尝试连接
    Console.WriteLine("数据库连接成功！");
}
catch (Exception ex)
{
    Console.WriteLine($"数据库连接失败: {ex.Message}");
    // 直接终止应用启动（可选）
    throw;
}

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<DatabaseService>(); // 注册 DatabaseService

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
// 在中间件管道中启用 CORS（需在 UseAuthorization 之前）
app.UseCors("AllowFrontend"); // 使用注册的策略名称
app.UseHttpsRedirection();
app.UseAuthentication(); // 添加认证中间件
app.UseAuthorization();
app.MapControllers();

app.Run();
