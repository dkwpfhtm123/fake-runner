using UnityEngine;

namespace Fake.FakeRunner.Unity
{
    public abstract class Food : MonoBehaviour
    {
        public abstract void Eat(Runner runner);
    }
}