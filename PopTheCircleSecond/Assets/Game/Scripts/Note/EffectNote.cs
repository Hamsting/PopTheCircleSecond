using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PopTheCircle.Game
{
    [System.Serializable]
    public class EffectNote : LongNote
    {
        /// <summary>
        /// 해당 노트가 롱노트 형태인지의 여부이다.
        /// </summary>
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

        /// <summary>
        /// 재생할 이펙트음의 종류이다.
        /// </summary>
        public EffectNoteSEType seType = EffectNoteSEType.None;

        /// <summary>
        /// 노트가 롱노트일 경우, 이펙트음 재생의 비트간격이다.
        /// </summary>
        public int seTickBeatRate = GlobalDefines.BeatPerBar / 1;

        /// <summary>
        /// 노트가 롱노트일 경우, 다음으로 이펙트음이 재생될 박자비트이다. 해당 값은 인게임에서 대입한다.
        /// </summary>
        public float seNextTickedBarBeat = 0.0f;

        /// <summary>
        /// 노트가 롱노트일 경우, 이펙트음의 재생이 시작될 박자비트이다. 해당 값은 인게임에서 대입한다.
        /// </summary>
        public float seTickStartBarBeat = 0.0f;

        /// <summary>
        /// 노트가 롱노트일 경우, 이펙트음의 재생이 끝날 박자비트이다. 해당 값은 인게임에서 대입한다.
        /// </summary>
        public float seTickEndBarBeat = 0.0f;
    }

    public enum EffectNoteSEType
    {
        None = 0,
        Clap = -1, // OLD
        SharpKick = -2, // OLD

        E001_DrumClap_1 = 1,
        E002_SharpKick_1,
        E003_DrumClap_2,
        E004_BangKit_1,
        E005_BangKit_2,
        E006_BangKit_3,
        E007_BangKit_4,
        E008_DrumKick_1,
        E009_DrumKick_2,
        E010_DrumKick_3,
        E011_DrumKick_4,
        E012_DrumKick_5,
        E013_DrumSnare_1,
        E014_DrumSnare_2,
        E015_DrumSnare_3,
        E016_DrumSnare_4,
        E017_DrunSnare_5,
        E018_Dubstep_1,
        E019_EastKit_1,
        E020_EchoKit_1,
        E021_LowKit_1,
        E022_ScrubKit_1,
        E023_StompKit_1,
        E024_StompKit_2,
        E025_StompKit_3,
        E026_StompKit_4,
    }
}