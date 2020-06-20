using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PopTheCircle.OldNoteDataConverter
{
    public class NoteDataJSONConverter : MonoBehaviour
    {
        public const int NoteDataFileVersion = 1;
        private const float minimumBarBeatUnit = 1.0f / (192.0f / 32.0f);

        public NoteManager nm;
        
        private float oneBarTimeLength = 1.0f;



        public JSONObject NoteDataToJSON()
        {
            try
            {
                JSONObject noteDataJson = new JSONObject();
                noteDataJson.AddField("version", NoteDataFileVersion);

                JSONObject headerJson = new JSONObject();
                headerJson.AddField("musicFilePath", "-");
                headerJson.AddField("musicTitle", "-");
                headerJson.AddField("musicArtist", "-");
                headerJson.AddField("musicBPM", nm.BPM.ToString());
                headerJson.AddField("musicStartTime", nm.StartTime);
                headerJson.AddField("standardBPM", nm.BPM.ToString());
                noteDataJson.AddField("Header", headerJson);

                oneBarTimeLength = 60.0f / (float)nm.BPM;

                JSONObject notesJson = new JSONObject();
                for (int i = 0; i < nm.noteDatas.Count; ++i)
                {
                    NoteData n = nm.noteDatas[i];
                    JSONObject nJson = GetNoteJSON(n);
                    if (nJson != null)
                        notesJson.Add(nJson);
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
            catch (Exception _e)
            {
                Debug.LogError("NoteDataJSONConverter::Converter main catched error : " + _e.ToString());
                return null;
            }
        }

        private JSONObject GetNoteJSON(NoteData _n)
        {
            try
            {
                string noteTypeName = _n.NoteDataType.ToString();

                float originBarBeat = (_n.Time / oneBarTimeLength);
                // float restedBarBeatUnit = originBarBeat % minimumBarBeatUnit;
                // float nearestBarBeat = (float)((int)(originBarBeat / minimumBarBeatUnit)) * minimumBarBeatUnit;
                // if (restedBarBeatUnit >= minimumBarBeatUnit * 0.5f)
                //     nearestBarBeat += minimumBarBeatUnit;
                float nearestBarBeat = originBarBeat;

                // Debug.Log("[" + noteTypeName + "] " + _n.Time + " -> " + originBarBeat);

                int bar = (int)nearestBarBeat;
                int beat = Mathf.RoundToInt((nearestBarBeat - (float)bar) * 192.0f);
                if (beat >= 192)
                {
                    beat = 0;
                    bar += 1;
                }

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
                            float originEndBarBeat = (_n.LongEndTime / oneBarTimeLength);
                            // float restedEndBarBeatUnit = originEndBarBeat % minimumBarBeatUnit;
                            // float nearestEndBarBeat = (float)((int)(originEndBarBeat / minimumBarBeatUnit)) * minimumBarBeatUnit;
                            // if (restedEndBarBeatUnit >= minimumBarBeatUnit * 0.5f)
                            //     nearestEndBarBeat += minimumBarBeatUnit;
                            float nearestEndBarBeat = originEndBarBeat;

                            int endBar = (int)nearestEndBarBeat;
                            int endBeat = Mathf.RoundToInt((nearestEndBarBeat - (float)endBar) * 192.0f);
                            if (endBeat >= 192)
                            {
                                endBeat = 0;
                                endBar += 1;
                            }

                            noteJson.AddField("endBar", endBar);
                            noteJson.AddField("endBeat", endBeat);
                            noteJson.AddField("connectedRail", _n.endLine);
                        }
                        break;
                    case "SpaceNote":
                        {
                            int endBar = 0;
                            int endBeat = 0;

                            if (_n.length != 0)
                            {
                                float originEndBarBeat = (_n.LongEndTime / oneBarTimeLength);
                                // float restedEndBarBeatUnit = originEndBarBeat % minimumBarBeatUnit;
                                // float nearestEndBarBeat = (float)((int)(originEndBarBeat / minimumBarBeatUnit)) * minimumBarBeatUnit;
                                // if (restedEndBarBeatUnit >= minimumBarBeatUnit * 0.5f)
                                //     nearestEndBarBeat += minimumBarBeatUnit;
                                float nearestEndBarBeat = originEndBarBeat;

                                endBar = (int)nearestEndBarBeat;
                                endBeat = Mathf.RoundToInt((nearestEndBarBeat - (float)endBar) * 192.0f);
                                if (endBeat >= 192)
                                {
                                    endBeat = 0;
                                    endBar += 1;
                                }
                            }

                            noteJson.AddField("endBar", endBar);
                            noteJson.AddField("endBeat", endBeat);
                            noteJson.SetField("railNumber", 4);
                        }
                        break;
                }

                return noteJson;
            }
            catch (Exception _e)
            {
                Debug.LogError("NoteDataJSONConverter::Note converting catched error : " + _e.ToString());
            }
            return null;
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