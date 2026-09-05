using GameRankingApi.Data;
using GameRankingApi.DTOs;
using GameRankingApi.Models;
using GameRankingApi.Endpoints;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=gameranking.db"));

var app = builder.Build();

app.UseExceptionHandler("/error");

app.MapOpenApi();
app.UseSwagger();
app.UseSwaggerUI();

app.MapScoreEndpoints();

// 默认首页
app.MapGet("/", () => "后端已启动！请在浏览器地址栏手动输入 /api/score");

app.Map("/error", () =>
{
    return Results.Problem(
        statusCode: 500,
        title: "服务器内部发生错误",
        detail: "服务器处理请求时发生了未预期的错误。"
    );
});

app.Run();