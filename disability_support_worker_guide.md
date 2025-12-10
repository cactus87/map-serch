# 장애인 활동지원사 직무 시스템 가이드

## 개요

본 문서는 장애인 활동지원사의 직무 체계를 데이터베이스 설계 기준에 맞게 분류·정의한 종합 가이드입니다. 직무 마스터, 개인별 희망 근무 형태, 세부업무 분류, 그리고 데이터베이스 스키마를 포함합니다.

### 문서 대상
- 장애인 활동지원 플랫폼 개발자
- 활동지원사–수급자 매칭 시스템 담당자
- 복지관·센터 관리 담당자
- 데이터베이스 설계자

### 주요 내용
1. 직무 분류 및 카테고리 정의
2. 직무 마스터 데이터 구성
3. 개인별 희망 근무 형태 및 가능 업무 설정
4. 세부업무 분류 및 예시
5. 데이터베이스 스키마 설계 모델

---

## 1. 직무 분류 체계

### 대분류 (Job Category)

장애인 활동지원 서비스는 법령과 실무 기준에 따라 4가지 주요 직무군으로 분류됩니다.

| 대분류 코드 | 직무 구분 | 설명 | 지원 대상 |
|-----------|---------|------|----------|
| PHYS | 신체활동지원 | 수급자의 신체 기능 유지, 위생, 안전을 직접적으로 지원하는 업무 | 모든 수급자 |
| HOUSE | 가사활동지원 | 수급자 가정의 일상 가사 중 생활과 직접 관련된 범위를 지원 | 일상생활 자립도가 낮은 수급자 |
| SOCI | 사회활동·이동지원 | 통학, 출근, 병원 방문, 지역사회 참여 등을 위한 이동 및 현장 지원 | 활동성 있는 수급자 |
| EMOT | 정서·의사소통지원 | 정서적 안정과 사회적 관계 유지, 정보 접근을 돕는 업무 | 소통이 필요한 모든 수급자 |

### 분류 설계 원칙

- **의료행위 제외**: 약물 투여, 의료기기 조작 등은 포함하지 않음
- **생활보조 중심**: 활동지원사 자격 범위 내 생활 기능 보조
- **유연한 조합**: 개인 수급자 상황에 따라 여러 직무 병행 가능
- **법적 기준 준수**: 「장애인활동지원에관한법률」 및 실행 지침 반영

---

## 2. 세부직무 분류

### 2.1 신체활동지원 (PHYS)

수급자의 신체 기능 유지 및 위생, 안전을 직접 돕는 업무입니다.

#### PHYS01: 기상·취침 보조
- **정의**: 기상 시 침대에서 일으켜 세우기, 취침 시 누워 안전하게 누이기
- **수행 내용**:
  - 기상 준비: 알람, 기상 유도, 체온·맥박 확인(기본)
  - 취침 준비: 야복 입히기, 침구 정리, 안전한 자세 맞추기
  - 자세 확인: 욕창 유발 자세 방지, 편안한 위치 유지
- **수행 시 주의**: 무리한 힘 사용 금지, 안전한 역학 활용

#### PHYS02: 체위변경·자세 유지
- **정의**: 욕창 예방 및 편의를 위한 신체 자세 변경 및 유지
- **수행 내용**:
  - 정기적 자세 변경(침대, 휠체어, 의자)
  - 베개·쿠션 조정으로 편안한 자세 유지
  - 팔·다리 위치 조정, 무릎 밑 받침 확인
- **수행 시 주의**: 신경·혈관 압박 유발 자세 방지

#### PHYS03: 실내이동·이송 보조
- **정의**: 침대↔휠체어, 휠체어↔의자, 실내 보행 시 신체적 지원
- **수행 내용**:
  - 침대에서 휠체어로 안전 이동(트래전서 가능 범위 내)
  - 휠체어 조작 및 이동 경로 안내
  - 보행 시 부축(양팔, 한팔 등 정도에 따라)
  - 계단·턱 통과 시 안전 지원
- **수행 시 주의**: 과도한 체중 이동(번딩) 불가, 허리 부상 방지

#### PHYS04: 세면·구강위생
- **정의**: 얼굴, 손 씻기, 양치질, 틀니 관리 보조
- **수행 내용**:
  - 세면 물 준비, 타올 제공
  - 얼굴·손 씻기 도움 (부분 또는 전체)
  - 양치질 준비: 칫솔, 치약, 물 제공
  - 틀니 세척 및 관리 보조
  - 면도(면도기 취급 안전성 확보)
- **수행 시 주의**: 눈·코에 물 흘러 들어가지 않도록 주의

#### PHYS05: 목욕 보조
- **정의**: 샤워·입욕 시 탈의, 신체 씻기, 착의, 욕실 안전 지원
- **수행 내용**:
  - 입욕 전 준비: 수온 확인(40–42℃), 미끄럼 방지 매트 확인
  - 탈의 보조 및 옷 보관
  - 신체 일부(등, 하지) 씻기 또는 전체 보조
  - 욕실 내 이동·의자 앉기 보조
  - 착의 및 드라이 보조
- **수행 시 주의**: 물에 빠지지 않도록 안전 확보, 스팀 화상 방지

#### PHYS06: 옷 갈아입히기
- **정의**: 상의, 하의, 속옷, 계절·상황에 맞는 옷 선택 및 입기 보조
- **수행 내용**:
  - 옷장에서 적절한 옷 꺼내기 (상의, 하의, 속옷, 양말)
  - 벗기 순서 안내 및 도움
  - 입기 순서 안내 및 도움
  - 지퍼, 단추, 끈 정리
  - 계절·실내외 온도에 맞는 의복 제안
