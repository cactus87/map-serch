# Cursor AI 2025 종합 가이드 - Session 2.1: Next.js 16 & React 19 규칙

## 📌 개요

이 파일은 `.cursor/rules/10-web-nextjs-react.mdc`에 저장되며, **모든 TypeScript/JavaScript 웹 개발**에 자동으로 로드됩니다.

---

## 📄 파일 내용: `.cursor/rules/10-web-nextjs-react.mdc`

```yaml
---
description: "Next.js 16 App Router & React 19 Standards"
globs:
  - "src/**/*.{ts,tsx}"
  - "app/**/*.{ts,tsx}"
  - "*.config.{ts,js}"
  - "next.config.js"
alwaysApply: false
priority: 5
---

# Next.js 16 & React 19 Standards (2025)

## 🎯 Core Philosophy

1. **Server-First Architecture**: Default to Server Components unless interactivity is explicitly needed.
2. **React 19 Native**: Use latest APIs (`useActionState`, `use()`, `useFormStatus`, `useOptimistic`).
3. **Type Safety**: TypeScript strict mode. No `any`. Prefer `unknown` + type guards.
4. **Async Data**: Fetch data in Server Components via `async` functions.
5. **Progressive Enhancement**: Forms work without JavaScript; enhance with client-side interactivity.

---

## 🔹 Project Structure

```
src/
├── app/                           # Next.js App Router
│   ├── (auth)/
│   │   ├── login/page.tsx        # Route pages only
│   │   └── register/page.tsx
│   ├── dashboard/
│   │   ├── layout.tsx            # Layouts (can be async)
│   │   ├── page.tsx              # Page component (async by default)
│   │   └── [id]/page.tsx         # Dynamic routes
│   ├── api/                       # API routes (optional if using FastAPI)
│   └── layout.tsx                # Root layout
│
├── components/
│   ├── ui/                        # shadcn/ui components (DO NOT MODIFY)
│   │   ├── button.tsx
│   │   ├── card.tsx
│   │   └── ...
│   │
│   ├── layout/                    # Application layout components
│   │   ├── Navbar.tsx            # Navigation bar
│   │   ├── Sidebar.tsx           # Sidebar (if client-rendered)
│   │   └── Footer.tsx            # Footer
│   │
│   └── features/                  # Domain-specific components
│       ├── dashboard/
│       │   ├── StatsCard.tsx
│       │   └── Charts.tsx
│       ├── users/
│       │   ├── UserTable.tsx
│       │   └── UserForm.tsx
│       └── ...
│
├── lib/
│   ├── supabase.ts               # Supabase client (server + client variants)
│   ├── types.ts                  # Shared type definitions
│   ├── utils.ts                  # Utility functions
│   ├── api.ts                    # Fetch wrapper with error handling
│   └── validations.ts            # Zod schemas for forms
│
├── hooks/
│   ├── useAuth.ts                # Custom hooks (client-only)
│   ├── useData.ts
│   └── usePagination.ts
│
└── styles/
    └── globals.css               # Global Tailwind styles
```

---

## 🔧 Component Patterns

### Pattern 1: Server Component (Default)

```typescript
// app/dashboard/page.tsx
import { Database } from '@/types/supabase'

export const revalidate = 3600 // Cache for 1 hour (ISR)

export interface DashboardPageProps {}

export default async function DashboardPage({}: DashboardPageProps) {
  // Fetch data server-side
  const stats = await fetchDashboardStats()
  
  if (!stats) {
    throw new Error('Failed to load dashboard')
  }

  return (
    <div className="space-y-6">
      <h1 className="text-3xl font-bold">Dashboard</h1>
      
      {/* Pass data to client component */}
      <StatsGrid data={stats} />
    </div>
  )
}

