using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PopTheCircle.NoteEditor
{
    public class OptionMenuUI : MonoBehaviour
    {
        [Header("NoteDataInfo")]
        public InputField musicFilePathInput;
        public InputField musicTitleInput;
        public InputField musicArtistInput;
        public InputField musicBPMInput;
        public InputField musicStartTime;
        public InputField standardBPM;
        [HideInInspector]
        public MakerUIManager ui;


        public void LoadNoteDataInfo()
        {
            NoteData nd = MakerManager.Instance.noteData;

            musicFilePathInput.text = nd.musicFilePath;
            musicTitleInput.text = nd.musicTitle;
            musicArtistInput.text = nd.musicArtist;
            musicBPMInput.text = nd.musicBPM;
            musicStartTime.text = nd.musicStartTime.ToString();
            standardBPM.text = nd.standardBPM.ToString();
        }

        public void ApplyNoteDataInfo(bool saveDataFromInput = true)
        {
            NoteData nd = MakerManager.Instance.noteData;

            // 데이터 적용
            if (saveDataFromInput)
            {

                nd.musicFilePath = musicFilePathInput.text;
                nd.musicTitle = musicTitleInput.text;
                nd.musicArtist = musicArtistInput.text;
                nd.musicBPM = musicBPMInput.text;

                nd.musicStartTime = ParseInt(musicStartTime.text, 0, true);
                nd.standardBPM = ParseFloat(standardBPM.text, 60.0f, true);
            }

            // 이후 처리
            ui.StartCoroutine(MusicLoadingCoroutine(nd.musicFilePath));
            MusicManager.Instance.MusicStartTime = (float)nd.musicStartTime * 0.001f;
        }

        private IEnumerator MusicLoadingCoroutine(string _path)
        {
            yield return null;
            ui.loadingPanel.SetActive(true);
            if (!string.IsNullOrEmpty(_path))
            {
                Coroutine loadCoroutine = ui.StartCoroutine(SaveLoad.LoadMusicFile(_path, OnMusicLoaded));
                yield return loadCoroutine;
            }
            ui.loadingPanel.SetActive(false);
            if (this.gameObject.activeSelf)
                ui.ToggleOptionMenu();
            yield return null;
        }

        private void OnMusicLoaded(AudioClip _clip)
        {
            MusicManager.Instance.Music = _clip;
            ui.UpdateDefaultMenuUI();
        }

        private int ParseInt(string _str, int _default = 0, bool _allowNegative = true)
        {
            if (string.IsNullOrEmpty(_str))
                return _default;
            int res = _default;
            int.TryParse(_str, out res);
            if (!_allowNegative && res < 0)
                res = 0;
            return res;
        }

        private float ParseFloat(string _str, float _default = 0.0f, bool _allowNegative = true)
        {
            if (string.IsNullOrEmpty(_str))
                return _default;
            float res = _default;
            float.TryParse(_str, out res);
            if (!_allowNegative && res < 0.0f)
                res = 0.0f;
            return res;
        }
    }
}