# Product Requirements Document (PRD)
# LMP-Link: Location-based Matching Platform v1.0

**Version:** 1.0  
**Date:** 2025-12-08  
**Author:** Development Team  
**Status:** Active Development

---

## 1. Executive Summary

LMP-Link is a location-based matching platform for connecting disability service users with support workers. The system replaces manual Excel-based workflows with an intelligent map interface, radius-based filtering, and AI-powered recommendations. This document defines requirements for version 1.0 release, targeting a 4-6 week development cycle for single-developer implementation.

**Key Objectives:**
- Reduce matching time from 30 minutes to under 3 minutes per case
- Provide real-time map visualization with radius-based search
- Integrate AI-powered compatibility analysis via n8n + Claude 3.5
- Deploy Windows desktop (MSIX) and Android mobile (APK) applications

---

## 2. Product Vision & Goals

### 2.1 Vision Statement

Transform disability support service matching from a manual, time-consuming process into a streamlined, data-driven workflow that prioritizes proximity, compatibility, and user preferences through intuitive visual interfaces and intelligent automation.

### 2.2 Business Goals

1. **Efficiency:** Cut coordinator workload by 50%+ through automation
2. **Quality:** Reduce mismatch rate by 20% through better compatibility analysis
3. **Scale:** Support 200+ users, 1,000+ assistants per agency without performance degradation
4. **User Experience:** Achieve 4/5+ satisfaction score from coordinators within first month

### 2.3 Success Metrics (v1.0)

| Metric | Target | Measurement |
|--------|--------|-------------|
| App start time | <2s | Cold launch to map ready |
| Map load time | <3s | Initial marker rendering |
| Radius search response | <500ms | Filter update after radius change |
| AI analysis turnaround | <10s | Request to result display |
| Coordinator satisfaction | 4+/5 | Post-deployment survey |
| Critical bugs | 0 | At release time |

---

## 3. Target Audience & User Personas

### 3.1 Primary User: Agency Coordinator

**Profile:**
- Role: Disability service coordinator
- Platform: Windows 10/11 desktop
- Technical skill: Intermediate (familiar with Excel, email)
- Pain points: Excel chaos, phone tag with assistants, manual distance calculation
- Goals: Fast matching, reduce no-shows, balance assistant workload

**Use Cases:**
1. Search for nearby assistants when new user registered
2. Filter candidates by time availability, gender preference, vehicle ownership
3. Receive AI recommendations for complex matching scenarios
4. Confirm match and notify assistant via app

### 3.2 Secondary User: Support Worker (Assistant)

**Profile:**
- Role: Disability support worker
- Platform: Android smartphone (9+)
- Technical skill: Basic (uses messaging apps, maps)
- Pain points: Last-minute schedule changes, unclear job details, long commutes
- Goals: Stable schedule, nearby assignments, clear job requirements

**Use Cases:**
1. View today's assignments
2. Receive match proposals (notification)
3. Accept/decline proposals
4. Update profile preferences (availability, skip list)

---

## 4. Functional Requirements

### 4.1 Core Features (Must-Have for v1.0)

#### F1: Map-Based Exploration
- **FR1.1:** Display all users and assistants as map markers (Naver Maps v3)
- **FR1.2:** User markers: blue circles (30x30px), Assistant markers: orange squares (24x24px)
- **FR1.3:** Marker clustering when zoomed out (1,000+ markers supported)
- **FR1.4:** Click user marker → draw 3km radius circle immediately (<100ms)
- **FR1.5:** Filter markers by radius (1km / 3km / 5km presets)
- **FR1.6:** Out-of-radius assistants dimmed (50% opacity)

#### F2: Radius-Based Search
- **FR2.1:** Real-time distance calculation using PostGIS ST_DWithin
- **FR2.2:** Filter assistant list in right panel to show only in-radius candidates
- **FR2.3:** Display distance in km (2 decimal places)
- **FR2.4:** Sort by distance (nearest first)

#### F3: Advanced Filtering
- **FR3.1:** Filter by time availability (weekday AM/PM, weekend)
- **FR3.2:** Filter by gender preference (any/male/female)
- **FR3.3:** Filter by vehicle ownership (include/exclude)
- **FR3.4:** Exclude assistants on user's skip list
- **FR3.5:** Filters applied as AND logic
- **FR3.6:** "Reset filters" button to clear all

#### F4: AI-Powered Recommendations
- **FR4.1:** Coordinator selects 1-3 assistant candidates
- **FR4.2:** Click "AI Analysis" button → trigger n8n webhook
- **FR4.3:** n8n workflow:
  - Fetch user context (requirements, preferences)
  - Fetch assistant contexts (experience, reviews)
  - Call Claude 3.5 LLM with comparison prompt
  - Store results in Supabase (score 0-100, reason text)
  - Broadcast via Realtime channel
