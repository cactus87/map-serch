# Cursor AI 2025 종합 가이드 - Session 2.2: FastAPI & Python 규칙

## 📌 개요

이 파일은 `.cursor/rules/30-backend-fastapi.mdc`에 저장되며, **모든 Python/FastAPI 백엔드 개발**에 자동으로 로드됩니다.

**핵심**: 기업 자동화의 "두뇌"역할을 하는 비동기 API 서버를 구축합니다.

---

## 📄 파일 내용: `.cursor/rules/30-backend-fastapi.mdc`

```yaml
---
description: "FastAPI 0.115+, Python 3.13, Async/SQLAlchemy Standards"
globs:
  - "src/backend/**/*.py"
  - "src/api/**/*.py"
  - "fastapi_app.py"
  - "main.py"
alwaysApply: false
priority: 6
---

# FastAPI 0.115+ Standards for Enterprise Backend (2025)

## 🎯 Core Philosophy

1. **Async by Default**: All I/O operations are async (database, API calls)
2. **Type Safety**: Full type hints, Pydantic v2 validation, generics
3. **DDD (Domain-Driven Design)**: Separate entities, repositories, services, schemas
4. **Error Handling**: Consistent error responses with proper HTTP status codes
5. **Testing First**: Write tests alongside code, achieve 80%+ coverage

---

## 📁 Project Structure

```
src/backend/
├── main.py                        # FastAPI app creation
├── config.py                      # Environment config (pydantic-settings)
├── requirements.txt               # Python dependencies
│
├── api/
│   ├── __init__.py
│   ├── v1/                        # API versioning
│   │   ├── __init__.py
│   │   ├── dependencies.py        # Shared dependencies (DB session, auth)
│   │   ├── users/
│   │   │   ├── __init__.py
│   │   │   ├── endpoints.py       # Route handlers
│   │   │   ├── schemas.py         # Request/Response models (Pydantic)
│   │   │   └── service.py         # Business logic
│   │   ├── tasks/
│   │   │   ├── endpoints.py
│   │   │   ├── schemas.py
│   │   │   └── service.py
│   │   └── agents/
│   │       ├── endpoints.py       # AI agent endpoints
│   │       ├── schemas.py
│   │       └── service.py
│   └── health.py                  # Health check
│
├── models/                        # SQLAlchemy ORM models
│   ├── __init__.py
│   ├── user.py
│   ├── task.py
│   └── agent_result.py
│
├── repositories/                  # Data access layer (Repository pattern)
│   ├── __init__.py
│   ├── base.py                    # Base repository with common methods
│   ├── user_repository.py
│   ├── task_repository.py
│   └── agent_repository.py
│
├── services/                      # Business logic layer
│   ├── __init__.py
│   ├── user_service.py
│   ├── task_service.py
│   ├── agent_service.py           # AI agent orchestration
│   └── cache.py                   # Caching logic (Redis)
│
├── database/
│   ├── __init__.py
│   ├── session.py                 # SQLAlchemy session management
│   ├── migrations/                # Alembic migrations
│   │   └── env.py
│   └── seed.py                    # Database seeding
│
├── schemas/                       # Pydantic models for validation
│   ├── __init__.py
│   ├── user.py
│   ├── task.py
│   └── responses.py               # Common response schemas
│
├── utils/
│   ├── __init__.py
│   ├── logging.py                 # Structured logging
│   ├── exceptions.py              # Custom exceptions
│   ├── validators.py              # Custom validators (Pydantic)
│   └── helpers.py                 # Utility functions
│
├── middleware/
│   ├── __init__.py
│   ├── error_handler.py           # Global error handling
│   ├── logging.py                 # Request/response logging
│   └── cors.py                    # CORS configuration
│
└── tests/
    ├── __init__.py
    ├── conftest.py                # Pytest fixtures
    ├── test_users.py
    ├── test_tasks.py
    ├── test_agents.py
    └── integration/
        └── test_workflows.py      # End-to-end tests
```

---

## 🔧 Core Patterns

### Pattern 1: FastAPI App with Configuration

```python
# src/backend/main.py
import logging
from contextlib import asynccontextmanager
from fastapi import FastAPI, Request
from fastapi.middleware.cors import CORSMiddleware
from fastapi.exceptions import RequestValidationError
import uvicorn

from config import settings
from api.v1 import users, tasks, agents
from api import health
from middleware.error_handler import setup_error_handlers
from middleware.logging import LoggingMiddleware

# Configure logging
logging.basicConfig(
    level=settings.LOG_LEVEL,
    format='%(asctime)s - %(name)s - %(levelname)s - %(message)s'
)
logger = logging.getLogger(__name__)

