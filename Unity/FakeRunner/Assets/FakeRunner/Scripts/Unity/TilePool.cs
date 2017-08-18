using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fake.FakeRunner.Unity
{
    public class TilePool : GameObjectPool
    {
        #region Fields
        private List<GameObject> allocateObjects;

        public List<GameObject> AllocateObjects
        {
            get { return allocateObjects; }
        }
        #endregion

        public TilePool(GameObject prefab, GameObject parent, string parentName) : base(prefab, parent, parentName)
        {
            allocateObjects = new List<GameObject>();
        }

        public new GameObject Allocate()
        {
            var createdObjects = base.Allocate();
            allocateObjects.Add(createdObjects);
            return createdObjects;
        }

        public new void Free(GameObject gameObject)
        {
            allocateObjects.Remove(gameObject);
            base.Free(gameObject);
        }
    }
}
