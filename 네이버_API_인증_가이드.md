# 네이버 Maps API 인증 문제 해결 가이드

**프로젝트**: LMP-Link MVP  
**작성일**: 2025-12-10  
**문서 버전**: 1.0

---

## 📋 현재 API 키 설정 상태

### 1. User Secrets (안전한 저장소)
```bash
# 설정 완료
NaverMapClientId: a5r744p216
NaverMapClientSecret: RukZHZPMeVKt1sAETtEnptFE3gFrcIdSRRVrbB7t
```

### 2. MainPage.xaml (JavaScript API)
```xml
<!-- Line 145 -->
<script src="https://oapi.map.naver.com/openapi/v3/maps.js?ncpKeyId=a5r744p216"></script>
```

### 3. .gitignore 확인
```
# 반드시 추가
.env.local
.env
*.env
```

---

## 🔍 네이버 Maps API 인증 체크리스트

### Step 1: 네이버 클라우드 플랫폼 콘솔 설정

1. **Web Dynamic Map 서비스 활성화**
   - URL: https://console.ncloud.com/naver-service/application
   - Application → 신청 → Web Dynamic Map 활성화

2. **Web 서비스 URL 등록** (필수!)
   - `http://localhost:5000`
   - `http://localhost:5000/`
   - `http://127.0.0.1:5000`
   - `http://127.0.0.1:5000/`
   - ⚠️ **슬래시(/) 유무 모두 등록** 필요!

3. **API Key 발급 확인**
   - Client ID (X-NCP-APIGW-API-KEY-ID): `a5r744p216`
   - Client Secret (X-NCP-APIGW-API-KEY): `Ruk...` (User Secrets에만 저장)

---

## 🚨 자주 발생하는 에러 & 해결 방법

### 에러 1: `navermap_authFailure` 호출됨
**증상**: 콘솔에 "네이버 지도 인증 실패" alert 표시

**원인**:
1. Client ID가 잘못되었거나
2. Web 서비스 URL이 등록되지 않음
3. Web Dynamic Map 서비스가 비활성화됨

**해결**:
```bash
# 1. Client ID 확인
echo "a5r744p216"

# 2. 네이버 클라우드 콘솔 → Application → Web Dynamic Map 확인
# 3. Web 서비스 URL 4개 모두 등록 확인
```

---

### 에러 2: `naver is not defined`
**증상**: 콘솔에 `ReferenceError: naver is not defined`

**원인**: API 스크립트가 로드되기 전에 JavaScript 실행됨

**해결**:
```javascript
// MainPage.xaml.cs - OnWebViewNavigated
await Task.Delay(500); // DOM ready 대기
await _mapService.InitMapAsync();
```

---

### 에러 3: CORS 에러 (Cross-Origin)
**증상**: `Access to fetch at 'https://...' has been blocked by CORS policy`

**원인**: WebView에서 외부 API 호출 시 CORS 정책

**해결**:
- MAUI WebView는 일반적으로 CORS 제약 없음
- Windows WebView2는 자동으로 허용
- 문제 발생 시: 네이버 클라우드 콘솔에서 허용 도메인 확인

---

### 에러 4: `Invalid Referer`
**증상**: 지도가 로드되지 않고 콘솔에 Referer 에러

**원인**: Referer 헤더가 등록된 URL과 다름

**해결**:
```bash
# 브라우저 개발자 도구 (F12) → Network 탭
# maps.js 요청 → Headers → Request Headers → Referer 확인

# Referer가 "about:blank" 또는 다른 URL인 경우:
# → 네이버 클라우드 콘솔에 해당 URL 추가 등록
```

---

## 🧪 디버깅 방법

### 1. 브라우저 개발자 도구 열기
```csharp
// MainPage.xaml.cs (Windows only)
#if WINDOWS
MapWebView.Source = new Uri("devtools://devtools/bundled/inspector.html");
#endif
```

