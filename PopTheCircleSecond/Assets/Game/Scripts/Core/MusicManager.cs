using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PopTheCircle.Game
{
    public class MusicManager : Singleton<MusicManager>
    {
        private AudioSource musicAudioSource;
        private AudioSource shotAudioSource;
        private AudioSource longTickAudioSource;
        private AudioSource seAudioSource;
        private float musicStartTime = 0.0f;
        private float musicSyncDelayTime = 0.0f;
        private float musicPosition = 0.0f;
        private bool isPlaying = false;
        private bool playedShotThisFrame = false;
        private bool playedLongTickThisFrame = false;

        public AudioClip[] shotClips;
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
                return musicPosition - musicStartTime - musicSyncDelayTime;
            }
            set
            {
                musicPosition = value + musicStartTime + musicSyncDelayTime;
                if (isPlaying)
                {
                    // musicAudioSource.Stop();
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
        public float MusicSyncDelayTime
        {
            get
            {
                return musicSyncDelayTime;
            }
            set
            {
                musicSyncDelayTime = value;
            }
        }
        public float MusicLength
        {
            get
            {
                if (isMusicLoaded)
                    return musicAudioSource.clip.length - musicStartTime - musicSyncDelayTime;
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
        public AudioSource MusicAudioSource
        {
            get
            {
                return musicAudioSource;
            }
        }



        protected override void Awake()
        {
            AudioSource[] audioSources = this.GetComponents<AudioSource>();
            musicAudioSource = audioSources[0];
            shotAudioSource = audioSources[1];
            longTickAudioSource = audioSources[2];
            longTickAudioSource.clip = shotClips[2];
            seAudioSource = audioSources[3];

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
                {
                    musicPosition += Time.deltaTime * MusicPlaySpeed;
                    if (musicAudioSource.isPlaying && Mathf.Abs(musicAudioSource.time - musicPosition) >= 0.032f)
                    {
                        musicPosition = musicAudioSource.time;
                    }
                }
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
            playedLongTickThisFrame = false;
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

        public void PlayShot(int _judge)
        {
            if (playedShotThisFrame)
                return;

            shotAudioSource.Stop();
            if (_judge == 1)
                shotAudioSource.clip = shotClips[0];
            else if (_judge == 2)
                shotAudioSource.clip = shotClips[1];

            shotAudioSource.time = 0.03f;
            shotAudioSource.Play();
            playedShotThisFrame = true;
        }

        public void PlayLongTick()
        {
            if (playedLongTickThisFrame)
                return;

            // longTickAudioSource.Stop();
            // longTickAudioSource.time = 0.0f;
            // longTickAudioSource.Play();
            playedLongTickThisFrame = true;
        }

        public void PlaySE(EffectNoteSEType _seType)
        {
            // string seName = "Effect_" + _seType.ToString();

            string seName = _seType.ToString();

            if (_seType == EffectNoteSEType.Clap)
                seName = EffectNoteSEType.E001_DrumClap_1.ToString();
            else if (_seType == EffectNoteSEType.SharpKick)
                seName = EffectNoteSEType.E002_SharpKick_1.ToString();

            foreach (var seClip in seClips)
            {
                if (seClip.name.Equals(seName))
                {
                    seAudioSource.Stop();
                    seAudioSource.clip = seClip;
                    seAudioSource.time = 0.0f;
                    seAudioSource.Play();
                    break;
                }
            }
        }
    }
}