- **FR4.4:** Display TOP 3 recommendations with score badges (e.g., "92/100")
- **FR4.5:** Show 2-3 sentence natural language explanation per recommendation
- **FR4.6:** Loading state: progress bar during analysis (5-10s typical)

#### F5: Matching Workflow
- **FR5.1:** Click "Propose Match" button on assistant card → open comparison modal
- **FR5.2:** Modal layout:
  - Left column: User details (name, address, requirements)
  - Right column: Assistant details (name, distance, experience)
  - Center: Compatibility bar (0-100, green gradient)
  - Bottom: AI recommendation reason (if available)
- **FR5.3:** "Confirm Match" button → save to `matches` table (status: confirmed)
- **FR5.4:** "Cancel" button → close modal without saving
- **FR5.5:** After confirmation → trigger Supabase Realtime event → notify assistant mobile app

#### F6: Authentication & Authorization
- **FR6.1:** Email/password login via Supabase Auth
- **FR6.2:** Role-based access:
  - Coordinator: full access to own agency data
  - Assistant: read-only access to own profile and assignments
- **FR6.3:** Row-level security (RLS) policies enforce agency isolation
- **FR6.4:** Session persists across app restarts (LocalStorage)
- **FR6.5:** Logout button clears session and redirects to login page

### 4.2 Secondary Features (Nice-to-Have for v1.0)

- **SF1:** Search bar for user name/ID lookup
- **SF2:** Layer toggles (show/hide users, show/hide assistants)
- **SF3:** Map zoom controls overlay
- **SF4:** Quick stats in AppBar (e.g., "Today: 12 matches processed / 2 pending")

### 4.3 Excluded from v1.0 (Future Roadmap)

- Real-time GPS tracking for assistants
- SMS/KakaoTalk notifications
- Web admin dashboard for agency managers
- Calendar/schedule integration
- Export to PDF/Excel reports

---

## 5. Non-Functional Requirements

### 5.1 Performance

| Requirement | Target | Rationale |
|-------------|--------|-----------|
| Cold start time | <2s | First impression matters |
| Map initial load | <3s | 30+ markers must render fast |
| Radius filter update | <500ms | Feels instant to user |
| RPC query execution | <500ms | PostGIS spatial index optimized |
| AI analysis latency | <10s | Acceptable for thoughtful recommendation |
| UI responsiveness | <100ms | All button clicks |

### 5.2 Scalability

- Support 200 users + 1,000 assistants per agency
- Map clustering handles 10,000+ markers without lag
- Database queries remain <500ms at 100x data scale
- Concurrent coordinator access: 10+ users without degradation

### 5.3 Security

- **SEC1:** All API keys stored in User Secrets or environment variables (never hardcoded)
- **SEC2:** Supabase RLS policies prevent cross-agency data access
- **SEC3:** n8n webhooks protected by secret token or IP whitelist
- **SEC4:** All external communication over HTTPS
- **SEC5:** No sensitive data (passwords, tokens) logged or displayed in error messages

### 5.4 Accessibility

- **A11Y1:** WCAG AA compliance (4.5:1 contrast for body text, 3:1 for large text)
- **A11Y2:** Keyboard navigation supported (tab through all interactive elements)
- **A11Y3:** Screen reader compatible (semantic HTML, ARIA labels)
- **A11Y4:** Marker differentiation by shape (circle vs square) not just color
- **A11Y5:** Mobile touch targets minimum 48x48px

### 5.5 Reliability

- **REL1:** No crashes during 8-hour continuous operation
- **REL2:** Graceful degradation if Naver Maps API unavailable (show cached data)
- **REL3:** Offline mode: display "Network unavailable" toast, retry on reconnect
- **REL4:** Data loss prevention: all mutations confirmed before UI update

### 5.6 Maintainability

- **MAINT1:** Modular component architecture (see §6 for component breakdown)
- **MAINT2:** Centralized service layer (AuthService, DataService, AIService)
- **MAINT3:** Consistent naming conventions (PascalCase for classes, camelCase for variables)
- **MAINT4:** No TODO/FIXME in production code
- **MAINT5:** Early-return pattern for flow control (minimize nesting)

---

## 6. System Architecture

### 6.1 High-Level Architecture