# Lifespan context for startup/shutdown
@asynccontextmanager
async def lifespan(app: FastAPI):
    """Startup and shutdown logic"""
    # Startup
    logger.info("🚀 FastAPI starting...")
    # Initialize database connections, caches, etc.
    yield
    # Shutdown
    logger.info("⏹️  FastAPI shutting down...")
    # Clean up resources

# Create FastAPI app
app = FastAPI(
    title="Enterprise Automation API",
    description="API for enterprise automation suite",
    version="1.0.0",
    docs_url="/api/docs" if settings.DEBUG else None,
    lifespan=lifespan,
)

# CORS configuration
app.add_middleware(
    CORSMiddleware,
    allow_origins=settings.CORS_ORIGINS,
    allow_credentials=True,
    allow_methods=["*"],
    allow_headers=["*"],
)

# Custom logging middleware
app.add_middleware(LoggingMiddleware)

# Setup error handlers
setup_error_handlers(app)

# Include routers
app.include_router(health.router, prefix="/api/health", tags=["health"])
app.include_router(users.router, prefix="/api/v1/users", tags=["users"])
app.include_router(tasks.router, prefix="/api/v1/tasks", tags=["tasks"])
app.include_router(agents.router, prefix="/api/v1/agents", tags=["agents"])

@app.get("/")
async def root():
    return {"message": "Enterprise Automation API"}

if __name__ == "__main__":
    uvicorn.run(
        "main:app",
        host=settings.API_HOST,
        port=settings.API_PORT,
        reload=settings.DEBUG,
        log_level=settings.LOG_LEVEL.lower(),
    )
```

---

### Pattern 2: Pydantic Models (Request/Response Validation)

```python
# src/backend/schemas/user.py
from pydantic import BaseModel, EmailStr, Field
from datetime import datetime
from typing import Optional

class UserCreate(BaseModel):
    """Request schema for creating a user"""
    name: str = Field(..., min_length=1, max_length=100)
    email: EmailStr
    is_admin: bool = False

    model_config = {
        "json_schema_extra": {
            "example": {
                "name": "John Doe",
                "email": "john@example.com",
                "is_admin": False,
            }
        }
    }

class UserUpdate(BaseModel):
    """Request schema for updating a user"""
    name: Optional[str] = Field(None, min_length=1, max_length=100)
    email: Optional[EmailStr] = None
    is_admin: Optional[bool] = None

class UserResponse(BaseModel):
    """Response schema for user data"""
    id: int
    name: str
    email: str
    is_admin: bool
    created_at: datetime
    updated_at: datetime

    model_config = {"from_attributes": True}  # ORM mode

class UserListResponse(BaseModel):
    """Paginated user response"""
    items: list[UserResponse]
    total: int
    page: int
    page_size: int
    total_pages: int
```

---

### Pattern 3: SQLAlchemy Async Models

```python
# src/backend/models/user.py
from sqlalchemy import Column, Integer, String, Boolean, DateTime, func
from sqlalchemy.ext.declarative import declarative_base
from datetime import datetime

Base = declarative_base()

class User(Base):
    __tablename__ = "users"
    __table_args__ = (
        {"schema": "public"},  # Supabase schema
    )

    id = Column(Integer, primary_key=True)
    name = Column(String(100), nullable=False)
    email = Column(String(255), unique=True, nullable=False, index=True)
    is_admin = Column(Boolean, default=False)
    is_active = Column(Boolean, default=True)
    created_at = Column(DateTime(timezone=True), server_default=func.now())
    updated_at = Column(DateTime(timezone=True), onupdate=func.now())

    def __repr__(self) -> str:
        return f"<User(id={self.id}, email={self.email})>"
```

---

### Pattern 4: Repository Pattern (Data Access)

```python
# src/backend/repositories/base.py
from typing import TypeVar, Generic, Optional, List, Any
from sqlalchemy.ext.asyncio import AsyncSession
from sqlalchemy import select, func
from pydantic import BaseModel

T = TypeVar("T")
S = TypeVar("S", bound=BaseModel)

