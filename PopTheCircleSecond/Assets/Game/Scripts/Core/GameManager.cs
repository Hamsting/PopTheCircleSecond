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
        [InspectorReadOnly]
        public int judgePerfectCount = 0;
        [InspectorReadOnly]
        public int judgeNiceCount = 0;
        [InspectorReadOnly]
        public int judgeMissCount = 0;
        [InspectorReadOnly]
        public int totalCombo = 0;
        [InspectorReadOnly]
        public ClearGaugeType clearGaugeType = ClearGaugeType.Normal;
        
        [SerializeField, InspectorReadOnly]
        private float clearGauge = 0.0f;
        public float ClearGauge
        {
            get
            {
                return clearGauge;
            }
            set
            {
                clearGauge = value;
                clearGauge = Mathf.Clamp(clearGauge, 0.0f, 100.0f);
            }
        }



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

            totalCombo = NoteTickCalculator.CalculateTotalTick(NoteManager.Instance.allNotes, BeatManager.Instance.BPMInfos, out int longNoteTotalCombo);
            ClearGaugeCalculator.CalculateResult(clearGaugeType, totalCombo, longNoteTotalCombo, 
                out JudgeManager.Instance.clearGaugeIncreaseAmount, out JudgeManager.Instance.clearGaugeDecreaseAmount);
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

            float timeDiff = BeatManager.Instance.GameTime - MusicManager.Instance.MusicPosition;
            if (Mathf.Abs(timeDiff) >= 0.033333f)
            {
                Debug.LogWarning("Fixed times");
                BeatManager.Instance.GameTime = MusicManager.Instance.MusicPosition;
            }
            // BeatManager.Instance.GameTime = Mathf.Lerp(BeatManager.Instance.GameTime, MusicManager.Instance.MusicPosition, Time.deltaTime * 2.0f);
        }
    }
}