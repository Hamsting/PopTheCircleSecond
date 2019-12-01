using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PopTheCircle.NoteEditor
{
    public class NoteRail : MonoBehaviour
    {
        public const float RailHeight = 2.41f;
        public const float RailOneBarWidth = 4.0f;

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

        [SerializeField]
        private TextMesh railNumberText;
        [SerializeField]
        private Transform railImage;
        [SerializeField]
        private SpriteRenderer barGridRenderer;
        [SerializeField]
        private SpriteRenderer barRailRenderer;
        private int railNumber = 0;
        private int startBar = 0;
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
            scale.x = barCount;
            railImage.localScale = scale;

            Vector2 size = barGridRenderer.size;
            size.x = 4.0f * barCount;
            barGridRenderer.size = size;

            size = barRailRenderer.size;
            size.x = 4.0f * barCount;
            barRailRenderer.size = size;
        }

        public void UpdateBarGridSprite(Sprite _spr)
        {
            barGridRenderer.sprite = _spr;
        }
    }
}