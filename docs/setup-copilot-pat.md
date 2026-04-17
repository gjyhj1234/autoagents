# 设置 COPILOT_PAT — 自动分配 Copilot 所需的个人令牌

## 为什么需要 PAT？

GitHub Actions 内置的 `GITHUB_TOKEN` 是一个 **安装令牌 (installation token)**，
**不是**用户令牌，因此无法调用 Copilot cloud agent 的分配 API。

根据 [GitHub 官方文档](https://docs.github.com/en/copilot/how-tos/use-copilot-agents/cloud-agent/create-a-pr#assigning-an-issue-to-copilot-via-the-github-api)：

> Make sure you're authenticating with the API using a **user token**,
> for example a personal access token or a GitHub App user-to-server token.

---

## 步骤 1：创建 Fine-grained Personal Access Token

1. 打开 <https://github.com/settings/personal-access-tokens/new>
2. 填写：
   - **Token name**: `copilot-autoagents`（任意名称）
   - **Expiration**: 建议 90 天（到期后需要更新 Secret）
   - **Resource owner**: 选择你自己（个人仓库）
   - **Repository access**: 选择 **Only select repositories** → 勾选 `autoagents`
3. 在 **Permissions** 中设置以下权限：

   | 权限类别         | 权限名称            | 访问级别          |
   |-----------------|--------------------|-----------------:|
   | Repository      | **Actions**        | Read and Write   |
   | Repository      | **Contents**       | Read and Write   |
   | Repository      | **Issues**         | Read and Write   |
   | Repository      | **Metadata**       | Read-only (自动) |
   | Repository      | **Pull requests**  | Read and Write   |

4. 点击 **Generate token**，复制生成的令牌（格式 `github_pat_...`）

> **也可以使用 Classic PAT**：
> 如果 Fine-grained PAT 不生效，可以创建 Classic PAT（<https://github.com/settings/tokens/new>），
> 勾选 `repo` 和 `workflow` scope。

---

## 步骤 2：将令牌存为仓库 Secret

1. 打开 <https://github.com/gjyhj1234/autoagents/settings/secrets/actions>
2. 点击 **New repository secret**
3. 填写：
   - **Name**: `COPILOT_PAT`
   - **Secret**: 粘贴上一步复制的令牌
4. 点击 **Add secret**

---

## 步骤 3：验证

1. 去 Actions 页面 → 选择 **🤖 01 — Issue → Agent** 工作流
2. 点击 **Run workflow** 手动触发
3. 查看运行日志，应看到 `✅ Assigned issue #XX to Copilot.`
4. 如果仍然失败：
   - 检查令牌是否过期
   - 确认 Copilot cloud agent 已在仓库中启用
   - 确认令牌的 Repository access 包含 `autoagents` 仓库

---

## 步骤 4：处理现有的 PR #40

PR #40 是 Copilot 为 Task-01 创建的 Draft PR，所有检查已通过。
设置好 `COPILOT_PAT` 后，你可以：

**方式 A（推荐）：手动触发 Auto-Merge 工作流**
1. 打开 <https://github.com/gjyhj1234/autoagents/actions/workflows/03-auto-merge.yml>
2. 点击 **Run workflow**，输入 PR number: `40`
3. 工作流会自动将 Draft 转为 Ready，然后合并

**方式 B：手动合并**
1. 打开 PR #40
2. 点击 **Ready for review** 将 Draft 转为正式 PR
3. 点击 **Squash and merge**

---

## 完整自动化流程

设置完成后，完整的自动化流程如下：

```
Issue created (with agent-task label)
  │
  ▼
Workflow 01: 用 COPILOT_PAT 自动分配 Copilot
  │
  ▼
Copilot cloud agent: 创建 Draft PR
  │
  ▼
Workflow 04: 给 PR 添加 auto-merge 标签
  │
  ▼
Workflow 02: 运行测试 (backend/frontend/docker)
  │
  ▼
Workflow 03: 测试通过后
  ├── 将 Draft PR 转为 Ready-for-Review
  ├── 自动 Squash Merge
  ├── 关闭关联的 Issue
  ├── 触发 Workflow 01 → 分配下一个任务
  │
  ▼
循环直到所有 Task 完成
```

---

## 常见问题

### Q: COPILOT_PAT 过期了怎么办？
A: 重新创建 PAT，然后更新仓库 Secret。在此期间，你仍然可以手动在 Issue 页面
点击 **Assign to Agent → Copilot** 来触发 Copilot。

### Q: 为什么 GITHUB_TOKEN 不行？
A: GitHub 出于安全考虑，限制了 `GITHUB_TOKEN` 的能力：
1. 不能分配 Copilot agent（需要用户身份）
2. 触发的事件不会启动其他工作流（防止无限循环）

使用 PAT 可以绕过这两个限制。

### Q: 可以用 GitHub App 代替 PAT 吗？
A: 可以，但需要创建一个 GitHub App 并使用 user-to-server token。
对于个人仓库，PAT 是最简单的方案。
