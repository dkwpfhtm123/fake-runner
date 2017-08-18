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

            while (true)
            {
                if (spriteRendererCache == null)
                    spriteRendererCache = GetComponent<SpriteRenderer>();

                time += Super.Instance.AnimationTimeline.DeltaTime;

                var interval = 0.4f;
                var tileLength = tileSprites.Length;

                spriteRendererCache.sprite = tileSprites[(int)(System.Math.Truncate(time / interval) % tileLength)];
                yield return null;
            }
        }
    }
}
