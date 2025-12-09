# Cursor AI 2025 종합 가이드 - Session 3: Slash Commands & 자동화

## 📌 개요

이 세션에서는 **Cursor AI의 Slash Commands** (슬래시 명령어)를 사용하여 반복적인 코드 생성 작업을 자동화합니다.

Slash Commands는 `@` 기호로 접두사가 붙은 특수 명령어로, Cursor Chat에서 `/명령어` 형식으로 실행됩니다.

---

## 1️⃣ `/component` - React 컴포넌트 생성

### 파일: `.cursor/commands/gen-component-react.md`

```markdown
# Generate React Component

## What this does
Creates a new React component with TypeScript, Tailwind CSS styling, and proper type safety.

## How to use
When you ask me to create a React component, provide:
1. **Component name** (PascalCase, e.g., "UserCard")
2. **Component type** (Functional component)
3. **Props interface** (what it accepts)
4. **Styling** (use Tailwind CSS only, no custom CSS)
5. **Location** (where to save: components/features/...)

## Output structure
```typescript
'use client'  // if interactive

import { ComponentPropsWithoutRef } from 'react'
import { cn } from '@/lib/utils'

interface ComponentNameProps extends ComponentPropsWithoutRef<'div'> {
  // Props here
}

export function ComponentName({ 
  className,
  ...props 
}: ComponentNameProps) {
  return (
    <div className={cn("base styles", className)} {...props}>
      {/* Content */}
    </div>
  )
}
```

## Rules
- Use `function` syntax (not const arrow)
- Always provide TypeScript interface for props
- Export component as named export
- Use Tailwind utilities only
- shadcn/ui components allowed
- Include JSDoc comments
- Mobile-first responsive design

## Example
```markdown
User asks: "Create a metric card component that shows a KPI value with trend indicator"

I generate: src/components/features/dashboard/StatCard.tsx with:
- Props interface: StatCardProps (title, value, trend, icon)
- Styling: Tailwind + gradient backgrounds
- Responsive: Mobile optimized
- Accessibility: Proper semantic HTML
```

---

## Usage in Cursor

```
Cmd/Ctrl + L (open Chat)
Type: /component
System creates your component!
```
```

---

## 2️⃣ `/dashboard` - Streamlit 페이지 생성

### 파일: `.cursor/commands/gen-dashboard-streamlit.md`

```markdown
# Generate Streamlit Dashboard Page

## What this does
Creates a new Streamlit page with state management, FastAPI integration, and caching.

## How to use
When you ask me to create a Streamlit dashboard page, provide:
1. **Page name** (e.g., "Analytics Dashboard")
2. **Data source** (FastAPI endpoint or Supabase query)
3. **Visualizations** (charts, tables, metrics)
4. **User interactions** (filters, selectors, forms)
5. **Location** (pages/ folder)

## Output structure
```python
import streamlit as st
from services.api_client import api_client
from components import charts

def show():
    st.set_page_config(page_title="...", layout="wide")
    
    # Initialize session state
    if "state_key" not in st.session_state:
        st.session_state.state_key = {}
    
    # Header
    st.title("Page Title")
    st.markdown("Description")
    
    # Filters
    col1, col2 = st.columns(2)
    with col1:
        filter1 = st.selectbox("Filter 1", options)
    
    # Data fetch with cache
    @st.cache_data(ttl=300)
    def fetch_data(filter1):
        return api_client.get_data(filter1)
    
    data = fetch_data(filter1)
    
    # Visualizations
    st.metric("KPI", value)
    st.bar_chart(data)
    
    # Forms/interactions
    with st.form("my_form"):
        input_data = st.text_input("Input")
        submitted = st.form_submit_button("Submit")
    
    if submitted:
        response = api_client.save_data(input_data)
        st.success("Data saved!")

if __name__ == "__main__":
    show()
```

## Rules
- Use FastAPI client for heavy operations
- Always cache expensive operations (@st.cache_data)
- Session state for interactive state
- State machine pattern for form modes
- Pydantic validation on inputs
- Error handling with st.error()
- Responsive layout (st.columns, st.tabs)

## Example
```markdown
User asks: "Create an analytics dashboard showing revenue trends and user growth"