```
┌─────────────────────────────────────────────────────────────┐
│  Client Layer (Blazor Hybrid - Windows/Android/Web)       │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐     │
│  │ Map Panel    │  │ List Panel   │  │ Modal Dialog │     │
│  │ (Naver Maps) │  │ (MudBlazor)  │  │ (Comparison) │     │
│  └──────────────┘  └──────────────┘  └──────────────┘     │
│         ↓                  ↓                  ↓             │
│  ┌────────────────────────────────────────────────────┐    │
│  │ Service Layer (C# Services)                        │    │
│  │ - AuthService   - DataService   - AIService        │    │
│  └────────────────────────────────────────────────────┘    │
└────────────────────────────┬────────────────────────────────┘
                             ↓ (HTTPS/WebSocket)
┌─────────────────────────────────────────────────────────────┐
│  BaaS Layer (Supabase)                                     │
│  ┌───────────┐  ┌───────────┐  ┌───────────┐  ┌─────────┐ │
│  │ Auth      │  │ Database  │  │ Realtime  │  │ RPC     │ │
│  │ (Session) │  │ (PostGIS) │  │ (WS)      │  │ (PL/PG) │ │
│  └───────────┘  └───────────┘  └───────────┘  └─────────┘ │
└────────────────────────────┬────────────────────────────────┘
                             ↓ (Webhook)
┌─────────────────────────────────────────────────────────────┐
│  Orchestration Layer (n8n)                                 │
│  ┌────────────┐  ┌────────────┐  ┌────────────┐           │
│  │ Excel      │  │ Geocoding  │  │ AI Match   │           │
│  │ Upload WF  │  │ Pipeline   │  │ Analysis   │           │
│  └────────────┘  └────────────┘  └────────────┘           │
└────────────────────────────┬────────────────────────────────┘
                             ↓ (API calls)
┌─────────────────────────────────────────────────────────────┐
│  External Services                                          │
│  - Naver Maps API v3        - Claude 3.5 (LLM)            │
└─────────────────────────────────────────────────────────────┘
```

### 6.2 Data Model

**Core Tables:**

```sql
-- profiles: user and assistant basic info
CREATE TABLE profiles (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  user_id UUID UNIQUE REFERENCES auth.users(id) ON DELETE CASCADE,
  role TEXT CHECK (role IN ('coordinator', 'assistant', 'user')),
  name TEXT NOT NULL,
  phone TEXT,
  agency_id UUID,
  created_at TIMESTAMP DEFAULT now(),
  updated_at TIMESTAMP DEFAULT now()
);

-- locations: spatial data with PostGIS
CREATE TABLE locations (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  profile_id UUID REFERENCES profiles(id) ON DELETE CASCADE,
  address TEXT NOT NULL,
  coordinate GEOGRAPHY(Point, 4326),
  updated_at TIMESTAMP DEFAULT now()
);
CREATE INDEX idx_locations_coordinate ON locations USING GIST (coordinate);

-- preferences: flexible JSONB for filtering criteria
CREATE TABLE preferences (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  profile_id UUID REFERENCES profiles(id) ON DELETE CASCADE,
  availability_slots JSONB, -- e.g., ["weekday_am", "weekend_pm"]
  gender_pref TEXT CHECK (gender_pref IN ('any', 'male', 'female')),
  has_vehicle BOOLEAN DEFAULT false,
  skip_list JSONB DEFAULT '[]'::jsonb,
  created_at TIMESTAMP DEFAULT now()
);

-- matches: matching records with AI metadata
CREATE TABLE matches (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  user_id UUID REFERENCES profiles(id),
  assistant_id UUID REFERENCES profiles(id),
  status TEXT CHECK (status IN ('draft', 'proposed', 'confirmed', 'cancelled')),
  ai_score INTEGER CHECK (ai_score BETWEEN 0 AND 100),
  ai_reason TEXT,
  created_at TIMESTAMP DEFAULT now(),
  updated_at TIMESTAMP DEFAULT now()
);
```

**RPC Functions:**

```sql
-- get_nearby_assistants: radius search with distance sorting
CREATE OR REPLACE FUNCTION get_nearby_assistants(
  p_user_lat DOUBLE PRECISION,
  p_user_lng DOUBLE PRECISION,
  p_radius_km DOUBLE PRECISION,
  p_agency_id UUID
)
RETURNS TABLE (
  assistant_id UUID,
  name TEXT,
  distance_km DOUBLE PRECISION
) AS $$
BEGIN
  RETURN QUERY
  SELECT 
    p.id,
    p.name,
    ST_Distance(
      l.coordinate,
      ST_SetSRID(ST_MakePoint(p_user_lng, p_user_lat), 4326)::geography
    ) / 1000.0 AS distance_km
  FROM profiles p
  JOIN locations l ON l.profile_id = p.id
  WHERE 
    p.role = 'assistant'
    AND p.agency_id = p_agency_id
    AND ST_DWithin(
      l.coordinate,
      ST_SetSRID(ST_MakePoint(p_user_lng, p_user_lat), 4326)::geography,
      p_radius_km * 1000
    )
  ORDER BY distance_km ASC;
END;
$$ LANGUAGE plpgsql;
```

