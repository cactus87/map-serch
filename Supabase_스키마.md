# Supabase 데이터베이스 스키마 v2.0

**프로젝트**: LMP-Link MVP  
**데이터베이스**: PostgreSQL 16 (Supabase)  
**확장 기능**: PostGIS 3.4+ (지리공간 데이터)  
**문서 버전**: 2.0  
**작성일**: 2025-12-10  
**최종 업데이트**: 2025-12-11

---

## 📊 ERD 다이어그램 (v2.0)

```
┌─────────────────────────────────────────────────────────────────────────────┐
│                              MASTER TABLES                                  │
├─────────────────────────────────────────────────────────────────────────────┤
│                                                                             │
│  ┌─────────────────┐      ┌─────────────────┐      ┌─────────────────┐     │
│  │ disability_types│      │   job_categories │      │   work_types    │     │
│  ├─────────────────┤      ├─────────────────┤      ├─────────────────┤     │
│  │ code (PK)       │      │ code (PK)       │      │ code (PK)       │     │
│  │ name            │      │ name            │      │ name            │     │
│  │ description     │      │ description     │      │ description     │     │
│  └────────┬────────┘      └────────┬────────┘      └────────┬────────┘     │
│           │                        │                        │               │
│           ▼                        ▼                        │               │
│  ┌─────────────────┐      ┌─────────────────┐               │               │
│  │ mobility_types  │      │   job_master    │               │               │
│  ├─────────────────┤      ├─────────────────┤               │               │
│  │ code (PK)       │      │ id (PK)         │               │               │
│  │ name            │      │ category_code   │               │               │
│  │ description     │      │ detail_code     │               │               │
│  └─────────────────┘      │ name            │               │               │
│                           │ description     │               │               │
│                           └────────┬────────┘               │               │
└────────────────────────────────────┼────────────────────────┼───────────────┘
                                     │                        │
┌────────────────────────────────────┼────────────────────────┼───────────────┐
│                              ENTITY TABLES                  │               │
├────────────────────────────────────┼────────────────────────┼───────────────┤
│                                    │                        │               │
│  ┌─────────────────────────────────┴────────────────────────┴─────────────┐ │
│  │                            users (수급자/이용자)                        │ │
│  ├────────────────────────────────────────────────────────────────────────┤ │
│  │ id (PK)              │ name              │ birth_date      │ age       │ │
│  │ gender               │ phone             │ email           │ address   │ │
│  │ latitude             │ longitude         │                             │ │
│  │ ─────────────────────┼─────────────────────────────────────────────────│ │
│  │ disability_type_code │ disability_level  │ disability_grade            │ │
│  │ uses_ventilator      │ mobility_code     │ care_grade                  │ │
│  │ monthly_hours        │ remaining_hours   │ service_start_date          │ │
│  │ ─────────────────────┼─────────────────────────────────────────────────│ │
│  │ preferred_gender     │ preferred_age_min │ preferred_age_max           │ │
│  │ preferred_job_codes  │ notes             │ emergency_contact           │ │
│  └────────────────────────────────────────────────────────────────────────┘ │
│                                                                             │
│  ┌────────────────────────────────────────────────────────────────────────┐ │
│  │                         assistants (활동지원사)                         │ │
│  ├────────────────────────────────────────────────────────────────────────┤ │
│  │ id (PK)              │ name              │ birth_date      │ age       │ │
│  │ gender               │ phone             │ email           │ address   │ │
│  │ latitude             │ longitude         │                             │ │
│  │ ─────────────────────┼─────────────────────────────────────────────────│ │
│  │ certification_date   │ certification_no  │ experience_years            │ │
│  │ education_level      │ has_vehicle       │ vehicle_type                │ │
│  │ ─────────────────────┼─────────────────────────────────────────────────│ │
│  │ work_type_code       │ preferred_region  │ max_weekly_hours            │ │
│  │ commute_time_minutes │ preferred_time_slots                            │ │
│  │ available_night      │ available_weekend │                             │ │
│  │ ─────────────────────┼─────────────────────────────────────────────────│ │
│  │ hard_restrictions    │ health_notes      │ notes                       │ │
│  └────────────────────────────────────────────────────────────────────────┘ │
│                                                                             │
│  ┌────────────────────────────────────────────────────────────────────────┐ │
│  │                     assistant_job_abilities (가능 업무)                 │ │
│  ├────────────────────────────────────────────────────────────────────────┤ │
│  │ id (PK)              │ assistant_id (FK) │ job_id (FK)                 │ │
│  │ proficiency_level    │ is_certified      │ certification_date          │ │
│  │ notes                │                                                 │ │
│  └────────────────────────────────────────────────────────────────────────┘ │
│                                                                             │
└─────────────────────────────────────────────────────────────────────────────┘
```

