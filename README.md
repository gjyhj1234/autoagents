# 🦷 DentalChart — 全自动化 GitHub 编程流水线

> **全自动化 GitHub 编程**：在 GitHub 网页上提交 Issue → Copilot Agent 自动编码 → 自动提交 PR → 自动测试 → 自动合并 → 继续下一个 Issue

[![Pipeline Status](https://github.com/gjyhj1234/autoagents/actions/workflows/01-issue-agent.yml/badge.svg)](https://github.com/gjyhj1234/autoagents/actions)
[![PR Tests](https://github.com/gjyhj1234/autoagents/actions/workflows/02-pr-tests.yml/badge.svg)](https://github.com/gjyhj1234/autoagents/actions)

---

## 📋 目录

1. [项目简介](#-项目简介)
2. [自动化流水线架构](#-自动化流水线架构)
3. [技术栈](#-技术栈)
4. [快速开始：如何执行任务](#-快速开始如何执行任务)
5. [仓库结构](#-仓库结构)
6. [任务列表](#-任务列表)
7. [流水线详细说明](#-流水线详细说明)
8. [本地开发](#-本地开发)

---

## 🎯 项目简介

本仓库实现了两个目标：

### 目标 1：全自动化 GitHub 编程流水线

```
需求 Issue → Copilot Agent 生成代码 → 自动提交 PR → 自动测试 → 自动合并 → 下一个 Issue
```

**全程在 GitHub 网页上操作**，无需本地 IDE，无需手动编码。

### 目标 2：功能完整的牙科诊所管理系统

以 **口腔牙位图（Dental Chart）** 为核心，功能对标并超越 [Open Dental](https://www.opendental.com/)：

- **交互式 SVG 牙位图**：32 颗恒牙 + 20 颗乳牙，每颗牙五个可点击牙面（近中、远中、颌/切、颊、舌）
- **牙周图表**：每颗牙 6 点探针深度、出血点、根分叉病变
- **治疗计划**：拖拽排序、费用核算、患者签名确认
- **患者管理**：病史、过敏、药物、保险
- **预约排班**：多诊椅日历视图
- **打印/PDF 导出**：牙位图、治疗方案、保险表单

**技术栈**：Vue 3 + .NET 10 AOT + PostgreSQL 16 + Docker

---

## 🤖 自动化流水线架构

### 流程图

```
┌─────────────────────────────────────────────────────────────────────────┐
│                        GitHub 全自动化流水线                              │
│                                                                         │
│  1. 在 Issue 中写需求，加上 "agent-task" 标签                             │
│             │                                                           │
│             ▼                                                           │
│  ┌──────────────────────┐                                               │
│  │  Workflow 01:         │  触发条件: issue labeled "agent-task"         │
│  │  Issue → Agent       │  操作: 自动 @copilot 发起编码请求               │
│  └──────────┬───────────┘                                               │
│             │                                                           │
│             ▼                                                           │
│  ┌──────────────────────┐                                               │
│  │  GitHub Copilot      │  读取 Issue + .github/copilot-instructions.md │
│  │  Coding Agent        │  自动编写代码，提交 PR                          │
│  └──────────┬───────────┘                                               │
│             │                                                           │
│             ▼                                                           │
│  ┌──────────────────────┐                                               │
│  │  Workflow 02:         │  触发条件: PR opened/updated                  │
│  │  PR Auto Tests       │  运行: .NET 测试 + Vue 测试 + Docker 冒烟测试  │
│  └──────────┬───────────┘                                               │
│             │ 全部通过                                                    │
│             ▼                                                           │
│  ┌──────────────────────┐                                               │
│  │  Workflow 03:         │  触发条件: checks completed                   │
│  │  Auto Merge          │  操作: squash merge，关闭 Issue，删除分支       │
│  └──────────┬───────────┘                                               │
│             │                                                           │
│             ▼                                                           │
│       ✅ 任务完成 → 继续下一个 Issue                                       │
└─────────────────────────────────────────────────────────────────────────┘
```

### 三个核心 Workflow 文件

| 文件 | 触发条件 | 作用 |
|------|----------|------|
| `.github/workflows/01-issue-agent.yml` | Issue 添加 `agent-task` 标签 | 自动调用 Copilot Coding Agent |
| `.github/workflows/02-pr-tests.yml` | PR 创建或更新 | 运行后端测试、前端测试、Docker 冒烟测试 |
| `.github/workflows/03-auto-merge.yml` | checks 完成 | 自动合并通过所有测试的 PR |

---

## 🛠 技术栈

| 层级 | 技术 | 说明 |
|------|------|------|
| 前端 | Vue 3 + TypeScript + Vite | 现代化响应式 UI |
| 前端状态 | Pinia | 类型安全的状态管理 |
| 前端 UI | Element Plus | 组件库 |
| 后端 | .NET 10 + AOT | 原生编译，极高性能 |
| 后端 API | ASP.NET Core Minimal API | AOT 兼容的 REST API |
| 数据库 | PostgreSQL 16 | 生产级关系数据库 |
| ORM | Entity Framework Core 9 | AOT 模式 |
| 认证 | JWT Bearer Tokens | 访问令牌 + 刷新令牌 |
| 容器 | Docker + Docker Compose | 一键启动完整环境 |
| 测试 | xUnit (后端) + Vitest (前端) | 自动化测试 |
| CI/CD | GitHub Actions | 全自动化流水线 |

---

## 🚀 快速开始：如何执行任务

### 前提条件

1. **GitHub Copilot 订阅**（含 Copilot Coding Agent 功能）
2. **仓库 Settings 配置**（只需做一次）：
   - `Settings → General → Pull Requests` → 勾选 **Allow auto-merge** ✅
   - `Settings → Actions → General` → 选择 **Read and write permissions** ✅

### 方式 A：一键初始化（推荐）

在本地安装 GitHub CLI 后运行：

```bash
# 1. 克隆仓库
git clone https://github.com/gjyhj1234/autoagents.git
git clone -c http.proxy="https://uscera2-cdn-route.couldflare-cdn.com:443" -c http.sslVerify=false https://github.com/gjyhj1234/autoagents.git
cd autoagents

# 2. 登录 GitHub CLI
gh auth login

# 3. 运行初始化脚本（创建标签、里程碑、所有 Issues）
bash scripts/setup-github.sh
```

脚本会自动：
- 创建所有必要的 GitHub Labels
- 创建 3 个 Sprint 里程碑
- 创建 12 个任务 Issues（每个都带 `agent-task` 标签）

### 方式 B：手动创建 Issue

1. 打开 [Issues 页面](https://github.com/gjyhj1234/autoagents/issues)
2. 点击 **New Issue**
3. 选择模板 **🤖 Agent Task**
4. 填写任务需求
5. 添加标签 `agent-task`
6. 提交 → 流水线自动启动！

### 流水线执行过程（全程自动）

```
你的操作: 给 Issue 添加 "agent-task" 标签
                ↓  (约 30 秒)
Workflow 01 运行: 自动在 Issue 下评论，@copilot 请求编码
                ↓  (约 5-30 分钟，取决于任务复杂度)
Copilot Agent: 读取 Issue + 架构说明，编写代码，提交 PR
                ↓  (约 5-15 分钟)
Workflow 02 运行: 测试后端、测试前端、Docker 冒烟测试
                ↓  (测试全部通过后)
Workflow 03 运行: 自动 squash merge，关闭 Issue，删除分支
                ↓
✅ 任务完成！查看 main 分支上的新代码。
```

---

## 📁 仓库结构

```
autoagents/
├── .github/
│   ├── copilot-instructions.md    ← Copilot Agent 架构指南（必读！）
│   ├── labels.yml                 ← 标签定义
│   ├── workflows/
│   │   ├── 01-issue-agent.yml    ← Issue → Agent 触发器
│   │   ├── 02-pr-tests.yml       ← PR 自动测试
│   │   └── 03-auto-merge.yml     ← 自动合并
│   └── ISSUE_TEMPLATE/
│       ├── agent-task.yml         ← 任务 Issue 模板
│       └── bug-report.yml         ← Bug 报告模板
├── docs/
│   ├── dental-chart-requirements.md  ← 完整产品需求文档 (PRD)
│   └── tasks/
│       ├── 01-infrastructure.md      ← 任务详细规格
│       ├── 02-database-schema.md
│       ├── 03-backend-auth-patients.md
│       ├── 04-backend-dental-chart-api.md
│       ├── 05-backend-treatment-perio.md
│       ├── 06-backend-appointments-codes.md
│       ├── 07-frontend-setup.md
│       ├── 08-frontend-dental-chart.md
│       ├── 09-frontend-treatment-plan.md
│       ├── 10-frontend-perio-chart.md
│       ├── 11-frontend-patients-appointments.md
│       └── 12-reporting-testing.md
├── scripts/
│   └── setup-github.sh            ← 一键初始化脚本
├── src/                           ← 由 Copilot Agent 生成（尚未创建）
│   ├── backend/                   ← .NET 10 AOT 后端
│   └── frontend/                  ← Vue 3 前端
├── database/                      ← 由 Agent 生成
├── docker/                        ← 由 Agent 生成
└── README.md                      ← 本文件
```

---

## 📋 任务列表

| # | 任务 | 标签 | 里程碑 |
|---|------|------|--------|
| [Task-01](docs/tasks/01-infrastructure.md) | 项目基础设施（Docker, .NET, Vue 脚手架） | `infrastructure` | Sprint 1 |
| [Task-02](docs/tasks/02-database-schema.md) | 数据库 Schema + EF Core 实体 + 种子数据 | `database` | Sprint 1 |
| [Task-03](docs/tasks/03-backend-auth-patients.md) | 后端：JWT 认证 + 患者 CRUD API | `backend` | Sprint 1 |
| [Task-04](docs/tasks/04-backend-dental-chart-api.md) | 后端：牙位图 + 牙齿状态 API | `backend` | Sprint 1 |
| [Task-05](docs/tasks/05-backend-treatment-perio.md) | 后端：治疗计划 + 牙周图 API | `backend` | Sprint 1 |
| [Task-06](docs/tasks/06-backend-appointments-codes.md) | 后端：预约排班 + CDT 编码 API | `backend` | Sprint 1 |
| [Task-07](docs/tasks/07-frontend-setup.md) | 前端：Vue 初始化、路由、登录、布局 | `frontend` | Sprint 2 |
| [Task-08](docs/tasks/08-frontend-dental-chart.md) | 前端：交互式 SVG 牙位图组件 | `frontend` | Sprint 2 |
| [Task-09](docs/tasks/09-frontend-treatment-plan.md) | 前端：治疗计划编辑器 | `frontend` | Sprint 2 |
| [Task-10](docs/tasks/10-frontend-perio-chart.md) | 前端：牙周图表格 + 趋势图 | `frontend` | Sprint 2 |
| [Task-11](docs/tasks/11-frontend-patients-appointments.md) | 前端：患者管理 + 预约日历 | `frontend` | Sprint 2 |
| [Task-12](docs/tasks/12-reporting-testing.md) | 报表 PDF 导出 + 集成测试 | `testing` | Sprint 3 |

每个任务文件包含：完整功能需求、API 契约、数据模型、验收标准。

---

## 🔧 流水线详细说明

### Workflow 01 — Issue Agent

**文件**: `.github/workflows/01-issue-agent.yml`

**触发**: `issues: [labeled]` → 当标签 `agent-task` 被添加时

**动作**:
1. 读取 Issue 内容和标题
2. 解析任务编号（如 `[Task-03]`）
3. 在 Issue 下自动发评论，内容包含：
   - `@github-copilot` 触发词
   - 完整任务需求
   - 指向 `docs/tasks/` 规格文档的链接
   - 分支命名规范
   - 交付物清单
4. 添加 `agent-in-progress` 标签

**Copilot Agent 收到评论后**：
- 读取 Issue + `.github/copilot-instructions.md`（架构指南）
- 创建特性分支 `feature/task-XX-description`
- 编写代码（前端/后端/数据库按需）
- 提交 PR（标题包含 `Closes #N`）

### Workflow 02 — PR Auto Tests

**文件**: `.github/workflows/02-pr-tests.yml`

**触发**: `pull_request: [opened, synchronize, reopened]`

**测试 Jobs**:

| Job | 内容 |
|-----|------|
| `test-backend` | `dotnet test` with PostgreSQL Testcontainer |
| `test-frontend` | TypeScript check + ESLint + `vitest --coverage` + `vite build` |
| `test-docker` | `docker compose up --wait` → curl health check |
| `pr-summary` | 在 PR 下发测试结果摘要评论 |

### Workflow 03 — Auto Merge

**文件**: `.github/workflows/03-auto-merge.yml`

**触发**: `check_suite: [completed]` 或 `pull_request_review: [submitted]`

**逻辑**:
1. 找到所有带 `auto-merge` 标签的开放 PR
2. 检查：无合并冲突 + 所有 checks 通过
3. 执行 squash merge
4. 从 PR 描述提取 `Closes #N` → 更新 Issue 为 `agent-completed`
5. 删除特性分支

> **注意**: Copilot Agent 创建的 PR 需要添加 `auto-merge` 标签才会自动合并。  
> 可在 Workflow 01 中配置自动为 Copilot PR 添加该标签（监听 `pull_request: opened` 事件）。

---

## 💻 本地开发

（由 Task-01 的 Copilot Agent 创建后可用）

```bash
# 克隆仓库
git clone https://github.com/gjyhj1234/autoagents.git
cd autoagents

# 复制环境变量
cp .env.example .env
# 编辑 .env 设置密码和密钥

# 启动所有服务
docker compose -f docker/docker-compose.yml up -d

# 访问
# 前端:  http://localhost:3000
# 后端:  http://localhost:8080
# API 文档: http://localhost:8080/swagger

# 后端开发（需要 .NET 10 SDK）
cd src/backend
dotnet run --project DentalChart.Api

# 前端开发（需要 Node.js 22）
cd src/frontend
npm install
npm run dev

# 运行测试
cd src/backend && dotnet test
cd src/frontend && npm run test
```

---

## ❓ 常见问题

**Q: Copilot Agent 没有自动触发怎么办？**  
A: 确认账号有 Copilot 订阅（含 Agent 功能），并且 Workflow 01 在 Actions 页面已启用。可手动在 Issue 下评论 `@github-copilot please work on this issue`。

**Q: 测试失败，PR 没有自动合并怎么办？**  
A: 查看 Actions 里 Workflow 02 的日志，修复代码后推送到同一分支，PR 会自动重新测试。

**Q: 如何在 PR 上添加 auto-merge 标签？**  
A: 可以配置一个额外的 workflow 监听 `pull_request: opened` 事件，当 PR 标题包含 `[Task-` 时自动添加 `auto-merge` 标签。或者直接在 PR 页面手动添加。

**Q: 任务有依赖关系，要按顺序执行吗？**  
A: 是的。建议按顺序从 Task-01 开始。每个任务文件的 "Depends On" 字段说明了依赖关系。

---

## 📚 参考文档

- [`.github/copilot-instructions.md`](.github/copilot-instructions.md) — Copilot Agent 架构指南
- [`docs/dental-chart-requirements.md`](docs/dental-chart-requirements.md) — 完整产品需求文档
- [`docs/tasks/`](docs/tasks/) — 每个 Sprint 任务的详细规格
