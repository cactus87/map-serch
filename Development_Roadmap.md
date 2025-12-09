# Development Roadmap (LMP-Link MVP → v1.0)

**Objective:** Transform the current MVP (functional, but rough) into a production-ready v1.0 that coordinators can use daily with confidence.

**Duration:** 4-6 weeks (1 developer, following this roadmap sequentially)

---

## 🎯 High-Level Strategy

1. **Phase 1 (Week 1-2):** UI Refinement + Theme System
2. **Phase 2 (Week 2-3):** Real Data Integration + Auth
3. **Phase 3 (Week 3-4):** AI Pipeline + n8n Webhook
4. **Phase 4 (Week 4-6):** Testing + Deployment Setup

---

## ⏱️ Phase 1: UI Refinement & Design System (2 weeks)

**Goal:** Make the app look and feel professional, not a prototype.

### 1-1. MudBlazor Theme Configuration
**Files to modify/create:**
- `wwwroot/css/custom-theme.css`
- `Components/Layout/MainLayout.razor`
- `App.razor` (MudThemeProvider setup)

**Tasks:**
- [ ] Define MudTheme with Dark palette:
  - Primary: Teal (#2DD4BF)
  - Surface: Dark Grey (#1F2937)
  - Background: Deep Navy (#111827)
- [ ] Set typography scale (headings, body, button styles)
- [ ] Test theme in both light/dark modes (prepare dark mode only for v1.0)
- [ ] Create reusable CSS utility classes (spacing, shadows, animations)

**Acceptance Criteria:** App opens with complete dark theme, all text readable, buttons visually distinct.

---

### 1-2. Component Refactoring & Extraction
**Current state:** Most logic is in single `.razor` files.  
**Target state:** Modular, reusable components.

**Components to create:**
1. **`Components/Map/NaverMap.razor`**
   - Handles map initialization, marker rendering, circle overlay
   - Props: `OnMarkerClick`, `OnMapLoaded`
   - Methods: `DrawRadius()`, `ClearRadius()`, `PanTo()`

2. **`Components/Markers/UserMarker.razor`**
   - Displays blue circle user marker
   - Props: User data, IsSelected, IsInRadius

3. **`Components/Markers/AssistantMarker.razor`**
   - Displays orange square assistant marker
   - Props: Assistant data, IsFiltered, Distance

4. **`Components/Panels/MatchingPanel.razor`** (Right HUD)
   - Contains tabs, filters, candidate list
   - Props: `SelectedUser`, `FilteredAssistants`, `OnAssistantSelect`

5. **`Components/Modals/ComparisonModal.razor`**
   - Side-by-side user vs assistant comparison
   - Props: `User`, `Assistant`, `MatchScore`, `OnConfirm`, `OnCancel`

6. **`Components/Cards/UserCard.razor`**
   - Single user list item in left panel
   - Props: User, OnClick, IsSelected

7. **`Components/Cards/AssistantCard.razor`**
   - Single assistant card in matching panel
   - Props: Assistant, Distance, OnQuickMatch

**Acceptance Criteria:** All components render without errors, data flows correctly through props.

---

### 1-3. Layout Restructuring
**File:** `Components/Layout/MainLayout.razor`

**Current structure:** Flat design  
**Target structure:**

```
<MudLayout>
  <MudAppBar>
    <!-- AppBar: Logo, Date/Time, Quick Stats, Coordinator Name -->
  </MudAppBar>
  <MudContainer Class="main-container">
    <MudGrid Spacing="0">
      <MudItem xs="8.4" md="8.4">
        <!-- Left: Map (70%) -->
        <NaverMap @ref="mapComponent" OnMarkerClick="HandleMarkerClick" />
      </MudItem>
      <MudItem xs="3.6" md="3.6">
        <!-- Right: HUD Panel (30%) -->
        <MatchingPanel SelectedUser="selectedUser" ... />
      </MudItem>
    </MudGrid>
  </MudContainer>
</MudLayout>
```

**Acceptance Criteria:** Map and panel maintain 70/30 ratio on resize, AppBar stays fixed, no overflow.

---

### 1-4. Interaction Polish
**Features to implement:**

- [ ] **Marker Click Event:**
  - User clicks User Marker → 3km circle draws instantly (no lag)
  - Assistant markers outside radius dim (opacity 50%)
  - Right panel scrolls to top and lists filtered assistants

- [ ] **List to Map Synchronization:**
  - Coordinator clicks assistant card in list → Assistant marker highlights in map

- [ ] **Radius Control Buttons:**
  - `[1km]` `[3km]` `[5km]` buttons toggle immediately
  - Map re-queries and re-filters list in <100ms

- [ ] **Visual Feedback:**
  - Loading spinner while searching
  - "No results" message when radius has no matches
  - Active filter badges showing which filters are on

**Acceptance Criteria:** All interactions respond within 100ms. Visual feedback clear for every user action.

---

### 1-5. Accessibility Audit
- [ ] Run WAVE or Axe accessibility scanner
- [ ] Fix contrast issues (all text WCAG AA or above)
- [ ] Test keyboard navigation (Tab through all interactive elements)
- [ ] Verify screen reader compatibility for key panels

**Acceptance Criteria:** Accessibility score ≥90%, no Critical violations.

---

## 🗄️ Phase 2: Real Data Integration & Authentication (2 weeks)

**Goal:** Replace mock data with real Supabase data; add login security.

### 2-1. Supabase Auth Setup
**File:** `Services/AuthService.cs`

**Tasks:**
- [ ] Implement Supabase C# SDK installation
- [ ] Create `AuthService` with methods:
  - `SignUpAsync(email, password, role)` → Returns Session
  - `SignInAsync(email, password)` → Returns Session + User ID
  - `SignOutAsync()` → Clears session
  - `GetCurrentUserAsync()` → Returns logged-in user
- [ ] Store session in secure storage (`SecureStorage` or browser LocalStorage for Web)
- [ ] Create login page (`Pages/Login.razor`)
  - Email + Password inputs (large, accessible)
  - "Sign In" button
  - "Forgot Password" link (stub for v1.0)
  - Error message display

**Acceptance Criteria:** Coordinator can log in with email/password, session persists on page refresh.

---

### 2-2. Database Schema Validation & Migration
**File:** `db/schema.sql`

**Tasks:**
- [ ] Verify Supabase tables exist:
  ```sql
  CREATE TABLE profiles (
    id UUID PRIMARY KEY,
    user_id UUID UNIQUE REFERENCES auth.users(id),
    role TEXT CHECK (role IN ('coordinator', 'assistant')),
    name TEXT,
    phone TEXT,
    agency_id UUID,
    created_at TIMESTAMP DEFAULT now()
  );

  CREATE TABLE locations (
    id UUID PRIMARY KEY,
    profile_id UUID REFERENCES profiles(id) ON DELETE CASCADE,
    address TEXT,
    coordinate GEOGRAPHY(Point, 4326),
    updated_at TIMESTAMP DEFAULT now()
  );

  CREATE TABLE preferences (
    id UUID PRIMARY KEY,
    profile_id UUID REFERENCES profiles(id),
    availability_slots JSONB,
    gender_pref TEXT,
    has_vehicle BOOLEAN,
    skip_list JSONB,
    created_at TIMESTAMP DEFAULT now()
  );

  CREATE TABLE matches (
    id UUID PRIMARY KEY,
    user_id UUID REFERENCES profiles(id),
    assistant_id UUID REFERENCES profiles(id),
    status TEXT CHECK (status IN ('draft', 'proposed', 'confirmed', 'cancelled')),
    ai_score INTEGER,
    ai_reason TEXT,
    created_at TIMESTAMP DEFAULT now(),
    updated_at TIMESTAMP DEFAULT now()
  );

  CREATE INDEX ON locations USING GIST (coordinate);
  ```
- [ ] Create RPC function `get_nearby_assistants`:
  ```sql
  CREATE OR REPLACE FUNCTION get_nearby_assistants(
    user_lat FLOAT,
    user_lng FLOAT,
    radius_meters INT,
    agency_id UUID
  )
  RETURNS TABLE (...) AS $$
    SELECT p.*, l.*, ST_Distance(l.coordinate, ...) as distance
    FROM profiles p
    JOIN locations l ON p.id = l.profile_id
    WHERE p.role = 'assistant'
      AND p.agency_id = agency_id
      AND ST_DWithin(l.coordinate, ST_SetSRID(ST_MakePoint(user_lng, user_lat), 4326), radius_meters)
    ORDER BY distance ASC;
  $$ LANGUAGE SQL;
  ```
- [ ] Test RPC call from C#

**Acceptance Criteria:** All tables exist in Supabase with correct schemas, RPC queries execute in <500ms.

---

### 2-3. Replace MockDataService with SupabaseService
**File:** `Services/SupabaseDataService.cs`

**Tasks:**
- [ ] Create `SupabaseDataService` class:
  - `GetNearbyAssistantsAsync(userLat, userLng, radiusKm)` → Calls RPC
  - `GetAllUsersAsync()` → SELECT from profiles where role='user'
  - `GetUserDetailsAsync(userId)` → Full profile + location + preferences
  
- [ ] Update `MainLayout.razor` to use new service instead of mock data
- [ ] Implement error handling:
  - Network timeout → Show "Connection error" snackbar
  - Auth token expired → Redirect to login
  - Invalid query → Log and show generic error

**Acceptance Criteria:** App loads real data from Supabase, displays 10+ users and 50+ assistants without lag.

---

### 2-4. RLS (Row Level Security) Policies
**Supabase SQL:**

```sql
-- Coordinators see only their agency's data
CREATE POLICY "Coordinators view own agency"
  ON profiles FOR SELECT
  USING (auth.uid() IN (SELECT user_id FROM profiles WHERE agency_id = profiles.agency_id));

-- Assistants see only their own profile
CREATE POLICY "Assistants view themselves"
  ON profiles FOR SELECT
  USING (auth.uid() = user_id AND role = 'assistant');
```

**Acceptance Criteria:** RLS policies enforced, unauthorized users cannot access data.

---

### 2-5. Sample Data Migration
**Tasks:**
- [ ] Export existing mock data as CSV
- [ ] Insert 100-200 realistic dummy users/assistants into Supabase Dev schema (use n8n or SQL INSERT scripts)
- [ ] Verify data quality (no null coordinates, valid addresses)

**Acceptance Criteria:** Coordinator log in → Sees 100+ records in map/list, no errors.

---

## 🧠 Phase 3: AI Pipeline & n8n Integration (2 weeks)

**Goal:** Connect AI recommendation system end-to-end.

### 3-1. n8n Workflow Setup
**Files to create/configure:**
- `n8n/workflows/match-analyze.json` (n8n workflow export)
- `Services/AIMatchingService.cs` (Blazor webhook caller)

**n8n Workflow Steps:**
1. **Webhook Trigger:** Accept POST from Blazor with payload:
   ```json
   {
     "user_id": "...",
     "candidate_ids": ["...", "..."],
     "user_context": "User text description",
     "assistant_contexts": ["...", "..."]
   }
   ```

2. **LLM Node (Claude 3.5):**
   - Prompt: "Compare this user's needs with these 3 assistants. Score each 0-100 and explain why."
   - Output: JSON with scores and reasons

3. **Supabase Node:**
   - Save results to `ai_recommendations` table
   - Update `matches.ai_score` and `matches.ai_reason`

4. **Supabase Realtime Broadcast:**
   - Emit event to channel `match_analysis:${user_id}` so Blazor app receives update

**Acceptance Criteria:** n8n workflow runs end-to-end, results saved in Supabase.

---

### 3-2. C# Service Layer for AI Calls
**File:** `Services/AIMatchingService.cs`

```csharp
public class AIMatchingService
{
    private readonly HttpClient _http;
    private readonly string _n8nWebhookUrl;

    public async Task<AIRecommendationResult> AnalyzeMatchAsync(
        string userId, 
        List<string> candidateIds,
        string userContext,
        List<string> assistantContexts)
    {
        var payload = new
        {
            user_id = userId,
            candidate_ids = candidateIds,
            user_context = userContext,
            assistant_contexts = assistantContexts
        };

        var request = new HttpRequestMessage(HttpMethod.Post, _n8nWebhookUrl)
        {
            Content = new StringContent(
                JsonSerializer.Serialize(payload),
                Encoding.UTF8,
                "application/json")
        };

        var response = await _http.SendAsync(request);
        
        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException($"n8n returned {response.StatusCode}");
        }

        // Parse response and return result
        return await JsonSerializer.DeserializeAsync<AIRecommendationResult>(
            await response.Content.ReadAsStreamAsync());
    }
}
```

**Acceptance Criteria:** Service calls n8n, handles errors gracefully.

---

### 3-3. Supabase Realtime Subscription in Blazor
**File:** `Pages/Matching.razor`

```csharp
protected override async Task OnInitializedAsync()
{
    // Subscribe to AI results
    var channel = supabase
        .Realtime
        .Channel($"match_analysis:{CurrentUserId}")
        .On("INSERT", new SupabaseEventHandler<AIRecommendation>(
            payload =>
            {
                AIResults = payload.Payload.NewRecord;
                StateHasChanged();
            }));
    
    await channel.Subscribe();
}
```

**Acceptance Criteria:** When n8n saves results, Blazor UI updates in real-time (WebSocket).

---

### 3-4. UI for AI Results Display
**File:** `Components/Cards/AIRecommendationCard.razor`

**Features:**
- [ ] Show "Analyzing..." spinner while AI is working
- [ ] Display TOP 3 results with:
  - Score badge (e.g., "92/100")
  - Assistant name + distance
  - 2-3 line AI reasoning in natural Korean
  - "Select" button to choose this recommendation

**Acceptance Criteria:** Coordinator clicks "AI Analyze" → Spinner shows → Results appear in 5-10 seconds → Can select one.

---

### 3-5. Integration Test: Full AI Flow
**Manual test:**
1. Coordinator selects a user and 3 assistants from filtered list
2. Clicks "AI Recommendation" button
3. Monitor:
   - Spinner appears
   - n8n webhook receives request
   - n8n processes LLM call
   - Supabase receives results
   - Blazor receives Realtime event
   - UI shows TOP 3 with scores
4. Coordinator clicks one result → Modal opens for final confirmation

**Acceptance Criteria:** Full flow works end-to-end, no errors in any component.

---

## 📦 Phase 4: Testing, Deployment & Polish (2 weeks)

**Goal:** Ensure app is production-ready and distributable.

### 4-1. Automated Testing
**Files:**
- `Tests/MatchingServiceTests.cs` (Unit tests)
- `Tests/NaverMapIntegrationTests.cs` (Integration tests)

**Test Cases:**
```csharp
[Fact]
public async Task GetNearbyAssistants_ReturnsFilteredResults()
{
    // Setup: Mock Supabase
    // Act: Call service with user lat/lng and 3km radius
    // Assert: Results count == expected, all within radius
}

[Fact]
public async Task AIAnalysis_ReturnsScoresWithinRange()
{
    // Setup: Mock n8n response
    // Act: Call AIMatchingService
    // Assert: All scores between 0-100, reasons are non-empty
}
```

**Acceptance Criteria:** Unit tests pass with >80% code coverage on critical paths.

---

### 4-2. Manual Regression Testing
**Checklist (run before each release):**
- [ ] Map loads and displays all markers within 3 seconds
- [ ] Clicking user marker draws radius and filters list instantly (<100ms)
- [ ] Changing radius preset button updates map and list correctly
- [ ] Filters (time, gender, vehicle) work individually and combined
- [ ] AI Recommendation button triggers analysis and displays results
- [ ] Clicking "Quick Match" opens modal with correct comparison data
- [ ] "Confirm Match" button saves to database
- [ ] Mobile app can view today's schedule and receive notifications
- [ ] Logout clears session and redirects to login

**Acceptance Criteria:** All items pass without errors.

---

### 4-3. Performance Optimization
- [ ] Profile app startup time (target: <2 seconds to show map)
- [ ] Measure query times (target: RPC <500ms, AI analysis <10s)
- [ ] Optimize marker rendering with clustering (target: 1000+ markers <lag)
- [ ] Minimize bundle size (remove unused MudBlazor components)

**Acceptance Criteria:** App meets all performance targets.

---

### 4-4. Security Audit
- [ ] Verify no API keys hardcoded in code (all in `appsettings.json` or environment vars)
- [ ] Test RLS policies (unauthorized users cannot access data)
- [ ] Verify HTTPS in all API calls
- [ ] Review n8n webhook authentication (use secret token or IP whitelist)

**Acceptance Criteria:** No security vulnerabilities found in code review.

---

### 4-5. Build & Package (Windows MSIX)
**File:** `Package.appxmanifest`

**Tasks:**
- [ ] Configure `Package.appxmanifest`:
  - App name: "LMP-Link"
  - Version: 1.0.0.0
  - Publisher: Agency name
- [ ] Build release configuration:
  ```bash
  dotnet publish -c Release -f net9.0-windows10.0.19041.0
  ```
- [ ] Create MSIX package:
  ```bash
  dotnet publish -c Release -p:WindowsAppSDKSelfContained=true
  ```
- [ ] Sign certificate (use self-signed for internal distribution)
- [ ] Test installation on clean Windows 10/11 machine

**Acceptance Criteria:** MSIX installs and runs without errors.

---

### 4-6. Build & Package (Android APK)
**File:** `Platforms/Android/AndroidManifest.xml`

**Tasks:**
- [ ] Configure KeyStore for signing:
  ```bash
  keytool -genkey -v -keystore lmp-link.keystore -keyalg RSA -keysize 2048 -validity 10000 -alias lmp-key
  ```
- [ ] Build release APK:
  ```bash
  dotnet publish -c Release -f net9.0-android
  ```
- [ ] Sign APK with KeyStore
- [ ] Test installation on Android 9+ device

**Acceptance Criteria:** APK installs on real device, app functions correctly.

---

### 4-7. Documentation & User Guides
**Files to create:**
- `docs/Installation_Guide_Windows.pdf`
  - System requirements
  - Step-by-step MSIX installation
  - Login with coordinator account
  - Basic operation (find user, filter assistants, run AI)
  
- `docs/Installation_Guide_Android.pdf`
  - APK download link
  - Installation via file manager
  - First login and notification setup
  
- `docs/FAQ.md`
  - "Why is the radius not showing?"
  - "How do I reset my password?"
  - "What if AI analysis takes too long?"
  - Contact support info

**Acceptance Criteria:** Guides are clear, screenshots match actual app.

---

### 4-8. Deployment & Release
**Tasks:**
- [ ] Tag git repo: `git tag -a v1.0.0 -m "Release 1.0.0"`
- [ ] Upload MSIX to internal file server
- [ ] Upload APK to internal file server
- [ ] Create release notes: New features, bug fixes, known issues
- [ ] Send installation guide to coordinators + activity support staff
- [ ] Set up support email/Slack channel for feedback

**Acceptance Criteria:** All stakeholders receive installation links and guides.

---

## 📊 Success Metrics

Before declaring v1.0 complete, verify:

1. **Performance:**
   - App startup: <2 seconds
   - Map load: <3 seconds
   - Radius query: <500ms
   - AI analysis: <10 seconds

2. **Functionality:**
   - 100% of Phase 1-4 tasks completed
   - All test cases passing
   - Zero Critical bugs

3. **User Feedback:**
   - Coordinator feedback positive (usable without developer)
   - No crashes on 8+ hours of usage
   - AI recommendations are useful (score ≥4/5)

4. **Data:**
   - Support 200+ users, 1000+ assistants
   - No data loss or corruption after 100+ matches
   - RLS policies working correctly

---

## 🚀 Post v1.0 Roadmap (Future)

Once v1.0 is stable in production, consider:

- **v1.1:** Real-time GPS tracking for assistants
- **v1.2:** SMS/KakaoTalk notifications for assignments
- **v2.0:** Web dashboard for agency administrators
- **v2.1:** Scheduling/calendar integration

---

## 📞 Support & Escalation

**During development:**
- Blocker: Immediately escalate to architect
- High: Fix within 24 hours
- Medium: Schedule in current sprint
- Low: Backlog for next sprint
