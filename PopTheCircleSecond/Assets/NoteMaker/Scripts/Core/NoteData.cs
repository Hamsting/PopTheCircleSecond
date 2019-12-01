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

        // Body
        public List<Note> notes;
    }
}