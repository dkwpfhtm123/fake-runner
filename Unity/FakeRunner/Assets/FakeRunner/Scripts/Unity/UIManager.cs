using UnityEngine;
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

        private bool optionPanelPlaying;
        #endregion

        private void Start()
        {
            optionPanelPlaying = false;

            gameoverPanel.SetActive(false);
            optionPanel.SetActive(false);
        }

        public void PlayOpenOptionPanel()
        {
            if (optionPanelPlaying == false && optionPanel.activeSelf == false)
            {
                Super.Instance.PlayPauseSound();
                StartCoroutine(OptionPanelAnimation());
            }

            optionPanel.SetActive(true);
        }

        public void RestartButton()
        {
            gameoverPanel.SetActive(false);
            Super.Instance.Initialize();
            Super.Instance.StartFirstMovie();
        }

        public void CloseOptionPanel()
        {
            if (optionPanelPlaying == false)
            {
                Super.Instance.PlayPauseSound();
                Super.Instance.SetRunnerControl(true);
                optionPanel.SetActive(false);

                Super.Instance.GameplayTimeline.SetTimeScale(1.0f);
            }
        }

        public void PlayGameOverUI()
        {
            StartCoroutine(GameOverAnimation());
        }

        private IEnumerator GameOverAnimation()
        {
            gameoverPanel.SetActive(true);

            Super.Instance.SetRunnerControl(false);
            Super.Instance.GameplayTimeline.SetTimeScale(0.0f);

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

        private IEnumerator OptionPanelAnimation()
        {
            Super.Instance.GameplayTimeline.SetTimeScale(0.0f);
            optionPanelPlaying = true;
            optionBackground.alpha = 0.0f;

            var countDown = 0.5f;
            var duration = countDown;

            while (countDown > 0.0f)
            {
                var value = duration - countDown;
                countDown -= Super.Instance.AnimationTimeline.DeltaTime;
                optionBackground.alpha = Mathf.Lerp(0, 1, value);
                yield return null;
            }

            optionPanelPlaying = false;
            optionBackground.alpha = 1.0f;
        }

    }
}