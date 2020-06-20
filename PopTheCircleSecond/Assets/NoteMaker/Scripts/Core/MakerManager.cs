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
        PopNote,
        MineNote,
        LongNote,
        SpaceNote,
        EffectNote,
        BPMChangeNote,
        CTChangeNote,
        CameraNote,
        EventNote,
        TickNote,
    }

    public class MakerManager : Singleton<MakerManager>
    {
        public static readonly List<int> BarGrids    = new List<int>() {  4,  8, 12, 16, 24, 32, 48, 64, 96 };
        public static readonly List<int> SETickRates = new List<int>() {  1,  2,  3,  4,  6,  8, 12, 16, 24 };

        public GameObject longTypePreview;
        public Sprite longNotePreviewSprite;
        public Sprite effectNotePreviewSprite;
        public Sprite spaceNotePreviewSprite;
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
        private SpriteRenderer notePreviewRenderer;



        protected override void Awake()
        {
            noteDataFilePath = "Untitled.ntd";

            notePreviewRenderer = longTypePreview.GetComponent<SpriteRenderer>();
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
            // int railNumber = (int)((-railLocalPos.y % railTotalHeight) / (NoteRail.RailHeight * 0.3f));
            int railNumber = NoteRailManager.Instance.RailYPosToRailNumber(-railLocalPos.y % railTotalHeight);
            // Debug.Log(-railLocalPos.y % railTotalHeight + " => Line : " + railNumber);


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
                            if (railNumber > 3)
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
                    case EditType.PopNote:
                        {
                            if (railNumber > 3)
                                break;
                            if (NoteManager.Instance.FindNote(bar, beat, railNumber) != null)
                                break;
                            PopNote popNote = new PopNote()
                            {
                                bar = bar,
                                beat = beat,
                                railNumber = railNumber
                            };
                            NoteManager.Instance.AddNote(popNote);
                        }
                        break;
                    case EditType.MineNote:
                        {
                            if (railNumber > 3)
                                break;
                            if (NoteManager.Instance.FindNote(bar, beat, railNumber) != null)
                                break;
                            MineNote mineNote = new MineNote()
                            {
                                bar = bar,
                                beat = beat,
                                railNumber = railNumber
                            };
                            NoteManager.Instance.AddNote(mineNote);
                        }
                        break;
                    case EditType.LongNote:
                        {
                            if (railNumber > 3)
                                break;
                            if (NoteManager.Instance.FindLongTypeNote(bar, beat, railNumber) != null)
                                break;

                            if (isPlacingLongTypeNote)
                            {
                                if (longTypeStartBarBeat < resultBarBeat)
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
                    case EditType.SpaceNote:
                        {
                            if (railNumber > 4)
                                break;
                            if (NoteManager.Instance.FindLongTypeNote(bar, beat, 4) != null)
                                break;

                            if (isPlacingLongTypeNote)
                            {
                                if (longTypeStartBarBeat <= resultBarBeat)
                                {
                                    SpaceNote spaceNote = new SpaceNote()
                                    {
                                        bar = (int)longTypeStartBarBeat,
                                        beat = (longTypeStartBarBeat - (int)longTypeStartBarBeat) * GlobalDefines.BeatPerBar,
                                        endBar = (longTypeStartBarBeat < resultBarBeat) ? bar : 0,
                                        endBeat = (longTypeStartBarBeat < resultBarBeat) ? beat : 0.0f,
                                        railNumber = 4
                                    };
                                    NoteManager.Instance.AddNote(spaceNote);
                                }
                                HideLongTypePreview();
                                isPlacingLongTypeNote = false;
                            }
                            else
                            {
                                longTypeStartBarBeat = resultBarBeat;
                                longTypeRailNumber = 4;
                                ShowLongTypePreview(resultBarBeat, 4);
                                isPlacingLongTypeNote = true;
                            }
                        }
                        break;
                    case EditType.EffectNote:
                        {
                            if (railNumber < 5 || railNumber > 6)
                                break;
                            Note note = NoteManager.Instance.FindLongTypeNote(bar, beat, railNumber);
                            if (note != null)
                            {
                                if (note.GetType() == typeof(EffectNote))
                                    MakerUIManager.Instance.popup.OpenEffectNotePopup((EffectNote)note);
                                break;
                            }
                            note = NoteManager.Instance.FindNote(bar, beat, railNumber, true);
                            if (note != null)
                            {
                                if (note.GetType() == typeof(EffectNote))
                                    MakerUIManager.Instance.popup.OpenEffectNotePopup((EffectNote)note);
                                break;
                            }

                            if (isPlacingLongTypeNote)
                            {
                                if (longTypeStartBarBeat <= resultBarBeat)
                                {
                                    EffectNote effectNote = new EffectNote()
                                    {
                                        bar = (int)longTypeStartBarBeat,
                                        beat = (longTypeStartBarBeat - (int)longTypeStartBarBeat) * GlobalDefines.BeatPerBar,
                                        endBar = (longTypeStartBarBeat < resultBarBeat) ? bar : 0,
                                        endBeat = (longTypeStartBarBeat < resultBarBeat) ? beat : 0.0f,
                                        railNumber = longTypeRailNumber,
                                        seType = EffectNoteSEType.None,
                                        seTickBeatRate = GlobalDefines.BeatPerBar / 4,
                                    };
                                    NoteManager.Instance.AddNote(effectNote);
                                    MakerUIManager.Instance.popup.OpenEffectNotePopup(effectNote);
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
                        {
                            MakerUIManager.Instance.popup.OpenBPMChangePopup(bar, beat);
                        }
                        break;
                    case EditType.CTChangeNote:
                        {
                            MakerUIManager.Instance.popup.OpenCTChangePopup(bar);
                        }
                        break;
                    case EditType.CameraNote:
                        {
                            if (railNumber != 8)
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
                                    railNumber = railNumber,
                                };
                                NoteManager.Instance.AddNote(cameraNote);
                                // WIP
                            }
                        }
                        break;
                    case EditType.EventNote:
                        {
                            MakerUIManager.Instance.popup.OpenEventNotePopup(bar, beat);
                        }
                        break;
                    case EditType.TickNote:
                        {
                            if (railNumber != 7)
                                break;
                            if (NoteManager.Instance.FindEffectNote(bar, beat, typeof(TickNote)) != null)
                                break;
                            TickNote tickNote = new TickNote()
                            {
                                bar = bar,
                                beat = beat,
                                railNumber = railNumber
                            };
                            NoteManager.Instance.AddNote(tickNote);
                        }
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
                            if (railNumber > 3)
                                break;
                            Note note = NoteManager.Instance.FindNote(bar, beat, railNumber);
                            if (note != null && note.GetType() == typeof(NormalNote))
                                NoteManager.Instance.RemoveNote(note);
                        }
                        break;
                    case EditType.PopNote:
                        {
                            if (railNumber > 3)
                                break;
                            Note note = NoteManager.Instance.FindNote(bar, beat, railNumber);
                            if (note != null && note.GetType() == typeof(PopNote))
                                NoteManager.Instance.RemoveNote(note);
                        }
                        break;
                    case EditType.MineNote:
                        {
                            if (railNumber > 3)
                                break;
                            Note note = NoteManager.Instance.FindNote(bar, beat, railNumber);
                            if (note != null && note.GetType() == typeof(MineNote))
                                NoteManager.Instance.RemoveNote(note);
                        }
                        break;
                    case EditType.LongNote:
                        {
                            if (railNumber > 3)
                                break;
                            Note note = NoteManager.Instance.FindLongTypeNote(bar, beat, railNumber, true);
                            if (note != null && note.GetType() == typeof(LongNote))
                                NoteManager.Instance.RemoveNote(note);
                        }
                        break;
                    case EditType.SpaceNote:
                        {
                            if (railNumber > 4)
                                break;
                            Note note = NoteManager.Instance.FindLongTypeNote(bar, beat, 4, true);
                            if (note == null)
                                note = NoteManager.Instance.FindNote(bar, beat, 4, true);

                            if (note != null && note.GetType() == typeof(SpaceNote))
                                NoteManager.Instance.RemoveNote(note);
                        }
                        break;
                    case EditType.EffectNote:
                        {
                            if (railNumber < 5 || railNumber > 6)
                                break;
                            Note note = NoteManager.Instance.FindLongTypeNote(bar, beat, railNumber, true);
                            if (note == null)
                                note = NoteManager.Instance.FindNote(bar, beat, railNumber, true);

                            if (note != null && note.GetType() == typeof(EffectNote))
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
                            if (railNumber != 8)
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
                    case EditType.TickNote:
                        {
                            if (railNumber != 7)
                                break;
                            Note note = NoteManager.Instance.FindEffectNote(bar, beat, typeof(TickNote));
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
            // int railNumber = (int)((-railLocalPos.y % railTotalHeight) / (NoteRail.RailHeight * 0.3f));
            int railNumber = NoteRailManager.Instance.RailYPosToRailNumber(-railLocalPos.y % railTotalHeight);

            switch (editType)
            {
                case EditType.NormalNote:
                    {
                        if (railNumber > 3)
                            break;
                        Note note = NoteManager.Instance.FindNote(bar, beat, railNumber);
                        if (note != null && note.GetType() == typeof(NormalNote))
                            NoteManager.Instance.RemoveNote(note);
                    }
                    break;
                case EditType.PopNote:
                    {
                        if (railNumber > 3)
                            break;
                        Note note = NoteManager.Instance.FindNote(bar, beat, railNumber);
                        if (note != null && note.GetType() == typeof(PopNote))
                            NoteManager.Instance.RemoveNote(note);
                    }
                    break;
                case EditType.MineNote:
                    {
                        if (railNumber > 3)
                            break;
                        Note note = NoteManager.Instance.FindNote(bar, beat, railNumber);
                        if (note != null && note.GetType() == typeof(MineNote))
                            NoteManager.Instance.RemoveNote(note);
                    }
                    break;
                case EditType.LongNote:
                    {
                        if (railNumber > 3)
                            break;
                        Note note = NoteManager.Instance.FindLongTypeNote(bar, beat, railNumber, true);
                        if (note != null && note.GetType() == typeof(LongNote))
                            NoteManager.Instance.RemoveNote(note);
                    }
                    break;
                case EditType.SpaceNote:
                    {
                        if (railNumber > 4)
                            break;
                        Note note = NoteManager.Instance.FindLongTypeNote(bar, beat, 4, true);
                        if (note == null)
                            note = NoteManager.Instance.FindNote(bar, beat, 4, true);

                        if (note != null && note.GetType() == typeof(SpaceNote))
                            NoteManager.Instance.RemoveNote(note);
                    }
                    break;
                case EditType.EffectNote:
                    {
                        if (railNumber < 5 || railNumber > 6)
                            break;
                        Note note = NoteManager.Instance.FindLongTypeNote(bar, beat, railNumber, true);
                        if (note == null)
                            note = NoteManager.Instance.FindNote(bar, beat, railNumber, true);

                        if (note != null && note.GetType() == typeof(EffectNote))
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
                        if (railNumber != 8)
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
                case EditType.TickNote:
                    {
                        if (railNumber != 7)
                            break;
                        Note note = NoteManager.Instance.FindEffectNote(bar, beat, typeof(TickNote));
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

        private void ShowLongTypePreview(float _barBeat, int _railNumber)
        {
            longTypePreview.SetActive(true);

            switch (editType)
            {
                case EditType.LongNote:
                    notePreviewRenderer.sprite = longNotePreviewSprite;
                    break;
                case EditType.EffectNote:
                    notePreviewRenderer.sprite = effectNotePreviewSprite;
                    break;
                case EditType.SpaceNote:
                    notePreviewRenderer.sprite = spaceNotePreviewSprite;
                    break;
                default:
                    break;
            }

            NoteRailLength rl = BeatManager.Instance.GetNoteRailLengthWithBarBeat(_barBeat);
            float localBarBeatDiff = _barBeat - rl.startBarBeat;
            float railTotalHeight = NoteRail.RailHeight + NoteRailManager.Instance.railSpacing;

            Vector3 pos = Vector3.zero;
            pos.x = localBarBeatDiff * NoteRail.RailOneBarWidth;
            // pos.y = (rl.railNumber - 1) * -railTotalHeight;
            // if (_railNumber == 0)
            //     pos.y += NoteRail.RailHeight * 0.333333f;
            pos.y = (rl.railNumber - 1) * -railTotalHeight + NoteRailManager.Instance.RailNumberToLineNoteYPos(_railNumber);
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



        // Developer HotKey Functions
        
        private void Update()
        {
            UpdateDeveloperHotKeyState();
        }

        private void UpdateDeveloperHotKeyState()
        {
            if (Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.RightShift) && Input.GetKeyDown(KeyCode.D))
            {
                MakerUIManager.Instance.developerMenu.SetActive(!MakerUIManager.Instance.developerMenu.activeSelf);
            }
            else if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            {
                if (Input.GetKey(KeyCode.D) && Input.GetMouseButtonDown(2))
                {
                    DeleteNoteNearestCursor();
                }
            }
        }

        private void DeleteNoteNearestCursor()
        {
            Vector3 cursorRailPos = NoteRailManager.Instance.railRoot.InverseTransformPoint(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            cursorRailPos.z = 0.0f;

            NoteRenderer target = null;
            float targetDis = 99999.0f;
            foreach (var ren in NoteManager.Instance.SpawnedRenderers)
            {
                Vector3 renPos = ren.transform.localPosition;
                renPos.z = 0.0f;

                float dis = Vector3.Distance(cursorRailPos, renPos);
                if (dis < targetDis)
                {
                    target = ren;
                    targetDis = dis;
                }
            }

            if (target != null)
            {
                NoteManager.Instance.RemoveNote(target.note);
            }
        }
    }
}