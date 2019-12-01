using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PopTheCircle.NoteEditor
{
    [System.Serializable]
    public class NoteRailLength
    {
        public int railNumber = 1;
        public float startBarBeat = 0.0f;
        public float barBeatLength = 4.0f;
        public float nextStartBarBeat = 4.0f;
    }
}