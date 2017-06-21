using System.Collections;
using UnityEngine;

namespace Fake.FakeRunner.Unity
{
    public class TileJump : MonoBehaviour
    {
        private Transform transformCache;

        private IEnumerator tileJumping;

        void Start()
        {
            transformCache = GetComponent<Transform>();
            tileJumping = null;
        }

        public void StartTileJumping()
        {
            if (tileJumping == null)
            {
                tileJumping = TileJumping();
                StartCoroutine(tileJumping);
            }
        }

        internal void StopTileJumping()
        {
            if (tileJumping != null)
            {
                StopCoroutine(tileJumping);
                tileJumping = null;
            }
        }

        private IEnumerator TileJumping()
        {
            SoundManager.Instance.PlayCoinSound();

            var oldPosition = transformCache.localPosition;

            var duration = 0.5f;
            var startTime = Super.Instance.AnimationTimeline.CurrentTime;
            var endTime = startTime + duration;

            while (Super.Instance.AnimationTimeline.CurrentTime < endTime)
            {
                var elapsedTime = Super.Instance.AnimationTimeline.CurrentTime - startTime;
                var time = elapsedTime / duration;

                transformCache.localPosition = new Vector3(transformCache.localPosition.x, oldPosition.y + Mathf.Sin(time * Mathf.PI) * 2.0f);
                yield return null;
            }

            transformCache.localPosition = oldPosition;
            tileJumping = null;
        }
    }
}