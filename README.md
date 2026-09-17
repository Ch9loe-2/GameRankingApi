# GameRankingApi 🎮

一个基于 **C# + ASP.NET Core Minimal API** 开发的游戏排行榜后端 API 项目。

玩家成绩的**增删改查**、**排行榜查询**、**游戏筛选**、**玩家筛选**以及 **Top N 排名**，全部通过 RESTful API 对外暴露。

项目使用 **Entity Framework Core + SQLite** 持久化数据，**Service 层**负责业务逻辑，**统一 API 响应格式 + 全局异常处理 + 日志 + Swagger 文档 + xUnit 自动化测试**全线覆盖。

---

## 项目简介

GameRankingApi 是一个个人后端开发实践项目。核心目标是实践 **Web API 全链路工程能力**，而不是堆砌框架。从路由注册到数据库访问、从参数校验到异常处理、从日志记录到自动化测试，每层都独立写代码管理，做到「能跑且经得起问」。

---

## 核心功能

| 接口 | 说明 |
|------|------|
| `GET /api/score` | 获取全部排行榜（按分数降序） |
| `GET /api/score/{id}` | 根据 ID 查询单条成绩 |
| `GET /api/score/game/{gameName}` | 按游戏名查询排行榜 |
| `GET /api/score/player/{playerName}` | 按玩家名查询成绩 |
| `GET /api/score/top/{count}` | 获取前 N 名 |
| `POST /api/score` | 添加成绩 |
| `PUT /api/score/{id}` | 修改成绩 |
| `DELETE /api/score/{id}` | 删除成绩 |

---

## 技术栈

- **C# / .NET 10**
- **ASP.NET Core Minimal API**
- **Entity Framework Core + SQLite**
- **Swagger / OpenAPI**
- **xUnit + Microsoft.AspNetCore.Mvc.Testing**
- **Git / GitHub**

---

## 项目架构

```
GameRankingApi/
├── Common/              → 通用数据结构（统一响应模型）
├── Data/                → EF Core 上下文
├── DTOs/                → 请求模型（与 Entity 分离）
├── Models/              → 数据库实体
├── Services/            → 业务逻辑层
├── Endpoints/           → API 路由定义
├── Migrations/          → 数据库迁移
├── GameRankingApi.Tests/ → xUnit 自动化集成测试
├── Program.cs           → 应用入口 & 中间件配置
├── GameRankingApi.http  → API 调试请求
└── test.http            → 边界场景测试请求
```

### 分层设计

| 层 | 职责 | 关键原则 |
|----|------|---------|
| **Endpoints** | HTTP 路由 + 请求/响应处理 | 不碰数据库，不写业务逻辑 |
| **Services** | 业务逻辑（CRUD + 校验 + 日志） | 不感知 HTTP |
| **Data** | EF Core DbContext | 仅配置数据库连接 |
| **Models** | 数据库实体 + 数据注解 | 字段约束在模型层定义 |
| **DTOs** | 接收客户端输入 | 与 Entity 分离，避免过载 |

---

## 核心技术实现

### 统一响应格式

所有接口返回 `ApiResponse<T>` 结构：

```json
{
  "code": 200,
  "message": "查询成功",
  "data": [...]
}
```

无论是成功、参数错误（400）、资源不存在（404）还是服务器错误（500），响应结构一致。

### 全局异常处理

通过 `app.UseExceptionHandler("/error")` 捕获所有未处理异常，统一返回 500 JSON，**不暴露内部堆栈跟踪**。

### 数据校验

两层校验：

1. **DTO 层**：`[Required]`、`[MaxLength]`、`[Range]` 数据注解（ASP.NET Core 模型绑定自动处理）
2. **Service 层**：`ValidateRequest()` 显式校验（防御性编程，避免绕过模型绑定的调用）

### 数据库自动初始化

启动时自动调用 `EnsureCreatedAsync()`，**删除 `gameranking.db` 后直接 `dotnet run` 即可重建表结构和索引**，无需手动执行迁移脚本。

### CancellationToken 传递

所有异步方法均接受 `CancellationToken ct = default` 参数，通过 `HttpContext.RequestAborted` 传递客户端断开信号，防止请求取消后数据库操作继续执行。

### 数据库索引

`PlayerName` 和 `GameName` 字段建立了数据库索引，优化按玩家和按游戏的查询性能。

### 自动化集成测试

使用 `WebApplicationFactory<Program>` + SQLite `:memory:` 数据库，15 个测试覆盖正常流程 + 所有已知异常路径，测试之间互相隔离。

---

## 运行方法

```bash
# 克隆
git clone https://github.com/Ch9loe-2/GameRankingApi.git
cd GameRankingApi

# 启动（自动建库）
dotnet run

# 访问
# http://localhost:5194          → 首页
# http://localhost:5194/swagger  → Swagger UI

# 运行测试
dotnet test GameRankingApi.Tests/GameRankingApi.Tests.csproj
```

---

## 项目亮点（面试可讲）

1. **完整的分层架构**：Endpoints → Services → EF Core → SQLite，每层职责单一，替换任意层不影响其他层
2. **数据库自举**：`EnsureCreatedAsync()` 保证项目克隆下来直接 `dotnet run` 就能用，零配置
3. **校验双层防御**：DTO 注解（框架级）+ Service 显式校验（代码级），不信任任何外部输入
4. **CancellationToken 全链路传递**：客户端断开 → ASP.NET Core 取消令牌 → EF Core 停止数据库操作，避免资源浪费
5. **集成测试使用内存数据库隔离**：每个测试类独立 `WebApplicationFactory`，互不干扰，15 个测试全通过

---

## 面试高频问题

<details>
<summary><b>为什么选择 Minimal API 而不是 Controller？</b></summary>

Minimal API 适合本项目这种接口数量少、逻辑清晰的场景。不需要 Controller/action 的额外抽象层，减少模板代码。如果项目扩展到 20+ 接口，会考虑切到 Controller 以获得更好的组织能力。
</details>

<details>
<summary><b>数据量变大（百万级）怎么办？</b></summary>

当前 SQLite 不适合高并发场景。迁移路径：切换 EF Core Provider 为 PostgreSQL/SQL Server，配置文件中改连接字符串即可，Service 层代码完全不用动。同时需要加分页参数（`page` / `pageSize`）避免全表扫描。
</details>

<details>
<summary><b>校验为什么写了两遍？</b></summary>

DTO 注解提供第一道防线，由 ASP.NET Core 模型绑定框架自动触发。Service 层的 `ValidateRequest()` 是第二道防线，确保即使有绕过模型绑定的调用路径（如直接单元测试 Service），校验依然生效。这是典型的防御性编程。
</details>

<details>
<summary><b>EnsureCreated vs Migrate 有什么区别？</b></summary>

`EnsureCreated()` 根据模型状态直接建表，不生成迁移记录，适合开发/小型项目。`Migrate()` 按迁移文件增量执行，适合多人协作/生产环境。本项目使用 `EnsureCreated()` 保持启动零配置。
</details>

<details>
<summary><b>测试为什么不用真实数据库？</b></summary>

SQLite `:memory:` 数据库在内存中运行，每个测试类创建独立实例，保证测试完全隔离。如果用真实数据库文件，测试之间的数据会互相污染，且无法并行执行。
</details>

---

## 项目状态

- **构建状态**：通过（0 警告，0 错误）
- **测试结果**：15/15 通过
- **GitHub**：`https://github.com/Ch9loe-2/GameRankingApi`