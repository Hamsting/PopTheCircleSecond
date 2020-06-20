using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PopTheCircle.NoteEditor
{
    public class NoteDataJSONConverter : Singleton<NoteDataJSONConverter>
    {
        public const int NoteDataFileVersion = 1;



        public JSONObject NoteDataToJSON()
        {
            JSONObject noteDataJson = new JSONObject();
            noteDataJson.AddField("version", NoteDataFileVersion);

            NoteData noteData = MakerManager.Instance.noteData;
            
            JSONObject headerJson = new JSONObject();
            headerJson.AddField("musicFilePath", noteData.musicFilePath);
            headerJson.AddField("musicTitle", noteData.musicTitle);
            headerJson.AddField("musicArtist", noteData.musicArtist);
            headerJson.AddField("musicBPM", noteData.musicBPM);
            headerJson.AddField("musicStartTime", noteData.musicStartTime);
            headerJson.AddField("standardBPM", noteData.standardBPM);
            noteDataJson.AddField("Header", headerJson);


            JSONObject notesJson = new JSONObject();
            foreach (Note n in NoteManager.Instance.notes)
            {
                notesJson.Add(GetNoteJSON(n));
            }
            noteDataJson.AddField("Notes", notesJson);


            JSONObject effectNotesJson = new JSONObject();
            foreach (Note n in NoteManager.Instance.effectNotes)
            {
                effectNotesJson.Add(GetNoteJSON(n));
            }
            noteDataJson.AddField("EffectNotes", effectNotesJson);


            JSONObject bpmChangesJson = new JSONObject();
            foreach (BPMInfo bpm in BeatManager.Instance.BPMInfos)
            {
                JSONObject bpmChangeJson = new JSONObject();
                bpmChangeJson.AddField("bpm", bpm.bpm);
                bpmChangeJson.AddField("bar", bpm.bar);
                bpmChangeJson.AddField("beat", bpm.beat);
                bpmChangeJson.AddField("stopEffect", bpm.stopEffect);
                bpmChangesJson.Add(bpmChangeJson);
            }
            noteDataJson.AddField("BPMChanges", bpmChangesJson);


            JSONObject ctChangesJson = new JSONObject();
            foreach (CTInfo ct in BeatManager.Instance.ctInfos)
            {
                JSONObject ctChangeJson = new JSONObject();
                ctChangeJson.AddField("bar", ct.bar);
                ctChangeJson.AddField("numerator", ct.numerator);
                ctChangesJson.Add(ctChangeJson);
            }
            noteDataJson.AddField("CTChanges", ctChangesJson);


            return noteDataJson;
        }

        public void JSONToNoteData(JSONObject _noteDataJson)
        {
            if (_noteDataJson == null)
            {
                Debug.Log("NoteDataJSONConverter::The Json is null.");
                return;
            }

            JSONObject noteDataJson = new JSONObject();
            int noteDataVersion = GetIntData(_noteDataJson, "version");

            NoteData noteData = new NoteData();

            JSONObject headerJson = _noteDataJson.GetField("Header");
            noteData.musicFilePath = GetStringData(headerJson, "musicFilePath");
            noteData.musicTitle = GetStringData(headerJson, "musicTitle");
            noteData.musicArtist = GetStringData(headerJson, "musicArtist");
            noteData.musicBPM = GetStringData(headerJson, "musicBPM");
            noteData.musicStartTime = GetIntData(headerJson, "musicStartTime");
            noteData.standardBPM = GetFloatData(headerJson, "standardBPM");
            MakerManager.Instance.noteData = noteData;


            NoteManager.Instance.notes.Clear();
            JSONObject notesJson = _noteDataJson.GetField("Notes");
            if (notesJson != null)
            {
                List<JSONObject> noteJsonList = notesJson.list;

                if (noteJsonList != null)
                {
                    foreach (JSONObject noteJson in noteJsonList)
                    {
                        Note n = GetNoteFromJSON(noteJson);
                        NoteManager.Instance.notes.Add(n);
                    }
                }
            }


            NoteManager.Instance.effectNotes.Clear();
            JSONObject effectNotesJson = _noteDataJson.GetField("EffectNotes");
            if (effectNotesJson != null)
            {
                List<JSONObject> effectNoteJsonList = effectNotesJson.list;

                if (effectNoteJsonList != null)
                {
                    foreach (JSONObject effectNoteJson in effectNoteJsonList)
                    {
                        Note n = GetNoteFromJSON(effectNoteJson);
                        NoteManager.Instance.effectNotes.Add(n);
                    }
                }
            }


            JSONObject bpmChangesJson = _noteDataJson.GetField("BPMChanges");
            List<JSONObject> bpmChangeJsonList = bpmChangesJson.list;

            BeatManager.Instance.BPMInfos.Clear();
            foreach (JSONObject bpmChangeJson in bpmChangeJsonList)
            {
                BPMInfo bpm = new BPMInfo()
                {
                    bpm = GetFloatData(bpmChangeJson, "bpm"),
                    bar = GetIntData(bpmChangeJson, "bar"),
                    beat = GetFloatData(bpmChangeJson, "beat"),
                    stopEffect = GetBoolData(bpmChangeJson, "stopEffect")
                };
                BeatManager.Instance.BPMInfos.Add(bpm);
            }


            JSONObject ctChangesJson = _noteDataJson.GetField("CTChanges");
            List<JSONObject> ctChangeJsonList = ctChangesJson.list;

            BeatManager.Instance.ctInfos.Clear();
            foreach (JSONObject ctChangeJson in ctChangeJsonList)
            {
                CTInfo ct = new CTInfo()
                {
                    bar = GetIntData(ctChangeJson, "bar"),
                    numerator = GetIntData(ctChangeJson, "numerator")
                };
                BeatManager.Instance.ctInfos.Add(ct);
            }


            BeatManager.Instance.UpdateBPMInfo();
            BeatManager.Instance.UpdateRailLengths();
            NoteRailManager.Instance.UpdateRailSpawnImmediately();
            NoteManager.Instance.UpdateNoteSpawn();
            MakerUIManager.Instance.optionMenu.ApplyNoteDataInfo(false);
        }

        private JSONObject GetNoteJSON(Note _n)
        {
            string noteTypeName = _n.GetType().Name;

            JSONObject noteJson = new JSONObject();
            noteJson.AddField("noteType", noteTypeName);
            noteJson.AddField("bar", _n.bar);
            noteJson.AddField("beat", _n.beat);
            noteJson.AddField("railNumber", _n.railNumber);

            switch (noteTypeName)
            {
                default:
                case "NormalNote":
                case "PopNote":
                case "MineNote":
                case "TickNote":
                    break;
                case "LongNote":
                    {
                        LongNote longNote = (LongNote)_n;
                        noteJson.AddField("endBar", longNote.endBar);
                        noteJson.AddField("endBeat", longNote.endBeat);
                    }
                    break;
                case "SpaceNote":
                    {
                        SpaceNote spaceNote = (SpaceNote)_n;
                        noteJson.AddField("endBar", spaceNote.endBar);
                        noteJson.AddField("endBeat", spaceNote.endBeat);
                    }
                    break;
                case "EffectNote":
                    {
                        EffectNote effectNote = (EffectNote)_n;
                        noteJson.AddField("endBar", effectNote.endBar);
                        noteJson.AddField("endBeat", effectNote.endBeat);
                        noteJson.AddField("seType", effectNote.seType.ToString());
                        noteJson.AddField("seTickBeatRate", effectNote.seTickBeatRate);
                    }
                    break;
                case "CameraNote":
                    {
                        CameraNote cameraNote = (CameraNote)_n;
                        noteJson.AddField("curve", Enum.GetName(typeof(CameraCurve), cameraNote.curve));
                        noteJson.AddField("duration", cameraNote.duration);
                        noteJson.AddField("positionChange", JSONTemplates.FromVector2(cameraNote.positionChange));
                        noteJson.AddField("rotationChange", cameraNote.rotationChange);
                        noteJson.AddField("sizeChange", cameraNote.sizeChange);
                    }
                    break;
                case "EventNote":
                    {
                        EventNote eventNote = (EventNote)_n;
                        noteJson.AddField("eventName", eventNote.eventName);
                    }
                    break;
            }

            return noteJson;
        }

        private Note GetNoteFromJSON(JSONObject _json)
        {
            Note n = null;

            string noteTypeName = GetStringData(_json, "noteType");
            switch (noteTypeName)
            {
                default:
                case "NormalNote":
                    n = new NormalNote();
                    break;
                case "PopNote":
                    n = new PopNote();
                    break;
                case "MineNote":
                    n = new MineNote();
                    break;
                case "TickNote":
                    n = new TickNote();
                    break;
                case "LongNote":
                    {
                        n = new LongNote()
                        {
                            endBar = GetIntData(_json, "endBar"),
                            endBeat = GetFloatData(_json, "endBeat")
                        };
                    }
                    break;
                case "SpaceNote":
                    {
                        n = new SpaceNote()
                        {
                            endBar = GetIntData(_json, "endBar"),
                            endBeat = GetFloatData(_json, "endBeat")
                        };
                    }
                    break;
                case "EffectNote":
                    {
                        n = new EffectNote()
                        {
                            endBar = GetIntData(_json, "endBar"),
                            endBeat = GetFloatData(_json, "endBeat"),
                            seTickBeatRate = GetIntData(_json, "seTickBeatRate", 4),
                        };

                        EffectNote effectNote = (EffectNote)n;

                        string seTypeStr = GetStringData(_json, "seType");
                        EffectNoteSEType seType = EffectNoteSEType.None;
                        if (string.IsNullOrEmpty(seTypeStr))
                            effectNote.seType = EffectNoteSEType.None;
                        else if (!Enum.TryParse<EffectNoteSEType>(seTypeStr, out seType))
                            effectNote.seType = EffectNoteSEType.None;
                        else
                            effectNote.seType = seType;
                    }
                    break;
                case "CameraNote":
                    {
                        n = new CameraNote()
                        {
                            curve = (CameraCurve)Enum.Parse(typeof(CameraCurve), GetStringData(_json, "curve")),
                            duration = GetFloatData(_json, "duration"),
                            positionChange = GetVector2Data(_json, "positionChange"),
                            rotationChange = GetFloatData(_json, "rotationChange"),
                            sizeChange = GetFloatData(_json, "sizeChange")
                        };
                    }
                    break;
                case "EventNote":
                    {
                        n = new EventNote()
                        {
                            eventName = GetStringData(_json, "eventName")
                        };
                    }
                    break;
            }
            if (n == null)
                return null;

            n.bar = GetIntData(_json, "bar");
            n.beat = GetFloatData(_json, "beat");
            n.railNumber = GetIntData(_json, "railNumber");
            Enum.TryParse<NoteType>(noteTypeName.Replace("Note", ""), out n.noteType);

            return n;
        }

        private string GetStringData(JSONObject _json, string _field)
        {
            if (_json == null)
                return "";
             JSONObject j = _json.GetField(_field);
            if (j == null)
                return "";
            return j.str;                
        }

        private int GetIntData(JSONObject _json, string _field, int _default = 0)
        {
            if (_json == null)
                return _default;
            JSONObject j = _json.GetField(_field);
            if (j == null)
                return _default;
            return (int)j.i;
        }

        private float GetFloatData(JSONObject _json, string _field)
        {
            if (_json == null)
                return 0.0f;
            JSONObject j = _json.GetField(_field);
            if (j == null)
                return 0.0f;
            return j.f;
        }

        private bool GetBoolData(JSONObject _json, string _field)
        {
            if (_json == null)
                return false;
            JSONObject j = _json.GetField(_field);
            if (j == null)
                return false;
            return j.b;
        }

        private Vector2 GetVector2Data(JSONObject _json, string _field)
        {
            if (_json == null)
                return Vector2.zero;
            JSONObject j = _json.GetField(_field);
            if (j == null)
                return Vector2.zero;
            return JSONTemplates.ToVector2(j);
        }
    }
}