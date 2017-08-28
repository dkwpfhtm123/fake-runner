using UnityEngine;

namespace Fake.FakeRunner.Unity
{
    public class RunnerTracker : MonoBehaviour
    {
        #region Fields
        [SerializeField]
        private Runner runner;
        [SerializeField]
        private Vector3 offset;

        private Transform transformCache;
        private Transform runnerTransformCache;
        private float currentSection;
        private float lastSection;
        #endregion

        public Vector3 Offset
        {
            get { return offset; }
            set { offset = value; }
        }

        private void Awake()
        {
            transformCache = GetComponent<Transform>();
            currentSection = 0.0f;
            lastSection = -100.0f;

            if (runner != null)
            {
                runnerTransformCache = runner.GetComponent<Transform>();
                runner.PositionChanged += OnRunnerPositionChanged;
            }
        }

        private void Update()
        {
            currentSection = Mathf.FloorToInt(transformCache.localPosition.x / 10);
        }

        public void Initialize()
        {
            SetPosition(Vector3.zero);
            lastSection = -100.0f;
        }

        private void OnDestroy()
        {
            if (runner != null)
                runner.PositionChanged -= OnRunnerPositionChanged;
        }

        private void OnRunnerPositionChanged(Runner sender)
        {
            if (runner == sender)
            {
                var position = runnerTransformCache.localPosition + offset;

                if (lastSection < currentSection)
                    lastSection = currentSection;

                if (position.y < 4.5f)
                    position.y = 4.5f;
                else if (position.y > 8.0f)
                    position.y = 8.0f;

                if (position.x < lastSection * 10)
                    position.x = lastSection * 10;

                SetPosition(position);
            }
        }

        private void SetPosition(Vector3 position)
        {
            transformCache.localPosition = position;
        }
    }
}