class BaseRepository(Generic[T, S]):
    """Base repository with common CRUD operations"""
    
    def __init__(self, session: AsyncSession, model: type[T]):
        self.session = session
        self.model = model
    
    async def create(self, schema: S) -> T:
        """Create a new record"""
        db_object = self.model(**schema.model_dump())
        self.session.add(db_object)
        await self.session.commit()
        await self.session.refresh(db_object)
        return db_object
    
    async def get_by_id(self, id: Any) -> Optional[T]:
        """Get record by ID"""
        query = select(self.model).where(self.model.id == id)
        result = await self.session.execute(query)
        return result.scalar_one_or_none()
    
    async def list_all(self, skip: int = 0, limit: int = 100) -> List[T]:
        """List all records with pagination"""
        query = select(self.model).offset(skip).limit(limit)
        result = await self.session.execute(query)
        return result.scalars().all()
    
    async def update(self, id: Any, schema: S) -> Optional[T]:
        """Update a record"""
        db_object = await self.get_by_id(id)
        if not db_object:
            return None
        
        for key, value in schema.model_dump(exclude_unset=True).items():
            setattr(db_object, key, value)
        
        self.session.add(db_object)
        await self.session.commit()
        await self.session.refresh(db_object)
        return db_object
    
    async def delete(self, id: Any) -> bool:
        """Delete a record"""
        db_object = await self.get_by_id(id)
        if not db_object:
            return False
        
        await self.session.delete(db_object)
        await self.session.commit()
        return True
    
    async def count(self) -> int:
        """Count total records"""
        query = select(func.count()).select_from(self.model)
        result = await self.session.execute(query)
        return result.scalar()

# src/backend/repositories/user_repository.py
from sqlalchemy import select
from sqlalchemy.ext.asyncio import AsyncSession
from models.user import User
from schemas.user import UserCreate, UserUpdate
from .base import BaseRepository

class UserRepository(BaseRepository[User, UserCreate]):
    def __init__(self, session: AsyncSession):
        super().__init__(session, User)
    
    async def get_by_email(self, email: str) -> Optional[User]:
        """Get user by email"""
        query = select(self.model).where(self.model.email == email)
        result = await self.session.execute(query)
        return result.scalar_one_or_none()
    
    async def list_admins(self) -> List[User]:
        """Get all admin users"""
        query = select(self.model).where(self.model.is_admin == True)
        result = await self.session.execute(query)
        return result.scalars().all()
```

---

### Pattern 5: Service Layer (Business Logic)

```python
# src/backend/services/user_service.py
from typing import Optional, List
from sqlalchemy.ext.asyncio import AsyncSession
from repositories.user_repository import UserRepository
from schemas.user import UserCreate, UserUpdate, UserResponse
from models.user import User
from utils.exceptions import UserNotFoundError, UserAlreadyExistsError

class UserService:
    """Business logic for user operations"""
    
    def __init__(self, repository: UserRepository):
        self.repository = repository
    
    async def create_user(self, user_data: UserCreate) -> UserResponse:
        """Create a new user with validation"""
        # Check if user already exists
        existing = await self.repository.get_by_email(user_data.email)
        if existing:
            raise UserAlreadyExistsError(f"User with email {user_data.email} already exists")
        
        # Create user
        user = await self.repository.create(user_data)
        return UserResponse.model_validate(user)
    
    async def get_user(self, user_id: int) -> UserResponse:
        """Get user by ID"""
        user = await self.repository.get_by_id(user_id)
        if not user:
            raise UserNotFoundError(f"User with ID {user_id} not found")
        return UserResponse.model_validate(user)
    
    async def list_users(self, skip: int = 0, limit: int = 100):
        """List users with pagination"""
        users = await self.repository.list_all(skip=skip, limit=limit)
        total = await self.repository.count()
        
        return {
            "items": [UserResponse.model_validate(u) for u in users],
            "total": total,
            "page": skip // limit + 1,
            "page_size": limit,
            "total_pages": (total + limit - 1) // limit,
        }
    
    async def update_user(self, user_id: int, user_data: UserUpdate) -> UserResponse:
        """Update user"""
        user = await self.repository.update(user_id, user_data)
        if not user:
            raise UserNotFoundError(f"User with ID {user_id} not found")
        return UserResponse.model_validate(user)
    
    async def delete_user(self, user_id: int) -> bool:
        """Delete user"""
        if not await self.repository.delete(user_id):
            raise UserNotFoundError(f"User with ID {user_id} not found")
        return True
```

---

### Pattern 6: API Endpoints with Dependency Injection

```python
# src/backend/api/v1/dependencies.py
from typing import AsyncGenerator
from sqlalchemy.ext.asyncio import AsyncSession
from database.session import get_async_session
from repositories.user_repository import UserRepository
from services.user_service import UserService

async def get_user_repository(
    session: AsyncSession = Depends(get_async_session)
) -> UserRepository:
    """Dependency: User repository"""
    return UserRepository(session)

async def get_user_service(
    repository: UserRepository = Depends(get_user_repository)
) -> UserService:
    """Dependency: User service"""
    return UserService(repository)

