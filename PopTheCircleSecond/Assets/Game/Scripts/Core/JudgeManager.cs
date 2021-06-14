using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Events;

namespace PopTheCircle.Game
{
    /// <summary>
    /// 사용자의 터치, 드래그 등 입력 관리를 담당한다.
    /// </summary>
    public class JudgeManager : Singleton<JudgeManager>
    {
        public UnityEvent judgeEvent;

        [InspectorReadOnly]
        public float clearGaugeIncreaseAmount = 0.0f;
        [InspectorReadOnly]
        public float clearGaugeDecreaseAmount = 0.0f;
        [SerializeField, InspectorReadOnly]
        private int tickLastBar = 0;
        [SerializeField, InspectorReadOnly]
        private float tickLastBeat = 0;



        protected override void Awake()
        {
            base.Awake();
        }
        
        public void UpdateJudgeForNotes()
        {
            UpdateTickBeat();

            for (int i = 0; i < NoteManager.Instance.spawnedNotes.Count; ++i)
            {
                Note note = NoteManager.Instance.spawnedNotes[i];

                if (note.noteType == NoteType.Long || 
                    (note.noteType == NoteType.Space && ((SpaceNote)note).IsLongType) ||
                    (note.noteType == NoteType.Effect && ((EffectNote)note).IsLongType))
                {
                    LongNote longNote = (LongNote)note;

                    // 롱노트 최초 틱 판정
                    if (!longNote.firstTicked)
                    {
                        float longFirstTickLimitTime = longNote.time + GlobalDefines.JudgeNiceTime;
                        if (longNote.firstPressed && BeatManager.Instance.GameTime <= longFirstTickLimitTime && BeatManager.Instance.GameTime >= longNote.time)
                        {
                            UpdateJudgeAndCombo(note, 1, 0, false, false);
                            longNote.firstTicked = true;

                        }
                        else if (!longNote.firstPressed && BeatManager.Instance.GameTime > longFirstTickLimitTime)
                        {
                            UpdateJudgeAndCombo(note, 0, 0, false, false);
                            longNote.firstTicked = true;
                        }
                    }


                    // 롱노트 릴리즈 판정
                    float longReleaseBarBeat = BeatManager.Instance.CorrectBarBeat(
                        longNote.lastPressedBarBeat
                        + ((float)GlobalDefines.LongNoteReleaseTerm / (float)GlobalDefines.BeatPerBar)
                        );
                    if (longReleaseBarBeat < longNote.tickEndBarBeat && longReleaseBarBeat <= BeatManager.ToBarBeat(BeatManager.Instance.Bar, BeatManager.Instance.Beat))
                        longNote.pressed = false;


                    // 롱노트 종료 판정
                    if (BeatManager.Instance.GameTime >= longNote.endTime)
                    {
                        if (longNote.pressed)
                        {
                            NoteManager.Instance.DespawnNote(note, false);
                            UpdateJudgeAndCombo(note, 1, 0, false, true);
                        }
                        else
                        {
                            if (!longNote.firstTicked)
                                UpdateJudgeAndCombo(note, 0, 0, false, false);
                            NoteManager.Instance.DespawnNote(note, false);
                            UpdateJudgeAndCombo(note, 0, 0, false, true);
                        }
                        --i;
                    }
                }
                else if (note.noteType == NoteType.Mine && BeatManager.Instance.GameTime >= note.time)
                {
                    bool isMissed = InputManager.Instance.inputStates[note.railNumber] != 0;
                    UpdateJudgeAndCombo(note, isMissed ? 0 : 1, 0, false, false);
                    NoteManager.Instance.DespawnNote(note, isMissed);
                }
                else if (BeatManager.Instance.GameTime >= note.time + GlobalDefines.JudgeNiceTime)
                {
                    NoteManager.Instance.DespawnNote(note, true);
                    UpdateJudgeAndCombo(note, 0, 0, false, false);
                    --i;
                }
            }
        }
        