### 6.3 Component Architecture

**Windows/Android Blazor App Structure:**

```
Components/
├── Layout/
│   ├── MainLayout.razor           # 70/30 split layout
│   └── AppBar.razor                # Top bar with stats
├── Map/
│   ├── NaverMap.razor              # Map wrapper component
│   └── MapControls.razor           # Radius presets, layer toggles
├── Panels/
│   ├── UserListPanel.razor         # Left: user list (desktop)
│   └── MatchingPanel.razor         # Right: HUD with filters + assistant list
├── Cards/
│   ├── UserCard.razor              # Single user item
│   ├── AssistantCard.razor         # Single assistant item with "Propose Match" button
│   └── AIRecommendationCard.razor  # TOP 3 AI results card
├── Modals/
│   └── ComparisonModal.razor       # Side-by-side user vs assistant comparison
└── Shared/
    ├── LoadingSpinner.razor        # Reusable spinner
    └── ErrorToast.razor            # User-friendly error display

Services/
├── AuthService.cs                  # Supabase Auth wrapper
├── SupabaseDataService.cs          # CRUD + RPC calls
├── AIMatchingService.cs            # n8n webhook caller
└── MapInteropService.cs            # JS interop for Naver Maps

wwwroot/
└── js/
    └── naverMap.js                 # JavaScript interop for map operations
```

---

## 7. Technology Stack

### 7.1 Frontend & Application Framework

| Component | Technology | Version | Purpose |
|-----------|------------|---------|---------|
| **Framework** | .NET MAUI | 9.0 LTS | Cross-platform native app development |
| **Language** | C# | 13 | Modern C# features (record types, pattern matching) |
| **UI Pattern** | MVVM | - | Separation of concerns, testability |
| **MVVM Toolkit** | CommunityToolkit.Mvvm | 8.2+ | ObservableProperty, RelayCommand source generators |
| **Dependency Injection** | Microsoft.Extensions.DI | Built-in | Service registration and lifecycle management |

**Rationale:**
- ✅ **Cross-platform**: Single codebase for Windows (MSIX) + Android (APK)
- ✅ **Performance**: Native compilation, app start <2s
- ✅ **Offline support**: Local caching, no server dependency for UI
- ✅ **Modern tooling**: Hot reload, XAML designer, Visual Studio integration

---

### 7.2 Backend & Data Layer

| Component | Technology | Version | Purpose |
|-----------|------------|---------|---------|
| **Database** | Supabase (PostgreSQL) | 16+ | User data, assistants, matches, notifications |
| **Geospatial** | PostGIS | 3.4+ | Distance calculation, radius queries |
| **Authentication** | Supabase Auth | - | Email/password, session management |
| **Realtime** | Supabase Realtime | - | Live notifications, match updates |
| **Client Library** | supabase-csharp | 0.16+ | C# SDK for Supabase API calls |
| **Row-Level Security** | RLS Policies | - | Agency isolation, role-based access |

**Data Flow:**
1. MAUI app → Supabase REST API (CRUD operations)
2. Supabase Realtime → MAUI app (live updates via WebSocket)
3. PostGIS ST_DWithin → Radius queries (<500ms)

---

### 7.3 Map Integration

| Component | Technology | Version | Purpose |
|-----------|------------|---------|---------|
| **Map Provider** | Naver Maps API | v3 | South Korea optimized, accurate address data |
| **Rendering** | WebView + HTML/JS | - | Embed Naver Maps in MAUI app |
| **Interop** | EvaluateJavaScriptAsync | - | C# ↔ JavaScript communication |
| **Marker Styles** | Custom HTML markers | - | Blue circle (user), orange square (assistant) |
| **Callback** | Custom URL Scheme | maui:// | JavaScript → C# marker click events |

**JavaScript Functions:**
- `window.initMap(lat, lng, zoom)` - Initialize map
- `window.addMarker(id, lat, lng, type, name)` - Add marker
- `window.drawCircle(lat, lng, radiusKm)` - Draw radius circle
- `window.clearCircle()` - Remove circle
- `window.setMarkerVisible(id, visible)` - Toggle marker visibility

**Reference:** Verified JS functions from `MapDemo/wwwroot/js/naverMap.js` (Blazor prototype)

---

### 7.4 AI & Automation

| Component | Technology | Version | Purpose |
|-----------|------------|---------|---------|
| **AI Model** | Claude 3.5 Sonnet | - | Compatibility analysis, recommendations |
| **Workflow Engine** | n8n | 1.0+ | Webhook orchestration, LLM integration |
| **HTTP Client** | HttpClient | Built-in | POST requests to n8n webhooks |
| **Serialization** | System.Text.Json | Built-in | JSON request/response |

