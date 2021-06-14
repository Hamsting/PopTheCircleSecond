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

        public List<NoteRenderer> SpawnedRenderers
        {
            get { return spawnedRenderers; }
        }
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
                    if ((n.GetType() == typeof(LongNote)) ||
                        (n.GetType() == typeof(SpaceNote) && ((SpaceNote)n).IsLongType) ||
                        (n.GetType() == typeof(EffectNote) && ((EffectNote)n).IsLongType))
                    {
                        LongNote longNote = (LongNote)n;
                        float endBarBeat = BeatManager.ToBarBeat(longNote.endBar, longNote.endBeat);
                        if (!(barBeat <= vEnd && vStart <= endBarBeat))
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
                noteObj.transform.localPosition = Vector3.zero;
                noteObj.transform.localRotation = Quaternion.identity;
                noteObj.transform.localScale = Vector3.one;

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
                noteObj.transform.localPosition = Vector3.zero;
                noteObj.transform.localRotation = Quaternion.identity;
                noteObj.transform.localScale = Vector3.one;

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
                bpmObj.transform.localPosition = Vector3.zero;
                bpmObj.transform.localRotation = Quaternion.identity;
                bpmObj.transform.localScale = Vector3.one;

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
                ctObj.transform.localPosition = Vector3.zero;
                ctObj.transform.localRotation = Quaternion.identity;
                ctObj.transform.localScale = Vector3.one;

                CTChangeNoteRenderer ctRen = ctObj.GetComponent<CTChangeNoteRenderer>();
                ctRen.ctInfo = ct;
                ctRen.Initialize();
                spawnedRenderers.Add(ctRen);
            }
            foreach (JPInfo jp in BeatManager.Instance.JPInfos)
            {
                float barBeat = BeatManager.ToBarBeat(jp.bar, jp.beat);
                if (barBeat < vStart || barBeat > vEnd)
                    continue;

                GameObject jpObj = ObjectPoolManager.Instance.Get("JPNote", true);
                jpObj.transform.parent = NoteRailManager.Instance.railRoot;
                jpObj.transform.localPosition = Vector3.zero;
                jpObj.transform.localRotation = Quaternion.identity;
                jpObj.transform.localScale = Vector3.one;

                JPNoteRenderer jpRen = jpObj.GetComponent<JPNoteRenderer>();
                jpRen.jpInfo = jp;
                jpRen.Initialize();
                spawnedRenderers.Add(jpRen);
            }
        }

        public void AddNote(Note _note, bool _updateSpawn = true)
        {
            if (_note.GetType() == typeof(CameraNote) || _note.GetType() == typeof(EventNote) || _note.GetType() == typeof(TickNote))
                effectNotes.Add(_note);
            else
                notes.Add(_note);

            if (_updateSpawn)
                UpdateNoteSpawn();
        }

        public void RemoveNote(Note _note, bool _updateSpawn = true)
        {
            if (_note.GetType() == typeof(CameraNote) || _note.GetType() == typeof(EventNote) || _note.GetType() == typeof(TickNote))
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
                    (n.GetType() == typeof(LongNote) ||
                    (n.GetType() == typeof(SpaceNote) && ((SpaceNote)n).IsLongType) ||
                    (n.GetType() == typeof(EffectNote) && ((EffectNote)n).IsLongType))
                    )
                    continue;

                if (n.bar == _bar && Mathf.Abs(n.beat - _beat) <= 0.5f)
                    return n;
            }
            return null;
        }

        public Note FindLongTypeNote(int _bar, float _beat, int _railNumber, bool _excludeEndBarBeat = false)
        {
            foreach (Note n in notes)
            {
                if (n.railNumber == _railNumber &&
                    (n.GetType() == typeof(LongNote) ||
                    (n.GetType() == typeof(SpaceNote) && ((SpaceNote)n).IsLongType) ||
                    (n.GetType() == typeof(EffectNote) && ((EffectNote)n).IsLongType))
                    )
                {
                    LongNote ln = (LongNote)n;
                    float barBeat = BeatManager.ToBarBeat(_bar, _beat);
                    float startBarBeat = BeatManager.ToBarBeat(ln.bar, ln.beat);
                    float endBarBeat = BeatManager.ToBarBeat(ln.endBar, ln.endBeat);
                    if (( _excludeEndBarBeat && barBeat >= startBarBeat && barBeat <  endBarBeat) ||
                        (!_excludeEndBarBeat && barBeat >= startBarBeat && barBeat <= endBarBeat))
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
            bool isNormalTickPlayed = false;
            foreach (Note n in notes)
            {
                if (n.GetType() == typeof(EffectNote))
                {
                    EffectNote effectNote = (EffectNote)n;

                    if (effectNote.ContainsInBarBeat(_startBarBeat, _endBarBeat))
                    {
                        MusicManager.Instance.PlaySE(effectNote.seType);
                        continue;
                    }
                    else if (effectNote.IsLongType && effectNote.ContainsInTickBarBeat(_startBarBeat, _endBarBeat))
                    {
                        MusicManager.Instance.PlaySE(effectNote.seType);
                        continue;
                    }
                }

                if (!isNormalTickPlayed)
                {
                    if (n.ContainsInBarBeat(_startBarBeat, _endBarBeat))
                    {
                        MusicManager.Instance.PlayShot();
                        isNormalTickPlayed = true;
                        continue;
                    }

                    if (n.GetType() == typeof(LongNote) ||
                        (n.GetType() == typeof(SpaceNote) && ((SpaceNote)n).IsLongType))
                    {
                        LongNote ln = (LongNote)n;
                        float endBarBeat = BeatManager.ToBarBeat(ln.endBar, ln.endBeat);
                        if (endBarBeat >= _startBarBeat && endBarBeat <= _endBarBeat)
                        {
                            MusicManager.Instance.PlayShot();
                            isNormalTickPlayed = true;
                            continue;
                        }
                    }
                }
            }
            foreach (Note n in effectNotes)
            {
                if (n.GetType() != typeof(TickNote))
                    continue;

                if (n.ContainsInBarBeat(_startBarBeat, _endBarBeat))
                {
                    MusicManager.Instance.PlayShot();
                    break;
                }
            }
        }
    }
}