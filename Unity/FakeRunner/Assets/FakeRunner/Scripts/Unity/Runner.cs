using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Fake.FakeRunner.Unity
{
    public delegate void RunnerEventHandler(Runner sender);

    public class Runner : MonoBehaviour
    {
        #region Fields
        private Vector3 velocity;
        [SerializeField]
        private MapCreator mapCreator;
        [SerializeField]
        private Slider healthBar;
        [SerializeField, Range(0.001f, 1.0f)]
        private float healthDownValue;

        private bool canControl;
        private Transform transformCache;
        private float maxSpeed;
        private int jumpNumber;
        private bool isAirborne;

        public event RunnerEventHandler PositionChanged;

        public Vector3 Velocity
        {
            get { return velocity; }
            set { velocity = value; }
        }

        public bool CanControl
        {
            get { return canControl; }
            set { canControl = value; }
        }
        #endregion

        private void Awake()
        {
            transformCache = GetComponent<Transform>();
            Velocity = Vector3.zero;
            maxSpeed = 5.0f;
            jumpNumber = 0;
            isAirborne = false;
            CanControl = true;
        }

        private void Update()
        {
            if (CanControl == true)
            {
                var horizontal = Input.GetAxisRaw("Horizontal");

                // horizontal 약 0.1 이하정도는 자동으로 0으로 설정하는것이 좋음
                if (Mathf.Abs(horizontal) < 0.2f)
                    horizontal = 0.0f;

                var friction = 3.0f * Super.Instance.GameplayTimeline.DeltaTime;

                if (Mathf.Abs(velocity.x) > friction)
                    velocity += new Vector3(-Mathf.Sign(velocity.x) * friction, 0); // -= 보다 += 로 하고 값에 -를 붙이는게 나음. 힘을 주는것 이니까
                else
                    velocity.x = 0;

                if (Input.GetKeyDown(KeyCode.UpArrow) && jumpNumber < 2)
                {
                    isAirborne = true;
                    velocity.y = 8.0f;
                    jumpNumber++;
                    SoundManager.Instance.PlayJumpSound();
                }
                //    if (velocity.sqrMagnitude < 0.1f * 0.1f) // 벡터.sqrMagnitude = x^2 + y^2 , magnitude = 루트계산 x^2 + y^2 , 루트계산은 느리므로 빼는게 최적화
                velocity += new Vector3(horizontal * 10.0f * Super.Instance.GameplayTimeline.DeltaTime, 0);

                if (isAirborne == true)
                    velocity += new Vector3(0, -9.8f * Super.Instance.GameplayTimeline.DeltaTime);

                if (Mathf.Abs(velocity.x) > maxSpeed)
                    velocity.x = Mathf.Sign(velocity.x) * maxSpeed;

                if (Mathf.Abs(velocity.y) > maxSpeed * 2.0f)
                    velocity.y = Mathf.Sign(velocity.y) * maxSpeed * 1.5f;

                Move();

                if (healthBar.value < 0.00001f)
                    Super.Instance.GameOver();
            }

            if (PositionChanged != null)
                PositionChanged(this);
        }

        public IEnumerator HealthDown()
        {
            var interval = 1.0f * Super.Instance.GameplayTimeline.Scale + Super.Instance.GameplayTimeline.CurrentTime;
            healthDownValue = 0.05f;

            while (true)
            {
                if (interval < Super.Instance.GameplayTimeline.CurrentTime && healthBar.value > 0.0f)
                {
                    interval = 1.0f * Super.Instance.GameplayTimeline.Scale + Super.Instance.GameplayTimeline.CurrentTime;
                    healthBar.value -= healthDownValue;
                }

                yield return null;
            }
        }

        public void HealthUp(float upValue)
        {
            healthBar.value += upValue;

            if (healthBar.value > 1.0f)
                healthBar.value = 1.0f;
        }

        #region Move()
        private void Move()
        {
            #region TileColliderCheck
            var allocatedTiles = mapCreator.AllocatedTiles;

            var oldPosition = new Vector2(transformCache.localPosition.x, transformCache.localPosition.y);
            var currentPosition = oldPosition + (new Vector2(velocity.x, velocity.y) * Super.Instance.GameplayTimeline.DeltaTime);

            var minPoint = new Vector2(-9999.9f, -9999.9f);
            var minNormal = Vector2.zero;
            GameObject mintile = null;
            var isCollision = false;

            var minPointOfPoint = Vector2.zero;

            var checkAirborne = false;

            foreach (var tile in allocatedTiles)
            {
                var tilePosition = tile.transform.localPosition;

                var xMin = tilePosition.x;
                var xMax = tilePosition.x + 1.0f;
                var yMin = tilePosition.y - 1.0f;
                var yMax = tilePosition.y;

                var normal = new Vector2(0.0f, 0.0f);

                // float 비교는 Mathf.Approximately

                var BottomRight = IntersectsLineRectangle(oldPosition + new Vector2(0.4f, 0.0f), currentPosition + new Vector2(0.4f, 0.0f), xMin, yMin, xMax, yMax, out normal);
                if (BottomRight.HasValue)
                {
                    if ((oldPosition - minPoint).sqrMagnitude > (oldPosition - BottomRight.Value).sqrMagnitude)
                    {
                        minPoint = BottomRight.Value;
                        minPointOfPoint = new Vector2(-0.4001f, 0.0001f);
                        isCollision = true;
                        minNormal = normal;
                        mintile = tile;
                    }
                }

                var BottomLeft = IntersectsLineRectangle(oldPosition + new Vector2(-0.4f, 0.0f), currentPosition + new Vector2(-0.4f, 0.0f), xMin, yMin, xMax, yMax, out normal);
                if (BottomLeft.HasValue)
                {
                    if ((oldPosition - minPoint).sqrMagnitude > (oldPosition - BottomLeft.Value).sqrMagnitude)
                    {
                        minPoint = BottomLeft.Value;
                        minPointOfPoint = new Vector2(0.4001f, 0.0001f);
                        isCollision = true;
                        minNormal = normal;
                        mintile = tile;
                    }
                }

                var TopRight = IntersectsLineRectangle(oldPosition + new Vector2(0.4f, 0.8f), currentPosition + new Vector2(0.4f, 0.8f), xMin, yMin, xMax, yMax, out normal);
                if (TopRight.HasValue)
                {
                    if ((oldPosition - minPoint).sqrMagnitude > (oldPosition - TopRight.Value).sqrMagnitude)
                    {
                        minPoint = TopRight.Value;
                        minPointOfPoint = new Vector2(-0.4001f, -0.8001f);
                        isCollision = true;
                        minNormal = normal;
                        mintile = tile;
                    }
                }

                var TopLeft = IntersectsLineRectangle(oldPosition + new Vector2(-0.4f, 0.8f), currentPosition + new Vector2(-0.4f, 0.8f), xMin, yMin, xMax, yMax, out normal);
                if (TopLeft.HasValue)
                {
                    if ((oldPosition - minPoint).sqrMagnitude > (oldPosition - TopLeft.Value).sqrMagnitude)
                    {
                        minPoint = TopLeft.Value;
                        minPointOfPoint = new Vector2(0.4001f, -0.8001f);
                        isCollision = true;
                        minNormal = normal;
                        mintile = tile;
                    }
                }

                var CheckAirborneLeft = IntersectsLineRectangle(oldPosition + new Vector2(-0.4f, 0.0f), oldPosition + new Vector2(-0.4f, -0.1f), xMin, yMin, xMax, yMax, out normal);
                var CheckAirborneRight = IntersectsLineRectangle(oldPosition + new Vector2(0.4f, 0.0f), oldPosition + new Vector2(0.4f, -0.1f), xMin, yMin, xMax, yMax, out normal);

                if ((CheckAirborneLeft.HasValue || CheckAirborneRight.HasValue))
                    checkAirborne = true;
            }

            if (checkAirborne == true)
                isAirborne = false;
            else
                isAirborne = true;

            if (minNormal.x > 0.0f)
            {
                minPoint.x += 0.01f;
                velocity.x = 0.0f;
            }
            else if (minNormal.x < 0.0f)
            {
                minPoint.x += -0.01f;
                velocity.x = 0.0f;
            }

            if (minNormal.y > 0.0f)
            {
                minPoint.y += 0.05f;
                isAirborne = false;
                velocity.y = 0.0f;
                jumpNumber = 0;
            }
            else if (minNormal.y < 0.0f)
            {
                minPoint.y += -0.05f;
                velocity.y = 0.0f;

                mintile.GetComponent<TileJump>().StartTileJumping();
            }

            if (isCollision == true)
            {

                foreach (var tile in allocatedTiles)
                {
                    var tilePosition = tile.transform.localPosition;

                    if (IntersectsPointRectangle(minPoint, tilePosition.x, tilePosition.x + 1.0f, tilePosition.y - 1.0f, tilePosition.y) == true)
                    {
                        Debug.Log("inside");
                        if (minPoint.y + 0.5f > tilePosition.y)
                            minPoint.y = tilePosition.y;
                        else
                            minPoint.y = tilePosition.y - 1.0f;
                    }
                }

                transformCache.localPosition = minPoint + minPointOfPoint;
            }
            else
                transformCache.localPosition += velocity * Super.Instance.GameplayTimeline.DeltaTime;
            #endregion

            var healthPacks = mapCreator.HealthPacks;

            foreach (var pack in healthPacks)
            {
                var packPosition = pack.transform.localPosition;

                var xMin = packPosition.x;
                var xMax = packPosition.x + 0.8f;
                var yMin = packPosition.y - 0.8f;
                var yMax = packPosition.y;

                if (pack.activeSelf == true && (
                IntersectsPointRectangle(transformCache.localPosition + new Vector3(0.5f, 0), xMin, xMax, yMin, yMax) == true ||
                IntersectsPointRectangle(transformCache.localPosition + new Vector3(-0.5f, 0), xMin, xMax, yMin, yMax) == true ||
                IntersectsPointRectangle(transformCache.localPosition + new Vector3(0.5f, 0.5f), xMin, xMax, yMin, yMax) == true ||
                IntersectsPointRectangle(transformCache.localPosition + new Vector3(0.5f, -0.5f), xMin, xMax, yMin, yMax) == true))
                {
                    SoundManager.Instance.PlayCoinSound();
                    pack.SetActive(false);
                    HealthUp(0.10f);
                }
            }
            #endregion
        }

        #region ColliderCheckMethod
        public static Vector2? IntersectsLineRectangle(Vector2 p0, Vector2 p1, float xMin, float yMin, float xMax, float yMax, out Vector2 normal)
        {
            var tMin = 0.0f;
            var tMax = float.MaxValue;

            var distance = 0.0f;
            normal = new Vector2(0.0f, 0.0f);

            var direction = (p1 - p0).normalized;

            if (Mathf.Abs(direction.x) < 0.000001f)
            {
                if (p0.x < xMin || xMax < p0.x)
                    return null;
            }
            else
            {
                var ood = 1.0f / direction.x;
                var t1 = (xMin - p0.x) * ood;
                var t2 = (xMax - p0.x) * ood;

                if (t1 <= t2)
                {
                    if (tMin < t1)
                    {
                        tMin = t1;
                        normal = new Vector2(-1.0f, 0.0f);
                    }
                    tMax = Mathf.Min(tMax, t2);
                }
                else
                {
                    if (tMin < t2)
                    {
                        tMin = t2;
                        normal = new Vector2(1.0f, 0.0f);
                    }
                    tMax = Mathf.Min(tMax, t1);
                }

                if (tMin > tMax)
                    return null;
            }

            if (Mathf.Abs(direction.y) < 0.000001f)
            {
                if (p0.y < yMin || yMax < p0.y)
                    return null;
            }
            else
            {
                var ood = 1.0f / direction.y;
                var t1 = (yMin - p0.y) * ood;
                var t2 = (yMax - p0.y) * ood;

                if (t1 <= t2)
                {
                    if (tMin < t1)
                    {
                        tMin = t1;
                        normal = new Vector2(0.0f, -1.0f);
                    }
                    tMax = Mathf.Min(tMax, t2);
                }
                else
                {
                    if (tMin < t2)
                    {
                        tMin = t2;
                        normal = new Vector2(0.0f, 1.0f);
                    }
                    tMax = Mathf.Min(tMax, t1);
                }

                if (tMin > tMax)
                    return null;
            }
            distance = tMin; // ray 길이

            var point = p0 + distance * direction;

            if ((p1 - p0).magnitude < (point - p0).magnitude)
                return null;
            else
            {
                return p0 + distance * direction;
            }
        }
        #endregion

        private bool IntersectsPointRectangle(Vector2 point, float xMin, float xMax, float yMin, float yMax)
        {
            if ((xMin <= point.x && point.x <= xMax) && (yMin <= point.y && point.y <= yMax))
            {
                return true;
            }

            return false;
        }
    }
}