# src/backend/api/v1/users/endpoints.py
from fastapi import APIRouter, Depends, HTTPException, Query
from schemas.user import UserCreate, UserUpdate, UserResponse
from services.user_service import UserService
from api.v1.dependencies import get_user_service
from utils.exceptions import UserNotFoundError, UserAlreadyExistsError

router = APIRouter()

@router.post("/", response_model=UserResponse, status_code=201)
async def create_user(
    user_data: UserCreate,
    service: UserService = Depends(get_user_service)
):
    """Create a new user"""
    try:
        return await service.create_user(user_data)
    except UserAlreadyExistsError as e:
        raise HTTPException(status_code=409, detail=str(e))

@router.get("/{user_id}", response_model=UserResponse)
async def get_user(
    user_id: int,
    service: UserService = Depends(get_user_service)
):
    """Get user by ID"""
    try:
        return await service.get_user(user_id)
    except UserNotFoundError as e:
        raise HTTPException(status_code=404, detail=str(e))

@router.get("/", response_model=dict)
async def list_users(
    skip: int = Query(0, ge=0),
    limit: int = Query(100, ge=1, le=1000),
    service: UserService = Depends(get_user_service)
):
    """List all users with pagination"""
    return await service.list_users(skip=skip, limit=limit)

@router.put("/{user_id}", response_model=UserResponse)
async def update_user(
    user_id: int,
    user_data: UserUpdate,
    service: UserService = Depends(get_user_service)
):
    """Update user"""
    try:
        return await service.update_user(user_id, user_data)
    except UserNotFoundError as e:
        raise HTTPException(status_code=404, detail=str(e))

@router.delete("/{user_id}", status_code=204)
async def delete_user(
    user_id: int,
    service: UserService = Depends(get_user_service)
):
    """Delete user"""
    try:
        await service.delete_user(user_id)
    except UserNotFoundError as e:
        raise HTTPException(status_code=404, detail=str(e))
```

---

## 📝 Configuration Management

```python
# src/backend/config.py
from pydantic_settings import BaseSettings
from typing import List

class Settings(BaseSettings):
    """Application settings"""
    
    # API
    API_HOST: str = "0.0.0.0"
    API_PORT: int = 8000
    DEBUG: bool = False
    LOG_LEVEL: str = "INFO"
    
    # Database (Supabase)
    DATABASE_URL: str  # postgresql://user:pass@host:5432/db
    DATABASE_ECHO: bool = False  # SQL logging
    
    # CORS
    CORS_ORIGINS: List[str] = ["http://localhost:3000"]
    
    # Redis (Caching)
    REDIS_URL: str = "redis://localhost:6379"
    
    # Security
    SECRET_KEY: str  # For JWT tokens
    ALGORITHM: str = "HS256"
    ACCESS_TOKEN_EXPIRE_MINUTES: int = 30
    
    # LLM/AI
    OPENAI_API_KEY: str
    LLM_MODEL: str = "gpt-4o"
    
    class Config:
        env_file = ".env"
        case_sensitive = True

settings = Settings()
```

---

## ✅ Testing Pattern

```python
# src/backend/tests/test_users.py
import pytest
from sqlalchemy.ext.asyncio import create_async_engine, AsyncSession
from sqlalchemy.pool import StaticPool

from main import app
from models.user import Base
from schemas.user import UserCreate

@pytest.fixture
async def test_db():
    """Create test database"""
    engine = create_async_engine(
        "sqlite+aiosqlite:///:memory:",
        connect_args={"check_same_thread": False},
        poolclass=StaticPool,
    )
    
    async with engine.begin() as conn:
        await conn.run_sync(Base.metadata.create_all)
    
    async with AsyncSession(engine) as session:
        yield session

@pytest.fixture
async def client():
    """FastAPI test client"""
    from fastapi.testclient import TestClient
    return TestClient(app)

@pytest.mark.asyncio
async def test_create_user(test_db):
    """Test user creation"""
    from repositories.user_repository import UserRepository
    from services.user_service import UserService
    
    repository = UserRepository(test_db)
    service = UserService(repository)
    
    user_data = UserCreate(name="Test User", email="test@example.com")
    user = await service.create_user(user_data)
    
    assert user.name == "Test User"
    assert user.email == "test@example.com"
    assert user.id is not None
```

---

## 🚫 Anti-Patterns (NEVER DO THIS)

```python
# ❌ WRONG: Synchronous blocking I/O
def get_user(user_id: int):
    user = db.query(User).get(user_id)  # Blocks entire thread!
    return user

# ✅ CORRECT: Async
async def get_user(user_id: int):
    result = await session.execute(select(User).where(User.id == user_id))
    return result.scalar_one_or_none()
