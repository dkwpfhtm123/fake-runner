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
            cameraCache = GetComponent<Camera>();
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

                UpdateHealthPacks(section - 1, section + 1);

                sectionChange = false;
            }
        }

        private GameObject CreateFreeObject(GameObject freeObject)
        {
            var createdObject = Instantiate(freeObject);
            createdObject.SetActive(false);

            return createdObject;
        }

#region Tiles
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

                allocatedTiles.Clear();

                if (section < 0)
                {
                    StartSectionTile(section);
                    UpdateTiles(section, section + 1);
                }
                else if (section > 10)
                {
                    EndSectionTile(section);
                    UpdateTiles(section - 1, section);
                }
                else
                {
                    UpdateTiles(section - 1, section + 1);
                }
            }
        }

        private GameObject AllocateTile(GameObject tile)
        {
            while (true)
            {
                if (freeTiles.Count == 0)
                {
                    freeTiles.Add(CreateFreeObject(tile));
                    // 리스트에 추가
                }

                freeTiles[0].GetComponent<SpriteRenderer>().sprite = tile.GetComponent<SpriteRenderer>().sprite;
                freeTiles[0].SetActive(true);
                freeTiles[0].transform.parent = tileParent.transform;
                allocatedTiles.Add(freeTiles[freeTiles.Count]);
                freeTiles.Remove(freeTiles[freeTiles.Count]);

                return freeTiles[freeTiles.Count];
            }
        }

        private void StartSectionTile(int startSection)
        {
            for (int k = 0; k < 10; k++)
            {
                PlaceTile(startSection * 10 + k, 0, dirtTile[Random.Range(0, dirtTile.Count - 1)]);
            }

            for (int k = 0; k < 10; k++)
            {
                PlaceTile(startSection * 10, k, goldTile);
            }

            // Pipe
            PlaceTile(-8, 1, goldTile);
            PlaceTile(-8, 2, goldTile);
            PlaceTile(-9, 1, goldTile);
            PlaceTile(-9, 2, goldTile);
        }

        private void EndSectionTile(int endSection)
        {
            for (int k = 0; k < 10; k++)
            {
                PlaceTile(endSection * 10 + k, 0, dirtTile[Random.Range(0, dirtTile.Count - 1)]);
            }

            for (int k = 0; k < 10; k++)
            {
                PlaceTile(endSection * 10 + 9, k, goldTile);
            }
        }

        private void UpdateTiles(int minSection, int maxSection)
        {
            List<Vector2> check = new List<Vector2>();

            for (int i = minSection; i <= maxSection; i++)
            {
                random = new System.Random(i);

                for (int k = 0; k < 15; k++)
                {
                    var x = random.Next(i * 10 + 1, (i + 1) * 10);
                    var y = random.Next(0, 10);

                    if (check.Contains(new Vector2(x, y)) == false)
                    {
                        check.Add(new Vector2(x, y));
                        PlaceTile(x, y, goldTile);
                    }
                }

                for (int k = 0; k < 10; k++)
                {
                    PlaceTile(i * 10 + k, 0, dirtTile[Random.Range(0, dirtTile.Count)]);
                }
            }
        }

        private void PlaceTile(int x, int y, GameObject tile)
        {
            var createdTile = AllocateTile(tile);

            if (createdTile.GetComponent<TileAnimation>() != null)
                StartCoroutine(createdTile.GetComponent<TileAnimation>().DoAnimateTile());

            createdTile.transform.localPosition = new Vector3(x, y, 0);
        }
        #endregion

        private GameObject AllocateHealthPacks(GameObject healthPack)
        {
            while (true)
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

                        PlaceHealthPacks(x, y);
                    }
                }
            }
        }

        private void PlaceHealthPacks(int x, int y)
        {
            AllocateHealthPacks(healthPackPrefab);

            var pack = allocatedHealthPacks[allocatedHealthPacks.Count - 1];
            pack.transform.localPosition = new Vector3(x, y);
        }
    }
}