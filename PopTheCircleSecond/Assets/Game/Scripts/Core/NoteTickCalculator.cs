using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PopTheCircle.Game
{
    public static class NoteTickCalculator
    {
        public static int CalculateTotalTick(List<Note> _notes, List<BPMInfo> _bpms, out int _longNoteTotalTick)
        {
            _longNoteTotalTick = 0;

            if (_notes == null || _bpms == null ||
                _notes.Count == 0 || _bpms.Count == 0)
                return 0;

            int totalTick = 0;
            int longTick = 0;

            foreach (Note note in _notes)
            {
                switch (note.noteType)
                {
                    case NoteType.Normal:
                    case NoteType.Pop:
                    case NoteType.Mine:
                        totalTick += 1;
                        break;
                    case NoteType.Long:
                        {
                            longTick = GetLongTypeTotalTick((LongNote)note, _bpms);
                            totalTick += longTick;
                            _longNoteTotalTick += longTick;
                        }
                        break;
                    case NoteType.Space:
                        {
                            SpaceNote spaceNote = (SpaceNote)note;
                            if (spaceNote.IsLongType)
                            {
                                longTick = GetLongTypeTotalTick((LongNote)note, _bpms);
                                totalTick += longTick;
                                _longNoteTotalTick += longTick;
                            }
                            else
                                totalTick += 1;
                        } break;
                    case NoteType.Effect:
                        {
                            EffectNote effectNote = (EffectNote)note;
                            if (effectNote.IsLongType)
                            {
                                longTick = GetLongTypeTotalTick((LongNote)note, _bpms);
                                totalTick += longTick;
                                _longNoteTotalTick += longTick;
                            }
                            else
                                totalTick += 1;
                        } break;
                    default:
                        break;
                }
            }

            return totalTick;
        }

        private static int GetLongTypeTotalTick(LongNote _longNote, List<BPMInfo> _bpms)
        {
            int resTick = 2;

            // float tickBarBeat = (float)GlobalDefines.TickBeatRate / (float)GlobalDefines.BeatPerBar;
            // float tickStartBarBeat = BeatManager.Instance.CorrectBarBeat(longNoteBarBeat + tickBarBeat);
            // float tickEndBarBeat = BeatManager.Instance.CorrectBarBeat(longNoteEndBarBeat - tickBarBeat);

            BPMInfo targetBPM = _bpms[0];
            int targetBPMIndex = 0;
            for (int i = _bpms.Count - 1; i >= 0; --i)
            {
                BPMInfo info = _bpms[i];
                int infoStartBar = (info.beat == 0.0f) ? info.bar : info.bar + 1;
                if (infoStartBar <= (int)_longNote.tickStartBarBeat)
                {
                    targetBPM = info;
                    targetBPMIndex = i;
                    break;
                }
            }

            for (int i = targetBPMIndex + 1; i < _bpms.Count; ++i)
            {
                BPMInfo info = _bpms[i];
                float infoOriginBarBeat = BeatManager.ToBarBeat(info.bar, info.beat);
                if (infoOriginBarBeat <= _longNote.tickEndBarBeat)
                {
                    float tickStartBarBeat = (float)((targetBPM.beat == 0.0f) ? targetBPM.bar : targetBPM.bar + 1);
                    float tickBarBeat = CalculateActualTickBarBeat(info.bpm);
                    if (tickStartBarBeat < _longNote.tickStartBarBeat)
                    {
                        tickStartBarBeat = _longNote.tickStartBarBeat;
                        float term = (_longNote.tickStartBarBeat % tickBarBeat);
                        if (term > 0.005f)
                        {
                            tickStartBarBeat = _longNote.tickStartBarBeat - term + tickBarBeat;
                            if (tickStartBarBeat >= infoOriginBarBeat)
                                tickStartBarBeat = (float)((targetBPM.beat == 0.0f) ? targetBPM.bar : targetBPM.bar + 1);
                        }
                    }

                    float tickEndBarBeat = infoOriginBarBeat;
                    float barBeatDiff = tickEndBarBeat - tickStartBarBeat;
                    float divide = barBeatDiff / tickBarBeat;
                    int tickCount = Mathf.Clamp((int)divide + (divide >= 0.0f ? 1 : 0), 0, 9999999);
                    resTick += tickCount;

                    targetBPM = info;
                    continue;
                }
                else
                    break;
            }

            if (true)
            {
                float bpmInfoBarBeat = (float)((targetBPM.beat == 0.0f) ? targetBPM.bar : targetBPM.bar + 1);
                float bpmInfoOriginBarBeat = BeatManager.ToBarBeat(targetBPM.bar, targetBPM.beat);
                float tickStartBarBeat = bpmInfoBarBeat;
                float tickBarBeat = CalculateActualTickBarBeat(targetBPM.bpm);
                if (tickStartBarBeat < _longNote.tickStartBarBeat)
                {
                    tickStartBarBeat = _longNote.tickStartBarBeat;
                    float term = (_longNote.tickStartBarBeat % tickBarBeat);
                    if (term > 0.005f)
                        tickStartBarBeat = _longNote.tickStartBarBeat - term + tickBarBeat;
                }

                float tickEndBarBeat = _longNote.tickEndBarBeat;
                float barBeatDiff = tickEndBarBeat - tickStartBarBeat;
                float divide = barBeatDiff / tickBarBeat;
                int tickCount = Mathf.Clamp((int)divide + (divide >= 0.0f ? 1 : 0), 0, 9999999);
                resTick += tickCount;
            }

            // __TEST__
            // _longNote.pupa2 = resTick;

            return resTick;
        }

        private static float CalculateActualTickBarBeat(float _bpm)
        {
            int tickBeatRate = GlobalDefines.TickBeatRate;
            float bpm = _bpm;
            while (bpm >= GlobalDefines.TargetBPMForHalfTickBeatRate)
            {
                tickBeatRate *= 2;
                bpm *= 0.5f;
            }
            float tickBarBeat = (float)tickBeatRate / (float)GlobalDefines.BeatPerBar;
            return tickBarBeat;
        }

        private static float CorrectBarBeat(float _barBeat, List<BPMInfo> _bpms)
        {
            int bar = (int)_barBeat;
            float beat = (_barBeat - bar) * GlobalDefines.BeatPerBar;

            foreach (BPMInfo info in _bpms)
            {
                if (info.bar > bar)
                    return _barBeat;
                else if (info.bar != bar)
                    continue;

                if (info.beat != 0.0f && info.beat <= beat)
                {
                    // 임시 작성, 계산식 수정 필요 (bpm 변경에 따른 보정)
                    float modifiedBeat = beat - info.beat;

                    float corrected = (float)bar + 1.0f + (modifiedBeat / GlobalDefines.BeatPerBar);
                    return corrected;
                }
            }
            return _barBeat;
        }
    }
}