- **수행 시 주의**: 피부 손상 방지, 존엄성 존중

#### PHYS07: 배뇨·배변 보조
- **정의**: 화장실 이용, 배변기구 사용, 기저귀·패드 교체 보조
- **수행 내용**:
  - 화장실 이동 및 변기 앉기·일어서기 보조
  - 이동식 변기(포티) 설치·사용 보조
  - 화장지 사용, 물 내리기 도움
  - 기저귀·패드 교체 (영역 청결 유지)
  - 하의 입고 벗기
  - 냄새 제거 및 환기
- **수행 시 주의**: 감염 방지(손 소독), 존엄성 존중, 피부 건강 관찰

#### PHYS08: 식사 보조
- **정의**: 조리된 음식 차리기, 식사 시 자세 보조, 음식 섭취 도움
- **수행 내용**:
  - 식사 상 준비 및 음식 담기
  - 식사 전 손 씻기, 식후 입가·손 정리
  - 식사 시 안전한 자세 유지
  - 숟가락·포크 이용 어려운 경우 떠먹여주기
  - 음료 제공 및 음욕 삼킴 확인
  - 식사 후 설거지 준비 (조리는 가사활동 범위)
- **수행 시 주의**: 질식 위험 방지(연하곤란 있는 경우 특별 주의), 영양 상태 관찰

#### PHYS09: 투약 보조(비의료)
- **정의**: 보호자가 준비한 약 복용 시간 알림 및 전달, 기본 지원
- **수행 내용**:
  - 약 복용 시간 알림
  - 준비된 약 전달
  - 물 제공
  - 복용 여부 확인 및 기록
- **수행 시 주의**: 약물 투여·조제는 불가, 의료인 또는 보호자가 준비한 약만 전달

---

### 2.2 가사활동지원 (HOUSE)

수급자 가정의 일상 가사 중 수급자의 생활과 직접 관련된 범위를 지원합니다.

#### HOUSE01: 주거공간 청소
- **정의**: 거주 공간(방, 거실, 욕실, 주방) 청소 및 정돈
- **수행 내용**:
  - 먼지 제거: 바닥 청소, 먼지떨이질, 청소기 사용
  - 욕실 청소: 변기, 세면대, 욕조 청소
  - 주방 청소: 싱크대, 가스레인지(안전한 범위) 청소
  - 쓰레기 수거 및 분리배출
  - 환기 및 곰팡이 관리(기본 수준)
- **수행 시 주의**: 화학 약품 안전 사용, 수급자 건강 상태 고려

#### HOUSE02: 침구·생활공간 정리정돈
- **정의**: 침대, 이불, 옷장, 수납공간 정리 정돈
- **수행 내용**:
  - 침대 정리: 시트 펴기, 이불 정리
  - 옷장 정리: 계절용 의류 정리, 너덜거린 옷 정렬
  - 책상·선반 정리: 필요 물건 구분, 버릴 물건 분류
  - 소품·물건 정리: 멀티탭, 리모콘, 휴대전화 위치 표준화
  - 방충·환기: 기본적 해충 관리, 창 열기
- **수행 시 주의**: 중요 물건 함부로 버리지 않기, 수급자 물건 존중

#### HOUSE03: 세탁
- **정의**: 의류, 수건, 침구 등 세탁 및 건조, 개기
- **수행 내용**:
  - 세탁물 분류(색, 소재, 오염도)
  - 세제·섬유유연제 사용 및 세탁기 조작
  - 손세탁 필요한 옷 관리
  - 건조기 또는 자연 건조
  - 세탁물 개기
  - 다리미(가능 범위) 또는 정리
- **수행 시 주의**: 옷감 손상 방지, 색 빠짐 주의, 안전한 온도 관리

#### HOUSE04: 취사(조리 및 식사 준비)
- **정의**: 밥 짓기, 반찬 조리, 식사 상 차리기
- **수행 내용**:
  - 밥 짓기: 쌀 씻기, 전자밥솥·냄비 사용
  - 반찬 조리: 국, 찌개, 볶음(단순 조리 수준)
  - 식재 준비: 채소 손질, 고기 준비
  - 식사 상 차리기: 밥, 국, 반찬 배치
  - 음식 온도 확인(수급자 연하곤란 등 고려)
  - 음식 보관(냉장·냉동 관리)
- **수행 시 주의**: 식중독·화상 예방, 영양가 있는 식단 구성

#### HOUSE05: 설거지
- **정의**: 식사 후 식기 세척, 주방 정리
- **수행 내용**:
  - 식기 헹굼 및 세제로 닦기
  - 냄비·팬 정리
  - 싱크대 물기 제거 및 정리
  - 음식물 쓰레기 처리
  - 주방 세제 관리
- **수행 시 주의**: 깨진 식기 관리, 물에 빠진 물건 회수

#### HOUSE06: 생필품 정리 및 재고 관리
- **정의**: 식료품, 의약품, 세제 등 생필품 정리 및 기본 재고 관리
- **수행 내용**:
  - 식료품 분류(냉장, 냉동, 상온)
  - 유통기한 확인 및 만료 식품 폐기
  - 의약품 보관 위치 표준화
  - 세제, 휴지 등 공급품 정렬
  - 간단한 쇼핑 리스트 작성 보조
- **수행 시 주의**: 유통기한 놓치지 않기, 식재료 보관 온도 관리

---

### 2.3 사회활동·이동지원 (SOCI)

