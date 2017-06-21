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

        public Vector3 Offset
        {
            get { return offset; }
            set { offset = value; }
        }

        private Transform transformCache;
        private Transform runnerTransformCache;
        #endregion

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
                transformCache.localPosition = runnerTransformCache.localPosition + offset;
        }
    }
}