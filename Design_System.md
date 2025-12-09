# Design System & UX Guidelines (LMP-Link v1.0)

## 🎨 Theme Concept: "Modern HUD for Professionals"

- **Background:** Deep Navy (#111827) for concentration and professionalism.
- **Surface:** Dark Grey/Blue (#1F2937) for panels/cards with subtle contrast.
- **Text:** Primary White (#F9FAFB), Secondary Grey (#9CA3AF), Tertiary Grey (#6B7280).
- **Accent/CTA:** Teal/Mint (#2DD4BF) for primary actions (AI Button, key interactions).
- **Warning/Error:** Amber (#FBBF24) for cautions, Red (#EF4444) for critical issues.
- **Success:** Green (#10B981) for confirmations and positive states.

---

## 📐 Layout Rules (Windows Desktop - Coordinator)

### 1. Split View Architecture
- **Left (70%):** Map Area (Full responsive height).
- **Right (30%):** Matching Panel (Scrollable).
- **Top (Full width):** AppBar with status information.

### 2. Top AppBar (High Visibility)
- **Left Section:**
  - Large Logo: "LMP-Link"
  - Subtitle (smaller): "장애인 활동지원 매칭 콘솔"
- **Center Section:**
  - Current Date/Time
  - Quick Stats: "Today: 12 processed / 2 Waiting for AI"
- **Right Section:**
  - Coordinator Name (logged-in user)
  - Dev/Prod Environment Toggle (clearly labeled)
  - Logout Button

### 3. Map Area (Left 70%)
- **Naver Map Full View**
- **Top-Right Floating Controls Box:**
  - Radius Preset Buttons: `[1km] [3km (default emphasis)] [5km]`
  - Layer Toggles: `[Show Users ON/OFF] [Show Assistants ON/OFF]`
- **Top-Left Search Box:**
  - Input: "Search User by Name/ID"
  - Button: "Search" (Blue Teal)
- **Marker Styling:**
  - **User:** Bold-bordered Blue Circle, slightly larger size (30x30px)
  - **Assistant:** Orange Square, same size (24x24px)
  - **Clustering:** When zoomed out, cluster bubbles with large white numbers inside
- **One-Click Radius (Key Interaction):**
  - Click any User Marker → Immediately draw 3km Circle
  - Map bottom shows label: "Radius: 3km / Assistants: 7 found"
  - Right panel auto-filters and scrolls to matching assistants

### 4. Right HUD Panel (30%)
- **Tab Navigation (Top):**
  - `[User List] [Matching Candidates / AI Recommendation]`
  
#### Tab 1: User List
- **Each Item (Large, scrollable list):**
  - User Name (Bold, 16pt+)
  - Address Summary (City/District only)
  - Main Requirement (1-line summary)
  - Current Status Badge (Unmatched / In Progress / Completed)
  - On Click: Highlight in map + Auto 3km radius + Jump to Tab 2
  
#### Tab 2: Matching Candidates / AI Recommendation
- **Top Filter Bar (Large, clear controls):**
  - Time Slot Dropdown (Weekday AM / Weekday PM / Weekend)
  - Gender Toggle (All / M / F)
  - Own Vehicle Toggle (Include / Exclude)
  - Skip Preference Checkbox (Exclude blacklist)
  - "Reset Filters" Link (right end)
  
- **Candidate List (Below filters):**
  - Each Assistant Card format:
    - Name, Distance (e.g., "2.1km")
    - Experience Summary, Time Compatibility Icon (Green check / Yellow warning)
    - "Quick Match" Button (right side, pill-shaped)
  
- **AI Recommendation Button (Center-Top, after filters):**
  - **Inactive State (before selection):** Grey, "Select candidates first"
  - **Active State (after 1+ candidate):** Bold Teal, "Run AI Analysis"
  - On Click:
    - Thin ProgressBar appears at top of panel
    - After completion: "AI Recommendation TOP 3" section appears (pinned to top)
    - Top 3 candidates show with score badges (e.g., "92/100")

---

## 📱 Mobile Rules (Android - Assistant App)

### 1. Navigation Structure: 3-Tab Bottom Bar
- `[Today's Schedule] [Notifications] [My Profile]`

### 2. Tab: Today's Schedule
- **Top Section:**
  - Large Date Display: "Today, Tuesday Dec 9"
  - "2 assignments today"
  
- **Schedule Cards (scrollable):**
  - Time slot (e.g., "10:00–12:00")
  - User Name
  - Address (up to district level)
  - Simple requirement (1 line)
  - Card colors by status: Today=Bright / Future=Grey / Done=Dimmed
  
- **On Tap:**
  - Detail page opens:
    - Full Address with "Open in Map" button (launches Naver Maps)
    - Coordinator Phone (large button, tap to call)
    - Special Notes (text block)

### 3. Tab: Notifications
- **Top Badge:** "New Match Proposal Available"
- **Notification Items:**
  - "[New Match] Kim Hong-gil – 3x/week Afternoon / Gangnam-gu"
    - Buttons: `[Review] [Decline]`
  - "Match Confirmed – You're assigned to Park Min-su"
  - "New Time Slot Available – Verify Availability"
  
- **On [Review] Tap:**
  - Detail page (similar to Schedule, but with Accept/Decline buttons at bottom)

### 4. Tab: My Profile
- **Profile Section:**
  - Profile Photo (optional)
  - Name
  - Qualification/Experience Tags
  
- **Settings Toggles:**
  - Preferred Work Hours (toggle)
  - Preferred Region (select)
  - Skip Preference (checkbox)
  
- **Save Button (bottom, large, fixed):**

### 5. Mobile Accessibility Rules
- **Font:** Bold, readable Korean font (Apple SD Gothic, Roboto)
- **Button Size:** Minimum 48px height
- **Font Size:** 18pt+ for all body text
- **Touch Target Spacing:** Minimum 8px between interactive elements
- **Colors:** High contrast on Dark background (WCAG AA compliant)

---

## 🧩 Key Component Specifications

### Component 1: HUD Panel (Right Side)
- **Purpose:** Real-time matching operation interface
- **Structure:**
  - Top: Tabs + Filter controls
  - Middle: Scrollable card list
  - Top-Fixed: AI Recommendation button & results
- **Interactions:**
  - Filter → List updates in <100ms
  - Card click → Map highlight
  - AI button → Loading state → Results

### Component 2: Comparison Modal (Detail View)
- **Trigger:** Click "Quick Match" button or double-click list item
- **Layout:**
  - Header: "Matching Review" + Close + Save buttons
  - Body:
    - Left Column: User Info Card
    - Right Column: Assistant Info Card
    - Center Top: Match Score Bar (0-100, green gradient)
    - Center: 3 small icons (Distance / Time Compatibility / Personality)
  - Bottom: AI Recommendation Reason (3-line summary in natural Korean)

### Component 3: AI Insight Box
- **Styling:** Distinct card with sparkle/star icon
- **Content:** 
  - Title: "AI Recommendation Reason"
  - Body: 3-4 short, natural Korean sentences
  - Action: "[Regenerate Opinion]" link
- **Font:** 14pt, friendly but professional

### Component 4: Marker Styling
- **User Marker (Blue Circle):**
  - Background: #3B82F6 (Blue)
  - Icon: White person silhouette
  - Border: Thick white (2-3px)
  - Size: 32x32px
  
- **Assistant Marker (Orange Square):**
  - Background: #F97316 (Orange)
  - Icon: White briefcase/document
  - Border: Thin white (1px)
  - Size: 24x24px

### Component 5: Cluster Marker
- **Style:** Rounded bubble with white background
- **Content:** Large white number (font 18pt+)
- **Background:** Semi-transparent grey (#64748B, opacity 80%)
- **Size:** Adaptive based on cluster size

---

## 🎯 Interaction Flows

### Flow 1: "Find Nearby Assistants" (Main Workflow)
1. User clicks on a **Blue Circle (User Marker)** on the map
2. **3km Circle** is instantly drawn around that coordinate
3. **Right panel filters:** All matching assistants appear in cards
4. Map **dims out** assistants outside the radius (opacity 50%)
5. Coordinator can change radius with preset buttons
6. Coordinator scans through cards, optionally triggers **AI Analysis**

### Flow 2: "Quick Match" (Simplified Matching)
1. Coordinator selects 1-3 assistants from filtered list
2. Clicks **AI Analysis** button
3. **ProgressBar** appears, analysis runs (2-10 seconds)
4. **AI TOP 3 section** materializes above the list
5. Each card shows score badge + brief reason
6. Coordinator clicks "Quick Match" → Confirmation Modal appears
7. On confirm → Record saved to `matches` table (status: Draft)

### Flow 3: "Finalize Match" (Admin Confirmation)
1. Modal title changes to "Match Confirmation"
2. Old button "Quick Match" is replaced with "Confirm & Notify"
3. On click → Status changes to "Confirmed"
4. Realtime event triggers → Mobile app displays notification

---

## 🌈 Color Palette Reference

| Element | Color Code | Hex | Usage |
|---------|-----------|-----|-------|
| Background | Deep Navy | #111827 | Page background, focus areas |
| Surface | Dark Grey | #1F2937 | Panels, cards, modals |
| Light Surface | Grey Blue | #111F35 | Slightly raised panels |
| Text Primary | Off White | #F9FAFB | Main text |
| Text Secondary | Light Grey | #9CA3AF | Secondary labels |
| Text Tertiary | Medium Grey | #6B7280 | Disabled, hints |
| Primary Action | Teal | #2DD4BF | AI button, primary CTA |
| User Marker | Blue | #3B82F6 | User markers, highlights |
| Assistant Marker | Orange | #F97316 | Assistant markers |
| Success | Green | #10B981 | Confirmations |
| Warning | Amber | #FBBF24 | Cautions |
| Error | Red | #EF4444 | Errors, critical alerts |

---

## 📝 Typography Guidelines

| Level | Size | Weight | Example |
|-------|------|--------|---------|
| Display | 28-32pt | Bold (700) | "LMP-Link", Modal titles |
| Heading 1 | 24pt | Bold (700) | Page sections |
| Heading 2 | 20pt | Bold (600) | Card titles |
| Body | 16-18pt | Regular (400) | Main content |
| Small | 14pt | Regular (400) | Secondary info |
| Caption | 12pt | Regular (400) | Hints, metadata |
| Button | 16-18pt | Bold (600) | Interactive elements |

---

## ✅ Accessibility & Usability Checklist

- [ ] **Color Contrast:** All text meets WCAG AA standard (4.5:1 for body, 3:1 for large text)
- [ ] **Touch Targets:** Minimum 48x48px for mobile buttons
- [ ] **Font Readability:** Bold, sans-serif Korean fonts (Apple SD Gothic, Roboto)
- [ ] **Shape Differentiation:** Not color-only → Use shapes (circle vs square) for marker distinction
- [ ] **Error Messages:** Clear, actionable text in user's language
- [ ] **Loading States:** Always show progress indicator for async operations
- [ ] **Focus Indicators:** Clear visual focus for keyboard/tab navigation
- [ ] **Dark Mode:** High contrast maintained throughout
