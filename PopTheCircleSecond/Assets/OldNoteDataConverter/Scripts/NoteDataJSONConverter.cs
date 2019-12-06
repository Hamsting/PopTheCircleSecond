using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PopTheCircle.OldNoteDataConverter
{
    public class NoteDataJSONConverter : MonoBehaviour
    {
        public const int NoteDataFileVersion = 1;

        public NoteManager nm;



        public JSONObject NoteDataToJSON()
        {
            JSONObject noteDataJson = new JSONObject();
            noteDataJson.AddField("version", NoteDataFileVersion);
                        
            JSONObject headerJson = new JSONObject();
            headerJson.AddField("musicFilePath", "-");
            headerJson.AddField("musicTitle", "-");
            headerJson.AddField("musicArtist", "-");
            headerJson.AddField("musicBPM", nm.BPM.ToString());
            headerJson.AddField("musicStartTime", nm.StartTime.ToString());
            headerJson.AddField("standardBPM", nm.BPM.ToString());
            noteDataJson.AddField("Header", headerJson);


            JSONObject notesJson = new JSONObject();
            foreach (NoteData n in nm.noteDatas)
            {
                notesJson.Add(GetNoteJSON(n));
            }
            noteDataJson.AddField("Notes", notesJson);
            
            JSONObject bpmChangesJson = new JSONObject();
            BPMInfo bpm = new BPMInfo()
            {
                bpm = (float)nm.BPM,
                bar = 0,
                beat = 0.0f,
                stopEffect = false,
            };
            JSONObject bpmChangeJson = new JSONObject();
            bpmChangeJson.AddField("bpm", bpm.bpm);
            bpmChangeJson.AddField("bar", bpm.bar);
            bpmChangeJson.AddField("beat", bpm.beat);
            bpmChangeJson.AddField("stopEffect", bpm.stopEffect);
            bpmChangesJson.Add(bpmChangeJson);
            noteDataJson.AddField("BPMChanges", bpmChangesJson);


            JSONObject ctChangesJson = new JSONObject();
            CTInfo ct = new CTInfo()
            {
                bar = 0,
                numerator = 4,                
            };
            JSONObject ctChangeJson = new JSONObject();
            ctChangeJson.AddField("bar", ct.bar);
            ctChangeJson.AddField("numerator", ct.numerator);
            ctChangesJson.Add(ctChangeJson);
            noteDataJson.AddField("CTChanges", ctChangesJson);


            return noteDataJson;
        }

        private JSONObject GetNoteJSON(NoteData _n)
        {
            string noteTypeName = _n.NoteDataType.ToString();

            float oneBarTimeLength = 60.0f / (float)nm.BPM;
            int bar = (int)(_n.Time / oneBarTimeLength);
            float beat = ((_n.Time % oneBarTimeLength) / oneBarTimeLength) * 192.0f;

            JSONObject noteJson = new JSONObject();
            noteJson.AddField("noteType", noteTypeName);
            noteJson.AddField("bar", bar);
            noteJson.AddField("beat", beat);
            noteJson.AddField("railNumber", _n.line);

            switch (noteTypeName)
            {
                default:
                case "NormalNote":
                case "PopNote":
                    break;
                case "LongNote":
                    {
                        int endBar = (int)(_n.LongEndTime / oneBarTimeLength);
                        float endBeat = ((_n.LongEndTime % oneBarTimeLength) / oneBarTimeLength) * 192.0f;

                        noteJson.AddField("endBar", endBar);
                        noteJson.AddField("endBeat", endBeat);
                    }
                    break;
            }

            return noteJson;
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

        private Vector2 GetVector2Data(JSONObject _json, string _field)
        {
            JSONObject j = _json.GetField(_field);
            if (j == null)
                return Vector2.zero;
            return JSONTemplates.ToVector2(j);
        }
    }
}