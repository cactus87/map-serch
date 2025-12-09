# Supabase 데이터베이스 스키마

**프로젝트**: LMP-Link MVP  
**데이터베이스**: PostgreSQL 16 (Supabase)  
**확장 기능**: PostGIS 3.4+ (지리공간 데이터)  
**문서 버전**: 1.0  
**작성일**: 2025-12-10

---

## 📊 ERD 다이어그램

```
┌─────────────────┐         ┌─────────────────┐
│   persons       │         │   matches       │
├─────────────────┤         ├─────────────────┤
│ id (PK)         │◄───────┤ user_id (FK)    │
│ name            │         │ assistant_id(FK)├──────►┌─────────────────┐
│ type            │         │ status          │       │   persons       │
│ email           │         │ score           │       └─────────────────┘
│ phone           │         │ reason          │
│ address         │         │ created_at      │
│ location (geo)  │         │ confirmed_at    │
│ gender          │         └─────────────────┘
│ has_vehicle     │                  │
│ agency_id (FK)  │                  │
│ created_at      │                  ▼
└─────────────────┘         ┌─────────────────┐
         │                  │  notifications  │
         │                  ├─────────────────┤
         │                  │ id (PK)         │
         └─────────────────►│ person_id (FK)  │
                            │ match_id (FK)   │
                            │ type            │
                            │ message         │
                            │ read_at         │
                            │ created_at      │
                            └─────────────────┘
```

---

## 🗄️ 테이블 정의

### 1. persons (이용자 + 지원사 통합)

**설명**: 이용자(User)와 활동지원사(Assistant)를 하나의 테이블에 저장

```sql
CREATE TABLE persons (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  
  -- 기본 정보
  name VARCHAR(100) NOT NULL,
  type VARCHAR(20) NOT NULL CHECK (type IN ('user', 'assistant')),
  email VARCHAR(255) UNIQUE NOT NULL,
  phone VARCHAR(20),
  
  -- 주소 & 위치
  address TEXT NOT NULL,
  location GEOGRAPHY(POINT, 4326) NOT NULL, -- PostGIS 지리공간 타입
  
  -- 프로필 (NULL 허용, 이용자는 일부만 사용)
  gender VARCHAR(10) CHECK (gender IN ('male', 'female', 'other', NULL)),
  has_vehicle BOOLEAN DEFAULT false,
  available_time_slots JSONB, -- {"weekday_am": true, "weekend_pm": false, ...}
  experience_years INTEGER,
  certifications JSONB, -- ["cert1", "cert2", ...]
  skip_list JSONB DEFAULT '[]'::JSONB, -- 기피 대상 ID 리스트
  
  -- 메타데이터
  agency_id UUID REFERENCES agencies(id),
  created_by UUID REFERENCES auth.users(id),
  created_at TIMESTAMPTZ DEFAULT NOW(),
  updated_at TIMESTAMPTZ DEFAULT NOW(),
  
  -- 인덱스
  CONSTRAINT persons_email_key UNIQUE (email)
);

-- 지리공간 인덱스 (반경 검색 성능 최적화)
CREATE INDEX idx_persons_location ON persons USING GIST (location);

-- 타입별 인덱스
CREATE INDEX idx_persons_type ON persons (type);

-- 기관별 인덱스
CREATE INDEX idx_persons_agency_id ON persons (agency_id);
```

**샘플 데이터**:
```sql
-- 이용자 예시
INSERT INTO persons (name, type, email, phone, address, location, gender)
VALUES (
  '김철수',
  'user',
  'user1@example.com',
  '010-1234-5678',
  '서울특별시 도봉구 마들로 656',
  ST_SetSRID(ST_MakePoint(127.0471, 37.6688), 4326),
  'male'
);

-- 지원사 예시
INSERT INTO persons (name, type, email, phone, address, location, gender, has_vehicle, experience_years)
VALUES (
  '이영희',
  'assistant',
  'assistant1@example.com',
  '010-9876-5432',
  '서울특별시 도봉구 방학동',
  ST_SetSRID(ST_MakePoint(127.0550, 37.6700), 4326),
  'female',
  true,
  3
);
```

---

### 2. matches (매칭 결과)

**설명**: 이용자-지원사 매칭 기록 (AI 추천 점수 포함)

