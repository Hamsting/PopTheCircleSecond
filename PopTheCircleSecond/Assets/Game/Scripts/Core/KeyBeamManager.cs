using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PopTheCircle.Game
{
    public class KeyBeamManager : Singleton<KeyBeamManager>
    {
        public float beamStartAlpha = 160.0f / 255.0f;
        public float beamDuration = 0.3f;
        public Color[] beamColors;
        public SpriteRenderer[] beamRens;

        private float[] timers;
        private bool[] isKeyBeamStateUpdated;



        protected override void Awake()
        {
            base.Awake();

            isKeyBeamStateUpdated = new bool[7];
            timers = new float[7];
            for (int i = 0; i < 7; ++i)
            {
                SetKeyBeamState(i, KeyBeamState.Normal);
                timers[i] = 0.0f;
                isKeyBeamStateUpdated[i] = false;
            }
        }

        private void Update()
        {
            for (int i = 0; i < 7; ++i)
            {
                if (!isKeyBeamStateUpdated[i] && 
                   (InputManager.Instance.inputStates[i] == InputManager.InputPress))
                {
                    timers[i] = beamDuration;
                    SetKeyBeamState(i, KeyBeamState.Normal);
                }

                Color c = beamRens[i].color;
                c.a = beamStartAlpha * (timers[i] / beamDuration);
                beamRens[i].color = c;
                timers[i] = Mathf.Clamp(timers[i] - Time.deltaTime, 0.0f, beamDuration);
            }
        }

        private void LateUpdate()
        {
            for (int i = 0; i < 7; ++i)
            {
                isKeyBeamStateUpdated[i] = false;
            }
        }

        public enum KeyBeamState { Normal = 0, Perfect = 1, Great = 2 }
        public void SetKeyBeamState(int _railNumber, KeyBeamState _state)
        {
            beamRens[_railNumber].color = beamColors[(int)_state];
            isKeyBeamStateUpdated[_railNumber] = true;
            timers[_railNumber] = beamDuration;
        }
    }
}