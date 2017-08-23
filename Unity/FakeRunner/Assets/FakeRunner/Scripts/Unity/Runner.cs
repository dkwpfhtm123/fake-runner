using UnityEngine;
using UnityEngine.UI;

namespace Fake.FakeRunner.Unity
{
    public delegate void RunnerEventHandler(Runner sender);

    public class Runner : MonoBehaviour
    {
        #region Fields
        [SerializeField]
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
        private int jumpCount;
        private bool isAirborne;
        private float kumbaCountDown;

        public event RunnerEventHandler PositionChanged;
        #endregion

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

        public float MaxSpeed
        {
            get { return maxSpeed; }
        }

        private void Awake()
        {
            Initialize();
        }

        private void Update()
        {
            if (canControl)
            {
                var horizontal = Input.GetAxisRaw("Horizontal");

                if (Mathf.Abs(horizontal) < 0.1f)
                    horizontal = 0.0f;

                if (Input.GetKeyDown(KeyCode.UpArrow) && jumpCount < 2)
                    Jump();

                UpdateVelocity(horizontal);
                Move();
                TimeLimit();
            }
            else
                velocity = Vector3.zero;

            if (PositionChanged != null)
                PositionChanged(this);
        }

        private void TimeLimit()
        {
            kumbaCountDown -= Super.Instance.GameplayTimeline.DeltaTime;

            if (kumbaCountDown < 0.0f)
            {
                kumbaCountDown = 3.0f;
                ChangeHealth(-healthDownValue);
            }

            if (healthBar.value < 0.00001f)
                Super.Instance.PlayGameOver();
        }

        private void Jump()
        {
            isAirborne = true;
            velocity.y = 8.0f;
            jumpCount++;
            Super.Instance.PlayJumpSound();
        }

        private void UpdateVelocity(float inputValue)
        {
            var friction = 3.0f * Super.Instance.GameplayTimeline.DeltaTime;

            if (Mathf.Abs(velocity.x) > friction)
                velocity += new Vector3(-Mathf.Sign(velocity.x) * friction, 0);
            else
                velocity.x = 0;

            velocity += new Vector3(inputValue * 20.0f * Super.Instance.GameplayTimeline.DeltaTime, 0);

            if (isAirborne == true)
                velocity += new Vector3(0, -9.8f * Super.Instance.GameplayTimeline.DeltaTime);

            if (Mathf.Abs(velocity.x) > maxSpeed)
                velocity.x = Mathf.Sign(velocity.x) * maxSpeed;

            if (Mathf.Abs(velocity.y) > maxSpeed * 2.0f)
                velocity.y = Mathf.Sign(velocity.y) * maxSpeed * 1.5f;
        }

        public void ChangeHealth(float value)
        {
            healthBar.value += value;

            if (healthBar.value > 1.0f)
                healthBar.value = 1.0f;

            if (healthBar.value < 0.0f)
                healthBar.value = 0.0f;
        }

        public void Initialize()
        {
            transformCache = GetComponent<Transform>();
            Velocity = Vector3.zero;
            maxSpeed = 8.0f;
            jumpCount = 0;
            isAirborne = false;
            canControl = true;
            healthDownValue = 0.1f;
            healthBar.value = 1.0f;

            kumbaCountDown = 3.0f;
        }

        private void Move()
        {
            var rigidbody2D = GetComponent<Rigidbody2D>();
            var movement = transformCache.localPosition + velocity * Super.Instance.GameplayTimeline.DeltaTime;

            var hitResult = new RaycastHit2D[4];
            var exValue = Vector3.zero;
            var direction = velocity.normalized;

            var number = rigidbody2D.Cast(direction, hitResult, (velocity.magnitude) * Super.Instance.GameplayTimeline.DeltaTime);

            if (isAirborne == false)
            {
                if (rigidbody2D.Cast(Vector3.down, new RaycastHit2D[4], 0.5f) == 0)
                    isAirborne = true;
            }

            if (number != 0)
            {
                var minHits = hitResult[0];
                var minDistance = 100.0f;

                for (int i = 0; i < number; i++)
                {
                    if (hitResult[i].distance < minDistance)
                    {
                        minDistance = hitResult[i].distance;
                        minHits = hitResult[i];
                    }
                }

                var food = minHits.collider.gameObject.GetComponent<Food>();
                var kumba = minHits.collider.gameObject.GetComponent<Kumba>();

                if (food != null)
                {
                    Super.Instance.PlayCoinSound();
                    food.Eat(this);
                }
                else if (kumba != null)
                {
                    Super.Instance.PlayRunnerHitSound();
                    kumba.Free();
                    ChangeHealth(-0.2f);
                }
                else // collider = tilePool
                {
                    if (minHits.distance > 0.001f)
                    {
                        if (minHits.normal.x != 0)
                        {
                            velocity.x = 0;
                            exValue += new Vector3(minHits.normal.x * 0.005f, 0);
                        }
                        else if (minHits.normal.y > 0)
                        {
                            velocity.y = 0;
                            exValue += new Vector3(0, 0.005f);
                            jumpCount = 0;
                            isAirborne = false;
                        }
                        else if (minHits.normal.y < 0)
                        {
                            velocity.y = 0;
                            exValue += new Vector3(0, -0.005f);
                        }

                        movement = transformCache.localPosition + direction * minHits.distance + exValue;
                    }
                }
            }

            rigidbody2D.MovePosition(movement);
        }
    }
}