---

## 🗄️ 마스터 테이블 정의

### 1. disability_types (장애 유형 마스터)

```sql
CREATE TABLE disability_types (
  code VARCHAR(20) PRIMARY KEY,
  name VARCHAR(50) NOT NULL,
  description TEXT,
  is_active BOOLEAN DEFAULT true,
  sort_order INT DEFAULT 0
);

-- 샘플 데이터
INSERT INTO disability_types (code, name, description, sort_order) VALUES
  ('PHYSICAL', '지체장애', '팔, 다리, 몸통 등 신체 기능 장애', 1),
  ('BRAIN', '뇌병변장애', '뇌성마비, 뇌졸중 등으로 인한 장애', 2),
  ('VISUAL', '시각장애', '시력 손실 또는 시야 결손', 3),
  ('HEARING', '청각장애', '청력 손실', 4),
  ('SPEECH', '언어장애', '언어 기능 장애', 5),
  ('INTELLECTUAL', '지적장애', '지적 기능 저하', 6),
  ('AUTISM', '자폐성장애', '자폐 스펙트럼 장애', 7),
  ('MENTAL', '정신장애', '정신질환으로 인한 장애', 8),
  ('KIDNEY', '신장장애', '신장 기능 장애, 투석', 9),
  ('HEART', '심장장애', '심장 기능 장애', 10),
  ('RESPIRATORY', '호흡기장애', '호흡 기능 장애', 11),
  ('LIVER', '간장애', '간 기능 장애', 12),
  ('FACIAL', '안면장애', '안면 변형 또는 기형', 13),
  ('INTESTINAL', '장루/요루장애', '장루 또는 요루 사용', 14),
  ('EPILEPSY', '뇌전증장애', '뇌전증(간질)', 15);
```

---

### 2. mobility_types (이동 능력 마스터)

```sql
CREATE TABLE mobility_types (
  code VARCHAR(20) PRIMARY KEY,
  name VARCHAR(50) NOT NULL,
  description TEXT,
  requires_assistance BOOLEAN DEFAULT false,
  sort_order INT DEFAULT 0
);

-- 샘플 데이터
INSERT INTO mobility_types (code, name, description, requires_assistance, sort_order) VALUES
  ('INDEPENDENT', '독립보행', '보조 없이 보행 가능', false, 1),
  ('CANE', '지팡이보행', '지팡이 사용하여 보행', false, 2),
  ('WALKER', '보행기보행', '보행기(워커) 사용하여 보행', true, 3),
  ('WHEELCHAIR_SELF', '휠체어(자가)', '스스로 휠체어 조작 가능', false, 4),
  ('WHEELCHAIR_ASSIST', '휠체어(도움)', '휠체어 조작 도움 필요', true, 5),
  ('ELECTRIC_WHEELCHAIR', '전동휠체어', '전동휠체어 사용', false, 6),
  ('BEDRIDDEN', '와상', '거동 불가, 침상 생활', true, 7);
```

---

### 3. job_categories (직무 대분류 마스터)

```sql
CREATE TABLE job_categories (
  code VARCHAR(10) PRIMARY KEY,
  name VARCHAR(50) NOT NULL,
  description TEXT,
  icon VARCHAR(50),
  color VARCHAR(20),
  sort_order INT DEFAULT 0
);

-- 샘플 데이터 (disability_support_worker_guide.md 기반)
INSERT INTO job_categories (code, name, description, icon, color, sort_order) VALUES
  ('PHYS', '신체활동지원', '신체 기능 유지, 위생, 안전 지원', '🧍', '#3B82F6', 1),
  ('HOUSE', '가사활동지원', '가정 내 일상 가사 지원', '🏠', '#10B981', 2),
  ('SOCI', '사회활동·이동지원', '외부활동 이동 및 현장 지원', '🚗', '#F59E0B', 3),
  ('EMOT', '정서·의사소통지원', '정서 지지 및 의사소통 보조', '💬', '#EC4899', 4);
```

---

### 4. job_master (직무 상세 마스터)

