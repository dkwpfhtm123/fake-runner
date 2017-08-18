using UnityEngine;

namespace Fake.FakeRunner.Unity
{
    public class Kumba : MonoBehaviour
    {
        #region Fields
        [SerializeField]
        private Sprite[] parachuteKumbaAnimation;

        private MapCreator mapCreator;

        private Transform transformCache;
        private SpriteRenderer spriteRendererCache;
        private Vector3 firstPosition;
        private float duration;
        private float startTime;

        public Vector3 FirstPosition
        {
            set { firstPosition = value; }
        }

        public MapCreator MapCreator
        {
            set { mapCreator = value; }
        }
        #endregion

        private void Start()
        {
            transformCache = GetComponent<Transform>();
            spriteRendererCache = GetComponent<SpriteRenderer>();

            firstPosition = transformCache.localPosition;
            startTime = Super.Instance.AnimationTimeline.CurrentTime;
            duration = 0.5f;
        }

        private void Update()
        {
            var elapsedTime = Super.Instance.AnimationTimeline.CurrentTime - startTime;
            var time = elapsedTime / duration * 0.5f;

            transformCache.localPosition = new Vector3(firstPosition.x + Mathf.Sin(time * Mathf.PI), transformCache.localPosition.y - 0.05f);

            Sprite nextSprite;
            var sinValue = Mathf.Sin(time * Mathf.PI);

            if (Mathf.Abs(sinValue) <= 0.3f)
                nextSprite = parachuteKumbaAnimation[1];
            else
            {
                nextSprite = parachuteKumbaAnimation[0];

                if (Mathf.Sign(sinValue) < 0)
                    spriteRendererCache.flipX = true;
                else
                    spriteRendererCache.flipX = false;
            }

            spriteRendererCache.sprite = nextSprite;

            if (transformCache.localPosition.y < -2.0f)
                KumbaFree();
        }

        public void KumbaFree()
        {
            mapCreator.ParachuteKumbaPool.Free(gameObject);
        }

        public void ChangeStartPosition()
        {
            firstPosition = transform.localPosition;
        }
    }
}