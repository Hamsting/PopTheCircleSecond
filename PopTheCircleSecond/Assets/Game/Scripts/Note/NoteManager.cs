using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PopTheCircle.Game
{
    /// <summary>
    /// 노트의 생성 및 제거를 담당한다.
    /// </summary>
    public class NoteManager : Singleton<NoteManager>
    {
        public GameObject[] notePrefabs;
        public Transform[] rails;

        [InspectorReadOnly]
        public List<Note> spawnedNotes;
        [InspectorReadOnly]
        public List<Note> allNotes;

        private List<Note> remainNotes;
        private float noteScale = 3.0f;
        private int maxBarNumber = -1;

        public float NoteScale
        {
            get
            {
                return noteScale;
            }
            set
            {
                noteScale = value;
                foreach (NoteRenderer nr in GameObject.FindObjectsOfType<NoteRenderer>())
                {
                    nr.SetNoteScale(noteScale);
                }
            }
        }

        public int MaxBarNumber
        {
            get
            {
                return maxBarNumber;
            }
        }



        protected override void Awake()
        {
            allNotes = new List<Note>();
            remainNotes = new List<Note>();
            spawnedNotes = new List<Note>();

            noteScale = UserSettings.noteScale;
        }
        
        private void Update()
        {
            // 화면에 보이기 시작할 노트 생성
            for (int i = 0; i < remainNotes.Count; ++i)
            {
                Note note = remainNotes[i];
                if (BeatManager.ToBarBeat(note.bar, note.beat) <= BeatManager.Instance.RailEndBarBeat)
                {
                    SpawnNote(note);
                    remainNotes.RemoveAt(i--);
                }
            }

            JudgeManager.Instance.UpdateJudgeForNotes();

            // __TEST__
            /*
            if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.T))
            {
                foreach (var n in allNotes)
                {
                    if (n.noteType == NoteType.Long ||
                       (n.noteType == NoteType.Space && ((SpaceNote)n).IsLongType) ||
                       (n.noteType == NoteType.Effect && ((EffectNote)n).IsLongType))
                    {
                        LongNote ln = (LongNote)n;
                        int pupaDiff = (ln.pupa2 - ln.pupa1);
                        if (pupaDiff > 0)
                            Debug.Log("NoteManager::LongNote pupa alert : " + "\n" +
                                "Start = " + ln.bar + " / " + ln.beat + ", End = " + ln.endBar + "/" + ln.endBeat + "\n" +
                                "TickStart = " + ln.tickStartBarBeat + ", TickEnd = " + ln.tickEndBarBeat + "\n" +
                                "1 = " + ln.pupa1 + ", 2 = " + ln.pupa2 + ", diff = " + pupaDiff);
                    }
                }
            }
            */
        }

        public void AddNote(Note _note)
        {
            if (maxBarNumber < _note.bar)
                maxBarNumber = _note.bar;

            NoteType noteType = _note.noteType;
            if (noteType == NoteType.Long || noteType == NoteType.Space || noteType == NoteType.Effect)
            {
                LongNote longNote = (LongNote)_note;
                longNote.endTime = BeatManager.Instance.BarBeatToTime(longNote.endBar, longNote.endBeat);
                longNote.endPosition = BeatManager.Instance.BarBeatToPosition(longNote.endBar, longNote.endBeat);

                float longNoteBarBeat = BeatManager.ToBarBeat(longNote.bar, longNote.beat);
                float longNoteEndBarBeat = BeatManager.ToBarBeat(longNote.endBar, longNote.endBeat);
                float tickBarBeat = (float)GlobalDefines.TickBeatRate / (float)GlobalDefines.BeatPerBar;
                longNote.tickStartBarBeat = BeatManager.Instance.CorrectBarBeat(longNoteBarBeat + tickBarBeat);
                longNote.tickEndBarBeat = BeatManager.Instance.CorrectBarBeatReversed(longNoteEndBarBeat - tickBarBeat);
                
                if (maxBarNumber < longNote.endBar)
                    maxBarNumber = longNote.endBar;
            }
            if (noteType == NoteType.Effect)
            {
                EffectNote effectNote = (EffectNote)_note;

                float effectNoteStartBarBeat = BeatManager.ToBarBeat(effectNote.bar, effectNote.beat);
                float effectNoteEndBarBeat = BeatManager.ToBarBeat(effectNote.endBar, effectNote.endBeat);
                float tickBarBeat = (float)effectNote.seTickBeatRate / (float)GlobalDefines.BeatPerBar;
                effectNote.seTickStartBarBeat = BeatManager.Instance.CorrectBarBeat(effectNoteStartBarBeat + tickBarBeat);
                effectNote.seTickEndBarBeat = BeatManager.Instance.CorrectBarBeat(effectNoteEndBarBeat);
                effectNote.seNextTickedBarBeat = effectNote.seTickStartBarBeat;
            }

            _note.time = BeatManager.Instance.BarBeatToTime(_note.bar, _note.beat);
            _note.position = BeatManager.Instance.BarBeatToPosition(_note.bar, _note.beat);
            
            allNotes.Add(_note);
            remainNotes.Add(_note);
        }

        private void SpawnNote(Note _note)
        {
            GameObject obj = null;

            NoteType noteType = _note.noteType;
            obj = ObjectPoolManager.Instance.Get(notePrefabs[(int)noteType].name);
            
            NoteRenderer noteRen = obj.GetComponent<NoteRenderer>();
            noteRen.note = _note;
            noteRen.InitializeNote();
            noteRen.SetNoteScale(noteScale);

            _note.noteObject = obj;
            obj.transform.parent = rails[_note.railNumber];
            obj.SetActive(true);
            spawnedNotes.Add(_note);
        }

        public void DespawnNote(Note _note, bool _isMissed = false)
        {
            if (_isMissed)
                _note.SetNoteMissed();
            else if (_note.noteObject != null)
            {
                NoteRenderer noteRen = _note.noteObject.GetComponent<NoteRenderer>();
                if (noteRen != null)
                {
                    noteRen.DestroyNote();
                }
            }

            spawnedNotes.Remove(_note);
        }
    }
}