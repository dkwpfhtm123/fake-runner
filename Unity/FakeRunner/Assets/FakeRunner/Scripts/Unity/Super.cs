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
        private UIManager uiManager;
        [SerializeField]
        private Text score;
        [SerializeField]
        private Timeline gameplayTimeline;
        [SerializeField]
        private Timeline animationTimeline;

        private static Super instance;
        private Runner runnerCache;
        private bool stopAnimationTime;
        private bool stopGamePlayTime;

        public Timeline GameplayTimeline
        {
            get { return gameplayTimeline; }
        }

        public Timeline AnimationTimeline
        {
            get { return animationTimeline; }
        }

        public bool StopAnimationTime
        {
            set { stopAnimationTime = value; }
        }

        public bool StopGameplayTime
        {
            set { stopGamePlayTime = value; }
        }

        #endregion

        public static Super Instance
        {
            get
            {
                if (instance == null)
                    instance = new Super();
                return instance;
            }
        }

        void Awake()
        {
            if (instance == null)
                instance = this;
        }

        public void Start()
        {
            runnerCache = runner.GetComponent<Runner>();

            gameplayTimeline = new Timeline(1.0f);
            animationTimeline = new Timeline(1.0f);

            stopAnimationTime = false;
            stopGamePlayTime = false;

            StartMovie();
        }

        private void Update()
        {
            gameplayTimeline.Update();
            animationTimeline.Update();
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
                set { currentTime = value; }
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

            public void StopTime()
            {
                scale = 0;
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
        public void StartMovie()
        {
            mapCreator.FreeAllPool();
            mapCreator.Restart();
            StartCoroutine(FirstMovie());
        }

        public void SetRunnerControl(bool value)
        {
            runnerCache.CanControl = value;
        }

        private IEnumerator FirstMovie()
        {
            runnerCache.CanControl = false;
            movieManager.OnPlayingMovie = true;
            stopAnimationTime = true;
            runnerCache.HealthUp(1.0f);

            movieManager.StartMovie();

            while (movieManager.OnPlayingMovie == true)
            {
                yield return null;
            }

            runnerCache.CanControl = true;
            stopAnimationTime = false;

            runner.GetComponent<RunnerAnimation>().StartAnimation();
        }

        public void EndMovie()
        {
            movieManager.StartEndMovie();
        }

        public void ClearBlackBoxs()
        {
            movieManager.StopEndMovie();
            movieManager.ClearBlackBoxs();
        }

        public void GameOver()
        {
            EndMovie();
            stopAnimationTime = true;
            score.text = "SCORE : " + Super.instance.GameplayTimeline.CurrentTime;
            uiManager.GameOver();
        }
    }
}
    #endregion