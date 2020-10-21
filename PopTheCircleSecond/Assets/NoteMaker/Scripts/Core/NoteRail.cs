using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PopTheCircle.NoteEditor
{
    [System.Serializable]
    public class NoteRail : MonoBehaviour
    {
        public const float RailLengthMul = 0.5f;
        public const float RailHeight = 3.60f;
        public const float RailOneBarWidth = 4.0f * RailLengthMul;

        public int RailNumber
        {
            get
            {
                return railNumber;
            }
            set
            {
                railNumber = value;
                UpdateRailNumber();
            }
        }
        public int StartBar
        {
            get
            {
                return startBar;
            }
            set
            {
                startBar = value;
            }
        }
        public float BarCount
        {
            get
            {
                return barCount;
            }
            set
            {
                barCount = value;
                UpdateBarCount();
            }
        }

        [SerializeField, InspectorReadOnly]
        private TextMesh railNumberText;
        [SerializeField, InspectorReadOnly]
        private Transform railImage;
        [SerializeField, InspectorReadOnly]
        private SpriteRenderer barGridRenderer;
        [SerializeField, InspectorReadOnly]
        private SpriteRenderer barRailRenderer;
        [SerializeField, InspectorReadOnly]
        private int railNumber = 0;
        [SerializeField, InspectorReadOnly]
        private int startBar = 0;
        [SerializeField, InspectorReadOnly]
        private float barCount = 4;



        private void Start()
        {
            UpdateRailNumber();
            UpdateBarCount();
        }

        private void UpdateRailNumber()
        {
            railNumberText.text = railNumber.ToString("D03");
        }

        private void UpdateBarCount()
        {
            Vector3 scale = railImage.localScale;
            scale.x = barCount * RailLengthMul;
            railImage.localScale = scale;

            Vector2 size = barGridRenderer.size;
            size.x = 4.0f * barCount;
            barGridRenderer.size = size;
            scale = barGridRenderer.transform.localScale;
            scale.x = RailLengthMul;
            barGridRenderer.transform.localScale = scale;

            size = barRailRenderer.size;
            size.x = 4.0f * barCount;
            barRailRenderer.size = size;
            scale = barRailRenderer.transform.localScale;
            scale.x = RailLengthMul;
            barRailRenderer.transform.localScale = scale;
        }

        public void UpdateBarGridSprite(Sprite _spr)
        {
            barGridRenderer.sprite = _spr;
        }
    }
}