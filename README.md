# AssetDownloader

Unity Addressables 에셋을 다운로드/갱신하기 위한 유틸리티 컴포넌트입니다.  
카탈로그 업데이트 → 용량 확인 → 다운로드를 순차적으로 수행합니다.

## 사용법

1. **컴포넌트 추가**
   ```csharp
   gameObject.AddComponent<AssetDownloader>();
   ```

2. **콜백 인터페이스 구현**
   ```csharp
   public class MyDownloaderCallback : AssetDownloader.CallbackInterface
   {
       public void ConfirmDownloadLargeSize(long totalSize, Action onYes, Action onNo) { /* 용량 확인 UI */ }
       public void Cancel() { /* 취소 처리 */ }
       public void OnComplete() { /* 완료 처리 */ }
       public void OnStart() { /* 시작 처리 */ }
   }
   ```

3. **실행**
   ```csharp
   var downloader = GetComponent<AssetDownloader>();
   downloader.Run(new MyDownloaderCallback(), DownloadConfirmSizeMb: 50);
   ```

## 주요 기능
- 자동 카탈로그 확인 및 업데이트
- 총 다운로드 용량 계산
- 지정 크기(MB) 이상 시 사용자 확인 요청
- 진행률/바이트 단위 상태 확인  
  - `DownloadedBytes`  
  - `TotalBytes`  
  - `Percent`
