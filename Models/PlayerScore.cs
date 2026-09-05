namespace GameRankingApi.Models;

public class PlayerScore
{
    public int Id { get; set; }

    public string PlayerName { get; set; } = string.Empty;

    public int Score { get; set; }

    public string GameName { get; set; } = string.Empty;
}