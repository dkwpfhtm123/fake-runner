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
        #endregion

        public Vector3 Offset
        {
            get { return offset; }
            set { offset = value; }
        }

        private void Awake()
        {
            transformCache = GetComponent<Transform>();

            if (runner != null)
            {
                runnerTransformCache = runner.GetComponent<Transform>();
                runner.PositionChanged += OnRunnerPositionChanged;
            }
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

                if (position.y < 4.5f)
                    position.y = 4.5f;
                else if (position.y > 8.0f)
                    position.y = 8.0f;

                if (position.x < -0.4f)
                    position.x = -0.4f;

                transformCache.localPosition = position;
            }
        }
    }
}