```sql
CREATE TABLE matches (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  
  -- 관계
  user_id UUID NOT NULL REFERENCES persons(id) ON DELETE CASCADE,
  assistant_id UUID NOT NULL REFERENCES persons(id) ON DELETE CASCADE,
  
  -- 매칭 상태
  status VARCHAR(20) NOT NULL DEFAULT 'proposed' 
    CHECK (status IN ('proposed', 'accepted', 'rejected', 'confirmed', 'cancelled')),
  
  -- AI 추천 결과
  score INTEGER CHECK (score BETWEEN 0 AND 100), -- AI 매칭 점수 (0-100)
  reason TEXT, -- AI 추천 이유 (자연어)
  
  -- 거리 정보
  distance_km DECIMAL(5,2), -- 거리(km, 소수점 2자리)
  
  -- 메타데이터
  created_by UUID REFERENCES auth.users(id), -- 생성한 코디네이터
  created_at TIMESTAMPTZ DEFAULT NOW(),
  confirmed_at TIMESTAMPTZ, -- 최종 확정 시각
  cancelled_at TIMESTAMPTZ,
  
  -- 제약 조건
  CONSTRAINT matches_user_assistant_unique UNIQUE (user_id, assistant_id, created_at),
  CONSTRAINT matches_check_different_persons CHECK (user_id != assistant_id)
);

-- 이용자별 매칭 조회
CREATE INDEX idx_matches_user_id ON matches (user_id);

-- 지원사별 매칭 조회
CREATE INDEX idx_matches_assistant_id ON matches (assistant_id);

-- 상태별 조회
CREATE INDEX idx_matches_status ON matches (status);
```

**샘플 데이터**:
```sql
INSERT INTO matches (user_id, assistant_id, status, score, reason, distance_km)
VALUES (
  '이용자_UUID',
  '지원사_UUID',
  'confirmed',
  92,
  '경력 3년, 시간대 호환 100%, 거리 2.1km로 가까움. 차량 보유로 이동 편리.',
  2.10
);
```

---

### 3. notifications (알림)

**설명**: 앱 내 알림 (매칭 제안, 확정, 취소 등)

```sql
CREATE TABLE notifications (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  
  -- 관계
  person_id UUID NOT NULL REFERENCES persons(id) ON DELETE CASCADE,
  match_id UUID REFERENCES matches(id) ON DELETE SET NULL,
  
  -- 알림 내용
  type VARCHAR(50) NOT NULL 
    CHECK (type IN ('match_proposed', 'match_confirmed', 'match_cancelled', 'message')),
  title VARCHAR(200) NOT NULL,
  message TEXT NOT NULL,
  
  -- 상태
  read_at TIMESTAMPTZ,
  created_at TIMESTAMPTZ DEFAULT NOW(),
  
  -- 메타데이터
  metadata JSONB -- 추가 데이터 (링크, 액션 등)
);

-- 사용자별 알림 조회
CREATE INDEX idx_notifications_person_id ON notifications (person_id);

-- 읽지 않은 알림 조회
CREATE INDEX idx_notifications_read_at ON notifications (read_at) WHERE read_at IS NULL;
```

**샘플 데이터**:
```sql
INSERT INTO notifications (person_id, match_id, type, title, message)
VALUES (
  '지원사_UUID',
  '매칭_UUID',
  'match_proposed',
  '새로운 매칭 제안',
  '김철수님과의 매칭이 제안되었습니다. 거리: 2.1km, AI 점수: 92점'
);
```

---

### 4. agencies (기관) - Optional

**설명**: 여러 기관을 지원하는 경우

```sql
CREATE TABLE agencies (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  name VARCHAR(200) NOT NULL,
  code VARCHAR(50) UNIQUE NOT NULL, -- 기관 코드
  address TEXT,
  phone VARCHAR(20),
  admin_email VARCHAR(255) NOT NULL,
  created_at TIMESTAMPTZ DEFAULT NOW()
);
```

---

## 🔒 Row-Level Security (RLS) 정책

### persons 테이블 RLS

```sql
-- RLS 활성화
ALTER TABLE persons ENABLE ROW LEVEL SECURITY;

-- 정책 1: 코디네이터는 자신의 기관 데이터만 조회
CREATE POLICY "coordinators_view_own_agency" ON persons
  FOR SELECT
  USING (
    agency_id IN (
      SELECT agency_id FROM coordinators WHERE user_id = auth.uid()
    )
  );

-- 정책 2: 지원사는 자신의 프로필만 조회/수정
CREATE POLICY "assistants_view_own_profile" ON persons
  FOR SELECT
  USING (
    type = 'assistant' AND id = auth.uid()
  );

-- 정책 3: 코디네이터는 자신의 기관 데이터 수정 가능
CREATE POLICY "coordinators_update_own_agency" ON persons
  FOR UPDATE
  USING (
    agency_id IN (
      SELECT agency_id FROM coordinators WHERE user_id = auth.uid()
    )
  );
```

---

### matches 테이블 RLS

```sql
ALTER TABLE matches ENABLE ROW LEVEL SECURITY;

-- 코디네이터: 자신의 기관 매칭 조회
CREATE POLICY "coordinators_view_agency_matches" ON matches
  FOR SELECT
  USING (
    user_id IN (SELECT id FROM persons WHERE agency_id = get_user_agency())
  );

-- 지원사: 자신의 매칭만 조회
CREATE POLICY "assistants_view_own_matches" ON matches
  FOR SELECT
  USING (
    assistant_id = auth.uid()
  );

-- 지원사: 자신의 매칭 상태 변경 (accept/reject)
CREATE POLICY "assistants_update_own_match_status" ON matches
  FOR UPDATE
  USING (assistant_id = auth.uid())
  WITH CHECK (status IN ('accepted', 'rejected'));
```

