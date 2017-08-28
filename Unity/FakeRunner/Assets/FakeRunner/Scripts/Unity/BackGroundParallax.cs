using UnityEngine;

namespace Fake.FakeRunner.Unity
{
    public class BackgroundParallax : MonoBehaviour
    {
        #region Fields
        [SerializeField]
        private float offset;

        private Transform transformCache;
        private Transform cameraTransformCache;
        #endregion

        private void Update()
        {
            if (transformCache != null)
                transformCache.localPosition = new Vector3(cameraTransformCache.localPosition.x - cameraTransformCache.localPosition.x / offset % 20, 5);
        }

        public void Initialize(Camera camera)
        {
            transformCache = GetComponent<Transform>();

            if (camera == null)
                Debug.Log("null");
            cameraTransformCache = camera.GetComponent<Transform>();
            transformCache.localPosition = new Vector3(cameraTransformCache.localPosition.x, 5);
        }
    }
}