// Helper function (server-side)
async function fetchDashboardStats(): Promise<Stats[] | null> {
  try {
    const response = await fetch('https://api.example.com/stats', {
      headers: {
        Authorization: `Bearer ${process.env.API_SECRET_KEY}`,
      },
    })
    
    if (!response.ok) throw new Error('API error')
    return response.json()
  } catch (error) {
    console.error('Failed to fetch stats:', error)
    return null
  }
}
```

**Key Points**:
- ✅ Async by default
- ✅ Direct database access (no API call overhead)
- ✅ Secrets safe (never exposed to client)
- ✅ Uses `revalidate` or `revalidatePath()` for caching

---

### Pattern 2: Client Component (Interactive Only)

```typescript
// components/features/dashboard/StatsGrid.tsx
'use client'

import { useState } from 'react'
import { Card } from '@/components/ui/card'
import type { Stats } from '@/lib/types'

export interface StatsGridProps {
  data: Stats[]
}

export function StatsGrid({ data }: StatsGridProps) {
  const [sortBy, setSortBy] = useState<'name' | 'value'>('name')

  const sorted = [...data].sort((a, b) => 
    sortBy === 'name' 
      ? a.name.localeCompare(b.name)
      : b.value - a.value
  )

  return (
    <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4">
      {sorted.map(stat => (
        <Card key={stat.id} className="p-4">
          <h3 className="font-semibold text-gray-600">{stat.name}</h3>
          <p className="text-2xl font-bold mt-2">{stat.value}</p>
        </Card>
      ))}
    </div>
  )
}
```

**Key Points**:
- ✅ Only `'use client'` when needed (hooks, event handlers, browser APIs)
- ✅ Props passed from Server Component
- ✅ No direct data fetching (get from parent)

---

### Pattern 3: React 19 Form with Server Action

```typescript
// app/users/create/page.tsx
import { createUser } from '@/app/actions'
import { UserForm } from '@/components/features/users/UserForm'

export default function CreateUserPage() {
  return <UserForm onSubmit={createUser} />
}

// app/actions.ts (server-side)
'use server'

import { revalidatePath } from 'next/cache'
import type { User } from '@/types/supabase'

export async function createUser(formData: FormData): Promise<{ 
  success: boolean
  error?: string
  data?: User 
}> {
  try {
    const name = formData.get('name') as string
    const email = formData.get('email') as string

    // Validate
    if (!name || !email) {
      return { success: false, error: 'Missing required fields' }
    }

    // Create in database
    const supabase = createClient()
    const { data, error } = await supabase
      .from('users')
      .insert({ name, email })
      .select()
      .single()

    if (error) throw error

    // Revalidate cache
    revalidatePath('/users')

    return { success: true, data }
  } catch (error) {
    console.error('Create user error:', error)
    return { success: false, error: 'Failed to create user' }
  }
}

// components/features/users/UserForm.tsx
'use client'

import { useActionState } from 'react'
import type { createUser } from '@/app/actions'

export interface UserFormProps {
  onSubmit: typeof createUser
}

export function UserForm({ onSubmit }: UserFormProps) {
  const [state, formAction, isPending] = useActionState(onSubmit, {
    success: false,
  })

  return (
    <form action={formAction} className="space-y-4">
      <div>
        <label htmlFor="name" className="block text-sm font-medium">
          Name
        </label>
        <input
          id="name"
          name="name"
          type="text"
          required
          className="mt-1 block w-full rounded border px-3 py-2"
        />
      </div>

      <div>
        <label htmlFor="email" className="block text-sm font-medium">
          Email
        </label>
        <input
          id="email"
          name="email"
          type="email"
          required
          className="mt-1 block w-full rounded border px-3 py-2"
        />
      </div>

      {state.error && (
        <div className="p-3 bg-red-100 text-red-800 rounded">
          {state.error}
        </div>
      )}

      <button
        type="submit"
        disabled={isPending}
        className="w-full bg-blue-600 text-white py-2 rounded disabled:opacity-50"
      >
        {isPending ? 'Creating...' : 'Create User'}
      </button>
    </form>
  )
}
```

**Key Points**:
- ✅ Server Action for mutation (`'use server'`)
- ✅ Client Component for form interaction (`'use client'` + `useActionState`)
- ✅ Progressive enhancement: form works without JavaScript
- ✅ Automatic revalidation after mutation

---

## 🎨 Styling Rules

### Tailwind CSS
```typescript
// ✅ GOOD: Utility-first
<div className="flex items-center gap-2 p-4 bg-white rounded-lg shadow">
  <h1 className="text-xl font-bold text-gray-900">Title</h1>
