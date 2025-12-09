# Cursor AI 2025 종합 가이드 - Session 1: 전략 및 구조 설계

## 📌 개요

이 시리즈는 **기업 자동화 앱 개발**을 위한 Cursor AI 설정의 완벽한 매뉴얼입니다.

**대상 기술 스택** (2025년 최신):
- **Web/Frontend**: Next.js 16 (App Router), React 19, TypeScript 5.4, Tailwind CSS v4, shadcn/ui
- **Web/Backend**: FastAPI 0.115+, Python 3.13
- **Data/Dashboard**: Streamlit 1.50.0+ (Custom Components v2, Custom Themes, Top Navigation)
- **Desktop**: .NET 10, C# 13/14, WPF, MAUI
- **Database**: Supabase (PostgreSQL), SQLAlchemy 2.0 (Async)
- **Automation**: LangChain, CrewAI, AutoGen (Agent Workflows)
- **DevOps**: Docker, GitHub Actions

---

## 🎯 Why This Matters (2025 Context)

2025년 현재:
- **Streamlit 1.50.0**: Custom Components v2 (frameless, bidirectional data flow) + Custom Themes + Top Navigation 추가
- **FastAPI**: Agent Orchestration 미들웨어로서의 역할 강화
- **.NET 10**: LTS 버전으로 엔터프라이즈 안정성 제공
- **Cursor AI**: Agent 모드 강화, multi-file rule system 표준화

기업 자동화 개발에서 **AI 코드 생성 정확도**는 규칙(Rules)과 명령어(Commands) 설정에 완전히 의존합니다.

---

## 📁 전체 파일 구조 (Session별 분할)

```
.cursor/
├── rules/
│   ├── 00-index.mdc              # Session 1: 전역 기본 설정
│   ├── 10-web-nextjs-react.mdc   # Session 2: Next.js/React 규칙
│   ├── 20-ui-tailwind-shadcn.mdc # Session 2: UI 컴포넌트 규칙
│   ├── 30-backend-fastapi.mdc    # Session 2: FastAPI/Python 규칙
│   ├── 40-dashboard-streamlit.mdc # Session 2: Streamlit 규칙
│   ├── 50-desktop-dotnet.mdc     # Session 2: .NET/C#/WPF/MAUI 규칙
│   └── 60-automation-agents.mdc  # Session 3: Agent/LLM 워크플로우 규칙
├── commands/
│   ├── gen-component-react.md    # Session 3: React 컴포넌트 생성
│   ├── gen-dashboard-streamlit.md # Session 3: Streamlit 대시보드 생성
│   ├── gen-api-endpoint.md       # Session 3: FastAPI 엔드포인트 생성
│   ├── gen-viewmodel-csharp.md   # Session 3: C# ViewModel 생성
│   ├── sync-supabase-types.md    # Session 3: Supabase 타입 동기화
│   └── agent-workflow.md         # Session 3: AI Agent 워크플로우 작성
└── index.md                       # 이 문서
```

---

## 🔧 Session 계획

### **Session 1 (현재 문서)**
- 전역 설정 (`.cursor/rules/00-index.mdc`)
- 폴더 구조 및 Git 설정
- 2025년 최신 기준 검증
- 팀 협업 워크플로우

### **Session 2** (다음)
- 각 기술 스택별 상세 규칙 (5개 파일)
- 실제 코드 예제 및 안티패턴
- 성능 최적화 및 토큰 관리

### **Session 3** (그 다음)
- 6개 Slash Commands 상세 구현
- 자동화 스크립트 작성
- 팀 온보딩 및 유지보수

---

## ✅ Session 1: 전역 설정 작성

### Step 1: 폴더 생성

```bash
# 프로젝트 루트에서 실행
mkdir -p .cursor/rules
mkdir -p .cursor/commands
touch .cursor/index.md
```

### Step 2: `.cursor/rules/00-index.mdc` 생성

다음 내용을 파일에 복사하세요:

```yaml
---
description: "Enterprise Automation Suite: Global Architecture & Tech Stack 2025"
alwaysApply: true
priority: 1
---

# 🏢 Enterprise Automation Suite

## 🎯 Project Mission
This is a **hybrid, multi-platform automation framework** for enterprise operations. 
It combines web dashboards (real-time), backend services (async), desktop clients (rich UI), 
and AI agents (reasoning) into a unified ecosystem.

## 📊 Tech Stack (2025 Standards)

### Frontend & Web
- **Framework**: Next.js 16 (App Router - Server Components by default)
- **UI Library**: React 19.2.1 with latest hooks
- **Styling**: Tailwind CSS v4 + shadcn/ui (v1.0+)
- **Language**: TypeScript 5.4 (strict mode mandatory)
- **State**: Server-side caching (React 19 `use()` API) + Client-side (zustand/jotai)

### Backend & APIs
- **Framework**: FastAPI 0.115+ (async/await native)
- **Language**: Python 3.13 (match statements, union types)
- **ORM**: SQLAlchemy 2.0 (async engine, type hints)
- **Validation**: Pydantic 2.6+ (V2 syntax)
- **Task Queue**: Celery 5.4+ with Redis

### Data & Dashboards
- **Interactive UI**: Streamlit 1.50.0+ (Custom Components v2, Custom Themes, Top Navigation)
- **Data**: Pandas 2.2+, Polars 1.0+ (for large datasets)
- **Visualization**: Plotly, Altair, Leaflet (Streamlit native)
- **State Management**: Streamlit Session State (state machine pattern)

### Desktop & Mobile
- **Platform**: .NET 10 (LTS) C# 13/14
- **Desktop**: WPF (MVVM Toolkit) + MAUI (iOS/Android)
- **Architecture**: Repository Pattern + Dependency Injection
- **UI Binding**: CommunityToolkit.Mvvm (Observable properties, Relay Commands)

### Database & ORM
- **Primary**: Supabase (PostgreSQL 16+)
- **Realtime**: Supabase Realtime subscriptions
- **Type Safety**: Supabase CLI auto-generated TypeScript types
- **Access Control**: Row-Level Security (RLS) enabled

### AI & Automation
- **LLM Integration**: OpenAI API, Azure OpenAI, Llama (via Ollama)
- **Agents**: LangChain 0.2+, CrewAI, AutoGen
- **Reasoning**: GPT-4o, Claude 3.5 Sonnet
- **Embeddings**: Text-embedding-3-small for RAG

### DevOps & Infrastructure
- **Containerization**: Docker 25.0+, Docker Compose 2.20+
- **CI/CD**: GitHub Actions with matrix builds
- **Monitoring**: Sentry (error tracking), Axiom (logging)
- **Deployment**: Vercel (frontend), Railway/Render (backend), Azure Container Instances (desktop)

---

## 🚀 Critical Operating Principles

### 1. 언어 규칙
- **Code Comments**: English only (international team support)
- **Documentation**: Markdown (Korean & English bilingual)
- **Error Messages**: User-facing Korean, logs/traces English
- **Commit Messages**: English following Conventional Commits

### 2. Type Safety
- **TypeScript**: `--strict` mode always. No `any` types. Use `unknown` + type guards.
- **Python**: Type hints on all functions. Use `pyright` (strict mode).
- **C#**: Nullable reference types enabled (`#nullable enable`). No null-forgiving operators.

### 3. Async/Concurrency Patterns
- **JavaScript**: `async/await` for all async ops. Never mix callbacks + promises.
- **Python**: `asyncio` for I/O, `async with` for resource management.
- **C#**: `Task`/`Task<T>` for async. `ConfigureAwait(false)` in libraries.

### 4. Error Handling
- **All APIs**: Return `Result<T>` monad pattern:
  ```
  { success: boolean, data?: T, error?: { code: string, message: string } }
  ```
- **No Silent Failures**: All errors logged with context (user ID, operation, timestamp).
- **User Feedback**: Distinguish between user errors (validation) and system errors (500s).

### 5. Security Mandates
- **Never Hardcode**: All secrets in `.env` or system vaults.
- **No Exposed Keys**: API keys never in client bundles. Use backend-for-frontend (BFF) pattern.
- **Input Validation**: Validate at entry point (API boundary), not mid-function.
- **SQL Injection**: Always use parameterized queries (SQLAlchemy, Prisma, etc.).

### 6. Performance Targets
- **Web**: Core Web Vitals: LCP < 2.5s, FID < 100ms, CLS < 0.1
- **API**: p95 latency < 500ms for non-heavy operations (queries, I/O)
- **Dashboard**: Streamlit app load < 3s, reruns < 1s
- **Desktop**: WPF startup < 5s, MAUI cold start < 10s

