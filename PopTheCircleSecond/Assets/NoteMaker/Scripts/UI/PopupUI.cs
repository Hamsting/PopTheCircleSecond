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
        private int popupInteractionFlag = 0;
        private Dictionary<string, object> popupStoredVars = new Dictionary<string, object>();



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

        public void SetPopupInteractionFlag(int _flag)
        {
            popupInteractionFlag = _flag;
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
                    (bi.bar == _bar && bi.beat == _beat)
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

            RectTransform seListContent = popup.transform.Find("SEListView")
                .Find("Viewport").Find("Content").GetComponent<RectTransform>();
            ToggleGroup seListToggleGroup = seListContent.GetComponent<ToggleGroup>();

            Dropdown tickRateDropdown = popup.transform.Find("TickRateDropdown").GetComponent<Dropdown>();
            tickRateDropdown.value = MakerManager.SETickRates.IndexOf(GlobalDefines.BeatPerBar / _effectNote.seTickBeatRate);

            popupStoredVars["effPopupSelectedEffIndex"] = (int)_effectNote.seType;
            if (!popupStoredVars.ContainsKey("effPopupInited") || (bool)popupStoredVars["effPopupInited"] == false)
            {
                Toggle seToggleSource = seListContent.Find("SEToggleSource").GetComponent<Toggle>();

                List<string> seTypeStrs = new List<string>(Enum.GetNames(typeof(EffectNoteSEType)));
                foreach (var seTypeStr in seTypeStrs)
                {
                    EffectNoteSEType seType = EffectNoteSEType.None;
                    if (!Enum.TryParse<EffectNoteSEType>(seTypeStr, out seType) || seType == EffectNoteSEType.None)
                        continue;

                    GameObject seToggleInsObj = GameObject.Instantiate<GameObject>(seToggleSource.gameObject, seListContent);
                    Toggle seToggleIns = seToggleInsObj.GetComponent<Toggle>();
                    seToggleIns.isOn = ((int)popupStoredVars["effPopupSelectedEffIndex"] == (int)seType);
                    seToggleIns.onValueChanged.AddListener((_isOn) =>
                    {
                        if (_isOn)
                        {
                            popupStoredVars["effPopupSelectedEffIndex"] = (int)seType;
                            MusicManager.Instance.PlaySE(seType);
                        }
                    });

                    Text seToggleInsLabel = seToggleInsObj.GetComponentInChildren<Text>();
                    seToggleInsLabel.text = seTypeStr;
                }
                
                Destroy(seToggleSource.gameObject);
                popupStoredVars["effPopupInited"] = true;
            }

            isWaitForApply = true;
            while (isWaitForApply)
            {
                if (popupInteractionFlag > 0)
                {
                    if (popupInteractionFlag == 1)
                    {
                        if (Enum.IsDefined(typeof(EffectNoteSEType), (int)popupStoredVars["effPopupSelectedEffIndex"]))
                            MusicManager.Instance.PlaySE((EffectNoteSEType)(int)popupStoredVars["effPopupSelectedEffIndex"]);
                    }
                    popupInteractionFlag = 0;
                }

                yield return null;
            }

            if (isApplied)
            {
                if (!seListToggleGroup.AnyTogglesOn())
                    _effectNote.seType = EffectNoteSEType.None;
                else if (Enum.IsDefined(typeof(EffectNoteSEType), (int)popupStoredVars["effPopupSelectedEffIndex"]))
                    _effectNote.seType = (EffectNoteSEType)(int)popupStoredVars["effPopupSelectedEffIndex"];

                _effectNote.seTickBeatRate = GlobalDefines.BeatPerBar / MakerManager.SETickRates[tickRateDropdown.value];
            }

            ClosePopup("EffectNote");
        }

        // Old Source
        /*
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
                if (popupInteractionFlag > 0)
                {
                    if (popupInteractionFlag == 1)
                    {
                        EffectNoteSEType previewType = EffectNoteSEType.None;
                        if (Enum.TryParse<EffectNoteSEType>(seDropdown.options[seDropdown.value].text, out previewType))
                            MusicManager.Instance.PlaySE(previewType);
                    }
                    popupInteractionFlag = 0;
                }


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
        */

        public void OpenJPNotePopup(int _bar, float _beat)
        {
            StartCoroutine(JPNotePopupCoroutine(_bar, _beat));
        }

        private IEnumerator JPNotePopupCoroutine(int _bar, float _beat)
        {
            GameObject popup = ShowPopup("JPNote");

            JPInfo existedInfo = null;
            if (BeatManager.Instance.JPInfos != null)
            {
                for (int i = 0; i < BeatManager.Instance.JPInfos.Count; ++i)
                {
                    JPInfo iter = BeatManager.Instance.JPInfos[i];
                    if (iter.bar == _bar && iter.beat == _beat)
                    {
                        existedInfo = iter;
                        break;
                    }
                }
            }

            InputField jumpBarInput = popup.transform.Find("BarInput").GetComponent<InputField>();
            jumpBarInput.text = (existedInfo != null) ? existedInfo.jumpBar.ToString() : "0";
            InputField jumpBeatInput = popup.transform.Find("BeatInput").GetComponent<InputField>();
            jumpBeatInput.text = (existedInfo != null) ? existedInfo.jumpBeat.ToString() : "0";

            isWaitForApply = true;
            while (isWaitForApply)
            {
                yield return null;
            }
            
            if (isApplied)
            {
                int jumpBar = int.Parse(jumpBarInput.text);
                float jumpBeat = float.Parse(jumpBeatInput.text);
                
                if (existedInfo != null)
                {
                    existedInfo.jumpBar = jumpBar;
                    existedInfo.jumpBeat = jumpBeat;
                }
                else
                    BeatManager.Instance.AddNewJPInfo(_bar, _beat, jumpBar, jumpBeat);

                NoteManager.Instance.FixIncorrectBarBeatNotes();
                NoteRailManager.Instance.UpdateRailSpawnImmediately();
                BeatManager.Instance.GotoTime(BeatManager.Instance.GameTime);
            }
            
            ClosePopup("JPNote");
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