통학, 출근, 병원 방문, 지역사회 참여 등을 위한 이동 및 현장 지원입니다.

#### SOCI01: 출퇴근·등하교 동행
- **정의**: 직장·학교까지 이동, 교통수단 이용, 현장 내 기본 보조
- **수행 내용**:
  - 외출 준비 도움 (옷, 신발, 가방)
  - 이동 경로 안내 (도보, 대중교통)
  - 대중교통 탑승·하차 보조 (계단, 휠체어 고정)
  - 교통카드 사용 보조 또는 현금 결제 지원
  - 현장 도착 후 안전한 위치 확보
  - 업무 중 화장실·식사 동행
  - 직장 내 기본 의사소통 보조
- **수행 시 주의**: 교통안전(신호, 차량 주의), 낙상 방지, 시간 엄수

#### SOCI02: 병원 동행
- **정의**: 병원 방문, 진료실 출입, 검사 절차 현장 동행
- **수행 내용**:
  - 병원 이동 및 주차 (자가용 또는 대중교통)
  - 접수·대기 도움
  - 진료실 출입 보조 (휠체어, 이동 안내)
  - 검사실·촬영실 동행 (의료 절차 제외)
  - 진료 결과 설명 청취 지원
  - 처방약 수령 보조
  - 복귀 이동
- **수행 시 주의**: 의료 절차 불개입, 환자 프라이버시 존중, 의료진과 원활한 소통

#### SOCI03: 관공서·은행 등 공공기관 동행
- **정의**: 주민센터, 은행, 우체국, 동사무소 등 공공기관 방문 시 동행
- **수행 내용**:
  - 번호표 수령 및 대기 안내
  - 창구·담당자에게 요청사항 전달 보조
  - 서류 준비 지원
  - 결제·수령 보조
  - 방문 목적 달성 확인
- **수행 시 주의**: 중요 서류 분실 방지, 보호자 권리 존중

#### SOCI04: 장보기·시장·마트 동행
- **정의**: 마트·시장·편의점 방문 시 쇼핑 동행 및 지원
- **수행 내용**:
  - 쇼핑 목록 확인 및 경로 안내
  - 장바구니·카트 밀기
  - 물건 위치 찾기 보조
  - 무거운 물건 들기 (중량 제한 있을 시 제한)
  - 계산 대기 및 결제 지원
  - 물건 정렬 및 봉투 정리
- **수행 시 주의**: 낙상 방지, 물건 손상 주의, 예산 범위 내 구매 지원

#### SOCI05: 여가·문화·종교활동 동행
- **정의**: 복지관 프로그램, 공연·전시, 종교시설, 동호회 등 여가·사회활동 동행
- **수행 내용**:
  - 행사장 또는 시설 이동 (대중교통, 자가용)
  - 좌석·신발장·짐 정리 보조
  - 프로그램·공연 관람 현장 안내
  - 휴식·화장실 필요 시 동행
  - 동료·친구와의 사회적 상호작용 지원
- **수행 시 주의**: 수급자의 사회적 자율성 존중, 개인 취향 고려

#### SOCI06: 산책·지역사회 참여
- **정의**: 동네 산책, 공원 이용, 주민 모임 등 일상적 지역사회 참여 동행
- **수행 내용**:
  - 산책 경로 안내 (보행 능력 고려)
  - 날씨·계절에 맞는 복장 지원
  - 벤치 앉기, 휠체어 관리
  - 동네 주민과의 자연스러운 상호작용 촉진
  - 지역 소식·행사 안내
- **수행 시 주의**: 낙상 방지, 과로 주의, 사회적 자립감 향상 격려

---

### 2.4 정서·의사소통지원 (EMOT)

정서적 안정과 사회적 관계 유지, 정보 접근을 돕는 업무입니다.

#### EMOT01: 말벗·정서적 지지
- **정의**: 일상 대화, 공감적인 경청, 외로움·불안감 경감 지원
- **수행 내용**:
  - 일상 대화 (오늘의 기분, 가족 소식, 주변 사건)
  - 관심사 공유 (취미, 책, 뉴스)
  - 공감적 경청 (수급자 감정 이해 및 지지)
  - 스트레스 완화 대화
  - 외로움·고독감 경감
  - 긍정적 강화 (격려, 칭찬)
- **수행 시 주의**: 전문 상담이 아님, 수급자의 감정 판단 금지, 비밀 보장

#### EMOT02: 일정·생활계획 함께 세우기
- **정의**: 병원 예약, 외출 일정, 하루 생활루틴 계획 상의
- **수행 내용**:
  - 병원 예약 일정 확인 및 미리 알림
  - 외출 계획 확인 (목적, 시간, 준비물)
  - 주간/월간 일정 정리 (캘린더 표시)
  - 아침·저녁 루틴 구조화 지원
  - 약속 시간 알림
  - 생활 목표 설정 지원 (간단한 수준)
- **수행 시 주의**: 강압적이지 않은 지원, 수급자 선택권 존중

#### EMOT03: 의사소통 보조
- **정의**: 전화, 문서 설명, 정보 전달 등 의사소통 어려움 보조
- **수행 내용**:
  - 전화 통화 보조 (통화 연결, 전화번호 입력, 스피커폰)
  - 가족·친구·담당자와의 의사소통 중재
  - 중요 안내문·안내장 설명 (의료기관, 행정기관)
  - 기본적인 정보 전달 (날씨, 뉴스)
  - 비상상황 알림 및 도움 요청 지원
- **수행 시 주의**: 사적인 대화 존중, 프라이버시 보호, 정확한 정보 전달