```sql
CREATE TABLE job_master (
  id SERIAL PRIMARY KEY,
  category_code VARCHAR(10) NOT NULL REFERENCES job_categories(code),
  detail_code VARCHAR(20) UNIQUE NOT NULL,
  name VARCHAR(100) NOT NULL,
  description TEXT,
  required_skills TEXT,
  health_restrictions TEXT,
  is_active BOOLEAN DEFAULT true,
  sort_order INT DEFAULT 0,
  created_at TIMESTAMPTZ DEFAULT NOW()
);

CREATE INDEX idx_job_master_category ON job_master(category_code);

-- 샘플 데이터 (disability_support_worker_guide.md 기반)
INSERT INTO job_master (category_code, detail_code, name, description, required_skills, health_restrictions, sort_order) VALUES
  -- 신체활동지원 (PHYS)
  ('PHYS', 'PHYS01', '기상·취침 보조', '침대에서 기상, 취침 시 안전한 자세', '신체활동 기초', NULL, 1),
  ('PHYS', 'PHYS02', '체위변경·자세 유지', '욕창 예방 자세 변경', '신체활동, 해부학 기초', '심한 허리질환 주의', 2),
  ('PHYS', 'PHYS03', '실내이동·이송 보조', '침대-휠체어 이동, 실내 보행', '신체활동, 역학 이해', '심한 허리질환, 고혈압 주의', 3),
  ('PHYS', 'PHYS04', '세면·구강위생', '세수, 양치, 틀니 관리', '위생 관리, 세심함', NULL, 4),
  ('PHYS', 'PHYS05', '목욕 보조', '샤워·입욕 시 안전 지원', '신체활동, 수온 관리, 안전의식', '허리 약한 경우 주의', 5),
  ('PHYS', 'PHYS06', '옷 갈아입히기', '계절·상황에 맞는 의복 입기', '상황 판단, 존엄성 존중', NULL, 6),
  ('PHYS', 'PHYS07', '배뇨·배변 보조', '화장실 이용, 기저귀 교체', '위생 관리, 감염 예방', NULL, 7),
  ('PHYS', 'PHYS08', '식사 보조', '식사 준비, 섭취 도움', '영양 기초, 안전의식', '연하곤란 있는 경우 특별 교육 필요', 8),
  ('PHYS', 'PHYS09', '투약 보조(비의료)', '약 복용 시간 알림, 전달', '약물 관리 기초', NULL, 9),
  
  -- 가사활동지원 (HOUSE)
  ('HOUSE', 'HOUSE01', '주거공간 청소', '방·거실·욕실·주방 청소', '청소 기술, 화학 약품 안전', '폐질환·알레르기 주의', 10),
  ('HOUSE', 'HOUSE02', '침구·생활공간 정리정돈', '침대, 옷장, 물건 정리', '정리정돈, 분류 능력', NULL, 11),
  ('HOUSE', 'HOUSE03', '세탁', '의류, 침구 세탁 및 건조', '세탁 기술, 섬유 관리', '알레르기 주의', 12),
  ('HOUSE', 'HOUSE04', '취사(조리)', '밥, 반찬 조리, 식사 준비', '조리 기술, 위생, 영양', '알레르기 확인 필수', 13),
  ('HOUSE', 'HOUSE05', '설거지', '식기 세척, 주방 정리', '기본 위생, 꼼꼼함', NULL, 14),
  ('HOUSE', 'HOUSE06', '생필품 정리 및 재고관리', '식료품, 의약품 정리', '정리, 재고 관리', NULL, 15),
  
  -- 사회활동·이동지원 (SOCI)
  ('SOCI', 'SOCI01', '출퇴근·등하교 동행', '직장·학교 이동, 현장 기본 보조', '이동 지원, 시간 관리', NULL, 16),
  ('SOCI', 'SOCI02', '병원 동행', '진료 시설 방문, 진료 현장 동행', '의료 기관 이해, 침착성', NULL, 17),
  ('SOCI', 'SOCI03', '관공서·은행 동행', '공공기관 방문, 서류 처리 동행', '서류 이해, 절차 파악', NULL, 18),
  ('SOCI', 'SOCI04', '장보기·마트 동행', '쇼핑 동행, 물품 구매 지원', '자금 관리, 안전의식', NULL, 19),
  ('SOCI', 'SOCI05', '여가·문화·종교활동 동행', '복지관·공연·종교시설 이동', '사회활동 이해, 배려', NULL, 20),
  ('SOCI', 'SOCI06', '산책·지역사회 참여', '동네 산책, 지역 활동 동행', '지역 안내, 대화 능력', NULL, 21),
  
  -- 정서·의사소통지원 (EMOT)
  ('EMOT', 'EMOT01', '말벗·정서적 지지', '일상 대화, 공감적 경청', '소통 능력, 공감 능력', NULL, 22),
  ('EMOT', 'EMOT02', '일정·생활계획 함께 세우기', '일정 관리, 생활 계획 상의', '조직 능력, 계획 능력', NULL, 23),
  ('EMOT', 'EMOT03', '의사소통 보조', '전화·면접·설명 보조', '소통 능력', NULL, 24),
  ('EMOT', 'EMOT04', '대독·대필', '문서 읽어주기, 기록 작성', '읽기/쓰기 능력, 세심함', NULL, 25),
  ('EMOT', 'EMOT05', '디지털 기기 사용 도움', '스마트폰·태블릿 사용 안내', '디지털 리터러시, 인내심', NULL, 26);
```

