using UnityEngine;
using System.Collections;

namespace Fake.FakeRunner.Unity
{
    public class RunnerAnimation : MonoBehaviour
    {
        #region Fields
        [SerializeField]
        private GameObject runner;
        [SerializeField]
        private Sprite[] rightWalk;
        [SerializeField]
        private Sprite[] rightRoll;
        [SerializeField]
        private Sprite rightIdle;

        private SpriteRenderer spriteRendererCache;
        #endregion

        private void Start()
        {
            spriteRendererCache = GetComponent<SpriteRenderer>();
            StartAnimation();
        }

        public void StartAnimation()
        {
            StartCoroutine(DoAnimation());
        }

        private IEnumerator DoAnimation()
        {
            var time = 0.0f;
            var lookDirection = false;

            while (true)
            {
                if (spriteRendererCache == null)
                    spriteRendererCache = GetComponent<SpriteRenderer>();

                var runnerCache = runner.GetComponent<Runner>();
                var velocity = runnerCache.Velocity;
                var maxSpeed = runnerCache.MaxSpeed;

                time += (Mathf.Abs(velocity.x) / maxSpeed) * Super.Instance.GameplayTimeline.DeltaTime * 10;

                if (velocity.x == 0)
                    spriteRendererCache.flipX = lookDirection;
                else
                {
                    lookDirection = Mathf.Sign(velocity.x) < 0;
                    spriteRendererCache.flipX = lookDirection;
                }

                if (Mathf.Abs(velocity.x) < 0.5f)
                    spriteRendererCache.sprite = rightIdle;
                else if (Mathf.Abs(velocity.x) < 8.0f)
                    spriteRendererCache.sprite = rightWalk[Mathf.FloorToInt(time) % rightWalk.Length];
                else
                    spriteRendererCache.sprite = rightRoll[Mathf.FloorToInt(time * 2) % rightRoll.Length];

                yield return null;
            }
        }
    }
}