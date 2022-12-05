using deepwaterooo.tetris3d.events;
using UnityEngine.EventSystems;

namespace deepwaterooo.tetris3d {

    public class ExtendedSlider : UnityEngine.UI.Slider, IBeginDragHandler, IEndDragHandler {
        // private const string TAG = "ExtendedSlider";

        public ExtendedEvent DragStart;
        public ExtendedEvent DragStop;
        public ExtendedEvent PointerDown;

        public void OnBeginDrag(PointerEventData eventData) {
            DragStart.Invoke(m_Value);
        }
        public void OnEndDrag(PointerEventData eventData) {
            DragStop.Invoke(m_Value);
        }
        public override void OnPointerDown(PointerEventData eventData) {
            base.OnPointerDown(eventData);
            PointerDown.Invoke(m_Value);
        }
    }
}
