using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static PopTheCircle.Game.NoteDataSet;

namespace PopTheCircle.Game
{
    public class MusicSelectUI : MonoBehaviour
    {
        [Header("Selected Music Info")]
        public SelectedMusicInfoUI selectedMusicInfoUI;
        public Button musicStartButton;
        public Text userSettingsInfoText;

        [Header("MusicInfo List")]
        public Transform musicInfoListContents;
        public GameObject musicInfoPrefab;

        [Header("Game Settings")]
        public Text gameSpeedLabel;
        public Slider gameSpeedSlider;
        public Text noteScaleLabel;
        public Slider noteScaleSlider;
        public Text musicDelayLabel;
        public Slider musicDelaySlider;
        public Dropdown clearGaugeTypeDropdown;
        public Dropdown noteAppearTypeDropdown;

        [Header("NoteDataRootPath")]
        public GameObject noteDataPathSettingPopup;
        public Text noteDataRootPathText;

        [Header("Loading Cirlce")]
        public GameObject loadingCircle;

        private List<MusicInfoUIItem> musicInfoItems;
        private bool isMusicItemLoaded = false;



        private void Awake()
        {
            isMusicItemLoaded = false;

            SelectMusicInfoSet(null);
        }

        private void Update()
        {
            bool needLoadingCircle = MusicSelectManager.Instance.isStartingGame || !NoteDataSetManager.Instance.isNoteDataLoaded;
            if (loadingCircle.activeSelf != needLoadingCircle)
                loadingCircle.SetActive(needLoadingCircle);

            if (NoteDataSetManager.Instance.isNoteDataLoaded)
            {
                if (!NoteDataSetManager.Instance.isNoteDataPathVerified && !noteDataPathSettingPopup.activeSelf)
                    AppearNoteDataRootPathSettingPopup();
                else if (!isMusicItemLoaded)
                    CreateMusicInfoItems();
            }
        }

        private void AppearNoteDataRootPathSettingPopup()
        {
            noteDataPathSettingPopup.SetActive(true);
            noteDataRootPathText.text = UserSettings.NoteDataRootPath;
        }

        public void ApplyNoteDataRootPath()
        {
            UserSettings.NoteDataRootPath = noteDataRootPathText.text;
            UserSettings.SaveUserSettings();

            noteDataPathSettingPopup.SetActive(false);

            NoteDataSetManager.Instance.StartLoadingNoteData();
        }

        private void CreateMusicInfoItems()
        {
            musicInfoItems = new List<MusicInfoUIItem>();

            foreach (var set in NoteDataSetManager.Instance.musicInfoSets)
            {
                GameObject itemObj = GameObject.Instantiate<GameObject>(musicInfoPrefab, musicInfoListContents);

                MusicInfoUIItem item = itemObj.GetComponent<MusicInfoUIItem>();
                item.FromMusicInfoSet(set);
                item.itemButton.onClick.AddListener(delegate ()
                {
                    SelectMusicInfoSet(set);
                });

                musicInfoItems.Add(item);
            }

            isMusicItemLoaded = true;
        }

        public void SelectMusicInfoSet(MusicInfoSet _set)
        {
            SelectNoteDifficulty(0);
            MusicSelectManager.Instance.selectedMusicInfoSet = _set;

            if (_set != null)
                selectedMusicInfoUI.InitFromMusicInfoSet(_set);
            else
                selectedMusicInfoUI.ClearInfo();
        }
        
        public void SelectNoteDifficulty(int _diff)
        {
            selectedMusicInfoUI.HighlightNoteDifficulty(_diff);
            MusicSelectManager.Instance.selectedDiff = (NoteDifficultyType)_diff;

            if (_diff == 0)
            {
                musicStartButton.interactable = false;
                userSettingsInfoText.text = "";
            }
            else
            {
                musicStartButton.interactable = true;
                userSettingsInfoText.text = 
                    $"Speed {UserSettings.UserGameSpeed.ToString("F0")} / " +
                    $"Note Scale {UserSettings.UserNoteScale.ToString("F01")} / " +
                    $"Music Delay {UserSettings.UserMusicSyncDelayMs.ToString()}ms";
            }
        }

        public void StartMusic()
        {
            MusicSelectManager.Instance.StartGame();
        }

        public void InitSettingsPopup()
        {
            gameSpeedSlider.value = (int)(UserSettings.UserGameSpeed * 10.0f);
            noteScaleSlider.value = (int)(UserSettings.UserNoteScale * 2.0f);
            musicDelaySlider.value = UserSettings.UserMusicSyncDelayMs;

            gameSpeedLabel.text = $"Game Speed : {UserSettings.UserGameSpeed.ToString("F01")}";
            noteScaleLabel.text = $"Note Scale : {UserSettings.UserNoteScale.ToString("F01")}x";
            musicDelayLabel.text = $"Music Delay : {UserSettings.UserMusicSyncDelayMs}ms";

            clearGaugeTypeDropdown.value = ClearGaugeTypeEnumToDropdownValue(UserSettings.ClearGaugeType);
            noteAppearTypeDropdown.value = (int)UserSettings.NoteAppearType;
        }

        public void SaveSettings()
        {
            UserSettings.SaveUserSettings();
        }

        public void OnGameSpeedValueChanged()
        {
            UserSettings.UserGameSpeed = (float)gameSpeedSlider.value * 0.1f;
            gameSpeedLabel.text = $"Game Speed : {UserSettings.UserGameSpeed.ToString("F01")}";
        }

        public void OnNoteScaleValueChanged()
        {
            UserSettings.UserNoteScale = (float)noteScaleSlider.value * 0.5f;
            noteScaleLabel.text = $"Note Scale : {UserSettings.UserNoteScale.ToString("F01")}x";
        }

        public void OnMusicDelayValueChanged()
        {
            UserSettings.UserMusicSyncDelayMs = (int)musicDelaySlider.value;
            musicDelayLabel.text = $"Music Delay : {UserSettings.UserMusicSyncDelayMs}ms";
        }

        public void OnClearGaugeTypeValueChanged()
        {
            UserSettings.ClearGaugeType = ClearGaugeTypeDropdownValueToEnum(clearGaugeTypeDropdown.value);
        }

        private static ClearGaugeType ClearGaugeTypeDropdownValueToEnum(int _value)
        {
            switch (_value)
            {
                case 0:
                    return ClearGaugeType.Easy;
                case 1:
                default:
                    return ClearGaugeType.Normal;
                case 2:
                    return ClearGaugeType.Hard;
                case 3:
                    return ClearGaugeType.NormalLife;
                case 4:
                    return ClearGaugeType.HardLife;
                case 5:
                    return ClearGaugeType.FullCombo;
            }
        }

        private static int ClearGaugeTypeEnumToDropdownValue(ClearGaugeType _value)
        {
            switch (_value)
            {
                case ClearGaugeType.Easy:
                    return 0;
                case ClearGaugeType.Normal:
                default:
                    return 1;
                case ClearGaugeType.Hard:
                    return 2;
                case ClearGaugeType.NormalLife:
                    return 3;
                case ClearGaugeType.HardLife:
                    return 4;
                case ClearGaugeType.FullCombo:
                    return 5;
            }
        }

        public void OnNoteAppearTypeValueChanged()
        {
            UserSettings.NoteAppearType = (NoteAppearType)noteAppearTypeDropdown.value;
        }
    }
}