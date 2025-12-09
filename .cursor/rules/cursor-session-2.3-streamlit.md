# Cursor AI 2025 종합 가이드 - Session 2.3: Streamlit 1.50+ 규칙

## 📌 개요

이 파일은 `.cursor/rules/40-dashboard-streamlit.mdc`에 저장되며, **Streamlit 데이터 앱 개발**에 자동으로 로드됩니다.

2025년 Streamlit 1.50.0의 혁신적인 기능들(Custom Components v2, Custom Themes, Top Navigation)을 활용합니다.

---

## 📄 파일 내용: `.cursor/rules/40-dashboard-streamlit.mdc`

```yaml
---
description: "Streamlit 1.50+ Enterprise Dashboards & Data Apps"
globs:
  - "src/dashboard/**/*.py"
  - "streamlit_app.py"
  - "pages/**/*.py"
alwaysApply: false
priority: 7
---

# Streamlit 1.50+ Standards for Enterprise Automation (2025)

## 🎯 Core Philosophy

1. **Multipage Apps**: Use `st.navigation()` with top navigation for complex dashboards
2. **Custom Components v2**: Build frameless, bidirectional UI components
3. **Session State as State Machine**: Manage app state explicitly (prevent re-runs)
4. **Type Safety**: Use Python 3.13 type hints, Pydantic for data validation
5. **FastAPI Backend Integration**: Streamlit frontend talks to FastAPI for heavy computation

---

## 📁 Project Structure

```
src/dashboard/
├── streamlit_app.py               # Main entry point
├── config.py                      # Configuration (API keys, theme, constants)
├── pages/
│   ├── 01_Dashboard.py           # Home/overview page
│   ├── 02_Users.py               # Users management
│   ├── 03_Analytics.py           # Analytics & reporting
│   ├── 04_Settings.py            # Admin settings
│   └── 05_AI_Agent.py            # AI agent interface
├── components/
│   ├── __init__.py
│   ├── charts.py                 # Chart components
│   ├── filters.py                # Filter widgets
│   ├── tables.py                 # Data table components
│   └── custom.py                 # Custom Streamlit components (v2)
├── services/
│   ├── __init__.py
│   ├── api_client.py             # FastAPI client (async)
│   ├── supabase.py               # Supabase queries
│   ├── cache.py                  # Caching layer (@st.cache_data)
│   └── auth.py                   # Authentication & session
├── utils/
│   ├── __init__.py
│   ├── formatting.py             # Format numbers, dates
│   ├── validation.py             # Input validation (Pydantic)
│   └── logging.py                # Structured logging
└── requirements.txt              # Python dependencies

# Theme customization (NEW in 1.50)
├── .streamlit/
│   ├── config.toml              # Base configuration
│   └── theme.json               # Custom light/dark themes
```

---

## 🔧 Core Patterns

### Pattern 1: Multipage App with Top Navigation (NEW 1.50)

```python
# streamlit_app.py
import streamlit as st
from pages import dashboard, users, analytics, settings

# Configure page
st.set_page_config(
    page_title="Enterprise Automation Suite",
    page_icon="🚀",
    layout="wide",
    initial_sidebar_state="expanded",
)

# Define navigation (TOP NAVIGATION - NEW!)
page = st.navigation(
    title="Navigation",
    pages=[
        st.Page(dashboard.show, title="Dashboard", icon="📊"),
        st.Page(users.show, title="Users", icon="👥"),
        st.Page(analytics.show, title="Analytics", icon="📈"),
        st.Page(settings.show, title="Settings", icon="⚙️"),
    ],
    position="top",  # Top navigation bar (NEW!)
)

# Initialize session state (state machine pattern)
if "initialized" not in st.session_state:
    st.session_state.initialized = True
    st.session_state.user_id = None
    st.session_state.filters = {}
    st.session_state.cache_timestamp = None

# Authentication check
if not st.session_state.user_id:
    st.warning("Please log in")
    st.stop()

# Run selected page
page.run()
```

**Key Features**:
- ✅ Top navigation (1.50 new feature)
- ✅ Centralized session state
- ✅ Authentication gate
- ✅ Multiple pages with clear entry points

---

### Pattern 2: Custom Component v2 (Frameless, Bidirectional)

```python
# components/custom.py (Streamlit Custom Component v2 - NEW!)
import streamlit as st
from streamlit.components.v1 import declare_component
import json

