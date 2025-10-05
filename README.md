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


# AssetLoader

Unity Addressables에 등록된 **프리팹을 비동기 로드·인스턴스화**하는 유틸리티 컴포넌트입니다.  
프리팹 로드 → 인스턴스화 → 부모 트랜스폼 부착 → 완료 후 자동 파괴까지 한 번에 처리합니다.

## 사용법

1. **컴포넌트 추가**
   ```csharp
   new GameObject(nameof(AssetLoader), typeof(AssetLoader));
   ```

2. **실행**
   ```csharp
   using oojjrs.oaddr; // namespace

   public class Sample : MonoBehaviour
   {
       public Transform mount;

       private async void Start()
       {
           var go = new GameObject(nameof(AssetLoader), typeof(AssetLoader));
           var loader = go.GetComponent<AssetLoader>();
           await loader.RunAsync(
               key: "MyPrefabKey",     // Addressables 키
               parentTransform: mount, // null이면 루트에 생성
               destroyOnComplete: true // 완료 후 로더 파괴
           );
       }
   }
   ```

## 주요 기능
- `Addressables.LoadAssetAsync<GameObject>(key)`로 프리팹 로드 후 `Instantiate`
- 부모 트랜스폼(`parentTransform`) 지정 시 `SetParent(parent, false)`로 부착
- `destroyOnComplete`(기본값 true)로 작업 종료 후 로더 GameObject 자동 삭제
- 로깅 커스터마이즈 가능(`ILogger` 미지정 시 `Debug.unityLogger`)
- 로드 실패 시 `OperationException.Message`를 Error 로그로 출력
