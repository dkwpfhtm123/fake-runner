using UnityEngine;
using UnityEngine.UI;

namespace Fake.FakeRunner.Unity
{
    public class SoundManager : MonoBehaviour
    {
        #region Fields
        [SerializeField]
        private AudioClip jumpSound;
        [SerializeField]
        private AudioClip pauseSound;
        [SerializeField]
        private AudioClip coinSound;
        [SerializeField]
        private AudioClip runnerHitSound;
        [SerializeField]
        private AudioClip pipeSound;
        [SerializeField]
        private AudioClip gameOverSound;

        private AudioSource myAudio;
        private float currentVolume;
        #endregion

        public float AudioVolume
        {
            get { return myAudio.volume; }
            set { myAudio.volume = value; }
        }

        private void Start()
        {
            myAudio = GetComponent<AudioSource>();
        }

        public void PlayJumpSound()
        {
            myAudio.PlayOneShot(jumpSound);
        }

        public void PlayPauseSound()
        {
            myAudio.PlayOneShot(pauseSound);
        }

        public void PlayCoinSound()
        {
            myAudio.PlayOneShot(coinSound);
        }

        public void PlayRunnerHitSound()
        {
            myAudio.PlayOneShot(runnerHitSound);
        }

        public void PlayPipeSound()
        {
            myAudio.PlayOneShot(pipeSound);
        }

        public void PlayGameOverSound()
        {
            myAudio.PlayOneShot(gameOverSound);
        }
    }
}