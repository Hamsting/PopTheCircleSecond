using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static PopTheCircle.Game.NoteDataSet;

namespace PopTheCircle.Game
{
    public class SelectedMusicInfoUI : MonoBehaviour
    {
        private static readonly Color JacketDisabledColor = new Color32(110, 110, 110, 255);


        public Image jacketImage;

        public Text titleText;
        public Text artistText;
        public Text bpmText;

        public Button diffNormalButton;
        public Text diffNormalLevelText;
        public Text diffNormalRankText;
        public Text diffNormalClearText;
        public Text diffNormalScoreText;

        public Button diffEnhancedButton;
        public Text diffEnhancedLevelText;
        public Text diffEnhancedRankText;
        public Text diffEnhancedClearText;
        public Text diffEnhancedScoreText;

        public Button diffExtremeButton;
        public Text diffExtremeLevelText;
        public Text diffExtremeRankText;
        public Text diffExtremeClearText;
        public Text diffExtremeScoreText;

        public Button diffSpecialButton;
        public Text diffSpecialLevelText;
        public Text diffSpecialRankText;
        public Text diffSpecialClearText;
        public Text diffSpecialScoreText;

        public RectTransform selectedDiffImage;



        public void InitFromMusicInfoSet(MusicInfoSet _set)
        {
            titleText.text = _set.musicTitle;
            artistText.text = _set.musicArtist;
            bpmText.text = $"BPM : {_set.musicDisplayBpm}";

            NoteDataSet normalNoteSet = _set.GetNoteDataWithDifficultyType(NoteDifficultyType.Normal);
            MusicUserScoreSet normalScoreSet = _set.GetUserScoreWithDifficultyType(NoteDifficultyType.Normal);
            if (normalNoteSet != null)
            {
                diffNormalButton.interactable = true;
                diffNormalLevelText.text = normalNoteSet.noteDifficultyLevel.ToString("D02");
                diffNormalRankText.text = (normalScoreSet != null) ? MusicScoreRankToString(normalScoreSet.userScoreRank) : "";
                diffNormalClearText.text = (normalScoreSet != null) ? MusicClearTypeToString(normalScoreSet.userClearType) : "";
                diffNormalScoreText.text = (normalScoreSet != null) ? MusicUserScoreToString(normalScoreSet.userHighScore) : "<color=#AFAFAF>0000000</color>";
            }
            else
            {
                diffNormalButton.interactable = false;
                diffNormalLevelText.text = "";
                diffNormalRankText.text = "";
                diffNormalClearText.text = "";
                diffNormalScoreText.text = "";
            }

            NoteDataSet enhancedNoteSet = _set.GetNoteDataWithDifficultyType(NoteDifficultyType.Enhanced);
            MusicUserScoreSet enhancedScoreSet = _set.GetUserScoreWithDifficultyType(NoteDifficultyType.Enhanced);
            if (enhancedNoteSet != null)
            {
                diffEnhancedButton.interactable = true;
                diffEnhancedLevelText.text = enhancedNoteSet.noteDifficultyLevel.ToString("D02");
                diffEnhancedRankText.text = (enhancedScoreSet != null) ? MusicScoreRankToString(enhancedScoreSet.userScoreRank) : "";
                diffEnhancedClearText.text = (enhancedScoreSet != null) ? MusicClearTypeToString(enhancedScoreSet.userClearType) : "";
                diffEnhancedScoreText.text = (enhancedScoreSet != null) ? MusicUserScoreToString(enhancedScoreSet.userHighScore) : "<color=#AFAFAF>0000000</color>";
            }
            else
            {
                diffEnhancedButton.interactable = false;
                diffEnhancedLevelText.text = "";
                diffEnhancedRankText.text = "";
                diffEnhancedClearText.text = "";
                diffEnhancedScoreText.text = "";
            }

            NoteDataSet extremeNoteSet = _set.GetNoteDataWithDifficultyType(NoteDifficultyType.Extreme);
            MusicUserScoreSet extremeScoreSet = _set.GetUserScoreWithDifficultyType(NoteDifficultyType.Extreme);
            if (extremeNoteSet != null)
            {
                diffExtremeButton.interactable = true;
                diffExtremeLevelText.text = extremeNoteSet.noteDifficultyLevel.ToString("D02");
                diffExtremeRankText.text = (extremeScoreSet != null) ? MusicScoreRankToString(extremeScoreSet.userScoreRank) : "";
                diffExtremeClearText.text = (extremeScoreSet != null) ? MusicClearTypeToString(extremeScoreSet.userClearType) : "";
                diffExtremeScoreText.text = (extremeScoreSet != null) ? MusicUserScoreToString(extremeScoreSet.userHighScore) : "<color=#AFAFAF>0000000</color>";
            }
            else
            {
                diffExtremeButton.interactable = false;
                diffExtremeLevelText.text = "";
                diffExtremeRankText.text = "";
                diffExtremeClearText.text = "";
                diffExtremeScoreText.text = "";
            }

            NoteDataSet specialNoteSet = _set.GetNoteDataWithDifficultyType(NoteDifficultyType.Special);
            MusicUserScoreSet specialScoreSet = _set.GetUserScoreWithDifficultyType(NoteDifficultyType.Special);
            if (specialNoteSet != null)
            {
                diffSpecialButton.interactable = true;
                diffSpecialLevelText.text = specialNoteSet.noteDifficultyLevel.ToString("D02");
                diffSpecialRankText.text = (specialScoreSet != null) ? MusicScoreRankToString(specialScoreSet.userScoreRank) : "";
                diffSpecialClearText.text = (specialScoreSet != null) ? MusicClearTypeToString(specialScoreSet.userClearType) : "";
                diffSpecialScoreText.text = (specialScoreSet != null) ? MusicUserScoreToString(specialScoreSet.userHighScore) : "<color=#AFAFAF>0000000</color>";
            }
            else
            {
                diffSpecialButton.interactable = false;
                diffSpecialLevelText.text = "";
                diffSpecialRankText.text = "";
                diffSpecialClearText.text = "";
                diffSpecialScoreText.text = "";
            }

            jacketImage.color = Color.white;
            jacketImage.sprite = normalNoteSet?.musicJacketSprite ?? null;
        }

