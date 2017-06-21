using UnityEngine;
using System.Collections;

namespace Fake.FakeRunner.Unity
{
    public class RunnerAnimation : MonoBehaviour
    {
        #region Fields
        [SerializeField, Range(0.001f, 1.0f)]
        private float duration = 0.2f;
        [SerializeField]
        private Sprite[] roll;

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

            while (Super.Instance.StopAnimation == false)
            {
                if (spriteRendererCache == null)
                    spriteRendererCache = GetComponent<SpriteRenderer>();

                time += Super.Instance.GameplayTimeline.DeltaTime;

                var interval = duration * Super.Instance.GameplayTimeline.Scale;
                var tileLength = roll.Length * Super.Instance.GameplayTimeline.Scale;

                spriteRendererCache.sprite = roll[(int)(System.Math.Truncate(time / interval) % tileLength)];
                yield return null;
            }
        }
    }
}