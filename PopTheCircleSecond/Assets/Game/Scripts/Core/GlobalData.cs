using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PopTheCircle.Game.NoteDataSet;

namespace PopTheCircle.Game
{
    public class GlobalData : Singleton<GlobalData>
    {
        [InspectorReadOnly] public AudioClip musicClip;
        [InspectorReadOnly] public JSONObject noteDataJson;



        [InspectorReadOnly] public NoteDataSet lastNoteDataSet = null;
        [InspectorReadOnly] public MusicUserScoreSet lastUserScoreSet = null;
        [InspectorReadOnly] public NoteDifficultyType lastNoteDifficultyType = NoteDifficultyType.Unknown;

        [InspectorReadOnly] public int lastMaxCombo = 0;
        [InspectorReadOnly] public int lastScore = 0;
        [InspectorReadOnly] public int lastJudgePerfectCount = 0;
        [InspectorReadOnly] public int lastJudgeNiceCount = 0;
        [InspectorReadOnly] public int lastJudgeMissCount = 0;
        [InspectorReadOnly] public int lastTotalCombo = 0;
        [InspectorReadOnly] public ClearGaugeType lastClearGaugeType = ClearGaugeType.Normal;
        [InspectorReadOnly] public float lastClearGauge = 0.0f;



        protected override void Awake()
        {
            base.Awake();

            DontDestroyOnLoad(this.gameObject);
        }
    }
}