**AI Workflow:**
1. Coordinator selects 1-3 assistants → Click "AI Analysis" button
2. MAUI app → HTTP POST to n8n webhook (user context + assistant contexts)
3. n8n workflow:
   - Fetch additional data from Supabase
   - Call Claude 3.5 API with comparison prompt
   - Store results (score, reason) in Supabase
   - Broadcast via Realtime channel
4. MAUI app receives live update → Display TOP 3 recommendations

**Expected Latency:** <10s (typical: 5-7s)

---

### 7.5 Development & Deployment

| Component | Technology | Purpose |
|-----------|------------|---------|
| **IDE** | Visual Studio 2022 | MAUI project templates, debugger |
| **Source Control** | Git + GitHub | Version control, CI/CD |
| **Build** | dotnet CLI | `dotnet build`, `dotnet publish` |
| **Windows Packaging** | MSIX | Windows Store, sideloading |
| **Android Packaging** | APK | Google Play, direct install |
| **Secrets Management** | User Secrets | API keys (Naver, Supabase) |
| **Configuration** | appsettings.json | Environment-specific settings |

**Build Commands:**
```bash
# Windows MSIX
dotnet publish -f net9.0-windows10.0.19041.0 -c Release -p:RuntimeIdentifierOverride=win10-x64

# Android APK
dotnet publish -f net9.0-android -c Release
```

---

### 7.6 Testing & Quality

| Component | Technology | Purpose |
|-----------|------------|---------|
| **Unit Testing** | xUnit | Service layer, ViewModel logic |
| **Mocking** | Moq | Mock ISupabaseService, ILocationService |
| **UI Testing** | (Manual) | CollectionView, WebView interaction |
| **Performance** | Stopwatch | Measure app start, map load, radius filter |

**Performance Budget:**
- App start: <2s (cold launch)
- Map load: <3s (initial marker rendering)
- Radius filter: <500ms (filter update)

---

### 7.7 Dependencies Summary

**NuGet Packages:**
```xml
<ItemGroup>
  <!-- MVVM -->
  <PackageReference Include="CommunityToolkit.Mvvm" Version="8.2.2" />
  
  <!-- Supabase -->
  <PackageReference Include="supabase-csharp" Version="0.16.0" />
  
  <!-- (Optional) Testing -->
  <PackageReference Include="xunit" Version="2.6.0" />
  <PackageReference Include="Moq" Version="4.20.0" />
</ItemGroup>
```

---

## 8. User Interface Specifications

### 7.1 Design System (Dark Theme HUD)

**Color Palette:**

| Element | Color | Hex | Usage |
|---------|-------|-----|-------|
| Background | Deep Navy | #111827 | Page background |
| Surface | Dark Gray | #1F2937 | Panels, cards, modals |
| Text Primary | Off-White | #F9FAFB | Main text |
| Text Secondary | Light Gray | #9CA3AF | Labels |
| Text Tertiary | Mid Gray | #6B7280 | Hints, disabled |
| Primary CTA | Teal | #2DD4BF | AI button, main actions |
| User Marker | Blue | #3B82F6 | User marker background |
| Assistant Marker | Orange | #F97316 | Assistant marker background |
| Success | Green | #10B981 | Confirmation feedback |
| Warning | Amber | #FBBF24 | Attention needed |
| Error | Red | #EF4444 | Critical issues |

**Typography:**

| Level | Size | Weight | Use Case |
|-------|------|--------|----------|
| Display | 28-32pt | Bold (700) | "LMP-Link" logo, modal titles |
| Heading 1 | 24pt | Bold (700) | Page sections |
| Heading 2 | 20pt | SemiBold (600) | Card titles |
| Body | 16-18pt | Regular (400) | Main content |
| Small | 14pt | Regular (400) | Metadata |
| Caption | 12pt | Regular (400) | Hints |
| Button | 16-18pt | SemiBold (600) | Interactive elements |

**Spacing & Layout:**

- Grid system: 12-column responsive grid
- Desktop split: 70% map / 30% HUD panel
- Mobile: Full-screen map + bottom sheet
- Minimum touch target: 48x48px (mobile)

### 7.2 Key Screens & Flows

#### Screen 1: Main Matching Console (Windows Desktop)

