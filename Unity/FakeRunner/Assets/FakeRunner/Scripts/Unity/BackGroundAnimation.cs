using UnityEngine;
using System.Collections.Generic;

namespace Fake.FakeRunner.Unity
{
    public class BackGroundAnimation : MonoBehaviour
    {
        #region Fields
        [SerializeField]
        private Camera cam;
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
            var cameraPosition = cam.transform.localPosition;

            var back = Instantiate(background);
            back.transform.localPosition = new Vector3(cameraPosition.x, 5);
            back.GetComponent<BackGroundParallax>().Camera = cam;
        }
    }
}