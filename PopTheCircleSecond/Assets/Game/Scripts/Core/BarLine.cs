using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PopTheCircle.Game
{
    public class BarLine : MonoBehaviour
    {
        public int bar = 0;
        public int railNumber = 0;
        public double position = 0.0f;

        private Vector3 barLinePos = new Vector3();



        public void Initialize()
        {
            Vector3 scale = this.transform.localScale;
            scale.x = (railNumber == 4) ? 106.0f : 32.5f;
            this.transform.localScale = scale;
        }

        private void Update()
        {
            double positionDiff = position - BeatManager.Instance.Position;
            barLinePos.x = 0.0f;
            barLinePos.y = (float)positionDiff * BeatManager.Instance.GameSpeed;
            // barLinePos.z = (float)(positionDiff / GlobalDefines.RailLength) * -0.01f;
            
            this.transform.localPosition = barLinePos;
            this.transform.localRotation = Quaternion.identity;


            if (BeatManager.Instance.Bar >= bar)
                ObjectPoolManager.Instance.Free(this.gameObject);
        }
    }
}