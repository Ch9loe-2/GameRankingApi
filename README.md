# GameRankingApi

一个基于 **C# + ASP.NET Core Minimal API** 开发的游戏排行榜后端 API 项目。

项目围绕玩家游戏成绩管理场景，实现成绩的增删改查、排行榜查询、游戏筛选、玩家筛选以及 Top N 排名等功能。

项目使用 **Entity Framework Core + SQLite** 进行数据持久化，并通过 Service 层进行业务逻辑管理，同时加入统一 API 响应格式、全局异常处理、日志记录、Swagger/OpenAPI 接口文档以及自动化接口测试。

## 项目简介

GameRankingApi 是一个个人后端开发实践项目，主要用于学习和实践 ASP.NET Core Minimal API、Entity Framework Core、SQLite、RESTful API、分层设计、接口测试以及 Git 版本管理。

目前项目已经完成从基础 API 到数据库持久化、业务逻辑分层以及自动化测试的逐步开发。

### 已实现功能

* 玩家成绩排行榜
* 根据 ID 查询成绩
* 根据游戏名称查询排行榜
* 根据玩家名称查询成绩
* 获取 Top N 排行榜
* 添加玩家成绩
* 修改玩家成绩
* 删除玩家成绩
* 请求参数基础校验
* 统一 API 响应格式
* 全局异常处理
* 日志记录
* Swagger / OpenAPI 接口文档
* 自动化接口测试

## 技术栈

* **C#**
* **.NET 10**
* **ASP.NET Core Minimal API**
* **Entity Framework Core**
* **SQLite**
* **Swagger / OpenAPI**
* **xUnit**
* **Microsoft.AspNetCore.Mvc.Testing**
* **Git / GitHub**

## 项目结构

```text
GameRankingApi/
├── Common/
│   └── ApiResponse.cs
├── Data/
│   └── AppDbContext.cs
├── DTOs/
│   └── PlayerScoreRequest.cs
├── Endpoints/
│   └── ScoreEndpoints.cs
├── Models/
│   └── PlayerScore.cs
├── Services/
│   └── ScoreService.cs
├── Migrations/
├── GameRankingApi.Tests/
│   ├── CustomWebApplicationFactory.cs
│   ├── GameRankingApi.Tests.csproj
│   └── UnitTest1.cs
├── Program.cs
├── GameRankingApi.csproj
├── GameRankingApi.http
├── test.http
├── gameranking.db
└── README.md
```

## 分层设计

项目按照不同职责进行了简单分层：

### Endpoints

负责 HTTP 请求与响应处理，将接口路由与具体业务逻辑进行分离。

主要文件：

```text
Endpoints/ScoreEndpoints.cs
```

### Services

负责成绩相关的业务逻辑，包括查询、创建、修改和删除等操作。

主要文件：

```text
Services/ScoreService.cs
```

### Data

负责 Entity Framework Core 数据库上下文配置。

主要文件：

```text
Data/AppDbContext.cs
```

### Models

定义数据库实体模型。

主要文件：

```text
Models/PlayerScore.cs
```

### DTOs

用于接收客户端提交的成绩数据，避免直接使用数据库实体作为请求模型。

主要文件：

```text
DTOs/PlayerScoreRequest.cs
```

### Common

用于存放通用的数据结构，例如统一 API 响应模型。

主要文件：

```text
Common/ApiResponse.cs
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

## 统一响应格式

项目使用统一的 API 响应结构：

```json
{
  "code": 200,
  "message": "查询成功",
  "data": []
}
```

主要字段：

| 字段      | 类型     | 说明      |
| ------- | ------ | ------- |
| code    | int    | API 状态码 |
| message | string | 操作结果信息  |
| data    | object | 返回的数据   |

例如查询排行榜成功：

```json
{
  "code": 200,
  "message": "查询成功",
  "data": [
    {
      "id": 1,
      "playerName": "小也",
      "score": 5000,
      "gameName": "测试游戏"
    }
  ]
}
```

## 数据校验

添加和修改成绩时，会进行基础参数校验：

* 玩家名称不能为空
* 分数不能小于 0
* 游戏名称不能为空

例如提交空玩家名称：

```json
{
  "playerName": "",
  "score": 1000,
  "gameName": "测试游戏"
}
```

接口返回 `400 Bad Request`。

## 异常处理

项目配置了全局异常处理机制。

对于未被业务代码处理的服务器异常，统一返回：

```json
{
  "code": 500,
  "message": "服务器内部发生错误",
  "data": null
}
```

避免直接向客户端暴露内部异常信息。

## 日志记录

项目在成绩相关业务处理中加入了日志记录，包括：

* 添加、修改、删除成绩时记录关键操作日志
* 修改或删除不存在的成绩记录时记录 Warning 日志

通过日志可以辅助定位接口运行过程中的问题。

## Swagger / OpenAPI

项目集成 Swagger / OpenAPI，可以通过 Swagger UI 查看和测试 API 接口。

项目启动后访问：

```text
http://localhost:5194/swagger
```

可以查看当前项目提供的接口。

## 自动化测试

项目使用 **xUnit + Microsoft.AspNetCore.Mvc.Testing** 编写自动化接口测试。

目前共包含 **15 个自动化测试**，覆盖主要接口和异常场景。

测试内容包括：

* 首页访问
* 获取排行榜
* 添加成绩
* 添加成绩参数校验
* 修改成绩
* 修改不存在的成绩
* 删除成绩
* 删除不存在的成绩
* 根据 ID 查询成绩
* 根据游戏名称查询成绩
* 根据玩家名称查询成绩
* Top N 排行榜
* Top N 参数校验

### 测试结果

当前自动化测试结果：

```text
15 Passed
0 Failed
```

测试使用 SQLite 内存数据库进行隔离，避免测试数据影响本地实际数据库。

运行测试：

```bash
dotnet test GameRankingApi.Tests/GameRankingApi.Tests.csproj
```

## 数据库

项目使用 SQLite 进行数据持久化。

数据库连接：

```text
Data Source=gameranking.db
```

Entity Framework Core 用于完成实体模型与数据库之间的数据访问。

项目使用 EF Core Migration 管理数据库结构变更。

## 如何运行

### 1. 克隆项目

```bash
git clone https://github.com/Ch9loe-2/GameRankingApi.git
cd GameRankingApi
```

### 2. 还原项目依赖

```bash
dotnet restore
```

### 3. 运行项目

```bash
dotnet run
```

项目默认运行地址：

```text
http://localhost:5194
```

### 4. 查看 Swagger

打开：

```text
http://localhost:5194/swagger
```

### 5. 运行自动化测试

```bash
dotnet test GameRankingApi.Tests/GameRankingApi.Tests.csproj
```

## 项目开发实践

在项目开发过程中，逐步完成了以下改进：

1. 搭建 ASP.NET Core Minimal API
2. 实现玩家成绩 CRUD
3. 使用 EF Core + SQLite 实现数据持久化
4. 增加数据库 Migration
5. 将业务逻辑抽取到 Service 层
6. 增加 DTO 接收客户端请求
7. 统一 API 响应格式
8. 增加全局异常处理
9. 增加日志记录
10. 集成 Swagger / OpenAPI
11. 使用 xUnit 编写自动化接口测试
12. 使用 Git 进行版本管理并同步至 GitHub

## 项目状态

项目目前已经完成核心功能开发，并通过自动化接口测试验证主要业务场景。

当前测试结果：

```text
15 Passed
0 Failed
```
