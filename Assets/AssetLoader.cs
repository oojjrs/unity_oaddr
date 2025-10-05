using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace oojjrs.oaddr
{
    public class AssetLoader : MonoBehaviour
    {
        public async Task RunAsync(string key, Transform parentTransform, bool destroyOnComplete = true, ILogger logger = default)
        {
            if (logger == default)
                logger = Debug.unityLogger;

            logger.Log($"{name}> BEGIN.");

            var handle = Addressables.LoadAssetAsync<GameObject>(key);
            var prefab = await handle.Task;
            if (prefab != default)
            {
                var go = Instantiate(prefab);
                if (parentTransform != default)
                    go.transform.SetParent(parentTransform, false);

                logger.Log($"{name}> END.");
            }
            else
            {
                logger.Log(LogType.Error, handle.OperationException.Message);
            }

            if (destroyOnComplete)
                Destroy(gameObject);
        }
    }
}
