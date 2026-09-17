using System.ComponentModel.DataAnnotations;

namespace GameRankingApi.DTOs;

public class PlayerScoreRequest
{
    [Required(ErrorMessage = "玩家名称不能为空")]
    [MaxLength(50, ErrorMessage = "玩家名称不能超过 50 个字符")]
    public string PlayerName { get; set; } = string.Empty;

    [Range(0, int.MaxValue, ErrorMessage = "分数不能小于 0")]
    public int Score { get; set; }

    [Required(ErrorMessage = "游戏名称不能为空")]
    [MaxLength(100, ErrorMessage = "游戏名称不能超过 100 个字符")]
    public string GameName { get; set; } = string.Empty;
}