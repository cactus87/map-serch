// 네이버 지도 인스턴스
let map = null;
let markers = {};
let circle = null;
let dotNetRef = null;

// DotNetObjectReference 설정
window.setDotNetReference = function (ref) {
    dotNetRef = ref;
};

// 네이버 지도 API 로드 확인
function ensureNaverMapsLoaded() {
    if (typeof naver === 'undefined' || typeof naver.maps === 'undefined') {
        console.error('네이버 지도 API가 로드되지 않았습니다. API 키를 확인하세요.');
        throw new Error('Naver Maps API not loaded. Please check API key configuration.');
    }
}

// 지도 초기화
window.initMap = function (elementId, lat, lng, zoom) {
    try {
        ensureNaverMapsLoaded();
        
        const mapOptions = {
            center: new naver.maps.LatLng(lat, lng),
            zoom: zoom
        };
        map = new naver.maps.Map(elementId, mapOptions);
        console.log('지도 초기화 완료');
    } catch (error) {
        console.error('지도 초기화 실패:', error);
        throw error;
    }
};

// 마커 추가
window.addMarker = function (id, lat, lng, type, name) {
    try {
        ensureNaverMapsLoaded();
        
        if (!map) {
            console.error('지도가 초기화되지 않았습니다.');
            return;
        }
    } catch (error) {
        console.error('마커 추가 실패:', error);
        return;
    }

    const position = new naver.maps.LatLng(lat, lng);

    // 마커 아이콘 설정
    let markerOptions = {
        position: position,
        map: map,
        title: name
    };

    if (type === 'User') {
        // 이용자: 파란 원
        markerOptions.icon = {
            content: '<div style="width:20px;height:20px;background-color:#4285F4;border:2px solid white;border-radius:50%;box-shadow:0 2px 4px rgba(0,0,0,0.3);"></div>',
            anchor: new naver.maps.Point(10, 10)
        };
    } else {
        // 활동지원사: 주황 사각형
        markerOptions.icon = {
            content: '<div style="width:16px;height:16px;background-color:#FF9800;border:2px solid white;box-shadow:0 2px 4px rgba(0,0,0,0.3);"></div>',
            anchor: new naver.maps.Point(8, 8)
        };
    }

    const marker = new naver.maps.Marker(markerOptions);
    markers[id] = marker;

    // 이용자 마커 클릭 이벤트
    if (type === 'User') {
        naver.maps.Event.addListener(marker, 'click', function () {
            if (dotNetRef) {
                dotNetRef.invokeMethodAsync('OnUserMarkerClicked', id, lat, lng);
            }
        });
    }
};

// 마커 표시/숨김
window.setMarkerVisible = function (id, visible) {
    if (markers[id]) {
        markers[id].setMap(visible ? map : null);
    }
};

// 모든 마커 표시
window.showAllMarkers = function () {
    for (let id in markers) {
        markers[id].setMap(map);
    }
};

// Circle 그리기
window.drawCircle = function (lat, lng, radiusKm) {
    try {
        ensureNaverMapsLoaded();
        
        if (!map) {
            console.error('지도가 초기화되지 않았습니다.');
            return;
        }
        
        clearCircle();

        circle = new naver.maps.Circle({
            map: map,
            center: new naver.maps.LatLng(lat, lng),
            radius: radiusKm * 1000, // km를 m로 변환
            fillColor: '#4285F4',
            fillOpacity: 0.2,
            strokeColor: '#4285F4',
            strokeOpacity: 0.6,
            strokeWeight: 2
        });
    } catch (error) {
        console.error('Circle 그리기 실패:', error);
    }
};

// Circle 제거
window.clearCircle = function () {
    if (circle) {
        circle.setMap(null);
        circle = null;
    }
};

// 모든 마커 제거
window.clearAllMarkers = function () {
    for (let id in markers) {
        markers[id].setMap(null);
    }
    markers = {};
};
