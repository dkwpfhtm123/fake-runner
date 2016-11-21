using UnityEngine;
using System.Collections;

namespace Fake.FakeRunner.Unity
{
    public delegate void RunnerEventHandler(Runner sender);

    public enum RunnerState
    {
        Idle,
        Running,
        Jump,
    }

    public class Runner : MonoBehaviour
    {
        private RunnerState state;
        private Transform transformCache;
        private float maxSpeed;
        private float speed;

        public event RunnerEventHandler PositionChanged;

        private void Awake()
        {
            state = RunnerState.Running;
            transformCache = GetComponent<Transform>();
            maxSpeed = 5.0f;
            speed = maxSpeed;
        }

        private void Update()
        {
            switch (state)
            {
                case RunnerState.Running:
                    OnRunning();
                    break;
            }
        }

        private void OnRunning()
        {
            transformCache.localPosition += Vector3.right * speed * Time.deltaTime;

            if (PositionChanged != null)
                PositionChanged(this);
        }
    }
}
