using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PopTheCircle.NoteEditor
{
    public class PopupUI : MonoBehaviour
    {
        public List<GameObject> popups;
        public Image backPanel;

        private bool isWaitForApply = false;
        private bool isApplied = false;



        private GameObject ShowPopup(string _popupName)
        {
            foreach (var p in popups)
            {
                if (p.name.Equals(_popupName))
                {
                    p.SetActive(true);
                    backPanel.enabled = true;
                    return p;
                }
            }
            return null;
        }

        private void ClosePopup(string _popupName)
        {
            foreach (var p in popups)
            {
                if (p.name.Equals(_popupName))
                {
                    p.SetActive(false);
                    backPanel.enabled = false;
                    break;
                }
            }
        }

        public void PopupApply()
        {
            isWaitForApply = false;
            isApplied = true;
        }

        public void PopupCancel()
        {
            isWaitForApply = false;
            isApplied = false;
        }

        public void OpenCTChangePopup(int _bar)
        {
            StartCoroutine(CTChangePopupCoroutine(_bar));
        }

        private IEnumerator CTChangePopupCoroutine(int _bar)
        {
            GameObject popup = ShowPopup("CTChange");
            InputField ctInput = popup.transform.Find("InputField").GetComponent<InputField>();
            ctInput.text = BeatManager.Instance.GetCTWithBarBeat(_bar).ToString();

            isWaitForApply = true;
            while (isWaitForApply)
            {
                yield return null;
            }

            if (isApplied)
            {
                int numerator = int.Parse(ctInput.text);

                int index = BeatManager.Instance.ctInfos.FindIndex((ct) => (ct.bar == _bar));
                if (index >= 0)
                {
                    BeatManager.Instance.ctInfos[index].numerator = numerator;
                    BeatManager.Instance.UpdateRailLengths();
                }
                else
                    BeatManager.Instance.AddNewCTInfo(_bar, numerator);

                NoteManager.Instance.FixIncorrectBarBeatNotes();
                NoteRailManager.Instance.UpdateRailSpawnImmediately();
                BeatManager.Instance.GotoTime(BeatManager.Instance.GameTime);
            }
            
            ClosePopup("CTChange");
        }

        public void OpenBPMChangePopup(int _bar, float _beat)
        {
            StartCoroutine(BPMChangePopupCoroutine(_bar, _beat));
        }

        private IEnumerator BPMChangePopupCoroutine(int _bar, float _beat)
        {
            GameObject popup = ShowPopup("BPMChange");

            BPMInfo curBpmInfo = BeatManager.Instance.GetBPMWithBarBeat(_bar, _beat);

            InputField bpmInput = popup.transform.Find("InputField").GetComponent<InputField>();
            bpmInput.text = curBpmInfo.bpm.ToString("0.###");

            Toggle stopToggle = popup.transform.Find("Toggle").GetComponent<Toggle>();
            stopToggle.isOn = curBpmInfo.stopEffect;

            isWaitForApply = true;
            while (isWaitForApply)
            {
                yield return null;
            }

            if (isApplied)
            {
                float bpm = float.Parse(bpmInput.text);
                bool stopEffect = stopToggle.isOn;

                int index = BeatManager.Instance.BPMInfos.FindIndex((bi) => 
                ((bi.bar == _bar && bi.beat == _beat) ||
                 (bi.beat > 0.0f && _bar == bi.bar + 1 && _beat == 0.0f))
                );
                if (index >= 0)
                {
                    var targetInfo = BeatManager.Instance.BPMInfos[index];
                    targetInfo.bpm = bpm;
                    targetInfo.stopEffect = stopEffect;
                    BeatManager.Instance.UpdateBPMInfo();
                    BeatManager.Instance.UpdateRailLengths();
                }
                else
                    BeatManager.Instance.AddNewBPMInfo(_bar, _beat, bpm, stopEffect);

                NoteManager.Instance.FixIncorrectBarBeatNotes();
                NoteRailManager.Instance.UpdateRailSpawnImmediately();
                BeatManager.Instance.GotoTime(BeatManager.Instance.GameTime);
            }

            ClosePopup("BPMChange");
        }

        public void OpenEventNotePopup(int _bar, float _beat)
        {
            StartCoroutine(EventNotePopupCoroutine(_bar, _beat));
        }

        private IEnumerator EventNotePopupCoroutine(int _bar, float _beat)
        {
            GameObject popup = ShowPopup("EventNote");
            InputField eventNameInput = popup.transform.Find("InputField").GetComponent<InputField>();
            Note existed = NoteManager.Instance.FindEffectNote(_bar, _beat, typeof(EventNote));
            if (existed != null)
                eventNameInput.text = ((EventNote)existed).eventName;
            else
                eventNameInput.text = "";

            isWaitForApply = true;
            while (isWaitForApply)
            {
                yield return null;
            }

            if (isApplied)
            {
                if (existed != null)
                    ((EventNote)existed).eventName = eventNameInput.text;
                else
                {
                    EventNote en = new EventNote()
                    {
                        bar = _bar,
                        beat = _beat,
                        railNumber = 0,
                        eventName = eventNameInput.text
                    };
                    NoteManager.Instance.AddNote(en);
                }
            }

            ClosePopup("EventNote");
        }

        public void OpenEffectNotePopup(EffectNote _effectNote)
        {
            StartCoroutine(EffectNotePopupCoroutine(_effectNote));
        }

        private IEnumerator EffectNotePopupCoroutine(EffectNote _effectNote)
        {
            GameObject popup = ShowPopup("EffectNote");

            Dropdown seDropdown = popup.transform.Find("SEDropdown").GetComponent<Dropdown>();
            Dropdown tickRateDropdown = popup.transform.Find("TickRateDropdown").GetComponent<Dropdown>();

            List<string> types = new List<string>(Enum.GetNames(typeof(EffectNoteSEType)));
            seDropdown.ClearOptions();
            seDropdown.AddOptions(types);
            seDropdown.value = types.IndexOf(_effectNote.seType.ToString());

            tickRateDropdown.value = MakerManager.SETickRates.IndexOf(GlobalDefines.BeatPerBar / _effectNote.seTickBeatRate);

            isWaitForApply = true;
            while (isWaitForApply)
            {
                yield return null;
            }

            if (isApplied)
            {
                EffectNoteSEType newType = EffectNoteSEType.None;
                if (Enum.TryParse<EffectNoteSEType>(seDropdown.options[seDropdown.value].text, out newType))
                    _effectNote.seType = newType;

                _effectNote.seTickBeatRate = GlobalDefines.BeatPerBar / MakerManager.SETickRates[tickRateDropdown.value];
            }

            ClosePopup("EffectNote");
        }

        public void OpenSyncUploadNotePopup()
        {
            StartCoroutine(SyncUploadPopupCoroutine());
        }

        private IEnumerator SyncUploadPopupCoroutine()
        {
            GameObject popup = ShowPopup("SyncUpload");

            Text serverDateText = popup.transform.Find("ServerDateText").GetComponent<Text>();
            Text changeListText = popup.transform.Find("ChangeListView").Find("Viewport").Find("Content").GetComponent<Text>();

            serverDateText.text = NoteDataSyncManager.Instance.loadedServerHashDateStr + " (ver " + NoteDataSyncManager.Instance.loadedServerHashVersion + ")";
            changeListText.text = NoteDataSyncManager.Instance.MakeUploadChangeListString();

            isWaitForApply = true;
            while (isWaitForApply)
            {
                yield return null;
            }

            NoteDataSyncManager.Instance.isPopupConfirmed = isApplied;
            NoteDataSyncManager.Instance.isUploadPopupNeededAndOpened = false;

            ClosePopup("SyncUpload");
        }

        public void OpenSyncDownloadNotePopup()
        {
            StartCoroutine(SyncDownloadPopupCoroutine());
        }

        private IEnumerator SyncDownloadPopupCoroutine()
        {
            GameObject popup = ShowPopup("SyncDownload");

            Text serverDateText = popup.transform.Find("ServerDateText").GetComponent<Text>();
            Text changeListText = popup.transform.Find("ChangeListView").Find("Viewport").Find("Content").GetComponent<Text>();

            serverDateText.text = NoteDataSyncManager.Instance.loadedServerHashDateStr + " (ver " + NoteDataSyncManager.Instance.loadedServerHashVersion + ")";
            changeListText.text = NoteDataSyncManager.Instance.MakeDownloadChangeListString();

            isWaitForApply = true;
            while (isWaitForApply)
            {
                yield return null;
            }

            NoteDataSyncManager.Instance.isPopupConfirmed = isApplied;
            NoteDataSyncManager.Instance.isDownloadPopupNeededAndOpened = false;

            ClosePopup("SyncDownload");
        }
    }
}