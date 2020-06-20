using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PopTheCircle.NoteEditor
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


        public bool ContainsInTickBarBeat(float _startBarBeat, float _endBarBeat)
        {
            float tickRateBarBeat = BeatManager.ToBarBeat(0, (float)seTickBeatRate);
            float tickStartBarBeat = BeatManager.ToBarBeat(bar, beat) + tickRateBarBeat;
            float tickEndBarBeat = BeatManager.ToBarBeat(endBar, endBeat);
            float tickLengthBarBeat = tickEndBarBeat - BeatManager.ToBarBeat(bar, beat) + tickRateBarBeat * 0.5f;
            tickLengthBarBeat = tickLengthBarBeat - (tickLengthBarBeat % tickRateBarBeat);
            tickEndBarBeat = tickStartBarBeat + tickLengthBarBeat - tickRateBarBeat * 1.5f;

            if (_startBarBeat > tickEndBarBeat || _endBarBeat < tickStartBarBeat)
                return false;

            float curBarBeatDiff = _endBarBeat - tickStartBarBeat;
            float curBarBeatDiffWithTickGrided = curBarBeatDiff - (curBarBeatDiff % tickRateBarBeat);
            float targetTickBarBeat = tickStartBarBeat + curBarBeatDiffWithTickGrided;

            if (targetTickBarBeat < _startBarBeat || targetTickBarBeat > _endBarBeat)
                return false;
            return true;
        }
    }

    public enum EffectNoteSEType
    {
        None = 0,
        Clap = 1,
        SharpKick = 2,
    }
}