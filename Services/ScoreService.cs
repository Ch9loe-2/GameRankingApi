using GameRankingApi.Data;
using GameRankingApi.DTOs;
using GameRankingApi.Models;
using Microsoft.EntityFrameworkCore;

namespace GameRankingApi.Services;

public class ScoreService
{
    private readonly AppDbContext _db;
    private readonly ILogger<ScoreService> _logger;

    public ScoreService(AppDbContext db, ILogger<ScoreService> logger)
    {
        _db = db;
        _logger = logger;
    }

    private static void ValidateRequest(PlayerScoreRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.PlayerName))
            throw new ArgumentException("玩家名称不能为空");

        if (request.Score < 0)
            throw new ArgumentException("分数不能小于 0");

        if (string.IsNullOrWhiteSpace(request.GameName))
            throw new ArgumentException("游戏名称不能为空");
    }

    // 获取全部排行榜
    public async Task<List<PlayerScore>> GetAllAsync(CancellationToken ct = default)
    {
        return await _db.PlayerScores
            .OrderByDescending(x => x.Score)
            .ToListAsync(ct);
    }

    // 根据 ID 获取成绩
    public async Task<PlayerScore?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return await _db.PlayerScores.FindAsync([id], ct);
    }

    // 根据游戏名称获取排行榜
    public async Task<List<PlayerScore>> GetByGameNameAsync(string gameName, CancellationToken ct = default)
    {
        return await _db.PlayerScores
            .Where(x => x.GameName == gameName)
            .OrderByDescending(x => x.Score)
            .ToListAsync(ct);
    }

    // 根据玩家名称获取成绩
    public async Task<List<PlayerScore>> GetByPlayerNameAsync(string playerName, CancellationToken ct = default)
    {
        return await _db.PlayerScores
            .Where(x => x.PlayerName == playerName)
            .OrderByDescending(x => x.Score)
            .ToListAsync(ct);
    }

    // 获取 Top N 排行榜
    public async Task<List<PlayerScore>> GetTopAsync(int count, CancellationToken ct = default)
    {
        return await _db.PlayerScores
            .OrderByDescending(x => x.Score)
            .Take(count)
            .ToListAsync(ct);
    }

    // 添加成绩
    public async Task<PlayerScore> CreateAsync(PlayerScoreRequest request, CancellationToken ct = default)
    {
        _logger.LogInformation(
            "开始添加玩家成绩：玩家={PlayerName}，分数={Score}，游戏={GameName}",
            request.PlayerName,
            request.Score,
            request.GameName);

        ValidateRequest(request);

        var score = new PlayerScore
        {
            PlayerName = request.PlayerName,
            Score = request.Score,
            GameName = request.GameName
        };

        _db.PlayerScores.Add(score);
        await _db.SaveChangesAsync(ct);

        _logger.LogInformation(
            "玩家成绩添加成功：Id={Id}，玩家={PlayerName}，分数={Score}，游戏={GameName}",
            score.Id,
            score.PlayerName,
            score.Score,
            score.GameName);

        return score;
    }

    // 修改玩家成绩
    public async Task<PlayerScore?> UpdateAsync(int id, PlayerScoreRequest request, CancellationToken ct = default)
    {
        ValidateRequest(request);

        var score = await _db.PlayerScores.FindAsync([id], ct);

        if (score == null)
        {
            _logger.LogWarning("修改成绩失败：未找到成绩记录 Id={Id}", id);
            return null;
        }

        score.PlayerName = request.PlayerName;
        score.Score = request.Score;
        score.GameName = request.GameName;

        await _db.SaveChangesAsync(ct);

        _logger.LogInformation(
            "玩家成绩修改成功：Id={Id}，玩家={PlayerName}，分数={Score}，游戏={GameName}",
            score.Id,
            score.PlayerName,
            score.Score,
            score.GameName);

        return score;
    }

    // 删除玩家成绩
    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        var score = await _db.PlayerScores.FindAsync([id], ct);

        if (score == null)
        {
            _logger.LogWarning("删除成绩失败：未找到成绩记录 Id={Id}", id);
            return false;
        }

        _db.PlayerScores.Remove(score);
        await _db.SaveChangesAsync(ct);

        _logger.LogInformation(
            "玩家成绩删除成功：Id={Id}，玩家={PlayerName}，分数={Score}，游戏={GameName}",
            score.Id,
            score.PlayerName,
            score.Score,
            score.GameName);

        return true;
    }
}