### 2. JavaScript 콘솔 로그 확인
```javascript
// MainPage.xaml Line 180-184
console.log('Map initialized:', lat, lng, zoom);
console.log('Marker added:', id, type, name);
console.error('initMap error:', error);
```

### 3. C# 디버그 로그 확인
```csharp
// Output 창에서 확인
System.Diagnostics.Debug.WriteLine("WebView navigation success.");
System.Diagnostics.Debug.WriteLine($"Map initialization failed: {ex.Message}");
```

---

## ✅ 정상 작동 확인 체크리스트

### 1. 지도 로드 확인
- [ ] 앱 시작 → 3초 이내 지도 표시
- [ ] 콘솔에 "Map initialized: 37.6688, 127.0471, 13" 출력

### 2. 마커 렌더링 확인
- [ ] 이용자 10개 (파란 원 20px) 표시
- [ ] 지원사 20개 (주황 사각형 16px) 표시
- [ ] 콘솔에 "Marker added: 1, user, 김철수" 출력 (총 30회)

### 3. 인증 성공 확인
- [ ] `navermap_authFailure` 호출되지 않음
- [ ] 네트워크 탭에서 `maps.js` 요청 200 OK

### 4. 상호작용 확인
- [ ] 이용자 클릭 → 지도 중심 이동 + 파란 원 표시
- [ ] 반경 버튼 (1km/3km/5km) → 원 크기 변경
- [ ] "전체" 버튼 → 원 제거 + 모든 마커 표시

---

## 📞 네이버 클라우드 고객 지원

### 1. 공식 문서
- **Maps API v3**: https://navermaps.github.io/maps.js.ncp/docs/
- **Getting Started**: https://navermaps.github.io/maps.js.ncp/docs/tutorial-2-Getting-Started.html

### 2. 네이버 클라우드 콘솔
- **Application 관리**: https://console.ncloud.com/naver-service/application
- **Web Dynamic Map**: https://console.ncloud.com/naver-service/application

### 3. 고객 지원 센터
- **Email**: ncloud_maps@navercorp.com
- **전화**: 1588-3820

---

## 🔒 보안 가이드라인

### 1. User Secrets 사용 (개발 환경)
```bash
# ✅ 올바른 방법
dotnet user-secrets set "NaverMapClientId" "a5r744p216"
dotnet user-secrets set "NaverMapClientSecret" "YOUR_SECRET"

# ❌ 잘못된 방법
# MainPage.xaml에 Client Secret 삽입 (절대 금지!)
```

### 2. .gitignore 설정
```gitignore
# 필수
.env.local
.env
*.env
appsettings.*.json  # 단, appsettings.json은 제외
```

### 3. Client Secret 사용 시나리오
- **JavaScript API**: Client ID만 필요 (`ncpKeyId` 파라미터)
- **Server-side API**: Client ID + Client Secret (HTTP 헤더)
  ```http
  X-NCP-APIGW-API-KEY-ID: a5r744p216
  X-NCP-APIGW-API-KEY: RukZHZPMeVKt1sAETtEnptFE3gFrcIdSRRVrbB7t
  ```

### 4. 프로덕션 배포 시
- Azure App Service: Application Settings 사용
- AWS: Systems Manager Parameter Store
- Docker: Docker Secrets 또는 환경변수

---

## 🎯 빠른 테스트

### 빌드 & 실행
```bash
cd C:\ai\projects\map-demo\src\LmpLink.MAUI

# 빌드
dotnet build --configuration Debug

# 실행 (Windows)
dotnet run

# 또는 Visual Studio에서 F5
```

### 예상 결과
1. 앱 시작
2. 지도 로드 (도봉구청 중심)
3. 마커 30개 표시
4. 좌측 패널에 이용자 10명 목록
5. 이용자 클릭 → 지도 반응

---

**문서 버전**: 1.0  
**마지막 업데이트**: 2025-12-10  
**다음 업데이트**: 네이버 API 정책 변경 시
