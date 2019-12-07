using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PopTheCircle.Game
{
    /// <summary>
    /// 사용자의 터치, 드래그 등 입력 관리를 담당한다.
    /// </summary>
    public class InputManager : Singleton<InputManager>
    {
        public const int InputPress     = 1;
        public const int InputStay      = 2;
        public const int InputRelease   = 3;

        [InspectorReadOnly]
        public List<TouchInfo> infos;



        protected override void Awake()
        {
            Input.multiTouchEnabled = true;
            infos = new List<TouchInfo>();
        }

        private void Update()
        {
            // 터치 기기의 경우. (Android, iOS 등)
            if (Input.touchSupported)
            {
                /*
                while (infos.Count != Input.touchCount)
                {
                    if (infos.Count > Input.touchCount)
                        infos.RemoveAt(infos.Count - 1);
                    else
                        infos.Add(new TouchInfo());
                }

                for (int i = 0; i < Input.touchCount; ++i)
                {
                    Touch t = Input.GetTouch(i);
                    TouchInfo info = infos[i];
                    info.lastPosition = info.position;
                    info.position = t.position;

                    if (t.phase == TouchPhase.Stationary || t.phase == TouchPhase.Moved)
                        info.state = InputStay;
                    else if (t.phase == TouchPhase.Began)
                    {
                        info.state = InputPress;
                        info.startPosition = t.position;
                        info.dragPosition = t.position;
                    }
                    else if (t.phase == TouchPhase.Ended)
                        info.state = InputRelease;

                    info.UpdateTouchDelta();
                }
                */
            }
            // 그 외의 경우. (에디터, 윈도우 등)
            else
            {
                if (Input.GetKeyDown(KeyCode.S))
                    JudgeManager.Instance.JudgeNoteAtLine(0, InputPress);
                else if (Input.GetKey(KeyCode.S))
                    JudgeManager.Instance.JudgeNoteAtLine(0, InputStay);

                if (Input.GetKeyDown(KeyCode.F))
                    JudgeManager.Instance.JudgeNoteAtLine(1, InputPress);
                else if (Input.GetKey(KeyCode.F))
                    JudgeManager.Instance.JudgeNoteAtLine(1, InputStay);

                if (Input.GetKeyDown(KeyCode.J))
                    JudgeManager.Instance.JudgeNoteAtLine(2, InputPress);
                else if (Input.GetKey(KeyCode.J))
                    JudgeManager.Instance.JudgeNoteAtLine(2, InputStay);

                if (Input.GetKeyDown(KeyCode.L))
                    JudgeManager.Instance.JudgeNoteAtLine(3, InputPress);
                else if (Input.GetKey(KeyCode.L))
                    JudgeManager.Instance.JudgeNoteAtLine(3, InputStay);
                
                if (Input.GetKeyDown(KeyCode.Space))
                    JudgeManager.Instance.JudgeNoteAtLine(4, InputPress);
                else if (Input.GetKey(KeyCode.Space))
                    JudgeManager.Instance.JudgeNoteAtLine(4, InputStay);

            }
        }
    }
}


/*
private void Update()
{
    // TEST
    if (Input.GetKeyDown(KeyCode.LeftArrow))
        JudgeManager.Instance.JudgeNoteAtLine(0, InputPress);
    else if (Input.GetKeyUp(KeyCode.LeftArrow))
        JudgeManager.Instance.JudgeNoteAtLine(0, InputRelease);
    else if (Input.GetKey(KeyCode.LeftArrow))
        JudgeManager.Instance.JudgeNoteAtLine(0, InputStay);
    if (Input.GetKeyDown(KeyCode.UpArrow))
    {
        JudgeManager.Instance.JudgeNoteAtLine(0, InputDrag);
        JudgeManager.Instance.JudgeNoteAtLine(0, InputPress);
    }

    if (Input.GetKeyDown(KeyCode.RightArrow))
        JudgeManager.Instance.JudgeNoteAtLine(1, InputPress);
    else if (Input.GetKeyUp(KeyCode.RightArrow))
        JudgeManager.Instance.JudgeNoteAtLine(1, InputRelease);
    else if (Input.GetKey(KeyCode.RightArrow))
        JudgeManager.Instance.JudgeNoteAtLine(1, InputStay);
    if (Input.GetKeyDown(KeyCode.DownArrow))
    {
        JudgeManager.Instance.JudgeNoteAtLine(1, InputDrag);
        JudgeManager.Instance.JudgeNoteAtLine(1, InputPress);
    }

}
*/
