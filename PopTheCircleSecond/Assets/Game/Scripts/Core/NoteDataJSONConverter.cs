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

            JSONObject noteDataJson = new JSONObject();
            int noteDataVersion = GetIntData(_noteDataJson, "version");
            
            
            JSONObject bpmChangesJson = _noteDataJson.GetField("BPMChanges");
            List<JSONObject> bpmChangeJsonList = bpmChangesJson.list;

            foreach (JSONObject bpmChangeJson in bpmChangeJsonList)
            {
                BPMInfo bpm = new BPMInfo()
                {
                    bpm = GetFloatData(bpmChangeJson, "bpm"),
                    bar = GetIntData(bpmChangeJson, "bar"),
                    beat = GetFloatData(bpmChangeJson, "beat"),
                    stopEffect = GetBoolData(bpmChangeJson, "stopEffect")
                };
                BeatManager.Instance.AddNewBPMInfo(
                    GetIntData(bpmChangeJson, "bar"),
                    GetFloatData(bpmChangeJson, "beat"),
                    GetFloatData(bpmChangeJson, "bpm"),
                    GetBoolData(bpmChangeJson, "stopEffect")
                    );
            }


            JSONObject notesJson = _noteDataJson.GetField("Notes");
            List<JSONObject> noteJsonList = notesJson.list;

            foreach (JSONObject noteJson in noteJsonList)
            {
                Note n = GetNoteFromJSON(noteJson);
                NoteManager.Instance.AddNote(n);
            }


            JSONObject headerJson = _noteDataJson.GetField("Header");
            MusicManager.Instance.MusicStartTime = (float)GetIntData(headerJson, "musicStartTime") * 0.001f;
            BeatManager.Instance.StandardBPM = (float)GetFloatData(headerJson, "standardBPM");
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
                case "LongNote":
                    {
                        n = new LongNote()
                        {
                            endBar = GetIntData(_json, "endBar"),
                            endBeat = GetFloatData(_json, "endBeat")
                        };
                    }
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

        private int GetIntData(JSONObject _json, string _field)
        {
            JSONObject j = _json.GetField(_field);
            if (j == null)
                return 0;
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