        private void UpdateTickBeat()
        {
            float curBarBeat = BeatManager.ToBarBeat(BeatManager.Instance.Bar, BeatManager.Instance.Beat);
            float tickLastBarBeat = BeatManager.ToBarBeat(tickLastBar, tickLastBeat);
            if (curBarBeat >= tickLastBarBeat)
            {
                foreach (Note note in NoteManager.Instance.spawnedNotes)
                {
                    if (note.noteType == NoteType.Long ||
                       (note.noteType == NoteType.Space && ((SpaceNote)note).IsLongType) ||
                       (note.noteType == NoteType.Effect && ((EffectNote)note).IsLongType))
                    {
                        LongNote longNote = (LongNote)note;
                        if (tickLastBarBeat >= longNote.tickStartBarBeat &&
                            tickLastBarBeat <= longNote.tickEndBarBeat)
                        {
                            UpdateJudgeAndCombo(longNote, (longNote.pressed) ? 1 : 0, 0, true, false);
                        }
                    }
                }

                float bpm = BeatManager.Instance.CurrentBPM;
                int tickBeatRate = GlobalDefines.TickBeatRate;
                while (bpm >= GlobalDefines.TargetBPMForHalfTickBeatRate)
                {
                    tickBeatRate *= 2;
                    bpm *= 0.5f;
                }

                tickLastBeat += tickBeatRate;
                if (tickLastBeat >= GlobalDefines.BeatPerBar)
                {
                    tickLastBar += (int)tickLastBeat / GlobalDefines.BeatPerBar;
                    tickLastBeat = tickLastBeat % (float)GlobalDefines.BeatPerBar;
                }
                if (!BeatManager.Instance.IsPossibleBarBeat(tickLastBar, tickLastBeat))
                {
                    ++tickLastBar;
                    tickLastBeat = 0;
                }
            }


            foreach (Note note in NoteManager.Instance.spawnedNotes)
            {
                if (note.noteType == NoteType.Effect && ((EffectNote)note).IsLongType)
                {
                    EffectNote effectNote = (EffectNote)note;
                    if (curBarBeat >= effectNote.seTickStartBarBeat &&
                        curBarBeat <= effectNote.seTickEndBarBeat &&
                        curBarBeat >= effectNote.seNextTickedBarBeat)
                    {
                        if (effectNote.pressed)
                            MusicManager.Instance.PlaySE(effectNote.seType);
                        effectNote.seNextTickedBarBeat += (float)effectNote.seTickBeatRate / (float)GlobalDefines.BeatPerBar;
                    }
                }
            }
        }

        public void JudgeNoteAtRail(int _railNumber, int _inputState)
        {
            Note targetNote = null;
            Note nearestNote = null;
            float targetTimeDiff = 999.0f;
            float nearestTimeDiff = 999.0f;
            foreach (Note note in NoteManager.Instance.spawnedNotes)
            {
                if (note.railNumber != _railNumber)
                    continue;

                float timeDiff = note.time - BeatManager.Instance.GameTime;

                if (timeDiff < nearestTimeDiff)
                {
                    nearestNote = note;
                    nearestTimeDiff = timeDiff;
                }

                if (note.noteType == NoteType.Mine)
                {
                    continue;
                }

                if ((note.noteType == NoteType.Long ||
                    (note.noteType == NoteType.Space && ((SpaceNote)note).IsLongType) ||
                    (note.noteType == NoteType.Effect && ((EffectNote)note).IsLongType))
                    && timeDiff <= -GlobalDefines.JudgePerfectTime && _inputState == InputManager.InputPress)
                {
                    LongNote longNote = (LongNote)note;
                    longNote.firstPressed = true;
                }

                if (note.noteType == NoteType.Pop)
                {
                    if (timeDiff < -GlobalDefines.JudgeNiceTime || timeDiff > GlobalDefines.JudgeNiceTime)
                        continue;
                }
                else
                {
                    if (timeDiff < -GlobalDefines.JudgeNiceTime || timeDiff > GlobalDefines.JudgeEarlyFailTime)
                        continue;
                }

                if (timeDiff < targetTimeDiff)
                {
                    targetNote = note;
                    targetTimeDiff = timeDiff;
                }
            }
            
            if (_inputState == InputManager.InputStay)
            {
                foreach (Note note in NoteManager.Instance.spawnedNotes)
                {
                    if (note.railNumber != _railNumber)
                        continue;
                    if (!(note.noteType == NoteType.Long ||
                         (note.noteType == NoteType.Space && ((SpaceNote)note).IsLongType) ||
                         (note.noteType == NoteType.Effect && ((EffectNote)note).IsLongType)))
                        continue;

                    LongNote longNote = (LongNote)note;
                    if (!longNote.firstPressed)
                        continue;

                    float barBeat = BeatManager.ToBarBeat(BeatManager.Instance.Bar, BeatManager.Instance.Beat);
                    float lnBarBeat = BeatManager.ToBarBeat(longNote.bar, longNote.beat);
                    // if (barBeat < longNote.tickStartBarBeat - GlobalDefines.JudgeNiceTime || barBeat > longNote.tickEndBarBeat)
                    if (barBeat < lnBarBeat - GlobalDefines.JudgeNiceTime || barBeat > longNote.tickEndBarBeat)
                        continue;

                    longNote.pressed = true;
                    longNote.lastPressedBarBeat = BeatManager.ToBarBeat(BeatManager.Instance.Bar, BeatManager.Instance.Beat);
                }
            }
            else if (_inputState == InputManager.InputPress)
            {
                if (targetNote != null && targetNote == nearestNote)
                {
                    if (targetNote.noteType == NoteType.Long ||
                       (targetNote.noteType == NoteType.Space && ((SpaceNote)targetNote).IsLongType) ||
                       (targetNote.noteType == NoteType.Effect && ((EffectNote)targetNote).IsLongType))
                    {
                        if (targetTimeDiff <= GlobalDefines.JudgeNiceTime)
                        {
                            LongNote targetLong = (LongNote)targetNote;
                            targetLong.pressed = true;
                            targetLong.firstPressed = true;
                            targetLong.lastPressedBarBeat = BeatManager.ToBarBeat(BeatManager.Instance.Bar, BeatManager.Instance.Beat);
                        }
                    }
                    else if (targetNote.noteType == NoteType.Pop)
                    {
                        NoteManager.Instance.DespawnNote(targetNote, false);
                        UpdateJudgeAndCombo(targetNote, 1, 0, false, false);
                    }
                    else
                    {
                        if (Mathf.Abs(targetTimeDiff) <= GlobalDefines.JudgePerfectTime)
                        {
                            NoteManager.Instance.DespawnNote(targetNote, false);
                            UpdateJudgeAndCombo(targetNote, 1, 0, false, false);
                        }
                        else if (Mathf.Abs(targetTimeDiff) <= GlobalDefines.JudgeNiceTime)
                        {
                            NoteManager.Instance.DespawnNote(targetNote, false);
                            UpdateJudgeAndCombo(targetNote, 2, (int)Mathf.Sign(targetTimeDiff), false, false);
                        }
                        else
                        {
                            NoteManager.Instance.DespawnNote(targetNote, true);
                            UpdateJudgeAndCombo(targetNote, 0, 0, false, false);
                        }
                    }
                }
            }
        }

