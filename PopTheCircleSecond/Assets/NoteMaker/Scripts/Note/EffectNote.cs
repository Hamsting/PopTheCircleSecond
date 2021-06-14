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
        
        public override Note GetInstance()
        {
            return new EffectNote()
            {
                bar = this.bar,
                beat = this.beat,
                noteType = this.noteType,
                railNumber = this.railNumber,

                endBar = this.endBar,
                endBeat = this.endBeat,
                connectedRail = this.connectedRail,

                seType = this.seType,
                seTickBeatRate = this.seTickBeatRate,

                position = this.position,
                time = this.time,
                isMissed = this.isMissed,
                noteObject = this.noteObject,
            };
        }
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
        E017_DrumSnare_5,
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