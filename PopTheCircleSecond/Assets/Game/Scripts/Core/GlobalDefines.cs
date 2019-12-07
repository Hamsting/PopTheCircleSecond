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
        public const float DefaultBarCount = 48.0f;
        public const float RailLength = 30.0f;
        public const int TickBeatRate = BeatPerBar / 4;
        public const float TargetBPMForHalfTickBeatRate = 240.0f;


        // 판정 관련 상수
        public const float JudgePerfectTime     = 0.049f;
        public const float JudgeNiceTime        = 0.128f;
        public const float JudgeEarlyFailTime   = 0.192f;

        public const int LongNoteReleaseTerm = BeatPerBar / 2;

        // 입력 관련 상수
        public static float DragMinDistance = 200f;
        public static float DragMinDelta = 6f;

        // public const float JudgePerfectTime  = 0.216f;
        // public const float JudgePerfectTime  = 0.216f;
        // public const float JudgeNiceTime     = 0.433f;
        // public const float JudgeNiceTime     = 0.433f;
        // public const float JudgeEarlyFailTime= 0.583f;
        // public const float JudgeEarlyFailTime= 0.583f;
        // public const float JudgePerfectTime     = 0.116f;
        // public const float JudgeNiceTime        = 0.233f;
        // public const float JudgeEarlyFailTime   = 0.349f;
    }
}       