#### EMOT04: 대독·대필
- **정의**: 안내문·문서 읽어주기, 간단한 메모·서류 작성 도움
- **수행 내용**:
  - 안내문·편지 읽어주기
  - 서류 내용 설명 (진단서, 처방전, 복지 안내)
  - 메모·일기·가계부 작성 구술 받아 적기
  - 간단한 편지·문자 작성 보조
  - 서명·날인 장소 안내
- **수행 시 주의**: 문서 내용 왜곡 금지, 법적 서류는 신중히 취급

#### EMOT05: 디지털 기기 사용 도움
- **정의**: 휴대전화, 스마트폰, 태블릿, 간단한 앱 사용 안내
- **수행 내용**:
  - 휴대전화 사용법 안내 (통화, 문자, 음성통화)
  - 카톡·라인 등 메신저 사용 보조
  - 사진 촬영 및 갤러리 관리 도움
  - 뉴스 앱, 날씨 앱 등 유용한 앱 안내
  - 검색 방법 간단히 설명
  - 사기 주의 및 보안 기본 교육
- **수행 시 주의**: 과정 반복 설명, 서두르지 않기, 기기 손상 방지

---

## 3. 직무 마스터 데이터

### 3.1 마스터 테이블 구조

```
JobMaster {
  job_id: INT (Primary Key)
  job_category_code: VARCHAR(10) (Foreign Key → JobCategory)
  job_detail_code: VARCHAR(20) (Unique)
  job_name: VARCHAR(50)
  job_description: TEXT
  required_skills: VARCHAR(200)
  health_restrictions: VARCHAR(200) (예: "무거운 체위변경 불가")
  is_active: BOOLEAN
  created_at: TIMESTAMP
  updated_at: TIMESTAMP
}
```

### 3.2 마스터 데이터 샘플

| job_id | job_category_code | job_detail_code | job_name | job_description | required_skills | health_restrictions | is_active |
|--------|-------------------|-----------------|----------|-----------------|-----------------|---------------------|-----------|
| 1 | PHYS | PHYS01 | 기상·취침 보조 | 침대에서 기상, 취침 시 안전한 자세 | 신체활동 기초 | 없음 | true |
| 2 | PHYS | PHYS02 | 체위변경·자세 유지 | 욕창 예방 자세 변경 | 신체활동, 해부학 기초 | 심한 허리질환 주의 | true |
| 3 | PHYS | PHYS03 | 실내이동·이송 보조 | 침대-휠체어 이동, 실내 보행 | 신체활동, 역학 이해 | 심한 허리질환, 고혈압 주의 | true |
| 4 | PHYS | PHYS04 | 세면·구강위생 | 세수, 양치, 틀니 관리 | 위생 관리, 세심함 | 없음 | true |
| 5 | PHYS | PHYS05 | 목욕 보조 | 샤워·입욕 시 안전 지원 | 신체활동, 수온 관리, 안전의식 | 허리 약한 경우 주의 | true |
| 6 | PHYS | PHYS06 | 옷 갈아입히기 | 계절·상황에 맞는 의복 입기 | 상황 판단, 존엄성 존중 | 없음 | true |
| 7 | PHYS | PHYS07 | 배뇨·배변 보조 | 화장실 이용, 기저귀 교체 | 위생 관리, 감염 예방 | 없음 | true |
| 8 | PHYS | PHYS08 | 식사 보조 | 식사 준비, 섭취 도움 | 영양 기초, 안전의식 | 연하곤란 있는 경우 특별 교육 필요 | true |
| 9 | PHYS | PHYS09 | 투약 보조(비의료) | 약 복용 시간 알림, 전달 | 약물 관리 기초 | 없음 | true |
| 10 | HOUSE | HOUSE01 | 주거공간 청소 | 방·거실·욕실·주방 청소 | 청소 기술, 화학 약품 안전 | 폐질환·알레르기 주의 | true |
| 11 | HOUSE | HOUSE02 | 침구·생활공간 정리정돈 | 침대, 옷장, 물건 정리 | 정리정돈, 분류 능력 | 없음 | true |
| 12 | HOUSE | HOUSE03 | 세탁 | 의류, 침구 세탁 및 건조 | 세탁 기술, 섬유 관리 | 알레르기 주의 | true |
| 13 | HOUSE | HOUSE04 | 취사(조리) | 밥, 반찬 조리, 식사 준비 | 조리 기술, 위생, 영양 | 알레르기 확인 필수 | true |
| 14 | HOUSE | HOUSE05 | 설거지 | 식기 세척, 주방 정리 | 기본 위생, 꼼꼼함 | 없음 | true |
| 15 | HOUSE | HOUSE06 | 생필품 정리 및 재고관리 | 식료품, 의약품 정리 | 정리, 재고 관리 | 없음 | true |
| 16 | SOCI | SOCI01 | 출퇴근·등하교 동행 | 직장·학교 이동, 현장 기본 보조 | 이동 지원, 시간 관리 | 없음 | true |
| 17 | SOCI | SOCI02 | 병원 동행 | 진료 시설 방문, 진료 현장 동행 | 의료 기관 이해, 침착성 | 없음 | true |
| 18 | SOCI | SOCI03 | 관공서·은행 동행 | 공공기관 방문, 서류 처리 동행 | 서류 이해, 절차 파악 | 없음 | true |
| 19 | SOCI | SOCI04 | 장보기·마트 동행 | 쇼핑 동행, 물품 구매 지원 | 자금 관리, 안전의식 | 없음 | true |
| 20 | SOCI | SOCI05 | 여가·문화·종교활동 동행 | 복지관·공연·종교시설 이동 | 사회활동 이해, 배려 | 없음 | true |
| 21 | SOCI | SOCI06 | 산책·지역사회 참여 | 동네 산책, 지역 활동 동행 | 지역 안내, 대화 능력 | 없음 | true |
| 22 | EMOT | EMOT01 | 말벗·정서적 지지 | 일상 대화, 공감적 경청 | 소통 능력, 공감 능력 | 없음 | true |
| 23 | EMOT | EMOT02 | 일정·생활계획 함께 세우기 | 일정 관리, 생활 계획 상의 | 조직 능력, 계획 능력 | 없음 | true |
| 24 | EMOT | EMOT03 | 의사소통 보조 | 전화·면접·설명 보조 | 소통 능력, 번역(필요시) | 없음 | true |
| 25 | EMOT | EMOT04 | 대독·대필 | 문서 읽어주기, 기록 작성 | 읽기/쓰기 능력, 세심함 | 없음 | true |
| 26 | EMOT | EMOT05 | 디지털 기기 사용 도움 | 스마트폰·태블릿 사용 안내 | 디지털 리터러시, 인내심 | 없음 | true |

