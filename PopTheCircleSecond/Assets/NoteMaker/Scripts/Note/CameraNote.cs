using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace PopTheCircle.NoteEditor
{
    public enum CameraCurve
    {
        Constant,   // 상수형 : 즉시 해당 값으로 이동
        Linear,     // 직선형 : 일정한 속도로 해당 값으로 이동
        EaseIn,     // 감속형 : 서서히 속도가 감소하며 해당 값으로 이동
        EaseOut,    // 가속형 : 서서히 속도가 증가하며 해당 값으로 이동
        EaseInOut   // 혼합형 : 가속 및 감속을 하며 해당 값으로 이동
    }

    [System.Serializable]
    public class CameraNote : Note
    {
        /// <summary>
        /// 카메라 이동 방식
        /// </summary>
        public CameraCurve curve = CameraCurve.Linear;

        /// <summary>
        /// 이동 지속 시간
        /// </summary>
        public float duration = 1.0f;

        /// <summary>
        /// 위치 변화값 (현재 위치에 대한 상대값)
        /// </summary>

        public Vector2 positionChange = Vector2.zero;
        /// <summary>
        /// 각도 변화값
        /// </summary>
        public float rotationChange = 0.0f;

        /// <summary>
        /// 카메라 줌 변화값
        /// </summary>
        public float sizeChange = 5.0f;
    }

}