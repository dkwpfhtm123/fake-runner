using UnityEngine;

namespace Fake.FakeRunner.Unity
{
    public class BackGroundParallax : MonoBehaviour
    {
        #region Fields
        [SerializeField]
        private float offset;

        private new Camera camera;
        private Transform transformCache;

        private Vector3 oldCameraPosition;

        public Camera Camera
        {
            get { return camera; }
            set { camera = value; }
        }
        #endregion

        void Start()
        {
            transformCache = GetComponent<Transform>();
            oldCameraPosition = camera.transform.localPosition;
        }

        void Update()
        {
            if (oldCameraPosition != camera.transform.localPosition)
                oldCameraPosition = camera.transform.localPosition;

            transformCache.localPosition = new Vector3(camera.transform.localPosition.x - camera.transform.localPosition.x / offset % 20, 5);

            if (Mathf.Abs(oldCameraPosition.x - camera.transform.localPosition.x) > 20.0f / offset)
            {
                oldCameraPosition = camera.transform.localPosition;
                camera.GetComponent<BackGrounds>().ResetBackGround(gameObject);
                Destroy(gameObject);
            }
        }
    }
}