using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using static PopTheCircle.Game.NoteDataSet;

namespace PopTheCircle.Game
{
    public class MusicSelectManager : Singleton<MusicSelectManager>
    {
        [InspectorReadOnly] public bool isStartingGame = false;
        [InspectorReadOnly] public MusicInfoSet selectedMusicInfoSet = null;
        [InspectorReadOnly] public NoteDifficultyType selectedDiff = NoteDifficultyType.Unknown;

        private bool isMusicLoaded = false;



        protected override void Awake()
        {
            base.Awake();

            UserSettings.LoadUserSettings();
        }

        public void StartGame()
        {
            if (selectedMusicInfoSet == null || selectedDiff == NoteDifficultyType.Unknown)
                return;

            StartCoroutine(StartGameCoroutine());
        }

        private IEnumerator StartGameCoroutine()
        {
            isStartingGame = true;

            NoteDataSet targetSet = selectedMusicInfoSet.GetNoteDataWithDifficultyType(selectedDiff);
            if (targetSet == null)
            {
                isStartingGame = false;
                yield break;
            }

            if (FindObjectOfType<GlobalData>() == null)
            {
                GameObject globalDataObj = new GameObject("PopTheCircle.Game.GlobalData");
                globalDataObj.AddComponent<GlobalData>();
            }

            isMusicLoaded = false;
            StartCoroutine(SaveLoad.LoadMusicFile(targetSet.musicFilePath, targetSet.dataFullPath, OnMusicLoaded));
            while (!isMusicLoaded)
                yield return null;

            GlobalData.Instance.noteDataJson = SaveLoad.LoadNoteDataJSON(targetSet.dataFullPath);

            GlobalData.Instance.lastNoteDataSet = targetSet;
            GlobalData.Instance.lastNoteDifficultyType = selectedDiff;
            GlobalData.Instance.lastUserScoreSet = selectedMusicInfoSet.GetUserScoreWithDifficultyType(selectedDiff);

            SceneManager.LoadScene("Game");
        }

        private void OnMusicLoaded(AudioClip _clip)
        {
            GlobalData.Instance.musicClip = _clip;
            isMusicLoaded = true;
        }
        
    }
}