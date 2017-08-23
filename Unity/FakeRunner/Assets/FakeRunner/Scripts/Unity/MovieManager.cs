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
        private GameObject blackHole;
        [SerializeField]
        private GameObject blackBox;

        private Coroutine currentMovie;
        private List<GameObject> blackBoxes;
        private bool onPlayingMovie;
        private Transform runnerTransformCache;
        #endregion

        public bool OnPlayingMovie
        {
            get { return onPlayingMovie; }
            set { onPlayingMovie = value; }
        }

        public Coroutine CurrentMovie
        {
            get { return currentMovie; }
        }

        private void Start()
        {
            runnerTransformCache = runner.GetComponent<Transform>();
            blackBoxes = new List<GameObject>();
        }

        public void StartFirstMovie()
        {
            if (runnerTransformCache == null)
                runnerTransformCache = runner.GetComponent<Transform>();

            runnerTransformCache.localPosition = new Vector3(-8, 0);

            currentMovie = StartCoroutine(FirstMovie());
        }

        public void StartEndMovie()
        {
            currentMovie = StartCoroutine(EndMovie());
        }

        public void StopMovie()
        {
            if (currentMovie != null)
                StopCoroutine(currentMovie);
        }

        public void ClearBlackBoxes()
        {
            if (blackBoxes.Count != 0)
            {
                var temp = blackBoxes;
                foreach (var box in temp)
                {
                    Destroy(box);
                }

                blackBoxes.Clear();
            }
        }

        private IEnumerator FirstMovie()
        {
            onPlayingMovie = true;
            Super.Instance.PlayPipeSound();

            var countDown = 1.0f;
            while (countDown > 0.0f)
            {
                countDown -= Super.Instance.GameplayTimeline.DeltaTime;
                runnerTransformCache.localPosition += new Vector3(0, Super.Instance.GameplayTimeline.DeltaTime * 2.2f, 0);
                yield return null;
            }

            onPlayingMovie = false;
        }

        private IEnumerator EndMovie()
        {
            Super.Instance.GameplayTimeline.SetTimeScale(0.0f);

            if (runnerTransformCache == null)
                runnerTransformCache = runner.GetComponent<Transform>();

            onPlayingMovie = true;

            var value = 9.5f;

            var createdEmptyBlackCircle = CreateBox(blackHole, runnerTransformCache.localPosition, new Vector3(1, 1), Quaternion.identity);
            var upBox = CreateBox(blackBox, createdEmptyBlackCircle.transform.localPosition + new Vector3(0, value, 0), new Vector3(20, 8), Quaternion.identity);
            var downBox = CreateBox(blackBox, createdEmptyBlackCircle.transform.localPosition + new Vector3(0, -value, 0), new Vector3(20, 8), Quaternion.identity);
            var rightBox = CreateBox(blackBox, createdEmptyBlackCircle.transform.localPosition + new Vector3(value, 0, 0), new Vector3(20, 8), Quaternion.Euler(0, 0, 90));
            var leftBox = CreateBox(blackBox, createdEmptyBlackCircle.transform.localPosition + new Vector3(-value, 0, 0), new Vector3(20, 8), Quaternion.Euler(0, 0, 90));

            var countDown = 3.0f;

            while (countDown > 0.0f)
            {
                var minimum = countDown;

                if (minimum < 1.5f)
                    minimum = 1.5f;

                countDown -= Super.Instance.AnimationTimeline.DeltaTime;

                createdEmptyBlackCircle.transform.localScale = Vector3.one * minimum;

                var temp = value * minimum;

                SetBoxPosition(upBox, createdEmptyBlackCircle.transform.localPosition + new Vector3(0, temp, 0), new Vector3(20, 8) * minimum, Quaternion.identity);
                SetBoxPosition(downBox, createdEmptyBlackCircle.transform.localPosition + new Vector3(0, -temp, 0), new Vector3(20, 8) * minimum, Quaternion.identity);
                SetBoxPosition(leftBox, createdEmptyBlackCircle.transform.localPosition + new Vector3(temp, 0, 0), new Vector3(20, 8) * minimum, Quaternion.Euler(0, 0, 90));
                SetBoxPosition(rightBox, createdEmptyBlackCircle.transform.localPosition + new Vector3(-temp, 0, 0), new Vector3(20, 8) * minimum, Quaternion.Euler(0, 0, 90));

                yield return null;
            }

            onPlayingMovie = false;
        }

        private GameObject CreateBox(GameObject box, Vector3 position, Vector3 scale, Quaternion rotation)
        {
            var createdBox = Instantiate(box);
            SetBoxPosition(createdBox, position, scale, rotation);
            blackBoxes.Add(createdBox);

            return createdBox;
        }

        private void SetBoxPosition(GameObject box, Vector3 position, Vector3 scale, Quaternion rotation)
        {
            var boxTransformCache = box.transform;
            boxTransformCache.localPosition = position;
            boxTransformCache.localScale = scale;
            boxTransformCache.localRotation = rotation;
        }
    }
}