---

### 5. work_types (근무 형태 마스터)

```sql
CREATE TABLE work_types (
  code VARCHAR(30) PRIMARY KEY,
  name VARCHAR(50) NOT NULL,
  description TEXT,
  time_examples TEXT,
  sort_order INT DEFAULT 0
);

-- 샘플 데이터
INSERT INTO work_types (code, name, description, time_examples, sort_order) VALUES
  ('WEEKDAY_MORNING', '평일 오전 위주', '월–금 오전 시간대 고정 근무', '09:00–13:00, 08:00–12:00', 1),
  ('WEEKDAY_AFTERNOON', '평일 오후 위주', '월–금 오후 시간대 고정 근무', '14:00–18:00, 13:00–17:00', 2),
  ('WEEKDAY_EVENING', '평일 저녁/야간', '월–금 저녁~밤 시간대', '19:00–23:00, 20:00–24:00', 3),
  ('WEEKEND_ONLY', '주말 위주', '토·일요일 위주 근무', '토·일 10:00–18:00', 4),
  ('FLEXIBLE', '탄력 근무', '주 단위 또는 월 단위 유동적 편성', '주 3일, 주 4일 등 협의', 5),
  ('FULL_TIME', '상시 근무', '주 5–6일, 장시간', '월–금 또는 월–토', 6);
```

---

## 🗄️ 엔티티 테이블 정의

### 1. users (수급자/이용자)

