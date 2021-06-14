#define USE_MAINKEYSET_6

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

        private KeyCode[] mainKeyCodes = new KeyCode[8]
        {
#if USE_MAINKEYSET_1
            KeyCode.S,
            KeyCode.F,
            KeyCode.J,
            KeyCode.L,
            KeyCode.Space,
            KeyCode.Space,
            KeyCode.LeftShift,
            KeyCode.RightShift,
#elif USE_MAINKEYSET_2
            KeyCode.S,
            KeyCode.D,
            KeyCode.L,
            KeyCode.Semicolon,
            KeyCode.Space,
            KeyCode.Space,
            KeyCode.A,
            KeyCode.Quote,
#elif USE_MAINKEYSET_3
            KeyCode.A,
            KeyCode.D,
            KeyCode.L,
            KeyCode.Quote,
            KeyCode.Space,
            KeyCode.Space,
            KeyCode.LeftShift,
            KeyCode.RightShift,
#elif USE_MAINKEYSET_4
            KeyCode.D,
            KeyCode.F,
            KeyCode.J,
            KeyCode.K,
            KeyCode.Space,
            KeyCode.Space,
            KeyCode.S,
            KeyCode.L,
#elif USE_MAINKEYSET_5
            KeyCode.S,
            KeyCode.D,
            KeyCode.L,
            KeyCode.Semicolon,
            KeyCode.Space,
            KeyCode.Space,
            KeyCode.A,
            KeyCode.Quote,
#elif USE_MAINKEYSET_6
            KeyCode.A,
            KeyCode.D,
            KeyCode.Keypad4,
            KeyCode.Keypad6,
            KeyCode.Space,
            KeyCode.Keypad0,
            KeyCode.CapsLock,
            KeyCode.KeypadEnter,
#endif
        };

        public bool isInputEnabled = true;
        [InspectorReadOnly]
        public List<TouchInfo> infos;
        public int[] inputStates;



        protected override void Awake()
        {
            Input.multiTouchEnabled = true;
            infos = new List<TouchInfo>();
            inputStates = new int[7];
        }

        private void Update()
        {
            if (!isInputEnabled)
                return;

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
                if (mainKeyCodes == null || mainKeyCodes.Length < 8)
                    return;

                if (Input.GetKeyDown(mainKeyCodes[0]))
                    InputRail(0, InputPress);
                else if (Input.GetKey(mainKeyCodes[0]))
                    InputRail(0, InputStay);

                if (Input.GetKeyDown(mainKeyCodes[1]))
                    InputRail(1, InputPress);
                else if (Input.GetKey(mainKeyCodes[1]))
                    InputRail(1, InputStay);

                if (Input.GetKeyDown(mainKeyCodes[2]))
                    InputRail(2, InputPress);
                else if (Input.GetKey(mainKeyCodes[2]))
                    InputRail(2, InputStay);

                if (Input.GetKeyDown(mainKeyCodes[3]))
                    InputRail(3, InputPress);
                else if (Input.GetKey(mainKeyCodes[3]))
                    InputRail(3, InputStay);
                
                if (Input.GetKeyDown(mainKeyCodes[4]) || Input.GetKeyDown(mainKeyCodes[5]))
                    InputRail(4, InputPress);
                else if (Input.GetKey(mainKeyCodes[4]) || Input.GetKey(mainKeyCodes[5]))
                    InputRail(4, InputStay);
                
                if (Input.GetKeyDown(mainKeyCodes[6]))
                    InputRail(5, InputPress);
                else if (Input.GetKey(mainKeyCodes[6]))
                    InputRail(5, InputStay);
                
                if (Input.GetKeyDown(mainKeyCodes[7]))
                    InputRail(6, InputPress);
                else if (Input.GetKey(mainKeyCodes[7]))
                    InputRail(6, InputStay);
            }
        }

        private void LateUpdate()
        {
            for (int i = 0; i < 7; ++i)
            {
                inputStates[i] = 0;
            }
        }

        private void InputRail(int _railNumber, int _state)
        {
            JudgeManager.Instance.JudgeNoteAtRail(_railNumber, _state);
            inputStates[_railNumber] = _state;
        }
    }
}
