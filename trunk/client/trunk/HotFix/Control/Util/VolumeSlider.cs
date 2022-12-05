using deepwaterooo.tetris3d;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace HotFix.Control {

    public class VolumeSlider : MonoBehaviour {

        public ExtendedSlider slider;
        // Start is called before the first frame update
        void Start()
            {
                slider.DragStart.AddListener(OnSliderBeginDrag);
                slider.DragStop.AddListener(OnSliderEndDrag);
                slider.PointerDown.AddListener(OnSliderClick);
            }

        void OnSliderBeginDrag(float value)
            {
                Debug.Log("开始拖拽：" + value);
            }

        void OnSliderClick(float value)
            {
                Debug.Log("点击：" + value);
            }

        void OnSliderEndDrag(float value)
            {
                Debug.Log("结束拖拽：" + value);
            }
    }

    
}
