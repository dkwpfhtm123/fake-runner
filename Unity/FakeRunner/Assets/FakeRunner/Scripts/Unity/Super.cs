using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Fake.FakeRunner.Unity
{
    public class Super : MonoBehaviour
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

        private static Super instance;
        private Runner runnerCache;
        private bool stopAnimation;

        [SerializeField]
        private Timeline gameplayTimeline;
        [SerializeField]
        private Timeline animationTimeline;
        #endregion

        public Timeline GameplayTimeline
        {
            get { return gameplayTimeline; }
        }

        public Timeline AnimationTimeline
        {
            get { return animationTimeline; }
        }

        public bool StopAnimation
        {
            set { stopAnimation = value; }
            get { return stopAnimation; }
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

            stopAnimation = false;

            StartMovie();
        }

        private void Update()
        {
            gameplayTimeline.Update();
            animationTimeline.Update();
        }

        [System.Serializable]
        public class Timeline
        {
            private float currentTime;
            [SerializeField]
            private float scale;
            private float deltaTime;

            public float Scale
            {
                get { return scale; }
                set
                {
                    if (value >= 0.0f)
                        scale = value;
                }
            }

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
        }

        public void StartMovie()
        {
            StartCoroutine(FirstMovie());
        }

        public void SetRunnerControl(bool check)
        {
            runnerCache.CanControl = check;
        }

        private IEnumerator FirstMovie()
        {
            runnerCache.CanControl = false;
            movieManager.OnPlayingMovie = true;
            stopAnimation = true;
            runnerCache.HealthUp(1.0f);

            movieManager.StartMovie();

            while (movieManager.OnPlayingMovie == true)
            {
                yield return null;
            }

            runnerCache.CanControl = true;
            stopAnimation = false;

            runner.GetComponent<RunnerAnimation>().StartAnimation(); 
        }

        public void GameOver()
        {
            stopAnimation = true;
            score.text = "SCORE : " + Super.instance.GameplayTimeline.CurrentTime;
            uiManager.GameOver();
        }
    }
}