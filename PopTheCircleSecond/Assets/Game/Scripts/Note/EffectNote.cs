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
        Clap = 1,
        SharpKick = 2,
    }
}