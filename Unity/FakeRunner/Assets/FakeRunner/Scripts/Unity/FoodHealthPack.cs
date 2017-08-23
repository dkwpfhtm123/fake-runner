using UnityEngine;

namespace Fake.FakeRunner.Unity
{
    public class FoodHealthPack : Food
    {
        public override void Eat(Runner runner)
        {
            runner.ChangeHealth(0.2f);
            Hide();
        }

        private void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}