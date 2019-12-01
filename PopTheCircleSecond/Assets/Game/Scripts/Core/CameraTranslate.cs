using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PopTheCircle.Game
{
    public class CameraTranslate : MonoBehaviour
    {
        public enum EaseType
        {
            None = 0,
            EaseIn = 1,
            EaseOut = 2,
            EaseInOut = 3,
        }

        public AnimationCurve[] easeCurves;



        public void MoveToward(float _duration, Vector3 _value, EaseType _easeType = EaseType.None)
        {
            
        }

        public void MoveToPos(float _duration, Vector3 _pos, EaseType _easeType = EaseType.None)
        {

        }

        public void RotateToward(float _duration, float _value, EaseType _easeType = EaseType.None)
        {

        }

        public void RotateToAngle(float _duration, float _angle, EaseType _easeType = EaseType.None)
        {

        }

        public void SizeToward(float _duration, float _value, EaseType _easeType = EaseType.None)
        {

        }

        public void SizeToScale(float _duration, float scale, EaseType _easeType = EaseType.None)
        {

        }

        private IEnumerator MoveTowardCoroutine(float _duration, Vector3 _value, EaseType _easeType = EaseType.None)
        {
            float timer = 0.0f;

            yield return null;
        }
    }
}