---

## 4. 개인별 희망 근무 형태 및 가능 업무 설정

### 4.1 희망 근무 형태 마스터

```
WorkType {
  work_type_code: VARCHAR(20) (Primary Key)
  work_type_name: VARCHAR(50)
  description: TEXT
  examples: VARCHAR(200)
}
```

| work_type_code | work_type_name | description | examples |
|----------------|----------------|-------------|----------|
| WEEKDAY_MORNING | 평일 오전 위주 | 월–금 오전 시간대 고정 근무 | 09:00–13:00, 08:00–12:00 |
| WEEKDAY_AFTERNOON | 평일 오후 위주 | 월–금 오후 시간대 고정 근무 | 14:00–18:00, 13:00–17:00 |
| WEEKDAY_EVENING | 평일 저녁/야간 | 월–금 저녁~밤 시간대 | 19:00–23:00, 20:00–24:00 |
| WEEKEND_ONLY | 주말 위주 | 토·일요일 위주 근무 | 토·일 10:00–18:00 |
| FLEXIBLE | 탄력 근무 | 주 단위 또는 월 단위 유동적 편성 | 주 3일, 주 4일 등 협의 |
| FULL_TIME | 상시 근무 | 주 5–6일, 장시간 | 월–금 또는 월–토 |

### 4.2 개인별 희망 근무 및 가능 업무 테이블

```
WorkerPreference {
  preference_id: INT (Primary Key)
  worker_id: INT (Foreign Key → Worker)
  preferred_work_type_code: VARCHAR(20) (Foreign Key → WorkType)
  preferred_time_slot: VARCHAR(100)
  preferred_region: VARCHAR(100)
  max_weekly_hours: INT
  is_available_night_shift: BOOLEAN
  is_available_weekend: BOOLEAN
  hard_restrictions: TEXT
  health_notes: TEXT
  created_at: TIMESTAMP
  updated_at: TIMESTAMP
}

WorkerJobAbility {
  ability_id: INT (Primary Key)
  worker_id: INT (Foreign Key → Worker)
  job_id: INT (Foreign Key → JobMaster)
  proficiency_level: ENUM('BASIC', 'INTERMEDIATE', 'ADVANCED')
  is_certified: BOOLEAN
  certification_date: DATE
  notes: TEXT
  created_at: TIMESTAMP
}
```

### 4.3 개인별 희망 근무·가능 업무 레코드 예시

#### 예시 1: 신체활동 중심, 평일 오전 근무자

```
worker_id: 1001
name: 김활동 활동지원사
```

**근무 희망 조건** (WorkerPreference)
- preferred_work_type_code: WEEKDAY_MORNING
- preferred_time_slot: 09:00–13:00
- preferred_region: 서울시 강남구 전체
- max_weekly_hours: 30시간
- is_available_night_shift: false
- is_available_weekend: false
- hard_restrictions: "완전 무게지지가 필요한 이송(번딩)은 불가, 목욕은 가벼운 보조만 가능"
- health_notes: "허리 디스크 병력, 무거운 신체활동 제한"

**가능 업무** (WorkerJobAbility)
| job_id | job_name | proficiency_level | is_certified | notes |
|--------|----------|-------------------|--------------|-------|
| 1 | 기상·취침 보조 | ADVANCED | true | 자격증 취득 (2023년) |
| 2 | 체위변경·자세 유지 | INTERMEDIATE | true | 교육 이수 |
| 4 | 세면·구강위생 | ADVANCED | true | 자격증 취득 |
| 6 | 옷 갈아입히기 | ADVANCED | false | 경험으로 숙련 |
| 7 | 배뇨·배변 보조 | ADVANCED | false | 경험 다수 |
| 8 | 식사 보조 | INTERMEDIATE | false | 기초 수준 |
| 10 | 주거공간 청소 | INTERMEDIATE | false | 기초 수준 |
| 11 | 침구·생활공간 정리정돈 | BASIC | false | 지원 의사 있음 |

#### 예시 2: 이동·외출 중심, 주말·저녁 근무자

```
worker_id: 1002
name: 박이동 활동지원사
```

**근무 희망 조건** (WorkerPreference)
- preferred_work_type_code: FLEXIBLE
- preferred_time_slot: 평일 19:00–23:00, 토·일 10:00–18:00
- preferred_region: 경기도 수원시, 성남시 인근
- max_weekly_hours: 20시간
- is_available_night_shift: true
- is_available_weekend: true
- hard_restrictions: "가사활동(취사·청소·세탁)은 제외, 외출·이동지원 및 신체 보조 위주 희망"
- health_notes: "알레르기 많음(먼지, 세제), 요리는 회피 권장"