```sql
CREATE TABLE users (
  id SERIAL PRIMARY KEY,
  
  -- === 기본 정보 ===
  name VARCHAR(100) NOT NULL,
  birth_date DATE,
  age INT GENERATED ALWAYS AS (
    EXTRACT(YEAR FROM AGE(CURRENT_DATE, birth_date))
  ) STORED,
  gender VARCHAR(10) CHECK (gender IN ('male', 'female')),
  phone VARCHAR(20),
  email VARCHAR(255) UNIQUE,
  
  -- === 주소 & 위치 ===
  address TEXT NOT NULL,
  address_detail VARCHAR(200), -- 상세주소 (동/호수)
  postal_code VARCHAR(10),
  latitude DOUBLE PRECISION NOT NULL,
  longitude DOUBLE PRECISION NOT NULL,
  
  -- === 장애 정보 ===
  disability_type_code VARCHAR(20) REFERENCES disability_types(code),
  disability_type_secondary VARCHAR(20) REFERENCES disability_types(code), -- 중복장애
  disability_level VARCHAR(20) CHECK (disability_level IN ('severe', 'mild')), -- 중증/경증
  disability_grade VARCHAR(20) CHECK (disability_grade IN ('1', '2', '3', '4', '5', '6')), -- 1~6급 (레거시)
  care_grade VARCHAR(20) CHECK (care_grade IN ('1', '2', '3', '4', '5')), -- 장기요양등급
  
  -- === 건강 상태 ===
  uses_ventilator BOOLEAN DEFAULT false, -- 인공호흡기 사용
  uses_suction BOOLEAN DEFAULT false, -- 흡인기 사용
  uses_tube_feeding BOOLEAN DEFAULT false, -- 경관영양(튜브 급식)
  mobility_code VARCHAR(20) REFERENCES mobility_types(code), -- 이동 능력
  has_dysphagia BOOLEAN DEFAULT false, -- 연하곤란 (삼킴 장애)
  health_notes TEXT, -- 기타 건강 특이사항
  
  -- === 서비스 정보 ===
  monthly_hours INT DEFAULT 0, -- 월 급여시간
  remaining_hours INT DEFAULT 0, -- 잔여시간
  service_start_date DATE, -- 서비스 시작일
  service_agency VARCHAR(200), -- 담당 기관
  case_manager VARCHAR(100), -- 담당 코디네이터
  
  -- === 선호 조건 ===
  preferred_gender VARCHAR(10) CHECK (preferred_gender IN ('male', 'female', 'any')),
  preferred_age_min INT,
  preferred_age_max INT,
  preferred_job_codes TEXT[], -- ARRAY of job_detail_codes
  skip_assistant_ids INT[], -- 기피 활동지원사 IDs
  
  -- === 비상 연락처 ===
  emergency_contact_name VARCHAR(100),
  emergency_contact_phone VARCHAR(20),
  emergency_contact_relation VARCHAR(50), -- 관계 (보호자, 자녀 등)
  
  -- === 메모 ===
  notes TEXT, -- 관리자 메모
  
  -- === 메타데이터 ===
  is_active BOOLEAN DEFAULT true,
  created_at TIMESTAMPTZ DEFAULT NOW(),
  updated_at TIMESTAMPTZ DEFAULT NOW()
);

-- 인덱스
CREATE INDEX idx_users_name ON users(name);
CREATE INDEX idx_users_disability_type ON users(disability_type_code);
CREATE INDEX idx_users_disability_level ON users(disability_level);
CREATE INDEX idx_users_mobility ON users(mobility_code);
CREATE INDEX idx_users_ventilator ON users(uses_ventilator);
CREATE INDEX idx_users_location ON users(latitude, longitude);
CREATE INDEX idx_users_active ON users(is_active);
```

---

### 2. assistants (활동지원사)

```sql
CREATE TABLE assistants (
  id SERIAL PRIMARY KEY,
  
  -- === 기본 정보 ===
  name VARCHAR(100) NOT NULL,
  birth_date DATE,
  age INT GENERATED ALWAYS AS (
    EXTRACT(YEAR FROM AGE(CURRENT_DATE, birth_date))
  ) STORED,
  gender VARCHAR(10) CHECK (gender IN ('male', 'female')),
  phone VARCHAR(20),
  email VARCHAR(255) UNIQUE,
  
  -- === 주소 & 위치 ===
  address TEXT NOT NULL,
  address_detail VARCHAR(200),
  postal_code VARCHAR(10),
  latitude DOUBLE PRECISION NOT NULL,
  longitude DOUBLE PRECISION NOT NULL,
  
  -- === 자격 정보 ===
  certification_date DATE, -- 자격 취득일
  certification_no VARCHAR(50), -- 자격증 번호
  certification_expiry DATE, -- 자격 만료일 (갱신 필요 시)
  education_level VARCHAR(50), -- 학력
  experience_years INT DEFAULT 0, -- 경력 년수
  
  -- === 차량 정보 ===
  has_vehicle BOOLEAN DEFAULT false,
  vehicle_type VARCHAR(50), -- 승용차, SUV, 밴 등
  can_transport_wheelchair BOOLEAN DEFAULT false, -- 휠체어 탑승 가능
  
  -- === 근무 희망 조건 ===
  work_type_code VARCHAR(30) REFERENCES work_types(code),
  preferred_region TEXT, -- 선호 지역 (예: "서울시 도봉구, 강북구")
  max_weekly_hours INT DEFAULT 40, -- 주당 최대 근무시간
  commute_time_max_minutes INT DEFAULT 60, -- 최대 출퇴근 시간 (분)
  
  -- 선호 시간대 (JSONB)
  -- {"weekday_am": true, "weekday_pm": false, "weekend": true, ...}
  preferred_time_slots JSONB DEFAULT '{}'::JSONB,
  
  available_night_shift BOOLEAN DEFAULT false, -- 야간 근무 가능
  available_weekend BOOLEAN DEFAULT false, -- 주말 근무 가능
  available_holiday BOOLEAN DEFAULT false, -- 공휴일 근무 가능
  
  -- === 제한 사항 ===
  hard_restrictions TEXT, -- 절대 불가 조건 (예: "무거운 이송 불가")
  health_notes TEXT, -- 건강 관련 참고사항 (예: "허리 디스크")
  
  -- === 선호 조건 ===
  preferred_disability_types TEXT[], -- 선호 장애 유형
  preferred_user_gender VARCHAR(10) CHECK (preferred_user_gender IN ('male', 'female', 'any')),
  preferred_age_min INT,
  preferred_age_max INT,
  skip_user_ids INT[], -- 기피 이용자 IDs
  
  -- === 메모 ===
  notes TEXT,
  
  -- === 메타데이터 ===
  is_active BOOLEAN DEFAULT true,
  created_at TIMESTAMPTZ DEFAULT NOW(),
  updated_at TIMESTAMPTZ DEFAULT NOW()
);

-- 인덱스
CREATE INDEX idx_assistants_name ON assistants(name);
CREATE INDEX idx_assistants_gender ON assistants(gender);
CREATE INDEX idx_assistants_has_vehicle ON assistants(has_vehicle);
CREATE INDEX idx_assistants_work_type ON assistants(work_type_code);
CREATE INDEX idx_assistants_location ON assistants(latitude, longitude);
CREATE INDEX idx_assistants_active ON assistants(is_active);
CREATE INDEX idx_assistants_night ON assistants(available_night_shift);
CREATE INDEX idx_assistants_weekend ON assistants(available_weekend);
```

