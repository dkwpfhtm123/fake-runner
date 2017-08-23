using UnityEngine;

namespace Fake.FakeRunner.Unity
{
    public class BackgroundParallax : MonoBehaviour
    {
        #region Fields
        [SerializeField]
        private float offset;

        private Camera cameraCache;
        private Transform transformCache;
        private Transform cameraTransformCache;
        #endregion

        private void Update()
        {
            transformCache.localPosition = new Vector3(cameraCache.transform.localPosition.x - cameraCache.transform.localPosition.x / offset % 20, 5);
        }

        public void Initialize(Camera camera)
        {
            transformCache = GetComponent<Transform>();
            this.cameraCache = camera;

            if (cameraTransformCache == null)
                cameraTransformCache = camera.GetComponent<Transform>();

            transformCache.localPosition = new Vector3(cameraTransformCache.localPosition.x, 5);
        }
    }
}