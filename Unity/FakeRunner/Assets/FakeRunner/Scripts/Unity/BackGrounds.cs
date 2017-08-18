using UnityEngine;
using System.Collections.Generic;

namespace Fake.FakeRunner.Unity
{
    public class BackGrounds : MonoBehaviour
    {
        #region Fields
        [SerializeField]
        private new Camera camera;
        [SerializeField]
        private GameObject[] backGrounds;
        #endregion

        private void Start()
        {
            foreach (var back in backGrounds)
                PlaceBackGround(back);
        }

        public void PlaceBackGround(GameObject background)
        {
            var cameraPosition = camera.transform.localPosition;

            var back = Instantiate(background);
            back.transform.localPosition = new Vector3(cameraPosition.x, 5);
            back.GetComponent<BackGroundParallax>().Camera = camera;
        }

        public void ResetBackGround(GameObject background)
        {
            background.transform.localPosition = new Vector3(camera.transform.localPosition.x, 5);
        }
    }
}