using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEngine;

namespace PopTheCircle.Game
{
    public class NoteDataSetManager : Singleton<NoteDataSetManager>
    {
        [InspectorReadOnly] public List<NoteDataSet> loadedNoteDataSets = new List<NoteDataSet>();
        [InspectorReadOnly] public List<MusicInfoSet> musicInfoSets;
        [InspectorReadOnly] public bool isNoteDataPathVerified = false;
        [InspectorReadOnly] public bool isNoteDataLoaded = false;

        private Thread t;
        private bool isTheadTaskFinished = false;



        protected override void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(this.gameObject);
                return;
            }

            base.Awake();
            isNoteDataLoaded = false;
            isTheadTaskFinished = false;
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            StartLoadingNoteData();
        }

        private void Update()
        {
            if (!isNoteDataLoaded && isTheadTaskFinished)
            {
                MusicUserScoreSetManager.instance.LoadUserScores();
                MusicUserScoreSetManager.instance.CreateAllScoreSetsFromNoteDatas(loadedNoteDataSets);
                CreateMusicDataSets();

                isNoteDataLoaded = true;
            }
        }

        public void StartLoadingNoteData()
        {
            isNoteDataLoaded = false;
            isTheadTaskFinished = false;

            t = new Thread(LoadNoteDataTask);
            t.Start();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            if (t != null && t.IsAlive)
            {
                t.Abort();
                t = null;
            }
        }

        private void LoadNoteDataTask()
        {
            try
            {
                isNoteDataPathVerified = VerifyNoteDataRootPath();
                if (isNoteDataPathVerified)
                    FindAllNoteDatas();
            }
            catch (ThreadAbortException _tae)
            {
            }
            catch (Exception _e)
            {
                Debug.LogError(_e);
            }
            finally
            {
                isTheadTaskFinished = true;
            }
        }

        private bool VerifyNoteDataRootPath()
        {
            string rootPath = UserSettings.NoteDataRootPath;
            if (string.IsNullOrEmpty(rootPath))
                return false;

            DirectoryInfo dirInfo = new DirectoryInfo(rootPath);
            if (!dirInfo.Exists)
                return false;

            return true;
        }

        private void FindAllNoteDatas()
        {
            loadedNoteDataSets = new List<NoteDataSet>();

            DirectoryInfo rootDirInfo = new DirectoryInfo(UserSettings.NoteDataRootPath);
            var fileInfos = rootDirInfo.GetFiles("*.ntd", SearchOption.AllDirectories);
            foreach (var fileInfo in fileInfos)
            {
                var noteDataSet = NoteDataSet.FromNoteDataPath(fileInfo.FullName);
                if (noteDataSet != null)
                    loadedNoteDataSets.Add(noteDataSet);
            }
        }

        private void CreateMusicDataSets()
        {
            musicInfoSets = new List<MusicInfoSet>();

            foreach (var nd in loadedNoteDataSets)
            {
                MusicInfoSet targetSet = musicInfoSets.Find((s) => (s.musicTitle.Equals(nd.musicTitle)));
                if (targetSet == null)
                {
                    targetSet = new MusicInfoSet()
                    {
                        musicTitle = nd.musicTitle,
                        musicArtist = nd.musicArtist,
                        musicDisplayBpm = nd.musicDisplayBpm,
                        noteDataSets = new List<NoteDataSet>(),
                    };
                    targetSet.noteDataSets.Add(nd);

                    musicInfoSets.Add(targetSet);
                }
                else
                {
                    targetSet.noteDataSets.Add(nd);
                }
            }

            foreach (var userScoreSet in MusicUserScoreSetManager.instance.userScoreSets)
            {
                MusicInfoSet targetSet = musicInfoSets.Find((s) => (s.musicTitle.Equals(userScoreSet.targetMusicTitle)));
                if (targetSet != null)
                {
                    if (targetSet.userScoreSets == null)
                        targetSet.userScoreSets = new List<MusicUserScoreSet>();
                    targetSet.userScoreSets.Add(userScoreSet);
                }
            }
        }
    }
}