using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PopTheCircle.Game
{
    public class NoteDataJSONConverter : Singleton<NoteDataJSONConverter>
    {
        public void JSONToNoteData(JSONObject _noteDataJson)
        {
            if (_noteDataJson == null)
            {
                Debug.Log("NoteDataJSONConverter::The Json is null.");
                return;
            }

            int noteDataVersion = GetIntData(_noteDataJson, "version");
            
            
            JSONObject bpmChangesJson = _noteDataJson.GetField("BPMChanges");
            List<JSONObject> bpmChangeJsonList = bpmChangesJson.list;

            foreach (JSONObject bpmChangeJson in bpmChangeJsonList)
            {
                BeatManager.Instance.AddNewBPMInfo(
                    GetIntData(bpmChangeJson, "bar"),
                    GetFloatData(bpmChangeJson, "beat"),
                    GetFloatData(bpmChangeJson, "bpm"),
                    GetBoolData(bpmChangeJson, "stopEffect")
                    );
            }


            JSONObject ctChangesJson = _noteDataJson.GetField("CTChanges");
            List<JSONObject> ctChangeJsonList = ctChangesJson.list;

            BeatManager.Instance.CTInfos.Clear();
            foreach (JSONObject ctChangeJson in ctChangeJsonList)
            {
                CTInfo ct = new CTInfo()
                {
                    bar = GetIntData(ctChangeJson, "bar"),
                    numerator = GetIntData(ctChangeJson, "numerator")
                };
                BeatManager.Instance.CTInfos.Add(ct);
            }


            JSONObject notesJson = _noteDataJson.GetField("Notes");
            List<JSONObject> noteJsonList = notesJson.list;

            foreach (JSONObject noteJson in noteJsonList)
            {
                Note n = GetNoteFromJSON(noteJson);
                if (n != null)
                    NoteManager.Instance.AddNote(n);
            }


            JSONObject headerJson = _noteDataJson.GetField("Header");
            // MusicManager.Instance.MusicStartTime = (float)GetIntData(headerJson, "musicStartTime") * 0.001f;
            MusicManager.Instance.MusicStartTime = (float)GetIntData(headerJson, "musicStartTime") * 0.001f + 0.033f;
            BeatManager.Instance.StandardBPM = GetFloatData(headerJson, "standardBPM");

            Debug.Log("standardBPM : " + BeatManager.Instance.StandardBPM.ToString("F06"));
        }
        
        private Note GetNoteFromJSON(JSONObject _json)
        {
            Note n = null;

            string noteTypeName = GetStringData(_json, "noteType");
            switch (noteTypeName)
            {
                case "NormalNote":
                    n = new NormalNote()
                    {
                        noteType = NoteType.Normal,
                    };
                    break;
                case "PopNote":
                    n = new PopNote()
                    {
                        noteType = NoteType.Pop,
                    };
                    break;
                case "MineNote":
                    n = new MineNote()
                    {
                        noteType = NoteType.Mine,
                    };
                    break;
                case "LongNote":
                    {
                        n = new LongNote()
                        {
                            noteType = NoteType.Long,
                            endBar = GetIntData(_json, "endBar"),
                            endBeat = GetFloatData(_json, "endBeat"),
                            connectedRail = GetIntData(_json, "connectedRail", -1)
                        };
                    }
                    break;
                case "SpaceNote":
                    {
                        n = new SpaceNote()
                        {
                            noteType = NoteType.Space,
                            endBar = GetIntData(_json, "endBar"),
                            endBeat = GetFloatData(_json, "endBeat")
                        };
                    }
                    break;
                case "EffectNote":
                    {
                        n = new EffectNote()
                        {
                            noteType = NoteType.Effect,
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
                default:
                    break;
            }
            if (n == null)
                return null;

            n.bar = GetIntData(_json, "bar");
            n.beat = GetFloatData(_json, "beat");
            n.railNumber = GetIntData(_json, "railNumber");

            return n;
        }

        private string GetStringData(JSONObject _json, string _field)
        {
             JSONObject j = _json.GetField(_field);
            if (j == null)
                return "";
            return j.str;                
        }

        private int GetIntData(JSONObject _json, string _field, int _default = 0)
        {
            JSONObject j = _json.GetField(_field);
            if (j == null)
                return _default;
            return (int)j.i;
        }

        private float GetFloatData(JSONObject _json, string _field)
        {
            JSONObject j = _json.GetField(_field);
            if (j == null)
                return 0.0f;
            return j.f;
        }

        private bool GetBoolData(JSONObject _json, string _field)
        {
            JSONObject j = _json.GetField(_field);
            if (j == null)
                return false;
            return j.b;
        }
    }
}