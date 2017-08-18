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
        private IEnumerator animationCoroutine;
        #endregion

        void Start()
        {
            spriteRendererCache = GetComponent<SpriteRenderer>();
            StartAnimation();
        }

        public void StartAnimation()
        {
            StartCoroutine(runnerAnimation());
        }

        private IEnumerator runnerAnimation()
        {
            var time = 0.0f;
            var temp = false;

            while (true)
            {
                if (spriteRendererCache == null)
                    spriteRendererCache = GetComponent<SpriteRenderer>();

                var velocity = runner.GetComponent<Runner>().Velocity;
                var maxSpeed = runner.GetComponent<Runner>().MaxSpeed;

                time += (Mathf.Abs(velocity.x) / maxSpeed) * Super.Instance.GameplayTimeline.DeltaTime * 10;

                if (Mathf.Sign(velocity.x) < 0)
                {
                    temp = true;
                    spriteRendererCache.flipX = temp;
                }
                else if (velocity.x == 0)
                    spriteRendererCache.flipX = temp;
                else
                {
                    temp = false;
                    spriteRendererCache.flipX = temp;
                }

                if (Mathf.Abs(velocity.x) < 0.01f)
                    spriteRendererCache.sprite = rightIdle;
                else if (Mathf.Abs(velocity.x) < 7.0f)
                    spriteRendererCache.sprite = rightWalk[Mathf.FloorToInt(time) % rightWalk.Length];
                else
                    spriteRendererCache.sprite = rightRoll[Mathf.FloorToInt(time * 2) % rightRoll.Length];

                yield return null;
            }
        }
    }
}