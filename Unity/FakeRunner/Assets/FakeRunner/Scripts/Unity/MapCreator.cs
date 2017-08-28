using System.Collections.Generic;
using UnityEngine;


namespace Fake.FakeRunner.Unity
{
    public class MapCreator : MonoBehaviour
    {
        #region Fields
        [SerializeField]
        private GameObject goldTilePrefab;
        [SerializeField]
        private GameObject dirtTilePrefab;
        [SerializeField]
        private Runner runner;
        [SerializeField]
        private GameObject healthPackPrefab;
        [SerializeField]
        private GameObject parachuteKumbaPrefab;
        [SerializeField]
        private GameObject kumbaParent;
        [SerializeField]
        private GameObject goldTileParent;
        [SerializeField]
        private GameObject dirtTileParent;
        [SerializeField]
        private GameObject healthPackParent;

        private Transform transformCache;

        private Map goldTiles;
        private Map dirtTiles;
        private Map healthPacks;
        private Map parachuteKumbas;
        private List<Vector2> mapPositions;
        private int currentSection;
        private int oldSection;
        private bool sectionChange;
        private float difficulty;
        private float countDown;
        private int randomMapSeed;
        #endregion

        public MapBase ParachueKumbas
        {
            get { return parachuteKumbas; }
        }

        private void Start()
        {
            transformCache = GetComponent<Transform>();

            goldTiles = new Map(goldTilePrefab, goldTileParent);
            healthPacks = new Map(healthPackPrefab, healthPackParent);
            dirtTiles = new Map(dirtTilePrefab, dirtTileParent);
            parachuteKumbas = new Map(parachuteKumbaPrefab, kumbaParent);

            mapPositions = new List<Vector2>();
            oldSection = -100;
            sectionChange = false;

            difficulty = 5.0f;
            countDown = difficulty;
            randomMapSeed = Random.Range(0, 10);
        }

        private void Update()
        {
            currentSection = Mathf.FloorToInt(transformCache.localPosition.x / 10);

            if (oldSection != currentSection)
            {
                sectionChange = true;
                oldSection = currentSection;
            }

            UpdateTiles();

            sectionChange = false;

            RiseDifficulty();
        }

        public void PlaceKumba(int count, int section)
        {
            for (int k = 0; k < count; k++)
            {
                var randomValue = Random.Range(0, 10);
                var position = AllocateRandomVector(randomValue, (section - 1) * 10 + 1, (section + 1) * 10 + 10, 15, 20);
                var createdobject = parachuteKumbas.Allocate();
                createdobject.GetComponent<Kumba>().Initialize(position, this);
            }
        }

        public void Initialize()
        {
            FreeAllTiles();
            FreeKumbas();
            mapPositions.Clear();
            difficulty = 5.0f;
            oldSection = -100;
        }

        private void RiseDifficulty()
        {
            countDown -= Super.Instance.GameplayTimeline.DeltaTime;

            if (countDown < 0.0f)
            {
                if (difficulty > 2.0f)
                    difficulty *= 0.95f;

                countDown = difficulty;
                PlaceKumba(6, currentSection);
            }
        }

        private void UpdateTiles()
        {
            if (sectionChange)
            {
                FreeAllTiles();
                mapPositions.Clear();

                if (currentSection < 1)
                    StartSectionTile(currentSection);

                PlaceSection(currentSection - 1, currentSection + 1);
                HideTiles(currentSection);
            }
        }

        private void FreeAllTiles()
        {
            var poolList = new List<Map>();
            poolList.Add(goldTiles);
            poolList.Add(dirtTiles);
            poolList.Add(healthPacks);

            foreach (var pool in poolList)
            {
                var temp = new List<GameObject>(pool.AllocatedObjects);

                foreach (var gameObject in temp)
                    pool.Free(gameObject);
            }
        }

        private void FreeKumbas()
        {
            var temp = new List<GameObject>(parachuteKumbas.AllocatedObjects);

            foreach (var gameObject in temp)
                parachuteKumbas.Free(gameObject);
        }

        private void StartSectionTile(int startSection)
        {
            // Pipe
            goldTiles.PlaceObject(new Vector2(-8, 1));
            goldTiles.PlaceObject(new Vector2(-8, 2));
            goldTiles.PlaceObject(new Vector2(-9, 1));
            goldTiles.PlaceObject(new Vector2(-9, 2));
        }

        private void HideTiles(int section)
        {
            for (int k = 0; k < 20; k++)
            {
                goldTiles.PlaceObject(new Vector3((section-1) * 10, k));
            }
        }

        private void PlaceSection(int minSection, int maxSection)
        {
            for (int i = minSection; i <= maxSection; i++)
            {
                PlaceObjectRandomPosiiton(i, 15, goldTiles);
                PlaceObjectRandomPosiiton(i, 2, healthPacks);
                PlaceDirtTiles(i, 10, dirtTiles);
            }
        }

        private void PlaceObjectRandomPosiiton(int section, int count, Map pool)
        {
            for (int k = 0; k < count; k++)
            {
                var position = AllocateRandomVector(section, section * 10 + 1, section * 10 + 10, 1, 10);

                pool.PlaceObject(position);
            }
        }

        private void PlaceDirtTiles(int section, int count, Map pool)
        {
            for (int k = 0; k < count; k++)
            {
                pool.PlaceObject(new Vector2(section * 10 + k, 0));
            }
        }

        private Vector2 AllocateRandomVector(int section, int minX, int maxX, int minY, int maxY)
        {
            var random = new System.Random(section + randomMapSeed);

            while (true)
            {
                var x = random.Next(minX, maxX);
                var y = random.Next(minY, maxY);
                var result = new Vector2(x, y);

                if (mapPositions.Contains(result) == false)
                {
                    mapPositions.Add(result);
                    return result;
                }
            }
        }

    }
}