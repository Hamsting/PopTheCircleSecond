using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;



namespace PopTheCircle.OldNoteDataConverter
{
	public class NoteManager : MonoBehaviour
	{
        public string directoryPath = "";
        public string fileName = "";
        public NoteDataJSONConverter con;
        public List<NoteData> noteDatas;

        // Old Datas
        private int bpm = 0;
        private int syncNumerator = 0;
        private int syncDenominator = 0;

        // New Datas
        public int BPM
        {
            get
            {
                return bpm;
            }
        }
        public int StartTime
        {
            get
            {
                return (int)((60.0f / (float)bpm) * ((float)syncNumerator / (float)syncDenominator) * 1000.0f);
            }
        }




        private void Awake()
		{
			noteDatas = new List<NoteData>();
		}

        private void Start()
        {
            ReadNoteDataString();
            WriteNoteDataString();
        }


        public void CreateNote(int _timeMillis, int _line, int _length = 0, bool _isPop = false, bool _isSpace = false, int _endLine = 0)
		{
			NoteData noteData = new NoteData()
			{
				timeMillis = _timeMillis,
				line = _line,
				length = _length,
				isPop = _isPop,
				isSpace = _isSpace,
				endLine = _endLine
			};

			noteDatas.Add(noteData);
		}
        
		public void ReadNoteDataString()
		{
			NoteData n = null;

            string fullPath = directoryPath + fileName;
            string noteDataString = File.ReadAllText(fullPath);

            string[] lines = noteDataString.Split(new char[] { '\n' });
			foreach (string line in lines)
			{
				string[] words = line.Split(new string[] { " | " }, System.StringSplitOptions.None);
				switch (words[0])
				{
					case "NoteDataStart":
						break;
					case "NoteDataEnd":
						break;
					case "NoteMakerSettings":
						bpm = int.Parse(words[1]);
						syncNumerator = int.Parse(words[3]);
						syncDenominator = int.Parse(words[4]);
						break;
					case "NormalNote":
						n = new NoteData()
						{
							line = int.Parse(words[1]) - 1,
							timeMillis = (int)(float.Parse(words[2]) * 1000.0f)
						};
						noteDatas.Add(n);
						break;
					case "PopNote":
						n = new NoteData()
						{
							line = int.Parse(words[1]) - 1,
							timeMillis = (int)(float.Parse(words[2]) * 1000.0f),
							isPop = true
						};
						noteDatas.Add(n);
						break;
					case "LongNote":
						n = new NoteData()
						{
							line = int.Parse(words[1]) - 1,
							timeMillis = (int)(float.Parse(words[2]) * 1000.0f),
							length = (int)((float.Parse(words[3]) - float.Parse(words[2])) * 1000.0f),
							endLine = int.Parse(words[4]) - 1
						};
						noteDatas.Add(n);
						break;
					case "SpaceNote":
						n = new NoteData()
						{
							timeMillis = (int)(float.Parse(words[1]) * 1000.0f),
							length = (int)((float.Parse(words[2]) - float.Parse(words[1])) * 1000.0f),
							isSpace = true
						};
						noteDatas.Add(n);
						break;
					default:
						Debug.Log("Unknown NoteData Word : " + words[0]);
						break;
				}
			}
		}

		public void WriteNoteDataString()
		{
            JSONObject json = con.NoteDataToJSON();

            string fullPath = directoryPath + fileName + "_converted.ntd";
            File.WriteAllText(fullPath, json.ToString(true));
        }
	}
}