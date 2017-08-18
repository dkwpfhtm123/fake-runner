using UnityEngine;
using System.Collections.Generic;

namespace Fake.FakeRunner.Unity
{
    public class GameObjectPool
    {
        #region Fields
        private List<GameObject> freeObjects;
        private List<GameObject> allocateObjects;
        private GameObject prefab;
        private GameObject parent;
        #endregion

        public GameObjectPool(GameObject prefab, GameObject parent, string parentName)
        {
            freeObjects = new List<GameObject>();
            allocateObjects = new List<GameObject>();
            this.prefab = prefab;
            this.parent = MonoBehaviour.Instantiate(parent);
            this.parent.name = parentName;
        }

        private GameObject CreateFreeGameObject()
        {
            var createdGameObject = MonoBehaviour.Instantiate(prefab, parent.transform);
            createdGameObject.SetActive(false);
            return createdGameObject;
        }

        public GameObject Allocate()
        {
            if (freeObjects.Count == 0)
                freeObjects.Add(CreateFreeGameObject());

            var allocatedFreeObject = freeObjects[freeObjects.Count - 1];
            allocatedFreeObject.SetActive(true);
            allocateObjects.Add(allocatedFreeObject);
            freeObjects.Remove(allocatedFreeObject);

            return allocatedFreeObject;
        }

        public void Free(GameObject gameObject)
        {
            gameObject.SetActive(false);
            freeObjects.Add(gameObject);
            allocateObjects.Remove(gameObject);
        }
    }
}