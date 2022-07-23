using UnityEngine;

namespace W
{
    public class AudioSystem : MonoBehaviour {
        public static AudioSystem I { get; private set; }
        private void Awake() {
            A.Assert(I == null);
            I = this;
        }


        public AudioListener Listener;
        public AudioSource Music;
        public AudioSource Sound;

        private AudioClip clipToPlay;
        private AudioClip[] clipsToPlay;


        private void Update() {
            UpdateSound();
            UpdateMusic();
        }

        private void UpdateSound() {
            if (clipToPlay != null) {
                Sound.PlayOneShot(clipToPlay);
            } else if (clipsToPlay != null) {
                AudioClip randomClip = clipsToPlay[UnityEngine.Random.Range(0, clipsToPlay.Length)];
                Sound.PlayOneShot(randomClip);
            }
            clipToPlay = null;
            clipsToPlay = null;
        }


        private float wait = 40;
        private const float waitInterval = 60;

        public string MusicName => Music.clip?.name;
        private void UpdateMusic() {
            if (!GameData.I.MusicEnabled) {
                return;
            }
            if (wait > waitInterval && !Music.isPlaying) {
                wait = 0;

                Music.clip = Musics[GameData.I.MusicIndex % Musics.Length];
                Music.Play();

                GameData.I.MusicIndex++;
            } else {
                wait += Time.deltaTime;
            }
        }
        public void StopMusic() {
            Music.Stop();
        }
        public void PlayMusic() {
            wait = waitInterval;
        }

        public void Play(AudioClip clip) {
            clipToPlay = clip;
            clipsToPlay = null;
        }

        public void Play(AudioClip[] clips) {
            clipToPlay = null;
            clipsToPlay = clips;
        }


        [Space]

        [SerializeField]
        public AudioClip[] Click;

        [SerializeField]
        public AudioClip[] Craft;

        [SerializeField]
        public AudioClip[] Move;

        [SerializeField]
        public AudioClip[] Discard;

        [SerializeField]
        public AudioClip[] Gacha;


        /// <summary>
        /// streaming
        /// </summary>
        [SerializeField]
        public AudioClip[] Musics;
    }
}


