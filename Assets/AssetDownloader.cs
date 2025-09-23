using System;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace oojjrs.oaddr
{
    public class AssetDownloader : MonoBehaviour
    {
        public interface CallbackInterface
        {
            void ConfirmDownloadLargeSize(long totalSize, Action onYes, Action onNo);
            void Cancel();
            void OnComplete();
            void OnStart();
        }

        private CallbackInterface Callback { get; set; }
        private AsyncOperationHandle CurrentHandle { get; set; }
        public long DownloadedBytes
        {
            get
            {
                if (CurrentHandle.IsValid())
                    return CurrentHandle.GetDownloadStatus().DownloadedBytes;
                else
                    return 0;
            }
        }
        public float Percent
        {
            get
            {
                if (CurrentHandle.IsValid())
                    return CurrentHandle.GetDownloadStatus().Percent;
                else
                    return 0;
            }
        }
        public long TotalBytes
        {
            get
            {
                if (CurrentHandle.IsValid())
                    return CurrentHandle.GetDownloadStatus().TotalBytes;
                else
                    return 0;
            }
        }

        public async void Run(CallbackInterface callback, int DownloadConfirmSizeMb = 10)
        {
            Callback = callback;
            if (Callback == default)
            {
                Debug.LogWarning($"{name}> DON'T HAVE CALLBACK FUNCTION.");
                return;
            }

            Debug.Log($"Addressables BuildPath : {Addressables.BuildPath}");
            Debug.Log($"Addressables PlayerBuildDataPath : {Addressables.PlayerBuildDataPath}");
            Debug.Log($"Addressables RuntimePath : {Addressables.RuntimePath}");

            Debug.Log("ALL ASSETS ARE CHECKING...");

            var locators = Addressables.ResourceLocators;
            // localization preload 기능 때문에 실질적으로 얘가 동작할 일이 없다.
            var keys = await Addressables.CheckForCatalogUpdates().Task;
            if (keys.Count > 0)
                locators = await Addressables.UpdateCatalogs().Task;

            var targetKeys = locators.Where(locator => Addressables.GetLocatorInfo(locator.LocatorId).CanUpdateContent).SelectMany(locator => locator.Keys).ToArray();
            var totalSize = await Addressables.GetDownloadSizeAsync(targetKeys.AsEnumerable()).Task;
            if (totalSize > 0)
            {
                var sizeAsMb = totalSize / (1 << 20);
                if ((Application.internetReachability != NetworkReachability.ReachableViaLocalAreaNetwork) || (sizeAsMb >= DownloadConfirmSizeMb))
                    Callback.ConfirmDownloadLargeSize(totalSize, () => StartDownload(targetKeys), Callback.Cancel);
                else
                    StartDownload(targetKeys);
            }
            else
            {
                Debug.Log("ALL ASSETS ARE PASSED.");

                Callback.OnComplete();
            }
        }

        private async void StartDownload(object[] targetKeys)
        {
            Callback.OnStart();

            CurrentHandle = Addressables.DownloadDependenciesAsync(targetKeys.AsEnumerable(), Addressables.MergeMode.Union);
            await CurrentHandle.Task;

            if (CurrentHandle.Status == AsyncOperationStatus.Succeeded)
            {
                Debug.Log("ALL ASSETS ARE DOWNLOADED.");

                Callback.OnComplete();
            }
            else
            {
                Debug.LogWarning($"SOME ASSETS ARE FAILED TO DOWNLOAD. ({CurrentHandle.OperationException?.Message})");

                Callback.Cancel();
            }

            CurrentHandle = default;
        }
    }
}
