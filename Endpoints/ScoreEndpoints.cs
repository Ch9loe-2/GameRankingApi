using GameRankingApi.DTOs;
using GameRankingApi.Services;

namespace GameRankingApi.Endpoints;

public static class ScoreEndpoints
{
    public static void MapScoreEndpoints(this WebApplication app)
    {
        // 获取排行榜
        app.MapGet("/api/score", async (ScoreService service) =>
        {
            var scores = await service.GetAllAsync();

            return Results.Ok(scores);
        })
        .WithSummary("获取排行榜")
        .WithDescription("获取所有玩家成绩，并按照分数从高到低排序。");

        // 根据 ID 获取成绩
        app.MapGet("/api/score/{id}", async (int id, ScoreService service) =>
        {
            var score = await service.GetByIdAsync(id);

            if (score == null)
            {
                return Results.NotFound();
            }

            return Results.Ok(score);
        })
        .WithSummary("根据 ID 获取成绩")
        .WithDescription("根据玩家成绩记录的 ID 查询具体成绩信息。");

        // 根据游戏名称获取排行榜
app.MapGet("/api/score/game/{gameName}", async (string gameName, ScoreService service) =>
{
    var scores = await service.GetByGameNameAsync(gameName);

    return Results.Ok(scores);
})
.WithSummary("根据游戏名称获取排行榜")
.WithDescription("查询指定游戏的所有玩家成绩，并按照分数从高到低排序。");

       // 根据玩家名称查询成绩
app.MapGet("/api/score/player/{playerName}", async (string playerName, ScoreService service) =>
{
    var scores = await service.GetByPlayerNameAsync(playerName);

    return Results.Ok(scores);
})
.WithSummary("根据玩家名称查询成绩")
.WithDescription("查询指定玩家的所有游戏成绩，并按照分数从高到低排序。");

        // 获取 Top N 排行榜
app.MapGet("/api/score/top/{count}", async (int count, ScoreService service) =>
{
    if (count <= 0)
    {
        return Results.BadRequest("数量必须大于 0");
    }

    var scores = await service.GetTopAsync(count);

    return Results.Ok(scores);
})
.WithSummary("获取 Top N 排行榜")
.WithDescription("获取分数最高的前 N 名玩家成绩。");

        

        // 添加玩家成绩
app.MapPost("/api/score", async (PlayerScoreRequest request, ScoreService service) =>
{
    try
    {
        var newScore = await service.CreateAsync(request);

        return Results.Created($"/api/score/{newScore.Id}", newScore);
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(ex.Message);
    }
})
.WithSummary("添加玩家成绩")
.WithDescription("添加一条新的玩家游戏成绩记录，并保存到 SQLite 数据库。");

        // 修改玩家成绩
app.MapPut("/api/score/{id}", async (int id, PlayerScoreRequest request, ScoreService service) =>
{
    try
    {
        var score = await service.UpdateAsync(id, request);

        if (score == null)
        {
            return Results.NotFound();
        }

        return Results.Ok(score);
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(ex.Message);
    }
})
.WithSummary("修改玩家成绩")
.WithDescription("根据成绩记录 ID 修改玩家名称、分数和游戏名称。");

        // 删除玩家成绩
app.MapDelete("/api/score/{id}", async (int id, ScoreService service) =>
{
    var deleted = await service.DeleteAsync(id);

    if (!deleted)
    {
        return Results.NotFound();
    }

    return Results.NoContent();
})
.WithSummary("删除玩家成绩")
.WithDescription("根据成绩记录 ID 删除指定的玩家成绩。");
    }
}