using GameRankingApi.Data;
using GameRankingApi.DTOs;
using GameRankingApi.Models;
using GameRankingApi.Endpoints;
using Microsoft.EntityFrameworkCore;
using GameRankingApi.Services;
using GameRankingApi.Common;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=gameranking.db"));
builder.Services.AddScoped<ScoreService>();
builder.Services.AddLogging();

var app = builder.Build();

// 启动时自动创建数据库（不存在时）
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.EnsureCreatedAsync();
}

app.UseExceptionHandler("/error");

app.MapOpenApi();
app.UseSwagger();
app.UseSwaggerUI();

app.MapScoreEndpoints();

// 默认首页
app.MapGet("/", () => "后端已启动！请在浏览器地址栏手动输入 /api/score");

app.Map("/error", () =>
{
    return Results.Json(
        new ApiResponse<object>(
            500,
            "服务器内部发生错误",
            null
        ),
        statusCode: 500
    );
});

app.Run();
public partial class Program
{
}