namespace GameRankingApi.DTOs;

public class PlayerScoreRequest
{
    public string PlayerName { get; set; } = string.Empty;
    public int Score { get; set; }
    public string GameName { get; set; } = string.Empty;
}