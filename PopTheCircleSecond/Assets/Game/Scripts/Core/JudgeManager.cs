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
        public UIJudgeImage uiJudgeImage;

        public UnityEvent judgeEvent;

        [SerializeField, InspectorReadOnly]
        private int tickLastBar = 0;
        [SerializeField, InspectorReadOnly]
        private float tickLastBeat = 0;
        private int[] touchCountAtLines;



        protected override void Awake()
        {
            base.Awake();

            touchCountAtLines = new int[2];
            for (int i = 0; i < touchCountAtLines.Length; ++i)
            {
                touchCountAtLines[i] = 0;
            }
        }

        public void UpdateJudgeForNotes()
        {
            UpdateTickBeat();

            for (int i = 0; i < NoteManager.Instance.spawnedNotes.Count; ++i)
            {
                Note note = NoteManager.Instance.spawnedNotes[i];

                if (note.GetType() == typeof(LongNote))
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
                            UpdateComboAndJudgeImage(note.railNumber, 1);
                        }
                        else
                        {
                            NoteManager.Instance.DespawnNote(note, true);
                            UpdateComboAndJudgeImage(note.railNumber, 0);
                        }
                        --i;
                    }
                }
                else if (note.GetType() == typeof(InfinityNote))
                {
                    InfinityNote infinityNote = (InfinityNote)note;
                    if (!infinityNote.canPress && BeatManager.Instance.GameTime >= infinityNote.time)
                        infinityNote.canPress = true;

                    if (BeatManager.Instance.GameTime >= infinityNote.endTime)
                    {
                        if (infinityNote.currentHitCount >= infinityNote.maxHitCount)
                        {
                            NoteManager.Instance.DespawnNote(note, false);
                            UpdateComboAndJudgeImage(0, 1);
                            UpdateJudgeImage(1, 1);
                        }
                        else
                        {
                            NoteManager.Instance.DespawnNote(note, true);
                            UpdateComboAndJudgeImage(0, 0);
                            UpdateJudgeImage(1, 0);
                        }
                        --i;
                    }
                }
                else if (BeatManager.Instance.GameTime >= note.time + GlobalDefines.JudgePerfectTime)
                {
                    NoteManager.Instance.DespawnNote(note, true);
                    UpdateComboAndJudgeImage(note.railNumber, 0);
                    --i;
                }
            }
        }

        private void LateUpdate()
        {
            for (int j = 0; j < touchCountAtLines.Length; ++j)
            {
                touchCountAtLines[j] = 0;
            }
        }

        private void UpdateTickBeat()
        {
            if (BeatManager.Instance.Bar >= tickLastBar && BeatManager.Instance.Beat >= tickLastBeat)
            {
                float curBarBeat = BeatManager.ToBarBeat(BeatManager.Instance.Bar, BeatManager.Instance.Beat);
                foreach (Note note in NoteManager.Instance.spawnedNotes)
                {
                    if (note.GetType() == typeof(LongNote))
                    {
                        LongNote longNote = (LongNote)note;
                        if (curBarBeat >= longNote.tickStartBarBeat &&
                            curBarBeat <= longNote.tickEndBarBeat)
                        {
                            UpdateComboAndJudgeImage(longNote.railNumber, (longNote.pressed) ? 1 : 0);
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
            if (_inputState == InputManager.InputPress || _inputState == InputManager.InputStay)
            {
                ++touchCountAtLines[_railNumber];
            }

            Note target = null;
            float targetTimeDiff = 999.0f;
            foreach (Note note in NoteManager.Instance.spawnedNotes)
            {
                if (note.GetType() == typeof(InfinityNote))
                {
                    InfinityNote targetInfinity = (InfinityNote)note;
                    if (_inputState != InputManager.InputPress ||
                        !targetInfinity.canPress)
                        continue;

                    ++targetInfinity.currentHitCount;
                    if (targetInfinity.currentHitCount <= targetInfinity.maxHitCount)
                        UpdateComboAndJudgeImage(_railNumber, 1);
                    else
                        UpdateJudgeImage(_railNumber, 1);

                    continue;
                }

                if (note.railNumber != _railNumber)
                    continue;

                float timeDiff = note.time - BeatManager.Instance.GameTime;
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
                    if (note.GetType() != typeof(LongNote))
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
            else if (_inputState == InputManager.InputDrag)
            {
                if (target != null && target.GetType() == typeof(DragNote))
                {
                    if (targetTimeDiff <= GlobalDefines.JudgePerfectTime)
                    {
                        NoteManager.Instance.DespawnNote(target, false);
                        UpdateComboAndJudgeImage(_railNumber, 1);
                    }
                    else if (targetTimeDiff <= GlobalDefines.JudgeNiceTime)
                    {
                        NoteManager.Instance.DespawnNote(target, false);
                        UpdateComboAndJudgeImage(_railNumber, 2);
                    }
                    else
                    {
                        NoteManager.Instance.DespawnNote(target, true);
                        UpdateComboAndJudgeImage(_railNumber, 0);
                    }
                }
            }
            else if (_inputState == InputManager.InputPress)
            {
                if (target != null && target.GetType() != typeof(DragNote))
                {
                    if (target.GetType() == typeof(LongNote))
                    {
                        Debug.Log(targetTimeDiff);
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
                        if (target.GetType() == typeof(DoubleNote) && touchCountAtLines[_railNumber] < 2)
                        {

                        }
                        else
                        {
                            if (targetTimeDiff <= GlobalDefines.JudgePerfectTime)
                            {
                                NoteManager.Instance.DespawnNote(target, false);
                                UpdateComboAndJudgeImage(_railNumber, 1);
                            }
                            else if (targetTimeDiff <= GlobalDefines.JudgeNiceTime)
                            {
                                NoteManager.Instance.DespawnNote(target, false);
                                UpdateComboAndJudgeImage(_railNumber, 2);
                            }
                            else
                            {
                                NoteManager.Instance.DespawnNote(target, true);
                                UpdateComboAndJudgeImage(_railNumber, 0);
                            }
                        }
                    }
                }
            }
        }

        public void UpdateComboAndJudgeImage(int _railNumber, int _judge)
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
                if (GameManager.Instance.currentCombo >= 100)
                    ++GameManager.Instance.crazyCombo;
                else if (GameManager.Instance.currentCombo >= 50)
                    ++GameManager.Instance.amazingCombo;

                if (judgeEvent != null)
                    judgeEvent.Invoke();
            }

            uiJudgeImage.AppearJudgeImage(_railNumber, _judge);
        }

        public void UpdateJudgeImage(int _railNumber, int _judge)
        {
            uiJudgeImage.AppearJudgeImage(_railNumber, _judge);
        }
    }
}
