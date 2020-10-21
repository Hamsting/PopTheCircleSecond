using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PopTheCircle.NoteEditor
{
    public class MusicManager : Singleton<MusicManager>
    {
        private AudioSource musicAudioSource;
        private AudioSource shotAudioSource;
        private AudioSource seAudioSource;
        private float musicStartTime = 0.0f;
        private float musicPosition = 0.0f;
        private bool isPlaying = false;
        private bool playedShotThisFrame = false;

        public AudioClip shotClip;
        public AudioClip[] seClips;
        [InspectorReadOnly]
        public bool isMusicLoaded = false;

        public AudioClip Music
        {
            get
            {
                if (isMusicLoaded)
                    return musicAudioSource.clip;
                else
                    return null;
            }
            set
            {
                musicAudioSource.clip = value;
                isMusicLoaded = (value != null);
            }
        }
        public bool IsPlaying
        {
            get
            {
                return isPlaying;
            }
        }
        public float MusicPosition
        {
            get
            {
                return musicPosition - musicStartTime;
            }
            set
            {
                musicPosition = value + musicStartTime;
                if (isPlaying)
                {
                    musicAudioSource.Stop();
                }
            }
        }
        public float MusicStartTime
        {
            get
            {
                return musicStartTime;
            }
            set
            {
                musicStartTime = value;
            }
        }
        public float MusicLength
        {
            get
            {
                if (isMusicLoaded)
                    return musicAudioSource.clip.length - musicStartTime;
                else
                    return 0.0f;
            }
        }
        public float MusicPlaySpeed
        {
            get
            {
                return musicAudioSource.pitch;
            }
            set
            {
                musicAudioSource.pitch = value;
            }
        }
        public float MusicVolume
        {
            get { return musicAudioSource.volume; }
            set { musicAudioSource.volume = value; }
        }
        public float ShotVolume
        {
            get { return shotAudioSource.volume; }
            set { shotAudioSource.volume = value; }
        }
        public float SEVolume
        {
            get { return seAudioSource.volume; }
            set { seAudioSource.volume = value; }
        }



        protected override void Awake()
        {
            AudioSource[] audioSources = this.GetComponents<AudioSource>();
            musicAudioSource = audioSources[0];
            shotAudioSource = audioSources[1];
            seAudioSource = audioSources[2];
            shotAudioSource.clip = shotClip;

            musicPosition = 0.0f;
            isPlaying = false;
        }

        private void Update()
        {
            if (isPlaying)
            {
                if (!musicAudioSource.isPlaying &&
                    musicPosition >= 0.0f && musicPosition < musicAudioSource.clip.length)
                {
                    musicAudioSource.time = musicPosition;
                    musicAudioSource.Play();
                }

                if (musicAudioSource.isPlaying && musicPosition >= musicAudioSource.clip.length)
                {
                    musicAudioSource.Stop();
                }

                if (musicPosition >= musicAudioSource.clip.length)
                    isPlaying = false;
                else
                    musicPosition += Time.deltaTime * MusicPlaySpeed;
            }
            else
            {
                if (musicAudioSource.isPlaying)
                {
                    musicAudioSource.Stop();
                }
            }
        }

        public void LateUpdate()
        {
            playedShotThisFrame = false;
        }

        public void PlayMusic()
        {
            if (isMusicLoaded)
            {
                isPlaying = true;
            }
        }

        public void PauseMusic()
        {
            if (isMusicLoaded)
            {
                isPlaying = false;
            }
        }

        public void StopMusic()
        {
            if (isMusicLoaded)
            {
                isPlaying = false;
                MusicPosition = 0.0f;
            }
        }

        public void PlayShot()
        {
            if (playedShotThisFrame)
                return;

            shotAudioSource.Stop();
            shotAudioSource.time = 0.0f;
            shotAudioSource.Play();
            playedShotThisFrame = true;
        }

        public void PlaySE(EffectNoteSEType _seType)
        {
            if (_seType == EffectNoteSEType.None)
                return;

            string seName = "Effect_" + _seType.ToString();
            foreach (var seClip in seClips)
            {
                if (seClip.name.Equals(seName))
                {
                    /*
                    seAudioSource.Stop();
                    seAudioSource.clip = seClip;
                    seAudioSource.time = 0.0f;
                    seAudioSource.Play();
                    */
                    seAudioSource.PlayOneShot(seClip);
                    break;
                }
            }
        }
    }
}