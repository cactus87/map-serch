## MVP Plan — 장애인 활동지원 매칭 (Naver Maps, Blazor Server)

### Goal
- Prove map-based radius search cuts matching time (Excel 30m → map 3m).

### Timeline & Phases
- **Day 1 — Phase 1: Project setup + map render**
  - Blazor Server (.NET 9) scaffold, MudBlazor wired.
  - MainLayout 30/70 split (list/map). Naver Maps JS API v3 interop ready.
  - Map centers on Dobong-gu Office (37.6688, 127.0471).
- **Day 2-3 — Phase 2: Mock data + markers**
  - `Person`, `PersonType` models; `MockDataService` with 10 users, 20 assistants (0–4km).
  - JS interop functions to add markers; styles: users blue circles, assistants orange squares.
- **Day 4-5 — Phase 3: Radius logic**
  - User marker click → draw semi-transparent blue circle at radius.
  - Haversine distance util; hide assistant markers outside radius.
  - Left MudDataGrid filters/syncs with radius results.
- **Day 6-7 — Phase 4: UI polish**
  - Top MudButtonGroup: 1km / 3km / All.
  - Distance (km) column sortable; radius change re-filters.
  - Final UX polish and demo prep.

### Acceptance Criteria
- Map initial load <5s; radius filter response <1s (for 30 markers).
- Markers visually distinct by role; circle renders on user click.
- Grid and map stay in sync for radius filters (1km/3km/All).
- Demo flow: select user → circle appears → assistants filtered; repeat quickly.

### Work Checklist
- [ ] Phase 1: map shows Dobong center; layout split present; Naver API key injected via config (no hardcode).
- [ ] Phase 2: mock data available; markers render with correct colors/shapes.
- [ ] Phase 3: radius selection hides out-of-range assistants; grid filtered; Haversine used.
- [ ] Phase 4: button group, distance column sorting, polish passes.
- [ ] Performance check: load/filter timings measured.
- [ ] Demo script drafted and rehearsed.

### Integration Notes
- JS interop surface (naverMap.js): init, addMarker, drawCircle, setMarkerVisibility, clearMarkers.
- Keep DTO minimal: `{ id, type, lat, lng, name, address }`.
- Avoid duplicate script loads; guard interop if key missing.

### Data & Testing
- Static mock IDs; no randomness. Distances rounded to 2 decimals in UI.
- Tests (manual/auto): map init smoke, marker counts, radius filtering, distance calc edges (zero distance, far pairs).

### Risks & Mitigations
- API key handling: use user secrets/env; document setup; fail fast on missing key.
- Performance: batch interop calls per radius change; debounce UI triggers.
- UX regressions: keep grid-map sync as single source of truth in C#.

