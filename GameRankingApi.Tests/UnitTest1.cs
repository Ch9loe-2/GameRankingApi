using System.Net;
using System.Net.Http.Json;
using GameRankingApi.Common;
using GameRankingApi.Models;
using Xunit;

namespace GameRankingApi.Tests;

public class ApiTests
{
    [Fact]
    public async Task GetHome_ReturnsSuccess()
    {
        // Arrange
        using var factory = new CustomWebApplicationFactory();
        using var client = factory.CreateClient();

        // Act
        var response = await client.GetAsync("/");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetScores_ReturnsScoresOrderedByScore()
    {
        // Arrange
        using var factory = new CustomWebApplicationFactory();
        using var client = factory.CreateClient();

        var score1 = new
        {
            PlayerName = "小明",
            Score = 1000,
            GameName = "测试游戏"
        };

        var score2 = new
        {
            PlayerName = "小红",
            Score = 5000,
            GameName = "测试游戏"
        };

        var score3 = new
        {
            PlayerName = "小王",
            Score = 3000,
            GameName = "测试游戏"
        };

        await client.PostAsJsonAsync("/api/score", score1);
        await client.PostAsJsonAsync("/api/score", score2);
        await client.PostAsJsonAsync("/api/score", score3);

        // Act
        var response = await client.GetAsync("/api/score");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<
            ApiResponse<List<PlayerScore>>
        >();

        Assert.NotNull(result);
        Assert.NotNull(result.Data);

        Assert.Equal(3, result.Data.Count);
        Assert.Equal(5000, result.Data[0].Score);
        Assert.Equal(3000, result.Data[1].Score);
        Assert.Equal(1000, result.Data[2].Score);
    }

    [Fact]
    public async Task CreateScore_WithValidData_ReturnsCreated()
    {
        // Arrange
        using var factory = new CustomWebApplicationFactory();
        using var client = factory.CreateClient();

        var request = new
        {
            PlayerName = "测试玩家",
            Score = 5000,
            GameName = "测试游戏"
        };

        // Act
        var response = await client.PostAsJsonAsync(
            "/api/score",
            request
        );

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<
            ApiResponse<PlayerScore>
        >();

        Assert.NotNull(result);
        Assert.NotNull(result.Data);

        Assert.Equal(201, result.Code);
        Assert.Equal("测试玩家", result.Data.PlayerName);
        Assert.Equal(5000, result.Data.Score);
        Assert.Equal("测试游戏", result.Data.GameName);
        Assert.True(result.Data.Id > 0);
    }

    [Fact]
    public async Task CreateScore_WithEmptyPlayerName_ReturnsBadRequest()
    {
        // Arrange
        using var factory = new CustomWebApplicationFactory();
        using var client = factory.CreateClient();

        var request = new
        {
            PlayerName = "",
            Score = 5000,
            GameName = "测试游戏"
        };

        // Act
        var response = await client.PostAsJsonAsync(
            "/api/score",
            request
        );

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateScore_WithNegativeScore_ReturnsBadRequest()
    {
        // Arrange
        using var factory = new CustomWebApplicationFactory();
        using var client = factory.CreateClient();

        var request = new
        {
            PlayerName = "测试玩家",
            Score = -100,
            GameName = "测试游戏"
        };

        // Act
        var response = await client.PostAsJsonAsync(
            "/api/score",
            request
        );

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateScore_WithEmptyGameName_ReturnsBadRequest()
    {
        // Arrange
        using var factory = new CustomWebApplicationFactory();
        using var client = factory.CreateClient();

        var request = new
        {
            PlayerName = "测试玩家",
            Score = 5000,
            GameName = ""
        };

        // Act
        var response = await client.PostAsJsonAsync(
            "/api/score",
            request
        );

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task UpdateScore_WithValidData_ReturnsSuccess()
    {
        // Arrange
        using var factory = new CustomWebApplicationFactory();
        using var client = factory.CreateClient();

        var createRequest = new
        {
            PlayerName = "原玩家",
            Score = 1000,
            GameName = "测试游戏"
        };

        var createResponse = await client.PostAsJsonAsync(
            "/api/score",
            createRequest
        );

        var created = await createResponse.Content.ReadFromJsonAsync<
            ApiResponse<PlayerScore>
        >();

        Assert.NotNull(created);
        Assert.NotNull(created.Data);

        var updateRequest = new
        {
            PlayerName = "修改后的玩家",
            Score = 5000,
            GameName = "新游戏"
        };

        // Act
        var response = await client.PutAsJsonAsync(
            $"/api/score/{created.Data.Id}",
            updateRequest
        );

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<
            ApiResponse<PlayerScore>
        >();

        Assert.NotNull(result);
        Assert.NotNull(result.Data);

        Assert.Equal(200, result.Code);
        Assert.Equal("修改后的玩家", result.Data.PlayerName);
        Assert.Equal(5000, result.Data.Score);
        Assert.Equal("新游戏", result.Data.GameName);
    }

    [Fact]
    public async Task UpdateScore_WithNonExistingId_ReturnsNotFound()
    {
        // Arrange
        using var factory = new CustomWebApplicationFactory();
        using var client = factory.CreateClient();

        var request = new
        {
            PlayerName = "测试玩家",
            Score = 5000,
            GameName = "测试游戏"
        };

        // Act
        var response = await client.PutAsJsonAsync(
            "/api/score/9999",
            request
        );

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DeleteScore_WithExistingId_ReturnsSuccess()
    {
        // Arrange
        using var factory = new CustomWebApplicationFactory();
        using var client = factory.CreateClient();

        var createRequest = new
        {
            PlayerName = "待删除玩家",
            Score = 3000,
            GameName = "测试游戏"
        };

        var createResponse = await client.PostAsJsonAsync(
            "/api/score",
            createRequest
        );

        var created = await createResponse.Content.ReadFromJsonAsync<
            ApiResponse<PlayerScore>
        >();

        Assert.NotNull(created);
        Assert.NotNull(created.Data);

        var id = created.Data.Id;

        // Act
        var deleteResponse = await client.DeleteAsync(
            $"/api/score/{id}"
        );

        // Assert
        Assert.Equal(HttpStatusCode.OK, deleteResponse.StatusCode);

        var result = await deleteResponse.Content.ReadFromJsonAsync<
            ApiResponse<object>
        >();

        Assert.NotNull(result);
        Assert.Equal(200, result.Code);
        Assert.Equal("删除成功", result.Message);

        // 再次查询，确认数据已经不存在
        var getResponse = await client.GetAsync(
            $"/api/score/{id}"
        );

        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    [Fact]
    public async Task DeleteScore_WithNonExistingId_ReturnsNotFound()
    {
        // Arrange
        using var factory = new CustomWebApplicationFactory();
        using var client = factory.CreateClient();

        // Act
        var response = await client.DeleteAsync(
            "/api/score/9999"
        );

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetScoreById_WithExistingId_ReturnsSuccess()
    {
        // Arrange
        using var factory = new CustomWebApplicationFactory();
        using var client = factory.CreateClient();

        var request = new
        {
            PlayerName = "查询玩家",
            Score = 4000,
            GameName = "测试游戏"
        };

        var createResponse = await client.PostAsJsonAsync(
            "/api/score",
            request
        );

        var created = await createResponse.Content.ReadFromJsonAsync<
            ApiResponse<PlayerScore>
        >();

        Assert.NotNull(created);
        Assert.NotNull(created.Data);

        var id = created.Data.Id;

        // Act
        var response = await client.GetAsync(
            $"/api/score/{id}"
        );

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<
            ApiResponse<PlayerScore>
        >();

        Assert.NotNull(result);
        Assert.NotNull(result.Data);

        Assert.Equal(200, result.Code);
        Assert.Equal("查询成功", result.Message);
        Assert.Equal(id, result.Data.Id);
        Assert.Equal("查询玩家", result.Data.PlayerName);
        Assert.Equal(4000, result.Data.Score);
        Assert.Equal("测试游戏", result.Data.GameName);
    }

    [Fact]
    public async Task GetScoresByGameName_ReturnsMatchingScores()
    {
        // Arrange
        using var factory = new CustomWebApplicationFactory();
        using var client = factory.CreateClient();

        await client.PostAsJsonAsync("/api/score", new
        {
            PlayerName = "小明",
            Score = 1000,
            GameName = "游戏A"
        });

        await client.PostAsJsonAsync("/api/score", new
        {
            PlayerName = "小红",
            Score = 5000,
            GameName = "游戏A"
        });

        await client.PostAsJsonAsync("/api/score", new
        {
            PlayerName = "小王",
            Score = 3000,
            GameName = "游戏B"
        });

        // Act
        var response = await client.GetAsync(
            "/api/score/game/游戏A"
        );

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<
            ApiResponse<List<PlayerScore>>
        >();

        Assert.NotNull(result);
        Assert.NotNull(result.Data);

        Assert.Equal(2, result.Data.Count);
        Assert.Equal("游戏A", result.Data[0].GameName);
        Assert.Equal("游戏A", result.Data[1].GameName);

        // 验证游戏A内部按照分数降序
        Assert.Equal(5000, result.Data[0].Score);
        Assert.Equal(1000, result.Data[1].Score);
    }

    [Fact]
    public async Task GetScoresByPlayerName_ReturnsMatchingScores()
    {
        // Arrange
        using var factory = new CustomWebApplicationFactory();
        using var client = factory.CreateClient();

        await client.PostAsJsonAsync("/api/score", new
        {
            PlayerName = "小明",
            Score = 1000,
            GameName = "游戏A"
        });

        await client.PostAsJsonAsync("/api/score", new
        {
            PlayerName = "小明",
            Score = 5000,
            GameName = "游戏B"
        });

        await client.PostAsJsonAsync("/api/score", new
        {
            PlayerName = "小红",
            Score = 3000,
            GameName = "游戏A"
        });

        // Act
        var response = await client.GetAsync(
            "/api/score/player/小明"
        );

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<
            ApiResponse<List<PlayerScore>>
        >();

        Assert.NotNull(result);
        Assert.NotNull(result.Data);

        Assert.Equal(2, result.Data.Count);

        Assert.Equal("小明", result.Data[0].PlayerName);
        Assert.Equal("小明", result.Data[1].PlayerName);

        // 验证玩家自己的成绩按照分数降序
        Assert.Equal(5000, result.Data[0].Score);
        Assert.Equal(1000, result.Data[1].Score);

        Assert.Equal("游戏B", result.Data[0].GameName);
        Assert.Equal("游戏A", result.Data[1].GameName);
    }

    [Fact]
public async Task GetTopScores_ReturnsTopNScores()
{
    // Arrange
    using var factory = new CustomWebApplicationFactory();
    using var client = factory.CreateClient();

    await client.PostAsJsonAsync("/api/score", new
    {
        PlayerName = "小明",
        Score = 1000,
        GameName = "游戏"
    });

    await client.PostAsJsonAsync("/api/score", new
    {
        PlayerName = "小红",
        Score = 5000,
        GameName = "游戏"
    });

    await client.PostAsJsonAsync("/api/score", new
    {
        PlayerName = "小王",
        Score = 3000,
        GameName = "游戏"
    });

    await client.PostAsJsonAsync("/api/score", new
    {
        PlayerName = "小李",
        Score = 2000,
        GameName = "游戏"
    });

    // Act
    var response = await client.GetAsync(
        "/api/score/top/2"
    );

    // Assert
    Assert.Equal(HttpStatusCode.OK, response.StatusCode);

    var result = await response.Content.ReadFromJsonAsync<
        ApiResponse<List<PlayerScore>>
    >();

    Assert.NotNull(result);
    Assert.NotNull(result.Data);

    Assert.Equal(2, result.Data.Count);

    // 验证只返回分数最高的两条
    Assert.Equal(5000, result.Data[0].Score);
    Assert.Equal(3000, result.Data[1].Score);
}

[Fact]
public async Task GetTopScores_WithInvalidCount_ReturnsBadRequest()
{
    // Arrange
    using var factory = new CustomWebApplicationFactory();
    using var client = factory.CreateClient();

    // Act
    var response = await client.GetAsync(
        "/api/score/top/0"
    );

    // Assert
    Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

    var result = await response.Content.ReadFromJsonAsync<
        ApiResponse<object>
    >();

    Assert.NotNull(result);
    Assert.Equal(400, result.Code);
    Assert.Equal("数量必须大于 0", result.Message);
}
}