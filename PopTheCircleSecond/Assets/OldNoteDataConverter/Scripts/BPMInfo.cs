using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PopTheCircle.OldNoteDataConverter
{
    /// <summary>
    /// BPM 정보이다.
    /// </summary>
    [System.Serializable]
    public class BPMInfo
    {
        public float bpm = 60.0f;
        public int bar = 0;
        public float beat = 0.0f;
        public bool stopEffect = false;

        public double position = 0.0d;
        public float time = 0.0f;
    }
}
