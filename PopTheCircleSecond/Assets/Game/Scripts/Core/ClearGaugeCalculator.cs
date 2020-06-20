using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PopTheCircle.Game
{
    public enum ClearGaugeType
    {
        Normal = 0,
        Easy = 1,
        Hard = 2,
        ExHard = 3,
        Life = 4,
        ExLife = 5,
        FullCombo = 6,
    }

    public static class ClearGaugeCalculator
    {
        public static void CalculateResult(ClearGaugeType _gaugeType, int _totalTick, int _longNoteTotalTick, out float _increaseAmount, out float _decreaseAmount)
        {
            _increaseAmount = 0.0f;
            _decreaseAmount = 0.0f;

            int chipNoteCount = _totalTick - _longNoteTotalTick;
            int totalTickUnit = chipNoteCount * GlobalDefines.ClearGaugeLongNoteTickRatio + _longNoteTotalTick;

            switch (_gaugeType)
            {
                default:
                case ClearGaugeType.Normal:
                    _increaseAmount = GlobalDefines.ClearGaugeHealRateNormal / totalTickUnit * GlobalDefines.ClearGaugeLongNoteTickRatio;
                    _decreaseAmount = GlobalDefines.ClearGaugeDamageNormal;
                    break;
            }
        }
    }
}