```
┌──────────────────────────────────────────────────────────────┐
│ [Logo] LMP-Link                   Dec 8, 14:35    [Logout]  │  ← AppBar
├─────────────────────────────────────────────┬────────────────┤
│                                              │ [Users] [Match]│  ← Tabs
│                                              ├────────────────┤
│         Naver Map (70%)                      │ Filters:       │
│  ┌──────────────────────────────┐            │ Radius: [3km▼] │
│  │ [User 🔵] [User 🔵]           │            │ Time: [All▼]  │
│  │                               │            │ Gender: [Any▼] │
│  │     [Assistant 🟧]            │            │ Vehicle: [✓]  │
│  │                               │            │ [Reset Filters]│
│  │  [Assistant 🟧] (dimmed 50%)  │            ├────────────────┤
│  │                               │            │ [AI Analysis]  │  ← Big teal button
│  │      3km radius circle        │            ├────────────────┤
│  │                               │            │ 🔍 7 nearby:   │
│  │                               │            │ ┌────────────┐ │
│  │                               │            │ │Kim (2.1km) │ │
│  └──────────────────────────────┘            │ │⏰ Weekday AM│ │
│                                              │ │[Propose]   │ │
│                                              │ └────────────┘ │
│                                              │ ┌────────────┐ │
│                                              │ │Lee (2.8km) │ │
│                                              │ │⏰ Weekend   │ │
│                                              │ │[Propose]   │ │
│                                              │ └────────────┘ │
└─────────────────────────────────────────────┴────────────────┘
```

**Interaction Flow:**
1. Coordinator clicks user marker (🔵) on map
2. 3km radius circle appears immediately
3. Right panel filters to show 7 nearby assistants
4. Out-of-radius assistants on map are dimmed
5. Coordinator applies additional filters (time, gender)
6. List updates in <100ms
7. Coordinator clicks "Propose Match" → opens comparison modal

#### Screen 2: Comparison Modal (Desktop)

```
┌──────────────────────────────────────────────────────────────┐
│  ✕  Matching Review                          [Cancel] [Confirm] │
├──────────────────────────────────┬───────────────────────────┤
│ User: Park Ji-woo                │ Assistant: Kim Min-soo    │
│ 📍 Dobong-gu, Seoul               │ 📍 2.1km away             │
│ 📞 010-1234-5678                  │ 📞 010-8765-4321          │
│                                   │                           │
│ Requirements:                     │ Experience:               │
│ - Weekday AM                      │ - 3 years in disability   │
│ - Female preferred                │   support                 │
│ - Vehicle needed                  │ - Has own car             │
│                                   │ - Positive reviews (4.8)  │
├──────────────────────────────────┴───────────────────────────┤
│ 🤖 AI Compatibility Score: 92/100 ████████████████░░         │
│                                                                │
│ Recommendation: Excellent match! Kim has extensive experience │
│ with similar cases and the distance is ideal. His morning    │
│ availability aligns perfectly with the user's needs.         │
└──────────────────────────────────────────────────────────────┘
```

**Interaction Flow:**
1. Modal opens with user (left) and assistant (right) details
2. Center shows AI score bar (green gradient)
3. Bottom displays natural language recommendation
4. "Confirm" button → saves match to DB → triggers Realtime event
5. "Cancel" button → closes modal without saving

#### Screen 3: Mobile App (Android - Assistant View)

```
┌──────────────────────────┐
│ Today: Tuesday, Dec 8    │  ← Top section
│ 📅 2 assignments          │
├──────────────────────────┤
│ ┌──────────────────────┐ │
│ │ 10:00 - 12:00        │ │
│ │ Park Ji-woo          │ │
│ │ 📍 Dobong-gu         │ │
│ │ [View Details]       │ │
│ └──────────────────────┘ │
│ ┌──────────────────────┐ │
│ │ 14:00 - 16:00        │ │
│ │ Lee Su-jin           │ │
│ │ 📍 Gangbuk-gu        │ │
│ │ [View Details]       │ │
│ └──────────────────────┘ │
├──────────────────────────┤
│ [Schedule] [Alerts] [Me] │  ← Bottom nav
└──────────────────────────┘
```

---

## 8. Technical Implementation Roadmap

### Phase 1: UI Polish & Design System (Weeks 1-2)

**Deliverables:**
- [ ] MudBlazor dark theme fully configured
- [ ] Component architecture refactored (see §6.3)
- [ ] 70/30 split layout working on desktop
- [ ] Marker styles finalized (blue circle, orange square)
- [ ] Accessibility score ≥90% (WAVE/Axe)

**Acceptance Criteria:**
- All components render without errors
- Layout maintains 70/30 ratio on window resize
- Marker clicks trigger <100ms radius draw
- WCAG AA compliance verified

---

### Phase 2: Data Integration & Auth (Weeks 2-3)

