using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PopTheCircle.Game
{
    public class TouchInfo
    {
        public Vector2 position = Vector2.zero;
        public Vector2 lastPosition = Vector2.zero;
        public Vector2 startPosition = Vector2.zero;
        public Vector2 dragPosition = Vector2.zero;
        public float touchDelta = 0f;
        public int state = 0;



        public void UpdateTouchDelta()
        {
            touchDelta = Mathf.Abs(Vector2.Distance(lastPosition, position));
        }

        public int CheckDrag()
        {
            if (Mathf.Abs(Vector2.Distance(dragPosition, position)) >= GlobalDefines.DragMinDistance &&
                touchDelta >= GlobalDefines.DragMinDelta)
            {
                if (dragPosition.x <= position.x)
                    return 1;
                else
                    return 0;
            }
            return -1;
        }

        public void ResetDrag()
        {
            dragPosition = position;
        }

        public int GetRailNumber()
        {
            if (startPosition.x < Screen.width / 2)
                return 0;
            else
                return 1;
        }
    }
}