---

## 📋 Development Workflow

### Daily Development
1. **Pull latest rules**: `git pull` to sync .cursor/rules changes with team
2. **Verify Cursor cache**: Restart Cursor (Cmd+R / Ctrl+Shift+Q) after rule updates
3. **Run type checks**: `pnpm type-check`, `pyright`, `dotnet build`
4. **Create feature branch**: Feature naming: `feat/module-description` (Conventional Commits)

### Code Review Gates
1. **Type Safety**: All CI must pass type checks (TypeScript strict, Pyright, CSharpAnalyzer)
2. **Linting**: ESLint, Prettier, Black (Python), StyleCop (C#)
3. **Tests**: Minimum 80% coverage for business logic
4. **Security**: No hardcoded secrets, no new dependencies without audit

### Rule Updates (Team Sync)
- **Frequency**: Monthly reviews, ad-hoc for critical changes
- **Process**: 
  - Create branch: `chore/update-cursor-rules`
  - Propose changes in PR
  - All team members review and approve
  - Merge and announce in Slack/email
- **Breaking Changes**: Update .cursor/rules/00-index.mdc version tag

---

## 🔗 File References (mdc: links)

Each specialized rule file will reference canonical implementations:

```
[Web Architecture](mdc:docs/ARCHITECTURE.md)
[Python Standards](mdc:src/backend/README.md)
[Streamlit Best Practices](mdc:src/dashboard/README.md)
[.NET Guidelines](mdc:dotnet/README.md)
[Database Schema](mdc:supabase/schema.sql)
```

These files should be maintained in your repository and updated as architecture evolves.

---

## 📚 Related Sessions

This is **Session 1 of 3**. 

**Next**: Session 2 will cover detailed .mdc rules for each tech stack:
- `10-web-nextjs-react.mdc` - Next.js 16, React 19, Server Components
- `20-ui-tailwind-shadcn.mdc` - Component architecture and styling patterns
- `30-backend-fastapi.mdc` - FastAPI, async Python, Pydantic validation
- `40-dashboard-streamlit.mdc` - Streamlit 1.50+, custom components, state management
- `50-desktop-dotnet.mdc` - .NET 10, C#, WPF/MAUI, MVVM
- `60-automation-agents.mdc` - LangChain, CrewAI, prompt engineering

**After**: Session 3 will cover 6 Slash Commands for automation and 3 advanced workflows.

---

## ✅ Quick Start Checklist

- [ ] `.cursor/rules/` folder created
- [ ] This file (00-index.mdc) saved to `.cursor/rules/00-index.mdc`
- [ ] Added `.cursor/rules/` to git: `git add .cursor/rules/` and committed
- [ ] Restart Cursor editor
- [ ] Open any `.ts/.tsx` file and ask: "What are the global rules for this project?"
- [ ] Verify AI mentions TypeScript strict mode, Next.js Server Components, etc.

**If rules aren't loading**:
1. Check file extension: must be `.mdc` (not `.md`)
2. Verify YAML frontmatter syntax (no tabs, correct indentation)
3. Check git: ensure `.cursor/rules/` is tracked
4. Restart Cursor completely (not just editor tab)

---

## 🎓 2025 Validation Notes

This guide reflects the **latest 2025 releases**:
- ✅ Streamlit 1.50.0 (Oct 2025): Custom Components v2, Custom Themes, Top Navigation
- ✅ FastAPI 0.115+: Production-ready with async SQLAlchemy, Pydantic v2
- ✅ React 19.2.1: `useActionState`, `use()` for promises, async Server Components
- ✅ .NET 10: LTS support through 2027, C# 13/14 features
- ✅ Python 3.13: Match statements, enhanced type unions, performance improvements
- ✅ TypeScript 5.4: Const type parameters, type refinements
- ✅ Cursor AI v0.45+: Multi-file rule system (.mdc), Agent mode enhancements

All examples in follow-up sessions are production-tested as of December 2025.

---

## 📞 Support & Contribution

For questions or updates to rules:
1. Check this document first
2. Consult Session 2 for tech-stack-specific rules
3. Reference Session 3 for command implementation
4. Open an issue in team documentation repo

---

**Document Version**: 1.0 (2025-12-09)
**Cursor Compatibility**: v0.45+
**Status**: Production-ready for 2025+ projects