---

### notifications 테이블 RLS

```sql
ALTER TABLE notifications ENABLE ROW LEVEL SECURITY;

-- 사용자는 자신의 알림만 조회
CREATE POLICY "users_view_own_notifications" ON notifications
  FOR SELECT
  USING (person_id = auth.uid());

-- 사용자는 자신의 알림 읽음 처리 가능
CREATE POLICY "users_update_own_notifications" ON notifications
  FOR UPDATE
  USING (person_id = auth.uid())
  WITH CHECK (person_id = auth.uid());
```

---

## 📐 PostGIS 지리공간 쿼리

### 반경 검색 (Radius Search)

```sql
-- 도봉구청 기준 3km 이내 지원사 검색
SELECT 
  id,
  name,
  address,
  ST_Distance(
    location,
    ST_SetSRID(ST_MakePoint(127.0471, 37.6688), 4326)::geography
  ) / 1000 AS distance_km -- 미터 → 킬로미터
FROM persons
WHERE 
  type = 'assistant'
  AND ST_DWithin(
    location,
    ST_SetSRID(ST_MakePoint(127.0471, 37.6688), 4326)::geography,
    3000 -- 3km = 3000m
  )
ORDER BY distance_km ASC;
```

**성능 최적화**:
- ✅ `ST_DWithin`은 GIST 인덱스를 사용하여 빠름 (O(log n))
- ✅ `ST_Distance`는 정확한 거리 계산용 (결과 정렬에만 사용)

---

### 두 지점 간 거리 계산

```sql
-- 이용자 ID와 지원사 ID로 거리 계산
SELECT 
  u.name AS user_name,
  a.name AS assistant_name,
  ST_Distance(u.location, a.location)::numeric / 1000 AS distance_km
FROM persons u
CROSS JOIN persons a
WHERE u.id = '이용자_UUID'
  AND a.id = '지원사_UUID';
```

---

## 🔧 Helper Functions

### 1. 사용자 기관 조회

```sql
CREATE OR REPLACE FUNCTION get_user_agency()
RETURNS UUID AS $$
  SELECT agency_id 
  FROM coordinators 
  WHERE user_id = auth.uid()
$$ LANGUAGE SQL STABLE;
```

---

### 2. 반경 내 지원사 검색 (Function)

```sql
CREATE OR REPLACE FUNCTION find_assistants_in_radius(
  p_user_id UUID,
  p_radius_km DECIMAL
)
RETURNS TABLE (
  id UUID,
  name VARCHAR,
  address TEXT,
  distance_km DECIMAL
) AS $$
BEGIN
  RETURN QUERY
  SELECT 
    a.id,
    a.name,
    a.address,
    (ST_Distance(u.location, a.location) / 1000)::DECIMAL(5,2) AS distance_km
  FROM persons u
  CROSS JOIN persons a
  WHERE u.id = p_user_id
    AND a.type = 'assistant'
    AND ST_DWithin(
      u.location,
      a.location::geography,
      p_radius_km * 1000
    )
  ORDER BY distance_km ASC;
END;
$$ LANGUAGE plpgsql STABLE;
```

**사용 예시**:
```sql
SELECT * FROM find_assistants_in_radius('이용자_UUID', 3.0);
```

---

## 🔗 Supabase Realtime 설정

### Realtime 채널 활성화

```sql
-- matches 테이블 Realtime 활성화
ALTER PUBLICATION supabase_realtime ADD TABLE matches;

-- notifications 테이블 Realtime 활성화
ALTER PUBLICATION supabase_realtime ADD TABLE notifications;
```

**C# 클라이언트 구독 예시**:
```csharp
// 매칭 업데이트 실시간 구독
var channel = supabase.Realtime.Channel("matches");
channel.On<Match>(
    ChannelEventType.Insert, 
    (sender, args) => {
        // 새 매칭 알림
    }
);
await channel.Subscribe();
```

---

## 📊 성능 최적화 체크리스트

- [x] persons.location에 GIST 인덱스 생성
- [x] persons.type에 인덱스 생성
- [x] matches.user_id, matches.assistant_id에 인덱스 생성
- [x] notifications.person_id에 인덱스 생성
- [x] RLS 정책 최적화 (agency_id 조인 최소화)
- [ ] EXPLAIN ANALYZE로 쿼리 성능 검증 (>1000 rows)
- [ ] Connection Pooling 설정 (Supabase 기본 제공)

---

## 🔗 참고 문서

- [PostGIS 공식 문서](https://postgis.net/docs/)
- [Supabase RLS 가이드](https://supabase.com/docs/guides/auth/row-level-security)
- [Supabase Realtime](https://supabase.com/docs/guides/realtime)

---

**문서 버전**: 1.0  
**마지막 업데이트**: 2025-12-10  
**다음 업데이트**: Phase 2 (Week 3) Supabase 연동 시