</div>

// ✅ GOOD: Responsive
<div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4">
  {items.map(item => <Item key={item.id} {...item} />)}
</div>

// ❌ BAD: Custom CSS (unless complex animation)
<div style={{ ... }}>
```

### shadcn/ui Import Pattern
```typescript
import { Button } from '@/components/ui/button'
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card'
import { Input } from '@/components/ui/input'
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/select'

// DO NOT modify shadcn components. If customization needed, create wrapper:
export function CustomButton({ variant = 'primary', ...props }) {
  return <Button variant={variant} className="custom-class" {...props} />
}
```

---

## 🔗 Database Patterns (Supabase)

### Server Component: Direct Query
```typescript
// app/users/page.tsx
import { createClient } from '@/lib/supabase/server'

export const revalidate = 60

export default async function UsersPage() {
  const supabase = createClient()
  
  // Direct query in Server Component
  const { data: users, error } = await supabase
    .from('users')
    .select('id, name, email, created_at')
    .order('created_at', { ascending: false })

  if (error) {
    throw new Error(`Failed to fetch users: ${error.message}`)
  }

  return (
    <div>
      <h1>Users</h1>
      <UserTable users={users} />
    </div>
  )
}
```

### Client Component: Fetch via API
```typescript
// components/features/users/UserTable.tsx
'use client'

import { useEffect, useState } from 'react'
import type { User } from '@/types/supabase'

export function UserTable({ initialUsers }: { initialUsers: User[] }) {
  const [users, setUsers] = useState(initialUsers)
  const [isLoading, setIsLoading] = useState(false)

  const refreshUsers = async () => {
    setIsLoading(true)
    try {
      const response = await fetch('/api/users')
      if (!response.ok) throw new Error('Failed to fetch')
      const data = await response.json()
      setUsers(data)
    } catch (error) {
      console.error('Error:', error)
    } finally {
      setIsLoading(false)
    }
  }

  return (
    <div>
      <button onClick={refreshUsers} disabled={isLoading}>
        {isLoading ? 'Refreshing...' : 'Refresh'}
      </button>
      {/* Render users */}
    </div>
  )
}
```

---

## 🚫 Anti-Patterns (NEVER DO THIS)

```typescript
// ❌ WRONG: useEffect for initial data in Server Component
export default function Page() {
  const [data, setData] = useState(null)
  useEffect(() => {
    fetch('/api/data').then(r => r.json()).then(setData)
  }, [])
  // BAD: Double render, waterfall, slower UX
}

// ✅ CORRECT: async Server Component
export default async function Page() {
  const data = await fetch('...').then(r => r.json())
  return <div>{data}</div>
}
```

```typescript
// ❌ WRONG: Mixing client and server code
export default function Page() {
  const secret = process.env.SECRET_KEY // 🚨 Exposed to client!
  return <div>{secret}</div>
}

// ✅ CORRECT: Server-side secrets
export default async function Page() {
  const result = await callSecureAPI(process.env.SECRET_KEY) // Safe
  return <div>{result}</div>
}
```

```typescript
// ❌ WRONG: 'use client' on everything
'use client'

export default function App() {
  return (
    <div>
      <Header /> {/* Now Header is also client! */}
      <Main />   {/* And Main! */}
    </div>
  )
}

// ✅ CORRECT: 'use client' on leaf components only
export default function App() {
  return (
    <div>
      <Header /> {/* Server Component */}
      <InteractiveChart /> {/* Client Component */}
    </div>
  )
}
```

---

## 📝 Type Definitions

```typescript
// lib/types.ts
import type { Database } from '@/types/supabase'

