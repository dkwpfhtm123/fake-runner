using UnityEngine;
using UnityEngine.SceneManagement;

namespace Fake.FakeRunner.Unity
{
    public class SceneChanger : MonoBehaviour
    {
        public void ToGamePlayScene()
        {
            SceneManager.LoadScene("GamePlay");
        }

        public void ToMainScene()
        {
            SceneManager.LoadScene("Main");
        }
    }
}