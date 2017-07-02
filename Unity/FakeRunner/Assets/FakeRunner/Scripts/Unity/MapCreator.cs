using UnityEngine;
using System.Collections.Generic;

namespace Fake.FakeRunner.Unity
{
    public class MapCreator : MonoBehaviour
    {
        #region Fields
        [SerializeField]
        private GameObject goldTile;
        [SerializeField]
        private List<GameObject> dirtTile;
        [SerializeField]
        private Runner runner;
        [SerializeField]
        private GameObject tileParent;
        [SerializeField]
        private GameObject healthPackParent;
        [SerializeField]
        private GameObject healthPackPrefab;

        private Transform transformCache;
        private List<GameObject> freeTiles;
        private List<GameObject> allocatedTiles;
        private List<GameObject> freeHealthPacks;
        private List<GameObject> allocatedHealthPacks;
        private List<Vector2> objectPosition;
        private List<int> healthPackSections;

        private Camera cameraCache;
        private System.Random random;
        private int section;
        private int oldSection;
        private bool sectionChange;

        public List<GameObject> AllocatedTiles
        {
            get { return allocatedTiles; }
        }

        public List<GameObject> HealthPacks
        {
            get { return allocatedHealthPacks; }
        }
        #endregion

        private void Start()
        {
            transformCache = GetComponent<Transform>();
            freeTiles = new List<GameObject>();
            allocatedTiles = new List<GameObject>();
            allocatedHealthPacks = new List<GameObject>();
            freeHealthPacks = new List<GameObject>();
            healthPackSections = new List<int>();
            objectPosition = new List<Vector2>();
            cameraCache = GetComponent<Camera>();
            random = new System.Random();
            oldSection = -100;
            sectionChange = false;
        }

        private void Update()
        {
            if (Super.Instance.StopAnimation == false)
            {
                section = Mathf.FloorToInt(transformCache.localPosition.x / 10);

                if (oldSection != section)
                {
                    sectionChange = true;
                    oldSection = section;
                }

                UpdateTiles();

                sectionChange = false;
            }
        }

        private void UpdateTiles()
        {
            if (sectionChange == true)
            {
                foreach (var tile in allocatedTiles)
                {
                    if (tile.GetComponent<TileJump>() != null)
                        tile.GetComponent<TileJump>().StopTileJumping();
                }

                for (int i = 0; i < allocatedTiles.Count; i++)
                {
                    var tile = allocatedTiles[i];
                    tile.SetActive(false);
                    freeTiles.Add(tile);
                }

                for (int i = 0; i < allocatedHealthPacks.Count; i++)
                {
                    var healthPack = allocatedHealthPacks[i];
                    healthPack.SetActive(false);
                    freeHealthPacks.Add(healthPack);
                }

                allocatedTiles.Clear();
                allocatedHealthPacks.Clear();
                objectPosition.Clear();

                if (section < 0)
                {
                    StartSectionTile(section);
                    UpdateObject(section, section + 1);
                }
                else if (section > 10)
                {
                    EndSectionTile(section);
                    UpdateObject(section - 1, section);
                }
                else
                {
                    UpdateObject(section - 1, section + 1);
                }
            }
        }

        private void StartSectionTile(int startSection)
        {
            for (int k = 0; k < 10; k++)
            {
                PlaceObject(startSection * 10 + k, 0, dirtTile[random.Next(0, dirtTile.Count)]);
            }

            for (int k = 0; k < 10; k++)
            {
                PlaceObject(startSection * 10, k, goldTile);
            }

            // Pipe
            PlaceObject(-8, 1, goldTile);
            PlaceObject(-8, 2, goldTile);
            PlaceObject(-9, 1, goldTile);
            PlaceObject(-9, 2, goldTile);
        }

        private void EndSectionTile(int endSection)
        {
            for (int k = 0; k < 10; k++)
            {
                PlaceObject(endSection * 10 + k, 0, dirtTile[random.Next(0, dirtTile.Count)]);
            }

            for (int k = 0; k < 10; k++)
            {
                PlaceObject(endSection * 10 + 9, k, goldTile);
            }
        }

