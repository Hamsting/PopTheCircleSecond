using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PopTheCircle.Game
{
    /// <summary>
    /// 게임의 진행을 담당한다.
    /// </summary>
    public class GameManager : Singleton<GameManager>
    {
        public const float StartDelayTime = 3.0f;

        [InspectorReadOnly] public int currentCombo = 0;
        [InspectorReadOnly] public int maxCombo = 0;
        [InspectorReadOnly] public int score = 0;
        [InspectorReadOnly] public int judgePerfectCount = 0;
        [InspectorReadOnly] public int judgeNiceCount = 0;
        [InspectorReadOnly] public int judgeMissCount = 0;
        [InspectorReadOnly] public int totalCombo = 0;

        private ClearGaugeType clearGaugeType = ClearGaugeType.Normal;
        private bool isClearGaugeLifeType = false;

        [SerializeField, InspectorReadOnly] private float clearGauge = 0.0f;
        public float ClearGauge
        {
            get
            {
                return clearGauge;
            }
            set
            {
                clearGauge = Mathf.Clamp(value, 0.0f, 100.0f);
            }
        }



        private void Start()
        {
            if (GlobalData.Instance != null)
            {
                NoteDataJSONConverter.Instance.JSONToNoteData(GlobalData.Instance.noteDataJson);

                BeatManager.Instance.GameSpeedNotRelatedBPM = (UserSettings.UserGameSpeed * 200.0f);

                MusicManager.Instance.Music = GlobalData.Instance.musicClip;
                MusicManager.Instance.PlayMusic();
            }

            // MusicManager.Instance.Music = MusicManager.Instance.MusicAudioSource.clip;
            // MusicManager.Instance.PlayMusic();
            
            MusicManager.Instance.MusicSyncDelayTime = (float)-UserSettings.UserMusicSyncDelayMs / 1000.0f;
            MusicManager.Instance.MusicPosition = -StartDelayTime;
            BeatManager.Instance.GotoStartDelayTime(-StartDelayTime);

            totalCombo = NoteTickCalculator.CalculateTotalTick(NoteManager.Instance.allNotes, BeatManager.Instance.BPMInfos, out int longNoteTotalCombo);

            clearGaugeType = UserSettings.ClearGaugeType;
            isClearGaugeLifeType = (clearGaugeType == ClearGaugeType.NormalLife || 
                                    clearGaugeType == ClearGaugeType.HardLife ||
                                    clearGaugeType == ClearGaugeType.FullCombo);
            if (isClearGaugeLifeType)
                clearGauge = 100.0f;
            ClearGaugeCalculator.CalculateResult(clearGaugeType, totalCombo, longNoteTotalCombo, 
                out JudgeManager.Instance.clearGaugeIncreaseAmount, out JudgeManager.Instance.clearGaugeDecreaseAmount);
        }

        private void Update()
        {
            UpdateScore();
            /*
            Debug.Log("MT : " + MusicManager.Instance.MusicPosition +
                      "RMT : " + MusicManager.Instance.MusicAudioSource.time +
                      ", BT : " + BeatManager.Instance.GameTime + 
                      ", MB D : " + (BeatManager.Instance.GameTime - MusicManager.Instance.MusicPosition) +
                      ", RM D : " + (MusicManager.Instance.MusicAudioSource.time - MusicManager.Instance.MusicPosition));
            */

            float timeDiff = BeatManager.Instance.GameTime - MusicManager.Instance.MusicPosition;
            if (MusicManager.Instance.IsPlaying && Mathf.Abs(timeDiff) >= 0.050001f)
            {
                Debug.LogWarning("Fixed times");
                BeatManager.Instance.GameTime = MusicManager.Instance.MusicPosition;
            }
            // BeatManager.Instance.GameTime = Mathf.Lerp(BeatManager.Instance.GameTime, MusicManager.Instance.MusicPosition, Time.deltaTime * 2.0f);

            if (!MusicManager.Instance.IsPlaying && MusicManager.Instance.MusicPosition > 0.5f)
            {
                FInishGame();
            }

            if (isClearGaugeLifeType && clearGauge <= 0.0f)
            {
                FInishGame();
            }
        }

        private void UpdateScore()
        {
            score = (int)(1000000.0f * ((float)(judgePerfectCount * 2 + judgeNiceCount) / (float)(totalCombo * 2)));
        }

        private void FInishGame()
        {
            GlobalData.Instance.lastMaxCombo = maxCombo;
            GlobalData.Instance.lastScore = score;
            GlobalData.Instance.lastJudgePerfectCount = judgePerfectCount;
            GlobalData.Instance.lastJudgeNiceCount = judgeNiceCount;
            GlobalData.Instance.lastJudgeMissCount = judgeMissCount;
            GlobalData.Instance.lastTotalCombo = totalCombo;
            GlobalData.Instance.lastClearGaugeType = clearGaugeType;
            GlobalData.Instance.lastClearGauge = clearGauge;

            SceneManager.LoadScene("Result");
        }
    }
}