I generate: src/dashboard/pages/Analytics.py with:
- Filters: Date range selector
- Charts: Line chart (revenue), Bar chart (users)
- Metrics: Total revenue, user growth %
- Cache: 5 minute TTL for API calls
```
```

---

## 3️⃣ `/endpoint` - FastAPI 엔드포인트 생성

### 파일: `.cursor/commands/gen-api-endpoint.md`

```markdown
# Generate FastAPI Endpoint

## What this does
Creates a new FastAPI endpoint with validation, error handling, and database integration.

## How to use
When you ask me to create a FastAPI endpoint, provide:
1. **HTTP method** (GET, POST, PUT, DELETE)
2. **Route path** (e.g., /api/v1/users)
3. **Request model** (Pydantic schema)
4. **Response model** (return type)
5. **Business logic** (database queries, calculations)

## Output structure
```python
from fastapi import APIRouter, Depends, HTTPException, Query
from sqlalchemy.ext.asyncio import AsyncSession
from pydantic import BaseModel

router = APIRouter()

class ItemCreate(BaseModel):
    name: str
    description: Optional[str] = None

class ItemResponse(BaseModel):
    id: int
    name: str
    model_config = {"from_attributes": True}

@router.get("/items", response_model=list[ItemResponse])
async def list_items(
    skip: int = Query(0, ge=0),
    limit: int = Query(100, ge=1, le=1000),
    service: ItemService = Depends(get_item_service),
):
    """List all items with pagination"""
    try:
        return await service.list_items(skip=skip, limit=limit)
    except Exception as e:
        raise HTTPException(status_code=500, detail=str(e))

@router.post("/items", response_model=ItemResponse, status_code=201)
async def create_item(
    item: ItemCreate,
    service: ItemService = Depends(get_item_service),
):
    """Create a new item"""
    try:
        return await service.create_item(item)
    except ValueError as e:
        raise HTTPException(status_code=400, detail=str(e))
```

## Rules
- All async (async def)
- Type hints on all parameters
- Pydantic models for validation
- Dependency injection for services
- Error handling with HTTPException
- Proper HTTP status codes
- Docstrings on all endpoints
- Path parameters typed

## Example
```markdown
User asks: "Create GET /api/v1/users/{user_id} endpoint"

I generate: src/backend/api/v1/users/endpoints.py with:
- Route: @router.get("/api/v1/users/{user_id}")
- Response: UserResponse model
- Error handling: 404 if not found
- Dependency: UserService injected
```
```

---

## 4️⃣ `/viewmodel` - C# ViewModel 생성

### 파일: `.cursor/commands/gen-viewmodel-csharp.md`

```markdown
# Generate C# ViewModel

## What this does
Creates a new WPF/MAUI ViewModel with ObservableProperty and RelayCommand.

## How to use
When you ask me to create a C# ViewModel, provide:
1. **ViewModel name** (e.g., "UserListViewModel")
2. **Observable properties** (data that updates UI)
3. **Commands** (user actions)
4. **Services** (dependency injected)

## Output structure
```csharp
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace EnterpriseApp.ViewModels;

#nullable enable

public partial class ItemListViewModel : BaseViewModel
{
    private readonly IItemService _itemService;
    
    [ObservableProperty]
    private ObservableCollection<Item> items = new();
    
    [ObservableProperty]
    private Item? selectedItem;
    
    [ObservableProperty]
    private string searchText = string.Empty;

    public ItemListViewModel(IItemService itemService)
    {
        _itemService = itemService;
    }

