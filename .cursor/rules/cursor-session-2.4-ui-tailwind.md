# Cursor AI 2025 종합 가이드 - Session 2.4: UI 컴포넌트 & 스타일링 규칙

## 📌 개요

이 파일은 `.cursor/rules/20-ui-tailwind-shadcn.mdc`에 저장되며, **모든 UI/스타일 개발**에 자동으로 로드됩니다.

**핵심**: Tailwind CSS v4와 shadcn/ui를 활용한 일관된 디자인 시스템 구축.

---

## 📄 파일 내용: `.cursor/rules/20-ui-tailwind-shadcn.mdc`

```yaml
---
description: "Tailwind CSS v4, shadcn/ui Components & Design System"
globs:
  - "**/*.tsx"
  - "**/*.css"
  - "components/ui/**/*"
  - "components/features/**/*"
alwaysApply: false
priority: 4
---

# Tailwind CSS v4 & shadcn/ui Standards (2025)

## 🎯 Core Philosophy

1. **Utility-First**: Use Tailwind classes exclusively. No custom CSS unless animation is complex.
2. **Component Library**: shadcn/ui is source of truth for UI patterns.
3. **Type-Safe Props**: All component props are fully typed interfaces.
4. **Responsive First**: Mobile-first approach with responsive modifiers.
5. **Accessibility**: WCAG 2.1 AA compliance by default (shadcn/ui provides this).

---

## 📁 Component Structure

```
components/
├── ui/                            # shadcn/ui imported components
│   ├── button.tsx                 # DO NOT MODIFY
│   ├── card.tsx
│   ├── input.tsx
│   ├── select.tsx
│   ├── table.tsx
│   ├── tabs.tsx
│   ├── dialog.tsx
│   ├── dropdown-menu.tsx
│   ├── alert.tsx
│   ├── badge.tsx
│   ├── toast.tsx
│   └── [other shadcn components]
│
├── layout/                        # App-level layout components
│   ├── Navbar.tsx                 # Top navigation bar
│   ├── Sidebar.tsx                # Left sidebar (if client component)
│   ├── Footer.tsx                 # Bottom footer
│   └── MainLayout.tsx             # Wrapper layout
│
├── features/                      # Domain-specific feature components
│   ├── auth/
│   │   ├── LoginForm.tsx          # Login form
│   │   ├── RegisterForm.tsx       # Registration form
│   │   └── PasswordReset.tsx
│   │
│   ├── dashboard/
│   │   ├── StatCard.tsx           # Metric card component
│   │   ├── ChartWidget.tsx        # Chart container
│   │   ├── KPIDashboard.tsx       # Full dashboard layout
│   │   └── DataTable.tsx          # Data table with sorting
│   │
│   ├── users/
│   │   ├── UserCard.tsx           # User profile card
│   │   ├── UserTable.tsx          # Users list table
│   │   ├── UserFormDialog.tsx     # User CRUD dialog
│   │   └── UserAvatar.tsx         # Avatar component
│   │
│   └── common/
│       ├── EmptyState.tsx         # Empty state illustration
│       ├── LoadingSpinner.tsx     # Loading indicator
│       ├── Breadcrumb.tsx         # Navigation breadcrumb
│       └── Pagination.tsx         # Pagination controls
│
└── hooks/
    ├── useMediaQuery.ts           # Responsive breakpoint hook
    ├── useDebounce.ts             # Debounce hook
    ├── useLocalStorage.ts         # LocalStorage hook
    └── usePagination.ts           # Pagination state
```

---

## 🎨 Tailwind CSS v4 Patterns

### Pattern 1: Utility-First Button Component

```typescript
// components/features/dashboard/StatCard.tsx
import { Card } from '@/components/ui/card'
import type { ReactNode } from 'react'

interface StatCardProps {
  title: string
  value: string | number
  subtitle?: string
  icon?: ReactNode
  trend?: 'up' | 'down' | 'neutral'
  onClick?: () => void
}