// User type from Supabase
export type User = Database['public']['Tables']['users']['Row']
export type UserInsert = Database['public']['Tables']['users']['Insert']

// Custom API response type
export interface ApiResponse<T = unknown> {
  success: boolean
  data?: T
  error?: {
    code: string
    message: string
  }
}

// Form validation schema (Zod)
import { z } from 'zod'

export const createUserSchema = z.object({
  name: z.string().min(1, 'Name is required'),
  email: z.string().email('Invalid email'),
})

export type CreateUserInput = z.infer<typeof createUserSchema>
```

---

## ✅ Code Review Checklist

Before shipping any code:

- [ ] No `any` types (run `npm run type-check`)
- [ ] No `'use client'` unless needed (hooks, browser APIs, event handlers)
- [ ] All async operations have error handling
- [ ] Forms use Server Actions (not `fetch`)
- [ ] Database queries only in Server Components
- [ ] Secrets never reach client bundle
- [ ] Components have JSDoc comments
- [ ] Props are typed with `interface`
- [ ] Tailwind classes used (no inline styles)
- [ ] shadcn/ui components imported correctly

---

## 🎓 2025 Features Used

- ✅ **React 19**: `useActionState`, `use()`, `useOptimistic`, `useFormStatus`
- ✅ **Next.js 16 App Router**: Server Components, Layouts, Middleware
- ✅ **Server Actions**: Form submission without API routes
- ✅ **ISR (Incremental Static Regeneration)**: `revalidate`, `revalidatePath`
- ✅ **TypeScript 5.4**: Const type parameters, decorator improvements

---

## 📚 Related Sessions

- **Session 2.2**: UI & Styling (Tailwind CSS, shadcn/ui)
- **Session 2.3**: Backend API (FastAPI)
- **Session 2.4**: Dashboard (Streamlit)
- **Session 2.5**: Desktop (.NET, WPF, MAUI)

---

**Document Version**: 1.0 (2025-12-09)
**Framework**: Next.js 16 + React 19
**Status**: Production-ready
```

---

## 💡 다음 단계

이제 `.cursor/rules/10-web-nextjs-react.mdc` 파일을 생성하는 방법:

1. **프로젝트 루트에서**:
   ```bash
   touch .cursor/rules/10-web-nextjs-react.mdc
   ```

2. **위의 전체 YAML/Markdown 블록을 복사하여** 파일에 붙여넣기

3. **검증**:
   ```bash
   # TypeScript 파일 열기
   code src/app/dashboard/page.tsx
   
   # Cursor Chat 열기 (Cmd/Ctrl + L)
   # 질문: "Next.js 규칙이 뭐야?"
   # 응답에 "Server Components", "React 19", "Server Actions" 언급 확인
   ```

4. **Git 커밋**:
   ```bash
   git add .cursor/rules/10-web-nextjs-react.mdc
   git commit -m "chore: add Next.js 16 & React 19 Cursor rules"
   ```

---

## 📋 Session 2 전체 파일 리스트

다음 순서대로 생성될 파일들:

1. ✅ **10-web-nextjs-react.mdc** (현재 문서)
2. 📄 **20-ui-tailwind-shadcn.mdc** (Shadow, Tailwind 패턴)
3. 📄 **30-backend-fastapi.mdc** (Python, FastAPI, SQLAlchemy)
4. 📄 **40-dashboard-streamlit.mdc** (Streamlit 1.50+ 패턴)
5. 📄 **50-desktop-dotnet.mdc** (.NET 10, C#, WPF/MAUI)
6. 📄 **60-automation-agents.mdc** (LangChain, CrewAI)

각 파일은 **독립적으로 사용 가능**하지만, 함께 로드되어 **하이브리드 프로젝트 전체를 커버**합니다.

---

**이제 준비 완료!** Session 2.2로 진행하시겠습니까? 아니면 다른 스택부터 시작하실까요?
