using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Fake.FakeRunner.Unity
{
    public partial class Super : MonoBehaviour
    {
        #region Fields
        [SerializeField]
        private MapCreator mapCreator;
        [SerializeField]
        private GameObject runner;
        [SerializeField]
        private MovieManager movieManager;
        [SerializeField]
        private SoundManager soundManager;
        [SerializeField]
        private UIManager uiManager;
        [SerializeField]
        private Text score;
        [SerializeField]
        private Timeline gameplayTimeline;
        [SerializeField]
        private Timeline animationTimeline;

        private static Super instance;
        private Runner runnerCache;
        #endregion

        public Timeline GameplayTimeline
        {
            get { return gameplayTimeline; }
        }

        public Timeline AnimationTimeline
        {
            get { return animationTimeline; }
        }

        public static Super Instance
        {
            get
            {
                if (instance == null)
                    instance = new Super();
                return instance;
            }
        }

              private void Awake()
              {
                  if (instance == null)
                      instance = this;
              } 

        private void Start()
        {
            runnerCache = runner.GetComponent<Runner>();

            gameplayTimeline = new Timeline(1.0f);
            animationTimeline = new Timeline(1.0f);

            StartFirstMovie();
        }

        private void Update()
        {
            gameplayTimeline.Update();
            animationTimeline.Update();
        }

        public void SetRunnerControl(bool value)
        {
            runnerCache.CanControl = value;
        }

        public void Initialize()
        {
            runnerCache.Initialize();
            mapCreator.Initialize();
            mapCreator.GetComponent<RunnerTracker>().Initialize();
            ClearBlackBoxes();
            gameplayTimeline.SetTimeScale(1.0f);
            animationTimeline.SetTimeScale(1.0f);
        }

        #region Timeline
        [System.Serializable]
        public class Timeline
        {
            private float currentTime;
            private float scale;
            private float deltaTime;

            public float DeltaTime
            {
                get { return deltaTime; }
            }

            public float CurrentTime
            {
                get { return currentTime; }
            }

            public Timeline(float speed)
            {
                this.currentTime = 0.0f;
                this.scale = speed;
                deltaTime = Time.deltaTime * scale;
            }

            public void Update()
            {
                deltaTime = Time.deltaTime * scale;
                currentTime += deltaTime;
            }

            public void SetTimeScale(float value)
            {
                scale = value;
            }
        }
        #endregion
    }

    #region MovieManager
    public partial class Super
    {
        public void StartFirstMovie()
        {
            StartCoroutine(FirstMovie());
        }

        public void EndMovie()
        {
            movieManager.StartEndMovie();
        }

        public void ClearBlackBoxes()
        {
            movieManager.StopMovie();
            movieManager.ClearBlackBoxes();
        }

        public void PlayGameOver()
        {
            PlayGameOverSound();
            EndMovie();
            score.text = "SCORE : " + instance.GameplayTimeline.CurrentTime;
            uiManager.PlayGameOverUI();
        }

        private IEnumerator FirstMovie()
        {
            SetRunnerControl(false);
            movieManager.StartFirstMovie();
            while (movieManager.OnPlayingMovie == true)
            {
                yield return null;
            }

            SetRunnerControl(true);
            runner.GetComponent<RunnerAnimation>().StartAnimation();
        }
    }
    #endregion

    #region SoundManager
    public partial class Super
    {
        public void PlayJumpSound()
        {
            soundManager.PlayJumpSound();
        }

        public void PlayPauseSound()
        {
            soundManager.PlayPauseSound();
        }

        public void PlayCoinSound()
        {
            soundManager.PlayCoinSound();
        }

        public void PlayRunnerHitSound()
        {
            soundManager.PlayRunnerHitSound();
        }

        public void PlayPipeSound()
        {
            soundManager.PlayPipeSound();
        }

        public void PlayGameOverSound()
        {
            soundManager.PlayGameOverSound();
        }

        public void ChangeVolume(float value)
        {
            soundManager.AudioVolume = value;
        }
    }
    #endregion
}