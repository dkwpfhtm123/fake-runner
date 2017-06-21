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
        private AudioClip stageclearSound;
        [SerializeField]
        private AudioClip pipeSound;

        private AudioSource myAudio;
        private static SoundManager instance;
        private float currentVolume;
        #endregion

        public static SoundManager Instance
        {
            get
            {
                if (instance == null)
                    instance = new SoundManager();
                return instance;
            }
        }

        void Awake()
        {
            if (instance == null)
                instance = this;
        }

        void Start()
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

        public void PlayStageClearSound()
        {
            myAudio.PlayOneShot(stageclearSound);
        }

        public void PlayPipeSound()
        {
            myAudio.PlayOneShot(pipeSound);
        }

        public void ChangeVolume(Slider slider)
        {
            myAudio.volume = slider.value;
        }
    }
}