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
        private bool onPlayingMovie;
        private Transform runnerTransformCache;

        public bool OnPlayingMovie
        {
            get { return onPlayingMovie; }
            set { onPlayingMovie = value; }
        }
        #endregion

        void Start()
        {
      //      onPlayingMovie = false;
            runnerTransformCache = runner.GetComponent<Transform>();
        }

        public void StartMovie()
        {
            if (runnerTransformCache == null)
                runnerTransformCache = runner.GetComponent<Transform>();

            runnerTransformCache.localPosition = new Vector3(-8, 0);

            StartCoroutine(UpPipe());
        }

        private IEnumerator UpPipe()
        {
            onPlayingMovie = true;

            SoundManager.Instance.PlayPipeSound();

            var duration = 1.0f * Super.Instance.GameplayTimeline.Scale;
            var startTime = Super.Instance.GameplayTimeline.CurrentTime;
            var endTime = startTime + duration;

            while (Super.Instance.GameplayTimeline.CurrentTime < endTime)
            {
                var elaspedTime = Super.Instance.GameplayTimeline.CurrentTime - startTime;
                var time = elaspedTime / duration;

                runnerTransformCache.localPosition += new Vector3(0, Super.Instance.GameplayTimeline.DeltaTime * 2.2f, 0);

                yield return null;
            }

            onPlayingMovie = false;
        }
    }
}