export function StatCard({ 
  title, 
  value, 
  subtitle, 
  icon, 
  trend,
  onClick 
}: StatCardProps) {
  const trendColor = trend === 'up' 
    ? 'text-green-600' 
    : trend === 'down' 
    ? 'text-red-600' 
    : 'text-gray-600'

  return (
    <Card 
      className="p-6 hover:shadow-lg transition-shadow cursor-pointer"
      onClick={onClick}
    >
      <div className="flex items-start justify-between">
        {/* Left side: content */}
        <div className="flex-1">
          <p className="text-sm font-medium text-gray-500">{title}</p>
          <p className="text-3xl font-bold text-gray-900 mt-2">{value}</p>
          {subtitle && (
            <p className={`text-sm mt-1 ${trendColor}`}>{subtitle}</p>
          )}
        </div>

        {/* Right side: icon */}
        {icon && (
          <div className="ml-4 flex-shrink-0">
            <div className="h-12 w-12 rounded-lg bg-blue-100 flex items-center justify-center">
              {icon}
            </div>
          </div>
        )}
      </div>
    </Card>
  )
}
```

**Key Patterns**:
- ✅ Tailwind classes for styling (no CSS files)
- ✅ Responsive: `flex`, `grid`, breakpoints (`md:`, `lg:`)
- ✅ Spacing: `p-6` (padding), `mt-2` (margin-top), `gap-4`
- ✅ Colors: `text-gray-900`, `bg-blue-100`, `text-green-600`
- ✅ Responsive states: `hover:shadow-lg`, `focus:ring-2`, `disabled:opacity-50`

---

### Pattern 2: Responsive Grid Layout

```typescript
// components/features/dashboard/KPIDashboard.tsx
'use client'

import { StatCard } from './StatCard'
import { ChartWidget } from './ChartWidget'
import { Users, TrendingUp, Clock, CheckCircle } from 'lucide-react'

interface MetricsData {
  totalUsers: number
  revenue: number
  activeProjects: number
  completedTasks: number
}

export function KPIDashboard({ data }: { data: MetricsData }) {
  return (
    <div className="space-y-6">
      {/* Header */}
      <div>
        <h1 className="text-3xl font-bold text-gray-900">Dashboard</h1>
        <p className="text-gray-600 mt-1">Welcome back! Here's your overview.</p>
      </div>

      {/* KPI Cards - Responsive Grid */}
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4">
        <StatCard
          title="Total Users"
          value={data.totalUsers}
          subtitle="+12% from last month"
          icon={<Users className="h-6 w-6 text-blue-600" />}
          trend="up"
        />
        <StatCard
          title="Revenue"
          value={`$${data.revenue.toLocaleString()}`}
          subtitle="+8% from last month"
          icon={<TrendingUp className="h-6 w-6 text-green-600" />}
          trend="up"
        />
        <StatCard
          title="Active Projects"
          value={data.activeProjects}
          subtitle="5 in progress"
          icon={<Clock className="h-6 w-6 text-purple-600" />}
          trend="neutral"
        />
        <StatCard
          title="Completed Tasks"
          value={data.completedTasks}
          subtitle="98% completion rate"
          icon={<CheckCircle className="h-6 w-6 text-emerald-600" />}
          trend="up"
        />
      </div>

      {/* Charts - Two Column Layout */}
      <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
        <ChartWidget title="Revenue Trend" type="line" />
        <ChartWidget title="User Distribution" type="pie" />
      </div>

      {/* Wide Section */}
      <div className="lg:col-span-2">
        <ChartWidget title="Activity Timeline" type="area" />
      </div>
    </div>
  )
}
```

**Responsive Breakdown**:
```
Mobile (< 768px):
  └─ 1 column (grid-cols-1)
  
Tablet (768px - 1024px):
  └─ 2 columns (md:grid-cols-2)
  
Desktop (> 1024px):
  └─ 4 columns (lg:grid-cols-4)
  
Charts:
  Mobile: 1 column
  Desktop: 2 columns (lg:grid-cols-2)
```

---

### Pattern 3: Tailwind CSS Dark Mode

```typescript
// components/layout/MainLayout.tsx
'use client'

import { useEffect, useState } from 'react'
import { Moon, Sun } from 'lucide-react'
import { Button } from '@/components/ui/button'

