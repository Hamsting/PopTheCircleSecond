using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PopTheCircle.OldNoteDataConverter
{
    using PopTheCircle.NoteEditor;

    public static class ConvertedNoteFitter : System.Object
    {
        public static void FitNoteToGrid(Note _note)
        {
            float barBeat = (float)_note.bar + ((float)_note.beat / (float)GlobalDefines.BeatPerBar);
            float res = FitBarBeatToBeatGrid(barBeat, Is3MulBeat(_note.beat) ? 24 : 32);
            _note.bar = (int)res;
            _note.beat = (res - (float)_note.bar) * (float)GlobalDefines.BeatPerBar;
        }

        public static void FitLongNoteToGrid(LongNote _longNote)
        {
            float barBeat = (float)_longNote.bar + ((float)_longNote.beat / (float)GlobalDefines.BeatPerBar);
            float res = FitBarBeatToBeatGrid(barBeat, Is3MulBeat(_longNote.beat) ? 24 : 32);
            _longNote.bar = (int)res;
            _longNote.beat = (res - (float)_longNote.bar) * (float)GlobalDefines.BeatPerBar;

            float endBarBeat = (float)_longNote.endBar + ((float)_longNote.endBeat / (float)GlobalDefines.BeatPerBar);
            float endRes = FitBarBeatToBeatGrid(endBarBeat, Is3MulBeat(_longNote.endBeat) ? 24 : 32);
            _longNote.endBar = (int)endRes;
            _longNote.endBeat = (endRes - (float)_longNote.endBar) * (float)GlobalDefines.BeatPerBar;
        }

        private static bool Is3MulBeat(float _beat)
        {
            int beatInt = (int)_beat;
            if (beatInt % 8 == 0)
                return true;
            else
                return false;
        }

        private static float FitBarBeatToBeatGrid(float _barBeat, int _gridBeat)
        {
            int bar = (int)_barBeat;
            float beat = (_barBeat - (float)bar) * (float)GlobalDefines.BeatPerBar;
            int beatInt = (int)beat;

            int beatUnit = GlobalDefines.BeatPerBar / _gridBeat;
            int restBeat = beatInt % beatUnit;

            int resultBeat = beatInt - restBeat;
            if (restBeat >= beatUnit / 2)
                resultBeat += beatUnit;

            if (resultBeat >= GlobalDefines.BeatPerBar)
            {
                resultBeat -= GlobalDefines.BeatPerBar;
                bar += 1;
            }

            float resultBarBeat = (float)bar + ((float)resultBeat / (float)GlobalDefines.BeatPerBar);
            return resultBarBeat;
        }

    }
}