using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PopTheCircle.OldNoteDataConverter
{
    /// <summary>
    /// CT(Change Time, 변박) 정보이다.
    /// </summary>
    [System.Serializable]
    public class CTInfo
    {
        public int bar = 0;
        // public float beat = 0.0f;

        public int numerator = 4;
        // 음표(분모 부분)는 변경할 수 없도록 고정.
        // public int denominator = 4;
    }
}
