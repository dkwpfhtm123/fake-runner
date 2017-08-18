using UnityEngine;
using System.Collections.Generic;


namespace Fake.FakeRunner.Unity
{
    public class MapCreator : MonoBehaviour
    {
        #region Fields
        [SerializeField]
        private GameObject goldTilePrefab;
        [SerializeField]
        private GameObject dirtTile;
        [SerializeField]
        private Runner runner;
        [SerializeField]
        private GameObject healthPackPrefab;
        [SerializeField]
        private GameObject parachuteKumbaPrefab;
        [SerializeField]
        private GameObject parentObject;
        private Transform transformCache;

        private TilePool goldTilePool;
        private TilePool dirtTilePool;
        private TilePool healthpackPool;
        private GameObjectPool parachuteKumbaPool;
        private List<Vector2> tilePositions;
        private System.Random random;
        private int currentSection;
        private int oldSection;
        private bool sectionChange;
        private float difficulty;
        private float countDown;

        public TilePool GoldTilePool
        {
            get { return goldTilePool; }
        }

        public TilePool HealthpackPool
        {
            get { return healthpackPool; }
        }

        public TilePool DirtTilePool
        {
            get { return dirtTilePool; }
        }

        public GameObjectPool ParachuteKumbaPool
        {
            get { return parachuteKumbaPool; }
        }
        #endregion

        private void Start()
        {
            transformCache = GetComponent<Transform>();

            goldTilePool = new TilePool(goldTilePrefab, parentObject, "GoldTile");
            healthpackPool = new TilePool(healthPackPrefab, parentObject, "HealthPack");
            dirtTilePool = new TilePool(dirtTile, parentObject, "DirtTile");
            parachuteKumbaPool = new GameObjectPool(parachuteKumbaPrefab, parentObject, "Kumba");

            tilePositions = new List<Vector2>();
            random = new System.Random();
            oldSection = -100;
            sectionChange = false;

            countDown = 5.0f;
            difficulty = countDown;
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
            //      test();

            sectionChange = false;

            StageDifficulty();
        }

        private void StageDifficulty()
        {
            countDown -= Super.Instance.GameplayTimeline.DeltaTime;

            if (countDown < 0.0f)
            {
                if (difficulty > 3.0f)
                    difficulty *= 0.9f;

                countDown = difficulty;
                PlaceKumba(6, currentSection);

                Debug.Log(countDown);
            }
        }

        private void UpdateTiles()
        {
            if (sectionChange == true)
            {
                foreach (var tile in goldTilePool.AllocateObjects)
                {
                    if (tile.GetComponent<TileJump>() != null)
                        tile.GetComponent<TileJump>().StopTileJumping();
                }

                FreeAllPool();

                tilePositions.Clear();

                if (currentSection < 0)
                    StartSectionTile(currentSection);

                PlaceSection(currentSection - 1, currentSection + 1);
            }
        }

        public void Restart()
        {
            oldSection = -100;
        }

        public void FreeAllPool()
        {
            var poolList = new List<TilePool>();
            poolList.Add(goldTilePool);
            poolList.Add(dirtTilePool);
            poolList.Add(healthpackPool);


            foreach (var pool in poolList)
            {
                var temp = new List<GameObject>(pool.AllocateObjects);

                foreach (var gameObject in temp)
                    pool.Free(gameObject);
            }
        }

        private void StartSectionTile(int startSection)
        {
            for (int k = 0; k < 10; k++)
                PlaceObject(new Vector2(startSection * 10, k), goldTilePool);

            // Pipe
            PlaceObject(new Vector2(-8, 1), goldTilePool);
            PlaceObject(new Vector2(-8, 2), goldTilePool);
            PlaceObject(new Vector2(-9, 1), goldTilePool);
            PlaceObject(new Vector2(-9, 2), goldTilePool);
        }

        private void PlaceSection(int minSection, int maxSection)
        {
            for (int i = minSection; i <= maxSection; i++)
            {
                PlaceObjectRandomPosiiton(i, 15, goldTilePool);
                PlaceObjectRandomPosiiton(i, 2, healthpackPool);
                PlaceDirtTiles(i, 10, dirtTilePool);
            }
        }

        private void PlaceObjectRandomPosiiton(int section, int count, TilePool pool)
        {
            for (int k = 0; k < count; k++)
            {
                var position = GetRandomPosition(section, section * 10 + 1, section * 10 + 10, 1, 10);

                PlaceObject(position, pool);
            }
        }

        private void PlaceDirtTiles(int section, int count, TilePool pool)
        {
            for (int k = 0; k < count; k++)
            {
                PlaceObject(new Vector2(section * 10 + k, 0), pool);
            }
        }

        public void PlaceKumba(int count, int section)
        {
            for (int k = 0; k < count; k++)
            {
                var position = GetRandomPosition(section, (section - 1) * 10 + 1, (section + 1) * 10 + 10, 10, 15);

                var createdobject = parachuteKumbaPool.Allocate();
                createdobject.transform.localPosition = position;
                createdobject.GetComponent<Kumba>().MapCreator = this;
                createdobject.GetComponent<Kumba>().ChangeStartPosition();
            }
        }

        private Vector2 GetRandomPosition(int section, int minX, int maxX, int minY, int maxY)
        {
            random = new System.Random(section);

            while (true)
            {
                var x = random.Next(/* section * 10 */minX, maxX /*(section + 1) * 10*/);
                var y = random.Next(minY, maxY);

                if (tilePositions.Contains(new Vector2(x, y)) == false)
                {
                    tilePositions.Add(new Vector2(x, y));
                    return new Vector2(x, y);
                }
            }
        }

        private void PlaceObject(Vector3 position, TilePool pool)
        {
            var createdObject = pool.Allocate();

            if (createdObject.GetComponent<TileAnimation>() != null)
                StartCoroutine(createdObject.GetComponent<TileAnimation>().DoAnimateTile());

            createdObject.transform.localPosition = position;
        }
    }
}