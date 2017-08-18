using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Fake.FakeRunner.Unity
{
    public class UIManager : MonoBehaviour
    {
        #region Fields
        [SerializeField]
        private GameObject optionPanel;
        [SerializeField]
        private GameObject gameoverPanel;
        [SerializeField]
        private CanvasGroup optionBackground;
        [SerializeField]
        private CanvasGroup gameoverBackground;

        private bool OptionIsPlaying;
        #endregion

        void Start()
        {
            OptionIsPlaying = false;

            gameoverPanel.SetActive(false);
            optionPanel.SetActive(false);
        }

        public void OpenOptionPanel()
        {
            if (OptionIsPlaying == false && optionPanel.activeSelf == false)
            {
                SoundManager.Instance.PlayPauseSound();
                StartCoroutine(OpenOptionPanelAnimation());
            }

            optionPanel.SetActive(true);
        }

        private IEnumerator OpenOptionPanelAnimation()
        {
            Super.Instance.SetRunnerControl(false);
            OptionIsPlaying = true;
            optionBackground.alpha = 0.0f;

            var duration = 0.5f;
            var startTime = Time.time;
            var endTime = startTime + duration;

            while (Time.time < endTime)
            {
                var elaspedTime = Time.time - startTime;
                var time = elaspedTime / duration;

                optionBackground.alpha = Mathf.Lerp(0, 1, time);
                yield return null;
            }

            optionBackground.alpha = 1.0f;

            OptionIsPlaying = false;
        }

        public void CloseOptionPanel()
        {
            if (OptionIsPlaying == false)
            {
                SoundManager.Instance.PlayPauseSound();
                Super.Instance.SetRunnerControl(true);
                optionPanel.SetActive(false);
            }
        }

        public void GameOver()
        {
            StartCoroutine(GameOverAnimation());
        }

        private IEnumerator GameOverAnimation()
        {
            gameoverPanel.SetActive(true);

            Super.Instance.SetRunnerControl(false);
            Super.Instance.GameplayTimeline.StopTime();

            gameoverBackground.alpha = 1.0f;

            var duration = 1.0f;
            var startTime = Time.time;
            var endTime = startTime + duration;

            while (Time.time < endTime)
            {
                var elaspedTime = Time.time - startTime;
                var time = elaspedTime / duration;

                gameoverBackground.alpha = Mathf.Lerp(0, 1, time);
                yield return null;
            }

            gameoverBackground.alpha = 1.0f;
        }

        public void RestartButton()
        {
            gameoverPanel.SetActive(false);
            Super.Instance.SetRunnerControl(true);
            Super.Instance.GameplayTimeline.SetTimeScale(1.0f);
            Super.Instance.AnimationTimeline.SetTimeScale(1.0f);
            Super.Instance.GameplayTimeline.CurrentTime = 0.0f;

            Super.Instance.ClearBlackBoxs();

            Super.Instance.StartMovie();
        }
    }
}