        public void UpdateJudgeAndCombo(Note _note, int _judge, int _timingDir, bool _isLongTick = false, bool _isLongLastTick = false)
        {
            bool isLongTypeNote =
               (_note.noteType == NoteType.Long ||
               (_note.noteType == NoteType.Space && ((SpaceNote)_note).IsLongType) ||
               (_note.noteType == NoteType.Effect && ((EffectNote)_note).IsLongType));

            switch (_judge)
            {
                default:
                    break;
                case 0:
                    ++GameManager.Instance.judgeMissCount;
                    if (isLongTypeNote)
                        GameManager.Instance.ClearGauge += -clearGaugeDecreaseAmount / (float)(GlobalDefines.ClearGaugeLongNoteTickRatio);
                    else
                        GameManager.Instance.ClearGauge += -clearGaugeDecreaseAmount;
                    break;
                case 1:
                    ++GameManager.Instance.judgePerfectCount;
                    if (isLongTypeNote)
                        GameManager.Instance.ClearGauge += clearGaugeIncreaseAmount / (float)(GlobalDefines.ClearGaugeLongNoteTickRatio);
                    else
                        GameManager.Instance.ClearGauge += clearGaugeIncreaseAmount;
                    break;
                case 2:
                    ++GameManager.Instance.judgeNiceCount;
                    if (isLongTypeNote)
                        GameManager.Instance.ClearGauge += clearGaugeIncreaseAmount / (float)(GlobalDefines.ClearGaugeLongNoteTickRatio);
                    else
                        GameManager.Instance.ClearGauge += clearGaugeIncreaseAmount;
                    break;
            }

            if (_judge == 0)
            {
                GameManager.Instance.currentCombo = 0;
            }
            else
            {
                ++GameManager.Instance.currentCombo;
                if (_isLongTick)
                    MusicManager.Instance.PlayLongTick();
                else if (_note.noteType == NoteType.Effect)
                {
                    EffectNote effectNote = (EffectNote)_note;
                    if (effectNote.seType != EffectNoteSEType.None && !_isLongLastTick)
                        MusicManager.Instance.PlaySE(effectNote.seType);
                    else if (effectNote.seType == EffectNoteSEType.None)
                        MusicManager.Instance.PlayShot(_judge);
                }
                else
                    MusicManager.Instance.PlayShot(_judge);

                if (judgeEvent != null)
                    judgeEvent.Invoke();

                KeyBeamManager.Instance.SetKeyBeamState(
                    _note.railNumber,
                    (_judge == 1) ? KeyBeamManager.KeyBeamState.Perfect : KeyBeamManager.KeyBeamState.Great);
            }

            if (GameManager.Instance.currentCombo > GameManager.Instance.maxCombo)
                GameManager.Instance.maxCombo = GameManager.Instance.currentCombo;
            GameUI.Instance.uIJudgeImageAndCombo.Appear(_note.railNumber, _judge, _timingDir);

            // __TEST__
            /*
            if (_note.noteType == NoteType.Long ||
               (_note.noteType == NoteType.Space && ((SpaceNote)_note).IsLongType) ||
               (_note.noteType == NoteType.Effect && ((EffectNote)_note).IsLongType))
            {
                LongNote ln = (LongNote)_note;
                ln.pupa1 += 1;
            }
            */
        }
    }
}
