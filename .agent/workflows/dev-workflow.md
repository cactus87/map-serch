---
description: 장애인 활동지원 매칭 시스템 MVP 개발 워크플로우
---

# 개발 워크플로우

## 기술 스택
- **Framework**: Blazor Server (.NET 9)
- **UI**: MudBlazor
- **지도**: Naver Maps JS API v3
- **데이터**: Mock JSON (메모리상 가짜 데이터)

## 개발 명령어

### 프로젝트 생성
// turbo
```bash
dotnet new blazorserver -n MapDemo -f net9.0
```

### MudBlazor 설치
// turbo
```bash
dotnet add package MudBlazor
```

### 실행
// turbo
```bash
dotnet run
```

### 빌드
// turbo
```bash
dotnet build
```

## 개발 단계

### Phase 1: 지도 띄우기
1. Blazor Server 프로젝트 생성
2. MudBlazor 설치 및 설정
3. 30%/70% Split View 레이아웃 구성
4. Naver Maps API v3 연동 (JS Interop)
5. 도봉구청 좌표 지도 표시

### Phase 2: Mock 데이터 + 마커
1. Person 모델 생성
2. MockDataService 구현 (이용자 10명, 지원사 20명)
3. JS Interop으로 마커 추가
4. 이용자: 파란 원, 지원사: 주황 사각형

### Phase 3: 반경 검색 로직
1. 이용자 마커 클릭 이벤트 핸들링
2. 클릭 시 Circle 그리기 (1km/3km)
3. Haversine 공식으로 거리 계산
4. 반경 밖 지원사 마커 숨김
5. MudDataGrid 필터링

### Phase 4: UI 다듬기
1. 상단 MudButtonGroup (1km/3km/전체)
2. 거리(km) 컬럼 추가
3. 가까운 순 정렬

## 성공 지표
- ✅ 지도 로딩 시 30개 마커가 5초 이내 표시
- ✅ 반경 필터링 1초 이내 완료
- ✅ 실제 코디네이터 피드백 긍정적
