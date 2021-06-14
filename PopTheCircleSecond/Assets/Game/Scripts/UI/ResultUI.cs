using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace PopTheCircle.Game
{
    public class ResultUI : MonoBehaviour
    {
        [Header("Music Info")]
        public Image musicJacketImage;
        public Text musicTitleText;
        public Text musicArtistText;
        public Text musicBpmText;
        public Image noteDifficultyBgImage;
        public Text noteDifficultyTypeText;
        public Text noteDifficultyLevelText;

        [Header("Rank And Score")]
        public Text rankText;
        public Text scoreText;
        public Text scoreDiffText;
        public GameObject newRecordImage;

        [Header("Gauge Clear Info")]
        public Image gaugeBgImage;
        public Text gaugeTypeText;
        public Text gaugePercentText;
        public Image gaugePreviewImage;
        public GameObject gaugePreviewGoalImage;

        [Header("Judge Result")]
        public Text judgePerText;
        public Text judgeGreText;
        public Text judgeMissText;
        public Text maxComboText;

        private MusicScoreRank resRank;
        private int resScore;
        private MusicClearType resClearType;
        private bool isCleared = false;
        private bool isFullComboCleared = false;
        private bool isPerfectCleared = false;


        private void Awake()
        {
            Init();
        }

        private void Init()
        {
            var noteDataSet = GlobalData.Instance.lastNoteDataSet;
            if (noteDataSet != null)
            {
                musicJacketImage.sprite = noteDataSet.musicJacketSprite;
                musicTitleText.text = noteDataSet.musicTitle;
                musicArtistText.text = noteDataSet.musicArtist;
                musicBpmText.text = "BPM : " + noteDataSet.musicDisplayBpm;

                noteDifficultyBgImage.color = Color.white; // TEMP
                noteDifficultyTypeText.text = noteDataSet.noteDifficultyType.ToString();
                noteDifficultyLevelText.text = noteDataSet.noteDifficultyLevel.ToString("D02");
            }
            else
            {
                musicJacketImage.sprite = null;
                musicTitleText.text = "";
                musicArtistText.text = "";
                musicBpmText.text = "";

                noteDifficultyBgImage.color = Color.white;
                noteDifficultyTypeText.text = "";
                noteDifficultyLevelText.text = "-";
            }


            resRank = CalculateRank(GlobalData.Instance.lastScore);
            rankText.text = resRank.ToString();
            resScore = GlobalData.Instance.lastScore;
            scoreText.text = MusicUserScoreToString(resScore);

            int scoreDiff = resScore - (GlobalData.Instance.lastUserScoreSet?.userHighScore ?? 0);
            scoreDiffText.text = (Mathf.Sign(scoreDiff) > 0.0f ? "+" : "-") + " " + MusicUserScoreToString(scoreDiff);



            // gaugeBgImage;
            // gaugePreviewImage;
            gaugePreviewImage.transform.localScale = new Vector3(GlobalData.Instance.lastClearGauge / 100.0f, 1.0f, 1.0f);

            isCleared = ClearGaugeCalculator.CheckGaugeCleared(GlobalData.Instance.lastClearGaugeType, GlobalData.Instance.lastClearGauge);
            isFullComboCleared = GlobalData.Instance.lastMaxCombo == GlobalData.Instance.lastTotalCombo;
            isPerfectCleared = resRank == MusicScoreRank.P;

            string gaugeTypeTextStr = null;
            if (isPerfectCleared)
            {
                resClearType = MusicClearType.PerfectCleared;
                gaugeTypeTextStr = "PERFECT!!!";
            }
            else if (isFullComboCleared)
            {
                resClearType = MusicClearType.FullComboCleared;
                gaugeTypeTextStr = "FULL COMBO!";
            }
            else
            {
                resClearType = (isCleared) ? (MusicClearType)((int)GlobalData.Instance.lastClearGaugeType) : MusicClearType.Failed;
                gaugeTypeTextStr = $"{ClearGaugeCalculator.ClearGaugeTypeToString(GlobalData.Instance.lastClearGaugeType)} ";
                gaugeTypeTextStr += (isCleared) ? "Cleared" : "Failed";
            }
            gaugeTypeText.text = gaugeTypeTextStr;

            gaugePercentText.text = $"{GlobalData.Instance.lastClearGauge.ToString("F0")}%";
            if (GlobalData.Instance.lastClearGaugeType == ClearGaugeType.Easy ||
                GlobalData.Instance.lastClearGaugeType == ClearGaugeType.Normal ||
                GlobalData.Instance.lastClearGaugeType == ClearGaugeType.Hard)
                gaugePreviewGoalImage.SetActive(true);
            else
                gaugePreviewGoalImage.SetActive(false);



            judgePerText.text = MakeGrayFilledString(GlobalData.Instance.lastJudgePerfectCount, 4);
            judgeGreText.text = MakeGrayFilledString(GlobalData.Instance.lastJudgeNiceCount, 4);
            judgeMissText.text = MakeGrayFilledString(GlobalData.Instance.lastJudgeMissCount, 4);
            maxComboText.text = MakeGrayFilledString(GlobalData.Instance.lastMaxCombo, 4) + 
                " / " + MakeGrayFilledString(GlobalData.Instance.lastTotalCombo, 4);



            SaveScore(out bool isNewRecord);
            newRecordImage.SetActive(isNewRecord);
        }

        public void GoMusicSelectScene()
        {
            SceneManager.LoadScene("MusicSelect");
        }

        private void SaveScore(out bool _isNewRecord)
        {
            _isNewRecord = false;

            var noteDataSet = GlobalData.Instance.lastNoteDataSet;
            if (noteDataSet == null)
                return;

            var userScoreSet = MusicUserScoreSetManager.Instance.userScoreSets.Find((s) => (
                s.targetMusicTitle.Equals(noteDataSet.musicTitle) && s.targetNoteDifficultyType == GlobalData.Instance.lastNoteDifficultyType
            ));
            if (userScoreSet == null)
                return;

            
            if (resScore > userScoreSet.userHighScore)
            {
                userScoreSet.userScoreRank = resRank;
                userScoreSet.userHighScore = resScore;
                _isNewRecord = true;
            }

            if (isCleared)
            {
                if ((int)resClearType > (int)userScoreSet.userClearType)
                    userScoreSet.userClearType = resClearType;
            }
            else
            {
                if (userScoreSet.userClearType == MusicClearType.NotPlayed)
                    userScoreSet.userClearType = MusicClearType.Failed;
            }

            MusicUserScoreSetManager.Instance.SaveUserScores();
        }

        private static MusicScoreRank CalculateRank(int _score)
        {
            if      (_score >= (int)MusicScoreRank.P    )
                return              MusicScoreRank.P    ;
            else if (_score >= (int)MusicScoreRank.SSS  )
                return              MusicScoreRank.SSS  ;
            else if (_score >= (int)MusicScoreRank.SS   )
                return              MusicScoreRank.SS   ;
            else if (_score >= (int)MusicScoreRank.S    )
                return              MusicScoreRank.S    ;
            else if (_score >= (int)MusicScoreRank.AAA  )
                return              MusicScoreRank.AAA  ;
            else if (_score >= (int)MusicScoreRank.AA   )
                return              MusicScoreRank.AA   ;
            else if (_score >= (int)MusicScoreRank.A    )
                return              MusicScoreRank.A    ;
            else if (_score >= (int)MusicScoreRank.B    )
                return              MusicScoreRank.B    ;
            else if (_score >= (int)MusicScoreRank.C    )
                return              MusicScoreRank.C    ;
            else if (_score >= (int)MusicScoreRank.D    )
                return              MusicScoreRank.D    ;
            else if (_score >= (int)MusicScoreRank.F    )
                return              MusicScoreRank.F    ;
            else
                return              MusicScoreRank.None ;
        }

        private static string MusicUserScoreToString(int _score)
        {
            int blindNumCharCount = 7;
            int tempScore = Mathf.Abs(_score);
            while (tempScore > 0)
            {
                tempScore /= 10;
                --blindNumCharCount;
            }

            string res = "<color=#AFAFAF>";
            while (blindNumCharCount-- > 0)
                res += "0";
            res += "</color>";
            if (_score != 0)
                res += Mathf.Abs(_score).ToString();

            return res;
        }

        private static string MakeGrayFilledString(int _score, int _defaultFill)
        {
            int blindNumCharCount = _defaultFill;
            int tempScore = Mathf.Abs(_score);
            while (tempScore > 0)
            {
                tempScore /= 10;
                --blindNumCharCount;
            }

            string res = "<color=#AFAFAF>";
            while (blindNumCharCount-- > 0)
                res += "0";
            res += "</color>";
            if (_score != 0)
                res += Mathf.Abs(_score).ToString();

            return res;
        }
    }
}