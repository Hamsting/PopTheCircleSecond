using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using UnityEngine;



namespace PopTheCircle.OldNoteDataConverter
{
	public class NoteManager : MonoBehaviour
	{
        public string readDirectoryPath = "";
        public string writeDirectoryPath = "";
        public NoteDataJSONConverter con;
        public List<NoteData> noteDatas;

        private string fileName = "";
        Thread t;

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
                return -(int)((60.0f / (float)bpm) * ((float)syncNumerator / (float)syncDenominator) * 1000.0f) * 2;
            }
        }




        private void Awake()
		{
			noteDatas = new List<NoteData>();
		}

        private void Start()
        {
            t = new Thread(ConvertAll);
            t.Start();

            // ConvertAll();
            
            // ReadNoteDataString();
            // WriteNoteDataString();
        }

        private void OnDestroy()
        {
            if (t != null && t.IsAlive)
            {
                t.Abort();
                t = null;
            }
        }

        private void ConvertAll()
        {
            var noteDataFiles = Directory.EnumerateFiles(readDirectoryPath, "*.*", SearchOption.AllDirectories)
                                .Where(s => s.EndsWith(".ntd") && (!s.Contains("_Converted") && !s.Contains("_converted")));

            int i = 0;
            int count = noteDataFiles.Count();
            Debug.Log("Total " + count + " files found.");

            foreach (var noteDataFile in noteDataFiles)
            {
                ++i;

                fileName = noteDataFile.Replace(readDirectoryPath, "");

                string debugProgressStr = "[" + i + "/" + count + "]";
                Debug.Log(debugProgressStr + " File found : " + readDirectoryPath + fileName + "\nThis will be saved at : " + writeDirectoryPath + fileName);

                ReadNoteDataString();
                WriteNoteDataString();

                noteDatas.Clear();
            }
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

            string fullPath = readDirectoryPath + fileName;
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
            if (json == null)
                return;

            string fullPath = writeDirectoryPath + fileName.Replace(".ntd", "") + "_converted.ntd";

            string fullDirPath = fullPath.Substring(0, fullPath.LastIndexOf("\\"));
            if (!Directory.Exists(fullDirPath))
                Directory.CreateDirectory(fullDirPath);

            File.WriteAllText(fullPath, json.ToString(true), System.Text.Encoding.Unicode);
        }
	}
}