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

        public int[] noteResRailTable = new int[7];



        protected override void Awake()
        {
            allNotes = new List<Note>();
            remainNotes = new List<Note>();
            spawnedNotes = new List<Note>();

            noteScale = UserSettings.UserNoteScale;

            InitNoteAppearType(UserSettings.NoteAppearType);
        }

        public void InitNoteAppearType(NoteAppearType _appearType)
        {
            noteResRailTable = new int[7] { 0, 1, 2, 3, 4, 5, 6 };
            switch (_appearType)
            {
                case NoteAppearType.Normal:
                default:
                    {
                    } break;

                case NoteAppearType.Mirror:
                    {
                        noteResRailTable = new int[7] { 3, 2, 1, 0, 4, 6, 5 };
                    } break;

                case NoteAppearType.R_Random:
                    {
                        int rotationCount = UnityEngine.Random.Range(1, 4);
                        for (int i = 0; i < 4; ++i)
                            noteResRailTable[i] = (i + rotationCount) % 4;
                        // EffectNote는 랜덤 X
                        noteResRailTable[4] = 4;
                    } break;

                case NoteAppearType.N_Random:
                    {
                        List<int> temp;
                        temp = new List<int>(new int[] { 0, 1, 2, 3, });
                        for (int i = 0; i < 4; ++i)
                        {
                            int tempR = UnityEngine.Random.Range(0, temp.Count);
                            noteResRailTable[i] = temp[tempR];
                            temp.RemoveAt(tempR);
                        }
                        temp = new List<int>(new int[] { 5, 6, });
                        for (int i = 0; i < 2; ++i)
                        {
                            int tempR = UnityEngine.Random.Range(0, temp.Count);
                            noteResRailTable[5 + i] = temp[tempR];
                            temp.RemoveAt(tempR);
                        }
                        noteResRailTable[4] = 4;
                    } break;

                case NoteAppearType.A_Random:
                    {
                        // TBD
                    } break;
            }
        }
        
        private void Update()
        {
            // 화면에 보이기 시작할 노트 생성
            for (int i = 0; i < remainNotes.Count; ++i)
            {
                Note note = remainNotes[i];
                // if (BeatManager.ToBarBeat(note.bar, note.beat) <= BeatManager.Instance.RailEndBarBeat)
                if (note.position <= BeatManager.Instance.RailEndPosition)
                {
                    SpawnNote(note);
                    remainNotes.RemoveAt(i--);
                }
            }

            JudgeManager.Instance.UpdateJudgeForNotes();
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

            _note.railNumber = noteResRailTable[_note.railNumber];

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

    public enum NoteAppearType
    {
        Normal = 0,
        Mirror = 1,
        R_Random = 2, // Rotation Random
        N_Random = 3, // Normal Random
        A_Random = 4, // All Random
    }
}