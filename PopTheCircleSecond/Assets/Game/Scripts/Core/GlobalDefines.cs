using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PopTheCircle.Game
{
    /// <summary>
    /// 여러 상수를 저장하는 클래스이다.
    /// </summary>
    public static class GlobalDefines
    {
        // 박자노트의 비트 계산 관련 상수
        public const int BeatPerBar = 192;
        public const float DefaultBarCount = 12.0f;
        public const float RailLength = 16.0f + 1.0f;
        public const int TickBeatRate = BeatPerBar / 2;
        public const float TargetBPMForHalfTickBeatRate = 240.0f;


        // 판정 관련 상수
        public const float JudgePerfectTime     = 0.066f;
        public const float JudgeNiceTime        = 0.166f;
        public const float JudgeEarlyFailTime   = 0.216f;

        public const int LongNoteReleaseTerm = BeatPerBar / 2;


        // 입력 관련 상수
        public static float DragMinDistance = 200f;
        public static float DragMinDelta = 6f;


        // 클리어게이지 관련 상수
        public const int   ClearGaugeLongNoteTickRatio = 2;

        public const float ClearGaugeHealRateEasy = 270.0f;
        public const float ClearGaugeDamageEasy = 1.5f;
        public const float ClearGaugeCriteriaEasy = 70.0f;

        public const float ClearGaugeHealRateNormal = 230.0f;
        public const float ClearGaugeDamageNormal   = 3.0f; // 4.0f; // 2.0f;
        public const float ClearGaugeCriteriaNormal = 70.0f;

        public const float ClearGaugeHealRateHard = 180.0f;
        public const float ClearGaugeDamageHard = 6.0f;
        public const float ClearGaugeCriteriaHard = 70.0f;

        public const float ClearGaugeHealRateNormalLife = 130.0f;
        public const float ClearGaugeDamageNormalLife = 9.0f;
        public const float ClearGaugeCriteriaNormalLife = 0.01f;

        public const float ClearGaugeHealRateHardLife = 110.0f;
        public const float ClearGaugeDamageHardLife = 15.0f;
        public const float ClearGaugeCriteriaHardLife = 0.01f;

        public const float ClearGaugeHealRateFullCombo = 0.0f;
        public const float ClearGaugeDamageFullCombo = 200.0f;
        public const float ClearGaugeCriteriaFullCombo = 0.01f;






        // public const float JudgePerfectTime  = 0.216f;
        // public const float JudgePerfectTime  = 0.216f;
        // public const float JudgeNiceTime     = 0.433f;
        // public const float JudgeNiceTime     = 0.433f;
        // public const float JudgeEarlyFailTime= 0.583f;
        // public const float JudgeEarlyFailTime= 0.583f;
        // public const float JudgePerfectTime     = 0.116f;
        // public const float JudgeNiceTime        = 0.233f;
        // public const float JudgeEarlyFailTime   = 0.349f;
        //public const float JudgePerfectTime = 0.066f;
        //public const float JudgeNiceTime = 0.166f;
        //public const float JudgeEarlyFailTime = 0.192f;
    }
}       