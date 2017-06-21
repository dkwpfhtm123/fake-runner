using UnityEngine;
using System.Collections;

namespace Fake.FakeRunner.Unity
{
    public class TileAnimation : MonoBehaviour
    {
        #region Fields
        [SerializeField]
        private Sprite[] tileSprites;

        private SpriteRenderer spriteRendererCache;
        #endregion

        void Start()
        {
            spriteRendererCache = GetComponent<SpriteRenderer>();
        }

        public IEnumerator DoAnimateTile()
        {
            var time = 0.0f;

            while (Super.Instance.StopAnimation == false)
            {
                if (spriteRendererCache == null)
                    spriteRendererCache = GetComponent<SpriteRenderer>();

                time += Super.Instance.GameplayTimeline.DeltaTime;

                var interval = 0.4f * Super.Instance.GameplayTimeline.Scale;
                var tileLength = tileSprites.Length * Super.Instance.GameplayTimeline.Scale;

                spriteRendererCache.sprite = tileSprites[(int)(System.Math.Truncate(time / interval) % tileLength)];
                yield return null;
            }
        }
    }
}
