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
                    (note.noteType == NoteType.Space && ((SpaceNote)note).IsLongType))
                {
                    LongNote longNote = (LongNote)note;
                    float longReleaseBarBeat = BeatManager.Instance.CorrectBarBeat(
                        longNote.lastPressedBarBeat
                        + ((float)GlobalDefines.LongNoteReleaseTerm / (float)GlobalDefines.BeatPerBar)
                        );
                    if (longReleaseBarBeat < longNote.tickEndBarBeat && longReleaseBarBeat <= BeatManager.ToBarBeat(BeatManager.Instance.Bar, BeatManager.Instance.Beat))
                        longNote.pressed = false;

                    if (BeatManager.Instance.GameTime >= longNote.endTime)
                    {
                        if (longNote.pressed)
                        {
                            NoteManager.Instance.DespawnNote(note, false);
                            UpdateJudgeAndCombo(note.railNumber, 1);
                        }
                        else
                        {
                            NoteManager.Instance.DespawnNote(note, true);
                            UpdateJudgeAndCombo(note.railNumber, 0);
                        }
                        --i;
                    }
                }
                else if (BeatManager.Instance.GameTime >= note.time + GlobalDefines.JudgePerfectTime)
                {
                    NoteManager.Instance.DespawnNote(note, true);
                    UpdateJudgeAndCombo(note.railNumber, 0);
                    --i;
                }
            }
        }
        
        private void UpdateTickBeat()
        {
            float curBarBeat = BeatManager.ToBarBeat(BeatManager.Instance.Bar, BeatManager.Instance.Beat);
            if (curBarBeat >= BeatManager.ToBarBeat(tickLastBar, tickLastBeat))
            {
                foreach (Note note in NoteManager.Instance.spawnedNotes)
                {
                    if (note.noteType == NoteType.Long ||
                    (note.noteType == NoteType.Space && ((SpaceNote)note).IsLongType))
                    {
                        LongNote longNote = (LongNote)note;
                        if (curBarBeat >= longNote.tickStartBarBeat &&
                            curBarBeat <= longNote.tickEndBarBeat)
                        {
                            UpdateJudgeAndCombo(longNote.railNumber, (longNote.pressed) ? 1 : 0);
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
        }

        public void JudgeNoteAtLine(int _railNumber, int _inputState)
        {
            Note target = null;
            float targetTimeDiff = 999.0f;
            foreach (Note note in NoteManager.Instance.spawnedNotes)
            {
                if (note.railNumber != _railNumber)
                    continue;
                
                float timeDiff = note.time - BeatManager.Instance.GameTime;

                if ((note.noteType == NoteType.Long ||
                    (note.noteType == NoteType.Space && ((SpaceNote)note).IsLongType)) 
                    && timeDiff <= -GlobalDefines.JudgePerfectTime && _inputState == InputManager.InputPress)
                {
                    LongNote longNote = (LongNote)note;
                    longNote.firstPressed = true;
                }

                if (timeDiff < -GlobalDefines.JudgePerfectTime || timeDiff > GlobalDefines.JudgeEarlyFailTime)
                    continue;

                if (timeDiff < targetTimeDiff)
                {
                    target = note;
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
                        (note.noteType == NoteType.Space && ((SpaceNote)note).IsLongType)))
                        continue;

                    LongNote longNote = (LongNote)note;
                    if (!longNote.firstPressed)
                        continue;

                    float barBeat = BeatManager.ToBarBeat(BeatManager.Instance.Bar, BeatManager.Instance.Beat);
                    if (barBeat < longNote.tickStartBarBeat - GlobalDefines.JudgeNiceTime || barBeat > longNote.tickEndBarBeat)
                        continue;

                    longNote.pressed = true;
                    longNote.lastPressedBarBeat = BeatManager.ToBarBeat(BeatManager.Instance.Bar, BeatManager.Instance.Beat);
                }
            }
            else if (_inputState == InputManager.InputPress)
            {
                if (target != null)
                {
                    if (target.noteType == NoteType.Long ||
                        (target.noteType == NoteType.Space && ((SpaceNote)target).IsLongType))
                    {
                        if (targetTimeDiff <= GlobalDefines.JudgeNiceTime)
                        {
                            LongNote targetLong = (LongNote)target;
                            targetLong.pressed = true;
                            targetLong.firstPressed = true;
                            targetLong.lastPressedBarBeat = BeatManager.ToBarBeat(BeatManager.Instance.Bar, BeatManager.Instance.Beat);
                        }
                    }
                    else
                    {
                        if (targetTimeDiff <= GlobalDefines.JudgePerfectTime)
                        {
                            NoteManager.Instance.DespawnNote(target, false);
                            UpdateJudgeAndCombo(_railNumber, 1);
                        }
                        else if (targetTimeDiff <= GlobalDefines.JudgeNiceTime)
                        {
                            NoteManager.Instance.DespawnNote(target, false);
                            UpdateJudgeAndCombo(_railNumber, 2);
                        }
                        else
                        {
                            NoteManager.Instance.DespawnNote(target, true);
                            UpdateJudgeAndCombo(_railNumber, 0);
                        }
                    }
                }
            }
        }

        public void UpdateJudgeAndCombo(int _railNumber, int _judge)
        {
            if (_judge == 0)
            {
                if (GameManager.Instance.currentCombo > GameManager.Instance.maxCombo)
                    GameManager.Instance.maxCombo = GameManager.Instance.currentCombo;

                GameManager.Instance.currentCombo = 0;
            }
            else
            {
                ++GameManager.Instance.currentCombo;
                MusicManager.Instance.PlayShot(_judge);

                if (judgeEvent != null)
                    judgeEvent.Invoke();
            }

            GameUI.Instance.uIJudgeImageAndCombo.Appear(_railNumber, _judge);
        }
    }
}