        public void ClearInfo()
        {
            titleText.text = "";
            artistText.text = "";
            bpmText.text = "";

            diffNormalButton.interactable = false;
            diffNormalLevelText.text = "";
            diffNormalRankText.text = "";
            diffNormalClearText.text = "";
            diffNormalScoreText.text = "";

            diffEnhancedButton.interactable = false;
            diffEnhancedLevelText.text = "";
            diffEnhancedRankText.text = "";
            diffEnhancedClearText.text = "";
            diffEnhancedScoreText.text = "";

            diffExtremeButton.interactable = false;
            diffExtremeLevelText.text = "";
            diffExtremeRankText.text = "";
            diffExtremeClearText.text = "";
            diffExtremeScoreText.text = "";

            diffSpecialButton.interactable = false;
            diffSpecialLevelText.text = "";
            diffSpecialRankText.text = "";
            diffSpecialClearText.text = "";
            diffSpecialScoreText.text = "";

            jacketImage.color = JacketDisabledColor;
            jacketImage.sprite = null;
        }

        public void HighlightNoteDifficulty(int _diff)
        {
            switch (_diff)
            {
                case 1:
                    selectedDiffImage.gameObject.SetActive(true);
                    selectedDiffImage.localPosition = diffNormalButton.transform.localPosition;
                    break;
                case 2:
                    selectedDiffImage.gameObject.SetActive(true);
                    selectedDiffImage.localPosition = diffEnhancedButton.transform.localPosition;
                    break;
                case 3:
                    selectedDiffImage.gameObject.SetActive(true);
                    selectedDiffImage.localPosition = diffExtremeButton.transform.localPosition;
                    break;
                case 4:
                    selectedDiffImage.gameObject.SetActive(true);
                    selectedDiffImage.localPosition = diffSpecialButton.transform.localPosition;
                    break;

                case 0:
                default:
                    selectedDiffImage.gameObject.SetActive(false);
                    break;
            }
        }

        private static string MusicUserScoreToString(int _score)
        {
            int blindNumCharCount = 7;
            int tempScore = _score;
            while (tempScore > 0)
            {
                tempScore /= 10;
                --blindNumCharCount;
            }

            string res = "<color=#AFAFAF>";
            while (blindNumCharCount-- > 0)
                res += "0";
            res += "</color>";
            res += _score.ToString();

            return res;
        }

        private static string MusicClearTypeToString(MusicClearType _type)
        {
            switch (_type)
            {
                case MusicClearType.Failed:
                    return "F";

                case MusicClearType.EasyCleared:
                    return "EC";
                case MusicClearType.NormalCleared:
                    return "NC";
                case MusicClearType.HardCleared:
                    return "HC";

                case MusicClearType.NormalLifeCleared:
                    return "NL";
                case MusicClearType.HardLifeCleared:
                    return "HL";

                case MusicClearType.FullComboCleared:
                    return "FC";
                case MusicClearType.PerfectCleared:
                    return "PC";

                case MusicClearType.NotPlayed:
                default:
                    return "";
            }
        }

        private static string MusicScoreRankToString(MusicScoreRank _rank)
        {
            switch (_rank)
            {
                case MusicScoreRank.F:
                case MusicScoreRank.D:
                case MusicScoreRank.C:
                case MusicScoreRank.B:
                case MusicScoreRank.A:
                case MusicScoreRank.AA:
                case MusicScoreRank.AAA:
                case MusicScoreRank.S:
                case MusicScoreRank.SS:
                case MusicScoreRank.SSS:
                case MusicScoreRank.P:
                    return _rank.ToString();

                case MusicScoreRank.None:
                default:
                    return "";
            }
        }
    }
}