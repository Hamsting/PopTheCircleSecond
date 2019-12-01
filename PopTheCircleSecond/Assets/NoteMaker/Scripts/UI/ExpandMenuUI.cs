using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PopTheCircle.NoteEditor
{
    public class ExpandMenuUI : MonoBehaviour
    {
        public List<Button> editModeButtons;
        public List<Button> editTypeButtons;
        public Text playSpeedText;
        public Text cameraZoomText;



        private void Awake()
        {
            ChangeEditMode("PositionBar");
            ChangeEditType("NormalNote");
        }

        public void ChangeEditMode(string _editModeStr)
        {
            MakerManager.Instance.editMode = (EditMode)Enum.Parse(typeof(EditMode), _editModeStr);
            foreach (Button btn in editModeButtons)
            {
                if (btn.gameObject.name.Equals(_editModeStr))
                    btn.interactable = false;
                else
                    btn.interactable = true;
            }
        }

        public void ChangeEditType(string _editTypeStr)
        {
            MakerManager.Instance.editType = (EditType)Enum.Parse(typeof(EditType), _editTypeStr);
            foreach (Button btn in editTypeButtons)
            {
                if (btn.gameObject.name.Equals(_editTypeStr))
                    btn.interactable = false;
                else
                    btn.interactable = true;
            }
        }

        public void ChangePlaySpeed(float _value)
        {
            MusicManager.Instance.MusicPlaySpeed = _value * 0.1f;
            playSpeedText.text = "Play Speed : " + MusicManager.Instance.MusicPlaySpeed.ToString("0.0") + "x";
        }

        public void ChangeCameraZoom(int _value)
        {
            NoteRailManager.Instance.CameraHeight += _value;
            cameraZoomText.text = "Camera Zoom : " + NoteRailManager.Instance.CameraHeight;
        }
    }
}