**Deliverables:**
- [ ] Supabase Auth setup (email/password login)
- [ ] RLS policies enforced (coordinators see own agency only)
- [ ] `SupabaseDataService` replaces `MockDataService`
- [ ] RPC `get_nearby_assistants` tested with 100+ records
- [ ] Sample data migrated (100 users, 200 assistants)

**Acceptance Criteria:**
- Coordinator can log in and see only their agency data
- Radius search returns correct assistants in <500ms
- Session persists across app restarts
- No RLS violations during testing

---

### Phase 3: AI Pipeline & n8n (Weeks 3-4)

**Deliverables:**
- [ ] n8n workflow: Excel upload → Geocoding → DB save
- [ ] n8n workflow: AI analysis → LLM call → Realtime broadcast
- [ ] `AIMatchingService` calls n8n webhook
- [ ] Blazor subscribes to Supabase Realtime channel
- [ ] TOP 3 recommendations displayed in UI

**Acceptance Criteria:**
- Full AI flow works end-to-end (5-10s latency)
- Coordinator sees TOP 3 cards with scores + reasons
- No data loss during async processing
- Error handling for n8n/LLM failures

---

### Phase 4: Testing & Deployment (Weeks 4-6)

**Deliverables:**
- [ ] Unit tests for service layer (80%+ coverage)
- [ ] Integration tests for RPC + Realtime
- [ ] Manual regression checklist (see §9.2)
- [ ] Windows MSIX package signed and tested
- [ ] Android APK signed and tested
- [ ] User installation guides (PDF)

**Acceptance Criteria:**
- All automated tests pass
- Manual regression checklist 100% pass
- MSIX installs on clean Windows 10/11
- APK installs on Android 9+
- Deployment artifacts uploaded to internal server

---

## 9. Testing Strategy

### 9.1 Test Pyramid

```
       /\
      /  \     E2E Tests (5%)
     /────\    - Manual regression checklist
    /      \   - Core user flows
   /────────\  Integration Tests (25%)
  /          \ - Supabase RPC calls
 /────────────\- Realtime subscriptions
/______________\ Unit Tests (70%)
                - Distance calculations
                - Filter logic
                - Data transformations
```

### 9.2 Manual Regression Checklist (Run Before Each Release)

**Critical Path:**

- [ ] **Login:** Email/password → successful login → session persists after app restart
- [ ] **Map Load:** All markers render within 3 seconds
- [ ] **Radius Search:** Click user marker → 3km circle appears <100ms → list filters correctly
- [ ] **Filters:** Time + Gender + Vehicle filters work individually and combined
- [ ] **AI Analysis:** Select 2-3 candidates → click AI button → spinner shows → results appear <10s
- [ ] **Propose Match:** Click "Propose Match" → modal opens with correct data
- [ ] **Confirm Match:** Click "Confirm" in modal → saves to DB → Realtime event fires
- [ ] **Mobile Notification:** Android app receives match notification within 5s
- [ ] **Logout:** Click logout → session cleared → redirects to login page

**Performance:**

- [ ] Cold start: App launches to map ready <2s
- [ ] Map with 1,000 markers: No lag, clustering works
- [ ] Radius change: Filter updates <500ms
- [ ] AI analysis: Completes <10s (95th percentile)

**Edge Cases:**

- [ ] Network offline: "No connection" toast appears
- [ ] Naver Maps API error: Graceful fallback or clear error message
- [ ] RLS violation attempt: Access denied without crash
- [ ] Empty search results: "No assistants found" message

### 9.3 Test Data Requirements

**Dev Environment:**
- 10 coordinators (different agencies)
- 100 users (distributed across Seoul/Gyeonggi)
- 200 assistants (mixed availability, vehicle ownership)
- 50 existing matches (various statuses)
- Coordinates covering 0-5km radius from test center (Dobong-gu office)

---

## 10. Deployment Plan

### 10.1 Build Artifacts

**Windows (MSIX):**
```bash
dotnet publish -c Release -f net9.0-windows10.0.19041.0 \
  -p:WindowsAppSDKSelfContained=true \
  -p:PublishProfile=MSIX
```

**Android (APK):**
```bash
dotnet publish -c Release -f net9.0-android \
  -p:AndroidKeyStore=true \
  -p:AndroidSigningKeyStore=lmp-link.keystore \
  -p:AndroidSigningKeyAlias=lmp-key
```

### 10.2 Deployment Checklist

**Pre-Deployment:**
- [ ] All Phase 4 deliverables complete
- [ ] Manual regression checklist 100% pass
- [ ] Version number updated in project files
- [ ] Release notes drafted (features, bug fixes, known issues)
- [ ] Git tag created (e.g., `v1.0.0`)

**Deployment:**
- [ ] Build MSIX and APK in Release configuration
- [ ] Sign packages with production certificates
- [ ] Upload to internal file server (or deployment portal)
- [ ] Send installation links + PDF guides to coordinators/assistants
- [ ] Monitor first 24 hours for critical bugs