```

```python
# ❌ WRONG: Business logic in endpoints
@app.post("/users")
async def create_user(data: UserCreate):
    # Business logic here is bad!
    if data.email already exists: raise...
    user = User(**data.dict())
    db.add(user)
    db.commit()

# ✅ CORRECT: Service layer
@app.post("/users")
async def create_user(data: UserCreate, service: UserService = Depends()):
    return await service.create_user(data)
```

```python
# ❌ WRONG: No error handling
@app.get("/users/{user_id}")
async def get_user(user_id: int, service: UserService = Depends()):
    return await service.get_user(user_id)  # What if user not found?

# ✅ CORRECT: Proper error handling
@app.get("/users/{user_id}")
async def get_user(user_id: int, service: UserService = Depends()):
    try:
        return await service.get_user(user_id)
    except UserNotFoundError as e:
        raise HTTPException(status_code=404, detail=str(e))
```

---

## 📊 Architecture Diagram

```
┌─────────────────────────────────────────┐
│   FastAPI Application                   │
│  ┌──────────────────────────────────┐  │
│  │  HTTP Endpoints (Async)          │  │
│  └──────────────┬───────────────────┘  │
│                 │                       │
│  ┌──────────────▼───────────────────┐  │
│  │  Dependency Injection Layer      │  │
│  │  (Services, Repositories)        │  │
│  └──────────────┬───────────────────┘  │
│                 │                       │
│  ┌──────────────▼───────────────────┐  │
│  │  Service Layer (Business Logic)  │  │
│  │  - Validation                    │  │
│  │  - Error handling                │  │
│  │  - Orchestration                 │  │
│  └──────────────┬───────────────────┘  │
│                 │                       │
│  ┌──────────────▼───────────────────┐  │
│  │  Repository Layer (Data Access)  │  │
│  │  - Database queries              │  │
│  │  - Transaction management        │  │
│  └──────────────┬───────────────────┘  │
│                 │                       │
│  ┌──────────────▼───────────────────┐  │
│  │  SQLAlchemy ORM (Async)          │  │
│  └──────────────┬───────────────────┘  │
│                 │                       │
└─────────────────┼───────────────────────┘
                  │
         ┌────────▼─────────┐
         │  Supabase/       │
         │  PostgreSQL      │
         └──────────────────┘
```

---

## ✅ Code Review Checklist

- [ ] All functions have type hints
- [ ] All I/O operations are async
- [ ] Repository pattern used (no direct DB in endpoints)
- [ ] Service layer handles business logic
- [ ] Pydantic validation on all inputs
- [ ] Error handling with proper HTTP status codes
- [ ] No hardcoded secrets (use environment variables)
- [ ] Tests covering happy path + error cases
- [ ] Logging at appropriate levels (info, error, debug)
- [ ] Documentation in docstrings

---

## 🎓 2025 Features

- ✅ **FastAPI 0.115+**: Latest async improvements, dependency injection
- ✅ **Python 3.13**: Type hints, match statements, performance
- ✅ **SQLAlchemy 2.0+**: Async engine, type hints, modern patterns
- ✅ **Pydantic v2**: V2 syntax, JSON schema improvements, validators
- ✅ **async/await**: Throughout entire stack

---

## 📚 Related Sessions

- **Session 2.1**: Next.js Frontend (consumes this API)
- **Session 2.3**: Streamlit Dashboard (consumes this API)
- **Session 3**: Slash Command for endpoint generation

---

**Document Version**: 1.0 (2025-12-09)
**Framework**: FastAPI 0.115+
**Status**: Production-ready
```

---

## 💡 핵심 설정 예시

### requirements.txt
```
fastapi==0.115.0
uvicorn[standard]==0.28.0
sqlalchemy[asyncio]==2.0.25
pydantic==2.6.0
pydantic-settings==2.1.0
psycopg[binary]==3.18.0
httpx==0.26.0
pytest==7.4.3
pytest-asyncio==0.23.2
```

### .env 파일
```
DATABASE_URL=postgresql+asyncpg://user:password@localhost:5432/automation_db
REDIS_URL=redis://localhost:6379
SECRET_KEY=your-secret-key-here
OPENAI_API_KEY=sk-...
DEBUG=True
LOG_LEVEL=DEBUG
```

### 실행 방법
```bash
# 개발
uvicorn main:app --reload

# 프로덕션
gunicorn main:app --workers 4 --worker-class uvicorn.workers.UvicornWorker
```

---

**다음**: Session 2.4 (UI Components & Styling) → Session 2.5 (.NET Desktop)
