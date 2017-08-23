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

        private void Start()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            ChangeSprite();
        }

        private void ChangeSprite()
        {
            var value = Random.Range(0, sprites.Length);
            spriteRenderer.sprite = sprites[value];
        }
    }
}