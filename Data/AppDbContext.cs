using GameRankingApi.Models;
using Microsoft.EntityFrameworkCore;

namespace GameRankingApi.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<PlayerScore> PlayerScores { get; set; }
}