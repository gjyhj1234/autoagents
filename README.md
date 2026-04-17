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
│  │  Workflow 01:         │  触发条件: task issue 变化 / 手动运行            │
│  │  Issue Queue → Agent │  操作: 自动把下一个任务 Assign 给 Copilot        │
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
| `.github/workflows/01-issue-agent.yml` | Task Issue 变化 / 手动运行 | 自动维护队列并把下一个任务分配给 Copilot |
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
   - `Settings → Copilot`（或个人 Copilot 设置）→ 确认 **Copilot cloud agent** 已启用 ✅

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
- 触发 Workflow 01，自动把最早的任务分配给 Copilot

### 方式 B：手动创建 Issue

1. 打开 [Issues 页面](https://github.com/gjyhj1234/autoagents/issues)
2. 点击 **New Issue**
3. 选择模板 **🤖 Agent Task**
4. 填写任务需求
5. 添加标签 `agent-task`
6. 提交 → Workflow 01 会把它加入队列
7. 当轮到该任务时，系统会自动执行与右侧 **Assign to Agent → Copilot** 相同的动作

### 流水线执行过程（全程自动）

```
你的操作: 给 Issue 添加 "agent-task" 标签
                ↓  (约 30 秒)
Workflow 01 运行: 自动把最早的待办 Issue Assign 给 Copilot
                ↓  (约 5-30 分钟，取决于任务复杂度)
Copilot Agent: 读取 Issue + 架构说明，编写代码，提交 PR
                ↓  (约 5-15 分钟)
你可能需要在 PR 上点一次 "Approve and run workflows"
                ↓
Workflow 02 运行: 测试后端、测试前端、Docker 冒烟测试
                ↓  (测试全部通过后)
Workflow 03 运行: 自动 squash merge，关闭 Issue，删除分支
                ↓
✅ 任务完成！查看 main 分支上的新代码。
```

### GitHub 网页端准确操作指南（零基础）

1. 打开仓库首页：<https://github.com/gjyhj1234/autoagents>
2. 先确认一次性设置：
   - `Settings → Actions → General → Workflow permissions → Read and write permissions`
   - `Settings → General → Pull Requests → Allow auto-merge`
   - 账号和仓库都已启用 **Copilot cloud agent**
3. 打开 **Issues** 页面。
4. 新建任务时：
   - 点 **New issue**
   - 选 **🤖 Agent Task**
   - 填内容
   - 确认有 `agent-task` 标签
   - 点 **Submit new issue**
5. 提交后看右侧和标签：
   - `agent-queued` = 已排队，未轮到
   - `agent-in-progress` = 已经分配给 Copilot，正在执行
   - `agent-completed` = 已完成并合并
6. 如果 Issue 右侧出现 **Copilot** assignee，或时间线出现 👀，说明已经真正触发。
7. 如果右侧能看到 **Assign to Agent** 按钮：这就是 GitHub 官方原生触发入口。现在 Workflow 01 已自动调用同样的分配动作，所以通常不用你手点。
8. 当 Copilot 开出 PR 后，进入该 PR 页面：
   - 如果看到 **Approve and run workflows**，点击它一次
   - 这是 GitHub 当前对 Copilot PR 的安全限制，不是仓库配置错误
9. 之后等待：
   - Workflow 02 测试
   - Workflow 03 自动合并
   - 下一个 `agent-queued` Issue 自动转成 `agent-in-progress`
10. 查看“当前是否正在执行”：
   - Issue 右侧 Assignees 是否为 **Copilot**
   - Issue 标签是否为 `agent-in-progress`
   - PR / Issue 时间线里是否出现 👀、Draft PR、`View session`
   - Actions 页面是否有 **Copilot cloud agent** 或对应 workflow 在运行

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

**触发**: `issues: [opened, reopened, labeled, unlabeled]` + `workflow_dispatch`

**动作**:
1. 扫描所有开放的 `agent-task` Issues
2. 保证同一时刻只允许一个任务处于 `agent-in-progress`
3. 自动把最早的待办 Issue 分配给 **Copilot**
4. 其余任务自动标记为 `agent-queued`
5. 在 Issue 下维护状态评论，提示你如何判断是否真正开始执行

**关键点**：
- 右侧 **Assign to Agent** 才是 GitHub 官方原生触发方式
- Workflow 01 现在通过 GitHub API 自动执行同样的“Assign to Copilot”动作
- 所以不再依赖 `@github-copilot` 评论触发

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

> **注意 1**: Copilot Agent 创建的 PR 需要 `auto-merge` 标签才会自动合并，本仓库已通过 Workflow 04 自动添加。  
> **注意 2**: GitHub 当前对 Copilot 创建的 PR 仍可能要求你点击一次 **Approve and run workflows**，这一步暂时不能完全消除。

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
A: 先看 Issue 右侧是否真的分配给了 **Copilot**。如果没有，点右侧 **Assign to Agent → Copilot** 可以立即手动触发；这也是 GitHub 官方原生方式。Workflow 01 现在会自动做这件事，但如果你是老的 Issues，请到 **Actions → 🤖 01 — Issue → Agent → Run workflow** 手动跑一次来重建队列。

**Q: 测试失败，PR 没有自动合并怎么办？**  
A: 先确认你有没有在 PR 页面点过 **Approve and run workflows**。如果已经点过，再查看 Actions 里 Workflow 02 的日志，修复代码后推送到同一分支，PR 会自动重新测试。

**Q: 如何在 PR 上添加 auto-merge 标签？**  
A: 已由 `.github/workflows/04-label-pr.yml` 自动处理，通常不需要手工添加。

**Q: 任务有依赖关系，要按顺序执行吗？**  
A: 是的。现在 Workflow 01 会按 Issue 编号顺序只放行一个任务，Workflow 03 合并后会自动唤醒下一个排队任务。

---

## 📚 参考文档

- [`.github/copilot-instructions.md`](.github/copilot-instructions.md) — Copilot Agent 架构指南
- [`docs/dental-chart-requirements.md`](docs/dental-chart-requirements.md) — 完整产品需求文档
- [`docs/tasks/`](docs/tasks/) — 每个 Sprint 任务的详细规格