export function MainLayout({ children }: { children: React.ReactNode }) {
  const [isDark, setIsDark] = useState(false)

  useEffect(() => {
    const prefersDark = window.matchMedia('(prefers-color-scheme: dark)').matches
    setIsDark(prefersDark)
    if (prefersDark) {
      document.documentElement.classList.add('dark')
    }
  }, [])

  const toggleTheme = () => {
    setIsDark(!isDark)
    if (!isDark) {
      document.documentElement.classList.add('dark')
    } else {
      document.documentElement.classList.remove('dark')
    }
  }

  return (
    <div className="min-h-screen bg-white dark:bg-slate-950 text-gray-900 dark:text-gray-50 transition-colors">
      {/* Header */}
      <header className="border-b border-gray-200 dark:border-slate-800 sticky top-0 z-50 bg-white dark:bg-slate-900">
        <div className="max-w-7xl mx-auto px-4 py-4 flex justify-between items-center">
          <h1 className="text-2xl font-bold">Enterprise App</h1>
          
          <Button
            variant="ghost"
            size="icon"
            onClick={toggleTheme}
            className="rounded-full"
          >
            {isDark ? (
              <Sun className="h-5 w-5" />
            ) : (
              <Moon className="h-5 w-5" />
            )}
          </Button>
        </div>
      </header>

      {/* Main Content */}
      <main className="max-w-7xl mx-auto px-4 py-8">
        {children}
      </main>
    </div>
  )
}
```

**Dark Mode Classes**:
- `dark:bg-slate-950` - Dark background
- `dark:text-gray-50` - Light text in dark mode
- `dark:border-slate-800` - Dark borders
- Automatic via `prefers-color-scheme` or manual toggle

---

## 🧩 shadcn/ui Component Patterns

### Pattern 1: Form with Validation

```typescript
// components/features/users/UserFormDialog.tsx
'use client'

import { useState } from 'react'
import { useActionState } from 'react'
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogHeader,
  DialogTitle,
  DialogTrigger,
} from '@/components/ui/dialog'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import { Alert, AlertDescription } from '@/components/ui/alert'
import { createUser } from '@/app/actions'

interface UserFormDialogProps {
  onSuccess?: () => void
}

export function UserFormDialog({ onSuccess }: UserFormDialogProps) {
  const [open, setOpen] = useState(false)
  const [state, formAction, isPending] = useActionState(createUser, {
    success: false,
  })

  const handleOpenChange = (newOpen: boolean) => {
    setOpen(newOpen)
    if (!newOpen && state.success) {
      onSuccess?.()
    }
  }

  return (
    <Dialog open={open} onOpenChange={handleOpenChange}>
      <DialogTrigger asChild>
        <Button>Add User</Button>
      </DialogTrigger>

      <DialogContent>
        <DialogHeader>
          <DialogTitle>Create New User</DialogTitle>
          <DialogDescription>
            Fill in the form below to create a new user.
          </DialogDescription>
        </DialogHeader>

        <form action={formAction} className="space-y-4">
          {/* Error Alert */}
          {state.error && (
            <Alert variant="destructive">
              <AlertDescription>{state.error}</AlertDescription>
            </Alert>
          )}

          {/* Name Field */}
          <div className="space-y-2">
            <Label htmlFor="name">Full Name</Label>
            <Input
              id="name"
              name="name"
              placeholder="John Doe"
              required
              disabled={isPending}
            />
          </div>

          {/* Email Field */}
          <div className="space-y-2">
            <Label htmlFor="email">Email Address</Label>
            <Input
              id="email"
              name="email"
              type="email"
              placeholder="john@example.com"
              required
              disabled={isPending}
            />
          </div>

          {/* Submit Button */}
          <Button
            type="submit"
            className="w-full"
            disabled={isPending}
          >
            {isPending ? 'Creating...' : 'Create User'}
          </Button>
        </form>
      </DialogContent>
    </Dialog>
  )
}
```

---

### Pattern 2: Data Table with Sorting & Filtering

```typescript
// components/features/users/UserTable.tsx
'use client'

import { useState, useMemo } from 'react'
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from '@/components/ui/table'
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from '@/components/ui/select'
import { Input } from '@/components/ui/input'
import { Button } from '@/components/ui/button'
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuTrigger,
} from '@/components/ui/dropdown-menu'
import { MoreHorizontal } from 'lucide-react'
import type { User } from '@/types/supabase'

