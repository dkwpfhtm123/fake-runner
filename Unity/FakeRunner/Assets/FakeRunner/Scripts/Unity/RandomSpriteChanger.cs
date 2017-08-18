using UnityEngine;

namespace Fake.FakeRunner.Unity
{
    public class RandomSpriteChanger : MonoBehaviour
    {
        #region Fields
        [SerializeField]
        private Sprite[] sprites;

        private SpriteRenderer spriteRenderer;
        #endregion

        void Start()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();

            ChangeSprite();
        }

        private void ChangeSprite()
        {
            var random = new System.Random(Random.Range(1, 10));

            spriteRenderer.sprite = sprites[random.Next(0, sprites.Length)];
        }
    }
}