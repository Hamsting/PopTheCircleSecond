using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static PopTheCircle.Game.NoteDataSet;

namespace PopTheCircle.Game
{
    public class MusicInfoUIItem : MonoBehaviour
    {
        private static readonly Color DifficultyDisabledColor = new Color32(144, 144, 144, 255);

        public Button itemButton;

        public Image jacketImage;

        public Text titleText;
        public Text artistText;
        public Text bpmText;

        public Image diffNormalBg;
        public Text  diffNormalLevelText;
        public Text  diffNormalRankText;
        public Text  diffNormalClearText;

        public Image diffEnhancedBg;
        public Text  diffEnhancedLevelText;
        public Text  diffEnhancedRankText;
        public Text  diffEnhancedClearText;

        public Image diffExtremeBg;
        public Text  diffExtremeLevelText;
        public Text  diffExtremeRankText;
        public Text  diffExtremeClearText;

        public Image diffSpecialBg;
        public Text  diffSpecialLevelText;
        public Text  diffSpecialRankText;
        public Text  diffSpecialClearText;




        public void FromMusicInfoSet(MusicInfoSet _set)
        {
            titleText.text = _set.musicTitle;
            artistText.text = _set.musicArtist;
            bpmText.text = $"BPM : {_set.musicDisplayBpm}";

            NoteDataSet normalNoteSet = _set.GetNoteDataWithDifficultyType(NoteDifficultyType.Normal);
            MusicUserScoreSet normalScoreSet = _set.GetUserScoreWithDifficultyType(NoteDifficultyType.Normal);
            if (normalNoteSet != null)
            {
                diffNormalLevelText.text = normalNoteSet.noteDifficultyLevel.ToString("D02");
                diffNormalRankText.text = (normalScoreSet != null) ? MusicScoreRankToString(normalScoreSet.userScoreRank) : "";
                diffNormalClearText.text = (normalScoreSet != null) ? MusicClearTypeToString(normalScoreSet.userClearType) : "";
            }
            else
            {
                diffNormalBg.color = DifficultyDisabledColor;
                diffNormalLevelText.text = "";
                diffNormalRankText.text = "";
                diffNormalClearText.text = "";
            }

            NoteDataSet enhancedNoteSet = _set.GetNoteDataWithDifficultyType(NoteDifficultyType.Enhanced);
            MusicUserScoreSet enhancedScoreSet = _set.GetUserScoreWithDifficultyType(NoteDifficultyType.Enhanced);
            if (enhancedNoteSet != null)
            {
                diffEnhancedLevelText.text = enhancedNoteSet.noteDifficultyLevel.ToString("D02");
                diffEnhancedRankText.text = (enhancedScoreSet != null) ? MusicScoreRankToString(enhancedScoreSet.userScoreRank) : "";
                diffEnhancedClearText.text = (enhancedScoreSet != null) ? MusicClearTypeToString(enhancedScoreSet.userClearType) : "";
            }
            else
            {
                diffEnhancedBg.color = DifficultyDisabledColor;
                diffEnhancedLevelText.text = "";
                diffEnhancedRankText.text = "";
                diffEnhancedClearText.text = "";
            }

            NoteDataSet extremeNoteSet = _set.GetNoteDataWithDifficultyType(NoteDifficultyType.Extreme);
            MusicUserScoreSet extremeScoreSet = _set.GetUserScoreWithDifficultyType(NoteDifficultyType.Extreme);
            if (extremeNoteSet != null)
            {
                diffExtremeLevelText.text = extremeNoteSet.noteDifficultyLevel.ToString("D02");
                diffExtremeRankText.text = (extremeScoreSet != null) ? MusicScoreRankToString(extremeScoreSet.userScoreRank) : "";
                diffExtremeClearText.text = (extremeScoreSet != null) ? MusicClearTypeToString(extremeScoreSet.userClearType) : "";
            }
            else
            {
                diffExtremeBg.color = DifficultyDisabledColor;
                diffExtremeLevelText.text = "";
                diffExtremeRankText.text = "";
                diffExtremeClearText.text = "";
            }

            NoteDataSet specialNoteSet = _set.GetNoteDataWithDifficultyType(NoteDifficultyType.Special);
            MusicUserScoreSet specialScoreSet = _set.GetUserScoreWithDifficultyType(NoteDifficultyType.Special);
            if (specialNoteSet != null)
            {
                diffSpecialLevelText.text = specialNoteSet.noteDifficultyLevel.ToString("D02");
                diffSpecialRankText.text = (specialScoreSet != null) ? MusicScoreRankToString(specialScoreSet.userScoreRank) : "";
                diffSpecialClearText.text = (specialScoreSet != null) ? MusicClearTypeToString(specialScoreSet.userClearType) : "";
            }
            else
            {
                diffSpecialBg.color = DifficultyDisabledColor;
                diffSpecialLevelText.text = "";
                diffSpecialRankText.text = "";
                diffSpecialClearText.text = "";
            }

            // 자켓은 임시
            jacketImage.sprite = normalNoteSet?.musicJacketSprite ?? null;
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