**가능 업무** (WorkerJobAbility)
| job_id | job_name | proficiency_level | is_certified | notes |
|--------|----------|-------------------|--------------|-------|
| 3 | 실내이동·이송 보조 | ADVANCED | true | 자격증 취득 (2022년) |
| 5 | 목욕 보조 | INTERMEDIATE | true | 교육 이수 |
| 16 | 출퇴근·등하교 동행 | ADVANCED | false | 3년 경험 |
| 17 | 병원 동행 | ADVANCED | false | 장애 유형 이해 깊음 |
| 18 | 관공서·은행 동행 | INTERMEDIATE | false | 기초 수준 |
| 19 | 장보기·마트 동행 | ADVANCED | false | 경험 많음 |
| 20 | 여가·문화·종교활동 동행 | INTERMEDIATE | false | 관심 있음 |
| 21 | 산책·지역사회 참여 | ADVANCED | false | 경험 많음 |
| 22 | 말벗·정서적 지지 | ADVANCED | false | 소통 능력 우수 |

#### 예시 3: 가사·정서 중심, 탄력 근무자

```
worker_id: 1003
name: 이가사 활동지원사
```

**근무 희망 조건** (WorkerPreference)
- preferred_work_type_code: FLEXIBLE
- preferred_time_slot: 협의 가능 (주 3–4일, 일과 이내)
- preferred_region: 서울시 은평구, 마포구
- max_weekly_hours: 18시간
- is_available_night_shift: false
- is_available_weekend: true
- hard_restrictions: "무거운 신체활동 불가 (건강 사유), 가사활동 및 정서지원 위주"
- health_notes: "관절염, 무거운 것 들기 제한"

**가능 업무** (WorkerJobAbility)
| job_id | job_name | proficiency_level | is_certified | notes |
|--------|----------|-------------------|--------------|-------|
| 4 | 세면·구강위생 | INTERMEDIATE | false | 기초 수준 |
| 6 | 옷 갈아입히기 | INTERMEDIATE | false | 기초 수준 |
| 10 | 주거공간 청소 | ADVANCED | false | 20년 가정주부 경험 |
| 11 | 침구·생활공간 정리정돈 | ADVANCED | false | 경험 풍부 |
| 12 | 세탁 | ADVANCED | false | 경험 풍부 |
| 13 | 취사(조리) | ADVANCED | true | 조리사 자격증 |
| 14 | 설거지 | ADVANCED | false | 경험 풍부 |
| 15 | 생필품 정리 및 재고관리 | INTERMEDIATE | false | 기초 수준 |
| 22 | 말벗·정서적 지지 | ADVANCED | false | 따뜻한 성격 |
| 23 | 일정·생활계획 함께 세우기 | INTERMEDIATE | false | 조직 능력 있음 |
| 25 | 대독·대필 | INTERMEDIATE | false | 기초 수준 |

---

## 5. 데이터베이스 스키마 설계

### 5.1 전체 ERD (Entity-Relationship Diagram) 개념도

```
Worker (활동지원사)
  ├─ WorkerPreference (근무 희망 조건) ──┐
  └─ WorkerJobAbility (가능 업무) ───────┼─→ JobMaster (직무 마스터)
                                          │         ├─ JobCategory
                                          │         └─ job_detail_code

Client (수급자)
  ├─ ClientPreference (선호 업무) ───────┼─→ JobMaster
  └─ ClientSchedule (근무 일정)

Assignment (활동지원사-수급자 매칭)
  ├─ worker_id (FK → Worker)
  ├─ client_id (FK → Client)
  ├─ assigned_jobs (FK → JobMaster)
  └─ assigned_schedule (FK → ClientSchedule)

WorkType (근무 형태 마스터)
```

### 5.2 핵심 테이블 정의

#### 1. JobCategory (직무 대분류 마스터)

```sql
CREATE TABLE JobCategory (
  job_category_code VARCHAR(10) PRIMARY KEY,
  job_category_name VARCHAR(50) NOT NULL,
  description TEXT,
  is_active BOOLEAN DEFAULT true,
  created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP
);

-- Sample Data
INSERT INTO JobCategory VALUES
  ('PHYS', '신체활동지원', '신체 기능 유지, 위생, 안전 지원', true, NOW(), NOW()),
  ('HOUSE', '가사활동지원', '가정 내 일상 가사 지원', true, NOW(), NOW()),
  ('SOCI', '사회활동·이동지원', '외부활동 이동 및 현장 지원', true, NOW(), NOW()),
  ('EMOT', '정서·의사소통지원', '정서 지지 및 의사소통 보조', true, NOW(), NOW());
```

#### 2. JobMaster (직무 마스터)

```sql
CREATE TABLE JobMaster (
  job_id INT PRIMARY KEY AUTO_INCREMENT,
  job_category_code VARCHAR(10) NOT NULL,
  job_detail_code VARCHAR(20) UNIQUE NOT NULL,
  job_name VARCHAR(50) NOT NULL,
  job_description TEXT,
  required_skills VARCHAR(200),
  health_restrictions VARCHAR(200),
  is_active BOOLEAN DEFAULT true,
  created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  FOREIGN KEY (job_category_code) REFERENCES JobCategory(job_category_code)
);

-- Index for query performance
CREATE INDEX idx_job_category ON JobMaster(job_category_code);
CREATE INDEX idx_job_active ON JobMaster(is_active);
```

