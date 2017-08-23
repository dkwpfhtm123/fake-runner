using System.Collections.Generic;
using UnityEngine;

namespace Fake.FakeRunner.Unity
{
    public class Map : MapBase
    {
        #region Fields
        private List<GameObject> allocatedObjects;
        #endregion

        public List<GameObject> AllocatedObjects
        {
            get { return allocatedObjects; }
        }

        public Map(GameObject prefab, GameObject parent) : base(prefab, parent)
        {
            allocatedObjects = new List<GameObject>();
        }

        public new GameObject Allocate()
        {
            var createdObjects = base.Allocate();
            allocatedObjects.Add(createdObjects);
            return createdObjects;
        }

        public new void Free(GameObject gameObject)
        {
            allocatedObjects.Remove(gameObject);
            base.Free(gameObject);
        }

        public void PlaceObject(Vector3 position)
        {
            var createdObject = Allocate();

            var tileAnimation = createdObject.GetComponent<TileAnimation>();
            if (tileAnimation != null)
                tileAnimation.StartAnimation();

            createdObject.transform.localPosition = position;
        }
    }
}
