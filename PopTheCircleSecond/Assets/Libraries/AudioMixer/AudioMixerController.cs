using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace PopTheCircle
{
    public class AudioMixerController : MonoBehaviour
    {
        public AudioSource audioSource;
        public AudioMixerGroup audioMixerGroup;

        [Range(10.0f, 22000.0f)]
        public float lowpassCutOffRangeTest = 10.0f;
        public float LowpassCutoffFreq
        {
            get
            {
                if (audioMixerGroup == null)
                    return 0.0f;
                audioMixerGroup.audioMixer.GetFloat("LowpassCutoffFreq", out float floatValue);
                return floatValue;
            }
            set
            {
                audioMixerGroup.audioMixer.SetFloat("LowpassCutoffFreq", Mathf.Clamp(value, 10.0f, 22000.0f));
            }
        }


        private void Awake()
        {
            if (audioSource != null)
                audioSource.outputAudioMixerGroup = audioMixerGroup;
            
            // audioMixerGroup.audioMixer.
        }

        private void Update()
        {
            LowpassCutoffFreq = lowpassCutOffRangeTest;
        }
    }
}