#### 3. WorkType (근무 형태 마스터)

```sql
CREATE TABLE WorkType (
  work_type_code VARCHAR(20) PRIMARY KEY,
  work_type_name VARCHAR(50) NOT NULL,
  description TEXT,
  examples VARCHAR(200)
);

-- Sample Data
INSERT INTO WorkType VALUES
  ('WEEKDAY_MORNING', '평일 오전 위주', '월–금 오전 시간대 고정 근무', '09:00–13:00, 08:00–12:00'),
  ('WEEKDAY_AFTERNOON', '평일 오후 위주', '월–금 오후 시간대 고정 근무', '14:00–18:00, 13:00–17:00'),
  ('WEEKDAY_EVENING', '평일 저녁/야간', '월–금 저녁~밤 시간대', '19:00–23:00, 20:00–24:00'),
  ('WEEKEND_ONLY', '주말 위주', '토·일요일 위주 근무', '토·일 10:00–18:00'),
  ('FLEXIBLE', '탄력 근무', '주 단위 또는 월 단위 유동적 편성', '주 3일, 주 4일 등 협의'),
  ('FULL_TIME', '상시 근무', '주 5–6일, 장시간', '월–금 또는 월–토');
```

#### 4. Worker (활동지원사)

```sql
CREATE TABLE Worker (
  worker_id INT PRIMARY KEY AUTO_INCREMENT,
  name VARCHAR(50) NOT NULL,
  phone VARCHAR(20),
  email VARCHAR(100),
  certification_date DATE,
  education_level VARCHAR(50),
  experience_years INT DEFAULT 0,
  is_active BOOLEAN DEFAULT true,
  created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP
);
```

#### 5. WorkerPreference (활동지원사 근무 희망 조건)

```sql
CREATE TABLE WorkerPreference (
  preference_id INT PRIMARY KEY AUTO_INCREMENT,
  worker_id INT NOT NULL,
  preferred_work_type_code VARCHAR(20) NOT NULL,
  preferred_time_slot VARCHAR(100),
  preferred_region VARCHAR(100),
  max_weekly_hours INT DEFAULT 40,
  is_available_night_shift BOOLEAN DEFAULT false,
  is_available_weekend BOOLEAN DEFAULT false,
  hard_restrictions TEXT,
  health_notes TEXT,
  created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  FOREIGN KEY (worker_id) REFERENCES Worker(worker_id),
  FOREIGN KEY (preferred_work_type_code) REFERENCES WorkType(work_type_code)
);
```

#### 6. WorkerJobAbility (활동지원사 가능 업무)

```sql
CREATE TABLE WorkerJobAbility (
  ability_id INT PRIMARY KEY AUTO_INCREMENT,
  worker_id INT NOT NULL,
  job_id INT NOT NULL,
  proficiency_level ENUM('BASIC', 'INTERMEDIATE', 'ADVANCED') DEFAULT 'BASIC',
  is_certified BOOLEAN DEFAULT false,
  certification_date DATE,
  notes TEXT,
  created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  FOREIGN KEY (worker_id) REFERENCES Worker(worker_id),
  FOREIGN KEY (job_id) REFERENCES JobMaster(job_id),
  UNIQUE KEY unique_worker_job (worker_id, job_id)
);

-- Index for quick lookup
CREATE INDEX idx_worker_ability ON WorkerJobAbility(worker_id);
CREATE INDEX idx_job_workers ON WorkerJobAbility(job_id);
```

#### 7. Client (수급자)

```sql
CREATE TABLE Client (
  client_id INT PRIMARY KEY AUTO_INCREMENT,
  name VARCHAR(50) NOT NULL,
  disability_level VARCHAR(20),
  disability_type VARCHAR(100),
  phone VARCHAR(20),
  address VARCHAR(200),
  is_active BOOLEAN DEFAULT true,
  created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP
);
```

#### 8. ClientPreference (수급자 선호 업무)

```sql
CREATE TABLE ClientPreference (
  client_preference_id INT PRIMARY KEY AUTO_INCREMENT,
  client_id INT NOT NULL,
  job_id INT,
  job_category_code VARCHAR(10),
  priority_level INT DEFAULT 0,
  notes TEXT,
  created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  FOREIGN KEY (client_id) REFERENCES Client(client_id),
  FOREIGN KEY (job_id) REFERENCES JobMaster(job_id),
  FOREIGN KEY (job_category_code) REFERENCES JobCategory(job_category_code)
);
```

#### 9. Assignment (활동지원사–수급자 매칭)

```sql
CREATE TABLE Assignment (
  assignment_id INT PRIMARY KEY AUTO_INCREMENT,
  worker_id INT NOT NULL,
  client_id INT NOT NULL,
  assigned_date DATE NOT NULL,
  scheduled_time_slot VARCHAR(100),
  assigned_jobs_ids VARCHAR(500), -- JSON 배열로 저장 권장
  status ENUM('ACTIVE', 'PAUSED', 'COMPLETED', 'CANCELLED') DEFAULT 'ACTIVE',
  notes TEXT,
  created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  FOREIGN KEY (worker_id) REFERENCES Worker(worker_id),
  FOREIGN KEY (client_id) REFERENCES Client(client_id)
);

-- Index
CREATE INDEX idx_assignment_worker ON Assignment(worker_id);
CREATE INDEX idx_assignment_client ON Assignment(client_id);
CREATE INDEX idx_assignment_status ON Assignment(status);
```

### 5.3 주요 쿼리 예시