interface UserTableProps {
  users: User[]
  onEdit?: (user: User) => void
  onDelete?: (userId: number) => void
}

export function UserTable({ users, onEdit, onDelete }: UserTableProps) {
  const [search, setSearch] = useState('')
  const [sortBy, setSortBy] = useState<'name' | 'email' | 'created_at'>('name')
  const [filterRole, setFilterRole] = useState<'all' | 'admin' | 'user'>('all')

  // Filtered and sorted data
  const filteredUsers = useMemo(() => {
    let result = users

    // Search filter
    if (search) {
      result = result.filter(
        user =>
          user.name.toLowerCase().includes(search.toLowerCase()) ||
          user.email.toLowerCase().includes(search.toLowerCase())
      )
    }

    // Role filter
    if (filterRole !== 'all') {
      result = result.filter(user =>
        filterRole === 'admin' ? user.is_admin : !user.is_admin
      )
    }

    // Sort
    return result.sort((a, b) => {
      if (sortBy === 'name') {
        return a.name.localeCompare(b.name)
      } else if (sortBy === 'email') {
        return a.email.localeCompare(b.email)
      } else {
        return new Date(b.created_at).getTime() - new Date(a.created_at).getTime()
      }
    })
  }, [users, search, sortBy, filterRole])

  return (
    <div className="space-y-4">
      {/* Filters */}
      <div className="flex gap-4 flex-wrap">
        <Input
          placeholder="Search users..."
          value={search}
          onChange={e => setSearch(e.target.value)}
          className="flex-1 min-w-[200px]"
        />

        <Select value={filterRole} onValueChange={setFilterRole}>
          <SelectTrigger className="w-[150px]">
            <SelectValue />
          </SelectTrigger>
          <SelectContent>
            <SelectItem value="all">All Roles</SelectItem>
            <SelectItem value="admin">Admin</SelectItem>
            <SelectItem value="user">User</SelectItem>
          </SelectContent>
        </Select>

        <Select value={sortBy} onValueChange={setSortBy}>
          <SelectTrigger className="w-[150px]">
            <SelectValue />
          </SelectTrigger>
          <SelectContent>
            <SelectItem value="name">Sort by Name</SelectItem>
            <SelectItem value="email">Sort by Email</SelectItem>
            <SelectItem value="created_at">Sort by Date</SelectItem>
          </SelectContent>
        </Select>
      </div>

      {/* Table */}
      <div className="border rounded-lg overflow-hidden">
        <Table>
          <TableHeader className="bg-gray-50 dark:bg-slate-800">
            <TableRow>
              <TableHead>Name</TableHead>
              <TableHead>Email</TableHead>
              <TableHead>Role</TableHead>
              <TableHead>Joined</TableHead>
              <TableHead className="text-right">Actions</TableHead>
            </TableRow>
          </TableHeader>

          <TableBody>
            {filteredUsers.map(user => (
              <TableRow key={user.id} className="hover:bg-gray-50 dark:hover:bg-slate-800">
                <TableCell className="font-medium">{user.name}</TableCell>
                <TableCell className="text-gray-600 dark:text-gray-400">{user.email}</TableCell>
                <TableCell>
                  <span className={`px-2 py-1 rounded-full text-xs font-semibold ${
                    user.is_admin
                      ? 'bg-purple-100 text-purple-800 dark:bg-purple-900 dark:text-purple-100'
                      : 'bg-blue-100 text-blue-800 dark:bg-blue-900 dark:text-blue-100'
                  }`}>
                    {user.is_admin ? 'Admin' : 'User'}
                  </span>
                </TableCell>
                <TableCell>
                  {new Date(user.created_at).toLocaleDateString()}
                </TableCell>
                <TableCell className="text-right">
                  <DropdownMenu>
                    <DropdownMenuTrigger asChild>
                      <Button variant="ghost" size="icon">
                        <MoreHorizontal className="h-4 w-4" />
                      </Button>
                    </DropdownMenuTrigger>
                    <DropdownMenuContent align="end">
                      <DropdownMenuItem onClick={() => onEdit?.(user)}>
                        Edit
                      </DropdownMenuItem>
                      <DropdownMenuItem
                        onClick={() => onDelete?.(user.id)}
                        className="text-red-600"
                      >
                        Delete
                      </DropdownMenuItem>
                    </DropdownMenuContent>
                  </DropdownMenu>
                </TableCell>
              </TableRow>
            ))}
          </TableBody>
        </Table>
      </div>

      {filteredUsers.length === 0 && (
        <div className="text-center py-8 text-gray-600">
          No users found. Try adjusting your filters.
        </div>
      )}

      <p className="text-sm text-gray-600">
        Showing {filteredUsers.length} of {users.length} users
      </p>
    </div>
  )
}
```

---

## 🎨 Color System & Tailwind Config

```javascript
// tailwind.config.ts
import type { Config } from 'tailwindcss'

