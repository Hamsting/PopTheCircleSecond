using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PopTheCircle.Game
{
    /// <summary>
    /// 게임의 진행을 담당한다.
    /// </summary>
    public class GameManager : Singleton<GameManager>
    {
        public const float StartDelayTime = 3.0f;

        [InspectorReadOnly]
        public int currentCombo = 0;
        [InspectorReadOnly]
        public int maxCombo = 0;
        [InspectorReadOnly]
        public int score = 0;

        

        private void Start()
        {
            if (GlobalData.Instance != null)
            {
                NoteDataJSONConverter.Instance.JSONToNoteData(GlobalData.Instance.noteDataJson);
                MusicManager.Instance.Music = GlobalData.Instance.musicClip;
                MusicManager.Instance.PlayMusic();
            }

            // MusicManager.Instance.Music = MusicManager.Instance.MusicAudioSource.clip;
            // MusicManager.Instance.PlayMusic();
            MusicManager.Instance.MusicPosition = -StartDelayTime;
            BeatManager.Instance.GotoStartDelayTime(-StartDelayTime);
        }

        private void Update()
        {
            /*
            Debug.Log("MT : " + MusicManager.Instance.MusicPosition +
                      "RMT : " + MusicManager.Instance.MusicAudioSource.time +
                      ", BT : " + BeatManager.Instance.GameTime + 
                      ", MB D : " + (BeatManager.Instance.GameTime - MusicManager.Instance.MusicPosition) +
                      ", RM D : " + (MusicManager.Instance.MusicAudioSource.time - MusicManager.Instance.MusicPosition));
            */
            if (Mathf.Abs(BeatManager.Instance.GameTime - MusicManager.Instance.MusicPosition) >= 0.032f)
            {
                Debug.LogWarning("Fixed times");
                BeatManager.Instance.GameTime = MusicManager.Instance.MusicPosition;
            }
        }
    }
}