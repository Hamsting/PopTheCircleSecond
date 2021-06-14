using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PopTheCircle.Game.NoteDataSet;

namespace PopTheCircle.Game
{
    [System.Serializable]
    public class MusicInfoSet : System.Object
    {
        public string musicTitle = "";
        public string musicArtist = "";
        public string musicDisplayBpm = "";

        public List<NoteDataSet> noteDataSets;
        public List<MusicUserScoreSet> userScoreSets;



        public NoteDataSet GetNoteDataWithDifficultyType(NoteDifficultyType _diff)
        {
            if (noteDataSets != null && noteDataSets.Count > 0)
                return noteDataSets.Find((s) => (s.noteDifficultyType == _diff));
            else
                return null;
        }

        public MusicUserScoreSet GetUserScoreWithDifficultyType(NoteDifficultyType _diff)
        {
            if (userScoreSets != null && userScoreSets.Count > 0)
                return userScoreSets.Find((s) => (s.targetNoteDifficultyType == _diff));
            else
                return null;
        }
    }

    [System.Serializable]
    public class MusicUserScoreSet : System.Object
    {
        public string targetMusicTitle = "";
        public NoteDifficultyType targetNoteDifficultyType = NoteDifficultyType.Unknown;

        public int userHighScore = 0;
        public MusicClearType userClearType = MusicClearType.NotPlayed;
        public MusicScoreRank userScoreRank = MusicScoreRank.None;



        public bool FromJson(JSONObject _json)
        {
            targetMusicTitle            =                     (_json.GetField("targetMusicTitle")           ?.str ?? "");
            targetNoteDifficultyType    = (NoteDifficultyType)(_json.GetField("targetNoteDifficultyType")   ?.i   ?? 0);

            userHighScore               =                (int)(_json.GetField("userHighScore")              ?.i   ?? 0);
            userClearType               =     (MusicClearType)(_json.GetField("userClearType")              ?.i   ?? 0);
            userScoreRank               =     (MusicScoreRank)(_json.GetField("userScoreRank")              ?.i   ?? 0);

            return (!string.IsNullOrEmpty(targetMusicTitle) && targetNoteDifficultyType != NoteDifficultyType.Unknown);
        }

        public JSONObject ToJson()
        {
            if (string.IsNullOrEmpty(targetMusicTitle) || targetNoteDifficultyType == NoteDifficultyType.Unknown)
                return null;

            JSONObject json = new JSONObject();

            json.SetField("targetMusicTitle", targetMusicTitle);
            json.SetField("targetNoteDifficultyType", (int)targetNoteDifficultyType);

            json.SetField("userHighScore", userHighScore);
            json.SetField("userClearType", (int)userClearType);
            json.SetField("userScoreRank", (int)userScoreRank);

            return json;
        }
    }


    public enum MusicClearType
    {
        NotPlayed = 0,
        Failed = 1,

        EasyCleared = 2,
        NormalCleared = 3,
        HardCleared = 4,

        NormalLifeCleared = 7,
        HardLifeCleared = 8,

        FullComboCleared = 11,
        PerfectCleared = 21,
    }

    public enum MusicScoreRank
    {
        None    =       0,
        F       =       1, 
        D       =  500000,
        C       =  700000,
        B       =  800000,
        A       =  900000,
        AA      =  920000,
        AAA     =  940000,
        S       =  950000,
        SS      =  970000,
        SSS     =  990000,
        P       = 1000000,
    }
}