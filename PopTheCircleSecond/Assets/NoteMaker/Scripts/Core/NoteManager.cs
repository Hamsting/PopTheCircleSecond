using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PopTheCircle.NoteEditor
{
    public class NoteManager : Singleton<NoteManager>
    {
        [InspectorReadOnly]
        public List<Note> notes;
        [InspectorReadOnly]
        public List<Note> effectNotes;

        private List<NoteRenderer> spawnedRenderers;



        protected override void Awake()
        {
            base.Awake();
            notes = new List<Note>();
            effectNotes = new List<Note>();
            spawnedRenderers = new List<NoteRenderer>();
        }

        public void UpdateNoteSpawn()
        {
            foreach (var r in spawnedRenderers)
            {
                ObjectPoolManager.Instance.Free(r.gameObject);
            }
            spawnedRenderers.Clear();

            float vStart = NoteRailManager.Instance.visibleRailStartBarBeat;
            float vEnd = NoteRailManager.Instance.visibleRailEndBarBeat;

            foreach (Note n in notes)
            {
                float barBeat = BeatManager.ToBarBeat(n.bar, n.beat);
                if (barBeat < vStart || barBeat > vEnd)
                {
                    if (n.GetType() == typeof(LongNote))
                    {
                        LongNote longNote = (LongNote)n;
                        float endBarBeat = BeatManager.ToBarBeat(longNote.endBar, longNote.endBeat);
                        if (endBarBeat < vStart || endBarBeat > vEnd)
                            continue;
                    }
                    else if (n.GetType() == typeof(InfinityNote))
                    {
                        InfinityNote infinityNote = (InfinityNote)n;
                        float endBarBeat = BeatManager.ToBarBeat(infinityNote.endBar, infinityNote.endBeat);
                        if (endBarBeat < vStart || endBarBeat > vEnd)
                            continue;
                    }
                    else
                        continue;
                }

                string noteName = n.GetType().Name;
                GameObject noteObj = ObjectPoolManager.Instance.Get(noteName, true);
                if (noteObj == null)
                {
                    Debug.LogError("NoteManager::Cannot found note prefab named " + noteName);
                    continue;
                }
                noteObj.transform.parent = NoteRailManager.Instance.railRoot;

                NoteRenderer noteRen = noteObj.GetComponent<NoteRenderer>();
                noteRen.note = n;
                noteRen.Initialize();
                spawnedRenderers.Add(noteRen);
            }
            foreach (Note n in effectNotes)
            {
                float barBeat = BeatManager.ToBarBeat(n.bar, n.beat);
                if (barBeat < vStart || barBeat > vEnd)
                    continue;

                string noteName = n.GetType().Name;
                GameObject noteObj = ObjectPoolManager.Instance.Get(noteName, true);
                if (noteObj == null)
                {
                    Debug.LogError("NoteManager::Cannot found note prefab named " + noteName);
                    continue;
                }
                noteObj.transform.parent = NoteRailManager.Instance.railRoot;

                NoteRenderer noteRen = noteObj.GetComponent<NoteRenderer>();
                noteRen.note = n;
                noteRen.Initialize();
                spawnedRenderers.Add(noteRen);
            }
            foreach (BPMInfo bpm in BeatManager.Instance.BPMInfos)
            {
                float barBeat = BeatManager.ToBarBeat(bpm.bar, bpm.beat);
                if (barBeat < vStart || barBeat > vEnd)
                    continue;

                GameObject bpmObj = ObjectPoolManager.Instance.Get("BPMChangeNote", true);
                bpmObj.transform.parent = NoteRailManager.Instance.railRoot;

                BPMChangeNoteRenderer bpmRen = bpmObj.GetComponent<BPMChangeNoteRenderer>();
                bpmRen.bpmInfo = bpm;
                bpmRen.Initialize();
                spawnedRenderers.Add(bpmRen);
            }
            foreach (CTInfo ct in BeatManager.Instance.ctInfos)
            {
                float barBeat = BeatManager.ToBarBeat(ct.bar, 0.0f);
                if (barBeat < vStart || barBeat > vEnd)
                    continue;

                GameObject ctObj = ObjectPoolManager.Instance.Get("CTChangeNote", true);
                ctObj.transform.parent = NoteRailManager.Instance.railRoot;

                CTChangeNoteRenderer ctRen = ctObj.GetComponent<CTChangeNoteRenderer>();
                ctRen.ctInfo = ct;
                ctRen.Initialize();
                spawnedRenderers.Add(ctRen);
            }
        }

        public void AddNote(Note _note, bool _updateSpawn = true)
        {
            if (_note.GetType() == typeof(CameraNote) || _note.GetType() == typeof(EventNote))
                effectNotes.Add(_note);
            else
                notes.Add(_note);

            if (_updateSpawn)
                UpdateNoteSpawn();
        }

        public void RemoveNote(Note _note, bool _updateSpawn = true)
        {
            if (_note.GetType() == typeof(CameraNote) || _note.GetType() == typeof(EventNote))
                effectNotes.Remove(_note);
            else
                notes.Remove(_note);

            if (_updateSpawn)
                UpdateNoteSpawn();
        }

        public Note FindNote(int _bar, float _beat, int _railNumber, bool _excludeLongType = true)
        {
            foreach (Note n in notes)
            {
                if (n.railNumber != _railNumber)
                    continue;

                if (_excludeLongType &&
                    (n.GetType() == typeof(LongNote) || n.GetType() == typeof(InfinityNote)))
                    continue;

                if (n.bar == _bar && Mathf.Abs(n.beat - _beat) <= 0.5f)
                    return n;
            }
            return null;
        }

        public Note FindLongTypeNote(int _bar, float _beat, int _railNumber)
        {
            foreach (Note n in notes)
            {
                if (n.GetType() == typeof(LongNote) && n.railNumber == _railNumber)
                {
                    LongNote ln = (LongNote)n;
                    float barBeat = BeatManager.ToBarBeat(_bar, _beat);
                    float startBarBeat = BeatManager.ToBarBeat(ln.bar, ln.beat);
                    float endBarBeat = BeatManager.ToBarBeat(ln.endBar, ln.endBeat);
                    if (barBeat >= startBarBeat && barBeat <= endBarBeat)
                        return n;
                }
                else if (n.GetType() == typeof(InfinityNote))
                {
                    InfinityNote fn = (InfinityNote)n;
                    float barBeat = BeatManager.ToBarBeat(_bar, _beat);
                    float startBarBeat = BeatManager.ToBarBeat(fn.bar, fn.beat);
                    float endBarBeat = BeatManager.ToBarBeat(fn.endBar, fn.endBeat);
                    if (barBeat >= startBarBeat && barBeat <= endBarBeat)
                        return n;
                }
                else
                    continue;
            }
            return null;
        }
        
        public Note FindEffectNote(int _bar, float _beat, Type _noteType)
        {
            foreach (Note n in effectNotes)
            {
                if (n.GetType() != _noteType)
                    continue;

                if (n.bar == _bar && n.beat == _beat)
                    return n;
            }
            return null;
        }

        public void FixIncorrectBarBeatNotes()
        {
            foreach (Note n in notes)
            {
                float barBeat = BeatManager.ToBarBeat(n.bar, n.beat);
                float corrected = BeatManager.Instance.CorrectBarBeat(barBeat);
                if (barBeat != corrected)
                {
                    n.bar = (int)corrected;
                    n.beat = (corrected - (int)corrected) * (float)GlobalDefines.BeatPerBar;
                }

                if (n.GetType() == typeof(LongNote))
                {
                    LongNote ln = (LongNote)n;
                    float endBarBeat = BeatManager.ToBarBeat(ln.endBar, ln.endBeat);
                    float endCorrected = BeatManager.Instance.CorrectBarBeat(endBarBeat);
                    if (endBarBeat != endCorrected)
                    {
                        ln.endBar = (int)endCorrected;
                        ln.endBeat = (endCorrected - (int)endCorrected) * (float)GlobalDefines.BeatPerBar;
                    }
                }
                else if (n.GetType() == typeof(InfinityNote))
                {
                    InfinityNote fn = (InfinityNote)n;
                    float endBarBeat = BeatManager.ToBarBeat(fn.endBar, fn.endBeat);
                    float endCorrected = BeatManager.Instance.CorrectBarBeat(endBarBeat);
                    if (endBarBeat != endCorrected)
                    {
                        fn.endBar = (int)endCorrected;
                        fn.endBeat = (endCorrected - (int)endCorrected) * (float)GlobalDefines.BeatPerBar;
                    }
                }
            }
            foreach (Note n in effectNotes)
            {
                float barBeat = BeatManager.ToBarBeat(n.bar, n.beat);
                float corrected = BeatManager.Instance.CorrectBarBeat(barBeat);
                if (barBeat != corrected)
                {
                    n.bar = (int)corrected;
                    n.beat = (corrected - (int)corrected) * (float)GlobalDefines.BeatPerBar;
                }
            }
        }


        public void UpdateShotSound(float _startBarBeat, float _endBarBeat)
        {
            foreach (Note n in notes)
            {
                if (n.ContainsInBarBeat(_startBarBeat, _endBarBeat))
                {
                    MusicManager.Instance.PlayShot();
                    break;
                }

                if (n.GetType() == typeof(LongNote))
                {
                    LongNote ln = (LongNote)n;
                    float endBarBeat = BeatManager.ToBarBeat(ln.endBar, ln.endBeat);
                    if (endBarBeat >= _startBarBeat && endBarBeat <= _endBarBeat)
                    {
                        MusicManager.Instance.PlayShot();
                        break;
                    }
                }
                else if (n.GetType() == typeof(InfinityNote))
                {
                    InfinityNote fn = (InfinityNote)n;
                    float endBarBeat = BeatManager.ToBarBeat(fn.endBar, fn.endBeat);
                    if (endBarBeat >= _startBarBeat && endBarBeat <= _endBarBeat)
                    {
                        MusicManager.Instance.PlayShot();
                        break;
                    }
                }
            }
        }
    }
}