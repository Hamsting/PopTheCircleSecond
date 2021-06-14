using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PopTheCircle.Game
{
    public enum ClearGaugeType
    {
        Easy = 2,
        Normal = 3,
        Hard = 4,

        NormalLife = 7,
        HardLife = 8,

        FullCombo = 11,
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
                case ClearGaugeType.Easy:
                    _increaseAmount = GlobalDefines.ClearGaugeHealRateEasy / totalTickUnit * GlobalDefines.ClearGaugeLongNoteTickRatio;
                    _decreaseAmount = GlobalDefines.ClearGaugeDamageEasy;
                    break;
                default:
                case ClearGaugeType.Normal:
                    _increaseAmount = GlobalDefines.ClearGaugeHealRateNormal / totalTickUnit * GlobalDefines.ClearGaugeLongNoteTickRatio;
                    _decreaseAmount = GlobalDefines.ClearGaugeDamageNormal;
                    break;
                case ClearGaugeType.Hard:
                    _increaseAmount = GlobalDefines.ClearGaugeHealRateHard / totalTickUnit * GlobalDefines.ClearGaugeLongNoteTickRatio;
                    _decreaseAmount = GlobalDefines.ClearGaugeDamageHard;
                    break;


                case ClearGaugeType.NormalLife:
                    _increaseAmount = GlobalDefines.ClearGaugeHealRateNormalLife / totalTickUnit * GlobalDefines.ClearGaugeLongNoteTickRatio;
                    _decreaseAmount = GlobalDefines.ClearGaugeDamageNormalLife;
                    break;
                case ClearGaugeType.HardLife:
                    _increaseAmount = GlobalDefines.ClearGaugeHealRateHardLife / totalTickUnit * GlobalDefines.ClearGaugeLongNoteTickRatio;
                    _decreaseAmount = GlobalDefines.ClearGaugeDamageHardLife;
                    break;

                case ClearGaugeType.FullCombo:
                    _increaseAmount = GlobalDefines.ClearGaugeHealRateFullCombo / totalTickUnit * GlobalDefines.ClearGaugeLongNoteTickRatio;
                    _decreaseAmount = GlobalDefines.ClearGaugeDamageFullCombo;
                    break;
            }
        }

        public static string ClearGaugeTypeToString(ClearGaugeType _type)
        {
            switch (_type)
            {
                case ClearGaugeType.Easy:
                    return "Easy";
                case ClearGaugeType.Normal:
                    return "Normal";
                case ClearGaugeType.Hard:
                    return "Hard";

                case ClearGaugeType.NormalLife:
                    return "Normal Life";
                case ClearGaugeType.HardLife:
                    return "Hard Life";

                case ClearGaugeType.FullCombo:
                    return "Full Combo Only";

                default:
                    return "";
            }
        }

        public static bool CheckGaugeCleared(ClearGaugeType _type, float _amount)
        {
            switch (_type)
            {
                case ClearGaugeType.Easy:
                    {
                        if (_amount >= GlobalDefines.ClearGaugeCriteriaEasy)
                            return true;
                        else
                            return false;
                    }
                case ClearGaugeType.Normal:
                    {
                        if (_amount >= GlobalDefines.ClearGaugeCriteriaNormal)
                            return true;
                        else
                            return false;
                    }
                case ClearGaugeType.Hard:
                    {
                        if (_amount >= GlobalDefines.ClearGaugeCriteriaHard)
                            return true;
                        else
                            return false;
                    }

                case ClearGaugeType.NormalLife:
                case ClearGaugeType.HardLife:
                case ClearGaugeType.FullCombo:
                    {
                        if (_amount > 0.0f)
                            return true;
                        else
                            return false;
                    }

                default:
                    return false;
            }
        }
    }
}