# Build custom component (JavaScript/React frontend)
# Compiled JS bundled, component receives data and sends back events

def interactive_table_editor(data: list[dict], key: str = None) -> list[dict]:
    """
    Custom component: Editable table with inline editing (bidirectional)
    
    Args:
        data: List of dicts (rows)
        key: Unique component key
    
    Returns:
        Updated data after user edits
    """
    # This uses the built component (pre-compiled)
    component_value = declare_component(
        "interactive_table_editor",
        url="http://localhost:3001",  # Dev mode
        # url="file://./components/build",  # Production
    )
    
    result = component_value(data=data, key=key)
    return result if result else data


# Usage in page
import streamlit as st
from components import custom

def show():
    st.header("Users Management")
    
    # Fetch users
    users = fetch_users_from_supabase()
    
    # Edit with custom component
    edited_users = custom.interactive_table_editor(users, key="user_table")
    
    # Detect changes and save
    if edited_users != users:
        save_users_to_supabase(edited_users)
        st.success("Users updated!")
```

**Key Points**:
- ✅ Frameless custom components (not wrapped in default Streamlit container)
- ✅ Bidirectional data flow (send data, receive updates)
- ✅ Real-time interactivity without full re-runs

---

### Pattern 3: Session State Management (State Machine)

```python
# pages/users.py
import streamlit as st
from services import api_client
from utils.validation import UserValidator
from pydantic import ValidationError

def show():
    st.header("Users Management")
    
    # Initialize session state for this page
    if "users_state" not in st.session_state:
        st.session_state.users_state = {
            "list": [],
            "selected_id": None,
            "form_mode": "view",  # "view" | "edit" | "create"
            "loading": False,
        }
    
    state = st.session_state.users_state
    
    # State machine: View users list
    if state["form_mode"] == "view":
        view_users_list(state)
    
    # State machine: Create new user
    elif state["form_mode"] == "create":
        create_user_form(state)
    
    # State machine: Edit user
    elif state["form_mode"] == "edit":
        edit_user_form(state)


def view_users_list(state: dict):
    """Display users in a table with actions"""
    col1, col2 = st.columns([10, 2])
    
    with col2:
        if st.button("➕ New User", use_container_width=True):
            state["form_mode"] = "create"
            st.rerun()
    
    # Fetch users (cached)
    users = fetch_users_cached()
    
    if not users:
        st.info("No users found")
        return
    
    # Display in columns
    cols = st.columns([2, 3, 3, 2])
    with cols[0]: st.text("ID")
    with cols[1]: st.text("Name")
    with cols[2]: st.text("Email")
    with cols[3]: st.text("Actions")
    
    st.divider()
    
    for user in users:
        cols = st.columns([2, 3, 3, 2])
        with cols[0]: st.text(user["id"])
        with cols[1]: st.text(user["name"])
        with cols[2]: st.text(user["email"])
        with cols[3]:
            col_a, col_b = st.columns(2)
            if col_a.button("Edit", key=f"edit_{user['id']}"):
                state["selected_id"] = user["id"]
                state["form_mode"] = "edit"
                st.rerun()
            if col_b.button("Delete", key=f"del_{user['id']}"):
                delete_user(user["id"])
                st.rerun()


def create_user_form(state: dict):
    """Form to create new user"""
    st.subheader("Create New User")
    
    with st.form("create_user_form", clear_on_submit=True):
        name = st.text_input("Name")
        email = st.text_input("Email")
        submitted = st.form_submit_button("Create")
    
    if submitted:
        # Validate with Pydantic
        try:
            user_data = UserValidator(name=name, email=email)
            
            # Call FastAPI backend
            response = api_client.create_user(user_data.dict())
            
            if response.success:
                st.success(f"User {name} created!")
                st.session_state.users_state["form_mode"] = "view"
                st.rerun()
            else:
                st.error(f"Error: {response.error}")
        
        except ValidationError as e:
            st.error(f"Validation error: {e.json()}")


# Cached data fetch
@st.cache_data(ttl=300)  # Cache for 5 minutes
def fetch_users_cached():
    """Fetch users from FastAPI backend"""
    response = api_client.get_users()
    return response.data if response.success else []


