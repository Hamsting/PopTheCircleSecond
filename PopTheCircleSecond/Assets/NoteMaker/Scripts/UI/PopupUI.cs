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
            InputField bpmInput = popup.transform.Find("InputField").GetComponent<InputField>();
            bpmInput.text = BeatManager.Instance.GetBPMWithBarBeat(_bar, _beat).ToString("0.###");

            isWaitForApply = true;
            while (isWaitForApply)
            {
                yield return null;
            }

            if (isApplied)
            {
                float bpm = float.Parse(bpmInput.text);

                int index = BeatManager.Instance.BPMInfos.FindIndex((bi) => 
                ((bi.bar == _bar && bi.beat == _beat) ||
                 (bi.beat > 0.0f && _bar == bi.bar + 1 && _beat == 0.0f))
                );
                if (index >= 0)
                {
                    BeatManager.Instance.BPMInfos[index].bpm = bpm;
                    BeatManager.Instance.UpdateBPMInfo();
                    BeatManager.Instance.UpdateRailLengths();
                }
                else
                    BeatManager.Instance.AddNewBPMInfo(_bar, _beat, bpm);

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
    }
}