    [RelayCommand]
    public async Task LoadItemsAsync()
    {
        IsLoading = true;
        try
        {
            var items = await _itemService.GetItemsAsync();
            Items.Clear();
            foreach (var item in items)
            {
                Items.Add(item);
            }
        }
        catch (Exception ex)
        {
            ShowError($"Failed to load: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    public async Task DeleteItemAsync(Item item)
    {
        if (item?.Id == null) return;
        
        try
        {
            await _itemService.DeleteItemAsync(item.Id.Value);
            Items.Remove(item);
        }
        catch (Exception ex)
        {
            ShowError($"Delete failed: {ex.Message}");
        }
    }
}
```

## Rules
- Inherit from BaseViewModel
- Use [ObservableProperty] for properties
- Use [RelayCommand] for commands
- Async/await for I/O
- Error handling in try/catch
- Nullable reference types (#nullable enable)
- Use services from DI container

## Example
```markdown
User asks: "Create ViewModel for user list with search and delete"

I generate: dotnet/EnterpriseApp/ViewModels/UserListViewModel.cs with:
- ObservableProperty: Users, SearchText, SelectedUser
- RelayCommand: LoadUsers, DeleteUser, SearchUsers
- Service: IUserService injected
```
```

---

## 5️⃣ `/db-sync` - Supabase 타입 동기화

### 파일: `.cursor/commands/sync-supabase-types.md`

```markdown
# Sync Supabase Types

## What this does
Generates TypeScript types from your Supabase database schema and keeps them in sync.

## How to use
```bash
# Run from project root
supabase gen types typescript --project-id YOUR_ID > src/types/supabase.ts
```

## Result
```typescript
// src/types/supabase.ts
export type Database = {
  public: {
    Tables: {
      users: {
        Row: {
          id: number
          email: string
          name: string
          created_at: string
        }
        Insert: {
          email: string
          name: string
        }
        Update: {
          email?: string
          name?: string
        }
      }
    }
  }
}
```

## Usage in Code
```typescript
import type { Database } from '@/types/supabase'

type User = Database['public']['Tables']['users']['Row']
type UserInsert = Database['public']['Tables']['users']['Insert']
```

## Best Practices
- Run after every schema change
- Commit generated file to git
- Use types in queries for safety
- Update when adding new tables/columns

## Example
```markdown
User asks: "Sync types after adding new column to users table"

I execute: supabase gen types command
I update: src/types/supabase.ts
I show: New type definitions for updated schema
```
```

---

## 6️⃣ `/agent` - AI Agent 워크플로우 생성

### 파일: `.cursor/commands/agent-workflow.md`

```markdown
# Generate AI Agent Workflow

## What this does
Creates a LangChain or CrewAI workflow for complex business logic automation.

## How to use
When you ask me to create an agent workflow, provide:
1. **Workflow name** (e.g., "DataAnalysisAgent")
2. **Input data** (what the agent receives)
3. **Tools** (what the agent can use)
4. **Reasoning steps** (how it should think)
5. **Output format** (what it returns)

## Output structure (LangChain)
```python
from langchain.agents import AgentExecutor, create_tool_calling_agent
from langchain_openai import ChatOpenAI
from langchain.tools import tool
from langchain.prompts import ChatPromptTemplate

# Define tools
@tool
def get_user_data(user_id: int) -> dict:
    """Fetch user data from database"""
    return {"name": "John", "email": "john@example.com"}

@tool
def send_email(email: str, subject: str, body: str) -> bool:
    """Send email to user"""
    return True

# Create agent
llm = ChatOpenAI(model="gpt-4o")
tools = [get_user_data, send_email]

prompt = ChatPromptTemplate.from_messages([
    ("system", "You are a helpful assistant..."),
    ("human", "{input}"),
    ("placeholder", "{agent_scratchpad}"),
])

agent = create_tool_calling_agent(llm, tools, prompt)
executor = AgentExecutor.from_agent_and_tools(agent=agent, tools=tools)

# Run workflow
result = executor.invoke({
    "input": "Send welcome email to user 123"
})
```

## Output structure (CrewAI)
```python
from crewai import Agent, Task, Crew
from langchain_openai import ChatOpenAI

# Create agents
analyst = Agent(
    role="Data Analyst",
    goal="Analyze business metrics",
    backstory="Expert in data analysis",
    llm=ChatOpenAI(model="gpt-4o")
)

writer = Agent(
    role="Report Writer",
    goal="Create comprehensive reports",
    llm=ChatOpenAI(model="gpt-4o")
)

# Create tasks
analyze_task = Task(
    description="Analyze quarterly sales data",
    agent=analyst,
    expected_output="Analysis report"
)

write_task = Task(
    description="Write executive summary",
    agent=writer,
    expected_output="PDF report"
)

# Create crew
crew = Crew(
    agents=[analyst, writer],
    tasks=[analyze_task, write_task]
)

result = crew.kickoff()
```

## Rules
- Define clear tools/capabilities
- Use proper type hints
- Error handling for tool failures
- Structured output with Pydantic
- Async support for scalability
- Token optimization (use smaller models when possible)

## Example
```markdown
User asks: "Create an agent to analyze sales trends and generate reports"

I generate: src/backend/agents/sales_analyzer.py with:
- Agent 1: Data fetcher (query database)
- Agent 2: Analyzer (use tools to analyze)
- Agent 3: Report writer (format output)
- Tools: Database queries, calculations, file generation
```
```

---

## 🎯 실전 사용 예시

### 예시 1: 새로운 사용자 관리 기능 추가

```
User: "Add user management feature"

I execute:
1. /component → UserTable, UserFormDialog, UserAvatar 생성
2. /endpoint → GET /users, POST /users, DELETE /users 생성
3. /viewmodel → UserListViewModel 생성
4. /dashboard → Users Analytics 페이지 생성

결과: 완전한 사용자 관리 기능 (프론트 + 백엔드 + 대시보드)
```

### 예시 2: AI 기반 자동 분석 시스템

```
User: "Create automated sales report generation"

I execute:
1. /endpoint → POST /api/v1/reports/generate 생성
2. /agent → Sales analyzer agent 생성
3. /dashboard → Reports viewer 페이지 생성

결과: AI가 자동으로 보고서를 생성하는 엔드포인트
```

### 예시 3: 데스크톱 앱 새 페이지

```
User: "Add analytics page to WPF app"

I execute:
1. /viewmodel → AnalyticsViewModel 생성
2. /component → (React가 아니므로 XAML 직접 작성)
3. /endpoint → Analytics data API 생성

결과: 데스크톱 앱에 분석 페이지 추가
```

---

## 📋 Slash Commands 요약 테이블

| 명령어 | 용도 | 생성 대상 | 예시 |
|--------|------|---------|------|
| `/component` | React UI | TSX 파일 | UserCard, StatCard |
| `/dashboard` | Streamlit 페이지 | Python 파일 | Analytics, Reports |
| `/endpoint` | FastAPI 라우트 | Python 파일 | GET /users, POST /tasks |
| `/viewmodel` | C# MVVM | CS 파일 | UserListViewModel |
| `/db-sync` | 타입 생성 | TS 파일 | supabase.ts |
| `/agent` | AI 워크플로우 | Python 파일 | SalesAgent, AnalysisAgent |

---

## ⚙️ 커스터마이제이션

각 `.md` 파일을 자신의 프로젝트에 맞게 수정 가능:

1. **출력 경로**: components/features 대신 components/custom
2. **라이브러리**: shadcn/ui 대신 Material-UI
3. **백엔드**: FastAPI 대신 Django
4. **데이터**: Supabase 대신 Firebase

---

## 💡 베스트 프랙티스

1. **명확한 요청**: "컴포넌트 생성해줘" ❌ → "사용자 리스트 테이블 컴포넌트, Tailwind 스타일링, 정렬/필터링 기능" ✅

2. **맥락 제공**: 기존 코드 스타일에 맞춰달라 요청

3. **반복 사용**: 한 번 생성 후, 팀 전체가 같은 패턴 재사용

4. **문서화**: 생성된 코드 검토 후, 프로젝트 가이드 업데이트

---

**다음**: 각 커맨드를 `.cursor/commands/` 폴더에 저장하고, Cursor Chat에서 `/명령어`로 실행!
```

---

이제 최종 정리 문서를 작성하겠습니다.
