using UnityEngine;

namespace Fake.FakeRunner.Unity
{
    public class RunnerTracker : MonoBehaviour
    {
        public Runner runner;
        public Vector3 offset;
        private Transform transformCache;
        private Transform runnerTransformCache;

        private void Awake()
        {
            transformCache = GetComponent<Transform>();

            if (runner != null)
            {
                runnerTransformCache = runner.GetComponent<Transform>();
                runner.PositionChanged += OnRunnerPositionChanged;
            }
        }

        private void OnRunnerPositionChanged(Runner sender)
        {
            if (runner == sender)
                transformCache.localPosition = runnerTransformCache.localPosition + offset;
        }
    }
}
