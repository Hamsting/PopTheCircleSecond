using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace PopTheCircle.Game
{
    [System.Serializable]
    public class NoteDataSet : System.Object
    {
        public string dataFullPath = "";
        public string musicFilePath = "";

        public string musicTitle = "Undefined";
        public string musicArtist = "Undefined";
        public string musicDisplayBpm = "0";
        public float musicStandardBpm = 0;
        public Sprite musicJacketSprite = null;

        public int noteDifficultyLevel = 0;
        public enum NoteDifficultyType { Unknown = 0, Normal = 1, Enhanced = 2, Extreme = 3, Special = 4, }
        public NoteDifficultyType noteDifficultyType = NoteDifficultyType.Unknown;



        public static NoteDataSet FromNoteDataPath(string _dataFullPath)
        {
            FileInfo fileInfo = new FileInfo(_dataFullPath);
            if (!fileInfo.Exists)
                return null;

            string noteDataJsonStr = File.ReadAllText(_dataFullPath, Encoding.UTF8);
            JSONObject noteDataJson = new JSONObject(noteDataJsonStr);
            if (noteDataJson == null)
                return null;

            JSONObject headerJson = noteDataJson.GetField("Header");
            if (headerJson == null)
                return null;

            NoteDataSet set = new NoteDataSet()
            {
                dataFullPath = _dataFullPath,
                musicFilePath           =      (headerJson.GetField("musicFilePath" )?.str ?? ""),

                musicTitle          =      (headerJson.GetField("musicTitle"    )?.str ?? "Undefined"),
                musicArtist         =      (headerJson.GetField("musicArtist"   )?.str ?? "Undefined"),
                musicDisplayBpm     =      (headerJson.GetField("musicBPM"      )?.str ?? "0"),
                musicStandardBpm    =      (headerJson.GetField("standardBPM"   )?.f   ?? 0.0f),
                // musicJacketSprite

                noteDifficultyLevel = (int)(headerJson.GetField("noteDifficultyLevel")?.i   ?? 0),
                noteDifficultyType  = (NoteDifficultyType)(headerJson.GetField("noteDifficultyType")?.i   ?? 0),
            };

            return set;
        }
    }
}