using UnityEngine;
using System.Collections.Generic;

namespace Fake.FakeRunner.Unity
{
    public class MapBase
    {
        #region Fields
        private List<GameObject> freeObjects;
        private List<GameObject> allocatedObjects;
        private GameObject prefab;
        private GameObject parent;
        #endregion

        public MapBase(GameObject prefab, GameObject parent)
        {
            freeObjects = new List<GameObject>();
            allocatedObjects = new List<GameObject>();
            this.prefab = prefab;
            this.parent = parent;
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
            allocatedObjects.Add(allocatedFreeObject);
            freeObjects.Remove(allocatedFreeObject);

            return allocatedFreeObject;
        }

        public void Free(GameObject gameObject)
        {
            gameObject.SetActive(false);
            freeObjects.Add(gameObject);
            allocatedObjects.Remove(gameObject);
        }
    }
}