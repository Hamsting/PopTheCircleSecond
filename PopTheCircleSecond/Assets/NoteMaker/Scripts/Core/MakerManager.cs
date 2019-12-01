using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace PopTheCircle.NoteEditor
{
    public enum EditMode
    {
        PositionBar,
        AddNote,
        RemoveNote
    }

    public enum EditType
    {
        NormalNote,
        DoubleNote,
        DragNote,
        LongNote,
        InfinityNote,
        BPMChangeNote,
        CTChangeNote,
        CameraNote,
        EventNote,
    }

    public class MakerManager : Singleton<MakerManager>
    {
        public GameObject longTypePreview;
        [InspectorReadOnly]
        public string noteDataFilePath = "";
        [InspectorReadOnly]
        public NoteData noteData = new NoteData();
        [InspectorReadOnly]
        public EditMode editMode = EditMode.PositionBar;
        [InspectorReadOnly]
        public EditType editType = EditType.NormalNote;
        [InspectorReadOnly]
        public int barGrid = 4;

        private bool isPlacingLongTypeNote = false;
        private float longTypeStartBarBeat = 0.0f;
        private int longTypeRailNumber = 0;



        protected override void Awake()
        {
            noteDataFilePath = "Untitled.ntd";
        }

        public void LeftClickField(Vector3 _worldPos)
        {
            Vector3 railLocalPos = NoteRailManager.Instance.railRoot.InverseTransformPoint(_worldPos);
            railLocalPos.y -= NoteRail.RailHeight * 0.5f;
            float railTotalHeight = NoteRail.RailHeight + NoteRailManager.Instance.railSpacing;
            bool isPressedInRail = (-railLocalPos.y >= 0.0f) && 
                                   (-railLocalPos.y % railTotalHeight) <= NoteRail.RailHeight;
            if (!isPressedInRail)
                return;

            int railNum = (int)(-railLocalPos.y / railTotalHeight) + 1;
            NoteRailLength rl = BeatManager.Instance.GetNoteRailLengthWithRailNumber(railNum);
            bool isPressedInBar = (railLocalPos.x >= 0.0f) && 
                                  (railLocalPos.x <= rl.barBeatLength * NoteRail.RailOneBarWidth);
            if (!isPressedInBar)
                return;

            float pressedLocalBarBeat = railLocalPos.x / NoteRail.RailOneBarWidth;
            float gridedLocalBarBeat = pressedLocalBarBeat - (pressedLocalBarBeat % (4.0f / (float)barGrid));
            float resultBarBeat = rl.startBarBeat + gridedLocalBarBeat;
            int bar = (int)resultBarBeat;
            float beat = (resultBarBeat - bar) * (float)GlobalDefines.BeatPerBar;
            int railNumber = (int)((-railLocalPos.y % railTotalHeight) / (NoteRail.RailHeight * 0.3f));

            if (editMode == EditMode.PositionBar)
            {
                BeatManager.Instance.GotoBarBeat(bar, beat);
                MusicManager.Instance.MusicPosition = BeatManager.Instance.GameTime;
                // Debug.Log(
                // "R : " + railNum +
                // ", P : " + pressedLocalBarBeat +
                // ", G : " + gridedLocalBarBeat +
                // ", R : " + resultBarBeat +
                // ", B : " + beat);
            }
            else if (editMode == EditMode.AddNote)
            {
                switch (editType)
                {
                    case EditType.NormalNote:
                        {
                            if (railNumber >= 2)
                                break;
                            if (NoteManager.Instance.FindNote(bar, beat, railNumber) != null)
                                break;
                            NormalNote normalNote = new NormalNote()
                            {
                                bar = bar,
                                beat = beat,
                                railNumber = railNumber
                            };
                            NoteManager.Instance.AddNote(normalNote);
                        }
                        break;
                    case EditType.DoubleNote:
                        {
                            if (railNumber >= 2)
                                break;
                            if (NoteManager.Instance.FindNote(bar, beat, railNumber) != null)
                                break;
                            DoubleNote doubleNote = new DoubleNote()
                            {
                                bar = bar,
                                beat = beat,
                                railNumber = railNumber
                            };
                            NoteManager.Instance.AddNote(doubleNote);
                        }
                        break;
                    case EditType.DragNote:
                        {
                            if (railNumber >= 2)
                                break;
                            Note note = NoteManager.Instance.FindNote(bar, beat, railNumber);
                            if (note != null)
                            {
                                DragNote dragNote = (DragNote)note;
                                dragNote.direction = 1 - dragNote.direction;
                                NoteManager.Instance.UpdateNoteSpawn();
                            }
                            else
                            {
                                DragNote dragNote = new DragNote()
                                {
                                    bar = bar,
                                    beat = beat,
                                    railNumber = railNumber,
                                    direction = 0
                                };
                                NoteManager.Instance.AddNote(dragNote);
                            }
                        }
                        break;
                    case EditType.LongNote:
                        {
                            if (railNumber >= 2)
                                break;
                            if (NoteManager.Instance.FindLongTypeNote(bar, beat, railNumber) != null)
                                break;

                            if (isPlacingLongTypeNote)
                            {
                                if (longTypeStartBarBeat != resultBarBeat)
                                {
                                    LongNote longNote = new LongNote()
                                    {
                                        bar = (int)longTypeStartBarBeat,
                                        beat = (longTypeStartBarBeat - (int)longTypeStartBarBeat) * GlobalDefines.BeatPerBar,
                                        endBar = bar,
                                        endBeat = beat,
                                        railNumber = longTypeRailNumber
                                    };
                                    NoteManager.Instance.AddNote(longNote);
                                }
                                HideLongTypePreview();
                                isPlacingLongTypeNote = false;
                            }
                            else
                            {
                                longTypeStartBarBeat = resultBarBeat;
                                longTypeRailNumber = railNumber;
                                ShowLongTypePreview(resultBarBeat, railNumber);
                                isPlacingLongTypeNote = true;
                            }
                        }
                        break;
                    case EditType.InfinityNote:
                        {
                            if (railNumber >= 2)
                                break;

                            Note existed = NoteManager.Instance.FindLongTypeNote(bar, beat, railNumber);
                            if (existed != null)
                            {
                                if (existed.GetType() == typeof(InfinityNote) && !isPlacingLongTypeNote)
                                    MakerUIManager.Instance.popup.OpenInfinityCountChangePopup((InfinityNote)existed);
                                break;
                            }

                            if (isPlacingLongTypeNote)
                            {
                                if (longTypeStartBarBeat != resultBarBeat)
                                {
                                    InfinityNote infinityNote = new InfinityNote()
                                    {
                                        bar = (int)longTypeStartBarBeat,
                                        beat = (longTypeStartBarBeat - (int)longTypeStartBarBeat) * GlobalDefines.BeatPerBar,
                                        endBar = bar,
                                        endBeat = beat,
                                        railNumber = longTypeRailNumber
                                    };
                                    NoteManager.Instance.AddNote(infinityNote);
                                    MakerUIManager.Instance.popup.OpenInfinityCountChangePopup(infinityNote);
                                }
                                HideLongTypePreview();
                                isPlacingLongTypeNote = false;
                            }
                            else
                            {
                                longTypeStartBarBeat = resultBarBeat;
                                longTypeRailNumber = railNumber;
                                ShowLongTypePreview(resultBarBeat, railNumber);
                                isPlacingLongTypeNote = true;
                            }
                        }
                        break;
                    case EditType.BPMChangeNote:
                        MakerUIManager.Instance.popup.OpenBPMChangePopup(bar, beat);
                        break;
                    case EditType.CTChangeNote:
                        MakerUIManager.Instance.popup.OpenCTChangePopup(bar);
                        break;
                    case EditType.CameraNote:
                        {
                            if (railNumber < 2)
                                break;
                            Note existed = NoteManager.Instance.FindEffectNote(bar, beat, typeof(CameraNote));
                            if (existed != null)
                            {
                                // WIP
                            }
                            else
                            {
                                CameraNote cameraNote = new CameraNote()
                                {
                                    bar = bar,
                                    beat = beat,
                                    railNumber = 0,
                                };
                                NoteManager.Instance.AddNote(cameraNote);
                                // WIP
                            }
                        }
                        break;
                    case EditType.EventNote:
                        MakerUIManager.Instance.popup.OpenEventNotePopup(bar, beat);
                        break;
                    default:
                        break;
                }
            }
            else if (editMode == EditMode.RemoveNote)
            {
                switch (editType)
                {
                    case EditType.NormalNote:
                        {
                            if (railNumber >= 2)
                                break;
                            Note note = NoteManager.Instance.FindNote(bar, beat, railNumber);
                            if (note != null && note.GetType() == typeof(NormalNote))
                                NoteManager.Instance.RemoveNote(note);
                        }
                        break;
                    case EditType.DoubleNote:
                        {
                            if (railNumber >= 2)
                                break;
                            Note note = NoteManager.Instance.FindNote(bar, beat, railNumber);
                            if (note != null && note.GetType() == typeof(DoubleNote))
                                NoteManager.Instance.RemoveNote(note);
                        }
                        break;
                    case EditType.DragNote:
                        {
                            if (railNumber >= 2)
                                break;
                            Note note = NoteManager.Instance.FindNote(bar, beat, railNumber);
                            if (note != null && note.GetType() == typeof(DragNote))
                                NoteManager.Instance.RemoveNote(note);
                        }
                        break;
                    case EditType.LongNote:
                        {
                            if (railNumber >= 2)
                                break;
                            Note note = NoteManager.Instance.FindLongTypeNote(bar, beat, railNumber);
                            if (note != null && note.GetType() == typeof(LongNote))
                                NoteManager.Instance.RemoveNote(note);
                        }
                        break;
                    case EditType.InfinityNote:
                        {
                            if (railNumber >= 2)
                                break;
                            Note note = NoteManager.Instance.FindLongTypeNote(bar, beat, railNumber);
                            if (note != null && note.GetType() == typeof(InfinityNote))
                                NoteManager.Instance.RemoveNote(note);
                        }
                        break;
                    case EditType.BPMChangeNote:
                        {
                            int index = BeatManager.Instance.BPMInfos.FindIndex(
                                (bi) => (
                                    BeatManager.Instance.CorrectBarBeat(BeatManager.ToBarBeat(bi.bar, bi.beat))
                                     == resultBarBeat
                                ));
                            if (index > 0)
                            {
                                BeatManager.Instance.BPMInfos.RemoveAt(index);
                                BeatManager.Instance.UpdateBPMInfo();
                                NoteManager.Instance.FixIncorrectBarBeatNotes();
                                BeatManager.Instance.UpdateRailLengths();
                                NoteRailManager.Instance.UpdateRailSpawnImmediately();
                                BeatManager.Instance.GotoTime(BeatManager.Instance.GameTime);
                            }
                        }
                        break;
                    case EditType.CTChangeNote:
                        {
                            int index = BeatManager.Instance.ctInfos.FindIndex((ct) => (ct.bar == bar));
                            if (index > 0)
                            {
                                BeatManager.Instance.ctInfos.RemoveAt(index);
                                NoteManager.Instance.FixIncorrectBarBeatNotes();
                                BeatManager.Instance.UpdateRailLengths();
                                NoteRailManager.Instance.UpdateRailSpawnImmediately();
                                BeatManager.Instance.GotoTime(BeatManager.Instance.GameTime);
                            }
                        }
                        break;
                    case EditType.CameraNote:
                        {
                            if (railNumber < 2)
                                break;
                            Note note = NoteManager.Instance.FindEffectNote(bar, beat, typeof(CameraNote));
                            if (note != null)
                                NoteManager.Instance.RemoveNote(note);
                        }
                        break;
                    case EditType.EventNote:
                        {
                            Note note = NoteManager.Instance.FindEffectNote(bar, beat, typeof(EventNote));
                            if (note != null)
                                NoteManager.Instance.RemoveNote(note);
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        public void RightClickField(Vector3 _worldPos)
        {
            Vector3 railLocalPos = NoteRailManager.Instance.railRoot.InverseTransformPoint(_worldPos);
            railLocalPos.y -= NoteRail.RailHeight * 0.5f;
            float railTotalHeight = NoteRail.RailHeight + NoteRailManager.Instance.railSpacing;
            bool isPressedInRail = (-railLocalPos.y >= 0.0f) &&
                                   (-railLocalPos.y % railTotalHeight) <= NoteRail.RailHeight;
            if (!isPressedInRail)
                return;

            int railNum = (int)(-railLocalPos.y / railTotalHeight) + 1;
            NoteRailLength rl = BeatManager.Instance.GetNoteRailLengthWithRailNumber(railNum);
            bool isPressedInBar = (railLocalPos.x >= 0.0f) &&
                                  (railLocalPos.x <= rl.barBeatLength * NoteRail.RailOneBarWidth);
            if (!isPressedInBar)
                return;

            float pressedLocalBarBeat = railLocalPos.x / NoteRail.RailOneBarWidth;
            float gridedLocalBarBeat = pressedLocalBarBeat - (pressedLocalBarBeat % (4.0f / (float)barGrid));
            float resultBarBeat = rl.startBarBeat + gridedLocalBarBeat;
            int bar = (int)resultBarBeat;
            float beat = (resultBarBeat - (int)resultBarBeat) * (float)GlobalDefines.BeatPerBar;
            int railNumber = (int)((-railLocalPos.y % railTotalHeight) / (NoteRail.RailHeight * 0.3f));

            switch (editType)
            {
                case EditType.NormalNote:
                    {
                        if (railNumber >= 2)
                            break;
                        Note note = NoteManager.Instance.FindNote(bar, beat, railNumber);
                        if (note != null && note.GetType() == typeof(NormalNote))
                            NoteManager.Instance.RemoveNote(note);
                    }
                    break;
                case EditType.DoubleNote:
                    {
                        if (railNumber >= 2)
                            break;
                        Note note = NoteManager.Instance.FindNote(bar, beat, railNumber);
                        if (note != null && note.GetType() == typeof(DoubleNote))
                            NoteManager.Instance.RemoveNote(note);
                    }
                    break;
                case EditType.DragNote:
                    {
                        if (railNumber >= 2)
                            break;
                        Note note = NoteManager.Instance.FindNote(bar, beat, railNumber);
                        if (note != null && note.GetType() == typeof(DragNote))
                            NoteManager.Instance.RemoveNote(note);
                    }
                    break;
                case EditType.LongNote:
                    {
                        if (railNumber >= 2)
                            break;
                        Note note = NoteManager.Instance.FindLongTypeNote(bar, beat, railNumber);
                        if (note != null && note.GetType() == typeof(LongNote))
                            NoteManager.Instance.RemoveNote(note);
                    }
                    break;
                case EditType.InfinityNote:
                    {
                        if (railNumber >= 2)
                            break;
                        Note note = NoteManager.Instance.FindLongTypeNote(bar, beat, railNumber);
                        if (note != null && note.GetType() == typeof(InfinityNote))
                            NoteManager.Instance.RemoveNote(note);
                    }
                    break;
                case EditType.BPMChangeNote:
                    {
                        int index = BeatManager.Instance.BPMInfos.FindIndex(
                            (bi) => (
                            BeatManager.Instance.CorrectBarBeat(BeatManager.ToBarBeat(bi.bar, bi.beat))
                             == resultBarBeat
                            ));
                        if (index > 0)
                        {
                            BeatManager.Instance.BPMInfos.RemoveAt(index);
                            BeatManager.Instance.UpdateBPMInfo();
                            NoteManager.Instance.FixIncorrectBarBeatNotes();
                            BeatManager.Instance.UpdateRailLengths();
                            NoteRailManager.Instance.UpdateRailSpawnImmediately();
                            BeatManager.Instance.GotoTime(BeatManager.Instance.GameTime);
                        }
                    }
                    break;
                case EditType.CTChangeNote:
                    {
                        int index = BeatManager.Instance.ctInfos.FindIndex((ct) => (ct.bar == bar));
                        if (index > 0)
                        {
                            BeatManager.Instance.ctInfos.RemoveAt(index);
                            NoteManager.Instance.FixIncorrectBarBeatNotes();
                            BeatManager.Instance.UpdateRailLengths();
                            NoteRailManager.Instance.UpdateRailSpawnImmediately();
                            BeatManager.Instance.GotoTime(BeatManager.Instance.GameTime);
                        }
                    }
                    break;
                case EditType.CameraNote:
                    {
                        if (railNumber < 2)
                            break;
                        Note note = NoteManager.Instance.FindEffectNote(bar, beat, typeof(CameraNote));
                        if (note != null)
                            NoteManager.Instance.RemoveNote(note);
                    }
                    break;
                case EditType.EventNote:
                    {
                        Note note = NoteManager.Instance.FindEffectNote(bar, beat, typeof(EventNote));
                        if (note != null)
                            NoteManager.Instance.RemoveNote(note);
                    }
                    break;
                default:
                    break;
            }
        }

        public void MiddleClickField(Vector3 _worldPos)
        {
            Vector3 railLocalPos = NoteRailManager.Instance.railRoot.InverseTransformPoint(_worldPos);
            railLocalPos.y -= NoteRail.RailHeight * 0.5f;
            float railTotalHeight = NoteRail.RailHeight + NoteRailManager.Instance.railSpacing;
            bool isPressedInRail = (-railLocalPos.y >= 0.0f) &&
                                   (-railLocalPos.y % railTotalHeight) <= NoteRail.RailHeight;
            if (!isPressedInRail)
                return;

            int railNum = (int)(-railLocalPos.y / railTotalHeight) + 1;
            NoteRailLength rl = BeatManager.Instance.GetNoteRailLengthWithRailNumber(railNum);
            bool isPressedInBar = (railLocalPos.x >= 0.0f) &&
                                  (railLocalPos.x <= rl.barBeatLength * NoteRail.RailOneBarWidth);
            if (!isPressedInBar)
                return;

            float pressedLocalBarBeat = railLocalPos.x / NoteRail.RailOneBarWidth;
            float gridedLocalBarBeat = pressedLocalBarBeat - (pressedLocalBarBeat % (4.0f / (float)barGrid));
            float resultBarBeat = rl.startBarBeat + gridedLocalBarBeat;
            float beat = (resultBarBeat - (int)resultBarBeat) * (float)GlobalDefines.BeatPerBar;
            
            BeatManager.Instance.GotoBarBeat((int)resultBarBeat, beat);
            MusicManager.Instance.MusicPosition = BeatManager.Instance.GameTime;
        }

        private void ShowLongTypePreview(float _barBeat, float _railNumber)
        {
            longTypePreview.SetActive(true);

            NoteRailLength rl = BeatManager.Instance.GetNoteRailLengthWithBarBeat(_barBeat);
            float localBarBeatDiff = _barBeat - rl.startBarBeat;
            float railTotalHeight = NoteRail.RailHeight + NoteRailManager.Instance.railSpacing;

            Vector3 pos = Vector3.zero;
            pos.x = localBarBeatDiff * NoteRail.RailOneBarWidth;
            pos.y = (rl.railNumber - 1) * -railTotalHeight;
            if (_railNumber == 0)
                pos.y += NoteRail.RailHeight * 0.333333f;
            pos.z = -0.5f;

            longTypePreview.transform.localPosition = pos;
        }
        
        private void HideLongTypePreview()
        {
            longTypePreview.SetActive(false);
        }
        
        public void LoadNoteData(string _path)
        {
            NoteDataJSONConverter.Instance.JSONToNoteData(SaveLoad.LoadNoteDataJSON(_path));

            string lastDirectory = _path;
            int i = _path.LastIndexOf('\\');
            if (i < 0)
                i = _path.LastIndexOf('/');
            if (i > 0)
                lastDirectory = _path.Substring(0, i);

            PlayerPrefs.SetString("LastDirectory", lastDirectory);
            PlayerPrefs.Save();

            noteDataFilePath = _path;
            if (MakerUIManager.Instance.optionMenu.gameObject.activeSelf)
                MakerUIManager.Instance.ToggleOptionMenu();
            MakerUIManager.Instance.UpdateDefaultMenuUI();
        }

        public void SaveNoteData(string _path)
        {
            SaveLoad.SaveNoteDataJSON(_path, NoteDataJSONConverter.Instance.NoteDataToJSON());

            string lastDirectory = _path;
            int i = _path.LastIndexOf('\\');
            if (i < 0)
                i = _path.LastIndexOf('/');
            if (i > 0)
                lastDirectory = _path.Substring(0, i);

            PlayerPrefs.SetString("LastDirectory", lastDirectory);
            PlayerPrefs.Save();

            noteDataFilePath = _path;
            if (MakerUIManager.Instance.optionMenu.gameObject.activeSelf)
                MakerUIManager.Instance.ToggleOptionMenu();
            MakerUIManager.Instance.UpdateDefaultMenuUI();
        }
    }
}