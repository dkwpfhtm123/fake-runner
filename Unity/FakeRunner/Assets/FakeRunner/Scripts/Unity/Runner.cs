using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

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
        private int jumpNumber;
        private bool isAirborne;
        private float countdown;

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

        public float MaxSpeed
        {
            get { return maxSpeed; }
        }
        #endregion

        private void Awake()
        {
            transformCache = GetComponent<Transform>();
            Velocity = Vector3.zero;
            maxSpeed = 8.0f;
            jumpNumber = 0;
            isAirborne = false;
            CanControl = true;

            countdown = 3.0f;
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

                countdown -= Super.Instance.GameplayTimeline.DeltaTime;

                if (countdown < 0.0f)
                {
                    countdown = 3.0f;
                    HealthDown(0.1f);
                }

                if (healthBar.value < 0.00001f)
                    Super.Instance.GameOver();
            }

            if (PositionChanged != null)
                PositionChanged(this);
        }

        public void HealthDown(float value)
        {
            healthBar.value -= healthDownValue;

            if (healthBar.value < 0.0f)
                healthBar.value = 0.0f;
        }

        public void HealthUp(float value)
        {
            healthBar.value += value;

            if (healthBar.value > 1.0f)
                healthBar.value = 1.0f;
        }

        private void Move()
        {
            var movement = transformCache.localPosition + velocity * Super.Instance.GameplayTimeline.DeltaTime;

            var hitResult = new RaycastHit2D[4];
            var exValue = Vector3.zero;
            var direction = velocity.normalized;

            var number = GetComponent<Rigidbody2D>().Cast(direction, hitResult, (velocity.magnitude) * Super.Instance.GameplayTimeline.DeltaTime);

            var healthPackPool = mapCreator.HealthpackPool;

            if (isAirborne == false)
            {
                if (GetComponent<Rigidbody2D>().Cast(Vector3.down, new RaycastHit2D[4], 0.5f) == 0)
                    isAirborne = true;
            }

            if (number != 0)
            {
                var minHit = hitResult[0];
                var minDistance = 100.0f;

                for (int i = 0; i < number; i++)
                {
                    if (hitResult[i].distance < minDistance)
                    {
                        minDistance = hitResult[i].distance;
                        minHit = hitResult[i];
                    }
                }

                if (minHit.collider.gameObject.GetComponent<Food>() != null)
                    minHit.collider.gameObject.GetComponent<Food>().Eat(this);
                else if (minHit.collider.gameObject.GetComponent<Kumba>() != null)
                    minHit.collider.gameObject.GetComponent<Kumba>().KumbaFree();
                else // collider = tilePool
                {
                    if (minHit.normal.x != 0)
                    {
                        velocity.x = 0;
                        exValue += new Vector3(minHit.normal.x * 0.01f, 0);
                    }
                    else if (minHit.normal.y > 0)
                    {
                        velocity.y = 0;
                        exValue += new Vector3(0, 0.01f);
                        jumpNumber = 0;
                        isAirborne = false;
                    }
                    else if (minHit.normal.y < 0)
                    {
                        velocity.y = 0;
                        exValue += new Vector3(0, -0.01f);
                    }

                    movement = transformCache.localPosition + direction * minHit.distance + exValue;
                }
            }

            GetComponent<Rigidbody2D>().MovePosition(movement);
        }
    }
}