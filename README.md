# GameRankingApi

一个基于 **C# + ASP.NET Core Minimal API** 开发的游戏排行榜后端项目。

项目实现了玩家成绩的增删改查、排行榜查询、游戏筛选、玩家筛选以及 Top N 排名等功能，并使用 SQLite + Entity Framework Core 进行数据持久化。

## 项目简介

GameRankingApi 是一个个人后端练习项目，主要用于学习和实践 ASP.NET Core Web API、Entity Framework Core、SQLite、RESTful API 以及 Git 版本管理。

目前已经实现：

* 玩家成绩排行榜
* 根据 ID 查询成绩
* 根据游戏名称查询排行榜
* 根据玩家名称查询成绩
* 获取 Top N 排行榜
* 添加玩家成绩
* 修改玩家成绩
* 删除玩家成绩
* 基础异常处理
* Swagger API 接口文档

## 技术栈

* C#
* .NET 10
* ASP.NET Core Minimal API
* Entity Framework Core
* SQLite
* Swagger / OpenAPI
* Git / GitHub

## 项目结构

```text
GameRankingApi/
├── Data/
│   └── AppDbContext.cs
├── DTOs/
│   └── PlayerScoreRequest.cs
├── Endpoints/
│   └── ScoreEndpoints.cs
├── Models/
│   └── PlayerScore.cs
├── Migrations/
├── Program.cs
├── gameranking.db
├── GameRankingApi.csproj
└── README.md
```

## API 接口

| 请求方式   | 接口                               | 功能           |
| ------ | -------------------------------- | ------------ |
| GET    | `/api/score`                     | 获取全部排行榜      |
| GET    | `/api/score/{id}`                | 根据 ID 获取成绩   |
| GET    | `/api/score/game/{gameName}`     | 根据游戏名称获取排行榜  |
| GET    | `/api/score/player/{playerName}` | 根据玩家名称查询成绩   |
| GET    | `/api/score/top/{count}`         | 获取 Top N 排行榜 |
| POST   | `/api/score`                     | 添加玩家成绩       |
| PUT    | `/api/score/{id}`                | 修改玩家成绩       |
| DELETE | `/api/score/{id}`                | 删除玩家成绩       |

## 数据模型

### PlayerScore

```text
Id          成绩记录 ID
PlayerName  玩家名称
Score       游戏分数
GameName    游戏名称
```

## 添加成绩

### POST `/api/score`

请求示例：

```json
{
  "playerName": "小也",
  "score": 5000,
  "gameName": "测试游戏"
}
```

接口会对请求数据进行基础校验：

* 玩家名称不能为空
* 分数不能小于 0
* 游戏名称不能为空

添加成功后返回 `201 Created`。

## 修改成绩

### PUT `/api/score/{id}`

例如：

```text
PUT /api/score/1
```

请求：

```json
{
  "playerName": "小也",
 
```
