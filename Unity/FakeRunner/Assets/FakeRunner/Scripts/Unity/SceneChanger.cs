using UnityEngine;
using UnityEngine.SceneManagement;

namespace Fake.FakeRunner.Unity
{
    public class SceneChanger : MonoBehaviour
    {
        public void GoToGamePlayScene()
        {
            SceneManager.LoadScene("GamePlay");
        }

        public void GoToMainScene()
        {
            SceneManager.LoadScene("Main");
        }
    }
}