def delete_user(user_id: str):
    """Delete user via API"""
    response = api_client.delete_user(user_id)
    if response.success:
        st.success(f"User {user_id} deleted")
        # Clear cache to refresh
        st.cache_data.clear()
    else:
        st.error(f"Delete failed: {response.error}")
```

**Key Points**:
- ✅ State machine pattern (explicit state management)
- ✅ Pydantic validation (type-safe)
- ✅ FastAPI backend calls (heavy logic)
- ✅ `@st.cache_data` for performance (5min TTL)

---

### Pattern 4: Custom Themes (Light/Dark) - NEW 1.50

```toml
# .streamlit/config.toml
[theme]
base = "light"
primaryColor = "#2196F3"
backgroundColor = "#FFFFFF"
secondaryBackgroundColor = "#F5F5F5"
textColor = "#262730"
font = "sans serif"

# Custom font styling
[font]
monospace = "IBM Plex Mono"

# Dark theme support
[darkTheme]
enabled = true
base = "dark"
primaryColor = "#BB86FC"
backgroundColor = "#121212"
secondaryBackgroundColor = "#1E1E1E"
textColor = "#E1E1E1"
```

**Usage in Code**:
```python
import streamlit as st

# Detect light/dark mode at runtime
theme = st.context.theme

if theme.base == "dark":
    st.write("🌙 Dark mode detected!")
else:
    st.write("☀️ Light mode detected!")
```

---

### Pattern 5: FastAPI Client with Async

```python
# services/api_client.py
import httpx
import asyncio
from typing import TypeVar, Generic
from pydantic import BaseModel

T = TypeVar("T")

class ApiResponse(BaseModel, Generic[T]):
    success: bool
    data: T | None = None
    error: str | None = None

# Async client
class FastAPIClient:
    def __init__(self, base_url: str = "http://localhost:8000"):
        self.base_url = base_url
        self.client = httpx.AsyncClient(base_url=base_url)
    
    async def get_users(self) -> ApiResponse[list[dict]]:
        """Fetch users from FastAPI"""
        try:
            response = await self.client.get("/api/users")
            response.raise_for_status()
            return ApiResponse(success=True, data=response.json())
        except httpx.HTTPError as e:
            return ApiResponse(success=False, error=str(e))
    
    async def create_user(self, user_data: dict) -> ApiResponse[dict]:
        """Create user in FastAPI"""
        try:
            response = await self.client.post("/api/users", json=user_data)
            response.raise_for_status()
            return ApiResponse(success=True, data=response.json())
        except httpx.HTTPError as e:
            return ApiResponse(success=False, error=str(e))

# Synchronous wrapper for Streamlit
api_client = FastAPIClient()

def get_users() -> ApiResponse[list[dict]]:
    """Sync wrapper for Streamlit"""
    return asyncio.run(api_client.get_users())

def create_user(data: dict) -> ApiResponse[dict]:
    """Sync wrapper for Streamlit"""
    return asyncio.run(api_client.create_user(data))
```

---

## 🎨 UI Components (Built-in Streamlit)

### Metrics & Status

```python
import streamlit as st

# Metric with sparkline (NEW 1.50!)
col1, col2, col3 = st.columns(3)

col1.metric(
    label="Total Users",
    value=1234,
    delta=45,
    sparkline=[1200, 1210, 1220, 1234],  # NEW!
)

col2.metric(
    label="Revenue",
    value="$45,231",
    delta="12%",
    delta_color="inverse",
)

col3.metric(
    label="CPU Usage",
    value="72%",
    delta="-3%",
)
```

### Data Editing (NEW 1.50!)

```python
import streamlit as st
import pandas as pd

# Editable columns in DataFrame
data = pd.DataFrame({
    'Name': ['Alice', 'Bob', 'Charlie'],
    'Score': [95, 87, 92],
    'Status': ['Active', 'Inactive', 'Active'],
})

# NEW: Configure editable columns
editor_config = {
    'Score': st.column_config.NumberColumn('Score', min_value=0, max_value=100),
    'Status': st.column_config.SelectboxColumn(
        'Status',
        options=['Active', 'Inactive', 'Pending'],
    ),
}

edited_df = st.data_editor(data, column_config=editor_config)

