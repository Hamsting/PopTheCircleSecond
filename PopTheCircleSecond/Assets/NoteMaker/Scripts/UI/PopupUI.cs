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

                int index = BeatManager.Instance.BPMInfos.FindIndex((bi) => (bi.bar == _bar && bi.beat == _beat));
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

        public void OpenInfinityCountChangePopup(InfinityNote _fn)
        {
            StartCoroutine(InfinityCountChangePopupCoroutine(_fn));
        }

        private IEnumerator InfinityCountChangePopupCoroutine(InfinityNote _fn)
        {
            GameObject popup = ShowPopup("InfinityCountChange");
            InputField countInput = popup.transform.Find("InputField").GetComponent<InputField>();
            countInput.text = _fn.maxHitCount.ToString();

            Text recommendText = popup.transform.Find("Recommend").GetComponent<Text>();
            float length = BeatManager.ToBarBeat(_fn.endBar, _fn.endBeat) 
                           - BeatManager.ToBarBeat(_fn.bar, _fn.beat);
            recommendText.text = "Recommend Hit Count : 2 ~ " + (int)(length / 0.25f);

            isWaitForApply = true;
            while (isWaitForApply)
            {
                yield return null;
            }

            if (isApplied)
            {
                int count = int.Parse(countInput.text);
                _fn.maxHitCount = count;
            }

            ClosePopup("InfinityCountChange");
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
    }
}