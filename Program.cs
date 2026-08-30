var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// 初始化一条“假数据”
var playerScores = new List<PlayerScore>()
{
    new PlayerScore
    {
        Id = 1,
        PlayerName = "示例玩家",
        Score = 1000,
        GameName = "测试游戏"
    }
};

// 获取排行榜
app.MapGet("/api/score", () =>
{
    var sorted = playerScores.OrderByDescending(x => x.Score).ToList();
    return Results.Ok(sorted);
});

// 根据 ID 获取成绩
app.MapGet("/api/score/{id}", (int id) =>
{
    var score = playerScores.FirstOrDefault(x => x.Id == id);

    if (score == null)
    {
        return Results.NotFound();
    }

    return Results.Ok(score);
});

// 添加玩家成绩
app.MapPost("/api/score", (PlayerScore newScore) =>
{
    newScore.Id = playerScores.Count == 0
        ? 1
        : playerScores.Max(x => x.Id) + 1;

    playerScores.Add(newScore);

    return Results.Ok(newScore);
});

// 修改玩家成绩
app.MapPut("/api/score/{id}", (int id, PlayerScore updatedScore) =>
{
    var score = playerScores.FirstOrDefault(x => x.Id == id);

    if (score == null)
    {
        return Results.NotFound();
    }

    score.PlayerName = updatedScore.PlayerName;
    score.Score = updatedScore.Score;
    score.GameName = updatedScore.GameName;

    return Results.Ok(score);
});

// 删除玩家成绩
app.MapDelete("/api/score/{id}", (int id) =>
{
    var score = playerScores.FirstOrDefault(x => x.Id == id);

    if (score == null)
    {
        return Results.NotFound();
    }

    playerScores.Remove(score);

    return Results.NoContent();
});

// 默认首页
app.MapGet("/", () => "后端已启动！请在浏览器地址栏手动输入 /api/score");

app.Run();

public class PlayerScore
{
    public int Id { get; set; }
    public string PlayerName { get; set; } = string.Empty;
    public int Score { get; set; }
    public string GameName { get; set; } = string.Empty;
}