        private void UpdateObject(int minSection, int maxSection)
        {
            for (int i = minSection; i <= maxSection; i++)
            {
                random = new System.Random(i);

                for (int k = 0; k < 15; k++)
                {
                    var position = ReturnRandomPosition(i);

                    PlaceObject((int)position.x, (int)position.y, goldTile);
                }

                for (int k = 0; k < 2; k++)
                {
                    var position = ReturnRandomPosition(i);

                    PlaceObject((int)position.x, (int)position.y, healthPackPrefab);
                }

                for (int k = 0; k < 10; k++)
                {
                    PlaceObject(i * 10 + k, 0, dirtTile[random.Next(0, dirtTile.Count)]);
                }
            }
        }

        private Vector2 ReturnRandomPosition(int section)
        {
            random = new System.Random(section);

            while (true)
            {
                var x = random.Next(section * 10 + 1, (section + 1) * 10);
                var y = random.Next(1, 10);

                if (objectPosition.Contains(new Vector2(x, y)) == false)
                {
                    objectPosition.Add(new Vector2(x, y));
                    return new Vector2(x, y);
                }
            }
        }

        private void PlaceObject(int x, int y, GameObject obj)
        {
            var createdObject = obj;

            if (createdObject == healthPackPrefab)
                createdObject = AllocateHealthPacks(obj);
            else
                createdObject = AllocateTile(obj);

            if (createdObject.GetComponent<TileAnimation>() != null)
                StartCoroutine(createdObject.GetComponent<TileAnimation>().DoAnimateTile());

            createdObject.transform.localPosition = new Vector3(x, y, 0);
        }


        private GameObject CreateFreeObject(GameObject freeObject)
        {
            var createdObject = Instantiate(freeObject);
            createdObject.SetActive(false);

            return createdObject;
        }

        private GameObject AllocateTile(GameObject tile)
        {
            while (true)
            {
                if (freeTiles.Count == 0)
                    freeTiles.Add(CreateFreeObject(tile));

                var allocatedFreeTile = freeTiles[freeTiles.Count - 1];

                allocatedFreeTile.SetActive(true);
                allocatedFreeTile.transform.parent = tileParent.transform;
                allocatedTiles.Add(allocatedFreeTile);
                freeTiles.Remove(allocatedFreeTile);

                return allocatedFreeTile;
            }
        }

        private GameObject AllocateHealthPacks(GameObject healthPack)
        {
            if (freeHealthPacks.Count == 0)
                freeHealthPacks.Add(CreateFreeObject(healthPack));

            var allocatedHelathPack = freeHealthPacks[freeHealthPacks.Count - 1];
            allocatedHelathPack.SetActive(true);
            allocatedHelathPack.transform.parent = healthPackParent.transform;
            allocatedHealthPacks.Add(allocatedHelathPack);
            freeHealthPacks.Remove(allocatedHelathPack);

            return allocatedHelathPack;
        }

        private void UpdateHealthPacks(int minSection, int maxSection)
        {
            if (sectionChange == true)
            {
                for (int i = 0; i < allocatedHealthPacks.Count; i++)
                {
                    var healthPack = allocatedHealthPacks[i];
                    healthPack.SetActive(false);
                    freeHealthPacks.Add(healthPack);
                }

                allocatedHealthPacks.Clear();
                healthPackSections.Clear();

                for (int i = minSection; i <= maxSection; i++)
                {
                    if (healthPackSections.Contains(i) == false) // 타일과 겹치지 않게 수정.
                    {
                        healthPackSections.Add(i);

                        random = new System.Random(i);

                        var x = random.Next(i * 10 + 1, (i + 1) * 10);
                        var y = random.Next(1, 10);

                        PlaceObject(x, y, healthPackPrefab);
                    }
                }
            }
        }
    }
}