const config: Config = {
  darkMode: 'class',
  content: [
    './src/**/*.{js,ts,jsx,tsx}',
    './components/**/*.{js,ts,jsx,tsx}',
  ],
  theme: {
    extend: {
      colors: {
        primary: '#2563eb',        // Blue
        secondary: '#64748b',      // Slate
        success: '#10b981',        // Green
        warning: '#f59e0b',        // Amber
        danger: '#ef4444',         // Red
      },
      spacing: {
        '128': '32rem',
        '144': '36rem',
      },
      borderRadius: {
        '3xl': '1.5rem',
      },
    },
  },
  plugins: [require('tailwindcss-animate')],
}

export default config
```

---

## 🚫 Anti-Patterns (NEVER DO THIS)

```typescript
// ❌ WRONG: Inline styles
<div style={{ color: 'blue', padding: '16px', fontSize: '14px' }}>
  Content
</div>

// ✅ CORRECT: Tailwind classes
<div className="text-blue-600 p-4 text-sm">
  Content
</div>
```

```typescript
// ❌ WRONG: Custom CSS for simple styling
// styles.css
.custom-card {
  padding: 24px;
  border-radius: 8px;
  background: white;
  box-shadow: 0 1px 3px rgba(0,0,0,0.1);
}

// ✅ CORRECT: Use shadcn/ui Card + Tailwind
import { Card } from '@/components/ui/card'
<Card className="p-6">Content</Card>
```

```typescript
// ❌ WRONG: Modifying shadcn/ui components
// components/ui/button.tsx - DON'T EDIT THIS!
export function Button() { ... }

// ✅ CORRECT: Create wrapper if customization needed
export function CustomButton({ variant, ...props }) {
  return <Button variant={variant} className="custom-class" {...props} />
}
```

---

## ✅ Code Review Checklist

- [ ] No inline styles (use Tailwind)
- [ ] No custom CSS (unless complex animation)
- [ ] All UI components use shadcn/ui
- [ ] Responsive design tested on mobile/tablet/desktop
- [ ] Dark mode support (dark: prefix used)
- [ ] Accessibility: aria labels, semantic HTML
- [ ] Props fully typed with interfaces
- [ ] No direct shadcn/ui modifications
- [ ] Color palette consistent across app
- [ ] Spacing consistent (use Tailwind spacing scale)

---

## 🎓 2025 Features

- ✅ **Tailwind CSS v4**: New engine, faster compilation
- ✅ **shadcn/ui v1.0+**: Stable component API
- ✅ **Dark Mode**: Native CSS class support
- ✅ **Responsive Design**: Mobile-first with breakpoints
- ✅ **Accessibility**: WCAG 2.1 AA built-in

---

**Document Version**: 1.0 (2025-12-09)
**Status**: Production-ready
```

---

## 💡 핵심 설치 및 사용

### Tailwind CSS 초기화
```bash
npm install -D tailwindcss postcss autoprefixer
npx tailwindcss init -p
```

### shadcn/ui 설치
```bash
npx shadcn-ui@latest init
# 선택: TypeScript, Tailwind CSS, Light/Dark mode
```

### 컴포넌트 추가
```bash
npx shadcn-ui@latest add button
npx shadcn-ui@latest add card
npx shadcn-ui@latest add input
npx shadcn-ui@latest add table
# ... 필요한 컴포넌트들
```

---

**다음**: Session 2.5 (.NET Desktop)로 계속...