---

### 3. assistant_job_abilities (활동지원사 가능 업무)

```sql
CREATE TABLE assistant_job_abilities (
  id SERIAL PRIMARY KEY,
  assistant_id INT NOT NULL REFERENCES assistants(id) ON DELETE CASCADE,
  job_id INT NOT NULL REFERENCES job_master(id) ON DELETE CASCADE,
  
  -- === 숙련도 ===
  proficiency_level VARCHAR(20) DEFAULT 'BASIC' 
    CHECK (proficiency_level IN ('BASIC', 'INTERMEDIATE', 'ADVANCED')),
  
  -- === 자격/교육 ===
  is_certified BOOLEAN DEFAULT false, -- 관련 자격증 보유
  certification_date DATE,
  training_completed BOOLEAN DEFAULT false, -- 교육 이수
  training_date DATE,
  
  -- === 메모 ===
  notes TEXT,
  
  -- === 메타데이터 ===
  created_at TIMESTAMPTZ DEFAULT NOW(),
  
  -- 중복 방지
  UNIQUE(assistant_id, job_id)
);

-- 인덱스
CREATE INDEX idx_abilities_assistant ON assistant_job_abilities(assistant_id);
CREATE INDEX idx_abilities_job ON assistant_job_abilities(job_id);
CREATE INDEX idx_abilities_proficiency ON assistant_job_abilities(proficiency_level);
```

---

### 4. user_required_jobs (이용자 필요 업무)

```sql
CREATE TABLE user_required_jobs (
  id SERIAL PRIMARY KEY,
  user_id INT NOT NULL REFERENCES users(id) ON DELETE CASCADE,
  job_id INT NOT NULL REFERENCES job_master(id) ON DELETE CASCADE,
  
  -- === 우선순위 ===
  priority INT DEFAULT 0, -- 높을수록 중요
  frequency VARCHAR(20) CHECK (frequency IN ('daily', 'weekly', 'monthly', 'as_needed')),
  
  -- === 특이사항 ===
  notes TEXT,
  
  -- === 메타데이터 ===
  created_at TIMESTAMPTZ DEFAULT NOW(),
  
  -- 중복 방지
  UNIQUE(user_id, job_id)
);

-- 인덱스
CREATE INDEX idx_user_jobs_user ON user_required_jobs(user_id);
CREATE INDEX idx_user_jobs_job ON user_required_jobs(job_id);
```

---

### 5. matches (매칭)

