using UnityEngine;

namespace Fake.FakeRunner.Unity
{
    public class FoodHealthPack : Food
    {
        public override void Eat(Runner runner)
        {
            Debug.Log("getit");
            runner.HealthUp(0.2f);
            gameObject.SetActive(false);
        }
    }
}