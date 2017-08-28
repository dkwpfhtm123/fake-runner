using UnityEngine;

namespace Fake.FakeRunner.Unity
{
    public class Kumba : MonoBehaviour
    {
        #region Fields
        [SerializeField]
        private Sprite rightParachuteKumbaSprite;
        [SerializeField]
        private Sprite middleParachuteKumbaSprite;

        private MapCreator mapCreator;
        private Transform transformCache;
        private SpriteRenderer spriteRendererCache;
        private Vector3 firstPosition;
        private float duration;
        private float startTime;
        #endregion

        private void Awake()
        {
            transformCache = GetComponent<Transform>();
            spriteRendererCache = GetComponent<SpriteRenderer>();

            startTime = Super.Instance.AnimationTimeline.CurrentTime;
            duration = 0.5f;
        }

        private void Update()
        {
            MoveKumba();

            if (transformCache.localPosition.y < -2.0f)
                Free();
        }

        public void Free()
        {
            mapCreator.ParachueKumbas.Free(gameObject);
        }

        public void Initialize(Vector3 position, MapCreator mapCreator)
        {
            this.mapCreator = mapCreator;
            transformCache.localPosition = position;
            firstPosition = transformCache.localPosition;
        }

        private void MoveKumba()
        {
            var elapsedTime = Super.Instance.GameplayTimeline.CurrentTime - startTime;
            var time = elapsedTime / duration * 0.5f;
            transformCache.localPosition = new Vector3(firstPosition.x + Mathf.Sin(time * Mathf.PI), transformCache.localPosition.y - 3.0f * Super.Instance.GameplayTimeline.DeltaTime);

            var nextSprite = rightParachuteKumbaSprite;
            var sinValue = Mathf.Sin(time * Mathf.PI);

            if (Mathf.Abs(sinValue) <= 0.3f)
                nextSprite = middleParachuteKumbaSprite;
            else
                spriteRendererCache.flipX = Mathf.Sign(sinValue) < 0;

            spriteRendererCache.sprite = nextSprite;
        }
    }
}