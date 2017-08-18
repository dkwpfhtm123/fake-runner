using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fake.FakeRunner.Unity
{
    public class MovieManager : MonoBehaviour
    {
        #region Fields
        [SerializeField]
        private GameObject runner;
        [SerializeField]
        private GameObject emptyBlackCircle;
        [SerializeField]
        private GameObject blackBox;

        private Coroutine currentMovie;
        private List<GameObject> blackBoxs;
        private bool onPlayingMovie;
        private Transform runnerTransformCache;

        public bool OnPlayingMovie
        {
            get { return onPlayingMovie; }
            set { onPlayingMovie = value; }
        }

        public Coroutine Movie
        {
            get { return currentMovie; }
        }
        #endregion

        void Start()
        {
            //      onPlayingMovie = false;
            runnerTransformCache = runner.GetComponent<Transform>();
            blackBoxs = new List<GameObject>();
        }

        public void StartMovie()
        {
            if (runnerTransformCache == null)
                runnerTransformCache = runner.GetComponent<Transform>();

            runnerTransformCache.localPosition = new Vector3(-8, 0);

            currentMovie = StartCoroutine(UpPipe());
        }

        private IEnumerator UpPipe()
        {
            onPlayingMovie = true;

            runner.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezePositionY;

            SoundManager.Instance.PlayPipeSound();

            var duration = 1.0f;
            var startTime = Super.Instance.GameplayTimeline.CurrentTime;
            var endTime = startTime + duration;

            while (Super.Instance.GameplayTimeline.CurrentTime < endTime)
            {
                runnerTransformCache.localPosition += new Vector3(0, Super.Instance.GameplayTimeline.DeltaTime * 2.2f, 0);
                yield return null;
            }

            onPlayingMovie = false;
            runner.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
        }

        public void StartEndMovie()
        {
            currentMovie = StartCoroutine(EndMovie());
        }

        public void StopEndMovie()
        {
            StopCoroutine(currentMovie);
        }

        private IEnumerator EndMovie()
        {
            Super.Instance.GameplayTimeline.StopTime();

            if (runnerTransformCache == null)
                runnerTransformCache = runner.GetComponent<Transform>();

            //      onPlayingMovie = true;

            var value = 5.5f;

            var createdEmptyBlackCircle = Instantiate(emptyBlackCircle);
            SetBoxPosition(createdEmptyBlackCircle, runnerTransformCache.localPosition, new Vector3(1, 1), Quaternion.identity);
            blackBoxs.Add(createdEmptyBlackCircle);

            var upBox = Instantiate(blackBox);
            SetBoxPosition(upBox, createdEmptyBlackCircle.transform.localPosition + new Vector3(0, value, 0), new Vector3(10, 4), Quaternion.identity);
            blackBoxs.Add(upBox);

            var downBox = Instantiate(blackBox);
            SetBoxPosition(downBox, createdEmptyBlackCircle.transform.localPosition + new Vector3(0, -value, 0), new Vector3(10, 4), Quaternion.identity);
            blackBoxs.Add(downBox);

            var rightBox = Instantiate(blackBox);
            SetBoxPosition(rightBox, createdEmptyBlackCircle.transform.localPosition + new Vector3(value, 0, 0), new Vector3(10, 4), Quaternion.Euler(0, 0, 90));
            blackBoxs.Add(rightBox);

            var leftBox = Instantiate(blackBox);
            SetBoxPosition(rightBox, createdEmptyBlackCircle.transform.localPosition + new Vector3(-value, 0, 0), new Vector3(10, 4), Quaternion.Euler(0, 0, 90));
            blackBoxs.Add(leftBox);

            var countDown = 3.0f;

            while (countDown > 0.0f)
            {
                Debug.Log(countDown);
                var minimum = countDown;

                if (minimum < 1.0f)
                    minimum = 1.0f;

                countDown -= Super.Instance.AnimationTimeline.DeltaTime;

                createdEmptyBlackCircle.transform.localScale = Vector3.one * minimum;

                var temp = value * minimum;

                SetBoxPosition(upBox, createdEmptyBlackCircle.transform.localPosition + new Vector3(0, temp, 0), new Vector3(10, 4) * minimum, Quaternion.identity);
                SetBoxPosition(downBox, createdEmptyBlackCircle.transform.localPosition + new Vector3(0, -temp, 0), new Vector3(10, 4) * minimum, Quaternion.identity);
                SetBoxPosition(leftBox, createdEmptyBlackCircle.transform.localPosition + new Vector3(temp, 0, 0), new Vector3(10, 4) * minimum, Quaternion.Euler(0, 0, 90));
                SetBoxPosition(rightBox, createdEmptyBlackCircle.transform.localPosition + new Vector3(-temp, 0, 0), new Vector3(10, 4) * minimum, Quaternion.Euler(0, 0, 90));

                yield return null;
            }
        }

        public void ClearBlackBoxs()
        {
            var temp = blackBoxs;
            foreach (var box in temp)
            {
                Destroy(box);
            }

            blackBoxs.Clear();
        }

        private void SetBoxPosition(GameObject box, Vector3 position, Vector3 scale, Quaternion rotation) // 선택형 매개변수 사용법? quaternion
        {
            box.transform.localPosition = position;
            box.transform.localScale = scale;
            box.transform.localRotation = rotation;
        }
    }
}