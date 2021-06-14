using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;


namespace PopTheCircle.Game
{
    public class MusicUserScoreSetManager : Singleton<MusicUserScoreSetManager>
    {
        public static readonly string UserScoreSetFilePath = "UserScores.dat";

        [InspectorReadOnly] public List<MusicUserScoreSet> userScoreSets;



        protected override void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(this.gameObject);
                return;
            }

            base.Awake();
            DontDestroyOnLoad(gameObject);
        }

        public void CreateAllScoreSetsFromNoteDatas(List<NoteDataSet> _noteDatas)
        {
            if (userScoreSets == null)
                return;

            foreach (var noteData in _noteDatas)
            {
                MusicUserScoreSet targetSet = userScoreSets.Find((s) => (
                    s.targetMusicTitle.Equals(noteData.musicTitle) &&
                    s.targetNoteDifficultyType == noteData.noteDifficultyType
                ));
                if (targetSet == null)
                {
                    targetSet = new MusicUserScoreSet()
                    {
                        targetMusicTitle = noteData.musicTitle,
                        targetNoteDifficultyType = noteData.noteDifficultyType,
                    };
                    userScoreSets.Add(targetSet);
                }
                else
                {
                    // Blank
                }
            }
        }

        public void LoadUserScores()
        {
            userScoreSets = new List<MusicUserScoreSet>();

            JSONObject userScoreRootJson = SaveLoad.LoadUserScoreJSON(Path.Combine(Application.dataPath, UserScoreSetFilePath));
            if (userScoreRootJson == null || !userScoreRootJson.IsArray)
                return;

            List<JSONObject> userScoresJson = userScoreRootJson.list;
            if (userScoresJson == null)
                return;

            foreach (var userScoreJson in userScoresJson)
            {
                MusicUserScoreSet userScoreSet = new MusicUserScoreSet();
                if (userScoreSet.FromJson(userScoreJson))
                    userScoreSets.Add(userScoreSet);
            }
        }

        public void SaveUserScores()
        {
            if (userScoreSets == null || userScoreSets.Count == 0)
                return;

            JSONObject rootJson = new JSONObject();
            foreach (var userScoreSet in userScoreSets)
            {
                JSONObject userScoreJson = userScoreSet.ToJson();
                if (userScoreJson != null)
                    rootJson.Add(userScoreJson);
            }

            SaveLoad.SaveUserScoreJSON(Path.Combine(Application.dataPath, UserScoreSetFilePath), rootJson);
        }
    }
}