# Detect changes
if not edited_df.equals(data):
    st.write("Changes detected:", edited_df)
    # Save to database
```

### Tabs & Containers

```python
import streamlit as st

# Top-level tabs
tab1, tab2, tab3 = st.tabs(["Overview", "Details", "Advanced"])

with tab1:
    st.write("Overview content")
    st.metric("Key Metric", value=42)

with tab2:
    st.write("Detailed view")
    st.dataframe(some_data)

with tab3:
    st.write("Advanced settings")
    st.slider("Confidence Level", 0, 100, 75)
```

---

## 🚫 Anti-Patterns (NEVER DO THIS)

```python
# ❌ WRONG: Global state without session_state
global user_data  # BAD!
user_data = fetch_users()  # Gets reset on every rerun

# ✅ CORRECT: Use session_state
if "user_data" not in st.session_state:
    st.session_state.user_data = fetch_users()
```

```python
# ❌ WRONG: Heavy computation on every rerun
data = heavy_ml_inference(input_data)  # Runs EVERY TIME!

# ✅ CORRECT: Cache with TTL
@st.cache_data(ttl=300)
def cached_inference(input_data):
    return heavy_ml_inference(input_data)

data = cached_inference(input_data)
```

```python
# ❌ WRONG: Form without form submission
for i in range(5):
    name = st.text_input(f"Name {i}")
    # Runs on every keystroke!

# ✅ CORRECT: Use st.form
with st.form("my_form"):
    for i in range(5):
        name = st.text_input(f"Name {i}")
    submitted = st.form_submit_button("Submit")

if submitted:
    # Process form data
    pass
```

---

## ✅ Deployment Checklist

Before pushing to production:

- [ ] All `@st.cache_data` has TTL (not permanent)
- [ ] FastAPI backend properly configured (error handling)
- [ ] Secrets loaded from environment variables
- [ ] No hardcoded API endpoints
- [ ] Custom themes tested (light & dark modes)
- [ ] Custom components built and bundled
- [ ] Error pages implemented (404, 500)
- [ ] Monitoring setup (Sentry or similar)

---

## 🎓 2025 Features

- ✅ **Top Navigation**: `position="top"` in `st.navigation()`
- ✅ **Custom Components v2**: Frameless, bidirectional data flow
- ✅ **Custom Themes**: Light & dark modes with `st.context.theme`
- ✅ **Sparklines**: `sparkline=` parameter in `st.metric()`
- ✅ **Cell Editing**: DataFrame column config with `SelectboxColumn`, etc.
- ✅ **PDF Rendering**: `st.pdf()` for document display
- ✅ **Multiselect Column**: Colorful editable lists in DataFrames

---

## 📚 Related Sessions

- **Session 2.2**: FastAPI (Backend for Streamlit)
- **Session 2.5**: Desktop Apps (Alternative to Streamlit)
- **Session 3**: Slash Commands for Streamlit generation

---

**Document Version**: 1.0 (2025-12-09)
**Framework**: Streamlit 1.50+
**Status**: Production-ready for 2025+
```

---

## 🚀 실무 팁

### 1. Streamlit + FastAPI 아키텍처
```
┌─────────────────┐
│  Streamlit UI   │ (Frontend, fast prototyping)
└────────┬────────┘
         │ HTTP/async
         ↓
┌─────────────────┐
│   FastAPI       │ (Backend, business logic)
└────────┬────────┘
         │ SQL
         ↓
┌─────────────────┐
│  Supabase/PG    │ (Database)
└─────────────────┘
```

### 2. 기업 자동화에서의 활용
- **대시보드**: Streamlit (빠른 개발)
- **API**: FastAPI (확장성, 마이크로서비스)
- **DB**: Supabase RLS (행 단위 보안)
- **Agent**: LangChain + FastAPI (비동기 작업)

### 3. 성능 최적화
```python
# 1. 캐싱 (가장 중요!)
@st.cache_data(ttl=300)
def fetch_expensive_data():
    ...

# 2. 선택적 재실행
@st.fragment  # Only reruns this section
def expensive_widget():
    ...

# 3. 상태 관리 (명시적)
st.session_state.my_state = value
```

---

**다음**: Session 2.4 (FastAPI) → Session 2.5 (.NET/Desktop) → Session 3 (Commands)
