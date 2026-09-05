using GameRankingApi.Data;
using GameRankingApi.DTOs;
using GameRankingApi.Models;
using Microsoft.EntityFrameworkCore;

namespace GameRankingApi.Endpoints;

public static class ScoreEndpoints
{
    public static void MapScoreEndpoints(this WebApplication app)
    {
        // 获取排行榜
        app.MapGet("/api/score", async (AppDbContext db) =>
        {
            var sorted = await db.PlayerScores
                .OrderByDescending(x => x.Score)
                .ToListAsync();

            return Results.Ok(sorted);
        })
        .WithSummary("获取排行榜")
        .WithDescription("获取所有玩家成绩，并按照分数从高到低排序。");

        // 根据游戏名称获取排行榜
        app.MapGet("/api/score/game/{gameName}", async (string gameName, AppDbContext db) =>
        {
            var scores = await db.PlayerScores
                .Where(x => x.GameName == gameName)
                .OrderByDescending(x => x.Score)
                .ToListAsync();

            return Results.Ok(scores);
        })
        .WithSummary("根据游戏名称获取排行榜")
        .WithDescription("查询指定游戏的所有玩家成绩，并按照分数从高到低排序。");

        // 根据玩家名称查询成绩
        app.MapGet("/api/score/player/{playerName}", async (string playerName, AppDbContext db) =>
        {
            var scores = await db.PlayerScores
                .Where(x => x.PlayerName == playerName)
                .OrderByDescending(x => x.Score)
                .ToListAsync();

            return Results.Ok(scores);
        })
        .WithSummary("根据玩家名称查询成绩")
        .WithDescription("查询指定玩家的所有游戏成绩，并按照分数从高到低排序。");

        // 获取 Top N 排行榜
        app.MapGet("/api/score/top/{count}", async (int count, AppDbContext db) =>
        {
            if (count <= 0)
            {
                return Results.BadRequest("数量必须大于 0");
            }

            var scores = await db.PlayerScores
                .OrderByDescending(x => x.Score)
                .Take(count)
                .ToListAsync();

            return Results.Ok(scores);
        })
        .WithSummary("获取 Top N 排行榜")
        .WithDescription("获取分数最高的前 N 名玩家成绩。");

        // 根据 ID 获取成绩
        app.MapGet("/api/score/{id}", async (int id, AppDbContext db) =>
        {
            var score = await db.PlayerScores.FindAsync(id);

            if (score == null)
            {
                return Results.NotFound();
            }

            return Results.Ok(score);
        })
        .WithSummary("根据 ID 获取成绩")
        .WithDescription("根据玩家成绩记录的 ID 查询具体成绩信息。");

        // 添加玩家成绩
        app.MapPost("/api/score", async (PlayerScoreRequest request, AppDbContext db) =>
        {
            var newScore = new PlayerScore
            {
                PlayerName = request.PlayerName,
                Score = request.Score,
                GameName = request.GameName
            };

            if (string.IsNullOrWhiteSpace(newScore.PlayerName))
            {
                return Results.BadRequest("玩家名称不能为空");
            }

            if (newScore.Score < 0)
            {
                return Results.BadRequest("分数不能小于 0");
            }

            if (string.IsNullOrWhiteSpace(newScore.GameName))
            {
                return Results.BadRequest("游戏名称不能为空");
            }

            db.PlayerScores.Add(newScore);
            await db.SaveChangesAsync();

            return Results.Created($"/api/score/{newScore.Id}", newScore);
        })
        .WithSummary("添加玩家成绩")
        .WithDescription("添加一条新的玩家游戏成绩记录，并保存到 SQLite 数据库。");

        // 修改玩家成绩
        app.MapPut("/api/score/{id}", async (int id, PlayerScoreRequest request, AppDbContext db) =>
        {
            var score = await db.PlayerScores.FindAsync(id);

            if (score == null)
            {
                return Results.NotFound();
            }

            if (string.IsNullOrWhiteSpace(request.PlayerName))
            {
                return Results.BadRequest("玩家名称不能为空");
            }

            if (request.Score < 0)
            {
                return Results.BadRequest("分数不能小于 0");
            }

            if (string.IsNullOrWhiteSpace(request.GameName))
            {
                return Results.BadRequest("游戏名称不能为空");
            }

            score.PlayerName = request.PlayerName;
            score.Score = request.Score;
            score.GameName = request.GameName;

            await db.SaveChangesAsync();

            return Results.Ok(score);
        })
        .WithSummary("修改玩家成绩")
        .WithDescription("根据成绩记录 ID 修改玩家名称、分数和游戏名称。");

        // 删除玩家成绩
        app.MapDelete("/api/score/{id}", async (int id, AppDbContext db) =>
        {
            var score = await db.PlayerScores.FindAsync(id);

            if (score == null)
            {
                return Results.NotFound();
            }

            db.PlayerScores.Remove(score);
            await db.SaveChangesAsync();

            return Results.NoContent();
        })
        .WithSummary("删除玩家成绩")
        .WithDescription("根据成绩记录 ID 删除指定的玩家成绩。");
    }
}