**Post-Deployment:**
- [ ] Verify login works in production
- [ ] Spot-check map loading and radius search
- [ ] Confirm AI analysis connects to production n8n
- [ ] Collect initial user feedback (email/Slack)

### 10.3 Rollback Plan

If critical bugs discovered:
1. Notify users immediately (email blast)
2. Re-upload previous version artifacts
3. Provide downgrade instructions
4. If database migration issue: restore Supabase backup to last known good state
5. Investigate root cause → fix → retest → redeploy as hotfix

---

## 11. Success Criteria & KPIs

### 11.1 Launch Criteria (Gate for v1.0 Release)

**Must-Have:**
- ✅ All Phase 1-4 deliverables complete
- ✅ Zero critical bugs in final build
- ✅ Performance targets met (see §5.1)
- ✅ Security audit passed (see §5.3)
- ✅ Accessibility score ≥90%
- ✅ MSIX/APK install successfully on clean devices

**Nice-to-Have:**
- 80%+ unit test coverage
- User documentation reviewed by coordinator
- Support channel (email/Slack) ready

### 11.2 Post-Launch KPIs (Track for 30 Days)

| KPI | Target | Measurement Method |
|-----|--------|-------------------|
| User adoption rate | 80%+ coordinators onboarded | Active login count |
| Average matching time | <3 min per case | Timestamp analysis (match created - user selected) |
| AI recommendation usage | 50%+ of matches use AI | Count matches with `ai_score` populated |
| Crash-free sessions | >95% | Error log analysis |
| User satisfaction | 4+/5 score | Post-deployment survey |
| Support tickets | <5 critical/week | Issue tracker count |

---

## 12. Risks & Mitigations

| Risk | Impact | Probability | Mitigation |
|------|--------|-------------|------------|
| Naver Maps API outage | High | Low | Cache last known marker positions; display static map |
| n8n server downtime | High | Medium | Implement retry logic; fallback to simple rule-based matching |
| Supabase RLS misconfiguration | Critical | Medium | Thorough testing of RLS policies in Dev; code review |
| LLM response quality poor | Medium | Medium | Curate prompt templates; A/B test different prompts |
| User resistance to new tool | High | Medium | Provide on-site training; create video tutorials |
| Performance degradation at scale | High | Low | Load test with 10x data; optimize RPC queries early |

---

## 13. Open Questions & Decisions

### 13.1 Resolved Decisions

- ✅ **Tech stack:** Blazor Hybrid + Supabase BaaS + n8n + Naver Maps
- ✅ **Deployment:** MSIX (Windows) + APK (Android), sideload (no store submission for v1.0)
- ✅ **AI provider:** Claude 3.5 via n8n workflow
- ✅ **Map provider:** Naver Maps v3 (JavaScript SDK)

### 13.2 Open Questions (To Be Resolved Before Phase 2)

- ❓ **Agency onboarding:** How to provision new agencies? (Manual SQL insert vs admin UI?)
- ❓ **Data migration:** Migrate existing Excel data via n8n or one-time SQL script?
- ❓ **Notification service:** Push notifications for mobile (FCM? Supabase Realtime only?)
- ❓ **Analytics:** Track user behavior (Google Analytics? Mixpanel? Self-hosted Plausible?)

---

## 14. Appendices

### 14.1 Glossary

- **RLS:** Row-Level Security (Supabase feature for data isolation)
- **RPC:** Remote Procedure Call (Supabase PostgreSQL functions callable via API)
- **PostGIS:** Spatial database extension for PostgreSQL
- **Realtime:** Supabase WebSocket service for live data updates
- **n8n:** Self-hosted workflow automation platform
- **LLM:** Large Language Model (Claude 3.5 in this project)

### 14.2 References

- [Naver Maps API v3 Documentation](https://navermaps.github.io/maps.js.ncp/docs/)
- [Supabase Documentation](https://supabase.com/docs)
- [MudBlazor Component Library](https://mudblazor.com)
- [n8n Workflow Examples](https://n8n.io/workflows)
- [WCAG 2.1 Guidelines](https://www.w3.org/WAI/WCAG21/quickref/)

### 14.3 Change Log

| Version | Date | Author | Changes |
|---------|------|--------|---------|
| 0.1 | 2025-11-20 | Dev Team | Initial draft (archived as PRD_v0) |
| 1.0 | 2025-12-08 | Dev Team | Complete rewrite based on roadmap + design system |

---

**Document Status:** APPROVED FOR DEVELOPMENT  
**Next Review Date:** 2025-12-22 (End of Phase 2)
