using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PopTheCircle.NoteEditor
{
    [System.Serializable]
    public class NoteData
    {
        // Header
        public string musicFilePath = "";
        public string musicTitle = "";
        public string musicArtist = "";
        public string musicBPM = "";
        public int musicStartTime = 0;
        public float standardBPM = 60.0f;

        public int noteDifficultyLevel = 0;
        public enum NoteDifficultyType { Unknown = 0, Normal = 1, Enhanced = 2, Extreme = 3, Special = 4, }
        public NoteDifficultyType noteDifficultyType = NoteDifficultyType.Unknown;

        // Body
        public List<Note> notes;
    }
}