```sql
CREATE TABLE matches (
  id SERIAL PRIMARY KEY,
  
  -- === 관계 ===
  user_id INT NOT NULL REFERENCES users(id) ON DELETE CASCADE,
  assistant_id INT NOT NULL REFERENCES assistants(id) ON DELETE CASCADE,
  
  -- === 매칭 상태 ===
  status VARCHAR(20) NOT NULL DEFAULT 'proposed' 
    CHECK (status IN ('proposed', 'accepted', 'rejected', 'confirmed', 'cancelled', 'completed')),
  
  -- === 매칭 정보 ===
  score INT CHECK (score BETWEEN 0 AND 100), -- AI 매칭 점수
  reason TEXT, -- 매칭 이유
  distance_km DECIMAL(5,2), -- 거리 (km)
  
  -- === 일정 ===
  scheduled_time_slots JSONB, -- {"mon": ["09:00-12:00"], "wed": ["14:00-17:00"]}
  start_date DATE,
  end_date DATE,
  
  -- === 담당 업무 ===
  assigned_job_ids INT[], -- job_master IDs
  
  -- === 메타데이터 ===
  created_by VARCHAR(100), -- 생성한 담당자
  created_at TIMESTAMPTZ DEFAULT NOW(),
  confirmed_at TIMESTAMPTZ,
  cancelled_at TIMESTAMPTZ,
  notes TEXT
);

-- 인덱스
CREATE INDEX idx_matches_user ON matches(user_id);
CREATE INDEX idx_matches_assistant ON matches(assistant_id);
CREATE INDEX idx_matches_status ON matches(status);
```

---

### 6. backups (백업 이력)

```sql
CREATE TABLE backups (
  id SERIAL PRIMARY KEY,
  
  -- === 백업 정보 ===
  backup_type VARCHAR(20) CHECK (backup_type IN ('auto', 'manual', 'before_import')),
  backup_date TIMESTAMPTZ DEFAULT NOW(),
  file_name VARCHAR(255),
  file_size_bytes BIGINT,
  file_path TEXT,
  
  -- === 데이터 요약 ===
  user_count INT,
  assistant_count INT,
  match_count INT,
  
  -- === 상태 ===
  status VARCHAR(20) DEFAULT 'completed' CHECK (status IN ('in_progress', 'completed', 'failed', 'restored')),
  error_message TEXT,
  
  -- === 메타데이터 ===
  created_by VARCHAR(100),
  restored_at TIMESTAMPTZ,
  notes TEXT
);
```

---

### 7. app_settings (앱 설정)

```sql
CREATE TABLE app_settings (
  key VARCHAR(100) PRIMARY KEY,
  value JSONB NOT NULL,
  description TEXT,
  updated_at TIMESTAMPTZ DEFAULT NOW()
);

-- 기본 설정
INSERT INTO app_settings (key, value, description) VALUES
  ('marker_colors', '{"user": "#3B82F6", "assistant": "#F97316"}', '마커 색상'),
  ('radius_colors', '{"fill": "#3B82F680", "stroke": "#3B82F6"}', '반경 원 색상'),
  ('ui_theme', '"dark"', 'UI 테마 (dark/light)'),
  ('ui_font_size', '"medium"', '폰트 크기 (small/medium/large)'),
  ('backup_schedule', '{"enabled": true, "frequency": "daily", "time": "03:00"}', '백업 스케줄'),
  ('default_radius_km', '3', '기본 검색 반경 (km)');
```

---

## 🔍 검색 쿼리 예시

### 1. 이용자 상세 검색

```sql
-- 다중 조건 이용자 검색
SELECT u.*, 
       dt.name as disability_type_name,
       mt.name as mobility_name
FROM users u
LEFT JOIN disability_types dt ON u.disability_type_code = dt.code
LEFT JOIN mobility_types mt ON u.mobility_code = mt.code
WHERE u.is_active = true
  -- 이름 검색
  AND (u.name ILIKE '%김%' OR :name_search IS NULL)
  -- 주소 검색
  AND (u.address ILIKE '%도봉구%' OR :address_search IS NULL)
  -- 연령 범위
  AND (u.age BETWEEN :age_min AND :age_max OR :age_min IS NULL)
  -- 성별
  AND (u.gender = :gender OR :gender IS NULL)
  -- 장애 유형
  AND (u.disability_type_code = :disability_type OR :disability_type IS NULL)
  -- 장애 등급 (최중증/중증/경증)
  AND (u.disability_level = :disability_level OR :disability_level IS NULL)
  -- 인공호흡기 사용
  AND (u.uses_ventilator = :uses_ventilator OR :uses_ventilator IS NULL)
  -- 이동 능력 (휠체어/보행)
  AND (u.mobility_code = :mobility_code OR :mobility_code IS NULL)
ORDER BY u.name;
```

### 2. 활동지원사 상세 검색

