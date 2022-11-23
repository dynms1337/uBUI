using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;
using UnityEngine.Events;
using System.Collections.Generic;
using System.Reflection;

namespace uBUI
{
    public class BContainer : SPanel
    {
        Canvas canvas;
        //SPanel container;
        RectTransform rtContainer;

        protected BContainer(LayoutGroup lg) :base(lg){
            rtContainer = this.goPanel.GetComponent<RectTransform>();
            canvas = this.goPanel.getParent().GetComponent<Canvas>();
        }

        /// <param name="renderMode">RenderMode.WorldSpace or RenderMode.ScreenSpaceOverlay</param>
        public static BContainer Create(RenderMode renderMode, string goCanvasName = "Canvas", Vector2? leftbottom = null, Vector2? windowSize = null,
             UIInfo uiInfo = null, bool draggable4screen = true,  // Canvas parameters
             LayoutType Layout = LayoutType.Vertical, // Panel parameters
             float canvasScale = 1f, GameObject parent = null)
        {
            //if (bgColor == null) bgColor = new Color(1f, 1f, 1f, 0.1f);
            if (uiInfo == null) uiInfo = UIInfo.CANVAS_DEFAULT;
            LayoutGroup lg = SWHelper.CreateCanvas(renderMode, leftbottom, windowSize,
                uiInfo: uiInfo, layoutGroup: LayoutType.Vertical,
                draggable4screen: draggable4screen, canvasScale: canvasScale, parent: parent, goName: goCanvasName);
            var ret = new BContainer(lg);
            return ret;
        }


        /// <summary>ScreenSpace only.</summary>
        public void locate_byPosition(float? left, float? bottom, float? width, float? height)
        {
            rtContainer.anchorMin = Vector2.zero;
            rtContainer.anchorMax = Vector2.zero;
            rtContainer.sizeDelta = new Vector2(width ?? rtContainer.sizeDelta.x, height ?? rtContainer.sizeDelta.y);
            rtContainer.position = new Vector2(left ?? rtContainer.position.x, bottom ?? rtContainer.position.y);
        }

        /// <summary>ScreenSpace only. Locate window by "percentage from screen circumference". </summary>
        public void locate_byMarginPct(float? left, float? right, float? top, float? bottom)
        {
            rtContainer.anchorMin = new Vector2(left ?? rtContainer.anchorMin.x, bottom ?? rtContainer.anchorMin.y);
            rtContainer.anchorMax = new Vector2((1 - right) ?? rtContainer.anchorMax.x, (1 - top) ?? rtContainer.anchorMax.y);
            rtContainer.offsetMin = Vector2.zero; 
            rtContainer.offsetMax = Vector2.zero;
        }

        /// <summary>ScreenSpace only. Locate window by "px from screen circumference". </summary>
        public void locate_byMarginPx(float? left, float? right, float? top, float? bottom)
        {
            rtContainer.anchorMin = Vector2.zero; 
            rtContainer.anchorMax = Vector2.one;
            rtContainer.offsetMin = new Vector2(left ?? rtContainer.offsetMin.x, bottom ?? rtContainer.offsetMin.y);
            rtContainer.offsetMax = new Vector2((-right) ?? rtContainer.offsetMax.x, (-top) ?? rtContainer.offsetMin.y);
        }

    }
}
