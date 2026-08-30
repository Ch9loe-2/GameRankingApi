var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// 初始化一条“假数据”让你直接看（就好像玩家已经提交过分数了）
var playerScores = new List<PlayerScore>()
{
    new PlayerScore { PlayerName = "示例玩家", Score = 1000, GameName = "测试游戏" }
};

/// 获取排行榜的接口
app.MapGet("/api/score", () =>
{
    var sorted = playerScores.OrderByDescending(x => x.Score).ToList();
    return Results.Ok(sorted);
});

// 添加玩家成绩
app.MapPost("/api/score", (PlayerScore newScore) =>
{
    playerScores.Add(newScore);
    return Results.Ok(newScore);
});

// 默认首页
app.MapGet("/", () => "后端已启动！请在浏览器地址栏手动输入 /api/score");

app.Run();

public class PlayerScore
{
    public string PlayerName { get; set; } = string.Empty;
    public int Score { get; set; }
    public string GameName { get; set; } = string.Empty;
}