#### 1. 특정 지역에서 출퇴근 동행 가능한 활동지원사 찾기

```sql
SELECT w.worker_id, w.name, wp.preferred_time_slot, wja.proficiency_level
FROM Worker w
JOIN WorkerPreference wp ON w.worker_id = wp.worker_id
JOIN WorkerJobAbility wja ON w.worker_id = wja.worker_id
JOIN JobMaster jm ON wja.job_id = jm.job_id
WHERE wp.preferred_region LIKE '%강남구%'
  AND jm.job_detail_code = 'SOCI01'
  AND wja.proficiency_level IN ('INTERMEDIATE', 'ADVANCED')
  AND wp.is_available_night_shift = false
  AND w.is_active = true;
```

#### 2. 허리 건강 문제가 없는 활동지원사 중 신체활동 전문가

```sql
SELECT w.worker_id, w.name, COUNT(wja.ability_id) as job_count
FROM Worker w
JOIN WorkerJobAbility wja ON w.worker_id = wja.worker_id
JOIN JobMaster jm ON wja.job_id = jm.job_id
JOIN WorkerPreference wp ON w.worker_id = wp.worker_id
WHERE jm.job_category_code = 'PHYS'
  AND wp.hard_restrictions NOT LIKE '%허리%'
  AND wja.proficiency_level = 'ADVANCED'
  AND w.is_active = true
GROUP BY w.worker_id
HAVING job_count >= 3;
```

#### 3. 수급자별 매칭된 활동지원사 및 담당 업무 조회

```sql
SELECT 
  c.client_id, c.name AS client_name,
  w.worker_id, w.name AS worker_name,
  GROUP_CONCAT(jm.job_name SEPARATOR ', ') as assigned_jobs,
  a.status
FROM Client c
JOIN Assignment a ON c.client_id = a.client_id
JOIN Worker w ON a.worker_id = w.worker_id
LEFT JOIN JobMaster jm ON FIND_IN_SET(jm.job_id, a.assigned_jobs_ids)
WHERE a.status = 'ACTIVE'
GROUP BY c.client_id, w.worker_id;
```

#### 4. 특정 시간대에 가능한 활동지원사 목록

```sql
SELECT w.worker_id, w.name, wp.preferred_time_slot, wp.max_weekly_hours
FROM Worker w
JOIN WorkerPreference wp ON w.worker_id = wp.worker_id
WHERE wp.preferred_time_slot LIKE '%09:00%'
  AND wp.max_weekly_hours >= 30
  AND w.is_active = true
ORDER BY wp.preferred_region, w.name;
```

---

## 6. 운영 가이드

### 6.1 데이터 관리 프로세스

1. **Job Master 초기 구성**
   - 직무 카테고리 및 세부직무 데이터 입력
   - 각 직무별 필수 역량, 건강 제약사항 정의
   - 정기적 업데이트 (제도 변화, 현장 피드백 반영)

2. **Worker 등록 및 Preference 관리**
   - 신규 활동지원사 등록 시 근무 희망 조건 수집
   - WorkerJobAbility를 통해 가능 업무 및 숙련도 입력
   - 정기적으로 근무 형태 및 가능 업무 검토

3. **Client 등록 및 매칭**
   - 수급자 등록 시 필요 업무 유형 파악
   - ClientPreference를 통해 선호 활동지원사 특성 기록
   - Assignment 생성 시 Worker–Client–Job 일관성 검증

4. **주기적 점검**
   - 월 1회: Worker 가능 업무 및 근무 조건 변경 여부 확인
   - 분기별: 매칭 효율성 분석 및 개선 방향 논의
   - 연 1회: 직무 분류 및 기준 검토

### 6.2 데이터 품질 관리

**필수 입력 항목 검증:**
- Worker: name, phone, certification_date
- WorkerPreference: worker_id, preferred_work_type_code, preferred_region
- WorkerJobAbility: worker_id, job_id, proficiency_level
- Client: name, disability_level, address
- Assignment: worker_id, client_id, assigned_date, assigned_jobs_ids

**정기 검증:**
- 비활성화 (is_active = false) 된 Worker 재검토
- Null 값 또는 불완전한 레코드 정리
- 활동지원사 자격 갱신 일정 추적

---

## 부록: 용어 정의

| 용어 | 정의 |
|------|------|
| **활동지원사** | 장애인의 일상생활 및 사회활동을 지원하기 위해 신체 및 가사·사회활동 지원을 담당하는 직원 |
| **수급자** | 장애인활동지원 서비스를 받는 등록 장애인 |
| **신체활동지원** | 배뇨·배변, 식사, 목욕 등 신체 기능 유지를 위한 직접 지원 |
| **가사활동지원** | 청소, 세탁, 취사 등 가정 내 일상 가사 지원 |
| **사회활동지원** | 출퇴근, 병원 방문, 외출 등 사회 참여를 위한 이동·동행 지원 |
| **정서지원** | 말벗, 상담, 의사소통 보조 등 정서적 안정 지원 |
| **매칭** | 활동지원사와 수급자를 배정하는 과정 |
| **직무 코드** | 각 직무를 고유하게 식별하는 알파벳-숫자 조합 (예: PHYS01) |
| **숙련도** | 활동지원사의 특정 업무 수행 능력 (BASIC, INTERMEDIATE, ADVANCED) |
| **근무 형태** | 활동지원사의 근무 시간대 및 요일 패턴 (예: 평일 오전, 주말 위주) |

---

**문서 작성일**: 2025년 12월 11일  
**최종 수정일**: 2025년 12월 11일  
**버전**: 1.0

