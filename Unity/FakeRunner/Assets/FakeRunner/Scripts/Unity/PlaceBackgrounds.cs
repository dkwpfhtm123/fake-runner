using UnityEngine;

namespace Fake.FakeRunner.Unity
{
    public class PlaceBackgrounds : MonoBehaviour
    {
        #region Fields
        [SerializeField]
        private new Camera camera;
        [SerializeField]
        private GameObject[] backgrounds;
        #endregion

        private void Start()
        {
            foreach (var back in backgrounds)
                PlaceBackground(back);
        }

        public void PlaceBackground(GameObject background)
        {
            var back = Instantiate(background);
            back.GetComponent<BackgroundParallax>().Initialize(camera);
        }
    }
}