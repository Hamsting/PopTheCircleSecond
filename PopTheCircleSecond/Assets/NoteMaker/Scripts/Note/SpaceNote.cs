using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PopTheCircle.NoteEditor
{
    [System.Serializable]
    public class SpaceNote : LongNote
    {
        public bool IsLongType
        {
            get
            {
                if (endBar == 0 && endBeat <= 0.0f)
                    return false;
                else
                    return true;
            }
        }
    }
}