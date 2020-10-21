using GracesGames.SimpleFileBrowser.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace PopTheCircle.NoteEditor
{
    public class MakerUIManager : Singleton<MakerUIManager>
    {
        [Header("Default Menu UI")]
        public Text noteDataFilePathText;
        public Text musicLengthText;
        public Text musicTimeText;
        public Slider musicPositionSlider;
        public Button expandMenuButton;
        public GameObject loadingPanel;

        [Header("Other Menu")]
        public GameObject developerMenu;
        public ExpandMenuUI expandMenu;
        public OptionMenuUI optionMenu;
        public PopupUI popup;



        private void Start()
        {
            optionMenu.ui = this;
            optionMenu.LoadVolumeSettings();

            UpdateDefaultMenuUI();
        }

        private void LateUpdate()
        {
            if (MusicManager.Instance.isMusicLoaded)
            {
                musicPositionSlider.value = BeatManager.Instance.GameTime;
                musicTimeText.text = TimeToString(BeatManager.Instance.GameTime);
            }
            else
            {
                musicPositionSlider.normalizedValue = 0.0f;
                musicTimeText.text = "00:00.000";
            }
        }

        public void PlayMusic()
        {
            MusicManager.Instance.MusicPosition = BeatManager.Instance.GameTime;
            MusicManager.Instance.PlayMusic();
            BeatManager.Instance.IsPlaying = true;
        }

        public void PauseMusic()
        {
            MusicManager.Instance.PauseMusic();
            BeatManager.Instance.IsPlaying = false;
        }

        public void StopMusic()
        {
            MusicManager.Instance.StopMusic();
            BeatManager.Instance.IsPlaying = false;
            BeatManager.Instance.GotoTime(0.0f);
            musicPositionSlider.normalizedValue = 0.0f;
        }

        public void ToggleExpandMenu()
        {
            expandMenu.gameObject.SetActive(!expandMenu.gameObject.activeSelf);
            expandMenuButton.transform.localEulerAngles = new Vector3(0.0f, 0.0f, (expandMenu.gameObject.activeSelf) ? 180.0f : 0.0f);
        }

        public void ToggleOptionMenu()
        {
            optionMenu.gameObject.SetActive(!optionMenu.gameObject.activeSelf);
        }

        public void ChangeBarGrid(int _index)
        {
            NoteRailManager.Instance.UpdateBarGrid(_index);
            MakerManager.Instance.barGrid = MakerManager.BarGrids[_index];
        }
                
        public void ApplyNoteDataInfo()
        {
            optionMenu.ApplyNoteDataInfo();
        }

        public void LoadNoteDataInfo()
        {
            optionMenu.LoadNoteDataInfo();
        }

        public void NewNoteDataFile()
        {
            SceneManager.LoadScene("Blank");
        }

        public void OpenNoteDataFile()
        {
            BrowserManager.Instance.OpenFileBrowser(FileBrowserMode.Load, MakerManager.Instance.LoadNoteData);
        }

        public void SaveNoteDataFile()
        {
            if (string.IsNullOrEmpty(MakerManager.Instance.noteDataFilePath) ||
                MakerManager.Instance.noteDataFilePath.Equals("Untitled.ntd"))
                SaveNoteDataFileAsNew();
            else
                MakerManager.Instance.SaveNoteData(MakerManager.Instance.noteDataFilePath);
        }

        public void SaveNoteDataFileAsNew()
        {
            BrowserManager.Instance.OpenFileBrowser(FileBrowserMode.Save, MakerManager.Instance.SaveNoteData);
        }

        public void UpdateDefaultMenuUI()
        {
            noteDataFilePathText.text = MakerManager.Instance.noteDataFilePath;
            if (MusicManager.Instance.isMusicLoaded)
            {
                float len = MusicManager.Instance.MusicLength;
                musicLengthText.text = TimeToString(len);
                musicPositionSlider.minValue = 0.0f;
                musicPositionSlider.maxValue = len;
            }
            else
            {
                musicLengthText.text = "--:--.---";
                musicPositionSlider.minValue = 0.0f;
                musicPositionSlider.maxValue = 1.0f;
            }
        }

        private string TimeToString(float _time)
        {
            int min = (int)(_time / 60.0f);
            int sec = (int)(_time % 60.0f);
            int mil = (int)(((_time % 60.0f) - (float)sec) * 1000.0f);
            return min.ToString("D02") + ":" + sec.ToString("D02") + "." + mil.ToString("D03");
        }

        public void SyncUpload()
        {
            if (string.IsNullOrEmpty(NoteDataSyncManager.Instance.syncRootPath))
                SetSyncPath();
            else
                NoteDataSyncManager.Instance.StartSyncUpload();
        }

        public void SyncDownload()
        {
            if (string.IsNullOrEmpty(NoteDataSyncManager.Instance.syncRootPath))
                SetSyncPath();
            else
                NoteDataSyncManager.Instance.StartSyncDownload();
        }

        public void SetSyncPath()
        {
            BrowserManager.Instance.OpenSyncPathSelectBrowser(NoteDataSyncManager.Instance.SetSyncRootPath);
        }




        // Developer Menu Functions

        public void NoteFitToGrid()
        {
            foreach (var note in NoteManager.Instance.notes)
            {
                if ( note.noteType == NoteType.Long ||
                    (note.noteType == NoteType.Effect && ((EffectNote)note).IsLongType) ||
                    (note.noteType == NoteType.Space && ((SpaceNote)note).IsLongType))
                    PopTheCircle.OldNoteDataConverter.ConvertedNoteFitter.FitLongNoteToGrid((LongNote)note);
                else
                    PopTheCircle.OldNoteDataConverter.ConvertedNoteFitter.FitNoteToGrid(note);
            }
            NoteManager.Instance.UpdateNoteSpawn();
        }

        public void PushNoteOneBar()
        {
            foreach (var note in NoteManager.Instance.notes)
            {
                if (note.noteType == NoteType.Long ||
                    (note.noteType == NoteType.Effect && ((EffectNote)note).IsLongType) ||
                    (note.noteType == NoteType.Space && ((SpaceNote)note).IsLongType))
                {
                    LongNote longNote = (LongNote)note;
                    longNote.bar += 1;
                    longNote.endBar += 1;
                }
                else
                {
                    note.bar += 1;
                }
            }
            foreach (var effNote in NoteManager.Instance.effectNotes)
            {
                effNote.bar += 1;
            }
            NoteManager.Instance.UpdateNoteSpawn();
        }
    }
}