```sql
-- 조건에 맞는 활동지원사 검색
SELECT a.*,
       wt.name as work_type_name,
       ARRAY_AGG(jm.name) as job_names
FROM assistants a
LEFT JOIN work_types wt ON a.work_type_code = wt.code
LEFT JOIN assistant_job_abilities aja ON a.id = aja.assistant_id
LEFT JOIN job_master jm ON aja.job_id = jm.id
WHERE a.is_active = true
  -- 이름 검색
  AND (a.name ILIKE '%이%' OR :name_search IS NULL)
  -- 성별
  AND (a.gender = :gender OR :gender IS NULL)
  -- 차량 보유
  AND (a.has_vehicle = :has_vehicle OR :has_vehicle IS NULL)
  -- 근무 형태
  AND (a.work_type_code = :work_type OR :work_type IS NULL)
  -- 야간 가능
  AND (a.available_night_shift = :night_shift OR :night_shift IS NULL)
  -- 주말 가능
  AND (a.available_weekend = :weekend OR :weekend IS NULL)
  -- 특정 업무 가능
  AND (jm.detail_code = ANY(:job_codes) OR :job_codes IS NULL)
GROUP BY a.id, wt.name
ORDER BY a.name;
```

### 3. 반경 내 활동지원사 검색

```sql
-- 이용자 기준 반경 내 활동지원사 검색 (Haversine)
SELECT a.*,
       (6371 * acos(
         cos(radians(:user_lat)) * cos(radians(a.latitude)) *
         cos(radians(a.longitude) - radians(:user_lon)) +
         sin(radians(:user_lat)) * sin(radians(a.latitude))
       )) AS distance_km
FROM assistants a
WHERE a.is_active = true
  AND (6371 * acos(
         cos(radians(:user_lat)) * cos(radians(a.latitude)) *
         cos(radians(a.longitude) - radians(:user_lon)) +
         sin(radians(:user_lat)) * sin(radians(a.latitude))
       )) <= :radius_km
ORDER BY distance_km ASC;
```

---

## 📋 C# 모델 클래스 (참고)

### User.cs

```csharp
public record User(
    int Id,
    string Name,
    DateTime? BirthDate,
    int? Age,
    string? Gender,
    string? Phone,
    string? Email,
    string Address,
    double Latitude,
    double Longitude,
    
    // 장애 정보
    string? DisabilityTypeCode,
    string? DisabilityLevel, // severe, mild
    string? MobilityCode,
    bool UsesVentilator,
    bool UsesSuction,
    bool UsesTubeFeeding,
    
    // 서비스 정보
    int MonthlyHours,
    int RemainingHours,
    
    // 선호 조건
    string? PreferredGender,
    int? PreferredAgeMin,
    int? PreferredAgeMax,
    
    // 비상 연락처
    string? EmergencyContactName,
    string? EmergencyContactPhone,
    
    // 메타
    bool IsActive,
    DateTime CreatedAt,
    DateTime UpdatedAt
);
```

### Assistant.cs

```csharp
public record Assistant(
    int Id,
    string Name,
    DateTime? BirthDate,
    int? Age,
    string? Gender,
    string? Phone,
    string? Email,
    string Address,
    double Latitude,
    double Longitude,
    
    // 자격 정보
    DateTime? CertificationDate,
    string? CertificationNo,
    int ExperienceYears,
    
    // 차량 정보
    bool HasVehicle,
    string? VehicleType,
    bool CanTransportWheelchair,
    
    // 근무 조건
    string? WorkTypeCode,
    string? PreferredRegion,
    int MaxWeeklyHours,
    int CommuteTimeMaxMinutes,
    bool AvailableNightShift,
    bool AvailableWeekend,
    
    // 제한 사항
    string? HardRestrictions,
    string? HealthNotes,
    
    // 메타
    bool IsActive,
    DateTime CreatedAt,
    DateTime UpdatedAt
);
```

---

## 🔗 참고 문서

- [disability_support_worker_guide.md](./disability_support_worker_guide.md) - 직무 분류 체계 상세
- [Supabase 연동 가이드](./Supabase_연동_가이드.md) - 연동 방법
- [MAUI 통합 로드맵](./MAUI_통합_로드맵.md) - 개발 일정

---

**문서 버전**: 2.0  
**마지막 업데이트**: 2025-12-11  
**변경 내역**:
- v1.0 (2025-12-10): 초기 버전
- v2.0 (2025-12-11): 상세 필드 확장 (장애유형, 이동능력, 직무분류 등)
