using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TrainWorld
{
    public class UiDragableSiblings : UiDragableComponent
    {
        public Action onPointerUp;

        private bool isDragging = false;
        private Vector2 offset;

        void Update()
        {
            if (isDragging)
            {
                transform.position = new Vector2(Input.mousePosition.x + offset.x, Input.mousePosition.y + offset.y);
            }
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            offset = new Vector2((transform.position.x - Input.mousePosition.x), (transform.position.y - Input.mousePosition.y));
            isDragging = true;
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            base.OnPointerUp(eventData);
            isDragging = false;
            onPointerUp?.Invoke();
        }
    }
}
