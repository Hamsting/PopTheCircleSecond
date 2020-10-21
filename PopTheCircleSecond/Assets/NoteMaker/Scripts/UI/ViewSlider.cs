using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace PopTheCircle.NoteEditor
{
    public class ViewSlider : MonoBehaviour, IPointerUpHandler
    {
        public float scrollSpeed = 3.0f;
        public Slider slider;

        private float dir = 0.0f;



        private void Update()
        {
            NoteRailManager.Instance.CurrentScroll += dir * scrollSpeed * Time.deltaTime;
        }

        public void OnValueChanged(float _value)
        {
            dir = _value;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            slider.value = 0.0f;
        }
    }
}