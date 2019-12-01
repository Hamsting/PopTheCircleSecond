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
        public GameObject normalNotePrefab;
        public GameObject longNotePrefab;
        public GameObject dragNotePrefab;
        public GameObject infinityNotePrefab;
        public GameObject doubleNotePrefab;

        public Transform leftRail;
        public Transform rightRail;
        [InspectorReadOnly]
        public List<Note> spawnedNotes;
        [InspectorReadOnly]
        private List<Note> allNotes;

        private List<Note> remainNotes;
        private float noteScale = 3.0f;

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
                    if (nr.GetType() != typeof(InfinityNoteRenderer))
                        nr.transform.localScale = new Vector3(noteScale, noteScale, noteScale);
                }
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
                if (note.GetType() == typeof(InfinityNote))
                {
                    InfinityNote infinityNote = (InfinityNote)note;
                    if (infinityNote.time <= BeatManager.Instance.GameTime + InfinityNoteRenderer.BeginAnimDuration)
                    {
                        SpawnNote(note);
                        remainNotes.RemoveAt(i--);
                    }
                }
                else if (BeatManager.ToBarBeat(note.bar, note.beat) <= BeatManager.Instance.RailEndBarBeat)
                {
                    SpawnNote(note);
                    remainNotes.RemoveAt(i--);
                }
            }

            JudgeManager.Instance.UpdateJudgeForNotes();
        }

        public void AddNote(Note _note)
        {
            Type noteType = _note.GetType();
            if (noteType == typeof(LongNote))
            {
                LongNote longNote = (LongNote)_note;
                longNote.endTime = BeatManager.Instance.BarBeatToTime(longNote.endBar, longNote.endBeat);
                longNote.endPosition = BeatManager.Instance.BarBeatToPosition(longNote.endBar, longNote.endBeat);

                float longNoteBarBeat = BeatManager.ToBarBeat(longNote.bar, longNote.beat);
                float longNoteEndBarBeat = BeatManager.ToBarBeat(longNote.endBar, longNote.endBeat);
                float tickBarBeat = (float)GlobalDefines.TickBeatRate / (float)GlobalDefines.BeatPerBar;
                longNote.tickStartBarBeat = BeatManager.Instance.CorrectBarBeat(longNoteBarBeat + tickBarBeat);
                longNote.tickEndBarBeat = BeatManager.Instance.CorrectBarBeat(longNoteEndBarBeat - tickBarBeat);
            }
            else if (noteType == typeof(InfinityNote))
            {
                InfinityNote infinityNote = (InfinityNote)_note;
                infinityNote.endTime = BeatManager.Instance.BarBeatToTime(infinityNote.endBar, infinityNote.endBeat);
                infinityNote.canPress = false;
                infinityNote.currentHitCount = 0;
            }

            _note.time = BeatManager.Instance.BarBeatToTime(_note.bar, _note.beat);
            _note.position = BeatManager.Instance.BarBeatToPosition(_note.bar, _note.beat);
            
            allNotes.Add(_note);
            remainNotes.Add(_note);
        }

        private void SpawnNote(Note _note)
        {
            GameObject obj = null;

            Type noteType = _note.GetType();
            if (noteType == typeof(NormalNote))
            {
                obj = ObjectPoolManager.Instance.Get(normalNotePrefab.name);
            }
            else if (noteType == typeof(LongNote))
            {
                obj = ObjectPoolManager.Instance.Get(longNotePrefab.name);
            }
            else if (noteType == typeof(DragNote))
            {
                obj = ObjectPoolManager.Instance.Get(dragNotePrefab.name);
            }
            else if (noteType == typeof(InfinityNote))
            {
                obj = ObjectPoolManager.Instance.Get(infinityNotePrefab.name);
            }
            else if (noteType == typeof(DoubleNote))
            {
                obj = ObjectPoolManager.Instance.Get(doubleNotePrefab.name);
            }
            
            NoteRenderer noteRen = obj.GetComponent<NoteRenderer>();
            noteRen.note = _note;
            noteRen.InitializeNote();

            _note.noteObject = obj;
            if (noteType == typeof(InfinityNote))
                obj.transform.parent = rightRail.parent;
            else
            {
                obj.transform.parent = (_note.railNumber == 0) ? leftRail : rightRail;
                obj.transform.localScale = new Vector3(noteScale, noteScale, noteScale);
            }
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
                    if (_note.GetType() == typeof(InfinityNote))
                        ((InfinityNoteRenderer)noteRen).SetNoteClear();
                    else
                        noteRen.DestroyNote();
                }
            }

            spawnedNotes.Remove(_note);
        }
    }
}