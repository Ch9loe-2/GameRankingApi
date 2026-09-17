using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace GameRankingApi.Models;

[Index(nameof(PlayerName))]
[Index(nameof(GameName))]
public class PlayerScore
{
    public int Id { get; set; }

    [MaxLength(50)]
    public string PlayerName { get; set; } = string.Empty;

    [Range(0, int.MaxValue)]
    public int Score { get; set; }

    [MaxLength(100)]
    public string GameName { get; set; } = string.Empty;
}