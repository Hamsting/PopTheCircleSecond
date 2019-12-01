using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace PopTheCircle.NoteEditor
{
    public class InputManager : Singleton<InputManager>
    {
        private void Update()
        {
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                NoteRailManager.Instance.CurrentScroll -= Input.mouseScrollDelta.y;

                if (Input.GetMouseButtonDown(0))
                {
                    Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    MakerManager.Instance.LeftClickField(worldPos);
                }
                else if (Input.GetMouseButtonDown(1))
                {
                    Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    MakerManager.Instance.RightClickField(worldPos);
                }
                else if (Input.GetMouseButtonDown(2))
                {
                    Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    MakerManager.Instance.MiddleClickField(worldPos);
                }
            }

            if (Input.GetKeyDown(KeyCode.LeftBracket))
                NoteRailManager.Instance.CameraHeight += 0.5f;
            else if (Input.GetKeyDown(KeyCode.RightBracket))
                NoteRailManager.Instance.CameraHeight -= 0.5f;
        }
    }
}